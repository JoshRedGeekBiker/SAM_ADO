using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFE.Properties;
using WMPLib;
public class FrontEngine : IDisposable
{
    #region "Constructores"

    /// <summary>
    /// Constructor principal
    /// </summary>
    public FrontEngine()
    {

    }


    /// <summary>
    /// Constructor Para levantar el sistema
    /// </summary>
    public FrontEngine(string _autobus, bool _modoPrueba, string _pass, string _passexit, string _version, bool _MoodGod, bool _btnOff, bool _btnPanico)
    {
        this.NumAutobus = _autobus;
        this.ModoPrueba = _modoPrueba;
        this.btnOff = _btnOff;
        this.btnPanico = _btnPanico;

        if (_MoodGod)
        {
            this.Pass = "1";
            this.PassSalida = "1";
            this.Version = "DEVELOPER";
        }
        else
        {
            this.Pass = _pass;
            this.PassSalida = _passexit;
            this.Version = _version;
        }

        ObtenerPantallas();
    }

    /// <summary>
    /// Constructor para configuración
    /// </summary>
    /// <param name="_modoPrueba"></param>
    /// <param name="_pass"></param>
    /// <param name="_passexit"></param>
    public FrontEngine(bool _modoPrueba, string _pass, string _passexit, string _version, bool MoodGod, bool _btnOff)
    {
        this.NumAutobus = "";
        this.ModoPrueba = _modoPrueba;
        this.btnOff = _btnOff;

        if (MoodGod)
        {
            this.Pass = "1";
            this.PassSalida = "1";
            this.Version = "DEVELOPER";
        }
        else
        {
            this.Pass = _pass;
            this.PassSalida = _passexit;
            this.Version = _version;
        }

        ObtenerPantallas();
    }
    #endregion

    #region "Variables"
    public string minsApagado = String.Empty;

    private bool btnOff = false;
    private bool btnPanico = false;

    private bool ModoPrueba = false;
    public bool ModoNocturno = false; // Modificación depurativa
    public bool Sincronizando = false;
    private bool ModoConfig = false;

    private string Pass = string.Empty;
    private string PassSalida = string.Empty;

    //Administración de Vistas
    private string VistaAnterior = string.Empty;
    private string VistaActual = string.Empty;
    private int TiempoCierreVentanas = 10;

    private Thread HiloCarga;
    private bool cargando = true;

    //Encabezado
    public double FR_Meta { get; set; }
    public double FR_Real { get; set; }
    private string Version = String.Empty;
    private string txtViajeCerrado = "El Viaje Se Encuentra Cerrado";

    //Pie de página
    public string NumAutobus { get; set; } //Se ocupa también para la configuración
    public string NomConductor { get; set; }
    public string NomConductorTemp { get; set; }
    public string ClvConductor { get; set; }
    public string ClvConductorTemp { get; set; }
    public bool EnViaje { get; set; }

    //CAN
    public bool FR_indicador { get; set; }
    public double Kms { get; set; }
    public double Lts { get; set; }
    public bool protocoloCAN { get; set; }

    //CONDUSAT
    public int Vel { get; set; }

    //VMD
    public string rutaVLC { get; set; } //Para que pueda levantar el reproductor
    public bool EnReproduccion { get; set; } //Para tener conocimiento si VMD está en reproducción Powered ByRED 18FEB2021
    public string NombrePelicula { get; set; }//Powered ByRED 18FEB2021
    public string DuracionPelicula { get; set; }//Powered ByRED 18FEB2021
    public string TiempoPelicula { get; set; }//Powered ByRED 18FEB2021
    public Size TamanoPantSec { get; set; }//Powered ByRED 24MAR2021

    //Sistemas
    public bool CAN { get; set; } = false;
    public bool VMD { get; set; } = false;
    public bool CONDUSAT { get; set; } = false;
    public bool PLAT { get; set; } = false;
    public bool SIA { get; set; } = false;
    public bool TELEMETRIA { get; set; } = false;
    public bool GPS { get; set; } = false; //Powered ByRED 01/JUL/2020
    public bool MINISIA { get; set; } = false;//Powered ByRED 17MAR2021
    public bool SIIAB_POI { get; set; } = false; //powered byToto

    //Lista De Vistas

    private List<string> ListaVistas = new List<string>();
    //private List<string> ListaVistasActividad = new List<string>();

    //ListaPantallas
    private List<Pantalla> ListaPantallas = new List<Pantalla>();

    //Nombre TipoMetaCAN
    public string tempNombreMetaCAN = string.Empty; //Powered ByRED 10/SEP/2020

    //SIA
    private bool MostrandoMensajeSIA = false;//Powered ByRED 19MAR2021
    private bool InLogin = false;// para saber si ya loggeadosPowered ByRED 19MAR2021
    private string MensajeCONDUCTOR = string.Empty;//Powered ByRED 19MAR2021
    private List<string> PISIA = new List<string>();//Powered ByRED 19MAR2021
    private bool MostrandoCintilloSIA = false;//Powered ByRED 29MAR2021
    private bool MostrandoPISIA = false;//Powered ByRED 29MAR2021
    //Powered ByRED 12ABR2021
    public bool HabilitarCintillo { get; set; }
    public string MensajeInicialCintillo { get; set; }
    public long? VueltasMensajeCintillo { get; set; }
    public long? AltoCintillo { get; set; }
    public string ColorFuenteCintillo { get; set; }
    public long? TamanioFuenteCintillo { get; set; }
    public string ColorFondoCintillo { get; set; }
    public long? VelocidadCintillo { get; set; }
    public string PosicionMarqueeCintillo { get; set; }
    public long TimerCintilloSegundos { get; set; }

    private int EstadoGPS { get; set; } = 0; //Powered ByRED 27MAY2021

    #endregion

    #region "VariablesTimers"

    private System.Windows.Forms.Timer timerActualizaEncabezado = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerActualizaCondusat = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerActualizaCAN = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerActualizaVMD = new System.Windows.Forms.Timer(); //Powered ByRED 20JUN2020
    //    private System.Windows.Forms.Timer timerActualizaPiePagina = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerMuestraFront = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerSiempreAdelante = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerVerificaReproductor = new System.Windows.Forms.Timer();

    private System.Windows.Forms.Timer timerActualizaSIA = new System.Windows.Forms.Timer();//Powered ByRED 19MAR2021
    private System.Windows.Forms.Timer timerTerminaPISIA = new System.Windows.Forms.Timer();//Powered ByRED 19MAR2021

    #endregion

    #region "Vistas(forms)" 
    //Leeme, si se añade un nuevo form, agregarlo al catalogo de forms
    //que se encuentra en el método ObtenerForm() y también en el método de ActivarModoNocturno()
    //Si deseas que el form se cierre en automatico agregarlo a ObtenerActividad() y ReiniciarActividad()
    //Powered ByRED 16JUN2020

    private frmCarga VistaCarga;//Para mostrar la carga de los sistemas

    //Forms
    public frmSistemas VistaSistemas; //Front Principal, El que encapsula a todos los sistemas
    private frmMenu VistaMenuCAN; // Front perteneciente a CAN
    private frmError VistaError; // Front que muestra un error al usuario
    private frmViaje VistaViaje; //Front que se ocupa para los Viajes
    private frmClvPoblacion VistaPoblacion; //Front que se ocupa para ingresar la Clave de Poblacion
    private frmPopUp VistaPopUp; //Front que se ocupa para mostrar datos de viaje y de GPS
    private frmMovtosCAN VistaMovtos; //Front que se ocupa para mostrar los datos de CAN y versiones
    private frmSync VistaSync; //Front que se ocupara para mostrar el proceso de sincronización
    private frmLogin VistaLogin; //Front que se ocupa para poder accesar al sistema mediante una contraseña
    private frmSalida VistaSalida;//Front que se encarga de pedir un pass para salir de la aplicación o apagar el sistema
    private frmReproductor VistaReproductor; // Front que se encarga de mostrar la reproducción de los videos

    //TVE
    private frmMenuTVE VistaMenuTVE;// Front que se encarga de mostrar el menú de TVE
    private frmTransferTVE VistaTransferTVE; //Front que muestra la ventana de transferencia de la TVE
    private frmStatusTVE VistaStatusTVE; // Front que muestra el resultado de la transferencia de TVE
    private frmQRTVE VistaQRTVE; // Front que muestra las opciones para ver la tarjeta de Viaje
    private frmLoginTVE VistaLoginTVE; //Front que muestra el una ventana para autenticarse por medio de una contraseña

    //Forms para la configruacion
    private frmConfigAutobus ConfigAutobus; //Sirve para configurar el número de autobus y el num de Region
    private frmConfiguracion ConfigSistemas;//Sirve para configurar el tipo de tarjeta de CAN y los sistemas a ocupar
    private frmMetaCAN ConfigMeta;//Sirve para configurar la meta Personalizada de CAN

    //Telemetria
    private frmDatosTelemetria VistaDatosTelemetria;//Sirve para mostrar un reporte del sistema Telemetria Powered ByRED 15JUN2020

    //VMD
    private frmHerramientasVMD VistaHerramientasVMD;//Contiene las herramientas para VMD Powered ByRED 16JUN2020
    private frmCargadorDePautas VistaCargadorPauta; //Contiene la logica para cargar Pauta desde Medios Externos Powered ByRED 15JUN2020


    //SIA
    private frmPanelMensajes VistaMensajesSIA;//Muestra los mensajes de SIA Powered ByRED 17MAR2021 
    private frmMostrarMensaje VistaMostrarSMS;//Muestra a detalle el contenido de un mensaje Powered ByRED19MAR2021
    private frmEnviandoMensaje VistaMensajeEnviado;//Muestra cuando un mensaje ha sido enviado Powered ByRED 19MAR2021

    //SIIAB POI
    public frmMenuSpots VistaMenuSpots;//Contiene el menu spots para VMD Powered ByToto 16JUN2020
    public frmSpots VistaSpots; //Contiene la logica para cargar Spots Powered ByToto ENERO2023


    #endregion

    #region "Eventos"

    //Para mostrar durante la sincronización
    public delegate void holaSyncSAM();
    public event holaSyncSAM EventoSyncSAM;

    //Para avisar a SAM que tenemos que abrir viaje
    public delegate bool ViajeCANSAM(string tipo, string clvConductor);
    public event ViajeCANSAM ViajeSAM;

    //Para verificar conductor en SAM
    public delegate string ValidarConductorSAM(string clvConductor);
    public event ValidarConductorSAM ConductorSAM;

    //Para verificar si tenemos protocolo
    public delegate bool ValidarProtocoloSAM();
    public event ValidarProtocoloSAM ProtocoloSAM;

    //Para verificar si la población es válida
    public delegate bool ValidaPoblacionSAM(string CVEPOB);
    public event ValidaPoblacionSAM PoblacionSAM;

    //Para verificar si tenemos alguna población
    public delegate bool EnPoblacion();
    public event EnPoblacion ChecarPoblacion;

    //Para traer los datos del GPS
    public delegate List<string> TraerDatosGPS();
    public event TraerDatosGPS DatosGPSSAM;

    //Para traer los datos que se mostraran en el Form de movtoscan
    public delegate List<string> TraerMovtosCAN();
    public event TraerMovtosCAN DatosMovtosSAM;

    //Para Avisar a SAM que tiene que cerrar el sistema y además apagarlo o reiniciar el equipo
    //Tipo:
    //0 : Apagado
    //1 : Reinicio
    //2: Salir del sistema
    public delegate void AvisarSAMApagar(bool ReinicioAutomatico = false);
    public event AvisarSAMApagar SAMCerrar;

    //Para apagar SAM si se encuentra en modo de configuración
    public delegate void ApagarSAMConfig();
    public event ApagarSAMConfig SAMCerrarConfig;

    //Manda la región a SAM para saber si existe o no
    public delegate string ValidarRegionSAM(long Reg);
    public event ValidarRegionSAM SAMRegion;

    //Se ocupa  para que SAM guarde la configuración
    public delegate void EnviarConfigSAM(int TarjetaCAN, bool Antivirus);
    public event EnviarConfigSAM ConfigSAM;

    //Se ocupa para que despues de la configuración
    //se mande lanzar a SAM
    public delegate void LanzarSAM();
    public event LanzarSAM SAM;

    //Manda a pedir a SAM una lista con las regiones existentes
    public delegate List<string> TraerRegiones();
    public event TraerRegiones Regiones;

    //Le dice a SAM que deberá encender o apagar el wifi
    public delegate bool WIFISAM(bool Encender, bool Transferencia);
    public event WIFISAM WIFI;

    //Le dice a SAM que deberá encender o apagar el Ethernet
    //Powered ByRED 27MAY2021
    public delegate bool EthernetSAM(bool Encender);
    public event EthernetSAM ETHERNET;

    //Se encarga de recuperar la velocidad que el sistema tiene actualmente
    public delegate int SAMVEL();
    public event SAMVEL Velocidad;

    //Se encarga de ejecutar la logica de Transferencia en Abordaje
    public delegate Task<bool> TVETransfer();
    public event TVETransfer AbordajeTrans;

    /// <summary>
    /// Se encarga de validar la transferencia de TVE
    /// </summary>
    /// <returns></returns>
    public delegate Task<bool> ValidarTransferTVE();
    public event ValidarTransferTVE TransferTVE;

    /// <summary>
    /// Se encarga de obtener el estado de la transferencia de TVE
    /// </summary>
    /// <returns></returns>
    public delegate bool EstadoTransferTVE();
    public event EstadoTransferTVE EstadoTVE;

    /// <summary>
    /// Se encarga de obtener el estado de la conexion
    /// con logica de TVE
    /// </summary>
    /// <returns></returns>
    public delegate Task<bool> ConexionTVE();
    public event ConexionTVE ConTVE;

    /// <summary>
    /// Obtiene la contraseña del sistema de TVE
    /// </summary>
    /// <returns></returns>
    public delegate string PasswordTVE();
    public event PasswordTVE PassTVE;

    /// <summary>
    /// Se encarga de generar los Qr para la consulta de Abordaje
    /// </summary>
    /// <returns></returns>
    public delegate Task<bool> GenerarQRConsulta();
    public event GenerarQRConsulta QrConsulta;

    /// <summary>
    /// Obtiene la informacion de la corrida
    /// </summary>
    /// <returns></returns>
    public delegate string InformacionCorrida();
    public event InformacionCorrida InfoCorrida;

    /// <summary>
    /// Se encarga de terminar con la lógica de TVE
    /// </summary>
    public delegate void TerminarTVE();
    public event TerminarTVE FinTVE;

    /// <summary>
    /// Se encarga de mandar a pedir el reporte de Telemetria
    /// </summary>
    public delegate List<string> ReporteTelemetria();
    public event ReporteTelemetria Telematics;

    /// <summary>
    /// Se encarga de mandar a pedir la lista de pautas disponibles para VMD
    /// </summary>
    /// <returns></returns>
    public delegate List<string> PautasListVMD(string tipo);
    public event PautasListVMD PautasVMD;
    /// <summary>
    /// Se encarga de mandar a pedir la lista de Spots disponibles para VMD
    /// Powered byToto♫♫
    /// </summary>
    /// <returns></returns>
    public delegate List<spotPOI> listaSpotsVMD(String tipo);
    public event listaSpotsVMD SpotsVMD;
    /// <summary>
    /// Se encarga de enviar el testigo de Spot reproducido
    /// Powered byToto♫♫
    /// </summary>
    /// <returns></returns>
    public delegate void evtTestigoSpotPoi(spotPOI sp);
    public event evtTestigoSpotPoi testigoSpotPOISAM;

    /// <summary>
    /// Se encarga de mandar a planchar la pauta
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="nombre"></param>
    /// <returns></returns>
    public delegate Task<bool> PlancharPauta(string tipo, string nombre);
    public event PlancharPauta Pauta;

    /// <summary>
    /// Se encarga de validar la Pauta
    /// Powered ByRED 16/JUL/2020
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="nombre"></param>
    /// <returns></returns>
    public delegate Task<bool> ValidarPauta(string tipo, string nombre);
    public event ValidarPauta ValidaPauta;

    /// <summary>
    /// Sirve para recuperar las pautas desde un medio extraíble
    /// </summary>
    /// <param name="_letraUnidad"></param>
    /// <returns></returns>
    public delegate List<string> RecuperarPautaUSB(string _letraUnidad);
    public event RecuperarPautaUSB PautaUSB;

    /// <summary>
    /// Se encarga de mandar a pedir el progreso del copiado
    /// </summary>
    /// <returns></returns>
    public delegate int PedirProgresoCopiado();
    public event PedirProgresoCopiado ProgresoCopiado;

    /// <summary>
    /// Se encarga de mandar a pedir las metas de CAN disponibles
    /// </summary>
    /// <returns></returns>
    public delegate bool MetaCAN();
    public event MetaCAN ValidarMetasCAN;

    /// <summary>
    /// Se encarga de recuperar las metas por region de CAN
    /// </summary>
    /// <returns></returns>
    public delegate List<string> ObtenerMetasCAN();
    public event ObtenerMetasCAN MetasCAN;

    /// <summary>
    /// Se encarga de obtener los mensajes para mostrar de SIA según su tipo
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    public delegate List<string> MensajesListSIA(int tipo);
    public event MensajesListSIA MensajesSIA;

