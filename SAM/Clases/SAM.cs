using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Data;
public class SAM : IBDContext, IBDContextCon, IBDContextTs, IGPS
{
    #region "Variables"

    //Version
    private string Version = "SAM " + Application.ProductVersion;

    //Velocidad del sistema Ya sea por CAN o CONDUSAT
    private int VelocidadCONDUSAT = 0;
    private int VelocidadCAN = 0;

    //Datos de viaje
    private int Clave_Operador = 0;
    private string Nombre_Operador = "";
    private bool EnViaje = false;
    private DateTime FechaViaje;
     
    //Ruta de VLC
    private string RutaReproductor = string.Empty;

    //EF´s
    public vmdEntities VMD_BD { get; }
    public condusatEntities CONDUSAT_BD { get; }
    public telematicsEntities TELEMATICS_BD { get; }

    //Lista de sistemas
    public List<ISistema> ListaSistemas;

    //Lista Procesos, sirve para guardar la ruta de los procesos que debemos
    //de detener al apagar SAM
    private List<string> ListaProcesos;

    //Lógicas
    private SyncSAM syncSAM;
    private DatosRegistros DatosSistemas;
    public GPSData Datos_GPS { get; set; }
    private can_poblaciones CANPob = null;

    //Parámetros
    public XML confXML;
    private can_parametrosinicio ParametrosInicio;
    private int TiempoEsperaMensajes = 1000;

    //FrontEngine
    private FrontEngine FE;

    //Timers
    public System.Windows.Forms.Timer timerActualizaSys = new System.Windows.Forms.Timer();
    public System.Windows.Forms.Timer timerCintillo = new System.Windows.Forms.Timer();

    //Flags
    private bool Wifi = false;
    private bool ipRenovada = false;
    private bool GPS = false;
    private bool MiniSIA = false; //CLAUS && ROJO

    //Abordaje TVE
    private Abordaje MyAbordaje;

    //Para la configuracion Global
    Config _config;
    #endregion

    #region "Variables de Eventos"

    //Para la sincronización
    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSync;

    //Para avisar que la Sync ha sido completada --Optimizar 
    public delegate void SyncComplete(bool Exitoso);
    public event SyncComplete SyncOK;

    //Para Actualizar Condusat
    public delegate void ActualizarVista(string ColorVel, int Vel_Real, int Vel_Max, bool UsarCAN, bool Param_ADOCAN, string imgAdvertencia);
    public event ActualizarVista RefreshViewCondusat;

    //Para actualizar IndicadorGPS
    public delegate void ActualizarIndicadorGPS(int estado, bool HorarioNocturno);
    public event ActualizarIndicadorGPS RefreshLedGPS;

    //Para Validar conductor en CAN
    public delegate string CANValidaConductor(string clvConductor);
    public event CANValidaConductor CANConductor;

    //Para ejecutar el administrador de viaje
    public delegate bool CANViaje(string tipo, string clvConductor, can_poblaciones PobActual);
    public event CANViaje CANAdmin;

    //Para mandar a ejecutar la logica de Abordaje
    public delegate Task<bool> AbordajeTransferencia();
    public event AbordajeTransferencia TVETransfer;

    //Para validar la transferencia de TVE
    public delegate Task<bool> ValidarTransferTVE();
    public event ValidarTransferTVE TransferTVE;

    //Obtiene el resultado de la transferencia de TVE
    public delegate bool EstadoTransferTVE();
    public event EstadoTransferTVE EstadoTVE;

    //Obtiene el estado de la conexion de TVE
    public delegate Task<bool> EstadoConTVE();
    public event EstadoConTVE ConexionTVE;

    //Para obtener el pass de TVE
    public delegate string ObtenerPassTVE();
    public event ObtenerPassTVE PassTVE;

    //Termina con la logica de TVE
    public delegate void TerminarTVE();
    public event TerminarTVE FinTVE;

    //Genera los Qr para las consultas de abordaje
    public delegate Task<bool> GenerarQR();
    public event GenerarQR QrConsulta;

    //Nos regresa la corrida de abordaje
    public delegate string InfoCorrida();
    public event InfoCorrida CorridaInfo;

    public delegate void VMDPlay(int idArchivo, string rutaVideo, int MinutosMax, bool detenerVideo, bool playSobrePlay, double posicion);
    public event VMDPlay EPlay;

    public delegate void VMDCintillo(string TextoMostrar,
                                  string PosicionCintillo,
                                  string ColorDFondo,
                                  int VelocidadDCintillo,
                                  int TamanioDFuente,
                                  string ColorDFuente,
                                  int VueltasCintillo);
    public event VMDCintillo ECintilloInicial;
    public delegate void VMDCintilloMensaje(string TextoMostrar);
    public event VMDCintilloMensaje ECintillo;

    /// <summary>
    /// Se encarga de enviar un mensaje al front
    /// para ser mostrado al conductor
    /// </summary>
    /// <param name="alerta"></param>
    public delegate void AlertaCodigoConductor(string alerta);
    public event AlertaCodigoConductor AlertaConductor;


    /// <summary>
    /// Para actualizar IndicadorTelemetria
    /// Powered ByRED 15JUN2020
    /// </summary>
    /// <param name="estado"></param>
    public delegate void ActualizarIndicadorTelemetria(int estado);
    public event ActualizarIndicadorTelemetria RefreshLedTelemetria;

    /// <summary>
    /// Se encarga de mandar a pedir el reporte al sistema de Telematics
    /// </summary>
    /// <param name="estado"></param>
    public delegate List<string> PeticionReporteTelemetria();
    public event PeticionReporteTelemetria ReporteTelemetria;

    /// <summary>
    /// Se encarga de mandar a planchar la pauta de VMD
    /// Powered ByRED 16JUn2020
    /// </summary>
    /// <returns></returns>
    public delegate Task<bool> PlancharPautaVMD(string _tipo, string _nombre);
    public event PlancharPautaVMD PautaVMD;

    /// <summary>
    /// Se encarga de validar la pauta de VMD
    /// Powered ByRED 16JUL2020
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombre"></param>
    /// <returns></returns>
    public delegate Task<bool> ValidarPautaVMD(string _tipo, string _nombre);
    public event ValidarPautaVMD ValidaPautaVMD;

    #region EventosSIA

    /// <summary>
    /// Se encarga de relanzar le información al FE
    /// Powered ByRED 23FEB2021
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tipo"></param>
    /// <param name="texto"></param>
    public delegate bool MensajeSIAFront(int tipo, string texto);
    public event MensajeSIAFront MensajeSIA;

    /// <summary>
    /// Se enecarga de enviar al front el estado del internet
    /// </summary>
    /// <param name="estado"></param>
    public delegate void InternetSIAFront(int estado);
    public event InternetSIAFront InternetSIA;

    /// <summary>
    /// Recibe de SIA un POI para ser enviado a Front
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <param name="Multimedia"></param>
    /// <returns></returns>
    public delegate bool POISIAFront(List<string> Multimedia);
    public event POISIAFront POISIA;

    #endregion

    #endregion

    //Constructor Principal de SAM
    public SAM()
    {
        VMD_BD = new vmdEntities();

        CONDUSAT_BD = new condusatEntities();

        TELEMATICS_BD = new telematicsEntities();

        HolaSAM();
    }

    #region "Metodos Publicos"

    private void HolaSAM()
    {
        //Cargamos la configuracion Inicial

        CargaConfiguracion();

        if (ParametrosInicio != null)
        {
            if (ParametrosInicio.Autobus.Count() > 0 && !ParametrosInicio.Autobus.Equals("0"))
            {
                LanzarSAM();
            }
            else
            {
                if (ConfiguraMovil())
                {
                    //Si no hubo error en la configuración reiniciamos la aplicación

                    //Validar si reiniciarmos sólo el sistema o todo el equipo

                    //ReiniciarSistema();
                    //Application.Restart();
                    HolaSAM();

                }

                //Thread.Sleep(2000);

                //Application.Restart();
            }
        }
        else
        {
            //Por si hubo error en la lectura de BD
        }
    }

    /// <summary>
    /// Inicia los sistemas configurados en la lista de sistemas
    /// configurados previamente
    /// </summary>
    public void IniciaSistemas()
    {
        foreach (ISistema x in ListaSistemas)
        {
            x.Inicializar();
        }
    }

    /// <summary>
    /// Detiene los sistemas enunciados en la lista de sistemas
    /// </summary>
    public void DetenerSistemas()
    {
        foreach (ISistema x in ListaSistemas) x.Finalizar();
    }

    #region "Sincronización"

