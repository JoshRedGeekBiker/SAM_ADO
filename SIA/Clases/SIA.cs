using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SIA : ISistema, IBDContext, IGPS, IBBContextSIA
{
    #region "Propiedades"

    #endregion

    #region "Propiedades Heredadas"
    public int PuertoSocket { get; set; }
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }

    public Sistema Sistema { get { return Sistema.SIA; } }

    public bool ModoPrueba { get; set; }
    public string GetVersionSistema { get; }//FaltaVersión de SIA

    public vmdEntities VMD_BD { get; }
    public bool ModoNocturno { get; set; }

    //Parametros recibidos de SIA
    public int calidadInternet { get; set; } = 0;
    public bool internetEncendido { get; set; } = false;
    public bool finCircuito { get; set; } = true;

    //MINISIA CLAUS && ROJO
    public GPSData Datos_GPS { get; set; }

    //Powered ByRED 13ABR2021
    public SIAEntities SIA_BD { get; }
    #endregion

    #region "Variables"
    private IPEndPoint Dir;
    private Socket SIASocket;


    private NetworkStream StreamServidor;
    private TcpClient cliente;

    

    //Hilos
    private Thread hiloActualizar;
    private Thread hiloProcesamiento;
    private Thread hiloComunicacion;
    private Thread hiloPuntosMiniSIA; //CLAUS && ROJO
    private Thread hiloPuntosInteres; //Powered ByRED 30MAR2021

    //Lógicas
    private Mensajes _Mensajes;
    private PuntosInteres _POI;//Powered ByRED 23MAR2021

    //DLLMINI SIA Claus & ROJO
    private DLL_Mini_SIA.MiniSIA.SIAVMD MiniSIADLL;
    private bool MiniSIA = false;

    
    #endregion

    #region "Variables de Evento
    /// <summary>
    /// Se encarga de recuperar los parametros de los sistemas
    /// para ser enviados através del socket
    /// </summary>
    public delegate void RecuperaParametrosSIA();
    public event RecuperaParametrosSIA RecuperaParametros;

    /// <summary>
    /// Se encarga de mandarle el mensaje a SAM
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tipo"></param>
    /// <param name="texto"></param>
    public delegate bool EnviarMensaje(int tipo, string texto);
    public event EnviarMensaje MandarMensajeSAM;

    /// <summary>
    /// Se encarga de mandar el estado del internet
    /// 0: Sin señal de internet
    /// 1: Señal de internet limitada
    /// 2: señal de internet buena
    /// </summary>
    /// <param name="status"></param>
    public delegate void EnviarStatusInternet(int status);
    public event EnviarStatusInternet statusInternet;

    /// <summary>
    /// Se encargará de pedir a SAM los datos de GPS
    /// CLAUS && ROJO
    /// </summary>
    /// <returns></returns>
    public delegate GPSData TraerGPS();
    public event TraerGPS GPS;

    /// <summary>
    /// Se encarga de enviar a SAM el POI de SIA
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <returns></returns>
    public delegate bool POIASAM(List<string> Multimedia);
    public event POIASAM POI_SAM;

    /// <summary>
    /// Se encarga de mendar mensajes de sincronización a SAM
    /// CLAUS && ROJO
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="final"></param>
    public delegate void dMensajeSincronizacion(string mensaje, int final);
    public event dMensajeSincronizacion MsjSync;

    #endregion

    #region "Constructores
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public SIA(bool _MiniSIA = false)
    {
        //CLAUS & ROJO
        this.MiniSIA = _MiniSIA;

        MiniSIADLL = new DLL_Mini_SIA.MiniSIA.SIA();//CLAUS && ROJO

        hiloComunicacion = new Thread(new ThreadStart(ComunicacionSIA));
        hiloProcesamiento = new Thread(new ThreadStart(ProcesaMensajesSIA));
        hiloActualizar = new Thread(new ThreadStart(ActualizarSIA));

        hiloPuntosMiniSIA = new Thread(new ThreadStart(DLLPuntosInteres)); //CLAUS && ROJO

        hiloPuntosInteres = new Thread(new ThreadStart(ProcesaPuntosInteres)); //Powered ByRED 30MAR2021

        SIA_BD = new SIAEntities();//Powered ByRED 13ABR2021
    }
    #endregion

    #region "Métodos Publicos"

    /// <summary>
    /// Se encarga de enviarle mensajes a SIA a través del socket
    /// </summary>
    public void EscribirSocket(string mensaje)
    {
        try
        {
            if (cliente.Connected)
            {
                Byte[] datos = Encoding.ASCII.GetBytes(mensaje);

                StreamServidor.Write(datos, 0, datos.Length);
                StreamServidor.Flush();

                Thread.Sleep(500);
            }
        }
        catch (Exception ex)
        {
            var mensaje2 = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de recuperar los mensajes según su tipo
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    public List<string> ObtenerMensajesSIA(int tipo)
    {

        var listaRetorno = new List<string>();
        switch (tipo)
        {
            case 1:
                listaRetorno = _Mensajes.ObtenerMensajesRecibidos();
                break;

            case 2:
                listaRetorno = _Mensajes.ObtenerMensajesEnviados();
                break;

            default:
                listaRetorno = _Mensajes.ObtenerMensajesPred();
                break;
        }

        return listaRetorno;
    }

    /// <summary>
    /// manda un mensaje através del satelite de SIA
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="msj"></param>
    /// <returns></returns>
    public bool EnviarMensajeSIA(string _msj)
    {
        return _Mensajes.EnviarMensajeaBD(_msj);
    }

    /// <summary>
    /// Se encarga de generar un registro de Alerta de Robo
    /// Powered ByRED 08JUN2021
    /// </summary>
    public void AlertaRobo()
    {
        bool estado = false;
        while (estado == false)
        {
            estado = _Mensajes.GenerarAlertaRobo();
        }
    }

    #endregion

    #region "Métodos Privados"

    /// <summary>
    /// Se encarga de enviar y rebibir información
    /// através del socket
    /// </summary>
    private void ComunicacionSIA()
    {
        if (cliente == null)
        {
            ConectarSocket();
        }
        else
        {
            //Recibimos Mensajes
            EscucharSocket();

            //Enviamos Mensajes
            RecuperaParametros();

            //Lo vaciamos
            StreamServidor = null;

            //Mandamos Mensajes;
            //EscribirSocket("{VMD:System.String:VMD~sistema:System.String:VMD~parametro:System.String:titulo_pelicula~valor:System.String:Prueba~prioridad:System.String:2~TipoDato:System.String:String}");


        }
    }

    /// <summary>
    /// Método principal para el procesamiento de mensaje SIA
    /// </summary>
    private void ProcesaMensajesSIA()
    {
        try
        {
            //Verificamos Mensajes
            _Mensajes.ProcesarMensajes();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de actualizar la información necesaria del sistema SIA
    /// </summary>
    private void ActualizarSIA()
    {
        //mandamos información del estatus de internet al SAM/Front
        statusInternet(calidadInternet);
    }

    /// <summary>
    /// Se encarga de establecer la comunicación con el socket
    /// </summary>
    private void ConectarSocket()
    {
        try
        {
            //SIASocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Dir = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PuertoSocket);

            //SIASocket.Connect(Dir);

            if(cliente == null)
            {
                cliente = new TcpClient("127.0.0.1", PuertoSocket);
                StreamServidor = cliente.GetStream();
            }
        }
        catch
        {
            cliente = null;//Powered ByRED 20ABR2021   
        }
    }

    /// <summary>
    /// Se encarga de recibir los datos del socket
    /// </summary>
    private void EscucharSocket()
    {
        try
        {
            if (cliente.Connected)
            {
                StreamServidor = cliente.GetStream();

                Byte[] bytes = new byte[1024];

                var tamaño = StreamServidor.Read(bytes, 0, bytes.Length);

                if (tamaño != 0)
                {
                    //Procesamos el mensaje para quedarnos con el mensaje Real
                    char[] separators = { '{', '}' };
                    var mensaje = Encoding.ASCII.GetString(bytes, 0, tamaño).Split(separators);
                    var mensajeReal = mensaje[1];

                    //Recuperamos el item que nos manda
                    char[] separators1 = { '~' };
                    var mensaje2 = mensajeReal.Split(separators1);
                    var item = mensaje2[1];

                    //Recupermos la información
                    char[] separators2 = { ':' };
                    var items = item.Split(separators2);

                    //Validamos que tipo de mensaje nos envia, para hacer una acción diferente
                    switch (items[0])
                    {
                        case "calidadinternet":
                            calidadInternet = Convert.ToInt32(items[2]);
                            break;

                        case "red":
                            internetEncendido = Convert.ToBoolean(item[2]);
                            break;

                        case "FinCircuito":
                            finCircuito = Convert.ToBoolean(item[2]);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        catch(Exception ex)
        {
            var error1 = ex.ToString();

            calidadInternet = 0;
            internetEncendido = false;
            finCircuito = false;
        }
        
    }

    /// <summary>
    /// Se encarga de ejecutar el engine de la DLL MINISIA
    /// CLAUS && ROJO
    /// </summary>
    private void DLLPuntosInteres()
    {
        try
        {
            //Mando a pedir los datos de GPS
            this.Datos_GPS = GPS();

            if (this.Datos_GPS != null)
            {
                this.MiniSIADLL.Verificar_PuntoInteres(Convert.ToDouble(this.Datos_GPS.Latitud),
                    Convert.ToDouble(this.Datos_GPS.Longitud),
                    this.Datos_GPS.LatitudNS,
                    this.Datos_GPS.LongitudWE,
                    this.Datos_GPS.Velocidad,
                    this.Datos_GPS.Sentido);
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de procesar los puntos de interes
    /// </summary>
    private void ProcesaPuntosInteres()
    {
        _POI.ProcesarPOI();
    }

    /// <summary>
    /// Se encarga de mandar el mensaje a SAM
    /// Powered ByRED 23FEB2021
    /// </summary>
    private bool MensajeSAM(int _tipo, string _texto)
    {
        return MandarMensajeSAM(_tipo, _texto);
    }

    /// <summary>
    /// Se encarga de enviar los datos del poi a SAM
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <param name="_Multimedia"></param>
    /// <returns></returns>
    private bool POISAM(List<string> _Multimedia)
    {
        return POI_SAM(_Multimedia);
    }

    /// <summary>
    /// Recibe el Idpunto para mandarlo a DLL MINISIA
    /// CLAUS && ROJO
    /// </summary>
    /// <param name="_idpunto"></param>
    /// <returns></returns>
    private void ReportarPuntoMiniSIA(int _idpunto)
    {
        try
        {
            if (this.MiniSIA)
            {
                MiniSIADLL.Testigo_Punto_Interes(_idpunto);
            }
        }
        catch
        {

        }
    }

    /// <summary>
    ///Se encarga de detonar el cintillo con el mensaje inicial
    ///Powered ByRED 14ABR2021
    /// </summary>
    public void MostrarCintilloInicial()
    {
        try
        {
            if (_Mensajes != null)
            {
                _Mensajes.MostrarCintilliInicial();
            }
        }
        catch
        {

        }
    }

    #endregion

    #region "Métodos Heredados
    public void Actualizar()
    {
        if (!MiniSIA)//CLAUS & ROJO
        {
            if (!hiloComunicacion.IsAlive)
            {
                hiloComunicacion = new Thread(new ThreadStart(ComunicacionSIA));
                hiloComunicacion.Start();
            }

            if (!hiloActualizar.IsAlive)
            {
                hiloActualizar = new Thread(new ThreadStart(ActualizarSIA));
                hiloActualizar.Start();
            }

            if (!hiloProcesamiento.IsAlive)
            {
                hiloProcesamiento = new Thread(new ThreadStart(ProcesaMensajesSIA));
                hiloProcesamiento.Start();
            }
        }
        else
        {
            //Vamos a ejecutar el dllpuntos
            if (!hiloPuntosMiniSIA.IsAlive)
            {
                hiloPuntosMiniSIA = new Thread(new ThreadStart(DLLPuntosInteres));
                hiloPuntosMiniSIA.Start();
            }
        }

        //Vamos a ejecutar puntos interes
        //Powered ByRED 30MAR2021
        if (!hiloPuntosInteres.IsAlive)
        {
            hiloPuntosInteres = new Thread(new ThreadStart(ProcesaPuntosInteres));
            hiloPuntosInteres.Start();
        }
    }

    public string Eventos()
    {
        throw new NotImplementedException();
    }

    public void Finalizar()
    {
        
    }

    public void Inicializar()
    {
        //Claus && ROJO
        if (!this.MiniSIA)
        {
            //Establecemos comunicación con el satélite
            ConectarSocket();
        }
        
        //Cargamos la lógica de Mensajes
        _Mensajes = new Mensajes();
        _Mensajes.MandarMensaje += this.MensajeSAM;

        //Powered ByRED 23MAR2021
        _POI = new PuntosInteres();
        _POI.POI += this.POISAM;
        _POI.MiniSIA += this.ReportarPuntoMiniSIA;//CLAUS && ROJO

    }

    /// <summary>
    /// Para DLL MINISIA sincronizar
    /// CLAUS & ROJO
    /// </summary>
    /// <returns></returns>
    public bool Sincronizar()
    {
        try
        {

            var respuesta = MiniSIADLL.Actualizar();

            switch (respuesta)
            {
                case 0:
                    MsjSync("No se encontraron actualizaciones pendientes", 0);
                    return true;

                case -1:
                    MsjSync(MiniSIADLL.Ultimo_Log_SIA, 0);
                    return false;

                default:
                    MsjSync(MiniSIADLL.Ultimo_Log_SIA, 0);
                    return true;
            }       

        }
        catch
        {
            return false;
        }
    }
    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"
    
    #endregion
}