    /// <summary>
    /// Envia un mensaje de texto através de SIA
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="msj"></param>
    /// <returns></returns>
    public delegate bool EnviarMensajeSIA(string msj);
    public event EnviarMensajeSIA MensajeSIA;

    /// <summary>
    /// Se encarga de enviar la alerta de Robo através de SIA
    /// Powered ByRED 08JUN2021
    /// </summary>
    /// <returns></returns>
    public delegate void EnviarAlertaRoboSIA();
    public event EnviarAlertaRoboSIA AlertaRobo;
    #endregion

    #region "Eventos VMD"
    //Se ocupa para inicializar el reproductor de VMD
    public delegate void LanzarSAMPLAY();
    public event LanzarSAMPLAY SAMPLAY;
    //Delegado BuscaPauta
    public delegate void LanazaBuscarPauta();
    public event LanazaBuscarPauta eVMDbuscarPauta;
    // Delegado ReiniciarPauta
    public delegate void LanzaReiniciarPauta(bool DetenerVideo);
    public event LanzaReiniciarPauta eVMDreiniciarPauta;
    // Delegado para actualización de timer
    public delegate void LanzaActualizaActividad(double posicion, double longitud);
    public event LanzaActualizaActividad eVMDActualizaActividad;



    public delegate void LanzaActualizaUltimaVez(int idArchivo);
    public event LanzaActualizaUltimaVez eVMDActualizaUltimaVez;

    public delegate void LanzaChecaSiguienteVideo();
    public event LanzaChecaSiguienteVideo eVMDChecaSiguienteVideo;

    public delegate void LanzaValidaVolumen();
    public event LanzaValidaVolumen eVMDValidaVolumen;

    public delegate void AgregarLog(string VideoURL, int idPelicula, int MinutosMax, bool Ejecuta);
    public event AgregarLog eVMDAgregarLog;

    #endregion

    #region "Barra de Tareas de Windows"
    [DllImport("user32.dll")]
    private static extern int FindWindow(string className, string windowText);
    [DllImport("user32.dll")]
    private static extern int ShowWindow(int hwnd, int command);

    private void BarraDeTareas(bool Activar)
    {
        int hWnd = FindWindow("Shell_TrayWnd", "");
        if (Activar)
        {
            ShowWindow(hWnd, 1);
        }
        else
        {
            ShowWindow(hWnd, 0);
        }
    }
    #endregion

    #region "BloquearControles"
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool BlockInput(bool fBlockIt);


    #endregion

