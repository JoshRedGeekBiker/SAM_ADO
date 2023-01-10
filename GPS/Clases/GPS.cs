using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.Threading;

public class GPS : ISistema, IBDContext, IGPS
{

    #region  "Propiedades"
    public DateTime FechaInicioSistema { get; set; }
    public bool ViajeAbierto { get; set; }
    #endregion

    #region "Propiedades Heredadas"
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }
    public Sistema Sistema { get { return Sistema.GPS; } }
    public string GetVersionSistema { get; }
    public vmdEntities VMD_BD { get; }
    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    public GPSData Datos_GPS { get; set; }
    #endregion

    #region "Variables"

    //Para conectarnos con la DLL Cliente del socket para ADO GPS
    public ADO_GPSCliente.Cliente ADOGPS;

    //Variables privadas
    private can_parametrosinicio Parametros_inicio;
    private clsTerminal Terminal;

    private string IPSocket = "127.0.0.1";
    private string PuertoSocket = "3600";
    private bool SocketIniciado = false;


    private int LimpiezaTerminal = 0;
    private int MinsLimpieza = 20;

    //Numero máximo de veces que el GPS se debe de reiniciar
    private int ReiniciosGPS = 1;


    private int NumLecGPSigual = 0;
    private DateTime UltiFechaGPS = DateTime.ParseExact("1993-10-30 00:15", "yyyy-MM-dd HH:mm", null); //FechaRED =)
    private DateTime UltiFechaGPS2 = DateTime.ParseExact("1993-10-30 00:15", "yyyy-MM-dd HH:mm", null); //FechaRED =)
    public bool GPSActivo = false;
    private int EstadoGpsAnt = -1;
    private bool EntroTerminal = false;
    private DateTime TiempoTerminal = DateTime.ParseExact("1993-10-30 00:15", "yyyy-MM-dd HH:mm", null); //FechaRED =)
    private string UltLat = string.Empty;
    private string UltLong = string.Empty;
    private double TachoSpeedbyCAN = 0.0;


    //Varibales Publicas
    public can_poblaciones PobActual;

    /// Timers
    private System.Windows.Forms.Timer tmrLimpiaTerminal = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer tmrConectarSocket = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer tmrRevisaTerminalReinicio = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer tmrRevisaGPS = new System.Windows.Forms.Timer();

    //Hilos
    private Thread hiloActualizar;
    private Thread hiloConectar;

    #endregion

    #region "Variables de Evento"

    //Actualiza el color del indicador GPS
    public delegate void ActualizaIndicador(int estado);
    public event ActualizaIndicador ActualizarIndicador;

    //Manda a reiniciar el equipo por que entró en terminal y superó el tiempo de espera
    public delegate void ReiniciarEquipo(bool pasajero, string mensaje);
    public event ReiniciarEquipo RebootByGPS;

    //Manda los datos de GPS hacia CAN
    public delegate void DatosACAN(GPSData _DatosGPS);
    public event DatosACAN MandarASAM;

    public delegate void ReiniciarAPP();
    public event ReiniciarAPP RebootGPS;

    #endregion

    #region "Constructor"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public GPS()
    {

        VMD_BD = new vmdEntities();
        Parametros_inicio = (from x in VMD_BD.can_parametrosinicio
                             select x).FirstOrDefault();

        Terminal = new clsTerminal();

        //Mandamos a crear el objeto del Socket
        CrearGPS();

        //hiloActualizar = new Thread(new ThreadStart(ActualizaGPS));
        //hiloConectar = new Thread(new ThreadStart(ConectarSocket));

    }
    #endregion

    #region "Métodos Publicos"

    #endregion

    #region "Métodos Privados"

    private void CrearGPS()
    {
        SocketIniciado = false;

        ADOGPS = new ADO_GPSCliente.Cliente();

        //Asignamos los eventos del ObjetoSocket
        ADOGPS.LecturaGPS += ADOGPS_Lectura;

        ADOGPS.errorLectura += ADOGPS_ERROR;


        Datos_GPS = new GPSData();
        //Verifico la conexión
        //tmrConectarSocket.Start();
    }

    private void DestruirGPS()
    {
        SocketIniciado = false;
        ADOGPS.desconectar();
        ADOGPS.Dispose();
    }


    /// <summary>
    /// Se encarga de ejecutar el método para establecer la conexión con el 
    /// socket de ADOGPS
    /// </summary>
    private void ConectarSocket()
    {
        var resultado = ADOGPS.conectar(IPSocket, PuertoSocket, "VMD");

        if (!resultado.Contains("Conectado"))
        {

            SocketIniciado = false;
        }
        else
        {
            SocketIniciado = true;
        }
    }

    /// <summary>
    /// Se encarga de configurar los timers
    /// </summary>
    private void PrepararTimers()
    {
        //Configuramos Hilos
        hiloActualizar = new Thread(new ThreadStart(ActualizaGPS));
        hiloConectar = new Thread(new ThreadStart(ConectarSocket));

        tmrLimpiaTerminal.Interval = 60000; //Valor rescatado de VMD Touch
        tmrLimpiaTerminal.Enabled = true;
        tmrLimpiaTerminal.Tick += new EventHandler(timerLimpiaTerminal_Tick);

        tmrConectarSocket.Interval = 1000;
        tmrConectarSocket.Enabled = true;
        tmrConectarSocket.Tick += new EventHandler(timerConectarSocket_Tick);

        tmrRevisaTerminalReinicio.Interval = 5000;
        tmrRevisaTerminalReinicio.Enabled = true;
        tmrRevisaTerminalReinicio.Tick += new EventHandler(tmrRevisaTerminalReinicio_Tick);

        tmrRevisaGPS.Interval = 1000;
        tmrRevisaGPS.Enabled = false;
        tmrRevisaGPS.Tick += new EventHandler(tmrRevisaGPS_Tick);
    }

    /// <summary>
    /// Se encarga de Verificar si por las coordenadas GPS
    /// nos encontramos en alguna Terminal
    /// </summary>
    /// <param name="latitud"></param>
    /// <param name="longitud"></param>
    private void UbicaTerminales(string latitud, string longitud)
    {
        try
        {
            if (latitud.Length <= 0 || longitud.Length <= 0) return;

            //Busco en la base de datos esa latitud y longitud, para obtener la terminal, la zona y ruta de descarga

            var esLatmas = Escanea(latitud, '+');
            var esLatmenos = Escanea(latitud, '-');
            var esLonmas = Escanea(longitud, '+');
            var esLonmenos = Escanea(longitud, '-');

            var terminal_Actual = (from x in VMD_BD.can_terminales
                                   join zona_descarga in VMD_BD.can_zonasdescarga on x.IdZonaDescarga equals zona_descarga.Id
                                   where x.Lat <= esLatmas && x.Lat >= esLatmenos &&
                                   x.Lon <= esLonmas && x.Lon >= esLonmenos
                                   orderby x.Id, zona_descarga.Id
                                   select new { idterminal = x.Id, terminal = x.Descripcion, idzonadescarga = zona_descarga.Id, zonadescarga = zona_descarga.Descripcion, zona_descarga.RutaDescarga }).FirstOrDefault();

            if (terminal_Actual != null)
            {
                //Si se encuentra una terminal, entonces grabo en memoria que terminales, y cuál es la ruta de descarga 
                //(según la zona de descarga)

                Terminal.Encontrado = true;
                Terminal.DescripcionTerminal = terminal_Actual.terminal;
                Terminal.DescipcionZona = terminal_Actual.zonadescarga;
                Terminal.IdTerminal = Convert.ToInt32(terminal_Actual.idterminal);
                Terminal.IdZonaDescarga = terminal_Actual.idzonadescarga.ToString();
                Terminal.RutaDescarga = terminal_Actual.RutaDescarga;

                //Activo el tiempo para la limpieza de los datos de la terminal encontrada
                tmrLimpiaTerminal.Start();

            }
            else
            {
                //Chance podemos eliminar algo por acá
            }
        }
        catch (Exception ex)
        {
            var exce = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de buscar en BD si estamos en alguna población actual
    /// 
    /// Checar si no para quitarlo
    /// 
    /// </summary>
    /// <param name="latitud"></param>
    /// <param name="longitud"></param>
    private void UbicaPoblacion(string latitud, string longitud)
    {

        double LatitudDec = CoordenadaToDecimal(Convert.ToDouble(latitud));

        double LongitudDec = CoordenadaToDecimal(Convert.ToDouble(longitud));

        this.PobActual = (from x in VMD_BD.can_poblaciones
                          where x.Nuevo == true && (x.LatMin >= LatitudDec && x.LatMax <= LatitudDec) &&
                          (x.LonMin <= LongitudDec && x.LonMax >= LongitudDec)
                          select x).FirstOrDefault();

    }

    /// <summary>
    /// Se encarga de Traer el margen superior
    /// o inferior de la coordenada GPS
    /// </summary>
    /// <param name="LatLon"></param>
    /// <param name="Caso"></param>
    /// <returns></returns>
    private int Escanea(string LatLon, char Caso)
    {
        int MargenGPS = Convert.ToInt32(Parametros_inicio.MargenGPS);
        int multiplo = 1000;

        int nLation = Convert.ToInt32((Convert.ToDouble(LatLon) * multiplo));

        switch (Caso)
        {
            case '+':
                return ((nLation + MargenGPS) / multiplo);

            case '-':
                return ((nLation - MargenGPS) / multiplo);

            default:
                return 0;
        }

    }

    /// <summary>
    /// Se encarga de manejar el indicador del gps que se muestra
    /// en el form de sistemas de acuerdo a las lecturas del GPS
    /// </summary>
    private void SemaforoGPS()
    {
        try
        {

            if (EstadoGpsAnt != Datos_GPS.EstadoGPS)
            {
                EstadoGpsAnt = Datos_GPS.EstadoGPS;

                switch (Datos_GPS.EstadoGPS)
                {
                    case 2:
                        GPSActivo = true;
                        break;

                    default:
                        GPSActivo = false;
                        break;

                }
                ActualizarIndicador(Datos_GPS.EstadoGPS);
            }
        }
        catch
        {

        }
        //try
        //{
        //    if ((NumLecGPSigual >= 100 || NumLecGPSigual == 0) || ((DateTime.Now - UltiFechaGPS).TotalSeconds > 100))
        //    {//Significa que no hay gps o que los datos se quedaron pegados

        //        if (this.GPSActivo != false) //Evitará lanzar el evento si ya se encontraba pintado
        //        {
        //            this.GPSActivo = false;
        //            //Lanzamos el evento para pintar en rojo
        //            ActualizarIndicador(false);
        //        }
        //    }
        //    else//Significa que si hay movimiento de GPS
        //    {
        //        if( this.GPSActivo != true) //Evitará lanzar el evento si ya se encontraba pintado
        //        {
        //            this.GPSActivo = true;
        //            //Lanzamos el evento para pintar en verde
        //            ActualizarIndicador(true);
        //        }
        //    }
        //}
        //catch
        //{

        //}
    }

    //éste método tiene que ir en lógica de VMD
    private void UbicacionGPS(string latitud, string longitud)
    {
        try
        {
            if ((Datos_GPS.Latitud.Length <= 0) || (Datos_GPS.Longitud.Length <= 0)) return;

            //Busco la base de datos esa latitu y longitud, para obtener la poblacion



        }
        catch
        {

        }

    }

    /// <summary>
    /// Se encarga de verificar a cada momento si se encuentra en terminal
    /// para programar un reincio controlado
    /// </summary>
    /// <param name="latitud"></param>
    /// <param name="LatitudNS"></param>
    /// <param name="longitud"></param>
    /// <param name="LongitudWE"></param>
    private void UbicaPoblacionReinicio(double latitud, string LatitudNS, double longitud, string LongitudWE)
    {
        try
        {
            //Descartamos con varias reglas de negocio

            if (!Convert.ToBoolean(Parametros_inicio.ReinicioTerminal)) return;

            if ((DateTime.Now - this.FechaInicioSistema).TotalHours < Parametros_inicio.HrsReinicioTerminal) return;

            if (latitud.ToString().Length <= 0 || longitud.ToString().Length <= 0) return;

            if (!ViajeAbierto) return;

            if (!LatitudNS.Equals("N")) latitud = latitud * (-1);

            if (!LongitudWE.Equals("E")) longitud = longitud * (-1);

            double LatitudDec = CoordenadaToDecimal(latitud);

            double LongitudDec = CoordenadaToDecimal(longitud);

            can_poblaciones poblacion = (from x in VMD_BD.can_poblaciones
                                         where x.IDTIPOPUNTO != 0 && x.Nuevo == true && (x.LatMin >= LatitudDec && x.LatMax <= LatitudDec) &&
                                         (x.LonMin <= LongitudDec && x.LonMax >= LongitudDec)
                                         select x).FirstOrDefault();

            if (poblacion != null) //Si se encuentra en una terminal, entonces:
            {
                if (!this.EntroTerminal)// si se detectó la terminal por primera vez
                {
                    TiempoTerminal = DateTime.Now;//grabamos la hora actual
                    this.EntroTerminal = true;//indicamos que ya se detectó que se encuentra en una terminal
                }


                if ((DateTime.Now - this.TiempoTerminal).TotalMinutes > Parametros_inicio.ReinicioTiempoTerminal) //Si aun no pasan N minutos en la terminal se sale de la funcion
                {
                    //Lanzar el evento de reiniciar el equipo, que ira en SAM
                    RebootByGPS(false, "Arribo a Terminal/Taller \nPor Operacion el equpo se reiniciariá automaticamente \nEspere por favor");
                }

            }
            else
            {
                this.EntroTerminal = false;
            }

        }
        catch
        {

        }
    }

    /// <summary>
    /// Sirve para convertir las coordenadas hacia Decimal
    /// </summary>
    /// <param name="coordenada"></param>
    /// <returns></returns>
    private double CoordenadaToDecimal(double coordenada)
    {
        bool negativo = false;


        if (coordenada < 0)
        {
            negativo = true;
            coordenada = coordenada * (-1);
        }
        else
        {
            negativo = false;
        }

        double grados = Convert.ToDouble(coordenada.ToString().Substring(0, 2));
        double minutos = (Convert.ToDouble(coordenada.ToString().Substring(2)) / 60);
        double coordenadaFinal = grados + minutos;

        if (negativo)
        {
            coordenadaFinal = coordenadaFinal * (-1);
        }

        return coordenadaFinal;

    }

    /// <summary>
    /// Método que recibe los datos que manda ADO_GPSCliente
    /// </summary>
    /// <param name="Latitud"></param>
    /// <param name="NE"></param>
    /// <param name="longitud"></param>
    /// <param name="WE"></param>
    /// <param name="Altitud"></param>
    /// <param name="Satelites"></param>
    /// <param name="Velocidad"></param>
    /// <param name="Precision"></param>
    /// <param name="sentido"></param>
    /// <param name="HoraUTC"></param>
    private void ADOGPS_Lectura(string Latitud, string NE, string longitud, string WE, string Altitud, int Satelites, string Velocidad, double Precision, string sentido, string HoraUTC, int EstadoGPS)
    {
        try
        {
            if (ADOGPS != null)
            {
                Datos_GPS.Latitud = Latitud;
                Datos_GPS.LatitudNS = NE;
                Datos_GPS.Longitud = longitud;
                Datos_GPS.LongitudWE = WE;
                Datos_GPS.Altitud = Altitud;
                Datos_GPS.Satelites = Satelites.ToString();
                Datos_GPS.Velocidad = Convert.ToDouble(Velocidad);
                Datos_GPS.Precision = Precision.ToString();
                Datos_GPS.Sentido = sentido;
                Datos_GPS.EstadoGPS = EstadoGPS;

                UltiFechaGPS2 = DateTime.Now;

                ReiniciosGPS = 1;


            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// Se encarga de cachar el evento de error del Cliente de ADOGPS
    /// para poder avisar al sistema que se cayó el GPS
    /// </summary>
    private void ADOGPS_ERROR()
    {
        try
        {
            GPSActivo = false;
            EstadoGpsAnt = -1;
            //Reiniciamos los valores del GPS
            Datos_GPS = new GPSData();

            ActualizarIndicador(-1);
        }
        catch
        {

        }

    }

    /// <summary>
    /// Se encarga de procesar todos los datos del GPS
    /// SE QUITA DE PRODUCCION
    /// </summary>
    private void ProcesaGPS()
    {
        try
        {
            if (Datos_GPS.Latitud.Length > 0 && Datos_GPS.Longitud.Length > 0)
            {
                //Checo si éste punto es una terminal, para tomar la ruta o zona de descarga o sincronización
                UbicaTerminales(Datos_GPS.Latitud, Datos_GPS.Longitud);

                //Validamos si hubo algún cambio en la lectura de las coordenadas
                //en caso de contar con 100 lecturas iguales indicamos la existencia de una
                //posible falla con el GPS

                if (UltLat.Equals(Datos_GPS.Latitud) && UltLong.Equals(Datos_GPS.Longitud))
                {
                    if ((bool)Parametros_inicio.CAN && TachoSpeedbyCAN == 0)
                    {
                        if (Convert.ToDouble(Datos_GPS.Latitud) == 0 && Convert.ToDouble(Datos_GPS.Longitud) == 0)
                        {
                            NumLecGPSigual = 0;
                        }
                        else
                        {
                            if (NumLecGPSigual == 0)
                            {
                                NumLecGPSigual = 1;
                                UltiFechaGPS = DateTime.Now;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToDouble(Datos_GPS.Latitud) == 0 && Convert.ToDouble(Datos_GPS.Longitud) == 0)
                        {
                            NumLecGPSigual = 0;
                        }
                        else
                        {
                            NumLecGPSigual++;
                        }
                    }
                }
                else
                {
                    if (Convert.ToDouble(Datos_GPS.Latitud) == 0 && Convert.ToDouble(Datos_GPS.Longitud) == 0)
                    {
                        NumLecGPSigual = 0;
                    }
                    else
                    {
                        NumLecGPSigual = 1;
                        UltiFechaGPS = DateTime.Now;
                    }
                }

                //Log de error de GPS, validar

            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Hijo del método ProcesaGPS, es una reestructura de la lógica para hacerla mejor 
    /// y más eficiente
    /// </summary>
    private void ProcesaDatosGPS()
    {
        try
        {
            if (Datos_GPS.Latitud.Length > 0 && Datos_GPS.Longitud.Length > 0)
            {
                UbicaTerminales(Datos_GPS.Latitud, Datos_GPS.Longitud);

                if (UltLat.Equals(Datos_GPS.Latitud) && UltLong.Equals(Datos_GPS.Longitud))
                {

                    //aqui hay que pegarle
                    if (Convert.ToDouble(Datos_GPS.Latitud) == 0.0 && Convert.ToDouble(Datos_GPS.Longitud) == 0.0)
                    {
                        NumLecGPSigual = 0;
                    }
                    else
                    {
                        NumLecGPSigual++;
                    }
                }
                else
                {
                    UltLat = Datos_GPS.Latitud;
                    UltLong = Datos_GPS.Longitud;
                    UltiFechaGPS = DateTime.Now;
                    NumLecGPSigual = 1;
                }
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Método que se ejecuta con el hilo de actualizar
    /// </summary>
    private void ActualizaGPS()
    {
        //Procesa Datos de GPS
        ProcesaDatosGPS();
        //Envio a CAN a através de SAM
        MandarASAM(this.Datos_GPS);
        //actualizo el semaforo en el front
        SemaforoGPS();

        //Mando a actualizar la población
        //UbicaPoblacion(this.Datos_GPS.Latitud, this.Datos_GPS.Longitud);
    }

    #endregion

    #region "Métodos Heredados"

    /// <summary>
    /// Se encarga de Actualizar todo lo necesario que impliquen
    /// los datos GPS
    /// </summary>
    public void Actualizar()
    {
        //if (!ADOGPS.conectado())
        //{
        //    ADOGPS.desconectar();
        //    tmrConectarSocket.Start();
        //}
        //else
        //{

        //}

        if (SocketIniciado)
        {
            if (!hiloActualizar.IsAlive)
            {
                hiloActualizar = new Thread(new ThreadStart(ActualizaGPS));
                hiloActualizar.Start();
            }
        }
    }

    public string Eventos()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Se encarga de inicializar todo lo necesario para la lógica de GPS
    /// </summary>
    public void Inicializar()
    {
        //Configuramos los timers
        PrepararTimers();

        //Enciendo timer de ubicación de terminal
        tmrRevisaTerminalReinicio.Start();

        //Enciendo timer de limpieza de terminal
        tmrLimpiaTerminal.Start();
    }

    /// <summary>
    /// Se encarga de Finalizar correctamente los recursos
    /// iniciados
    /// </summary>
    public void Finalizar()
    {
        tmrLimpiaTerminal.Stop();
        tmrLimpiaTerminal.Dispose();
        tmrRevisaTerminalReinicio.Stop();
        tmrRevisaTerminalReinicio.Dispose();
        tmrConectarSocket.Stop();
        tmrConectarSocket.Dispose();
        tmrRevisaGPS.Stop();
        tmrRevisaGPS.Dispose();
        DestruirGPS();
    }

    /// <summary>
    /// Se hereda de ISistema, pero en GPS no lo ocuparemos
    /// </summary>
    public bool Sincronizar()
    {
        return true;
    }

    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"

    /// <summary>
    /// Timer que se encarga de verificar si de acuerdo al parametro del tiempo
    /// tiene que borrar la terminal.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerLimpiaTerminal_Tick(object sender, EventArgs e)
    {
        tmrLimpiaTerminal.Stop();
        try
        {

            //Si han pasado los minutos mínimo, borro las variables y no activo el timer.
            //Si no, deja que haga más vueltas en el timer.
            if (LimpiezaTerminal >= MinsLimpieza)
            {
                Terminal.LimpiaTerminal();

                LimpiezaTerminal = 0;
            }
            else
            {
                LimpiezaTerminal++;
                tmrLimpiaTerminal.Start();
            }
        }
        catch
        {

        }

        tmrLimpiaTerminal.Start();
    }

    /// <summary>
    /// Se encarga de relanzar la verificación de la conexión con el 
    /// socket de ADOGPS
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerConectarSocket_Tick(object sender, EventArgs e)
    {
        tmrConectarSocket.Stop();

        try
        {
            if (!SocketIniciado)
            {
                if (!hiloConectar.IsAlive)
                {
                    hiloConectar = new Thread(new ThreadStart(ConectarSocket));
                    hiloConectar.Start();
                }

                tmrConectarSocket.Start();
            }
            else
            {
                UltiFechaGPS = DateTime.Now;
                NumLecGPSigual = 1;
                tmrConectarSocket.Stop();
                tmrRevisaGPS.Enabled = true;
                tmrRevisaGPS.Start();
            }
        }
        catch
        {
            tmrConectarSocket.Start();
        }
    }

    /// <summary>
    /// Se encarga de estar verificando cada 5 segundo (Valor por default)
    /// si nos encontramos en una terminal y tenemos que reiniciar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrRevisaTerminalReinicio_Tick(object sender, EventArgs e)
    {
        tmrRevisaTerminalReinicio.Stop();

        try
        {
            UbicaPoblacionReinicio(Convert.ToDouble(Datos_GPS.Latitud), Datos_GPS.LatitudNS, Convert.ToDouble(Datos_GPS.Longitud), Datos_GPS.LongitudWE);
        }
        catch
        {
            
        }
        tmrRevisaTerminalReinicio.Start();
    }


    /// <summary>
    /// Se encarga de revisar el estado del GPS
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrRevisaGPS_Tick(object sender, EventArgs e)
    {
        
        if ((DateTime.Now - UltiFechaGPS2).TotalMinutes > 1)
        {
            //Significa que nos quedamos sin lectura de GPS

            tmrRevisaGPS.Stop();

            //valido si no se ha reiniciado más de 3 veces
            if (ReiniciosGPS <= 3)
            {
                ReiniciosGPS++;

                GPSActivo = false;
                ActualizarIndicador(-1);
                EstadoGpsAnt = -1;

                DestruirGPS();

                CrearGPS();

                //Envio a CAN a através de SAM
                MandarASAM(this.Datos_GPS);

                //Mando a Reiniciar la App de GPS
                RebootGPS();

                //Verifico la conexión
                tmrConectarSocket.Start();
                UltiFechaGPS2 = DateTime.Now;
            }
        }
    }
    #endregion
}

