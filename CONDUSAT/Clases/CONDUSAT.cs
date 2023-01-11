using System;
using System.Collections.Generic;
using System.Linq;
using InterfazSistema.ModelosBD;
using System.Threading;

public class CONDUSAT : ISistema, IBDContext, IMessage
{
    #region "Propiedades"


    #endregion

    #region "Propiedades heredadas"
    public int PuertoSocket { get; set; }
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }
    public Sistema Sistema { get { return Sistema.CONDUSAT; } }
    public string GetVersionSistema { get; }
    public vmdEntities VMD_BD { get; }
    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    #endregion

    #region "Variables"

    private can_parametrosinicio ParametrosInicio;

    private ADO_CONDUSAT_Cliente.CONDUSAT_Cliente.ValoresCONDUSAT ADO_Condusat;

    
    // Flags
    
    private bool Actualizando = false;

    //Timers
    private System.Windows.Forms.Timer timerProcesaCondusat = new System.Windows.Forms.Timer();

    //Hilos
    private Thread hiloActualizar;

    //Propiedades
    private string ValorCondusat { get; set; } = string.Empty;
    private int VelocidadMaxima { get; set; } = 0;
    private int VelocidadReal { get; set; } = 0;
    private string ColorVelocidad { get; set; } = "RED";
    private string AdvertenciaProceso { get; set; } = string.Empty;
    private string MensajeAdvertencia { get; set; } = string.Empty;
    private string ImgAdvertenciaCondusat { get; set; } = string.Empty;
    private string ColorSemaforo { get; set; } = string.Empty;

    //Variables temporales para mejorar el performance
    private int VelTemp = 0;
    private int VelMaxTemp = 0;

    private string MensajeCondusat { get; set; } = string.Empty;
    private bool DescargaCondusatActiva { get; set; } = false;
    private bool Sincronizando { get; set; } = false;

    private bool PresentaCondusat { get; set; } = false;

    public bool Param_ADOCAN { get; set; } = false;
    public clsMessage AdvMsg { get; set; }

    #endregion

    #region "Variables de Evento"

    public delegate void ActualizaVista(string ColorVel, int VelReal, int VelMax, string imgAdvertencia);
    public event ActualizaVista ActualizarCondusat;

    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSyncCONDUSAT;

    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor de CONDUSAT
    /// </summary>
    public CONDUSAT()
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();

        GetVersionSistema = "1.00.0";

        hiloActualizar = new Thread(new ThreadStart(ActualizaCONDUSAT));

    }
    #endregion

    #region "Métodos Publicos"


    #endregion

    #region  "Métodos Privados"

    /// <summary>
    /// Inicia el Socket Listener con CONDUSAT
    /// </summary>
    private void IniciarSocket()
    {
        try
        {

            ADO_Condusat = new ADO_CONDUSAT_Cliente.CONDUSAT_Cliente.ADO_CONDUSAT_Cliente();

            ADO_Condusat.Iniciar_Cliente();
        }
        catch(Exception ex)
        {

        }
        
    }

    private void ProcesaADOCONDUSAT(clsMessage AdvMsg)
    {
        try
        {
            this.VelocidadMaxima = Convert.ToInt32(AdvMsg.GetValuebyKey("lblVelocidadMaxima"));
            this.VelocidadReal = Convert.ToInt32(AdvMsg.GetValuebyKey("lblVelocidadActual"));
            this.ColorVelocidad = AdvMsg.GetValuebyKey("ColorVelocidad").ToString();
            this.AdvertenciaProceso = AdvMsg.GetValuebyKey("AdvertenciaProceso").ToString();
            this.MensajeAdvertencia = AdvMsg.GetValuebyKey("lblAdvertencia").ToString();
            this.ImgAdvertenciaCondusat = AdvMsg.GetValuebyKey("pctAdvertencia").ToString();
            this.ColorSemaforo = AdvMsg.GetValuebyKey("frmColor").ToString();

            if (AdvMsg.GetValuebyKey("Mensaje") != null)
            {
                this.MensajeCondusat = AdvMsg.GetValuebyKey("Mensaje").ToString();
            }

            if (AdvMsg.GetValuebyKey("DescargaCondusat") != null)
            {
                this.DescargaCondusatActiva = Convert.ToBoolean(AdvMsg.GetValuebyKey("DescargaCondusat"));
            }
        }

        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de recopilar los datos del socket y traerlos a variables
    /// para hacer uso de ellas
    /// </summary>
    private void ProcesaADOCONDUSAT()
    {
        try
        {
            if(ADO_Condusat.VAL_CONDUSAT_Velocidad_Maxima != null && !ADO_Condusat.VAL_CONDUSAT_Velocidad_Maxima.Equals("")){
                this.VelocidadMaxima = Convert.ToInt32(ADO_Condusat.VAL_CONDUSAT_Velocidad_Maxima);
            }

            if( ADO_Condusat.VAL_CONDUSAT_Velocidad_Real != null && !ADO_Condusat.VAL_CONDUSAT_Velocidad_Real.Equals(""))
            {
                this.VelocidadReal = Convert.ToInt32(ADO_Condusat.VAL_CONDUSAT_Velocidad_Real);
            }

            if(ADO_Condusat.VAL_CONDUSAT_Color != null)
            {
                this.ColorVelocidad = ADO_Condusat.VAL_CONDUSAT_Color;
            }
            
            this.AdvertenciaProceso = ADO_Condusat.VAL_CONDUSAT_Advertencia_Proceso;

            this.MensajeAdvertencia = ADO_Condusat.VAL_CONDUSAT_Advertencia_Mensaje;

            if(ADO_Condusat.VAL_CONDUSAT_Advertencia_Imagen != null)
            {
                this.ImgAdvertenciaCondusat = ADO_Condusat.VAL_CONDUSAT_Advertencia_Imagen;
            }

            this.ColorSemaforo = ADO_Condusat.VAL_CONDUSAT_Color;


            if (ADO_Condusat.VAL_CONDUSAT_Sincroniza_Mensaje != null && !ADO_Condusat.VAL_CONDUSAT_Sincroniza_Mensaje.Equals(""))
            {
                List<string> Mensaje = new List<string>();

                string msgTemp = ADO_Condusat.VAL_CONDUSAT_Sincroniza_Mensaje;

                string[] _split = msgTemp.Split('$');


                foreach (string linea in _split)
                {
                    Mensaje.Add(linea);
                }

                if (Mensaje.Count > 0)
                {
                    this.DescargaCondusatActiva = Convert.ToBoolean(Convert.ToInt32(Mensaje[1]));
                    this.MensajeCondusat = Mensaje[0];
                }
            }
        }
        catch(Exception ex)
        {

        }
    }

    /// <summary>
    /// Se encarga de configurar los timers que se van a ocupar
    /// </summary>
    private void PreparaTimers()
    {
        //Timer encargado de procesar la información de Condusat
        timerProcesaCondusat.Interval = 500;
        timerProcesaCondusat.Enabled = true;
        timerProcesaCondusat.Tick += new EventHandler(timerProcesaCondusat_Tick);
    }

    /// <summary>
    /// Método que se ejecuta con el hilo de actualizar
    /// </summary>
    private void ActualizaCONDUSAT()
    {
        if (this.VelTemp != VelocidadReal || this.VelMaxTemp != VelocidadMaxima)
        {
            if (!Sincronizando)
            {
                this.VelTemp = VelocidadReal;
                this.VelMaxTemp = VelocidadMaxima;

                ActualizarCondusat(ColorVelocidad, VelocidadReal, VelocidadMaxima, ImgAdvertenciaCondusat);
            }
        }
    }
    #endregion

    #region #Métodos Heredados"
    /// <summary>
    /// Lo manda a llamar SAM para que actualice los valores
    /// en el front
    /// </summary>
    public void Actualizar()
    {
        if (!hiloActualizar.IsAlive)
        {
            hiloActualizar = new Thread(new ThreadStart(ActualizaCONDUSAT));
            hiloActualizar.Start();
        }
    }

    public string Eventos()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Se encarga de Finalizar y destruir lo necesario
    /// para un correcto apagado de Condusat
    /// </summary>
    public void Finalizar()
    {
        timerProcesaCondusat.Stop();
        timerProcesaCondusat.Dispose();
    }

    /// <summary>
    /// Inicializa los componentes necesarios para que
    /// la Lógica de CONDUSAT Funcione
    /// </summary>
    public void Inicializar()
    {
        //Mandamos a inciar el Socket
        IniciarSocket();

        //Preparamos los timers
        PreparaTimers();

        //Mandamos a iniciar el Timer que recopila datos del socket
        timerProcesaCondusat.Start();
    }

    /// <summary>
    /// Método que se invoca desde SAM y nos sirve para
    /// mandar sincronizar el sistema de CONDUSAT
    /// </summary>
    /// <returns></returns>
    public bool Sincronizar()
    {
        bool exito = false;

        try
        {
            if (!this.Sincronizando)
            {
                this.Sincronizando = true;

                //Mandamos a invocar el proceso de sincronicazión de la App Condusat
                ADO_Condusat.CAN_Sincronizacion("1");

                //Avisamos al Front
                EventoSyncCONDUSAT("Iniciando sincronización de Condusat...", 0);

                //Damos 30 segundos en espera de que condusat confirme
                DateTime InicioSync = DateTime.Now;

                while (this.DescargaCondusatActiva == false)
                {
                    //Mandamos a procesar los datos que envia CONDUSAT
                    ProcesaADOCONDUSAT();

                    if ((DateTime.Now - InicioSync).TotalSeconds > 40)
                    {
                        break;
                    }
                }

                //Si se sale por tiempo, lo más seguro es que se cancele la sincronización
                if (this.DescargaCondusatActiva)
                {

                    InicioSync = DateTime.Now;
                    int numTemp;
                    int numMensaje;


                    string mensajeTemp = string.Empty;
                    while (DescargaCondusatActiva)
                    {
                        ProcesaADOCONDUSAT();

                        numTemp = mensajeTemp.Length;
                        numMensaje = MensajeCondusat.Length;

                        if (mensajeTemp.Equals(this.MensajeCondusat))
                        {
                            if ((DateTime.Now - InicioSync).TotalSeconds >= 150)
                            {//Se quedó pegado o algo sucedio en la sincronización
                                //exito = false;
                                this.MensajeCondusat = "Se agotó el tiempo de espera, intente de nuevo.";
                                break;
                            }
                        }
                        else
                        {
                            EventoSyncCONDUSAT(this.MensajeCondusat, 0);

                            mensajeTemp = MensajeCondusat;

                        }
                    }

                    EventoSyncCONDUSAT(this.MensajeCondusat, 0);

                    if (mensajeTemp.Contains("Carga y descarga terminada"))
                    {
                        exito = true;
                    }
                    else
                    {
                        exito = false;
                    }

                    Thread.Sleep(2000);

                    //Manda mensaje final, pero se pone al termino de SYNC en SAM

                }
                else
                {
                    //Error sincronización CONDUSAT
                    EventoSyncCONDUSAT("Error de comunicación,  \nla descarga de condusat no puede llevar a cabo.", 0);
                    exito = false;
                }
            }
        }
        catch
        {
            exito = false;
        }

        //Powered ByRED 30ABR2021
        this.Sincronizando = false;
        return exito;
    }
    #endregion

    #region "Eventos"
    /// <summary>
    /// Sirve para mandar a firmar viaje en CONDUSAT
    /// </summary>
    /// <param name="_autobus"></param>
    /// <param name="_operador"></param>
    /// <param name="_tipo"></param>
    /// <param name="_fechaapertura"></param>
    /// <param name="_fechacierre"></param>
    /// <param name="_cambioManos"></param>
    public void FirmarCAN(string _autobus, string _operador, string _tipo, string _fechaapertura, string _fechacierre, string _cambioManos)
    {
        try
        {
            //ADO_Condusat.CAN_Firma(_autobus, _operador, _tipo, _fechaapertura, _fechacierre, _cambioManos);

        }
        catch
        {

        }

    }
    #endregion

    #region "Timers"

    /// <summary>
    /// Lógica del timer para procesar la información de CONDUSAT
    /// Proveniente del Socket
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerProcesaCondusat_Tick(object sender, EventArgs e)
    {
        timerProcesaCondusat.Stop();

        ProcesaADOCONDUSAT();

        timerProcesaCondusat.Start();
    }
    #endregion
}