    #region "Métodos Públicos"
    /// <summary>
    /// Levanta la interface Gráfica
    /// </summary>
    /// <returns></returns>
    public bool HolaMundo()
    {
        try
        {
            //Configuramos las primeras vistas
            FirstView();

            //Configura los diversos Timers
            ConfigurarTimers();

            //Inicia todos los times
            IniciaTimers();

            //Iniciamos las variables, si son nulas

            if (this.NomConductor == null) this.NomConductor = string.Empty;

            if (this.ClvConductor == null) this.ClvConductor = string.Empty;

            if (this.NomConductorTemp == null) this.NomConductorTemp = string.Empty;

            if (this.ClvConductorTemp == null) this.ClvConductorTemp = string.Empty;

            this.FR_indicador = false;
            this.FR_Meta = 0.00;
            this.FR_Real = 0.00;


            //Agregamos primero la vista de sistemas
            ListaVistas.Add(VistaSistemas.Name);

            //Lanzamos la vista de Login
            this.MostrarLogin();

            //Si no estamos en modo prueba se activa éste timer
            if (!ModoPrueba)
            {
                timerSiempreAdelante.Enabled = true;
                timerSiempreAdelante.Start();
            }

            //Para que regrese el foco a vista sistemas cuando terminemos de loggear
            // this.VistaAnterior = VistaSistemas.Name;

            if (this.VMD)
            {
                //Para el reproductor de video
                VistaReproductor.Show();
            }

            this.MostrarCargando(false);

            Application.Run(VistaSistemas);

            return true;
        }
        catch (Exception ex)
        {
            MostrarCargando(false);
            MessageBox.Show("Error Crítico, detalles:  \n" + ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }
    }

    /// <summary>
    /// Levanta la interface Gráfica de la configuración
    /// </summary>
    /// <returns></returns>
    public bool HolaConfig()
    {
        try
        {
            //Flageamos que estamos en Modo Configuracion
            ModoConfig = true;

            //Configuramos las primeras vistas
            FirstViewConfig();

            //Configuramos Timers
            ConfigTimersRender();

            //Si no estamos en modo prueba se activa éste timer
            if (!ModoPrueba)
            {
                //timerSiempreAdelante.Enabled = true;
                //timerSiempreAdelante.Start();
                //Ocultamos la Barra de tarreas
                BarraDeTareas(false);
            }

            //Damos el foco a la vista de login
            FocusON(VistaLogin);

            Application.Run(VistaReproductor);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error Crítico al tratar de lanzar el La configuración \n" + ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    /// <summary>
    /// Se encarga de finalizar el front
    /// en modo operación normal
    /// </summary>
    public void TerminarFront()
    {
        //Detengo timers
        timerActualizaEncabezado.Stop();
        timerActualizaEncabezado.Dispose();

        timerActualizaCondusat.Stop();
        timerActualizaCondusat.Dispose();

        timerActualizaCAN.Stop();
        timerActualizaCAN.Dispose();

        //Powered ByRED 19MAR2021
        timerActualizaVMD.Stop();
        timerActualizaVMD.Dispose();

        //Powered ByRED 19MAR2021
        timerActualizaSIA.Stop();
        timerActualizaSIA.Dispose();


        Application.ExitThread();
    }

    /// <summary>
    /// Se encarga de finalizar el front
    /// en modo configuracion de la aplicación
    /// </summary>
    public void TerminarConfigFront()
    {
        if (ConfigSistemas != null)
        {
            ConfigSistemas.Close();
        }
        Application.ExitThread();
    }

    /// <summary>
    /// Se encarga de destruir el front
    /// </summary>
    public void Dispose()
    {
        //Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region "Métodos Privados"

    /// <summary>
    /// Se encarga de configurar la primer vista de los sistemas
    /// </summary>
    private void FirstView()
    {
        VistaSistemas = new frmSistemas(ModoPrueba, this.ModoNocturno, this.CAN, this.VMD, this.CONDUSAT, this.SIA, this.TELEMETRIA, this.GPS, this.btnOff, this.btnPanico, this.SIIAB_POI);
        VistaSistemas.NumAutobus = this.NumAutobus;
        VistaSistemas.Version = this.Version;

        DatosPieDePagina();

        //Asignamos los eventos
        VistaSistemas.MuestraMenu += MostrarMenu;
        VistaSistemas.MandaError += MostrarError;
        VistaSistemas.MuestraSalir += VistaCerrarSistema;
        //VistaSistemas.MuestraSalir += MostrarApagar_Salir;
        VistaSistemas.Carga += TerminarCarga;
        VistaSistemas.Repro += this.InicializarRepro;
        VistaSistemas.EReiniciar += this.Func_ReiniciarReproductor;
        //VistaSistemas.EBuscaPauta += this.Func_BuscarPauta;
        VistaSistemas.EReiniciarPauta += this.Func_ReiniciarPauta;
        VistaSistemas.EControlVolumen += this.Func_ControlVolumen;
        VistaSistemas.EDetenerPelicula += this.Func_DetenerPelicula;
        VistaSistemas.LockPant += this.BloquearPantalla;
        VistaSistemas.Ubicacion += this.GetlocationPant;
        VistaSistemas.MuestraHerramientasVMD += this.MostrarHerramientasVMD; //Powered ByRED 16JUN2020
        VistaSistemas.MuestraMenuSpots += this.MuestraMenuSpots; //Powered ByToto 16JUN2020
        VistaSistemas.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020
        VistaSistemas.MuestraMensajesSIA += this.MostrarMensajesSIA; //Powered ByRED 17MAR2021
        VistaSistemas.LedGPSView += this.LedGPSHorarioNocturno;//Powered ByRED 27MAY2021
        VistaSistemas.Robo += this.EnviarAlertaRobo;//Powered byRED 08JUN2021
        VistaSistemas.CargadorSpots += this.MostarCargadorSpots; //POWERED BYTOTO CAMBIO DE REQUERIMIENTO 20ABRIIL23



        //Si tenemos VMD...
        if (this.VMD)
        {
            //Se inicializa el form de Reproductor
            VistaReproductor = new frmReproductor(this.ModoPrueba, rutaVLC);
            VistaReproductor.evtActualizaActividad += this.Func_ActualizaActividad;
            VistaReproductor.evtActualizarUltimaVez += this.Func_ActualizarUltimaVez;
            VistaReproductor.evtChecarSiguienteVideo += this.Func_ChecaSiguienteVideo;
            VistaReproductor.evtInfoVideo += this.Func_InfoVideo;
            VistaReproductor.evtPosicionVideo += this.Func_PosicionVideo;
            VistaReproductor.evtEstadoReproduccion += this.Func_EstadoReproduccion;
            VistaReproductor.evtAgregarLog += this.Func_AgregarLogReproductor;
            VistaReproductor.evtTerminarPoi += this.TerminaSpotPoi;
            VistaReproductor.ReanudarPeliculaPOI += this.InicializarRepro;
            VistaReproductor.evtEnviarTestigo += this.testigoSpotPOI;
            VistaReproductor.enviaMSG += this.MostrarPopUp;


            //Asignamos la posición al reproductor
            VistaReproductor.Location = GetlocationPantSec();


            VistaReproductor.Cintillo += this.eMostrandoCintillo;//Powered ByRED 14ABR2021

            //ParaCintillo SIA
            //Powered ByRED 13ABR2021
            if (this.SIA && this.HabilitarCintillo)
            {
                VistaReproductor.ParamCintillo = new frmReproductor.ParametrosCintillo();
                VistaReproductor.ParamCintillo.PosicionCintillo = this.PosicionMarqueeCintillo.ToUpper().Equals("T") ? DockStyle.Top : DockStyle.Bottom;
                VistaReproductor.ParamCintillo.ColorDFondo = this.ColorFondoCintillo;
                VistaReproductor.ParamCintillo.VelocidadDCintillo = Convert.ToInt32(this.VelocidadCintillo);
                var Fuente = new System.Drawing.Font("Arial",
                                                Convert.ToInt32(this.TamanioFuenteCintillo),
                                                FontStyle.Regular,
                                                GraphicsUnit.Point,
                                                ((byte)(0)));
                VistaReproductor.ParamCintillo.TamanioDFuente = Fuente;
                VistaReproductor.ParamCintillo.ColorDFuente = this.ColorFuenteCintillo;
                VistaReproductor.ParamCintillo.VueltaCintillo = Convert.ToInt32(this.VueltasMensajeCintillo);
            }
        }


        //Agregamos las vistas de actividad a la lista
        //Que son las que se tienen que cerrar si no hay actividad
        //ListaVistasActividad.Add("frmMenu");
        //ListaVistasActividad.Add("frmError");
        //ListaVistasActividad.Add("frmViaje");
        //ListaVistasActividad.Add("frmClvPoblacion");
        //ListaVistasActividad.Add("frmPopUp");
        //ListaVistasActividad.Add("frmMovtosCAN");
        //ListaVistasActividad.Add("frmSalida");
        ////ListaVistasActividad.Add("frmMenuTVE");
        //ListaVistasActividad.Add("frmSync");
        //ListaVistasActividad.Add("frmHerramientasVMD");
    }
    /// <summary>
    /// Se encarga de levantar la primer vista
    /// para el modo configuracion
    /// </summary>
    private void FirstViewConfig()
    {
        //Se inicializa el form de Login
        VistaLogin = new frmLogin(Pass, this.Version, ModoPrueba, true, this.ModoNocturno, this.btnOff);
        VistaLogin.Apagar += this.MostrarApagar_Salir;
        VistaLogin.Error += this.MostrarError;
        VistaLogin.Config += this.SiguientePantallaConfiguracion;
        VistaLogin.LoginOK += this.LoginOK;
        VistaLogin.Ubicacion += this.GetlocationPant;

        VistaLogin.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020

        VistaReproductor = new frmReproductor(this.ModoPrueba);

        //Asignamos la posición al reproductor
        VistaReproductor.Location = GetlocationPantSec();
    }

    /// <summary>
    /// Se encarga de recopilar los datos de las pantallas
    /// que hay en el dispositivo
    /// </summary>
    private void ObtenerPantallas()
    {
        //Recolectamos las pantallas
        foreach (var screen in Screen.AllScreens)
        {
            Pantalla nuevaPantalla;

            nuevaPantalla = new Pantalla();

            nuevaPantalla.Nombre = screen.DeviceName;
            nuevaPantalla.X = screen.Bounds.X;
            nuevaPantalla.Y = screen.Bounds.Y;
            nuevaPantalla.Width = screen.Bounds.Width;
            nuevaPantalla.Height = screen.Bounds.Height;
            nuevaPantalla.Primary = screen.Primary;

            ListaPantallas.Add(nuevaPantalla);
        }
    }

    /// <summary>
    /// fuera de servicio
    /// </summary>
    private void AcomodarPantallas()
    {
        //Valdidamos si traemos dos pantallas

        if (ListaPantallas.Count > 1)
        {
            //Validamos quien es la pantalla del equipo
            var Pant1 = ListaPantallas.Where(x => x.Nombre.Contains("1")).ToList();
            foreach (var pantalla in Pant1)
            {
                var MyPant = (Pantalla)pantalla;

                MyPant.Primary = true;

                VistaSistemas.Location = new Point(MyPant.X, MyPant.Y);
            }

            if (this.VMD)
            {
                var Pant2 = ListaPantallas.Where(x => x.Nombre.Contains("2")).ToList();

                foreach (var pantalla in Pant2)
                {
                    var MyPant = (Pantalla)pantalla;
                    VistaReproductor.Location = new Point(MyPant.X + 1, 0);
                }
            }
        }
        else//No debería de suceder si traermos VMD
        {
            if (this.VMD)
            {
                VistaReproductor.Location = new Point(0, 0);
            }
        }
    }

    /// <summary>
    /// Se encarga de obtener el punto de locación para
    /// dibujar las ventanas
    /// </summary>
    /// <returns></returns>
    private Point GetlocationPant()
    {
        Point nuevoPunto = new Point(0, 0);

        //Sólo si tengo más de dos pantallas
        if (ListaPantallas.Count > 1)
        {
            var Pant = ListaPantallas.Where(x => x.Primary).ToList();
            foreach (var pantalla in Pant)
            {
                var MyPant = (Pantalla)pantalla;
                nuevoPunto = new Point(MyPant.X, MyPant.Y);
            }
        }

        return nuevoPunto;
    }

    /// <summary>
    /// Se encarga de obtener el punto de locación para
    /// dibujar la pantalla de VMD
    /// </summary>
    /// <returns></returns>
    private Point GetlocationPantSec()
    {
        Point nuevoPunto = new Point(0, 0);

        if (ListaPantallas.Count > 1)
        {
            var Pant = ListaPantallas.Where(x => !x.Primary).ToList();
            foreach (var pantalla in Pant)
            {
                var MyPant = (Pantalla)pantalla;
                nuevoPunto = new Point(MyPant.X + 1, 0);
                AjustarTamañañoReproductor(MyPant.Width, MyPant.Height);
            }
        }
        return nuevoPunto;
    }

    /// <summary>
    /// Se encarga de ajustar la ventana del reproductor
    /// a la pantalla secundaria
    /// </summary>
    /// <param name="Width"></param>
    /// <param name="Height"></param>
    private void AjustarTamañañoReproductor(int Width, int Height)
    {
        //Powered ByRED 24MAR2021
        TamanoPantSec = new Size(Width, Height);

        //VistaReproductor.Size = new Size(Width, Height);
        //if (!ModoConfig)
        //{
        //    VistaReproductor.vlcControl1.Size = new Size(Width, Height);
        //}
        //VistaReproductor.BackImage.Size = new Size(Width, Height);
        //VistaReproductor.pictureBox1.Size = new Size(Width, Height);

        VistaReproductor.Size = TamanoPantSec;
        if (!ModoConfig)
        {
            VistaReproductor.vlcControl1.Size = TamanoPantSec;
        }
        VistaReproductor.BackImage.Size = TamanoPantSec;
        VistaReproductor.pictureBox1.Size = TamanoPantSec;
        VistaReproductor.TamañoPantalla = TamanoPantSec;
    }

    /// <summary>
    /// Se encarga de configurar los timers antes de lanzarlos
    /// </summary>
    private void ConfigurarTimers()
    {
        if (this.CONDUSAT)
        {
            //Para actualizar la vista de CONDUSAT
            timerActualizaCondusat.Interval = 500;
            timerActualizaCondusat.Enabled = true;
            timerActualizaCondusat.Tick += new EventHandler(timerActualizarCONDUSAT_Tick);
        }

        if (this.CAN)
        {
            //Para actualizar la vista de CAN
            timerActualizaCAN.Interval = 500;
            timerActualizaCAN.Enabled = true;
            timerActualizaCAN.Tick += new EventHandler(timerActualizarCAN_Tick);

            //Para actualizar la vista encabezado del Front Sistemas
            timerActualizaEncabezado.Interval = 500;
            timerActualizaEncabezado.Enabled = true;
            timerActualizaEncabezado.Tick += new EventHandler(timerActualizarEncabezado_Tick);

        }

        if (this.VMD)
        {
            //Para Verificar la posición del form del reproductor
            timerVerificaReproductor.Interval = 30000;
            timerVerificaReproductor.Enabled = true;
            timerVerificaReproductor.Tick += new EventHandler(timerVerificaReproductor_Tick);

            //Para actualizar la vista de VMD //Powered ByRED 20JUN2020
            timerActualizaVMD.Interval = 500;
            timerActualizaVMD.Enabled = true;
            timerActualizaVMD.Tick += new EventHandler(timerActualizarVMD_Tick);
        }

        if (this.SIA || this.MINISIA)//Powered ByRED 29MAR2021
        {
            timerActualizaSIA.Interval = 5000;
            timerActualizaSIA.Enabled = true;
            timerActualizaSIA.Tick += new EventHandler(timerActualizarSIA_Tick);
        }

        //timers de render
        ConfigTimersRender();
    }

    /// <summary>
    /// Se encarga de configurar los timers para el render
    /// de la aplicación (retraso y siempre adelante)
    /// se ocupa en modo de lanzamiento normal y en el de
    /// configuración
    /// </summary>
    private void ConfigTimersRender()
    {
        //Para retrasar la aparicion de la vista
        timerMuestraFront.Interval = 100;
        timerMuestraFront.Enabled = true;
        timerMuestraFront.Tick += new EventHandler(timerMuestraFront_Tick);

        //En caso de no estar en modo prueba, para tener siempre adelante el form
        timerSiempreAdelante.Interval = 100;
        timerSiempreAdelante.Enabled = false;
        timerSiempreAdelante.Tick += new EventHandler(timerSiempreAdelante_Tick);
    }

    /// <summary>
    /// Se encarga de iniciar todos los timers
    /// </summary>
    private void IniciaTimers()
    {
        //Para actualizar el encabezado de la vista del Front Sistemas
        timerActualizaEncabezado.Start();

        //Para actualizar la vista de CONDUSAT
        timerActualizaCondusat.Start();

        //Para actualizar la vista de CAN
        timerActualizaCAN.Start();

        //Para actualizar la vista de VMD
        timerVerificaReproductor.Start();
        timerActualizaVMD.Start(); //Powered ByRED 19MAR2021

        //Para actualizar la vista de SIA
        timerActualizaSIA.Start(); //Powered ByRED 19MAR2021


        //Para actualizar el pie de pagina de la vista del Front Sistemas
    }

    /// <summary>
    /// Se encarga de buscar el form por medio de un nombre
    /// para poder acceder a sus propiedades comunes
    /// Funciona como un catalogo de Forms, es necesario de de alta cualquier FORM nuevo
    /// para poder acceder a sus propiedades
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    private Form ObtenerForm(string nombre)
    {
        switch (nombre)
        {
            case "frmSistemas":
                return VistaSistemas;

            case "frmMenu":
                return VistaMenuCAN;

            case "frmError":
                return VistaError;

            case "frmViaje":
                return VistaViaje;

            case "frmClvPoblacion":
                return VistaPoblacion;

            case "frmPopUp":
                return VistaPopUp;

            case "frmMovtosCAN":
                return VistaMovtos;

            case "frmSync":
                return VistaSync;

            case "frmLogin":
                return VistaLogin;

            case "frmSalida":
                return VistaSalida;

            case "frmReproductor":
                return VistaReproductor;

            case "frmConfigAutobus":
                return ConfigAutobus;

            case "frmConfiguracion":
                return ConfigSistemas;

            case "frmMenuTVE":
                return VistaMenuTVE;

            case "frmTransferTVE":
                return VistaTransferTVE;

            case "frmStatusTVE":
                return VistaStatusTVE;

            case "frmQRTVE":
                return VistaQRTVE;

            case "frmLoginTVE":
                return VistaLoginTVE;

            case "frmDatosTelemetria":
                return VistaDatosTelemetria; //Powered ByRED15JUN2020

            case "frmHerramientasVMD":
                return VistaHerramientasVMD; //Powered ByRED 16JUN2020

            case "frmMenuSpots":
                return VistaMenuSpots; //Powered ByToto ENERO 2023

            case "frmCargadorDePautas":
                return VistaCargadorPauta; // Powered ByRED 16JUN2020
            case "frmSpots":
                return VistaSpots; // Powered ByRED 16JUN2020

            case "frmPanelMensajes":
                return VistaMensajesSIA;//Powered ByRED 17MAR2021

            case "frmMostrarMensaje":
                return VistaMostrarSMS;//Powered ByRED 19MAR2021

            case "frmEnviandoMensaje":
                return VistaMensajeEnviado; //Powered ByRED 19MAR2021

            default: return new Form();
        }
    }

    /// <summary>
    /// Se encarga de obtener el estado de la actividad del form
    /// solicitado
    /// </summary>
    /// <param name="nombre"></param>
    /// <returns></returns>
    private bool ObtenerActividad(string nombre)
    {
        switch (nombre)
        {
            case "frmSistemas":
                return VistaSistemas.VerificaActividad(TiempoCierreVentanas);

            case "frmMenu":
                return VistaMenuCAN.VerificaActividad(TiempoCierreVentanas);

            case "frmError":
                return VistaError.VerificaActividad(TiempoCierreVentanas);

            case "frmViaje":
                return VistaViaje.VerificaActividad(TiempoCierreVentanas);

            case "frmClvPoblacion":
                return VistaPoblacion.VerificaActividad(TiempoCierreVentanas);

            case "frmPopUp":
                return VistaPopUp.VerificaActividad(TiempoCierreVentanas);

            case "frmMovtosCAN":
                return VistaMovtos.VerificaActividad(TiempoCierreVentanas);

            case "frmSalida":
                return VistaSalida.VerificaActividad(TiempoCierreVentanas);

            case "frmSync":
                return VistaSync.VerificaActividad(TiempoCierreVentanas);

            case "frmHerramientasVMD":
                return VistaHerramientasVMD.VerificaActividad(TiempoCierreVentanas);

            case "frmMenuSpots":
                return VistaMenuSpots.VerificaActividad(TiempoCierreVentanas);

            case "frmSpots":
                return VistaSpots.VerificaActividad(TiempoCierreVentanas);


            default: return true; //Si no enlistamos algun Form Aquí es porque no queremos que se cierre en automatico

        }
    }

    /// <summary>
    /// Se encarga de poner el foco en una vista dada
    /// </summary>
    /// <param name="vista"></param>
    private void FocusON(Form vista)
    {
        vista.Show();

        //vista.SendToBack();

        //Nueva lógica

        timerSiempreAdelante.Stop();

        this.VistaAnterior = ListaVistas.LastOrDefault();
        ListaVistas.Add(vista.Name);

        #region "Quitamos ésta logica"

        //Le ponemos el foco al nuevo form y almacenamos el anterior
        //this.VistaAnterior = this.VistaActual;
        //this.VistaActual = vista.Name;

        #endregion

        timerMuestraFront.Start();
    }

    /// <summary>
    /// Se encarga de darle un retraso a la aparicion del form
    /// para esperar a que cargue completamente en segundo plano
    /// y no en primer foco y no se vea un efecto de lentitud.
    /// </summary>
    private void RetrasoFocus()
    {

        timerSiempreAdelante.Start();

        //le quitamos el foco
        if (VistaAnterior != null && !VistaAnterior.Equals(""))
        {
            ObtenerForm(this.VistaAnterior).TopMost = false;
        }

        //Le asignamos la opacidad al 100 %
        ObtenerForm(ListaVistas.LastOrDefault()).Opacity = 1.00;


        //ObtenerForm(this.VistaActual).TopMost = true;

        ////Le quitamos el foco al form anterior
        //if (!VistaAnterior.Equals(""))
        //{
        //    ObtenerForm(this.VistaAnterior).TopMost = false;
        //}

        //this.VistaAnterior = VistaActual;
    }

    /// <summary>
    /// Se encarga se cerrar el form que le mandemos
    /// </summary>
    /// <param name="Vista"></param>
    private void CerrarForm(Form Vista)
    {
        //Quitamos la vista de la lista para quitarle el foco
        //ListaVistas.Remove(ListaVistas.LastOrDefault());

        //prueba
        //Tenemos que quitar el nombre de la vista que se está cerrando
        ListaVistas.Remove(Vista.Name);

        //verificamos si la siguiente es una ventana emergente
        //Para reiniciar la actividad
        ReiniciarActividad(ListaVistas.LastOrDefault());

        if (Vista != null || Vista.IsDisposed != true)
        {
            Vista.Hide();
            Vista.Close();
            Vista.Dispose();
        }
        ////Le devolvemos el foco al form anterior
        //if (!this.VistaAnterior.Equals(""))
        //{
        //    ObtenerForm(VistaAnterior).TopMost = true;
        //    VistaActual = VistaAnterior;
        //}        
    }

    /// <summary>
    /// Se encarga se cerrar el form de mostrar SMS sia,
    /// para evitar que se abrá otra ventana si llega otro sms
    /// Powered ByRED 19MAR2021
    /// </summary>
    /// <param name="Vista"></param>
    private void CerrarFormMostrarSMS(Form Vista)
    {
        //Quitamos la vista de la lista para quitarle el foco
        //ListaVistas.Remove(ListaVistas.LastOrDefault());

        //prueba
        //Tenemos que quitar el nombre de la vista que se está cerrando
        ListaVistas.Remove(Vista.Name);

        //verificamos si la siguiente es una ventana emergente
        //Para reiniciar la actividad
        ReiniciarActividad(ListaVistas.LastOrDefault());

        if (Vista != null || Vista.IsDisposed != true)
        {
            Vista.Hide();
            Vista.Close();
            Vista.Dispose();
        }

        this.MostrandoMensajeSIA = false;
    }

    /// <summary>
    ///Sirve para reinicar el flag de actividad de las vistas
    ///para evitar su cierre automatico cuando estén en 
    ///cascada
    /// </summary>
    /// <param name="vista"></param>
    private void ReiniciarActividad(string nombre)
    {
        switch (nombre)
        {
            case "frmMenu":
                VistaMenuCAN.ReiniciaActividad();
                break;

            case "frmError":
                VistaError.ReiniciaActividad();
                break;

            case "frmViaje":
                VistaViaje.ReiniciaActividad();
                break;

            case "frmClvPoblacion":
                VistaPoblacion.ReiniciaActividad();
                break;

            case "frmPopUp":
                VistaPopUp.ReiniciaActividad();
                break;

            case "frmMovtosCAN":
                VistaMovtos.ReiniciaActividad();
                break;

            case "frmSalida":
                VistaSalida.ReiniciaActividad();
                break;

            case "frmHerramientasVMD":
                VistaHerramientasVMD.ReiniciaActividad();
                break;
            case "frmMenuSpots":
                VistaMenuSpots.ReiniciaActividad();
                break;

            default: return;
        }
        //var existe = (from x in ListaVistasActividad)

        //var existe1 = ListaVistasActividad.FirstOrDefault(x => x.Equals(vista.Name));

        //if (existe1 != null)
        //{
        //    //chido weh
        //}
    }

    /// <summary>
    /// Sirve para activar el modo Nocturno en cada una de las vistas
    /// </summary>
    /// <param name="nombreVista"></param>
    public void ActivarModoNocturno(bool Activar)
    {
        foreach (string Vista in ListaVistas)
        {
            switch (Vista)
            {
                case "frmSistemas":
                    VistaSistemas.ActivarModonocturno(Activar);
                    break;

                case "frmMenu":
                    VistaMenuCAN.ActivarModonocturno(Activar);
                    break;

                case "frmError":
                    VistaError.ActivarModonocturno(Activar);
                    break;

                case "frmViaje":
                    VistaViaje.ActivarModonocturno(Activar);
                    break;

                case "frmClvPoblacion":
                    VistaPoblacion.ActivarModonocturno(Activar);
                    break;

                case "frmPopUp":
                    VistaPopUp.ActivarModonocturno(Activar);
                    break;

                case "frmMovtosCAN":
                    VistaMovtos.ActivarModonocturno(Activar);
                    break;

                case "frmSync":
                    VistaSync.ActivarModonocturno(Activar);
                    break;

                case "frmLogin":
                    VistaLogin.ActivarModonocturno(Activar);
                    break;

                case "frmSalida":
                    VistaSalida.ActivarModonocturno(Activar);
                    break;

                case "frmReproductor":
                    break;

                case "frmConfigAutobus":
                    ConfigAutobus.ActivarModonocturno(Activar);
                    break;

                case "frmConfiguracion":
                    ConfigSistemas.ActivarModonocturno(Activar);
                    break;

                case "frmMenuTVE":
                    VistaMenuTVE.ActivarModonocturno(Activar);
                    break;

                case "frmTransferTVE":
                    VistaTransferTVE.ActivarModonocturno(Activar);
                    break;

                case "frmStatusTVE":
                    VistaStatusTVE.ActivarModonocturno(Activar);
                    break;

                case "frmQRTVE":
                    VistaQRTVE.ActivarModonocturno(Activar);
                    break;

                case "frmLoginTVE":
                    VistaLoginTVE.ActivarModonocturno(Activar);
                    break;

                //PoweredByRED 15JUN2020
                case "frmDatosTelemetria":
                    VistaDatosTelemetria.ActivarModonocturno(Activar);
                    break;

                //PoweredByRED 16JUN2020
                case "frmHerramientasVMD":
                    VistaHerramientasVMD.ActivarModonocturno(Activar);
                    break;
                //PoweredByToto enero2023
                case "frmMenuSpots":
                    VistaMenuSpots.ActivarModonocturno(Activar);
                    break;

                //PoweredByRED 16JUN2020
                case "frmCargadorDePauta":
                    VistaCargadorPauta.ActivarModonocturno(Activar);
                    break;
                //PoweredByTOTO ENERO2023
                case "frmSpots":
                    VistaSpots.ActivarModonocturno(Activar);
                    break;

                //Powered ByRED 17MAR2021
                case "frmPanelMensajes":
                    VistaMensajesSIA.ActivarModonocturno(Activar);
                    break;

                //Powered ByRED 19MAR2021
                case "frmMostrarMensaje":
                    VistaMostrarSMS.ActivarModonocturno(Activar);
                    break;

                //Powered ByRED 19MAR2021
                case "frmEnviandoMensaje":
                    VistaMensajeEnviado.ActivarModonocturno(Activar);
                    break;

                default: break;
            }
        }
    }

    /// <summary>
    /// Ejecuta el evento que avisará a SAM que debe apagar el sistema
    /// </summary>
    private void ApagarSistema()
    {
        //Si tenemos VMD
        if (VMD)
        {
            if (VistaReproductor.EnReproduccion())
            {//Si estamos reproducciendo algo, mandamos a detener la pelicula
                this.Func_DetenerPelicula();
            }
            VistaReproductor.Close();
            //VistaReproductor.Dispose();
        }

        //Powered ByRED 21JUL2021
        //CerrarForm(VistaSistemas);
        SAMCerrar();
        CerrarForm(VistaSistemas);
        Application.ExitThread();
    }

    /// <summary>
    /// Manda apagar el sistema en modoConfig
    /// </summary>
    private void ApagarSistemaConfig()
    {
        SAMCerrarConfig();
        //Application.ExitThread();
    }

    /// <summary>
    /// Ejecuta el evento que avisará a SAM que debe de cerrar el sistema
    /// </summary>
    private void CerrarSistema()
    {
        //Si tenemos VMD
        if (VMD)
        {
            if (VistaReproductor.EnReproduccion())
            {//Si estamos reproducciendo algo, mandamos a detener la pelicula
                this.Func_DetenerPelicula();
            }
            VistaReproductor.Close();
            VistaReproductor.Dispose();
        }
        CerrarForm(VistaSistemas);
        SAMCerrar(true);
        //Por si las moscas, mostramos la barra de tareas
        BarraDeTareas(true);
        Application.ExitThread();
    }

    /// <summary>
    /// se encarga de configurar la pantalla secundaria para mostrar un PI de SIA
    /// Powered ByRED 29MAR2021
    /// </summary>
    private void MostrarPISIA(bool Activar)
    {

        if (Activar)
        {
            if (this.PISIA.Count == 3)
            {
                //Flaggeamos que estamos mostrando PI
                this.MostrandoPISIA = true;

                VistaReproductor.AudioTemp = VistaReproductor.AudioInicial;
                VistaReproductor.AudioInicial = 0;
                VistaReproductor.ReproducirMP3(this.PISIA.ElementAt(0));
                VistaReproductor.vlcControl1.Audio.Volume = VistaReproductor.AudioInicial;
                VistaReproductor.vlcControl1.Size = new Size(CalcularFactor(TamanoPantSec.Width, 50), CalcularFactor(TamanoPantSec.Height, 50));
                VistaReproductor.vlcControl1.Location = new Point(CalcularFactor(TamanoPantSec.Width, 5), CalcularFactor(TamanoPantSec.Height, 21));

                VistaReproductor.BackgroundImage = Resources.FondoPISIA;
                VistaReproductor.pictureBox1.Visible = false;
                var ruta = this.PISIA.ElementAt(1);
                VistaReproductor.img_PISIA.BackgroundImage = Image.FromFile(@ruta);
                VistaReproductor.img_PISIA.Location = new Point(CalcularFactor(TamanoPantSec.Width, 63), CalcularFactor(TamanoPantSec.Height, 21));
                VistaReproductor.img_PISIA.Visible = true;
                VistaReproductor.Refresh();
                //Configuramos el timer
                timerTerminaPISIA = new System.Windows.Forms.Timer();
                timerTerminaPISIA.Interval = (Convert.ToInt32(this.PISIA.ElementAt(2)) * 1000);
                timerTerminaPISIA.Enabled = false;
                timerTerminaPISIA.Tick += new EventHandler(timerTerminaPISIA_Tick);
                //Empezamos el timer
                timerTerminaPISIA.Start();
                this.PISIA.Clear();
            }
        }
        else
        {

            VistaReproductor.AudioInicial = VistaReproductor.AudioTemp;
            VistaReproductor.AudioTemp = 0;
            VistaReproductor.vlcControl1.Audio.Volume = VistaReproductor.AudioInicial;
            VistaReproductor.vlcControl1.Location = new Point(0, 0);
            VistaReproductor.vlcControl1.Size = TamanoPantSec;

            VistaReproductor.pictureBox1.Visible = true;
            VistaReproductor.BackgroundImage = null;
            VistaReproductor.img_PISIA.Visible = false;
            VistaReproductor.img_PISIA.BackgroundImage = null;

            //Flaggeamos que estamos dejando de mostrar PI
            this.MostrandoPISIA = false;
        }
    }

    /// <summary>
    /// Se encarga de calcular el factor para obtener el valor de redimensión
    /// del recurso visual
    /// Powered ByRED 30MAR2021
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private int CalcularFactor(int input, int factor)
    {
        int retorno = 0;
        try
        {
            double dou = ((factor * input) / 100);

            retorno = Convert.ToInt32(Math.Truncate(dou));
        }
        catch
        {
            retorno = input;
        }
        return retorno;
    }

    /// <summary>
    /// Se encarga de redibujar el recurso de imagen de GPS
    /// Powered ByRED 27MAY2021
    /// </summary>
    private void LedGPSHorarioNocturno()
    {
        RecibeLedDeGPS(this.EstadoGPS, true);
    }

    /// <summary>
    /// Se encarga de enviar la alerta de Robo hacia SIA
    /// Powered ByRED 08JUN2021
    /// </summary>
    private void EnviarAlertaRobo()
    {
        AlertaRobo();
    }
    #endregion

    #region "Manejador de Eventos SAM"
    /// <summary>
    /// Recibe el valor con el que tiene que pintar el indicador de gps
    /// </summary>
    /// <param name="activo"></param>
    public void RecibeLedDeGPS(int Estado, bool HorarioNocturno)
    {
        if (!HorarioNocturno) { EstadoGPS = Estado; }

        switch (Estado)
        {
            case -1:
                if (ModoNocturno)
                {
                    VistaSistemas.ledGPS.BackColor = Color.Black;
                    VistaSistemas.ledGPS.Image = Resources.gps_rojoNoc;
                }
                else
                {
                    VistaSistemas.ledGPS.BackColor = Color.Transparent;
                    VistaSistemas.ledGPS.Image = Resources.gps_rojo;
                }
                break;

            case 0:
                if (ModoNocturno)
                {
                    VistaSistemas.ledGPS.BackColor = Color.Black;
                    VistaSistemas.ledGPS.Image = Resources.gps_amarilloNoc;
                }
                else
                {
                    VistaSistemas.ledGPS.BackColor = Color.Transparent;
                    VistaSistemas.ledGPS.Image = Resources.gps_amarillo;
                }
                break;

            case 1:
                if (ModoNocturno)
                {
                    VistaSistemas.ledGPS.BackColor = Color.Black;
                    VistaSistemas.ledGPS.Image = Resources.gps_naranjaNoc;
                }
                else
                {
                    VistaSistemas.ledGPS.BackColor = Color.Transparent;
                    VistaSistemas.ledGPS.Image = Resources.gps_naranja;
                }
                break;

            case 2:
                if (ModoNocturno)
                {
                    VistaSistemas.ledGPS.BackColor = Color.Black;
                    VistaSistemas.ledGPS.Image = Resources.gps_verdeNoc;
                }
                else
                {
                    VistaSistemas.ledGPS.BackColor = Color.Transparent;
                    VistaSistemas.ledGPS.Image = Resources.gps_verde;
                }
                break;
        }

        #region "Logica anterior GPS"
        ////si activo viene en true, significa que debe de pintar verde, de lo contrario, debe de pintar rojo
        //if (activo)
        //{//Si tenemos que pintar en verde
        //    if (GPSActivo != true)
        //    {
        //        GPSActivo = true;

        //        if (ModoNocturno)
        //        {
        //            VistaSistemas.ledGPS.BackColor = Color.Black;
        //            VistaSistemas.ledGPS.Image = Resources.gps_verdeNoc;
        //        }
        //        else
        //        {
        //            VistaSistemas.ledGPS.BackColor = Color.Transparent;
        //            VistaSistemas.ledGPS.Image = Resources.gps_verde;
        //        }
        //    }
        //}
        //else
        //{// si tenemos que pintar rojo
        //    if (GPSActivo != false)
        //    {
        //        GPSActivo = false;

        //        if (ModoNocturno)
        //        {
        //            VistaSistemas.ledGPS.BackColor = Color.Black;
        //            VistaSistemas.ledGPS.Image = Resources.gps_rojoNoc;
        //        }
        //        else
        //        {
        //            VistaSistemas.ledGPS.BackColor = Color.Transparent;
        //            VistaSistemas.ledGPS.Image = Resources.gps_rojo;
        //        }
        //    }
        //}
        #endregion
    }

    /// <summary>
    /// Recibe los datos de Condusat para pintarlos en el front
    /// </summary>
    /// <param name="ColorVel"></param>
    /// <param name="Vel_Real"></param>
    /// <param name="Vel_Max"></param>
    /// <param name="UsarCAN"></param>
    /// <param name="Param_ADOCAN"></param>
    /// <param name="ModoNocturno"></param>
    public void RecibeDeCondusat(string ColorVel, int Vel_Real, int Vel_Max, bool UsarCAN, bool Param_ADOCAN, string imgAdvertencia)
    {
        if (!UsarCAN || Param_ADOCAN)
        {
            VistaSistemas.lblVReal.Text = Vel_Real.ToString();
        }

        VistaSistemas.lblVMax.Text = Vel_Max.ToString();


        //mostramos la imagen que viene de condusat
        try
        {
            if (!imgAdvertencia.Equals(""))
            {
                //Obtenemos la imagen
                var imgAdver = ObtieneAlertaCondusat(imgAdvertencia);

                //Si no está vacia entonces se la asignamos a la imagen para mostrar
                if (imgAdver != null)
                {
                    VistaSistemas.imgAlertaCondusat.BackgroundImage = imgAdver;
                    VistaSistemas.imgAlertaCondusat.Visible = true;
                }
            }
            else
            {
                if (VistaSistemas.imgAlertaCondusat.Visible)
                {
                    VistaSistemas.imgAlertaCondusat.Visible = false;
                    VistaSistemas.imgAlertaCondusat.BackgroundImage = null;
                }
            }
        }
        catch
        {
            VistaSistemas.imgAlertaCondusat.Visible = false;
            VistaSistemas.imgAlertaCondusat.BackgroundImage = null;
        }

        ///Cambiamos el semáforo de control de velocidad///

        if (ColorVel.Equals("Verde") || ColorVel.Equals(""))
        {

            if (this.ModoNocturno)
            {
                VistaSistemas.lblVReal.ForeColor = Color.Green;
            }
            else
            {
                VistaSistemas.lblVReal.ForeColor = Color.Black;
            }

        }
        else if (ColorVel.Equals("Amarillo"))
        {
            if (Convert.ToInt32(VistaSistemas.lblVMax.Text.Split(' ').First()) < Convert.ToInt32(VistaSistemas.lblVReal.Text.Split(' ').First()))
            {
                VistaSistemas.lblVReal.ForeColor = Color.Yellow;

                //if (this.ModoNocturno)
                //{
                //    VistaSistemas.lblVReal.ForeColor = Color.Yellow;
                //}
                //else
                //{
                //    VistaSistemas.lblVReal.ForeColor = Color.Yellow;
                //}
            }
            else
            {
                if (this.ModoNocturno)
                {
                    VistaSistemas.lblVReal.ForeColor = Color.Green;
                }
                else
                {
                    VistaSistemas.lblVReal.ForeColor = Color.Black;
                }
            }

        }
        else if (ColorVel.Equals("Rojo"))
        {
            if (Convert.ToInt32(VistaSistemas.lblVMax.Text.Split(' ').First()) < Convert.ToInt32(VistaSistemas.lblVReal.Text.Split(' ').First()))
            {

                VistaSistemas.lblVReal.ForeColor = Color.Red;

                //if (this.ModoNocturno)
                //{
                //    VistaSistemas.lblVReal.ForeColor = Color.Red;
                //}
                //else
                //{
                //    VistaSistemas.lblVReal.ForeColor = Color.Red;
                //}
            }
            else
            {
                if (this.ModoNocturno)
                {
                    VistaSistemas.lblVReal.ForeColor = Color.Green;
                }
                else
                {
                    VistaSistemas.lblVReal.ForeColor = Color.Black;
                }
            }
        }
    }

    /// <summary>
    /// Se encarga de 
    /// </summary>
    /// <param name="nombreimagen"></param>
    /// <returns></returns>
    private Image ObtieneAlertaCondusat(string nombreimagen)
    {
        switch (nombreimagen)
        {
            case "Advertencia":

                if (ModoNocturno)
                {
                    return Resources.AdvertenciaNoc;
                }
                else
                {
                    return Resources.Advertencia;
                }

            case "Alto":

                if (ModoNocturno)
                {
                    return Resources.AdvertenciaNoc;
                }
                else
                {
                    return Resources.Advertencia;
                }

            case "cruceropeligroso":
                if (ModoNocturno)
                {
                    return Resources.cruceropeligrosoNoc;
                }
                else
                {
                    return Resources.cruceropeligroso;
                }

            case "curvapeligrosa":
                if (ModoNocturno)
                {
                    return Resources.curvapeligrosaNoc;
                }
                else
                {
                    return Resources.curvapeligrosa;
                }

            case "Ferrocarril":
                if (ModoNocturno)
                {
                    return Resources.FerrocarrilNoc;
                }
                else
                {
                    return Resources.Ferrocarril;
                }

            case "PasoPeatonal":
                if (ModoNocturno)
                {
                    return Resources.PasoPeatonalNoc;
                }
                else
                {
                    return Resources.PasoPeatonal;
                }

            case "Semaforo":
                if (ModoNocturno)
                {
                    return Resources.SemaforoNoc;
                }
                else
                {
                    return Resources.Semaforo;
                }


            case "topes":
                if (ModoNocturno)
                {
                    return Resources.topesNoc;
                }
                else
                {
                    return Resources.topes;
                }

            case "Warning":
                if (ModoNocturno)
                {
                    return Resources.AdvertenciaNoc;
                }
                else
                {
                    return Resources.Advertencia;
                }

            default:
                return null;
        }
    }

    /// <summary>
    /// Recibe un mensaje que se deba de mostrar en la pantalla de SYNC
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="final"></param>
    public void EventoSync(string _mensaje, int _final)
    {
        if (VistaSync != null && VistaSync.IsDisposed != false)
        {

        }
        VistaSync.EventoSync(_mensaje, _final);
    }

    /// <summary>
    /// Se encarga de comunicarle al Front que debe de terminar la SYNC
    /// </summary>
    public void SyncOK(bool Exitoso)
    {
        if (VistaSync != null && VistaSync.IsDisposed != false)
        {

        }
        VistaSync.SyncOK(Exitoso);
    }
    /// <summary>
    /// Se encarga de recibir un viaje que ya exista en CAN
    /// Se ocupa principalmente al inicio de la aplicación
    /// para recuperar un viaje sin cerrar
    /// </summary>
    /// <param name="operador"></param>
    /// <param name="nom_operador"></param>
    public void RecuperarViaje(string operador, string nom_operador)
    {
        this.EnViaje = true;
        this.ClvConductor = operador;
        this.NomConductor = nom_operador;

        DatosPieDePagina();
    }

    /// <summary>
    /// Se encarga de recibir un mensaje que debe de ser mostrado
    /// al conductor como una alerta
    /// </summary>
    /// <param name="Alerta"></param>
    public void RecibeAlertaSAM(string Alerta)
    {
        MostrarError(Alerta);
    }


    /// <summary>
    /// Recibe el valor con el que tiene que pintar el indicador de Telemetria
    /// </summary>
    /// <param name="activo"></param>
    public void RecibeLedDeTelemetria(int Estado)
    {
        switch (Estado)
        {
            case 0:
                //Fuera de línea
                if (ModoNocturno)
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Black;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_rojoNoc;
                }
                else
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Transparent;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_rojo;
                }
                break;

            case 1:
                //Encendido, funcionando
                if (ModoNocturno)
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Black;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_verdeNoc;
                }
                else
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Transparent;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_verde;
                }
                break;

            case 2:
                //Enviando Lote
                if (ModoNocturno)
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Black;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_naranjaNoc;
                }
                else
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Transparent;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_naranja;
                }
                break;