    /// <summary>
    /// Manda a sincronizar los sistemas enunciados en la BD.OrdenDescarga
    /// </summary>
    private async void SincronizarSistemas()
    {
        //Indicamos al Front que estamos en sincronización
        FE.Sincronizando = true;

        //Detenemos las actualizaciones de los sistemas
        timerActualizaSys.Stop();

        //Creamos objeto de lógica SAM
        syncSAM = new SyncSAM();

        //Preparar el Log
        syncSAM.BorrarLogSync(Application.StartupPath);

        //Log
        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Inicia sincronización, autobús: " + ParametrosInicio.Autobus);

        //Obtenemos la orden de descargas
        List<orden_descarga> ordenDes = (from x in VMD_BD.orden_descarga
                                         orderby x.Orden
                                         select x).ToList();

        if (ordenDes.Count > 0)
        {//Si hay registros

            //Log
            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Sincronizando " + ordenDes.Count + " sistemas");

            //Preparamos todo para empezar a sincronizar los sistemas
            if (await ConexionSync())
            {

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Red obtenida: " + syncSAM.IpActual);


                if (await ConexionServer())
                {
                    Boolean SyncSistemas = true;


                    //Mandamos a sincronizar todos los sistemas que se hayan iniciado
                    foreach (orden_descarga x in ordenDes)
                    {

                        #region "Prueba de codigo"
                        // var actualiza = ListaSistemas.Where(y => y.Sistema.ToString() == x.Sistema).FirstOrDefault();


                        // if (actualiza != null)
                        // {
                        //
                        //     FormaSync.txtLog.Text = "Inicia " + x.Sistema.ToString() + "...";
                        //
                        //     FormaSync.lblMensajeFinal.Text = FormaSync.lblMensajeFinal.Text + "\n " + actualiza.Sincronizar();
                        //
                        //
                        // }

                        #endregion

                        var actualiza = ListaSistemas.Where(y => y.Sistema.ToString() == x.Sistema).ToList();
                        foreach (ISistema i in actualiza)
                        {
                            EnviaraSync("Inicia: " + x.Sistema.ToString() + "...", 0);
                            //Log
                            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Iniciando: " + x.Sistema.ToString());

                            await Task.Delay(TiempoEsperaMensajes);

                            switch (x.Sistema.ToString())
                            {
                                case "CONDUSAT":
                                    var myCONDUSAT = (CONDUSAT)i;
                                    myCONDUSAT.EventoSyncCONDUSAT += this.EnviaraSync;
                                    //Resultados.Add(myCONDUSAT.Sincronizar());

                                    if (myCONDUSAT.Sincronizar())
                                    {
                                        EnviaraSync("CONDUSAT: Correcto", 1);
                                    }
                                    else
                                    {
                                        EnviaraSync("CONDUSAT: Incorrecto", 1);
                                        SyncSistemas = false;
                                    }

                                    break;

                                case "VMD":

                                    var myVMD = (VMD)i;
                                    myVMD.evMensajeSincronizacion += this.EnviaraSync;
                                    //Resultados.Add(myVMD.Sincronizar(syncSAM.VersionServer, syncSAM.strVersionesSistemas, syncSAM.IpActual, syncSAM.SqlCon, syncSAM.ServerAlterno, ref syncSAM.log));

                                    if (syncSAM.ServerNube)
                                    {//Método Nube
                                        if (myVMD.Sincronizar(syncSAM.servicioWCF, ref syncSAM.log))
                                        {
                                            EnviaraSync("VMD: Correcto", 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("VMD: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }
                                    else
                                    {//Método Tradicional
                                        if (myVMD.Sincronizar(syncSAM.SqlCon, ref syncSAM.log))
                                        {
                                            EnviaraSync("VMD: Correcto", 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("VMD: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }

                                    break;

                                case "CAN":

                                    var myCAN = (CAN)i;

                                    myCAN.EventoSyncCAN += this.EnviaraSync;

                                    //Resultados.Add(myCAN.Sincronizar(syncSAM.VersionServer, syncSAM.strVersionesSistemas, syncSAM.IpActual, syncSAM.SqlCon, syncSAM.ServerAlterno, ref syncSAM.log));

                                    if (syncSAM.ServerNube)
                                    {//Se va por el método Nube
                                        if (myCAN.Sincronizar(syncSAM.servicioWCF, syncSAM.strVersionesSistemas, syncSAM.IpActual, ref syncSAM.log))
                                        {
                                            EnviaraSync("CAN: Correcto, Codigo Descarga: " + myCAN.CodigoDescarga, 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("CAN: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }
                                    else
                                    {
                                        if (myCAN.Sincronizar(syncSAM.VersionServer, syncSAM.strVersionesSistemas, syncSAM.IpActual, syncSAM.SqlCon, syncSAM.ServerAlterno, syncSAM.AnilloRedCAN, ref syncSAM.log))
                                        {
                                            EnviaraSync("CAN: Correcto, Codigo Descarga: " + myCAN.CodigoDescarga, 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("CAN: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }

                                    break;

                                case "SIA":

                                    //Claus & ROJO
                                    if (this.MiniSIA)
                                    {
                                        var mySIA = (SIA)i;
                                        mySIA.MsjSync += this.EnviaraSync;

                                        if (mySIA.Sincronizar())
                                        {
                                            EnviaraSync("Mini SIA: Correcto", 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("Mini SIA: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }
                                    
                                    break;


                                case "PLAT":

                                    //Si estamos en modo DIOS no ejecutamos PLAT
                                    if (!ParSAM.ModoDeveloper)
                                    {
                                        var myPlat = (PLAT)i;
                                        myPlat.EventoSyncPlat += this.EnviaraSync;
                                        //Resultados.Add(myPlat.Sincronizar());

                                        if (myPlat.Sincronizar())
                                        {
                                            EnviaraSync("PLAT: Correcto", 1);
                                        }
                                        else
                                        {
                                            EnviaraSync("PLAT: Incorrecto", 1);
                                            SyncSistemas = false;
                                        }
                                    }
                                    break;
                            }
                        }

                        //EnviaraSync(x.Sistema.ToString() + ": ", 1);

                        await Task.Delay(TiempoEsperaMensajes);
                    }

                    //Si no hubo ningún error en la sincronización, mandará a apagar el equipo
                    if (SyncSistemas)
                    {
                        EnviaraSync("El equipo se apagará en:", 0);
                        SyncOK(true);
                    }
                    else
                    {
                        EnviaraSync("Hubo un error en la sincronización" + Environment.NewLine + "Intente de nuevo", 0);
                        SyncOK(false);
                    }
                }
                else
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se pudo establecer conexión con el Servidor");
                    EnviaraSync("No se pudo conectar con el servidor \nIntente de nuevo", 0);
                    SyncOK(false);
                }
            }
            else
            {//Si hubo algún error en la configuración del sistema para la sincronización, llegará aquí

                EnviaraSync("No se pudo conectar a la Red \nIntente de nuevo", 0);
                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se pudo conectar a la Red y/u Obtención de una Ip Válida");
                SyncOK(false);
            }
        }
        else
        {//No debería de pasar por aquí, pero por si las moscas Cawn

            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No hay sistemas Por Actualizar");

            EnviaraSync("No hay sistemas por actualizar", 1);
            SyncOK(false);

        }

        this.ipRenovada = false;

        //Encendemos/Apagamos los adaptadores
        try
        {
            //Apagamos Wifi
            syncSAM.Wifi(false, ParametrosInicio.IDWIFI, false, ParametrosInicio.Autobus);
            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Wifi Apagado");

            await Task.Delay(TiempoEsperaMensajes);

            //Encendemos LAN
            if ((bool)ParametrosInicio.SIA || (bool)ParametrosInicio.TELEMATICS)
            {
                syncSAM.Ethernet(true, ParametrosInicio.IdLAN);
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Ethernet para Telematics/SIA Encendido");

                //Si tenemos SIIAB Telematics...
                if ((bool)ParametrosInicio.TELEMATICS)
                {
                    var MyTelematics = ListaSistemas.Where(x => x.Sistema == Sistema.TELEMETRIA).ToList();

                    foreach (TELEMETRIA _telematics in MyTelematics)
                    {
                        //Iniciamos el cliente
                        _telematics.ClienteTelemetria(true);
                    }
                }
            }
        }
        catch
        {

        }
        //Cerramos la conexión
        
        if (syncSAM.ServerNube)
        {//Mandamos a finalizar la conexión al webservice si es que estamos descargando en NUBE
            syncSAM.servicioWCF.Close();
            syncSAM.ServerNube = false;
        }
        else
        {//Mandamos a finalizar la conexión SQL para esquema de descarga normal
            syncSAM.CerrarConexión();
        }
        await Task.Delay(TiempoEsperaMensajes);

        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Se terminó la sincronización");

        //Método para grabar  el LOG
        syncSAM.GuardarLogSync(Application.StartupPath);

        //Indicamos el front que salimos de la Sync
        FE.Sincronizando = false;

        timerActualizaSys.Start();
    }

    /// <summary>
    /// lógica encargada de preparar los adaptadores de RED para
    /// la sincronización
    /// </summary>
    /// <returns></returns>
    private Task<bool> ConexionSync()
    {
        return Task<bool>.Run(
            async () =>
            {
                //Si hay SIA Mandamos a desactivar el Ethernet
                if ((bool)ParametrosInicio.SIA || (bool)ParametrosInicio.TELEMATICS)
                {

                    //Si tenemos SIIAB Telematics...
                    if ((bool)ParametrosInicio.TELEMATICS)
                    {
                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Empaquetando la ultima información de codigos de Telemetria");

                        EnviaraSync("Empaquetando información" + Environment.NewLine + "de TELEMETRIA", 0);

                        var MyTelematics = ListaSistemas.Where(x => x.Sistema == Sistema.TELEMETRIA).ToList();

                        foreach (TELEMETRIA _telematics in MyTelematics)
                        {
                            _telematics.CierreDeCodigos();

                            //Detenemos el cliente
                            _telematics.ClienteTelemetria(false);
                        }
                    }

                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Desactivando Ethernet");

                    EnviaraSync("Deshabilitando" + Environment.NewLine + "la tarjeta de Ethernet", 0);
                    await Task.Delay(TiempoEsperaMensajes);

                    if (!syncSAM.Ethernet(false, ParametrosInicio.IdLAN))
                    {//Si falla el proceso de desactivar el internet mando un falso

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Hubo Un error al desabilitar la tarjeta de RED Ethernet");
                        return false;
                    }

                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Ethernet Deshabilitado");
                }

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Verificando IPRenovada");

                //Verificamos si ya existe una IpRenovada (Que no debería)
                if (!this.ipRenovada)
                {
                    //Habilitamos el WIFI que en teoria debería de estar deshabilitado
                    EnviaraSync("Habilitando WIFI" + Environment.NewLine + Environment.NewLine + "Espere por favor...", 0);
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Habilitando Wifi");

                    //Se manda a activar el Wifi y se valida el proceso
                    if (syncSAM.Wifi(true, ParametrosInicio.IDWIFI, false, ParametrosInicio.Autobus))
                    {
                        await Task.Delay(TiempoEsperaMensajes);
                        this.Wifi = true;

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Wifi Habilitado");

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Validando Anillo de Red");

                        //Verificamos si se encuentra en un anillo de red Válido
                        if (!syncSAM.ValidaIP(ParametrosInicio.AnilloRedValido))
                        {
                            //Log
                            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No hay aún Anillo de red Válido, esperando por uno...");

                            int cont = 1;

                            while (cont <= 3)
                            {
                                //checo si hay dirección IP Valida

                                if (!syncSAM.ValidaIP(ParametrosInicio.AnilloRedValido))
                                {
                                    EnviaraSync("Conectando a Red..." + Environment.NewLine + Environment.NewLine + "Intento: " + cont, 0);

                                    //Log
                                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Conectando a Red, intento " + cont);

                                    //Se le incrementa el tiempo, para que pueda conectarse
                                    await Task.Delay(30000);

                                    //Se manda a liberar y a renovar la dirección Ip
                                    syncSAM.ReleaseIP_Renew();

                                    //Espero unos segs
                                    //await Task.Delay(TiempoEsperaMensajes * 3);
                                }
                                else
                                {
                                    EnviaraSync("Red Actual:" + Environment.NewLine + Environment.NewLine + syncSAM.IpActual, 0);

                                    //Log
                                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Anillo de Red Valido: " + syncSAM.IpActual);

                                    this.ipRenovada = true;

                                    await Task.Delay(TiempoEsperaMensajes);

                                    return true;

                                }

                                cont++;
                            }

                            //Si pasa por aquí, quiere decir que no pudo obtener ip, por lo que mando a deshabilitar el WiFi
                            //La deshabilito para que cuando vuelva a ejecutarse la sincronización la habilite

                            syncSAM.Wifi(false, ParametrosInicio.IDWIFI, false, ParametrosInicio.Autobus);

                            //Log
                            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se pudo obtener una IP Valida");

                            EnviaraSync("No se pudo conectar" + Environment.NewLine + "para sincronizar..." +
                                Environment.NewLine + Environment.NewLine + "Intente de Nuevo...", 0);

                            await Task.Delay(TiempoEsperaMensajes);

                            return false;
                        }
                        else
                        {// Ya hay IP, entonces mando la IP que hay actualmente
                            EnviaraSync("Red Actual:" + Environment.NewLine + Environment.NewLine + syncSAM.IpActual, 0);

                            //Log
                            if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Anillo de Red Valido: " + syncSAM.IpActual);

                            await Task.Delay(TiempoEsperaMensajes);

                            this.ipRenovada = true;
                        }
                    }
                    else
                    {

                        this.Wifi = false;
                        EnviaraSync("Error al activar la tarjeta wifi, se aborta la sinconización", 0);
                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Error, al activar el Wifi");

                        await Task.Delay(TiempoEsperaMensajes);

                        SyncOK(false);

                        return false;
                    }
                }
                else
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Error, ya existe una IPRenovada");
                    return false;
                }
                return true;
            });
    }

    /// <summary>
    /// Se encarga de configurar al conexión al servidor para iniciar
    /// la sincronización
    /// </summary>
    /// <returns></returns>
    private Task<bool> ConexionServer()
    {
        return Task<bool>.Run(
            async () =>
            {
                //Valido por IP de red o por PuntoGPS a qué servidor se hará la descarga, se separa lógica de Anillo y GPS ByRED

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Buscando a qué servidor sincronizar");

                if (!syncSAM.CualServidorxIP(VMD_BD))
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se encontró servidor por anillo de red, buscando por punto GPS");

                    //Si no se encontró por anillo de RED, entonces, se buscará por punto GPS
                    //Le mandamos condusat o gps, no sé VALIDAR
                    var _sistema = ListaSistemas.Where(x => x.Sistema == Sistema.GPS).FirstOrDefault();

                    if (!syncSAM.CualServidorxGPS(_sistema))
                    {
                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se encontró servidor por posición GPS, se le asigna el valor por default -> " + ParametrosInicio.CnStrServer);
                        //no se encontró tampoco por GPS se le asigna la que viene por Default en Parametros_Inicio
                        syncSAM.CualServidorCAN = ParametrosInicio.CnStrServer;
                    }
                }

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Detectando si es servidor alterno o central");

                //NUBE
                if (syncSAM.ServerNube)
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Servidor Cloud Detectado por " + syncSAM.FormaIdentiZona + ": " + syncSAM.ZonaDescarga);

                    EnviaraSync("Sincronizando en..." + Environment.NewLine + syncSAM.ZonaDescarga + Environment.NewLine + syncSAM.FormaIdentiZona + Environment.NewLine + "Espere por favor...", 0);

                    await Task.Delay(TiempoEsperaMensajes);
                }
                else if(syncSAM.ServerAlterno)
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Servidor Alterno Detectado por " + syncSAM.FormaIdentiZona + ": " + syncSAM.ZonaDescarga);

                    EnviaraSync("Sincronizando en..." + Environment.NewLine + syncSAM.ZonaDescarga + Environment.NewLine + syncSAM.FormaIdentiZona + Environment.NewLine + "Espere por favor...", 0);

                    await Task.Delay(TiempoEsperaMensajes);
                }
                else
                {
                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Servidor Central Detectado... \n");

                    EnviaraSync("Sincronizando..." + Environment.NewLine + Environment.NewLine + "Espere por favor...", 0);

                    await Task.Delay(TiempoEsperaMensajes);
                }

                //Log 
                if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Intetando conexión con Servidor");


                if (syncSAM.ServerNube)
                {

                    //if (syncSAM.PruebaCloud(confXML.PuertoWS)) lógica Anterior
                    //Powered ByRED 18ENE2022 No se le envía puerto esta vez, lo toma de una clase interna
                    //Se manda a abrir y probar la conexión hacia el WebService de CAN
                    if (syncSAM.PruebaCloud())
                    {
                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Conectado a servidor");

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Sincronizando Fecha y Hora con Servidor...");

                        //Sincronizo la Fecha y Hora con el Server
                        syncSAM.CambiarFechaHora();

                        // Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Fecha y Hora Sincronizados...");

                        //Sleep(1000);

                        EnviaraSync("La Fecha y Hora se" + Environment.NewLine + "Ajustaron:" + Environment.NewLine + DateTime.Now.ToString(), 0);

                        await Task.Delay(TiempoEsperaMensajes);

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Obteniendo Versiones de los sistemas");

                        //Obtenemos las versiones de los sistemas
                        syncSAM.SistemasVersiones(VMD_BD);

                        //si todo sale bien, regresará un True
                        return true;
                    }
                    else
                    {
                        //Si la conexión con el servidor no fue exitoso, entonces...


                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se pudo conectar a la red, Autobus: " + ParametrosInicio.Autobus);

                        EnviaraSync("No se pudo conectar al servidor, intente de nuevo...", 0);

                        await Task.Delay(TiempoEsperaMensajes);
                        return false;
                    }
                }
                else
                {
                    //Se manda a abrir la conexión para caso SQL
                    if (syncSAM.PruebaSQLServer())
                    {

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Conectado a servidor");

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Sincronizando Fecha y Hora con Servidor...");

                        //Sincronizo la Fecha y Hora con el Server
                        syncSAM.CambiarFechaHora();

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Fecha y Hora Sincronizados...");

                        //Sleep(1000);
 
                        EnviaraSync("La Fecha y Hora se" + Environment.NewLine + "Ajustaron:" + Environment.NewLine + DateTime.Now.ToString(), 0);

                        await Task.Delay(TiempoEsperaMensajes);

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Obteniendo Versión de SQLServer");

                        //Obtenemos la versión del Server
                        syncSAM.VersionSQL();

                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("Obteniendo Versiones de los sistemas");

                        //Obtenemos las versiones de los sistemas
                        syncSAM.SistemasVersiones(VMD_BD);

                        //Si todo el proceso está bien, regresará un true
                        return true;

                    }
                    else
                    {
                        //Si la conexión con el servidor no fue exitoso, entonces...


                        //Log
                        if ((bool)ParametrosInicio.LogSincronizacionBien) syncSAM.AgregarLogSync("No se pudo conectar a la red, Autobus: " + ParametrosInicio.Autobus);

                        EnviaraSync("No se pudo conectar al servidor, intente de nuevo...", 0);

                        await Task.Delay(TiempoEsperaMensajes);

                        return false;
                    }
                }
            });
    }
    #endregion

    /// <summary>
    /// Manda a actualizar los sistemas
    /// </summary>
    public void ActualizarSistemas()
    {
        foreach (ISistema x in ListaSistemas) x.Actualizar();
    }

    #endregion

    #region "Metodos Privados"
    /// <summary>
    /// Lanza la aplicación principal
    /// </summary>
    private void LanzarSAM()
    {
        CargaFront("SAM");
        //Mandamos a desactivar el wifi (por si quedara activo)
        WiFi(false, false);

        //Mandamos a encender el Adaptador de red por si tenemos SIA o Telematics
        //Powered ByRED 27MAY2021
        if ((bool)ParametrosInicio.SIA || (bool)ParametrosInicio.TELEMATICS)
        {
            Ethernet(true);
        }

        //Mandamos a mostrar la pantalla de Carga
        FE.MostrarCargando(true);

        //Llenamos la lista con los sistemas que ejecutaremos
        var res = CargaSistemas();

        //Validamos si todos los aplicativos que de deben de cargar
        //Existen para el funcionamiento correcto de SAM
        if (res.Equals(""))
        {
            //Iniciamos los sistemas cargados
            IniciaSistemas();
            //Iniciamos Timers
            IniciarTimers();

            FE.rutaVLC = this.RutaReproductor;

            //Lanzamos el Front
            if (!FE.HolaMundo())
            {//Hubo algún error al lanzar el Front

                //Mandamos a apagar el sistema
            }
        }
        else
        {
            FE.MostrarCargando(false);
            MessageBox.Show(res + Environment.NewLine + "Contacta con Soporte", "Error al cargar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //Validar que chingaos podemos hacer aquí
            //Para interrumpir el proceso goooe
        }

    }

    /// <summary>
    /// Configura la aplicación de acuerdo a los parametros
    /// introducidos en el modo Configuración
    /// </summary>
    private bool ConfiguraMovil()
    {
        try
        {

            //Encendemos el wifi durante la configuración para que pueda obtener el MacAddress
            WiFi(true, true);

            try
            {
                String rutaCarpeta;
                Process process;

                if (Environment.Is64BitOperatingSystem)
                {
                    rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                }
                else
                {
                    rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                }
                var rutaADOCAN = rutaCarpeta + @"\SIIAB - ADO\SIIAB - ADOCAN\Interface Servidor.exe";

                FileInfo _ADOCAN = new FileInfo(rutaADOCAN);

                if (_ADOCAN.Exists)
                {
                    process = new Process();
                    process.StartInfo.FileName = rutaADOCAN;

                    //Validamos si no existe

                    Process[] procesos = Process.GetProcesses();
                    foreach (Process p in procesos)
                    {
                        if (p.ProcessName == "Interface Servidor")
                        {
                            p.Kill();
                        }
                    }

                    process.Start();

                    Thread.Sleep(4000);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            CargaFront("CONFIG");

            //Lanzamos el front
            return FE.HolaConfig() ? true : false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// se encarga de configurar el FrontEngine de acuerdo al tipo 
    /// que se requiera, ya sea para lanzar la aplicación principal o
    /// para modo de configuración
    /// </summary>*
    private void CargaFront(String Tipo)
    {
        //Preparamos el FronEngine con los parametros que necesita saber

        switch (Tipo)
        {
            case "SAM":
                FE = new FrontEngine(ParametrosInicio.Autobus, confXML.ModoPrueba, ParametrosInicio.Password, ParametrosInicio.CodigoSalir, Version, ParSAM.ModoDeveloper, (bool)ParametrosInicio.BotonOff, (bool)ParametrosInicio.BotonPanico);
                FE.minsApagado = ParametrosInicio.MinApagadoSincronizacion.ToString();
                break;

            case "CONFIG":
                FE = new FrontEngine(confXML.ModoPrueba, ParametrosInicio.Password, ParametrosInicio.CodigoSalir, Version, ParSAM.ModoDeveloper, (bool)ParametrosInicio.BotonOff);
                break;
        }

        //Asignamos Eventos provenientes del FRONT
        AsignarEventosFront();
    }

    /// <summary>
    /// Configura e inicia los timers de los sistemas
    /// </summary>
    private void IniciarTimers()
    {
        timerActualizaSys.Interval = 250;
        timerActualizaSys.Enabled = true;
        timerActualizaSys.Tick += new EventHandler(tm_Tick);
    }

    /// <summary>
    /// Asigna los eventos provenientes del Front
    /// </summary>
    private void AsignarEventosFront()
    {
        //Para mandar a sincronizar los sistemas
        FE.EventoSyncSAM += eFSincronizar;

        //Para llamar al administrador de viaje
        FE.ViajeSAM += this.eFAdminViaje;

        //Para validar al conductor
        FE.ConductorSAM += this.eFValidarConductor;

        //Para Validar la existencia del protocolo
        FE.ProtocoloSAM += this.eFValidaProtocolo;

        //Para validar la poblacion
        FE.PoblacionSAM += this.eFVerificaPoblacion;

        //Para checar la población
        FE.ChecarPoblacion += this.eFPoblacion;

        //Para mandar a traer los datos del GPS
        FE.DatosGPSSAM += this.eFExtraerDatosGPS;

        //Para mandar a traer los datos de movtosCAN
        FE.DatosMovtosSAM += this.eFMovtosCAN;

        //Para validar si existe la región
        FE.SAMRegion += this.eFValidarRegion;

        //Para lanzar el SAM después de una configuración
        FE.SAM += this.LanzarSAM;

        //Recibe los parametros del modo configuración
        FE.ConfigSAM += this.eFGuardaConfiguracion;

        //Mandar a apagar o reiniciar el sistema
        FE.SAMCerrar += this.CerrarSistema;

        //Mandar a apagar el sistema cuando esté en modo Config
        FE.SAMCerrarConfig += this.CerrarSistemaConfig;

        //Para traer la lista de regiones
        FE.Regiones += this.ObtenerRegiones;

        //Manda a apagar o encender el wifi
        FE.WIFI += this.WiFi;

        //Manda a encender o a apagar el Ethernet
        //Powered ByRED 27MAY2021
        FE.ETHERNET += this.Ethernet;

        FE.Velocidad += this.eFVelocidad;

        FE.SAMPLAY += this.eFIniciarReproduccionVMD;
        FE.eVMDbuscarPauta += this.eFBuscaPauta;
        FE.eVMDreiniciarPauta += this.eFReiniciarPauta;
        FE.eVMDActualizaActividad += this.eFActualizaActividad;
        FE.eVMDActualizaUltimaVez += this.eFActualizaUltimaVez;
        FE.eVMDChecaSiguienteVideo += this.eFChecaSiguienteVideo;
        FE.eVMDValidaVolumen += this.eFValidaVolumen;
        FE.eVMDAgregarLog += this.eFAgregarLogVMD;

        FE.PautasVMD += this.eFObtenerListaPautas; //Powered ByRED16JUN2020
        FE.Pauta += this.eFPlancharPautaVMD; //Powered ByRED 16JUN2020
        FE.PautaUSB += this.eFRecuperarPautaUSB; //Powered ByRED 17JUN2020
        FE.SpotsVMD += this.eFObtenerListaSpots; //Powered ByToto16JUN2020
        //para hcer commit
        FE.ProgresoCopiado += this.eFPedirProgresoCopiado; //Powered ByRED 17JUN2020

        FE.ValidaPauta += this.eFValidaPautaVMD; //Powered ByRED 16JUL2020

        //Abordaje
        FE.AbordajeTrans += this.eFTransferenciaTVE;
        FE.TransferTVE += this.eFValidarTransferTVE;
        FE.EstadoTVE += this.eFEstadoTransferenciaTVE;
        FE.ConTVE += this.eFConexionTVE;
        FE.PassTVE += this.eFPassTVE;
        FE.FinTVE += this.eFTerminarTVE;
        FE.QrConsulta += this.eFGenerarQR;
        FE.InfoCorrida += this.eFInformacionCorrida;

        //Telematics
        FE.Telematics += this.eFReporteTelemetria;

        //MetasCAN Powered ByRED 10SEP2020
        FE.MetasCAN += this.eFObtenerMetasCAN;
        FE.ValidarMetasCAN += this.eFValidarMetasCAN;

        //SIA Powered ByRED 18MAR2021
        FE.MensajesSIA += this.eFObtenerMensajes;
        FE.MensajeSIA += this.eFMandarMensajeSIA;     
        FE.AlertaRobo += this.eFMandarAlertaRobo;//Powered ByRED 08JUN2021
    }

    /// <summary>
    /// Carga la configuración del XML y de BD.can_parametrosinicio
    /// </summary>
    private void CargaConfiguracion()
    {
        //Cargamos configuracion de XML
        confXML = new XML();

        //Cargamos Configuracion de BD
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();

        //Cargamos la clase de configuracion
        //Powered ByRED 10/SEP/2020
        _config = new Config();
    }

    /// <summary>
    /// Manda a cargar a la lista los sistemas seleccionados en la configuración
    /// </summary>
    private string CargaSistemas()
    {
        ListaSistemas = new List<ISistema>();

        ListaProcesos = new List<string>();

        String rutaCarpeta;
        Process process;

        if (Environment.Is64BitOperatingSystem)
        {
            rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }
        else
        {
            rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        //Reportamos la versión de SAM
        //Powered ByRED 15ENE2021
        ReportarVersion();

        //Cargamos PLAT

        if ((bool)ParametrosInicio.PLAT)
        {
            ListaSistemas.Add(new PLAT());
            var _PLAT = ListaSistemas.Where(x => x.Sistema == Sistema.PLAT).ToList();

            foreach (PLAT p in _PLAT)
            {
                var MyPlat = (PLAT)p;
                MyPlat.ModoPrueba = confXML.ModoPrueba;
            }
        }

        //Cargamos CAN

        if ((bool)ParametrosInicio.CAN)
        {
            ListaSistemas.Add(new CAN());

            FE.CAN = true;

            FE.GPS = true; //Powered ByRED 01/JUL/2020

            var _CAN = ListaSistemas.Where(x => x.Sistema == Sistema.CAN).ToList();

            foreach (CAN c in _CAN)
            {
                var MyCan = (CAN)c;

                MyCan.ModoPrueba = confXML.ModoPrueba;

                MyCan.EventoCAN += EventoCAN;
                this.CANConductor += MyCan.ValidarOperador;
                this.CANAdmin += MyCan.Viaje;
                MyCan.AvisarViaje += this.StatusViajeCAN;
                MyCan.EventoFirmaCondusat += this.CANToCondusat;
                MyCan.AvisarViajeSAM += this.RecibeViajeDeCAN;

            }

            //También cargamos la lógica de Abordaje
            MyAbordaje = new Abordaje();

            //Asignamos los eventos y parametros para abordaje
            this.TVETransfer += MyAbordaje.Transferencia;
            this.TransferTVE += MyAbordaje.ValidarTransferencia;
            this.EstadoTVE += MyAbordaje.EstadoTransferencia;
            this.ConexionTVE += MyAbordaje.EstadoConexionTVE;
            this.FinTVE += MyAbordaje.Finalizar;
            this.PassTVE += MyAbordaje.PassAbordaje;
            this.QrConsulta += MyAbordaje.GenerarQr;
            this.CorridaInfo += MyAbordaje.InformacionCorrida;


            //Preguntamos de Telemetría para cargar su lógica

            if ((bool)ParametrosInicio.TELEMATICS)
            {

                ListaSistemas.Add(new TELEMETRIA());

                FE.TELEMETRIA = true;

                //Asignamos Eventos de Telemetria
                var _Telematics = ListaSistemas.Where(x => x.Sistema == Sistema.TELEMETRIA).ToList();

                //Obtenemos los parametros de telemetria
                parametrostelematics ParTel = (from x in TELEMATICS_BD.parametrostelematics select x).FirstOrDefault();
                foreach (TELEMETRIA myTelematics in _Telematics)
                {
                    myTelematics.AlertaSAM += this.AlertaTELEMETRIA;
                    this.AlertaConductor += FE.RecibeAlertaSAM;

                    this.RefreshLedTelemetria += FE.RecibeLedDeTelemetria;
                    myTelematics.IndicadorLed += this.ActualizaIndicadorTelemetria;

                    this.ReporteTelemetria += myTelematics.GenerarReporte;

                    if (ParTel != null)
                    {
                        //Validamos si hay lotes resagados
                        myTelematics.VerificarLotes(ParTel.GuardarLote);
                    }
                }

                //Se trasladó ésta lógica hacia TELEMETRIA
                ////Preguntamos si deseamos que se guarden lo archivos de lote por más de 2 días            
                //parametrostelematics ParTel = (from x in TELEMATICS_BD.parametrostelematics
                //                               select x).FirstOrDefault();

                //if (ParTel != null)
                //{
                //    //PReguntamos si debemos de guardar el lote o no
                //    if (ParTel.GuardarLote == 0)
                //    {
                //        //Recuperamos los archivos para ver si se tiene que borrar algo
                //        try
                //        {
                //            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                //            foreach (var fi in di.GetFiles("*.zip"))
                //            {
                //                //Validamos si la fecha de creación del archivo es mayor a 2 días
                //                if((DateTime.Now - fi.LastWriteTime).Days >= 2)
                //                {
                //                    //De ser así, elimino la información
                //                    if (File.Exists(fi.Name))
                //                    {
                //                        File.Delete(fi.Name);
                //                    }
                //                }
                //            }
                //        }
                //        catch
                //        {

                //        }
                //    }
                //}
            }

            try
            {
                var rutaADOCAN = rutaCarpeta + @"\SIIAB - ADO\SIIAB - ADOCAN\Interface Servidor.exe";

                FileInfo _ADOCAN = new FileInfo(rutaADOCAN);

                if (_ADOCAN.Exists)
                {
                    process = new Process();
                    process.StartInfo.FileName = rutaADOCAN;

                    //Validamos si no existe

                    Process[] procesos = Process.GetProcesses();
                    foreach (Process p in procesos)
                    {
                        if (p.ProcessName == "Interface Servidor")
                        {
                            p.Kill();
                            break;//Powered ByRED 22ABR2021
                        }
                    }

                    process.Start();

                    ListaProcesos.Add(process.ProcessName);
                    Thread.Sleep(confXML.TiempoADOCAN);
                }
                else
                {
                    return "No se encontró Aplicativo ADOCAN";
                }
            }
            catch
            {

            }
        }

        //Iniciamos GPS
        if ((bool)ParametrosInicio.ADOGPS)
        {
            ListaSistemas.Add(new GPS());
            var _GPS = ListaSistemas.Where(x => x.Sistema == Sistema.GPS).ToList();
            foreach (GPS g in _GPS)
            {
                var MyGPS = (GPS)g;
                MyGPS.ModoPrueba = confXML.ModoPrueba;

                MyGPS.FechaInicioSistema = DateTime.Now;

                //Eventos Redisparadores hacia el Front
                //
                this.RefreshLedGPS += FE.RecibeLedDeGPS;


                //Eventos Provenientes de la lógica
                //
                //Iniciamos el evento para el indicador del GPS
                MyGPS.ActualizarIndicador += this.ActualizaIndicadorGPS;
                //Qué evento, disparará qué método
                //
                //Iniciamos el evento para el apagado del equipo
                MyGPS.RebootByGPS += this.ReinicioPorGps;
                //
                //Iniciamos el evento para el envío de datos hacia CAN
                MyGPS.MandarASAM += this.ultimosGPS;

                //Evento que manda a reiniciar la aplicación
                MyGPS.RebootGPS += this.ReiniciarADOGPS;

            }
            try
            {
                var rutaADOGPS = rutaCarpeta + @"\SIIAB - ADO\SIIAB - ADOGPS\ADOGPS.exe";

                FileInfo _ADOGPS = new FileInfo(rutaADOGPS);

                if (_ADOGPS.Exists)
                {
                    process = new Process();
                    process.StartInfo.FileName = rutaADOGPS;

                    //Validamos si no existe

                    Process[] procesos = Process.GetProcesses();
                    foreach (Process p in procesos)
                    {
                        if (p.ProcessName == "ADOGPS")
                        {
                            p.Kill();
                            break;//Powered ByRED 22ABR2021
                        }
                    }

                    process.Start();

                    ListaProcesos.Add(process.ProcessName);
                    Thread.Sleep(confXML.TiempoADOGPS);
                }
                else
                {
                    return "No se encontró Aplicativo ADOGPS";
                }
            }
            catch
            {

            }
        }

        //Cargamos Condusat

        if ((bool)ParametrosInicio.CONDUSAT)
        {
            ListaSistemas.Add(new CONDUSAT());

            FE.CONDUSAT = true;

            FE.GPS = true; //Powered ByRED 01/JUL/2020

            var _CONDUSAT = ListaSistemas.Where(x => x.Sistema == Sistema.CONDUSAT).ToList();

            foreach (CONDUSAT con in _CONDUSAT)
            {
                var MyCondusat = (CONDUSAT)con;
                MyCondusat.ModoPrueba = confXML.ModoPrueba;
                MyCondusat.ActualizarCondusat += EventoCondusat;

                //Le envio parametros
                MyCondusat.Param_ADOCAN = confXML.ADOCAN;
                MyCondusat.PuertoSocket = 3601;

                //Para relanzar de SAM hacia el Front
                //Qué evento, dispará qué método
                RefreshViewCondusat += FE.RecibeDeCondusat;
            }

            try
            {
                var rutaADOCONDUSAT = rutaCarpeta + @"\SIIAB - ADO\SIIAB - CONDUSAT\CONDUSAT.exe";

                FileInfo _FCONDUSAT = new FileInfo(rutaADOCONDUSAT);

                if (_FCONDUSAT.Exists)
                {
                    process = new Process();
                    process.StartInfo.FileName = rutaADOCONDUSAT;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                    Process[] procesos = Process.GetProcesses();

                    foreach (Process p in procesos)
                    {
                        if (p.ProcessName == "CONDUSAT")
                        {
                            p.Kill();
                            break;//Powered ByRED 22ABR2021
                        }
                    }

                    process.Start();
                    ListaProcesos.Add(process.ProcessName);
                    Thread.Sleep(confXML.TiempoCONDUSAT);
                }
                else
                {
                    return "No se encontró Aplicativo CONDUSAT";
                }
            }
            catch
            {

            }

        }


        //Cargamos SIA O miniSIA
        //CLAUS & ROJO
        if ((bool)ParametrosInicio.SIA || (bool)ParametrosInicio.MiniSIA)
        {
            if ((bool) ParametrosInicio.MiniSIA)
            {
                ListaSistemas.Add(new SIA(true));
                FE.MINISIA = true;
                this.MiniSIA = true; //Powered ByRED 21ABR2021
            }
            else
            {
                ListaSistemas.Add(new SIA());

                FE.SIA = true;

                //Powered ByRED 12ABR2021
                FE.HabilitarCintillo = (bool)ParametrosInicio.HabilitarCintillo;
                if (FE.HabilitarCintillo)
                {//Cargamos de los demás parametros
                    FE.MensajeInicialCintillo = ParametrosInicio.MensajeInicial;
                    FE.VueltasMensajeCintillo = (long)ParametrosInicio.VueltasMensaje;
                    FE.AltoCintillo = (long)ParametrosInicio.AltoCintillo;
                    FE.ColorFuenteCintillo = ParametrosInicio.ColorFuente;
                    FE.TamanioFuenteCintillo = ParametrosInicio.TamanoFuente;
                    FE.ColorFondoCintillo = ParametrosInicio.ColorFondo;
                    FE.VelocidadCintillo = (long)ParametrosInicio.Velocidad;
                    FE.PosicionMarqueeCintillo = ParametrosInicio.PosicionMarquee;
                    FE.TimerCintilloSegundos = (long)ParametrosInicio.tmrSegVMDSIA;
                }

                //Powered ByRED 22ABR2021
                try
                {
                    var rutaSIA = rutaCarpeta + @"\ADO\SIIAB - Sistema Internet Abordo\Internet_GPS.exe";

                    FileInfo _SIAmovil = new FileInfo(rutaSIA);

                    if (_SIAmovil.Exists)
                    {
                        process = new Process();
                        process.StartInfo.FileName = rutaSIA;

                        //Validamos si no existe

                        Process[] procesos = Process.GetProcesses();
                        foreach (Process p in procesos)
                        {
                            if (p.ProcessName == "Internet_GPS")
                            {
                                p.Kill();
                                break;
                            }
                        }

                        process.Start();

                        ListaProcesos.Add(process.ProcessName);
                        Thread.Sleep(confXML.TiempoSIA);
                    }
                    else
                    {
                        return "No se encontró Aplicativo SIA Móvil";
                    }
                }
                catch
                {

                }

            }

            var _SIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

            foreach (SIA s in _SIA)
            {
                var MySIA = (SIA)s;
                MySIA.ModoPrueba = confXML.ModoPrueba;
                MySIA.PuertoSocket = 3502;

                //Configuramos Eventos dentro de SIA
                MySIA.RecuperaParametros += this.ParametrosaSIA;
                MySIA.MandarMensajeSAM += this.MensajeSIAaFront;
                MySIA.statusInternet += this.EstadoInternetFront;
                MySIA.GPS += this.MandaGPSSIA;//CLAUS && ROJO
                MySIA.POI_SAM += this.POISIAaFRONT;//Powered ByRED 23MAR2021

                //Configuramos Eventos para enviar al Front
                MensajeSIA += FE.RecibeMensajeDeSIA;
                InternetSIA += FE.RecibeInternetSIA;
                POISIA += FE.RecibePOISIA;//Powered ByRED 23MAR2021
            }
        }
           

        //Cargamos VMD

        if ((bool)ParametrosInicio.VMD)
        {
            ListaSistemas.Add(new VMD());

            FE.VMD = true;

            var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

            foreach (VMD v in _VMD)
            {
                var MyVMD = (VMD)v;
                MyVMD.ModoPrueba = confXML.ModoPrueba;
                MyVMD.evRepPlay += VMDToFront;
                
                EPlay += FE.Func_RepPlay;
                ECintilloInicial += FE.Func_ReproducirCintillo;
                ECintillo += FE.Func_ReproducirCintillo;
                PautaVMD += MyVMD.CargarPauta;

                //Powered ByRED 16/JUL/2020
                ValidaPautaVMD += MyVMD.ValidadPauta;


                if (confXML.ReinicioAutomatico)
                {
                    MyVMD.inicializaVMD();
                }
            }

            //Verificamos la existencia del Reproductor VLC

            try
            {
                var _rutaVLC = rutaCarpeta + @"\VideoLAN\VLC\vlc.exe";

                FileInfo _FVLC = new FileInfo(_rutaVLC);

                if (_FVLC.Exists)
                {
                    RutaReproductor = rutaCarpeta + @"\VideoLAN\VLC";
                }
                else
                {
                    return "No se encontró Aplicativo VLC";
                }
            }
            catch
            {

            }

        }

        Thread.Sleep(5000);

        return "";
    }

    /// <summary>
    /// Método que se encarga de preparar todo para apagar el sistema
    /// Tipos de apagado:
    /// Salir = false || Manda a cerrar sólo el programa
    /// Salir = True || Manda a Cerrar el programa y también apaga el equipo
    /// </summary>
    /// <param name="Salir"></param>
    private void CerrarSistema(bool Salir = false)
    {
        //Detengo Timers
        timerActualizaSys.Stop();

        //Mandamos a finalizar los sistemas
        DetenerSistemas();

        //Mandamos a finalizar el Front
        FE.TerminarFront();
        FE.Dispose();

        ///Si estamos en modo prueba o queremos salir, 
        ///sólo cerramos la aplicación sin apagar
        ///el equipo
        //Se modificó las lineas de arriba
        //if (confXML.ModoPrueba || Salir) { return; }

        //Cambios Autorizados: Gilberto Chavarría
        //Sin importar si está en modo prueba o no, el equipo se apaga
        if (Salir) { return; }

        //Configuramos el proceso para apagar
        var apagar = ConfigurarProceso(0);

        //Si estoy en modo Developer no lo apago
        if (!ParSAM.ModoDeveloper)
        {
            //Terminamos los procesos
            TerminarProcesoshijos();

            //Si el Xml está en ModoPrueba lo quitamos para el siguiente inicio del sistema
            //Cambios autorizados Gilberto Chavarría
            if (confXML.ModoPrueba)
            {
                confXML.ModoPrueba = false;
                ArchivoXML.CrearXML(confXML);
            }

            //Mandamos a apagar
            apagar.Start();
        }
    }

    /// <summary>
    /// /Se encarga de cerrar el sistema si se encuentra en modoConfiguración
    /// </summary>
    private void CerrarSistemaConfig()
    {
        var apagar = ConfigurarProceso(0);

        //Si no se encuentra en ModoDev, apagamos el sistema
        if (!ParSAM.ModoDeveloper)
        {
            //Mandamos a apagar
            apagar.Start();
        }
    }

    /// <summary>
    /// Se ejecuta despues de la configuración de la aplicación
    /// </summary>
    private void ReiniciarSistema(bool ReinicioAutomatico = false)
    {
        //Verificamos si es Reinicio automatico o por configuración
        if (ReinicioAutomatico)
        {
            //Guardamos en el XML que fué reinicio automatico
            confXML.ReinicioAutomatico = true;

            ArchivoXML.CrearXML(confXML);

            //Detengo Timers
            timerActualizaSys.Stop();

            //Mandamos a finalizar los sistemas
            DetenerSistemas();

            //Mandamos a finalizar el Front
            FE.TerminarFront();
            FE.Dispose();
        }
        else
        {
            //Mandamos a finalizar el Front
            FE.TerminarConfigFront();
            FE.Dispose();
        }

        //Configuramos el reinicio del sistema
        var Reboot = ConfigurarProceso(1);

        if (!ParSAM.ModoDeveloper)
        {
            Reboot.Start();
        }
    }

    /// <summary>
    /// Nos sirve para configurar el tipo de proceso que creará
    /// Apagar: 0
    /// Reiniciar: 1
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    private Process ConfigurarProceso(int tipo)
    {
        Process Proceso = new Process();
        Proceso.StartInfo.UseShellExecute = false;
        Proceso.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        Proceso.StartInfo.RedirectStandardOutput = true;
        Proceso.StartInfo.FileName = "shutdown.exe";

        if (tipo == 1)
        {
            //mandamos a reiniciar el equipo
            Proceso.StartInfo.Arguments = "/r /f /t 1";
        }
        else if (tipo == 0)
        {
            //Mandamos a apagar el equipo
            Proceso.StartInfo.Arguments = "/s /f /t 1";
        }

        return Proceso;
    }

    /// <summary>
    /// Se encarga de finalizar los procesos externos que SAM ejecutó
    /// </summary>
    private void TerminarProcesoshijos()
    {
        Process[] procesos = Process.GetProcesses();

        try
        {
            foreach (string name in ListaProcesos)
            {
                foreach (Process proceso in procesos)
                {
                    if (proceso.ProcessName == name)
                    {
                        proceso.Kill();
                    }
                }
            }

        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de obtener las regiones y enviarlas al front
    /// para la configuración de la región
    /// </summary>
    /// <returns></returns>
    private List<string> ObtenerRegiones()
    {
        List<can_referenciaregion> Regiones = (from x in VMD_BD.can_referenciaregion
                                               select x).ToList();

        List<string> ListaReg = new List<string>();

        foreach (can_referenciaregion reg in Regiones)
        {
            ListaReg.Add(reg.IdRegion.ToString() + " - Region " + reg.Region);
        }


        return ListaReg;
    }

    /// <summary>
    /// se encarga de ejecutar el script para encender
    /// o apagar el wifi según se requiera
    /// </summary>
    /// <param name="encender"></param>
    private bool WiFi(bool _encender, bool transferencia)
    {
        var syncSAM = new SyncSAM();

        return syncSAM.Wifi(_encender, ParametrosInicio.IDWIFI, transferencia, ParametrosInicio.Autobus);
    }

    /// <summary>
    /// se encarga de encender/apagar el adaptador de red Ethernet
    /// Powered ByRED 27MAY2021
    /// </summary>
    /// <param name="Encender"></param>
    /// <returns></returns>
    private bool Ethernet(bool Encender)
    {
        if((bool)ParametrosInicio.SIA || (bool)ParametrosInicio.TELEMATICS)
        {
            var syncSAM = new SyncSAM();

            return syncSAM.Ethernet(Encender, ParametrosInicio.IdLAN);
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Se encarga de avisar el front si debe de entrar en modo
    /// nocturno
    /// </summary>
    private void VerificaModoNocturno()
    {
        DateTime inicio = DateTime.ParseExact(ParametrosInicio.horaInicioModoNocturno, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        DateTime final = DateTime.ParseExact(ParametrosInicio.horaFinModoNocturno, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);


        if (DateTime.Now < final || DateTime.Now > inicio)
        {
            //Debe cambiar a modo Nocturno
            if (!FE.ModoNocturno)
            {
                FE.ActivarModoNocturno(true);
                FE.ModoNocturno = true;
            }
        }
        else
        {
            //Debe cambiar a modoDiruno
            if (FE.ModoNocturno)
            {
                FE.ActivarModoNocturno(false);
                FE.ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// Se encarga de reportar la versión a la tabla de Plat_versiones
    /// Powered ByRED 15ENE2021
    /// </summary>
    private void ReportarVersion()
    {
        try
        {
            //Mandamos a planchar la version de SAM en la tabla Versiones de PLAT

            var plat_versiones = (from x in VMD_BD.plat_versiones
                                  select x).ToList();

            if (plat_versiones != null)
            {
                var version_sam = plat_versiones.Where(x => x.Sistemas.Equals("SAM")).FirstOrDefault();

                //No existe el registro en la tabla, lo agregamos
                if (version_sam == null)
                {
                    plat_versiones nuevaVersion = new plat_versiones();

                    nuevaVersion.Sistemas = "SAM";
                    nuevaVersion.Versiones = Application.ProductVersion;

                    VMD_BD.plat_versiones.Add(nuevaVersion);

                }
                else//sólo planchamos la version
                {
                    version_sam.Versiones = Application.ProductVersion;
                }

                VMD_BD.SaveChanges();
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de cargar o Recargar la lógica de VMD
    /// </summary>
    private void CargarlogicaVMD()
    {

    }
    #endregion

    #region "Manejador de Eventos"

    /// <summary>
    /// se encarga de regresar la velocidad del sistema
    /// de acuerdo a si tenemos condusat o no
    /// </summary>
    private int eFVelocidad()
    {
        if ((bool)ParametrosInicio.CONDUSAT)
        {
            return VelocidadCONDUSAT;
        }
        else if ((bool)ParametrosInicio.CAN)
        {
            return VelocidadCAN;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// ëste método lo dispara el Form de Sync
    /// </summary>
    /// <param name="VistaSync"></param>
    public void eFSincronizar()
    {
        //Declaramos los eventos a ocupar durante la sincronización

        if (EventoSync == null)
        {
            EventoSync += FE.EventoSync;
        }

        if (SyncOK == null)
        {
            SyncOK += FE.SyncOK;
        }

        //Mandamos a sincronizar
        this.SincronizarSistemas();
    }

    /// <summary>
    /// Método que se encarga de disparar los mensajes hacia el FormSync
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="final"></param>
    private void EnviaraSync(string mensaje, int final)
    {
        EventoSync(mensaje, final);
    }

    /// <summary>
    /// Recibe información de CAN
    /// </summary>
    /// <param name="FrMeta"></param>
    /// <param name="FRActual"></param>
    /// <param name="kms"></param>
    /// <param name="FR"></param>
    private void EventoCAN(double _FrMeta, double _FRActual, double _kms, double _lts, bool _FRIndicador, bool _Protocolo, double VelCAN)
    {
        FE.FR_indicador = _FRIndicador;
        FE.FR_Meta = _FrMeta;
        FE.FR_Real = _FRActual;
        FE.Kms = _kms;
        FE.Lts = _lts;
        FE.protocoloCAN = _Protocolo;

        try
        {
            VelocidadCAN = Convert.ToInt32(VelCAN);
        }
        catch
        {
            VelocidadCAN = 0;
        }
    }

    /// <summary>
    /// Recibe información de CONDUSAT
    /// </summary>
    /// <param name="vel"></param>
    private void EventoCondusat(string ColorVer, int VelReal, int VelMax, string _imgAdvertencia)
    {
        //Guardamos la velocidad que viene de CONDUSAT
        VelocidadCONDUSAT = VelReal;

        //Desencadena el siguiente evento hacia el front
        RefreshViewCondusat(ColorVer, VelReal, VelMax, (bool)ParametrosInicio.CAN, confXML.ADOCAN, _imgAdvertencia);
    }

    /// <summary>
    /// Se encarga de recojer las versiones que estan guardadas en la tabla
    /// de PLAT_versiones
    /// </summary>
    /// <param name="caso"></param>
    /// <returns></returns>
    public string MostrarVersiones(int caso)
    {
        DatosSistemas = new DatosRegistros();
        string Mensaje = string.Empty;
        switch (caso)
        {
            case 1:

                //Para Obtener la versión de cuadros de Condusat
                Mensaje = DatosSistemas.VerCuadrosCONDUSAT(CONDUSAT_BD);
                break;
        }
        return Mensaje;
    }

    /// <summary>
    /// Se encarga de relanzar el evento hacia el Front
    /// </summary>
    /// <param name="activo"></param>
    private void ActualizaIndicadorGPS(int Estado)
    {
        //Desencadena el siguiente evento hacia el front
        RefreshLedGPS(Estado, false);
    }

    /// <summary>
    /// Se encarga de enviar datos de firma de viaje de CAN a CONDUSAT
    /// </summary>
    /// <param name="_autobus"></param>
    /// <param name="_operador"></param>
    /// <param name="_tipo"></param>
    /// <param name="_fechaapertura"></param>
    /// <param name="_fechacierre"></param>
    /// <param name="_cambioManos"></param>
    private void CANToCondusat(string _autobus, string _operador, string _tipo, string _fechaapertura, string _fechacierre, string _cambioManos)
    {
        if ((bool)ParametrosInicio.CONDUSAT)
        {
            var _condusat = ListaSistemas.Where(x => x.Sistema == Sistema.CONDUSAT);
            foreach (CONDUSAT myCondusat in _condusat)
            {
                myCondusat.FirmarCAN(_autobus, _operador, _tipo, _fechaapertura, _fechacierre, _cambioManos);
            }
        }
    }

    /// <summary>
    /// Recibe los datos de viaje de CAN para que SAM esté enterado y otros
    /// sistemas puedan consusmir estos datos
    /// </summary>
    /// <param name="_claveOperador"></param>
    /// <param name="_nombreOperador"></param>
    /// <param name="_enViaje"></param>
    /// <param name="_fechaViaje"></param>
    private void RecibeViajeDeCAN(int _claveOperador, string _nombreOperador, bool _enViaje, DateTime _fechaViaje, long _idpob, string _despob, string _cvepob)
    {
        this.Clave_Operador = _claveOperador;
        this.Nombre_Operador = _nombreOperador;
        this.EnViaje = _enViaje;
        this.FechaViaje = _fechaViaje;

        //if telemtria
        var _telemetria = ListaSistemas.Where(x => x.Sistema == Sistema.TELEMETRIA).ToList();
        foreach (TELEMETRIA mytelemetria in _telemetria)
        {
            mytelemetria.Clave_operador = _claveOperador;
            mytelemetria.Nombre_Operador = _nombreOperador;
            mytelemetria.EnViaje = _enViaje;
            mytelemetria.FechaViaje = _fechaViaje;
            mytelemetria.IdPob = _idpob;
            mytelemetria.DescPob = _despob;
            mytelemetria.CvePob = _cvepob;
        }

    }

    /// <summary>
    /// Se encarga de Avisar a los sistemas si hay un viaje abierto, en caso de un inicio de sistema
    /// </summary>
    /// <param name="activo"></param>
    public void StatusViajeCAN(string operador, string nom_operador)
    {
        //Mandamos el status al Front
        FE.RecuperarViaje(operador, nom_operador);

        //mandamos el status a GPS
        var _gps = ListaSistemas.Where(x => x.Sistema == Sistema.GPS);
        foreach (GPS myGPS in _gps)
        {
            myGPS.ViajeAbierto = true;
        }
    }

    /// <summary>
    ///Se encarga de mandar a reinciar el equipo  
    /// </summary>
    /// <param name="pasajero"></param>
    /// <param name="mensaje"></param>
    private void ReinicioPorGps(bool pasajero, string mensaje)
    {
        //debemos de lanzar el form de error para que mande a reiniciar el sistema

        //avisamos si debemos avisar al pasajero

        //Escribimos en el archivo de config que es un reinicio automático
    }

    /// <summary>
    /// Sirve para mandar a reiniciar la aplicación de GPS
    /// </summary>
    private void ReiniciarADOGPS()
    {
        try
        {
            //Mandamos a detener el GPS si es que se está ejecutando
            Process[] procesos = Process.GetProcesses();
            foreach (Process p in procesos)
            {
                if (p.ProcessName == "ADOGPS")
                {
                    p.Kill();
                }
            }
        }
        catch (Exception ex)
        {
            var hola = ex.ToString();
        }

        ListaProcesos.Remove("ADOGPS");

        String rutaCarpeta;
        Process process;

        if (Environment.Is64BitOperatingSystem)
        {
            rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }
        else
        {
            rutaCarpeta = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        var rutaADOGPS = rutaCarpeta + @"\SIIAB - ADO\SIIAB - ADOGPS\ADOGPS.exe";

        process = new Process();
        process.StartInfo.FileName = rutaADOGPS;

        process.Start();

        ListaProcesos.Add(process.ProcessName);

    }


    //Se deja para que no se olvide y se valide al último, pero no creo que sea necesario
    private void ChecaUltimoGPS()
    {


    }

    /// <summary>
    /// Se encarga de enviar los últimos registros de GPS hacia CAN
    /// </summary>
    /// <param name="_Datos_GPS"></param>
    /// <returns></returns>
    private void ultimosGPS(GPSData _Datos_GPS)
    {
        //Guardamos los datos de GPS en SAM
        this.Datos_GPS = _Datos_GPS;

        var MyCAN = ListaSistemas.Where(x => x.Sistema == Sistema.CAN).ToList();

        foreach (CAN _can in MyCAN)
        {
            if (_can._Globales != null)
            {
                _can._Globales.UltLat = _Datos_GPS.Latitud;
                _can._Globales.UltLatNS = _Datos_GPS.LatitudNS;
                _can._Globales.UltLon = _Datos_GPS.Longitud;
                _can._Globales.UltLonWE = _Datos_GPS.LongitudWE;
                _can._Globales.UltVel = _Datos_GPS.Velocidad;
            }
        }

        //Se los mandamos también a telemetria
        var MyTelematics = ListaSistemas.Where(x => x.Sistema == Sistema.TELEMETRIA).ToList();
        foreach(TELEMETRIA _telemetria in MyTelematics)
        {
            _telemetria.Datos_GPS = _Datos_GPS;
        }

    }

    /// <summary>
    /// Reproducción del video
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <param name="rutaVideo"></param>
    /// <param name="MinutosMax"></param>
    /// <param name="detenerVideo"></param>
    /// <param name="playSobrePlay"></param>
    /// <param name="posicion"></param>
    private void VMDToFront(int idArchivo, string rutaVideo, int MinutosMax, bool detenerVideo, bool playSobrePlay, double posicion)
    {
        EPlay(idArchivo, rutaVideo, MinutosMax, detenerVideo, playSobrePlay, posicion);

        //Si tenemos sía activado, iniciamos los cintillos
        var MySIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();
        foreach (SIA _sia in MySIA)
        {
            //Func_IniciarMensajesSIA();
        }
    }

    /// <summary>
    /// Evento que ejecuta la validación del conductor en CAN
    /// </summary>
    /// <param name="clvConductor"></param>
    /// <returns></returns>
    private string eFValidarConductor(string clvConductor)
    {
        return CANConductor(clvConductor);
    }

    /// <summary>
    /// Evento que ejecuta el Administrador de viajes en CAN
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    private bool eFAdminViaje(string tipo, string clvConductor)
    {
        return CANAdmin(tipo, clvConductor, this.CANPob);
    }

    /// <summary>
    /// Verifica si el sistema GPS detectó alguna poblacion
    /// de lo contrario manda un falso al front para que 
    /// levante el form de Clave de Población
    /// </summary>
    /// <returns></returns>
    private bool eFPoblacion()
    {
        var MyGPS = ListaSistemas.Where(x => x.Sistema == Sistema.GPS).ToList();

        foreach (GPS _gps in MyGPS)
        {
            this.GPS = _gps.GPSActivo;
        }

        return this.GPS;
        #region "Por si las moscas"

        //var MyGPS = ListaSistemas.Where(x => x.Sistema == Sistema.GPS).ToList();

        //foreach (GPS _gps in MyGPS)
        //{
        //    this.CANPob = _gps.PobActual;
        //}

        //if (this.CANPob != null)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}

        #endregion
    }

    /// <summary>
    /// Verifica si existe la población introducida
    /// </summary>
    /// <param name="ClvPob"></param>
    /// <returns></returns>
    private bool eFVerificaPoblacion(string ClvPob)
    {
        CANPob = (from x in VMD_BD.can_poblaciones
                  where x.CVEPOB == ClvPob
                  select x).FirstOrDefault();

        if (this.CANPob != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Evento que consulta en el sistema si tenemos protocolo de CAN
    /// conectado para pode determinar si se puede abrir viaje o no
    /// </summary>
    /// <returns></returns>
    private bool eFValidaProtocolo()
    {
        var _CAN = ListaSistemas.Where(x => x.Sistema == Sistema.CAN).ToList();


        foreach (CAN c in _CAN)
        {
            try
            {
                var MyCan = (CAN)c;
                return MyCan.Protocolo;
            }
            catch
            {
                return false;
            }
        }

        return false;

    }

    /// <summary>
    /// Se encarga de extraer los datos del GPS para enviarlos al FRONT
    /// </summary>
    /// <returns></returns>
    private List<string> eFExtraerDatosGPS()
    {
        List<string> Datos = new List<string>();

        if (Datos_GPS != null)
        {
            Datos.Add(Datos_GPS.Latitud);
            Datos.Add(Datos_GPS.LatitudNS);
            Datos.Add(Datos_GPS.Longitud);
            Datos.Add(Datos_GPS.LongitudWE);
            Datos.Add(Math.Round(Datos_GPS.Velocidad, 2).ToString());
        }
        else
        {
            Datos.Add("-");
        }

        return Datos;
    }

    /// <summary>
    /// Se encarga de recopilar la información para ser mostrada
    /// en el menu general de CAN (MovtosCAN)
    /// </summary>
    /// <returns></returns>
    private List<string> eFMovtosCAN()
    {
        List<string> Datos = new List<string>();
        try
        {
            //Agregamos la cantidad de registros de movtoscan
            Datos.Add(VMD_BD.can_movtoscan.Count().ToString());

            //Agregamos el numero de autobus
            Datos.Add(ParametrosInicio.Autobus);

            //Agregamos la versión de la App
            Datos.Add(this.Version);

            //Agregamos las versiones de condusat
            Datos.Add((from x in VMD_BD.plat_versiones
                       where x.Sistemas.Equals("SIIAB-CONDUSAT")
                       select x.Versiones).FirstOrDefault() + " / VC:" + (from t in CONDUSAT_BD.versiones
                                                                          where t.tabla == "cuadro"
                                                                          select t.version).FirstOrDefault());

        }
        catch (Exception ex)
        {
        }
        return Datos;
    }

    /// <summary>
    /// Valida el numero de region
    /// </summary>
    /// <param name="reg"></param>
    /// <returns></returns>
    private string eFValidarRegion(long reg)
    {
        return _config.VerificarRegion(this.VMD_BD, reg);
    }

    /// <summary>
    /// Se encarga de guardar la configuración de la aplicación
    /// </summary>
    /// <param name="_TipoCAN"></param>
    /// <param name="_Antivirus"></param>
    private void eFGuardaConfiguracion(int _TipoCAN, bool _Antivirus)
    {
        try
        {
            ParametrosInicio.CAN = FE.CAN;
            ParametrosInicio.VMD = FE.VMD;
            ParametrosInicio.CONDUSAT = FE.CONDUSAT;
            ParametrosInicio.PLAT = FE.PLAT;
            ParametrosInicio.SIA = FE.SIA;
            ParametrosInicio.SIIAB_POI = FE.SIIAB_POI;

            //Powered ByRED2021 & ClausMolko
            //Por Regla de negocio, no se puede tener ambos funcionando
            //Si se tiene MiniSIA, SIA debe de estar desactivado
            if (FE.MINISIA)
            {
                ParametrosInicio.MiniSIA = true;
                ParametrosInicio.SIA = false;
            }
            else
            {
                ParametrosInicio.MiniSIA = false;
            }

            ParametrosInicio.TELEMATICS = FE.TELEMETRIA;

            //Logica para ADOGPS 
            //Para controlar la ejecución no necesaria de ADOGPS
            //Powered ByRED 16/JUL/2020
            if( FE.CAN || FE.CONDUSAT)
            {
                ParametrosInicio.ADOGPS = true;
            }
            else
            {
                ParametrosInicio.ADOGPS = false;
            }


            ParametrosInicio.Autobus = FE.NumAutobus;

            ParametrosInicio.NombreTipoMetaCAN = FE.tempNombreMetaCAN;

            ////Para el perfil de Wifi Realtek
            //var perfilWifi = new RegistroWifi.Perfil();
            //perfilWifi.CrearLlave(FE.NumAutobus);

            ////Configuramos el orden de descarga

            try
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;

                //Borramos la tabla de Orden de descarga
                objCtx.ExecuteStoreCommand("Truncate Table orden_descarga");

                //Borramos la tabla de MovTosCAN
                objCtx.ExecuteStoreCommand("Truncate Table can_movtoscan");

                //Borramos la tabla de logprogramacion de VMD
                objCtx.ExecuteStoreCommand("Truncate Table logprogramacion");
            }
            catch
            {

            }

            try
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)CONDUSAT_BD).ObjectContext;

                //Borramos la tabla de evento de CONDUSAT
                objCtx.ExecuteStoreCommand("Truncate Table evento");

                //Boramos la tabla de puntualidad de CONDUSAT
                objCtx.ExecuteStoreCommand("Truncate Table llegadas_salidas");
            }
            catch
            {

            }

            try
            {
                var objctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)TELEMATICS_BD).ObjectContext;

                //Boramos la tabla de codigos de telemetria
                objctx.ExecuteStoreCommand("Truncate Table codigo");
            }
            catch
            {

            }

            //Borramos los archivos temporales dañinos o resagados .zip tanto de movtoscan como de telemetria

            //Recupramos los archivos que se tienen que mandar
            try
            {
                DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var fi in di.GetFiles("*.zip"))
                {
                    if (File.Exists(fi.Name))
                    {
                        File.Delete(fi.Name);
                    }
                }
            }
            catch
            {

            }

            //Contador para el orden
            int i = 1;

            orden_descarga nuevoOrden;
            //Para PLAT
            if (FE.PLAT)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "PLAT";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);

                i++;
            }

            //Para CONDUSAT
            if (FE.CONDUSAT)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "CONDUSAT";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);

                i++;
            }

            //Para VMD
            if (FE.VMD)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "VMD";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);

                i++;
            }

            //Para el Antivirus
            if (_Antivirus)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "Antivirus";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);

                i++;
            }

            //Powered ByRED 21ABR2021
            if (FE.MINISIA)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "SIA";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);

                i++;
            }

            //Para CAN

            if (FE.CAN)
            {
                nuevoOrden = new orden_descarga();

                nuevoOrden.Sistema = "CAN";
                nuevoOrden.Orden = i;

                VMD_BD.orden_descarga.Add(nuevoOrden);
            }

            VMD_BD.SaveChanges();

            //Mandamos configurar la tarjeta de CAN

            var Mycan = new CAN();

            switch (_TipoCAN)
            {
                case 0:
                    break;

                case 2: Mycan.ConfigurarCAN("DG");
                    break;

                case 4: Mycan.ConfigurarCAN("DIDCOM");
                    break;

                case 5: Mycan.ConfigurarCAN("DIDCOM_TELEMETRIA");
                    break;

                default: Mycan.ConfigurarCAN("ST");
                    break;
            }



            //Se comenta para pasarlo al inicio de la aplicación
            ////Mandamos a planchar la version de SAM en la tabla Versiones de PLAT
            //var plat_versiones = (from x in VMD_BD.plat_versiones
            //                      select x).ToList();

            //if (plat_versiones != null)
            //{
            //    var version_sam = plat_versiones.Where(x => x.Sistemas.Equals("SAM")).FirstOrDefault();

            //    //No existe el registro en la tabla, lo agregamos
            //    if (version_sam == null)
            //    {
            //        plat_versiones nuevaVersion = new plat_versiones();

            //        nuevaVersion.Sistemas = "SAM";
            //        nuevaVersion.Versiones = Application.ProductVersion;

            //        VMD_BD.plat_versiones.Add(nuevaVersion);

            //    }
            //    else//sólo planchamos la version
            //    {
            //        version_sam.Versiones = Application.ProductVersion;
            //    }

            //    VMD_BD.SaveChanges();
            //}

            //Config _config = new Config(); //Powered ByRED 10/SEP/2020

            //Cambiamos el nombre del equipo
            if (!ParSAM.ModoDeveloper)
            {
                _config.CambiarNombreEquipo(ParametrosInicio.Zona + "-" + ParametrosInicio.Region + "-" + ParametrosInicio.Autobus);
            }

            //Obtenemos la MACAddress
            _config.InsertarMacAddress(VMD_BD, _config.ObtenerMacAddress());

            //Destruios la variable global de configuracion
            //Powered ByRED 10/SEP/2020
            _config = null;

            //Detenemos la aplicacion de configuración en el SAM

            //Mandamos a iniciar SAM

        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// Se encarga de ejecutar la logica de transferencia de Abordaje Electronico
    /// </summary>
    private Task<bool> eFTransferenciaTVE()
    {
        return TVETransfer();
    }

    /// <summary>
    /// Se encarga de obtener el estado de la tranferencia de TVE
    /// </summary>
    /// <returns></returns>
    private Task<bool> eFValidarTransferTVE()
    {
        return TransferTVE();
    }

    /// <summary>
    /// Se encarga de obtener el resultado de la transferencia
    /// de TVE
    /// </summary>
    /// <returns></returns>
    private bool eFEstadoTransferenciaTVE()
    {
        return EstadoTVE();
    }

    /// <summary>
    /// Obtiene el estado de la conexión con TVE
    /// </summary>
    /// <returns></returns>
    private Task<bool> eFConexionTVE()
    {
        return ConexionTVE();
    }

    /// <summary>
    /// Obtiene el pass de TVE
    /// </summary>
    /// <returns></returns>
    private string eFPassTVE()
    {
        return PassTVE();
    }

    /// <summary>
    /// Genera los Qr para consulta
    /// </summary>
    /// <returns></returns>
    private Task<bool> eFGenerarQR()
    {
        return QrConsulta();
    }

    /// <summary>
    /// obtiene la informacion de la corrida de Abordaje
    /// </summary>
    /// <returns></returns>
    private string eFInformacionCorrida()
    {
        return CorridaInfo();
    }

    /// <summary>
    /// se encarga de terminar con la logica de TVE
    /// </summary>
    /// <returns></returns>
    private void eFTerminarTVE()
    {
        FinTVE();
    }

    /// <summary>
    /// Se encarga de recibir una alerta del sistema TELEMETRIA
    /// para ser mostrada en el Front
    /// </summary>
    /// <param name="alerta"></param>
    private void AlertaTELEMETRIA(string alerta)
    {
        AlertaConductor(alerta);
    }


    /// <summary>
    /// Se encarga de actualizar el estatus del indicador de Telemetria
    /// Powered ByRED 15JUN2020
    /// </summary>
    /// <param name="activo"></param>
    private void ActualizaIndicadorTelemetria(int Estado)
    {
        //Desencadena el siguiente evento hacia el front
        RefreshLedTelemetria(Estado);
    }


    /// <summary>
    /// Manda a pedir al sistema Telemetria un reporte para mostrar
    /// Powered ByRED 15JUN2020
    /// </summary>
    /// <returns></returns>
    private List<string> eFReporteTelemetria()
    {
        return ReporteTelemetria();
    }

    /// <summary>
    /// Se encarga de pedirle al Sistema VMD la pauta alojada en el 
    /// disco de peliculas
    /// </summary>
    /// <returns></returns>
    private List<string> eFObtenerListaPautas(string tipo)
    {
        var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();
        var listanueva = new List<string>();

        foreach (VMD v in _VMD)
        {
            var MyVMD = (VMD)v;
            listanueva =  MyVMD.ObtenerPautas(tipo);
        }

        return listanueva;
    }

    /// <summary>
    /// Se encarga de recibir el tipo y nombre de pauta
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombrePauta"></param>
    /// <returns></returns>
    private Task<bool> eFPlancharPautaVMD(string _tipo, string _nombrePauta)
    {
        //var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();
        //var flag = false;

        //foreach (VMD v in _VMD)
        //{
        //    var MyVMD = (VMD)v;
        //    flag = MyVMD.CargarPauta(_tipo, _nombrePauta);
        //}

        //return flag;

        return PautaVMD(_tipo, _nombrePauta);
    }

    /// <summary>
    /// Se encarga de validar la pauta que se quiere ingresar
    /// Powered ByRED 16/JUL/2020
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombrePauta"></param>
    /// <returns></returns>
    private Task<bool> eFValidaPautaVMD(string _tipo, string _nombrePauta)
    {
        return ValidaPautaVMD(_tipo, _nombrePauta);
    }

    /// <summary>
    /// Se encarga de recuperar las pautas de ciertas unidades en el USB
    /// </summary>
    /// <param name="_letraUnidad"></param>
    /// <returns></returns>
    private List<string>eFRecuperarPautaUSB(string _letraUnidad)
    {
        var listaRetorno = new List<string>();
        var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach(VMD v in _VMD)
        {
            var MyVMD = (VMD)v;
            listaRetorno = MyVMD.RecuperarScripts(_letraUnidad);
        }

        return listaRetorno;
    }
    /// <summary>
    /// Se encarga de pedirle al Sistema VMD la pauta alojada en el 
    /// disco de peliculas
    /// </summary>
    /// <returns></returns>
    private List<string> eFObtenerListaSpots(int tipo)
    {
        var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();
        var listanueva = new List<string>();

        foreach (VMD v in _VMD)
        {
            var MyVMD = (VMD)v;
            listanueva = MyVMD.ObtenerSpots();
        }

        return listanueva;
    }
    /// <summary>
    /// Se encarga de replicar el progreso de copiado 
    /// de las peliculas
    /// </summary>
    /// <returns></returns>
    private int eFPedirProgresoCopiado()
    {
        var _VMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();
        var i = 0;
        foreach (VMD v in _VMD)
        {
            var MyVMD = (VMD)v;
            i = MyVMD.ProgesoCopiado();
        }
        return i;
    }


    /// <summary>
    /// Se encarga de reportar la existencia de metas CAN 
    /// </summary>
    /// <returns></returns>
    private bool eFValidarMetasCAN()
    {
        return _config.ValidarMetasCAN();
    }

    /// <summary>
    /// Se encarga de verificar si existen Metas por región de CAN
    /// </summary>
    /// <returns></returns>
    private List<string> eFObtenerMetasCAN()
    {
        return _config.RecuperarListaMetas();
    }
    

    #region SIA

    /// <summary>
    /// Se encarga de recopilar la información para ser enviada a través del socket 
    /// Powered ByRED 19FEB2021
    /// </summary>
    private void ParametrosaSIA()
    {
        try
        {
            //Buffer para los 
            List<string> Mensajes = new List<string>();

            if ((bool)ParametrosInicio.CAN)
            {
                //FR_META
                Mensajes.Add("{CAN:System.String:CAN~sistema:System.String:CAN~parametro:System.String:fr_meta~valor:System.String:" + FE.FR_Meta.ToString() +
                    "~prioridad:System.String:2~TipoDato:System.String:Integer}");
                //FR_ACTUAL
                Mensajes.Add("{CAN:System.String:CAN~sistema:System.String:CAN~parametro:System.String:fr_actual~valor:System.String:" + FE.FR_Real.ToString() +
                    "~prioridad:System.String:2~TipoDato:System.String:Integer}");
                //KMS
                Mensajes.Add("{CAN:System.String:CAN~sistema:System.String:CAN~parametro:System.String:km~valor:System.String:" + FE.Kms.ToString() +
                    "~prioridad:System.String:2~TipoDato:System.String:Integer}");
                //Litros
                Mensajes.Add("{CAN:System.String:CAN~sistema:System.String:CAN~parametro:System.String:litros~valor:System.String:" + FE.Lts.ToString() +
                    "~prioridad:System.String:2~TipoDato:System.String:Integer}");
            }

            if ((bool)ParametrosInicio.VMD)
            {
                var estatusReproduccion = "Detenida";
                if (FE.EnReproduccion) { estatusReproduccion = "Avanzando"; }

                //Estado de la reproducción
                Mensajes.Add("{ VMD: System.String:VMD~sistema:System.String:VMD~parametro:System.String:estatus_reproduccion~valor:System.String:" + estatusReproduccion +
                    "~prioridad:System.String:2~TipoDato:System.String:String}");

                if (FE.EnReproduccion)
                {
                    //Titulo
                    Mensajes.Add("{ VMD: System.String:VMD~sistema:System.String:VMD~parametro:System.String:titulo_pelicula~valor:System.String:" + FE.NombrePelicula +
                        "~prioridad:System.String:2~TipoDato:System.String:String}");
                    //Duración
                    Mensajes.Add("{ VMD: System.String:VMD~sistema:System.String:VMD~parametro:System.String:duracion~valor:System.String:" + FE.DuracionPelicula +
                        "~prioridad:System.String:2~TipoDato:System.String:String}");
                    //Tiempo_Reproducción
                    Mensajes.Add("{ VMD: System.String:VMD~sistema:System.String:VMD~parametro:System.String:tiempo_reproduccion~valor:System.String:" + FE.TiempoPelicula +
                        "~prioridad:System.String:2~TipoDato:System.String:String}");
                }
            }

            
            if (Mensajes.Count > 0) //si tenemos mensaje por reportar...
            {
                //Enviamos los parametros que recuperamos
                var _SIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

                foreach (SIA s in _SIA)
                {
                    var MySIA = (SIA)s;

                    foreach (string mensaje in Mensajes)
                    {
                        MySIA.EscribirSocket(mensaje);
                    }

                }
            }
        }
        catch
        {

        }
    }


    /// <summary>
    /// Se encarga de de mandar el mensaje de sia hacia Front
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_tipo"></param>
    /// <param name="_texto"></param>
    private bool MensajeSIAaFront(int _tipo, string _texto)
    {
        return MensajeSIA(_tipo, _texto);
        //Func_IniciarMensajesSIA();
    }

    /// <summary>
    /// resibe de SIA el estado del internet
    /// </summary>
    /// <param name="estado"></param>
    private void EstadoInternetFront(int estado)
    {
        InternetSIA(estado);
    }

    /// <summary>
    /// Regresa a SIA los datos de GPS
    /// </summary>
    /// <returns></returns>
    private GPSData MandaGPSSIA()
    {
        return this.Datos_GPS;
    }

    /// <summary>
    /// Recibe de SIA un POI para ser mostrado en Front
    /// </summary>
    /// <param name="_multimedia"></param>
    /// <returns></returns>
    private bool POISIAaFRONT(List<string> _multimedia)
    {
        return POISIA(_multimedia);
    }


    /// <summary>
    /// Obtiene los mensajes segun el tipo de SIA para ser mostrados
    /// 0: mensajes predefinidos
    /// 1: mensajes recibidos
    /// 2: mensajes enviados
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    private List<string> eFObtenerMensajes(int tipo)
    {
        var listaRetorno = new List<string>();
        var _SIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

        foreach (SIA s in _SIA)
        {
            var MySIA = (SIA)s;

            listaRetorno = MySIA.ObtenerMensajesSIA(tipo);
        }

        return listaRetorno;
    }

    /// <summary>
    /// Se encarga de enviar el mensaje de texto a SIA
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="_msj"></param>
    /// <returns></returns>
    private bool eFMandarMensajeSIA(string _msj)
    {
        var retorno = false;

        var _SIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

        foreach (SIA s in _SIA)
        {
            var MySIA = (SIA)s;

            retorno = MySIA.EnviarMensajeSIA(_msj);
        }

        return retorno;
    }

    /// <summary>
    /// Se encarga de enviar una alerta de Robo por medio de SIA
    /// </summary>
    private void eFMandarAlertaRobo()
    {
        var _SIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

        foreach (SIA s in _SIA)
        {
            var MySIA = (SIA)s;

            MySIA.AlertaRobo();

        }
    }
    #endregion

    #endregion

    #region "Timers"

    /// <summary>
    /// Timer que sive para actualizar la información de los sistemas
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tm_Tick(object sender, EventArgs e)
    {
        timerActualizaSys.Stop();
        ActualizarSistemas();
        VerificaModoNocturno();
        timerActualizaSys.Start();
    }
    #endregion

    #region "Funciones de invocación VMD"
    /// <summary>
    /// Se encarga de iniciar el reproductor de VMD (Si existe)
    /// y se manda a mostrar el cintillo con el mensaje inicial //Powered ByRED 14ABR2021
    /// </summary>
    private void eFIniciarReproduccionVMD()
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.inicializaVMD();
        }

        //Si tenemos SIA
        //Powered ByRED 14ABR2021

        var MySIA = ListaSistemas.Where(x => x.Sistema == Sistema.SIA).ToList();

        foreach (SIA _sia in MySIA)
        {
            _sia.MostrarCintilloInicial();
        }
    }

    private void eFBuscaPauta()
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.BuscaPauta();
        }
    }

    /// <summary>
    /// Se manda a reinicar la pauta, ya sea por reincio de usuario
    /// o termino por sincronización
    /// </summary>
    /// <param name="DetenerPelicula"></param>
    public void eFReiniciarPauta(bool DetenerPelicula)
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.ReiniciarSecuencia(DetenerPelicula);
        }
    }

    /// <summary>
    /// Se encarga de guardar en base de datos la actividad
    /// </summary>
    /// <param name="posicion"></param>
    /// <param name="longitud"></param>
    public void eFActualizaActividad(double posicion, double longitud)
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.ActualizarInfoSecuencia(posicion, longitud);
        }
    }

    public void eFActualizaUltimaVez(int idArchivo)
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.ActualizarUltimaVez(idArchivo);
        }
    }

    public void eFChecaSiguienteVideo()
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.ChecaSigtVideo();
        }
    }

    public void eFValidaVolumen()
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();

        foreach (VMD _vmd in MyVMD)
        {
            _vmd.ControlVolumen();
        }

    }

    public void eFAgregarLogVMD(string VideoURL, int idPelicula, int MinutosMax, bool Ejecuta)
    {
        var MyVMD = ListaSistemas.Where(x => x.Sistema == Sistema.VMD).ToList();
        foreach (VMD _vmd in MyVMD)
        {
            _vmd.AgregaLog(VideoURL, idPelicula, MinutosMax, Ejecuta, string.Empty);
        }
    }

    #endregion

    #region "Integración de cintillos SIA"
    public string ErrorSIA_ { get; set; }
    public bool CintillosSIAIniciado { get; set; } = false;  
    DataSet GetDataSet(string sql)
    {
        // creates resulting dataset
        var result = new DataSet();

        // creates a data access context (DbContext descendant)
        using (var context = new vmdEntities())
        {
            // creates a Command 

            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            try
            {
                // executes
                context.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                // loop through all resultsets (considering that it's possible to have more than one)
                do
                {
                    // loads the DataTable (schema will be fetch automatically)
                    var tb = new DataTable();
                    tb.Load(reader);
                    result.Tables.Add(tb);

                } while (!reader.IsClosed);
            }
            finally
            {
                // closes the connection
                context.Database.Connection.Close();
            }
        }

        // returns the DataSet
        return result;
    }

    public void Func_IniciarMensajesSIA()
    {
        if (!CintillosSIAIniciado)
        {
            CintillosSIAIniciado = true; 
            var mSIA = new MensajesSIA();
            mSIA.HabilitarCintillo = ParametrosInicio.HabilitarCintillo;
            mSIA.MensajeInicial = ParametrosInicio.MensajeInicial;
            mSIA.VueltasMensaje = ParametrosInicio.VueltasMensaje;
            mSIA.AltoCintillo = ParametrosInicio.AltoCintillo;
            mSIA.ColorFuente = ParametrosInicio.ColorFuente;
            mSIA.TamanioFuente = ParametrosInicio.TamanoFuente;
            mSIA.ColorFondo = ParametrosInicio.ColorFondo;
            mSIA.Velocidad = ParametrosInicio.Velocidad;
            mSIA.PosicionMarquee = ParametrosInicio.PosicionMarquee == "T" ? MensajesSIA.PosicionMensaje.T : MensajesSIA.PosicionMensaje.B;
            var TimerCintilloSegundos = ParametrosInicio.tmrSegVMDSIA;
            if ((bool)mSIA.HabilitarCintillo)
            {
                ECintilloInicial(mSIA.MensajeInicial, 
                                 mSIA.PosicionMarquee == MensajesSIA.PosicionMensaje.T ? "T" : "B", mSIA.ColorFondo, 
                                 (int)mSIA.Velocidad, 
                                 (int)mSIA.TamanioFuente,
                                 mSIA.ColorFuente, 
                                 (int)mSIA.VueltasMensaje);
                System.Threading.Thread.Sleep(5000);
                LstOfSMSTouch = Func_ObtenerMensajesSIA();

                timerCintillo.Interval = (int)TimerCintilloSegundos * 1000;
                timerCintillo.Enabled = true;
                timerCintillo.Tick += new EventHandler(tm_Tick_Cintillo);
            }                   
        }
    }


    //Lo lanza cuando recibe un nuevo cintillo
    private void tm_Tick_Cintillo(object sender, EventArgs e)
    {
        if (LstOfSMSTouch.Count > 0)
        {
            var CantidadEntrue = LstOfSMSTouch.Where(x => x.Mostrado == false).Count();
            if (CantidadEntrue == 0) foreach (var mensaje in LstOfSMSTouch) { mensaje.Mostrado = false;}
            var elementoMensaje = LstOfSMSTouch.Where(x => x.Mostrado == false).FirstOrDefault();
            //Mostrar cintillo
            ECintillo(elementoMensaje.TexoSMS);
            LstOfSMSTouch.Where(x => x.idSmsTouch == elementoMensaje.idSmsTouch).FirstOrDefault().Mostrado = true;

            
        }
    }

    public class MensajesSIA
    {
        public enum PosicionMensaje
        {
            B = 0,
            T = 1
        }
        public bool? HabilitarCintillo { get; set; }
        public string MensajeInicial { get; set; }
        public long? VueltasMensaje { get; set; }
        public long? AltoCintillo { get; set; }
        public string ColorFuente { get; set; }
        public long? TamanioFuente { get; set; }
        public string ColorFondo { get; set; }
        public long? Velocidad { get; set; }
        public PosicionMensaje PosicionMarquee { get; set; }
    }

    public List<SMSTouch> Func_ObtenerMensajesSIA()
    {
        var Query = "select idsmstouch, textoSMS from SMSTouch Where IdDestinatario = 1;";
        var DSInfo = GetDataSet(Query);
        if (DSInfo.Tables.Count > 0) return DSInfo.Tables[0].AsEnumerable().Select(x => new SMSTouch(Convert.ToInt32(x[0].ToString()),x[1].ToString().Trim())).ToList();
        this.ErrorSIA_ = "No se tienen registros en la tabla SMSTouch";
        return null;
    }


    public List<SMSTouch> LstOfSMSTouch { get; set; }

    

    public class SMSTouch
    {
        public int idSmsTouch { get; set; }
        public string TexoSMS {get; set;}
        public bool Mostrado {get; set;}
        public SMSTouch(int IdSmsTouch, string TextoSMS )
        {
            this.idSmsTouch = IdSmsTouch;
            this.TexoSMS = TextoSMS;
            this.Mostrado = false;
        }
    }
    #endregion
}