            case 3:
                //Enviando Falla
                if (ModoNocturno)
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Black;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_amarilloNoc;
                }
                else
                {
                    VistaSistemas.LedTelemetria.BackColor = Color.Transparent;
                    VistaSistemas.LedTelemetria.Image = Resources.telemetria_amarillo;
                }
                break;
        }
    }

    /// <summary>
    /// Recibe mensaje de sia para ser mostrado hacia el conductor o en el cintillo de VMD
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_tipo"></param>
    /// <param name="_texto"></param>
    public bool RecibeMensajeDeSIA(int _tipo, string _texto)
    {
        bool respuesta = false;

        if (this.InLogin)//Si no estoy loggeado, no muestro nada
        {
            switch (_tipo)
            {
                //1 Cintillo
                case 1:
                    /*if (!this.VMD) { respuesta = true; }*/ //Si no tenemos VMD activo (Pantalla Secundaria) no puedo mostrar nada Powered ByRED 30MAR2021
                    if (!this.MostrandoPISIA)
                    {
                        if (!this.MostrandoCintilloSIA)
                        {
                            Func_ReproducirCintillo(_texto);
                            respuesta = true;
                        }
                        else
                        {

                        }
                    }
                    break;

                //2 mensaje conductor
                default:
                    if (this.MensajeCONDUCTOR.Equals(""))
                    {
                        this.MensajeCONDUCTOR = _texto;
                        respuesta = true;
                    }
                    break;
            }
        }
        return respuesta;
    }

    /// <summary>
    /// Recibe el estado del internet desde SIA
    /// Powered ByRED 17MAR2021
    /// </summary>
    /// <param name="estado"></param>
    public void RecibeInternetSIA(int estado)
    {
        switch (estado)
        {

            case 1:

                VistaSistemas.lblStatusSIA.Text = "SEÑAL DE INTERNET LIMITADA";

                if (ModoNocturno)
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.medio_internetNoc;
                }
                else
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.medio_internet;
                }

                break;

            case 2:

                VistaSistemas.lblStatusSIA.Text = "SEÑAL DE INTERNET BUENA";

                if (ModoNocturno)
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.si_internetNoc;
                }
                else
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.si_internet;
                }

                break;

            default:

                VistaSistemas.lblStatusSIA.Text = "SIN SEÑAL DE INTERNET";

                if (ModoNocturno)
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.no_internetNoc;
                }
                else
                {
                    VistaSistemas.imgStatusInternet.BackgroundImage = Resources.no_internet;
                }

                break;
        }
    }

    /// <summary>
    /// Se encarga de recibir la multimedia necesaria para 
    /// poder mostrar el POI
    /// Powered ByRED 29MAR2021
    /// </summary>
    /// <param name="Multimedia"></param>
    /// <returns></returns>
    public bool RecibePOISIA(List<String> _multimedia)
    {
        bool respuesta = false;

        if (!this.VMD) { respuesta = true; } //Si no tenemos VMD activo (Pantalla Secundaria) no puedo mostrar nada Powered ByRED 30MAR2021

        if (this.InLogin)
        {
            if (!this.MostrandoCintilloSIA)//Powered ByRED 14ABR2021
            {
                if (this.PISIA.Count == 0)
                {
                    this.PISIA = _multimedia;
                    respuesta = true;
                }
            }
        }
        return respuesta;
    }

    #endregion

    #region "Manejador de Eventos Vistas"

    /// <summary>
    /// Se encarga de mostrar la pantalla de "carga"
    /// para esperar a que se carguen los programas satelites
    /// </summary>
    /// <param name="mostrar"></param>
    public void MostrarCargando(bool mostrar)
    {
        if (mostrar)
        {
            if (!this.ModoPrueba)
            {
                BarraDeTareas(false);
                BlockInput(true);


            }
            this.HiloCarga = new Thread(new ThreadStart(this.Cargando));
            HiloCarga.Start();
        }
        else
        {
            this.cargando = false;
            BlockInput(false);
        }
    }

    /// <summary>
    /// Se encarga de mostrar el form de cargando
    /// mientras carga las aplciaciones
    /// </summary>
    public void Cargando()
    {
        if (VistaCarga == null || VistaCarga.IsDisposed)
        {
            VistaCarga = new frmCarga();
            VistaCarga.Cerrar += this.flagcarga;
            VistaCarga.Ubicacion += this.GetlocationPant;
        }

        try
        {
            Application.Run(VistaCarga);
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se ocupa como una propiedad
    /// regresa el valor de la carga
    /// </summary>
    /// <returns></returns>
    public bool flagcarga()
    {
        return this.cargando;
    }

    /// <summary>
    /// Se encarga de flagear que se terminó
    /// la carga
    /// </summary>
    public void TerminarCarga()
    {
        this.cargando = false;
    }

    /// <summary>
    /// Se encarga de regresar el valor
    /// para que la vista de Menú sepa si nos encontramos
    /// sincronizando
    /// </summary>
    /// <returns></returns>
    private bool EnSync()
    {
        return this.Sincronizando;
    }

    /// <summary>
    /// Se encarga de mostrar la vista de Login
    /// </summary>
    private void MostrarLogin()
    {
        //indicamos que no estamos Loggeados
        //Powered ByRED 19MAR2021
        this.InLogin = false;

        //Se inicializa el form de Login

        if (VistaLogin == null || VistaLogin.IsDisposed)
        {
            VistaLogin = new frmLogin(Pass, this.Version, ModoPrueba, this.ModoNocturno, this.btnOff);
            VistaLogin.Apagar += this.MostrarApagar_Salir;
            VistaLogin.Error += this.MostrarError;
            VistaLogin.Cerrar += this.CerrarForm;
            VistaLogin.LoginOK += this.LoginOK;
            VistaLogin.Ubicacion += this.GetlocationPant;

            VistaLogin.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020
        }

        FocusON(VistaLogin);
    }

    /// <summary>
    /// Se encarga de mostrar la vista de sistemas
    /// despues de un loginCorrecto
    /// </summary>
    private void LoginOK()
    {
        //VistaLogin.Hide();
        //VistaLogin.Close();
        //VistaLogin.Dispose();

        //this.VistaActual = "frmSistemas";

        CerrarForm(VistaLogin);

        ObtenerForm(ListaVistas.LastOrDefault()).WindowState = FormWindowState.Normal;
        //Powered ByRED 19MAR2021
        this.InLogin = true;

        //Mandamos a reproducir la pelicula si estaba reproduciendo algo: 10/Sep/2019
        InicializarRepro();
    }

    /// <summary>
    /// Proviene del MasterView, frmCAN
    /// Mostrará el menú que está dentro de CAN
    /// </summary>
    private void MostrarMenu()
    {
        //Si no existe, o fué destruido, creamos el form y le asignamos las propiedades necesarias
        if (VistaMenuCAN == null || VistaMenuCAN.IsDisposed)
        {
            VistaMenuCAN = new frmMenu(this.ModoPrueba, this.ModoNocturno, this.Version, this.TELEMETRIA);

            VistaMenuCAN.EnViaje += this.VerificaViaje; //Pregunta si hay viaje abierto
            VistaMenuCAN.MandaSync += this.MandarSyncSAM; // Manda instrucción a SAM para sincronizar
            VistaMenuCAN.MandaError += this.MostrarError; //Muestra un error
            VistaMenuCAN.MostrarViaje += this.MostrarViaje; // Lanza el front de VIAJE
            VistaMenuCAN.Protocolo += this.VerificaProtocolo; // Pregunta la existencia de protocolo de CAN
            VistaMenuCAN.Velocidad += this.VerificaVelocidad;

            VistaMenuCAN.DatosViaje += this.MostrarPopUp;//Para mostrar los datos de viaje
            VistaMenuCAN.DatosGPS += this.MostrarPopUp;//Para mostrar los datos de GPS
            VistaMenuCAN.DatosMovtos += this.MostrarMovtosCAN;//Para mostrar los datos de movtos CAN
            VistaMenuCAN.TVE += this.MostrarMenuTVE; //Para mostrar el menú de TVE
            VistaMenuCAN.TELEMETRIA += this.MostrarTelemetria; //Para mostrar el reporte de telemetria Powered ByRED 15JUN2020

            VistaMenuCAN.Cerrar += this.CerrarForm;//Para indicar a Front que debe cerrar el form
            VistaMenuCAN.MuestraSalir += this.MostrarApagar_Salir;//Para mandar a mostrar la vista de salir de la App
            VistaMenuCAN.EnSync += this.EnSync;//Se encarga de regresar el valor de la variable "Sincronizando"

            VistaMenuCAN.Ubicacion += this.GetlocationPant;//Se encarga de obtener la ubicación donde se deben de pintar los forms
        }

        //Mostramos la vista
        FocusON(VistaMenuCAN);

        //Añadimos a la lista de sistemas
        //ListaSistemas.Add(VistaMenuCAN.Name);
    }

    /// <summary>
    /// Proviene de frmMenu, se encarga de lanzar el form con las caracteristicas
    /// necesarias para su funcionamiento
    /// </summary>
    /// <param name="Tipo"></param>
    private void MostrarViaje(string Tipo)
    {
        ////Verificamos que no tengamos velocidad
        //if (Velocidad() > 0)
        //{//Si tenemos no dejamos abrir ni cerrar viaje
        //    MostrarError("La unidad debe de estar detenenida");
        //}
        //else
        //{si no tenemos, podremos acceder a un viaje o cerrarlo
        //}
        if (VistaViaje == null || VistaViaje.IsDisposed)
        {
            VistaViaje = new frmViaje(this.ModoPrueba, this.ModoNocturno, this.btnOff);
            VistaViaje.MuestraSalir += this.MostrarApagar_Salir;
            VistaViaje.Cerrar += this.CerrarForm;
            VistaViaje.Ubicacion += this.GetlocationPant;
        }

        switch (Tipo)
        {
            case "VA":
                VistaViaje.lblTitulo.Text = "Abrir Viaje";
                VistaViaje.Viaje += this.AbrirViaje;

                break;

            case "CM":
                VistaViaje.lblTitulo.Text = "Relevo";
                VistaViaje.Viaje += this.CambioManos;

                break;

            case "VC":
                VistaViaje.lblTitulo.Text = "Cerrar Viaje";
                VistaViaje.Viaje += this.CerrarViaje;

                break;
        }

        //Render
        FocusON(VistaViaje);
        //ListaSistemas.Add(VistaViaje.Name);
    }

    /// <summary>
    /// Proviene del administrador de viajes, se encarga de levantar
    /// el entorno grafico para introducir manualmente una clave de 
    /// poblacion
    /// </summary>
    /// <param name="Tipo"></param>
    /// <param name="clvConductor"></param>
    private void MostrarCLVPoblacion(string Tipo, string clvConductor)
    {
        if (VistaPoblacion == null || VistaPoblacion.IsDisposed)
        {
            VistaPoblacion = new frmClvPoblacion(Tipo, clvConductor, this.ModoPrueba, this.ModoNocturno);
        }

        VistaPoblacion.ChecaPob += this.VerificaPoblacion;
        VistaPoblacion.Viaje += this.MandarViaje;
        VistaPoblacion.MuestraSalir += this.MostrarApagar_Salir;
        VistaPoblacion.Cerrar += this.CerrarForm;
        VistaPoblacion.Ubicacion += this.GetlocationPant;

        //Render
        FocusON(VistaPoblacion);

        //ListaSistemas.Add(VistaPoblacion.Name);
    }

    /// <summary>
    /// Se encarga de levantar la ventana de informacion de
    /// viaje y de gps
    /// </summary>
    /// <param name="Tipo"></param>
    private void MostrarPopUp(string Tipo)
    {
        if (VistaPopUp == null || VistaPopUp.IsDisposed)
        {
            VistaPopUp = new frmPopUp(this.ModoPrueba, this.ModoNocturno);
        }

        switch (Tipo)
        {
            case "GPS":

                VistaPopUp.Datos += this.DatosGPS;

                break;

            case "VIAJE":

                VistaPopUp.Datos += this.DatosViaje;

                break;

            case "ESPERA1":

                VistaPopUp.Datos += this.DatosEspera_1;

                break;

            case "ESPERA2":
                VistaPopUp.Datos += this.DatosEspera_2;
                break;

            case "ESPERA3":
                VistaPopUp.Datos += this.DatosEspera_3;
                break;

            case "DatosPauta":
                VistaPopUp.Datos += this.DatosPauta;
                break;

            case "POI":
                VistaPopUp.Datos += this.DatosPOI;
                break;
        }

        VistaPopUp.Cerrar += this.CerrarForm;
        VistaPopUp.Ubicacion += this.GetlocationPant;


        VistaPopUp.ConfigurarForm(Tipo);

        FocusON(VistaPopUp);

        //ListaSistemas.Add(VistaPopUp.Name);
    }

    /// <summary>
    /// Se encarga de levantar la ventana de informacion de
    /// viaje y de gps cuando está en modo Configuración
    /// </summary>
    /// <param name="Tipo"></param>
    private void MostrarPopUp(object Tipo)
    {
        var _tipo = Tipo.ToString();

        if (VistaPopUp == null || VistaPopUp.IsDisposed)
        {
            VistaPopUp = new frmPopUp(this.ModoPrueba, this.ModoNocturno);
        }

        switch (_tipo)
        {
            case "ESPERA1":

                VistaPopUp.Datos += this.DatosEspera_1;

                break;

            case "ESPERA2":
                VistaPopUp.Datos += this.DatosEspera_2;

                break;

            case "ESPERA3":
                VistaPopUp.Datos += this.DatosEspera_3;

                break;
        }

        VistaPopUp.ConfigurarForm(_tipo);
        VistaPopUp.Cerrar += this.CerrarForm;
        VistaPopUp.Ubicacion += this.GetlocationPant;
        VistaPopUp.TopMost = true;

        Application.Run(VistaPopUp);

        //VistaPopUp.Show();
        //FocusON(VistaPopUp);
    }

    /// <summary>
    /// Se encarga de mostrar el form de apagar o de salir
    /// según sea necesario
    /// </summary>
    /// <param name="Tipo"></param>
    private void MostrarApagar_Salir(int Tipo)
    {
        if (VistaSalida == null || VistaSalida.IsDisposed)
        {
            switch (Tipo)
            {
                case 0:
                    VistaSalida = new frmSalida(Pass, Version, this.ModoPrueba, Tipo, this.ModoNocturno, this.ModoConfig);
                    break;

                case 1:
                    VistaSalida = new frmSalida(PassSalida, Version, this.ModoPrueba, Tipo, this.ModoNocturno, this.ModoConfig);
                    break;

                default:
                    VistaSalida = new frmSalida();
                    break;
            }
        }

        VistaSalida.Salir += this.CerrarSistema;
        VistaSalida.Apagar += this.ApagarSistema;
        VistaSalida.ApagarConfig += this.ApagarSistemaConfig;
        VistaSalida.Cerrar += this.CerrarForm;
        VistaSalida.Ubicacion += this.GetlocationPant;

        //Render
        FocusON(VistaSalida);

        //ListaSistemas.Add(VistaSalida.Name);

    }

    /// <summary>
    /// Manda a mostrar la vista de Login para simular un bloqueo de pantalla
    /// </summary>
    private void BloquearPantalla()
    {
        if (!(VerificaVelocidad() > 0) || this.ModoPrueba)
        {
            this.MostrarLogin();
        }
    }

    /// <summary>
    /// Manda a mostrar la vista de apagado, ya sea de cerrar sistema o apagar el sistema
    /// </summary>
    private void VistaCerrarSistema(int Tipo)
    {
        if (!(VerificaVelocidad() > 0) || this.ModoPrueba)
        {
            this.MostrarApagar_Salir(Tipo);
        }
    }

    /// <summary>
    /// Se encarga de levantar la ventana de informacion
    /// de los movimientos de can
    /// </summary>
    private void MostrarMovtosCAN()
    {
        if (VistaMovtos == null || VistaMovtos.IsDisposed)
        {
            VistaMovtos = new frmMovtosCAN(DatosMovtosSAM(), this.ModoPrueba, this.ModoNocturno);
            VistaMovtos.Cerrar += this.CerrarForm;
            VistaMovtos.Ubicacion += this.GetlocationPant;
        }

        //Render
        FocusON(VistaMovtos);

        //ListaSistemas.Add(VistaMovtos.Name);
    }

    /// <summary>
    /// Se encarga de mostrar el menú de TVE
    /// </summary>
    private void MostrarMenuTVE()
    {
        if (VistaMenuTVE == null || VistaMenuTVE.IsDisposed)
        {
            VistaMenuTVE = new frmMenuTVE(this.ModoPrueba, this.ModoNocturno, this.btnOff);
            VistaMenuTVE.Transfer += this.MostrarTransferTVE;
            VistaMenuTVE.Consulta += this.MostrarConsultaTVE;
            VistaMenuTVE.MuestraSalir += this.MostrarApagar_Salir;
            VistaMenuTVE.Cerrar += this.CerrarForm;
            VistaMenuTVE.Ubicacion += this.GetlocationPant;
            VistaMenuTVE.FinTVE += eTerminarTVE;
        }

        FocusON(VistaMenuTVE);

        //ListaSistemas.Add(VistaMenuTVE.Name);
    }

    /// <summary>
    /// Se encarga de mostrar la pantalla de
    /// transferencia de TVE
    /// </summary>
    private async void MostrarTransferTVE()
    {
        //Si se logra levantar bien la lógica de abordaje
        //creamos y levantamos el Front
        Thread Hilo;
        Hilo = new Thread(new ParameterizedThreadStart(this.MostrarPopUp));
        timerSiempreAdelante.Stop();
        Hilo.Start("ESPERA3");

        try
        {
            if (await AbordajeTrans())
            {
                timerSiempreAdelante.Start();
                if (VistaTransferTVE == null || VistaTransferTVE.IsDisposed)
                {
                    VistaTransferTVE = new frmTransferTVE(this.ModoPrueba, this.ModoNocturno, this.NumAutobus);
                    VistaTransferTVE.StatusTVE += this.MostrarStatusTransferTVE;
                    VistaTransferTVE.MuestraSalir += this.MostrarApagar_Salir;
                    VistaTransferTVE.Cerrar += this.CerrarForm;
                    VistaTransferTVE.Wifi += this.AdministrarWifi;
                    VistaTransferTVE.Ubicacion += this.GetlocationPant;
                    VistaTransferTVE.Transfer += this.eValidarTransferTVE;
                    VistaTransferTVE.ConexionTVE += this.eConexionTVE;
                    VistaTransferTVE.ethernet += this.AdminsitrarEthernet;
                }
                FocusON(VistaTransferTVE);
            }
            else
            {
                timerSiempreAdelante.Start();
                MostrarError("Error al cargar componentes de TVE");
            }
        }
        catch
        {

        }
        finally
        {
            timerSiempreAdelante.Start();
        }
    }

    /// <summary>
    /// Se encarga de mostrar la pantalla del estado de la transferencia de TVE
    /// </summary>
    /// <param name="Exitoso"></param>
    private void MostrarStatusTransferTVE()
    {
        if (VistaStatusTVE == null || VistaStatusTVE.IsDisposed)
        {
            VistaStatusTVE = new frmStatusTVE(this.ModoPrueba, EstadoTVE(), this.ModoNocturno);
            VistaStatusTVE.MuestraSalir += this.MostrarApagar_Salir;
            VistaStatusTVE.Cerrar += this.CerrarForm;
            VistaStatusTVE.Ubicacion += this.GetlocationPant;
            VistaStatusTVE.GenerarQR += this.eGenerarQR;
            VistaStatusTVE.InfoCorrida += this.einfoCorrida;
        }

        //Cerramos la vista de transferencia
        //CerrarForm(VistaTransferTVE);

        //Render
        FocusON(VistaStatusTVE);

        //ListaSistemas.Add(VistaStatusTVE.Name);
    }

    /// <summary>
    /// Se encarga de mostrar la pantalla de
    /// consulta de TVE
    /// </summary>
    private void MostrarConsultaTVE()
    {
        if (VistaQRTVE == null || VistaQRTVE.IsDisposed)
        {
            VistaQRTVE = new frmQRTVE(this.ModoPrueba, this.ModoNocturno);
            VistaQRTVE.MuestraSalir += this.MostrarApagar_Salir;
            VistaQRTVE.Cerrar += this.CerrarForm;
            VistaQRTVE.Ubicacion += this.GetlocationPant;
            VistaQRTVE.Login += this.eMostrarLoginTVE;
            VistaQRTVE.Error += this.MostrarError;
        }

        FocusON(VistaQRTVE);

        //ListaSistemas.Add(VistaQRTVE.Name);
    }

    /// <summary>
    /// Proviene de frmMenu, se activa al presionar el botón de Sincronizar
    /// Lanza otro evento que avisa a SAM para que entre en Modo Sincronización
    /// </summary>
    private void MandarSyncSAM()
    {
        if (!this.Sincronizando)
        {//Si no estoy sincronizando...

            if (VistaSync == null || VistaSync.IsDisposed)
            {
                VistaSync = new frmSync(this.ModoPrueba, this.ModoNocturno);
                VistaSync.Cerrar += this.CerrarForm;
                VistaSync.Apagar += this.ApagarSistema;
                VistaSync.Ubicacion += this.GetlocationPant;
                VistaSync.MinutosApagado = this.minsApagado;
            }

            //Render
            FocusON(VistaSync);

            //Mandamos a detener la pelicula si es que traemos VMD
            if (this.VMD)
            {
                StopMovieSync();
            }

            EventoSyncSAM();
        }
    }

    /// <summary>
    /// Se encarga de mostrar un error en el frmError
    /// </summary>
    /// <param name="mensaje_error"></param>
    private void MostrarError(string mensaje_error)
    {
        if (VistaError == null || VistaError.IsDisposed)
        {
            VistaError = new frmError(mensaje_error, this.ModoPrueba, this.ModoNocturno);
            VistaError.Cerrar += this.CerrarForm;
            VistaError.Ubicacion += this.GetlocationPant;
        }

        //Mostramos la vista
        FocusON(VistaError);
        //ListaSistemas.Add(VistaError.Name);
    }

    /// <summary>
    /// Se encarga de mostrar el REPORTE de TELEMETRIA
    /// Powered ByRED 15JUN2020
    /// </summary>
    private void MostrarTelemetria()
    {
        if (VistaDatosTelemetria == null || VistaDatosTelemetria.IsDisposed)
        {

            VistaDatosTelemetria = new frmDatosTelemetria(this.ModoPrueba, this.ModoNocturno, Telematics());
            VistaDatosTelemetria.Cerrar += this.CerrarForm;
            VistaDatosTelemetria.Ubicacion += this.GetlocationPant;
        }

        FocusON(VistaDatosTelemetria);

        //ListaSistemas.Add(VistaMenuTVE.Name);
    }

    /// <summary>
    /// Se encarga de mostrar la vista de Herramientas para VMD
    /// Powered ByRED 16JUN2020
    /// </summary>
    private void MostrarHerramientasVMD()
    {
        if (VistaHerramientasVMD == null || VistaHerramientasVMD.IsDisposed)
        {
            VistaHerramientasVMD = new frmHerramientasVMD(this.ModoPrueba, this.ModoNocturno, Version);
            VistaHerramientasVMD.Cerrar += this.CerrarForm;
            VistaHerramientasVMD.Ubicacion += this.GetlocationPant;
            VistaHerramientasVMD.CargadorPautas += this.MostrarCargadorDePautasVMD;
            VistaHerramientasVMD.CargadorSpots += this.MostarCargadorSpots;

        }

        FocusON(VistaHerramientasVMD);
    }
    /// <summary>
    /// Se encarga de mostrar la vista de Herramientas para VMD
    /// Powered ByRED 16JUN2020
    /// </summary>
    private void MuestraMenuSpots()
    {
        if (VistaMenuSpots == null || VistaMenuSpots.IsDisposed)
        {
            VistaMenuSpots = new frmMenuSpots(this.ModoPrueba, this.ModoNocturno, Version);
            VistaMenuSpots.Cerrar += this.CerrarForm;
            VistaMenuSpots.Ubicacion += this.GetlocationPant;
            //VistaMenuSpots.CargadorSpots += this.MostarCargadorSpots; Cambio de requerimiento

        }

        FocusON(VistaMenuSpots);
    }


    /// <summary>
    /// Se encarga de mostrar la vista de cargador de pauta de VMD
    /// Powered ByRED 16JUN2020
    /// </summary>
    private void MostrarCargadorDePautasVMD(string tipo)
    {
        var listaPautas = PautasVMD(tipo);

        if (listaPautas.Count > 0)
        {
            //Mandamos a detener la película, si hubiera en reproducción
            Func_DetenerPelicula();

            if (VistaCargadorPauta == null || VistaCargadorPauta.IsDisposed)
            {
                switch (tipo)
                {
                    case "USB":
                        VistaCargadorPauta = new frmCargadorDePautas(this.ModoPrueba, this.ModoNocturno, listaPautas, tipo);
                        break;

                    default:
                        VistaCargadorPauta = new frmCargadorDePautas(this.ModoPrueba, this.ModoNocturno, listaPautas);
                        break;
                }

                VistaCargadorPauta.Cerrar += this.CerrarForm;
                VistaCargadorPauta.Ubicacion += this.GetlocationPant;
                VistaCargadorPauta.Pauta += this.ePlanchaPauta;
                VistaCargadorPauta.PautaUSB += this.eRecuperarPautasUSB;
                VistaCargadorPauta.MandaError += this.MostrarError;
                VistaCargadorPauta.ProgresoCopiado += this.ePedirProgresoCopiado;
                VistaCargadorPauta.PopUp += this.MostrarPopUp;

                VistaCargadorPauta.ValidaPauta += this.eValidaPauta; //Powered ByRED 16JUL2020
            }

            FocusON(VistaCargadorPauta);
        }
        else
        {
            //Mandamos a mostrar un error, según su tipo
            switch (tipo)
            {
                case "USB":
                    MostrarError("No se encontraron Medios Extraibles");
                    break;

                default:
                    MostrarError("No se pudieron Recuperar Pautas");
                    break;
            }
        }
    }
    /// <summary>
    /// Se encarga de mostrar la vista de cargador de pauta de VMD
    /// Powered ByToto 
    /// </summary>
    private void MostarCargadorSpots(String tipo)
    {
        List<spotPOI> listaSpots = new List<spotPOI>();
        listaSpots = SpotsVMD(tipo);

        if (listaSpots.Count > 0)
        {
            //Mandamos a detener la película, si hubiera en reproducción
            //Func_DetenerPelicula();

            if (VistaSpots == null || VistaSpots.IsDisposed)
            {
                VistaSpots = new frmSpots(this.ModoPrueba, this.ModoNocturno, listaSpots, tipo);

                VistaSpots.Cerrar += this.CerrarForm;
                VistaSpots.Ubicacion += this.GetlocationPant;
                VistaSpots.Pauta += this.ePlanchaPauta;
                VistaSpots.MandaError += this.MostrarError;
                VistaSpots.ProgresoCopiado += this.ePedirProgresoCopiado;
                VistaSpots.PopUp += this.MostrarPopUp;
                VistaSpots.ReproducirSpot += this.ReproduceSpotPOI;
                VistaSpots.ValidaPauta += this.eValidaPauta; //Powered byToto 2023
            }

            FocusON(VistaSpots);
        }
        else
        {
            //Mandamos a mostrar un error, según su tipo
            switch (tipo)
            {
                case "audio":
                    MostrarError("No se encontraron Medios de Audio");
                    break;
                case "video":
                    MostrarError("No se encontraron Spots para Reproducir");//Powered ByRED 25OCT2023
                    break;

                default:
                    MostrarError("No se pudieron Recuperar Pautas");
                    break;
            }
        }
    }

    /// <summary>
    /// Se detona por que el Sistema Operativo está apagandose
    /// Powered ByRED 10DIC2020
    /// </summary>
    private void ApagadoPorSistema()
    {   //Valida si estamos en Modo Configuración o no
        if (this.ModoConfig) { ApagarSistemaConfig(); } else { /*ApagarSistema();*/ }
    }

    /// <summary>
    /// Se encarga de mostrar la vista de los mensajes de SIA
    /// Powered ByRED 17MAR2021
    /// </summary>
    private void MostrarMensajesSIA()
    {
        if (VistaMensajesSIA == null || VistaMensajesSIA.IsDisposed)
        {
            VistaMensajesSIA = new frmPanelMensajes(this.ModoPrueba, this.ModoNocturno);
            VistaMensajesSIA.Cerrar += this.CerrarForm;
            VistaMensajesSIA.Ubicacion += this.GetlocationPant;
            VistaMensajesSIA.Error += this.MostrarError;
            VistaMensajesSIA.MensajeSIA += this.ObtenerMensajesSIA;
            VistaMensajesSIA.EnviarSIA += this.EnviarMensajeaSIA;
            VistaMensajesSIA.TextoSMS += this.MostrarMensajeSMSSIA;
            VistaMensajesSIA.Enviado += this.MostrarMensajeEnviado;
        }

        //Mandamos a cargar la primera vista
        //Se pone afuera de su propio constructor, porque necesitamos de los eventos
        VistaMensajesSIA.Crearlayout("Predeterminados", 0);

        FocusON(VistaMensajesSIA);
    }

    /// <summary>
    /// Se encarga de mostrar un pop up de mensaje enviado
    /// Powered ByRED 19MAR2021
    /// </summary>
    private void MostrarMensajeEnviado()
    {
        if (VistaMensajeEnviado == null || VistaMensajeEnviado.IsDisposed)
        {
            VistaMensajeEnviado = new frmEnviandoMensaje(this.ModoPrueba, this.ModoNocturno);
            VistaMensajeEnviado.Cerrar += this.CerrarForm;
            VistaMensajeEnviado.Ubicacion += this.GetlocationPant;
            VistaMensajeEnviado.Error += this.MostrarError;
        }

        FocusON(VistaMensajeEnviado);
    }

    /// <summary>
    /// Se encarga de mostrar un mensaje SMS entrante o en detalle
    /// Powered ByRED 19MAR2021
    /// </summary>
    private void MostrarMensajeSMSSIA(string _texto)
    {
        this.MostrandoMensajeSIA = true;

        if (VistaMostrarSMS == null || VistaMostrarSMS.IsDisposed)
        {
            VistaMostrarSMS = new frmMostrarMensaje(this.ModoPrueba, this.ModoNocturno, _texto);
            VistaMostrarSMS.Cerrar += this.CerrarFormMostrarSMS;
            VistaMostrarSMS.Ubicacion += this.GetlocationPant;
            VistaMostrarSMS.Error += this.MostrarError;
        }

        FocusON(VistaMostrarSMS);
    }

    #endregion


    #region "Manejador de Eventos Front"

    /// <summary>
    /// Verifica si el Operador existe en la base de datos
    /// </summary>
    /// <param name="clvConductor"></param>
    /// <returns></returns>
    private bool ValidaConductor(string clvConductor)
    {
        var NombreConductor = ConductorSAM(clvConductor);

        if (!NombreConductor.Equals("NE"))
        {
            //Guardo los datos del conductor temporal
            this.ClvConductorTemp = clvConductor;
            this.NomConductorTemp = NombreConductor;

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Regresa el estado del viaje
    /// </summary>
    /// <returns></returns>
    private bool VerificaViaje()
    {
        DatosPieMenuCAN();
        return this.EnViaje;
    }

    /// <summary>
    /// Pregunta a SAM si tenemos protocolo de CAN
    /// para poder acceder al administrador de viajes
    /// </summary>
    /// <returns></returns>
    private bool VerificaProtocolo()
    {
        return ProtocoloSAM();
    }

    /// <summary>
    /// Pregunta si el sistema se encuentra en modo PRUEBA
    /// </summary>
    /// <returns></returns>
    private bool VerificaModoPrueba()
    {
        return this.ModoPrueba;
    }

    /// <summary>
    /// Se encarga de recuperar la velocidad de SAM
    /// </summary>
    /// <returns></returns>
    private int VerificaVelocidad()
    {
        return Velocidad();
    }

    /// <summary>
    /// Pregunta a SAM si tenemos la poblacion
    /// introducida es Válida
    /// </summary>
    /// <param name="CVEPOB"></param>
    /// <returns></returns>
    private bool VerificaPoblacion(string CVEPOB)
    {
        return PoblacionSAM(CVEPOB);
    }

    /// <summary>
    /// Proviene de frmViaje, manda a abrir viaje CAN através de SAM
    /// </summary>
    private void AbrirViaje(string clvConductor)
    {
        if (!this.EnViaje)
        {
            if (VistaViaje.txtDisplay.Text.Length > 0)
            {
                if (ValidaConductor(clvConductor))
                {

                    if (ChecarPoblacion())//Para verificar si GPS encontó poblacion
                    {
                        //Mandamos el viaje a SAM
                        this.MandarViaje("VA", clvConductor);
                    }
                    else
                    {//Si no tenemos población, se manda el form de clave de poblacion

                        this.MostrarCLVPoblacion("VA", clvConductor);
                    }

                    #region "Por cualquier cosa"
                    //if (ViajeSAM("VA", clvConductor))
                    //{
                    //    this.EnViaje = true;

                    //    this.NomConductor = this.NomConductorTemp;
                    //    this.ClvConductor = this.ClvConductorTemp;

                    //    this.DatosPieDePagina(true);

                    //    VistaViaje.indicador(this.NomConductor, true, true);
                    //}
                    //else
                    //{

                    //    this.NomConductorTemp = string.Empty;
                    //    this.ClvConductorTemp = string.Empty;

                    //    this.EnViaje = false;

                    //    VistaViaje.indicador("", false, false);
                    //}

                    //VistaViaje.txtDisplay.Text = "";

                    #endregion
                }
                else
                {
                    VistaViaje.indicador("La clave es Incorrecta", false, true);
                }
            }
            else
            {
                VistaViaje.indicador("La clave es Incorrecta", false, true);
            }
        }
        else
        {
            VistaViaje.indicador("El Viaje se encuentra Abierto", false, true);
        }
    }

    /// <summary>
    /// Proviene de frmViaje, manda a cambiar manos CAN através de SAM
    /// </summary>
    private void CambioManos(string clvConductor)
    {
        if (VistaViaje.txtDisplay.Text.Length > 0)//Validamos que no esté en blanco
        {
            if (!clvConductor.Equals(this.ClvConductor))//Validamos que sea diferente a la que ya se tiene
            {
                if (ValidaConductor(clvConductor))//Se valida la clave del conductor
                {
                    if (ChecarPoblacion())//Para verificar si el GPS encontró población
                    {
                        //Mandamos Viaje a SAM
                        this.MandarViaje("CM", clvConductor);
                    }
                    else
                    {
                        this.MostrarCLVPoblacion("CM", clvConductor);
                    }

                    #region "Por si las moscas cawn"
                    //if (ViajeSAM("CM", clvConductor))//Se manda a hacer el Cambio de Manos
                    //{
                    //    this.EnViaje = true;

                    //    this.NomConductor = this.NomConductorTemp;
                    //    this.ClvConductor = this.ClvConductorTemp;

                    //    this.DatosPieDePagina(true);

                    //    VistaViaje.indicador(this.NomConductor, true, true);

                    //}
                    //else
                    //{
                    //    this.NomConductorTemp = string.Empty;
                    //    this.ClvConductorTemp = string.Empty;

                    //    VistaViaje.indicador("", false, false);

                    //    //Se muestra error hacia el usuario
                    //    MostrarError("Error interno al hacer cambio de Manos");
                    //}
                    #endregion  
                }
                else
                {
                    VistaViaje.indicador("La clave es Incorrecta", false, true);
                }
            }
            else
            {
                VistaViaje.indicador("La clave está en uso", false, true);
            }
        }
        else
        {
            VistaViaje.indicador("La clave es Incorrecta", false, true);
        }
    }

    /// <summary>
    /// Proviene de frmViaje, manda a cerrar viaje CAN a través de SAM
    /// </summary>
    /// <param name="clvConductor"></param>
    /// <returns></returns>
    private void CerrarViaje(string clvConductor)
    {
        if (this.EnViaje)
        {
            if (VistaViaje.txtDisplay.Text.Length > 0)
            {
                if (ValidaConductor(clvConductor))
                {
                    if (clvConductor.Equals(this.ClvConductor))
                    {
                        if (ChecarPoblacion())//Para verificar si el GPS encontró población
                        {
                            //Si tenemos ya poblacion mandamos el viaje a SAM
                            this.MandarViaje("VC", clvConductor);
                        }
                        else
                        {
                            this.MostrarCLVPoblacion("VC", clvConductor);
                        }

                        #region "Por si las moscas cawn"

                        //if (ViajeSAM("VC", clvConductor))
                        //{
                        //    this.EnViaje = false;

                        //    this.NomConductor = string.Empty;
                        //    this.NomConductorTemp = string.Empty;

                        //    this.ClvConductor = string.Empty;
                        //    this.ClvConductorTemp = string.Empty;

                        //    VistaViaje.indicador("El viaje fue cerrado correctamente", true, true);

                        //    DatosPieDePagina(false);
                        //}
                        //else
                        //{
                        //    this.EnViaje = true;

                        //    this.NomConductorTemp = string.Empty;
                        //    this.ClvConductorTemp = string.Empty;

                        //    VistaViaje.indicador("", false, false);

                        //    MostrarError("Error al cerrar Viaje, intente de nuevo");
                        //}

                        #endregion
                    }
                    else
                    {
                        VistaViaje.indicador("La clave es Incorrecta", false, true);
                    }
                }
                else
                {
                    VistaViaje.indicador("La clave es Incorrecta", false, true);
                }
            }
            else
            {
                VistaViaje.indicador("La clave es Incorrecta", false, true);
            }
        }
        else
        {
            VistaViaje.indicador("El Viaje se encuentra Cerrado", false, true);
        }
    }

    /// <summary>
    /// Se encarga de mandar los datos de Viaje hacia SAM
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="clvConductor"></param>
    private void MandarViaje(string tipo, string clvConductor)
    {
        if (ViajeSAM(tipo, clvConductor))
        {
            switch (tipo)
            {
                case "VA":

                    this.EnViaje = true;

                    this.NomConductor = this.NomConductorTemp;
                    this.ClvConductor = this.ClvConductorTemp;

                    this.DatosPieDePagina();
                    this.DatosPieMenuCAN();

                    VistaViaje.indicador(this.NomConductor, true, true);

                    VistaViaje.txtDisplay.Text = "";

                    break;

                case "CM":
                    this.EnViaje = true;

                    this.NomConductor = this.NomConductorTemp;
                    this.ClvConductor = this.ClvConductorTemp;

                    this.DatosPieDePagina();
                    this.DatosPieMenuCAN();

                    VistaViaje.indicador(this.NomConductor, true, true);
                    break;

                case "VC":
                    this.EnViaje = false;

                    this.NomConductor = string.Empty;
                    this.NomConductorTemp = string.Empty;

                    this.ClvConductor = string.Empty;
                    this.ClvConductorTemp = string.Empty;

                    VistaViaje.indicador("El viaje fue cerrado correctamente", true, true);


                    if (VMD)
                    {
                        //Detener la pelicula
                        Func_DetenerPelicula();
                    }


                    DatosPieDePagina();
                    this.DatosPieMenuCAN();
                    break;
            }
        }
        else
        {

            switch (tipo)
            {
                case "VA":
                    this.NomConductorTemp = string.Empty;
                    this.ClvConductorTemp = string.Empty;

                    this.EnViaje = false;

                    VistaViaje.indicador("", false, true);

                    VistaViaje.txtDisplay.Text = "";

                    //Se muestra error hacia el usuario
                    MostrarError("Error interno al iniciar viaje");

                    break;

                case "CM":

                    this.NomConductorTemp = string.Empty;
                    this.ClvConductorTemp = string.Empty;

                    VistaViaje.indicador("", false, false);

                    //Se muestra error hacia el usuario
                    MostrarError("Error interno al hacer cambio de Manos");

                    break;

                case "VC":

                    this.EnViaje = true;

                    this.NomConductorTemp = string.Empty;
                    this.ClvConductorTemp = string.Empty;

                    VistaViaje.indicador("", false, false);

                    MostrarError("Error al cerrar Viaje, intente de nuevo");

                    break;

            }
        }
    }

    /// <summary>
    /// Se encarga de mostrar o ocultar los datos de viaje
    /// segun sea el caso, despues de un evento de Viaje CAN
    /// </summary>
    /// <param name="Mostrar"></param>
    private void DatosPieDePagina()
    {
        if (this.EnViaje)
        {
            VistaSistemas.lblCvlAutobus.Text = this.NumAutobus;
            VistaSistemas.lblNombreOperador.Text = this.NomConductor;
            VistaSistemas.lblCvlOperador.Text = this.ClvConductor;
        }
        else
        {
            VistaSistemas.lblCvlAutobus.Text = string.Empty;
            VistaSistemas.lblNombreOperador.Text = this.txtViajeCerrado;
            VistaSistemas.lblCvlOperador.Text = string.Empty;
        }
    }

    /// <summary>
    /// Se encarga de actualizar los datos de viaje
    /// según sea el caso, despues de u nevento de Viaje CAN
    /// en la vista de menú de CAN
    /// </summary>
    private void DatosPieMenuCAN()
    {
        if (VistaMenuCAN != null || !VistaMenuCAN.IsDisposed)
        {
            if (this.EnViaje)
            {
                VistaMenuCAN.lblCvlOperador.Text = this.ClvConductor;
                VistaMenuCAN.lblNombreOperador.Text = this.NomConductor;
                VistaMenuCAN.lblCvlAutobus.Text = this.NumAutobus;
            }
            else
            {
                VistaMenuCAN.lblCvlOperador.Text = string.Empty;
                VistaMenuCAN.lblNombreOperador.Text = this.txtViajeCerrado;
                VistaMenuCAN.lblCvlAutobus.Text = string.Empty;
            }
        }
    }

    /// <summary>
    /// Se encarga de recopilar la información necesaria
    /// para ser mostrada en el PopUp De datos de Viaje
    /// </summary>
    /// <returns></returns>
    private List<string> DatosViaje()
    {
        List<string> Datos = new List<string>();

        if (this.EnViaje)
        {
            Datos.Add("Abierto");
            Datos.Add(this.ClvConductor);
            Datos.Add(this.NomConductor);
        }
        else
        {
            Datos.Add("Cerrado");
        }

        return Datos;
    }

    /// <summary>
    /// Se encarga de pedir a SAM la información del GPS
    /// Para ser mostrada en el PopUp de Datos de GPS
    /// </summary>
    /// <returns></returns>
    private List<string> DatosGPS()
    {
        List<string> Datos = new List<string>();

        Datos = DatosGPSSAM();

        return Datos;
    }

    /// <summary>
    /// Sirve para mandar a apagar el wifi despues de una
    /// tranferencia de TVE
    /// </summary>
    private Task<bool> AdministrarWifi(bool encender, bool Transferencia)
    {
        return Task<bool>.Run(
           async () =>
           {
               await Task.Delay(1);
               try
               {
                   WIFI(encender, Transferencia);
               }
               catch
               {

               }
               return true;
           });
    }

    /// <summary>
    /// Se encarga de activar/descativar el Ethernet
    /// Powered ByRED 27MAY2021
    /// </summary>
    /// <param name="encender"></param>
    /// <returns></returns>
    private Task<bool> AdminsitrarEthernet(bool encender)
    {
        return Task<bool>.Run(
            async () =>
            {
                await Task.Delay(1);
                try
                {
                    ETHERNET(encender);
                }
                catch
                {

                }
                return true;
            });
    }

    /// <summary>
    /// Se encarga de obtener el resultado de
    /// </summary>
    /// <returns></returns>
    private Task<bool> eValidarTransferTVE()
    {
        return TransferTVE();
    }

    /// <summary>
    /// Se encarga de obtener el estado 
    /// de la conexion con la logica de TVE
    /// </summary>
    /// <returns></returns>
    private Task<bool> eConexionTVE()
    {
        return ConTVE();
    }

    //Se encarga de terminar con la lógica de TVE
    private void eTerminarTVE()
    {
        FinTVE();
    }

    /// <summary>
    /// se encarga de mostrar la vista de loginTVE
    /// </summary>
    /// <returns></returns>
    private void eMostrarLoginTVE()
    {
        if (VistaLoginTVE == null || VistaLoginTVE.IsDisposed)
        {
            VistaLoginTVE = new frmLoginTVE(PassTVE(), this.ModoPrueba, this.ModoNocturno);
            VistaLoginTVE.Ubicacion += this.GetlocationPant;
            VistaLoginTVE.Error += this.MostrarError;
            VistaLoginTVE.Cerrar += this.CerrarForm;
            VistaLoginTVE.LoginBien += this.eFlagearloginTVE;

        }

        FocusON(VistaLoginTVE);

        //Mostramos la vista pero fuera de la lógica de administrador de pantallas
        //return VistaLoginTVE.ShowDialog() == DialogResult.OK ? true : false;

    }

    /// <summary>
    /// Se encarga de enviarle el resultado del login
    /// al form de Consulta TVE
    /// </summary>
    /// <param name="Correcto"></param>
    private void eFlagearloginTVE(bool Correcto)
    {
        if (VistaQRTVE != null || !VistaQRTVE.IsDisposed)
        {
            VistaQRTVE.EstadoLogin = Correcto;
            VistaQRTVE.ValidarLogin();
        }
    }

    /// <summary>
    /// Manda a generar los Qr para consulta
    /// </summary>
    /// <returns></returns>
    private Task<bool> eGenerarQR()
    {
        return QrConsulta();
    }

    /// <summary>
    /// Se encarga de regresar la informacion de la corrida
    /// </summary>
    /// <returns></returns>
    private string einfoCorrida()
    {
        return InfoCorrida();
    }

    /// <summary>
    /// Se encarga de mandar a planchar la pauta
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombrepauta"></param>
    /// <returns></returns>
    private Task<bool> ePlanchaPauta(string _tipo, string _nombrepauta)
    {
        return Pauta(_tipo, _nombrepauta);
    }

    /// <summary>
    /// Se encarga de validar la pauta
    /// Powered ByRED 16JUL2020
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombreCarpeta"></param>
    /// <returns></returns>
    private Task<bool> eValidaPauta(string _tipo, string _nombreCarpeta)
    {
        return ValidaPauta(_tipo, _nombreCarpeta);
    }

    /// <summary>
    /// Se encarga de recuperar las pautas alojadas en una unidad USB
    /// Powered ByRED 16JUN2020
    /// </summary>
    /// <param name="_letraUnidad"></param>
    /// <returns></returns>
    private List<string> eRecuperarPautasUSB(string _letraUnidad)
    {
        return PautaUSB(_letraUnidad);
    }

    /// <summary>
    /// Se encarda de mandar a pedir a SAM el progreso del copiado
    /// </summary>
    /// <returns></returns>
    private int ePedirProgresoCopiado()
    {
        return ProgresoCopiado();
    }


    /// <summary>
    /// Se encarga de pedir la lista de Mensajes de SIA
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="_tipo"></param>
    /// <returns></returns>
    private List<string> ObtenerMensajesSIA(int _tipo)
    {
        return MensajesSIA(_tipo);
    }

    /// <summary>
    /// manda un mensaje de texto através de SIA
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="msj"></param>
    /// <returns></returns>
    private bool EnviarMensajeaSIA(string _msj)
    {
        return MensajeSIA(_msj);
    }

    /// <summary>
    /// Se encarga de recibir el flag de si está mostrando o no el cintillo
    /// </summary>
    /// <param name="Mostrando"></param>
    private void eMostrandoCintillo(bool _mostrando)
    {
        this.MostrandoCintilloSIA = _mostrando;
    }

    #endregion

    #region "Manejador de Eventos Configuración"

    /// <summary>
    /// Se encarga de lanzar el form para la configuración del
    /// numero economico del autobus y la región
    /// PANTALLA CONFIGURACION #1
    /// </summary>
    private void ConfigNumAutobus(int pantallaOrigen)
    {
        if (ConfigAutobus == null || ConfigAutobus.IsDisposed)
        {
            ConfigAutobus = new frmConfigAutobus(Version, this.ModoPrueba, Regiones(), this.ModoNocturno, this.btnOff);
            ConfigAutobus.Config += this.SiguientePantallaConfiguracion;
            ConfigAutobus.MandaRegion += this.ChecarRegion;
            ConfigAutobus.MuestraSalir += this.MostrarApagar_Salir;
            ConfigAutobus.Error += this.MostrarError;
            ConfigAutobus.Ubicacion += this.GetlocationPant;

            ConfigAutobus.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020
        }

        if (ConfigAutobus.Cargado)
        {
            ConfigAutobus.Show();

            switch (pantallaOrigen)
            {
                case 2:
                    ConfigMeta.TopMost = false;
                    break;

                case 3:
                    ConfigSistemas.TopMost = false;
                    break;
            }

            ConfigAutobus.TopMost = true;

            switch (pantallaOrigen)
            {
                case 2:
                    ConfigMeta.Hide();
                    break;

                case 3:
                    ConfigSistemas.Hide();
                    break;
            }
        }
        else
        {
            FocusON(ConfigAutobus);
            ConfigAutobus.Cargado = true;

            switch (pantallaOrigen)
            {
                case 2:
                    ConfigMeta.Hide();
                    break;

                case 3:
                    ConfigSistemas.Hide();
                    break;
            }

            //Mandamos cerrar el form de Login.
            //CerrarForm(VistaLogin);
            VistaLogin.Hide();
            VistaLogin.Dispose();
        }
    }

    /// <summary>
    /// Se encarga de mostrar la vista para cargar la Meta de CAN
    /// Powered ByRED 10/SEP/2020
    /// PANTALLA CONFIGURACION #2
    /// </summary>
    private void ConfigTipoMetaCAN(int pantallaOrigen)
    {

        if (ConfigMeta == null || ConfigMeta.IsDisposed)
        {
            ConfigMeta = new frmMetaCAN(this.ModoPrueba, this.ModoNocturno);
            ConfigMeta.Ubicacion += this.GetlocationPant;
            ConfigMeta.MandaError += this.MostrarError;
            ConfigMeta.Config += this.SiguientePantallaConfiguracion;
            ConfigMeta.PantallaAnterior += this.PantallaConfiguracionAnterior;

            ConfigMeta.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020
        }

        //Para cargar las metas por si se regresó a cambiar la región seleccionada
        ConfigMeta.CrearLayout(MetasCAN());

        if (ConfigMeta.Cargado)
        {
            ConfigMeta.Show();

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.TopMost = false;
                    break;

                case 3:
                    ConfigSistemas.TopMost = false;
                    break;
            }

            ConfigMeta.TopMost = true;

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.Hide();
                    break;

                case 3:
                    ConfigSistemas.Hide();
                    break;
            }
        }
        else
        {
            FocusON(ConfigMeta);
            ConfigMeta.Cargado = true;

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.Hide();
                    break;

                case 3:
                    ConfigSistemas.Hide();
                    break;
            }
        }
    }

    /// <summary>
    /// Se encarga de lanzar el form para la configuracion
    /// PANTALLA CONFIGURACION #3
    /// </summary>
    /// <param name="region"></param>
    /// <param name="autobus"></param>
    private void ConfigTipoAutobus(int pantallaOrigen)
    {

        if (ConfigSistemas == null || ConfigSistemas.IsDisposed)
        {
            ConfigSistemas = new frmConfiguracion(this.ModoPrueba, this.ModoNocturno);
            ConfigSistemas.Error += this.MostrarError;
            ConfigSistemas.CerrarConfig += this.TerminarConfig;
            ConfigSistemas.PantallaAnterior += this.PantallaConfiguracionAnterior;
            ConfigSistemas.Ubicacion += this.GetlocationPant;

            ConfigSistemas.ApagarPorSistema += this.ApagadoPorSistema; //Powered ByRED 10DIC2020
        }

        if (ConfigSistemas.Cargado)
        {
            ConfigSistemas.Show();

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.TopMost = false;
                    break;

                case 2:
                    ConfigMeta.TopMost = false;
                    break;
            }

            ConfigSistemas.TopMost = true;

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.Hide();
                    break;

                case 2:
                    ConfigMeta.Hide();
                    break;
            }
        }
        else
        {
            FocusON(ConfigSistemas);
            ConfigSistemas.Cargado = true;

            switch (pantallaOrigen)
            {
                case 1:
                    ConfigAutobus.Hide();
                    break;

                case 2:
                    ConfigMeta.Hide();
                    break;
            }
        }
    }

    /// <summary>
    /// Se encarga de validar cual será la siguiente pantalla a mostrar
    /// Powered ByRED 10/SEP/2020
    /// </summary>
    /// <param name="PantallaOrigen"></param>
    private void SiguientePantallaConfiguracion(int PantallaOrigen)
    {
        switch (PantallaOrigen)
        {
            case 0:
                //Para configurar el número de autobús
                ConfigNumAutobus(PantallaOrigen);
                break;

            case 1:
                //Guardamos el número de autobús
                this.NumAutobus = ConfigAutobus.NumAutobus;

                //Validamos que vista es la siguiente a mostrar
                if (ValidarMetasCAN())
                {
                    //lanzamos pantalla #2
                    ConfigTipoMetaCAN(PantallaOrigen);
                }
                else
                {
                    //Lanzamos pantalla #3
                    ConfigTipoAutobus(PantallaOrigen);
                }

                break;

            case 2:

                //Guardamos la meta
                this.tempNombreMetaCAN = ConfigMeta.MetaSeleccionada;

                //Lanzamos pantalla #3
                ConfigTipoAutobus(PantallaOrigen);
                break;
        }

    }


    /// <summary>
    /// Se encarga de validar cual es la pantalla anterior
    /// Powered ByRED 10/SEP/2020
    /// </summary>
    private void PantallaConfiguracionAnterior(int PantallaOrigen)
    {
        switch (PantallaOrigen)
        {
            case 2:
                ConfigNumAutobus(PantallaOrigen);
                break;

            case 3:
                if (ValidarMetasCAN())
                {
                    ConfigTipoMetaCAN(PantallaOrigen);
                }
                else
                {
                    ConfigNumAutobus(PantallaOrigen);
                }
                break;
        }
    }


    /// <summary>
    /// Manda a verificar si existe la región.
    /// </summary>
    /// <param name="reg"></param>
    /// <returns></returns>
    private string ChecarRegion(long reg)
    {
        return SAMRegion(reg);
    }


    /// <summary>
    /// Termina la configuración de la aplicación
    /// </summary>
    /// <param name="TarjetaCAN"></param>
    /// <param name="CatSistemas"></param>
    private async void TerminarConfig(int _TarjetaCAN, Sistemas _CatSistemas)
    {
        this.CAN = _CatSistemas.CAN;
        this.VMD = _CatSistemas.VMD;
        this.CONDUSAT = _CatSistemas.CONDUSAT;
        this.PLAT = _CatSistemas.PLAT;
        this.TELEMETRIA = _CatSistemas.TELEMETRIA;
        //Powered ByRED 17MAR2021
        this.SIA = _CatSistemas.SIA;
        this.MINISIA = _CatSistemas.SIADLL;
        this.SIIAB_POI = _CatSistemas.SIIAB_POI;//powered byToto

        ConfigSAM(_TarjetaCAN, _CatSistemas.Antivirus);

        Thread Hilo;

        ConfigSistemas.TopMost = false;

        Hilo = new Thread(new ParameterizedThreadStart(this.MostrarPopUp));
        Hilo.Start("ESPERA1");
        await Task.Delay(5000);


        Hilo = new Thread(new ParameterizedThreadStart(this.MostrarPopUp));
        Hilo.Start("ESPERA2");
        await Task.Delay(5000);

        this.CerrarForm(ConfigSistemas);

        VistaReproductor.Hide();
        VistaReproductor.Close();
        VistaReproductor.Dispose();


        //Application.ExitThread();
    }

    /// <summary>
    /// Sirve para dar mensaje al usuario_1
    /// </summary>
    /// <returns></returns>
    private List<string> DatosEspera_1()
    {
        List<string> Datos = new List<string>();

        Datos.Add("Los Datos Fueron Almacenados " + Environment.NewLine + "Correctamente");

        return Datos;
    }

    /// <summary>
    /// Sirve para dar mensaje al usuario_2
    /// </summary>
    /// <returns></returns>
    private List<string> DatosEspera_2()
    {
        List<string> Datos = new List<string>();

        Datos.Add("El sistema se reiniciará " + Environment.NewLine + "Espere por favor.");

        return Datos;
    }

    /// <summary>
    /// Para mandar mensaje de espera para Abordaje Electronico
    /// </summary>
    /// <returns></returns>
    private List<string> DatosEspera_3()
    {
        List<string> Datos = new List<string>();

        Datos.Add("Cargando..." + Environment.NewLine + "Espere por favor.");

        return Datos;
    }

    /// <summary>
    /// Sirve para mostrar mensaje de confirmación de Pauta
    /// </summary>
    /// <param name="nombrePauta"></param>
    /// <returns></returns>
    private List<string> DatosPauta()
    {
        List<string> Datos = new List<string>();

        Datos.Add("La Pauta para VMD" + Environment.NewLine + "Se Cargó Correctamente");

        return Datos;
    }

    /// <summary>
    /// Sirve para mostrar mensaje de encolamiento spot
    /// </summary>
    /// <param name="nombrePauta"></param>
    /// <returns></returns>
    private List<string> DatosPOI()
    {
        List<string> Datos = new List<string>();

        Datos.Add("Spot agregado.");

        return Datos;
    }



    //private List<string> DatosSync()
    //{
    //    List<string> Datos = new List<string>();

    //    Datos.Add("Preparando la Sincronización \nEspere por favor.");

    //    return Datos;
    //}

    #endregion

    #region "Timers"
    private void timerActualizarEncabezado_Tick(object sender, EventArgs e)
    {
        timerActualizaEncabezado.Stop();
        //

        //Actualizamos el encabezado
        VistaSistemas.lblMetaFR.Text = this.FR_Meta.ToString("N2");
        VistaSistemas.lblRealFR.Text = this.FR_Real.ToString("N2");

        if (this.FR_indicador)
        {
            VistaSistemas.lblRealFR.ForeColor = Color.Green;
        }
        else
        {
            VistaSistemas.lblRealFR.ForeColor = Color.Red;
        }

        //verificamos si tenemos que darle foco a la vista de los sistemas

        if (this.VistaActual.Equals(VistaSistemas.Name)) VistaSistemas.TopMost = true;

        timerActualizaEncabezado.Start();
    }

    private void timerActualizarCAN_Tick(object sender, EventArgs e)
    {
        timerActualizaCAN.Stop();

        if (protocoloCAN)
        {
            VistaSistemas.lblECAN.Text = "Factor de Rendimiento";
            VistaSistemas.lblVFRMeta.Text = this.FR_Meta.ToString("N2");
            VistaSistemas.lblVFR.Text = this.FR_Real.ToString("N2");
            //VistaSistemas.lblVKm.Text = this.Kms.ToString("G");
            VistaSistemas.lblVKm.Text = this.Kms.ToString("N2");
            //VistaSistemas.lblVlitros.Text = this.Lts.ToString("G");
            VistaSistemas.lblVlitros.Text = this.Lts.ToString("N2");

            //Actualizamos el indicador de FR en vista de CAN
            if (this.FR_indicador)
            {
                if (ModoNocturno)
                {
                    VistaSistemas.smfCAN.Image = Resources.FRGrandeGreenNoc;
                }
                else
                {
                    VistaSistemas.smfCAN.Image = Resources.FRGrandeGreen;
                }
            }
            else
            {
                if (ModoNocturno)
                {
                    VistaSistemas.smfCAN.Image = Resources.FRGrandeRedNoc;
                }
                else
                {
                    VistaSistemas.smfCAN.Image = Resources.FRGrandeRed;
                }
            }
        }
        else
        {
            VistaSistemas.lblECAN.Text = "Sin Datos de CAN";
            //VistaSistemas.lblMetaFR.Text = "0.00";
        }

        //if (FR_indicador == true)
        //{
        //    VistaSistemas.smfCAN.Image = Resources.FRGrandeGreen;
        //}
        //else
        //{
        //    VistaSistemas.smfCAN.Image = Resources.FRGrandeRed;           
        //}

        //
        timerActualizaCAN.Start();
    }

    private void timerActualizarCONDUSAT_Tick(object sender, EventArgs e)
    {
        timerActualizaCondusat.Stop();

        timerActualizaCondusat.Start();
    }

    /// <summary>
    /// Se encarga de actualizar el estatus de VMD
    /// Powered ByRED2020
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerActualizarVMD_Tick(object sender, EventArgs e)
    {
        timerActualizaVMD.Stop();

        if (VistaReproductor != null)
        {
            //Actualizamos el tiempo Real de reproducción
            VistaSistemas.lblREproduccion.Text = VistaReproductor.TiempoReal.ToString(@"hh\:mm\:ss") + " Tiempo de reproducción";
            this.TiempoPelicula = VistaReproductor.TiempoReal.ToString(@"hh\:mm\:ss");
            VistaSistemas.lblDuracion.Text = VistaReproductor.TiempoDuracion.ToString(@"hh\:mm\:ss") + " Duración total";
            this.DuracionPelicula = VistaReproductor.TiempoDuracion.ToString(@"hh\:mm\:ss");
            VistaSistemas.lblTituloPelicula.Text = VistaReproductor.NombreVideo;
            this.NombrePelicula = VistaReproductor.NombreVideo;
        }

        timerActualizaVMD.Start();
    }

    /// <summary>
    /// Para obtener nuevos mensajes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerActualizarSIA_Tick(object sender, EventArgs e)
    {
        timerActualizaSIA.Stop();

        if (!this.MostrandoMensajeSIA)
        {
            if (!this.MensajeCONDUCTOR.Equals(""))
            {
                MostrarMensajeSMSSIA(this.MensajeCONDUCTOR);

                this.MensajeCONDUCTOR = string.Empty;
            }
        }

        if (this.VMD)//Powered ByRED 10JUN2021
        {
            //Powered ByRED 29MAR2021
            if (!this.MostrandoPISIA)
            {
                if (this.PISIA.Count != 0)
                {
                    //Mandamos a mostrar el Punto de Interes
                    MostrarPISIA(true);
                    //this.PISIA.Clear();
                }
            }
        }

        timerActualizaSIA.Start();
    }

    /// <summary>
    /// Para quitar el modo PI del reproductor
    /// Powered ByRED 29MAR2021
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerTerminaPISIA_Tick(object sender, EventArgs e)
    {
        timerTerminaPISIA.Stop();
        timerTerminaPISIA.Enabled = false;

        //Para dejar de mostrar el PI
        MostrarPISIA(false);
    }

    /// <summary>
    /// Se encarga de verificar la posision de la ventana del reproductor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerVerificaReproductor_Tick(object sender, EventArgs e)
    {
        timerVerificaReproductor.Stop();
        //Validamos las posiciones de la pantalla, no deben estar movidas
        //Más de 10 pixeles

        if (ListaPantallas.Count > 1)
        {
            var xR = VistaReproductor.Location.X;
            int xP = 0;

            var pant = ListaPantallas.Where(x => !x.Primary).ToList();

            foreach (Pantalla p in pant)
            {
                var MyPant = (Pantalla)p;

                xP = MyPant.X;
            }
            if (ModoPrueba)
            {
                if (!(xR >= xP - 20 && xR <= xP + 20))
                {
                    VistaReproductor.Location = GetlocationPantSec();
                }
            }
            else
            {
                //Evita un reacomodo mientras de pintan puntos de interes o cintillo
                //Powered ByRED 23ABR2021
                if (!this.MostrandoCintilloSIA)
                {
                    if (!this.MostrandoPISIA)
                    {
                        if (xR != xP)
                        {
                            VistaReproductor.Location = GetlocationPantSec();
                        }
                    }
                }
            }
        }

        timerVerificaReproductor.Start();
    }
    /// <summary>
    /// Sirve para dar retraso de focus a las vistas que están por abrir
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerMuestraFront_Tick(object sender, EventArgs e)
    {
        timerMuestraFront.Stop();
        RetrasoFocus();
    }

    /// <summary>
    /// Se encarga de mantener la vista(form) siempre adelante
    /// cuando no se encuentra en modo prueba
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerSiempreAdelante_Tick(object sender, EventArgs e)
    {
        timerSiempreAdelante.Stop();

        //Si nos encontramos en modo configuración no pondremos el topmost a todas
        //las vistas, sólo para mandar un error o un popup
        if (!ModoConfig)
        {
            ObtenerForm(ListaVistas.LastOrDefault()).TopMost = true;
        }
        else
        {
            if (ListaVistas.LastOrDefault().Equals("frmError") || ListaVistas.LastOrDefault().Equals("frmPopUp"))
            {
                ObtenerForm(ListaVistas.LastOrDefault()).TopMost = true;
            }
        }


        //Verificamos si tenemos que cerrar la ventana por falta de actividad
        if (!ObtenerActividad(ListaVistas.LastOrDefault()))
        {
            //Para regresarme el foco el form interno de FrmSistemas
            if (ListaVistas.LastOrDefault().Equals("frmSistemas"))
            {
                VistaSistemas.SistemaFocus();
            }
            else
            {
                CerrarForm(ObtenerForm(ListaVistas.LastOrDefault()));
            }
        }

        timerSiempreAdelante.Start();
    }



    #endregion

    #region "VMD"
    public void Func_RepPlay(int idArchivo, string rutaVideo, int MinutosMax, bool detenerVideoActual, bool playToPlay, double posicion)
    {
        VistaReproductor.PlaySobrePlay = playToPlay;
        VistaReproductor.Func_VMDPlay(new frmReproductor.VideoVMD { idArchivo = idArchivo, rutaVideo = rutaVideo, MinutosMax = MinutosMax, posicion = posicion }, detenerVideoActual);
    }

    //Para cintillo Inicial
    public void Func_ReproducirCintillo(string TextoMostrar,
                                  string PosicionCintillo,
                                  string ColorDFondo,
                                  int VelocidadDCintillo,
                                  int TamanioDFuente,
                                  string ColorDFuente,
                                  int VueltasCintillo)
    {
        VistaReproductor.Func_AddCintillo(TextoMostrar, PosicionCintillo, ColorDFondo, VelocidadDCintillo, TamanioDFuente, ColorDFuente, VueltasCintillo);
    }


    /// <summary>
    /// Se ocupara cuando FE reciba desde SIA un texto
    /// </summary>
    /// <param name="TextoMostrar"></param>
    public void Func_ReproducirCintillo(string TextoMostrar)
    {
        VistaReproductor.Func_AddCintillo(TextoMostrar);
    }


    public void Func_ReiniciarReproductor()
    {
        VistaReproductor.Func_VMDReiniciar();
    }
    /// <summary>
    /// se encarga de inicializar la logica del reproductor de VMD
    /// </summary>
    private void InicializarRepro()
    {
        if (this.VMD && !VistaReproductor.EnReproduccion()) //Powered ByRED 20JUN2020
        {
            SAMPLAY();
        }
    }
   

    /// <summary>
    /// se encarga de inicializar la logica del reproductor de VMD Spots
    /// </summary>
    private void InicializarReproSpots(int Tipo)
    {
        if (this.VMD) //Powered ByRED 20JUN2020
        {
            SAMPLAY();
        }
    }
    /// <summary>
    /// SE encarga de iniciar la logica para reproducir un nuevo spot
    /// Powered ByToto
    /// </summary>
    public Boolean ReproduceSpotPOI(spotPOI sp)
    {
        //VistaReproductor.PlaySobrePlay = playToPlay;
        Boolean res = false;
        if (this.InLogin)
        {
            if (VistaReproductor != null)
            {
                if (!VistaReproductor.spotPoi) { Func_DetenerPeliculaPOI(); }
                //switch (tipo)
                //{
                //    case 0:
                //        VistaReproductor.ReproducirMP3POI(nombreSpot);
                //        break;
                //    case 1:
                //        VistaReproductor.Func_SpotPlay(new frmReproductor.SpotPoi
                //        {
                //            idArchivo = 0,
                //            rutaVideo = nombreSpot
                //        },
                //    true);
                //        break;
                //}
                VistaReproductor.Func_SpotPlay(sp);
                res = true;
            }
        }
        return res;
    }
    /// <summary>
    /// SE encarga de enviar el testigo de reproduccion de SPOT
    /// </summary>
    /// Powered ByTOTO♫♫
    private void testigoSpotPOI(spotPOI sp)
    {
        testigoSpotPOISAM(sp);
    }
    /// <summary>
    /// 
    /// </summary>
    private void TerminaSpotPoi()
    {
        if (VistaReproductor != null)
        {
            VistaReproductor.spotPoi = false;
            InicializarRepro();
        }
    }
    // public void Func_BuscarPauta()
    //  {
    //      eVMDbuscarPauta();
    //  }

    public void Func_ReiniciarPauta(bool DetenerVideo)
    {
        VistaReproductor.Func_PararTmrActividad();
        eVMDreiniciarPauta(false);
    }

    public void Func_ActualizaActividad(double posicion, double longitud)
    {
        eVMDActualizaActividad(posicion, longitud);
    }

    public void Func_ActualizarUltimaVez(int idArchivo)
    {
        eVMDActualizaUltimaVez(idArchivo);
    }

    public void Func_ChecaSiguienteVideo()
    {

        eVMDChecaSiguienteVideo();
    }

    /// <summary>
    /// *Tarjeta de circulacion Vencida*
    /// </summary>
    /// <param name="nombreVideo"></param>
    /// <param name="TiempoVideo"></param>
    public void Func_InfoVideo(string nombreVideo, TimeSpan TiempoVideo)
    {
        VistaSistemas.InfoVideo(nombreVideo, TiempoVideo);
    }

    /// <summary>
    /// *Tarjeta de circulacion Vencida*
    /// </summary>
    /// <param name="Posicion"></param>
    public void Func_PosicionVideo(TimeSpan Posicion)
    {
        VistaSistemas.PosicionVideo(Posicion);
    }

    public int Func_ControlVolumen(SMFE.Forms.frmVolumen.TipoDeVolumen Tipo)
    {
        if (VistaReproductor == null) return -1;

        if (Tipo == SMFE.Forms.frmVolumen.TipoDeVolumen.Mas)
            return VistaReproductor.Func_SubirVolumen();
        else
            return VistaReproductor.Func_BajarVolumen();
    }

    /// <summary>
    /// Se encarga de cambiar el icono del estado de la reproduccion Avanzando - Detenida
    /// Powered ByRED2020
    /// </summary>
    /// <param name="estado"></param>
    public void Func_EstadoReproduccion(string estado)
    {
        //Powered ByRED 18FEB2021
        switch (estado)
        {
            case "play":
                EnReproduccion = true;
                break;
            default:
                EnReproduccion = false;
                break;
        }

        VistaSistemas.Func_EstablecerEstadoReproductor(estado);
    }

    public void Func_AgregarLogReproductor(string VideoURL, int idPelicula, int MinutosMax, bool Ejecuta)
    {
        eVMDAgregarLog(VideoURL, idPelicula, MinutosMax, Ejecuta);
    }

    public void Func_DetenerPelicula()
    {
        VistaReproductor.Func_DetenerPelicula();
    }

    public void Func_DetenerPeliculaPOI()
    {
        VistaReproductor.Func_DetenerPeliculaPOI();
    }
    /// <summary>
    /// Se encarga de detener la pelicula por sincronización ByRed
    /// </summary>
    private void StopMovieSync()
    {
        VistaReproductor.Func_DetenerPelicula();
        eVMDreiniciarPauta(true);
    }

    #endregion
}