using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Threading;
using InterfazSistema.WSCAN;

public class CAN : ISistema, IBDContext, IGPS
{

    #region "Propiedades"

    #endregion

    #region "Propiedades Heredadas"

        public int PuertoSocket { get; set; }
        public int OrdenDescarga { get; set; }
        public int OrdenLoad { get; set; }
        public Sistema Sistema { get { return Sistema.CAN; } }
        public string GetVersionSistema { get; }
        public vmdEntities VMD_BD { get; }
        public bool ModoNocturno { get; set; }
    //  public clsMessage AdvMsg { get; set; }
        public bool ModoPrueba { get;  set; }
        public GPSData Datos_GPS { get; set; }


    #endregion

    #region "Variables"
    //Timers
    private System.Windows.Forms.Timer timerActualiza = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerProcesaCAN = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerGrabaCAN = new System.Windows.Forms.Timer();

    //Hilos
    private Thread hiloActualizar;

    //logicas
    public ProtocoloCAN _ProtocoloCAN;
    public Globales _Globales;
    public Bitacora _Bitacora;
    private Secuencia _Secuencia;
    private Conductor _Conductor;
    private AdminViaje _AdminViaje;
    public SyncCAN _syncCAN;
    
    private string Log = string.Empty;

    public bool ViajePrueba;
    public bool Protocolo;

    private int TiempoEsperaMensajes = 500;

    public string CodigoDescarga = string.Empty;

    //Parametros
    public can_parametrosinicio ParametrosInicio;

    #endregion

    #region "Variables de evento"
    public delegate void holaCAN(double FrMeta, double FRActual, double kms, double lts, bool FR, bool proto, double velCAN);
    public event holaCAN EventoCAN;

    //
    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSyncCAN;

    public delegate void FirmarCondusat(string _autobus, string _operador, string _tipo, string _fechaapertura, string _fechacierre, string _cambioManos);
    public event FirmarCondusat EventoFirmaCondusat;

    //Para notificar a SAM que va a recibir el status del viaje ****Pendiente
    public delegate void ViajeAbierto(string operador, string nom_operador);
    public event ViajeAbierto AvisarViaje;

    //Avisa los datos de viaje a SAM **Telemetria
    public delegate void ViajeSAM(int operador, string nom_operador, bool EnViaje, DateTime FechaViaje, long Origen, string DescPob, string CVEPob);
    public event ViajeSAM AvisarViajeSAM;


    #endregion

    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public CAN()
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = VMD_BD.can_parametrosinicio.FirstOrDefault();
        ViajePrueba = false;
        Protocolo = false;

        //Datos_GPS = new GPSData();

        //_ProtocoloCAN = new ProtocoloCAN();
        //_Globales = new Globales();
        //_Secuencia = new Secuencia(ref _Globales, ref _ProtocoloCAN, ParametrosInicio);
        //_AdminViaje = new AdminViaje(ref _Globales, ref _ProtocoloCAN);
        //_Bitacora = new Bitacora(ref _Globales, ref _ProtocoloCAN, this.ModoPrueba);
        //_Conductor = new Conductor(ref _Globales, ref _Bitacora, ref _ProtocoloCAN, ref _Secuencia);

        //flipflop = false;

        //hiloActualizar = new Thread(new ThreadStart(ActualizaCAN));
    }

    #endregion

    #region  "Metodos Publicos"

    /// <summary>
    /// Valida un comando dado, para saber que acción tomar
    /// Tarjeta de circulación vencida, se puso para evitar cortar
    /// el flujo en algún momento dado durante la migracion
    /// </summary>
    /// <param name="cmd"></param>
    public void ValidaComando(string cmd)
    {
        try
        {
            if (cmd.Length <= 0)
            {
                return;
            }

            bool SinConfirmar = false;

            if (cmd.Equals(ParametrosInicio.InicioViaje))
            {
                if (_Globales.EsperaCambioManos)
                {
                    //PantallaCAN RELEVO
                    //MuestraModo();
                    return;
                }

                if (_Globales.EsperaFinViaje)
                {
                    //PantallaCAN FIN DE VIAJE
                    //MuestraModo();
                    return;
                }

                if (_Globales.EsperaIniciaViaje)
                {
                    //PantallaCAN INICIO DE VIAJE
                    //MuestraModo();
                    return;
                }

                if (_Globales.ViajeAbierto)
                {
                    //PantallaCAN VIAJE ACTUAL ABIERTO
                    //MuestraModo();
                }
                else if (_Globales.Corrida.ViajeActual <= 0)
                {
                    //Si no se valida la clave de conductor ni secuencia, entonces dejo pasar cualquier clave

                    _Globales.EsperaIniciaViaje = true;
                    _Globales.EsperaFinViaje = false;
                    _Globales.EsperaCambioManos = false;
                    _Globales.Corrida.ViajeActual = 1;
                    //PANTALLACAN INICIO DE VIAJE
                    //MUESTRAMODO();


                }
                else
                {
                    foreach (var i in _Globales.Viajes)
                    {
                        if (!i.Confirmado)
                        {
                            _Globales.Corrida.ViajeActual = i.IDDetSecuencia;
                            SinConfirmar = true;
                            break;
                        }
                    }

                    //Se permite por el momento abrir viajes sin necesidad de secuencias
                    if (SinConfirmar || true)
                    {
                        _Globales.EsperaIniciaViaje = true;
                        _Globales.EsperaFinViaje = false;
                        _Globales.EsperaCambioManos = false;
                        //PantallaCAN INICIO DE VIAJE
                        //MuestraMODO()
                    }
                    else
                    {
                        //PANTALLACAN TODOS LOS VIAJES ESTAN CERRRADOS
                        //MuestraMODO();
                    }



                }
                return;
            }
            else if (cmd.Equals(ParametrosInicio.FinViaje))
            {

                if (_Globales.EsperaCambioManos)
                {
                    //PantallaCAN RELEVO
                    //MuestraModo();
                    return;
                }
                else if (_Globales.EsperaFinViaje)
                {
                    //PantallaCAN FIN DE VIAJE
                    //MuestraMODO();
                    return;
                }
                else if (_Globales.EsperaIniciaViaje)
                {
                    //PantallaCAN INICIO DE VIAJE
                    //MuestraMODO();
                    return;
                }

                if (_Globales.ViajeAbierto)
                {
                    _Globales.EsperaFinViaje = true;
                    _Globales.EsperaCambioManos = false;
                    _Globales.EsperaIniciaViaje = false;
                    //PantallaCan FIN DE VIAJE
                    //MuestraModo();
                }
                else
                {
                    //Si no se valida la Clave de conductor ni secuencia, entonces dejo pasar cualquier clave
                    if (!(bool)ParametrosInicio.ValidarCveCondSec)
                    {
                        if (_Globales.Corrida.ViajeActual > 0)
                        {
                            _Globales.EsperaFinViaje = true;
                            _Globales.EsperaCambioManos = false;
                            _Globales.EsperaIniciaViaje = false;
                        }
                    }

                    //PantallaCAN = No existe viaje abierto
                    //MuestraModo();
                }
                return;
            }
            else if (cmd.Equals(ParametrosInicio.CambioManos))
            {
                if (_Globales.EsperaCambioManos)
                {
                    //PantallaCAN RELEVO
                    //MuestraModo();
                    return;
                }
                else if (_Globales.EsperaFinViaje)
                {
                    //PantallaCAN Fin De VIAJE
                    //MuestraModo();
                    return;
                }
                else if (_Globales.EsperaIniciaViaje)
                {
                    //PantallaCAN = INICIO DE VIAJE
                    //MuestraModo();
                    return;
                }

                if (_Globales.ViajeAbierto)
                {
                    _Globales.EsperaCambioManos = true;
                    _Globales.EsperaFinViaje = false;
                    _Globales.EsperaIniciaViaje = false;
                    //PantallaCAN RELEVO
                    //MuestraModo();
                }
                else
                {
                    //PantallaCAN No Existe viaje abierto
                    //MuestraModo();
                }

                return;
            }
            else if (cmd.Equals(ParametrosInicio.Sincronizar))
            {
                if (_Globales.EsperaCambioManos)
                {
                    //PantallaCAN RELEVO
                    //MuestraModo();
                    return;
                }
                else if (_Globales.EsperaFinViaje)
                {
                    //PantallaCAN FIN DE VIAJE
                    //MuestraModo();
                    return;
                }
                else if (_Globales.EsperaIniciaViaje)
                {
                    //PantallaCAN INICIO DE VIAJE
                    //MuestraModo();
                    return;
                }

                //SincronizaciónCAN2(); ************************************

                //Deshabilito la tarjert
                if (!ParametrosInicio.IDWIFI.Equals(""))
                {
                    //Comando para desactivar el dispositivo de WIFI
                }

            }
            else if (cmd.Equals(ParametrosInicio.DatosCAN))
            {
                _Globales.MostrandoDatosCAN = true;
                //MuestraDatosCAN(); *************************************
                //Activamos el timer para que los datos se estén refrescando
                //tmrDatosCAN.Enables;
                return;
            }
            else if (cmd.Equals(_Globales.Param_CAN_DatosViaje))
            {
                //MuestraDatosViaje();********************
                return;
            }
            else if (cmd.Equals(_Globales.Param_CAN_DatosGPS))
            {
                _Globales.MostrandoDatosGPS = true;
                //MuestraDatosGPS() ***********
                return;
            }
            else if (cmd.Equals(ParametrosInicio.ViajePrueba))
            {
                if (_Globales.EnViajePrueba)
                {
                    _Globales.EnViajePrueba = false;
                    //PantallaCAN VIAJE PRUEBA SE DESACTIVO
                    //MuestraModo();
                }
                else
                {
                    _Globales.EnViajePrueba = true;
                    //PantallaCAN VIAJE PRUEBA SE ACTIVO 
                    //MuestraModo();
                }
                return;
            }
            else
            {
                if (_Globales.EsperaCambioManos || _Globales.EsperaIniciaViaje)
                {
                    //Checo si es un conductor
                    //ChecaConductor(cmd);**************************

                }
                else if (_Globales.EsperaFinViaje)
                {
                    if (_Globales.Corrida.ConductorActualID != Int32.Parse(cmd))
                    {
                        //PantallaCAN FIN DE VIAJE
                        //MuestraModo();
                    }
                    else
                    {
                        //Checo si es conductor
                        //ChecaConductor(cmd);********************************************
                    }

                }
                else
                {
                    //Pantalla CAN Comando no valido
                    //MuestraModo();
                }
            }



        }
        catch (Exception ex)
        {
            //MuestraError("ValidaComandoCAN")*****************************************
        }
    }

    /// <summary>
    /// Valida si la clave del conductor es válida
    /// </summary>
    /// <param name="clvConductor"></param>
    /// <returns></returns>
    public string ValidarOperador(string clvConductor)
    {
        return _Conductor.ValidarConductor(clvConductor);
    }

    /// <summary>
    /// Se encarga de ejecutar la acción de acuerdo al tipo de viaje
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="clvConductor"></param>
    /// <param name="PobActual"></param>
    /// <returns></returns>
    public bool Viaje(string tipo, string clvConductor, can_poblaciones PobActual)
    {
        timerGrabaCAN.Stop();

        bool resultado = false;
        switch (tipo)
        {
            case "VA":
                resultado = _AdminViaje.AbrirViaje(clvConductor, PobActual);
                break;


            case "CM":
                resultado = _AdminViaje.CambioDeManos(clvConductor, PobActual);
                break;


            case "VC":
                resultado = _AdminViaje.CerrarViaje(clvConductor, PobActual);
                break;

            default:
                resultado = false;
                break;
        }

        timerGrabaCAN.Start();
        return resultado;
    }

    /// <summary>
    /// Se encarga de configurar el servidor de can de acuerdo
    /// al tipo de tarjeta que se requiere
    /// </summary>
    /// <param name="tipo"></param>
    public void ConfigurarCAN(string tipo)
    {
        _ProtocoloCAN = new ProtocoloCAN();

        _ProtocoloCAN.Iniciar_ADOCAN();

        _ProtocoloCAN.Configura_ADOCAN(tipo);

        _ProtocoloCAN.Detener_ADOCAN();

        _ProtocoloCAN = null;
    }
    #endregion

    #region "Métodos Privados"

    /// <summary>
    /// Se encarga de cargar las diferentes lógicas que se necesitan
    /// </summary>
    private void CargarLogicas()
    {
        Datos_GPS = new GPSData();

        _ProtocoloCAN = new ProtocoloCAN();

        _Globales = new Globales();

        _Secuencia = new Secuencia(ref _Globales, ref _ProtocoloCAN, ParametrosInicio);
        _Secuencia.ValidaConductor += this.ValidarConductor;


        _AdminViaje = new AdminViaje(ref _Globales, ref _ProtocoloCAN);

        _Bitacora = new Bitacora(ref _Globales, ref _ProtocoloCAN, this.ModoPrueba);

        _Conductor = new Conductor(ref _Globales, ref _Bitacora, ref _ProtocoloCAN, ref _Secuencia);
        _Conductor.AvisarViajeaFront += this.DatosViaje;
    }

    /// <summary>
    /// Carga el factor de rendimiento de CAN
    /// </summary>
    private void CargaMetaCAN()
    {
        _Globales.Param_CAN_MetaRendimiento = (double)ParametrosInicio.MetaRendimiento;

        var mes = GetMes(DateTime.Today.Month);

        var query = (from x in VMD_BD.can_catmetasregionros
                     where x.Year == DateTime.Today.Year && x.Mes == mes && x.Tipo == ParametrosInicio.NombreTipoMetaCAN && x.Region == ParametrosInicio.NombreRegionOperCAN
                     select x.Odometro).FirstOrDefault();

        if (query > 0)
        {
            _Globales.Param_CAN_MetaRendimiento = Convert.ToDouble(query);
        }

    }

    /// <summary>
    /// Configura los timers que se van a ocupar
    /// </summary>
    private void PreparaTimers()
    {
        //Para el procesmaiento de los datos provenientes del protoclo de CAN
        timerProcesaCAN.Interval = 500;
        timerProcesaCAN.Enabled = true;
        timerProcesaCAN.Tick += new EventHandler(timerProcesaCAN_Tick);

        //Para la grabación de los registros de movtoscan
        timerGrabaCAN.Interval = 1000;
        timerGrabaCAN.Enabled = true;
        timerGrabaCAN.Tick += new EventHandler(timerGrabaCAN_Tick);

        //Hilos
        hiloActualizar = new Thread(new ThreadStart(ActualizaCAN));
    }

    /// <summary>
    /// Se encarga de asignar los eventos
    /// </summary>
    private void AsignaEventos()
    {
        _AdminViaje.RegistroCANTemp += this.GrabaCANTemp;
        _AdminViaje.RegistroCAN += this.GrabaCAN;
        _AdminViaje.RegistroCondusat += this.FirmaCondusat;
        _AdminViaje.ViajeSAM += this.MandarViajeSAM;
    }

    /// <summary>
    /// Método que se ejecuta con el hilo de actualizar
    /// </summary>
    private void ActualizaCAN()
    {
        
        bool Fr = false;

        try
        {
            //Se omiten FLags

            if(_ProtocoloCAN.NombreProtocolo != null)
            {

                if (_ProtocoloCAN.NombreProtocolo.Substring(0, 2).Equals("DG"))
                {

                    if (_ProtocoloCAN.FuelEconomyEstimationFE < _ProtocoloCAN.LtsIniDG) return;

                    if (_ProtocoloCAN.TripDistanceEstimationVS < _ProtocoloCAN.KmsIniDg) return;

                    //Cargamos las variables de DG si es que éstas, no han sido actualizadas

                    if( _ProtocoloCAN.KmsIniDg == 0) _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;

                    if (_ProtocoloCAN.LtsIniDG == 0) _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;

                    double FactorRendimiento; 

                    if (ViajePrueba)
                    {
                        if((_ProtocoloCAN.FuelEconomyEstimationFE - _ProtocoloCAN.LtsIniDG) > 0)
                        {
                            FactorRendimiento = FactorRendimiento = ValorDG2(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg, _ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG);

                            if (FactorRendimiento >= _Globales.Param_CAN_MetaRendimientoPrueba)
                            {
                                Fr = true;
                            }


                            EventoCAN(_Globales.Param_CAN_MetaRendimientoPrueba,
                                FactorRendimiento,
                                ValorDG(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg),
                                ValorDG(_ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG),
                                Fr,
                                Protocolo,
                                _ProtocoloCAN.TachoVehicleSpeed);
                            

                        }
                        else
                        {
                            EventoCAN(_Globales.Param_CAN_MetaRendimientoPrueba,
                                0,
                                ValorDG(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg),
                                ValorDG(_ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG),
                                Fr,
                                Protocolo,
                                _ProtocoloCAN.TachoVehicleSpeed);
                        }


                    }
                    else //No es Viaje de Prueba
                    {
                        if((_ProtocoloCAN.FuelEconomyEstimationFE - _ProtocoloCAN.LtsIniDG) > 0)
                        {
                            FactorRendimiento = ValorDG2(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg, _ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG);


                            if (FactorRendimiento >= _Globales.Param_CAN_MetaRendimiento)
                            {
                                Fr = true;
                            }


                            EventoCAN(_Globales.Param_CAN_MetaRendimiento,
                                FactorRendimiento,
                                ValorDG(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg),
                                ValorDG(_ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG),
                                Fr,
                                Protocolo,
                                _ProtocoloCAN.TachoVehicleSpeed);
                        }
                        
                        else
                        {
                            EventoCAN(_Globales.Param_CAN_MetaRendimiento,
                                0,
                                ValorDG(_ProtocoloCAN.TripDistanceEstimationVS, _ProtocoloCAN.KmsIniDg),
                                ValorDG(_ProtocoloCAN.FuelEconomyEstimationFE, _ProtocoloCAN.LtsIniDG),
                                Fr,
                                Protocolo,
                                _ProtocoloCAN.TachoVehicleSpeed);
                        }
                    }
                    
                }
                else //Es St
                {

                    if (Truncar(_ProtocoloCAN.RealKmPerLiterHighLevel, 2) >= _Globales.Param_CAN_MetaRendimiento)
                    {
                        Fr = true;
                    }

                    if (ViajePrueba)
                    {
                        EventoCAN(_Globales.Param_CAN_MetaRendimientoPrueba,
                            Truncar(_ProtocoloCAN.RealKmPerLiterHighLevel,2),
                            Truncar(_ProtocoloCAN.TripDistanceEstimationVS,2),
                            Truncar(_ProtocoloCAN.FuelEconomyEstimationFE,2),
                            Fr,
                            Protocolo,
                            _ProtocoloCAN.TachoVehicleSpeed);
                    }
                    else
                    {
                        EventoCAN(_Globales.Param_CAN_MetaRendimiento,
                            Truncar(_ProtocoloCAN.RealKmPerLiterHighLevel,2),
                            Truncar(_ProtocoloCAN.TripDistanceEstimationVS,2),
                            Truncar(_ProtocoloCAN.FuelEconomyEstimationFE,2),
                            Fr,
                            Protocolo,
                            _ProtocoloCAN.TachoVehicleSpeed);
                    }
                }
            }
            

        }
        catch(Exception ex)
        {

        }
    }

    /// <summary>
    /// Se encarga de grabar el registro temporal de CAN
    /// </summary>
    /// <param name="IdSecuencia"></param>
    /// <param name="IdDetSecuencia"></param>
    /// <param name="IdConductor"></param>
    private void GrabaCANTemp(int IdSecuencia, int IdDetSecuencia, int IdConductor)
    {
        _Bitacora.GrabaSecuenciaTemp(IdSecuencia, IdDetSecuencia, IdConductor);
    }

    /// <summary>
    /// Se encarga de grabar el registro CAN
    /// </summary>
    /// <param name="accion"></param>
    /// <param name="CANPob"></param>
    private void GrabaCAN(string accion, can_poblaciones CANPob)
    {
        _Bitacora.GrabaRegistroCAN(accion, CANPob);
    }

    /// <summary>
    /// Se encarga de enviar la firma a CONDUSAT
    /// </summary>
    /// <param name="_autobus"></param>
    /// <param name="_operador"></param>
    /// <param name="_tipo"></param>
    /// <param name="_fechaapertura"></param>
    /// <param name="_fechacierre"></param>
    /// <param name="_cambioManos"></param>
    private void FirmaCondusat(string _autobus, string _operador, string _tipo, string _fechaapertura, string _fechacierre, string _cambioManos)
    {
        EventoFirmaCondusat(_autobus, _operador, _tipo, _fechaapertura, _fechacierre, _cambioManos);
    }


    /// <summary>
    /// Se encarga de recopilar los datos de viaje y enviarlos a SAM
    /// </summary>
    private void MandarViajeSAM(can_poblaciones _CANPob = null)
    {
        try
        {
            //Verificamos si tenemos viaje
            if (_Globales.Corrida.ViajeActual > 0)
            {
                if( _CANPob != null)
                {
                    AvisarViajeSAM(_Globales.conductor,
                    _Conductor.ValidarConductor(_Globales.conductor.ToString()),
                    _Globales.ViajeAbierto,
                    _Globales.Viajes[_Globales.Corrida.ViajeActual - 1].FechaHora,
                    _CANPob.idpob, _CANPob.despob, _CANPob.CVEPOB);
                }
                else
                {
                    AvisarViajeSAM(_Globales.conductor,
                    _Conductor.ValidarConductor(_Globales.conductor.ToString()),
                    _Globales.ViajeAbierto,
                    _Globales.Viajes[_Globales.Corrida.ViajeActual - 1].FechaHora,
                    0, "", "");
                }
            }
            else
            {//en caso de ser viaje cerrado, entonces mandamos la fecha del equipo

                if (_CANPob != null)
                {
                    AvisarViajeSAM(_Globales.conductor,
                    _Conductor.ValidarConductor(_Globales.conductor.ToString()),
                    _Globales.ViajeAbierto,
                    DateTime.Now, _CANPob.idpob, _CANPob.despob, _CANPob.CVEPOB);
                }
                else
                {
                    AvisarViajeSAM(_Globales.conductor,
                    _Conductor.ValidarConductor(_Globales.conductor.ToString()),
                    _Globales.ViajeAbierto,
                    DateTime.Now, 0, "", "");
                }
                
            }
        }
        catch(Exception ex)
        {

        }
    }

    #endregion

    #region "Métodos heredados"

    /// <summary>
    /// Se encarga de actualizar las vistas
    /// </summary>
    public void Actualizar()
    {
        if (!hiloActualizar.IsAlive)
        {
            hiloActualizar = new Thread(new ThreadStart(ActualizaCAN));
            hiloActualizar.Start();
        }
    }

    /// <summary>
    /// Recibe algún tipo de evento
    /// </summary>
    /// <returns></returns>
    public string Eventos()
    {
        string tempLog = Log;

        Log = string.Empty;

        return tempLog;
    }

    /// <summary>
    /// Se encarga de finalizar los componentes para un correcto apagado
    /// </summary>
    public void Finalizar()
    {
        _ProtocoloCAN.Detener_ADOCAN();

        timerGrabaCAN.Stop();
        timerGrabaCAN.Dispose();

        timerProcesaCAN.Stop();
        timerProcesaCAN.Dispose();
    }

    /// <summary>
    /// Se encarga de inicializar los componentes necesarios
    /// </summary>
    public void Inicializar()
    {
        CargarLogicas();

        _ProtocoloCAN.Iniciar_ADOCAN();

        _Secuencia.AsignaSecuenciaCAN();

        CargaMetaCAN();

        PreparaTimers();

        AsignaEventos();

        //iniciamos Timer de procesamiento
        timerProcesaCAN.Start();

        //Iniciar Timer de Grabación CAN
        timerGrabaCAN.Start();

    }

    /// <summary>
    /// Manda a llamar los métodos necesarios para su sincronización
    /// </summary>
    public bool Sincronizar()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Interface para la sincronización de CAN
    /// Método Tradicional (Esquema VMD 7)
    /// </summary>
    /// <param name="versionServer"></param>
    /// <param name="versionSistema"></param>
    /// <param name="IPActual"></param>
    /// <returns></returns>
    public bool Sincronizar(int versionServer, string versionSistema, string IPActual, SqlConnection _SqlConServer, bool ServerAlterno, double AnilloRed, ref List<string> Log)
    {
        _syncCAN = new SyncCAN();
        bool exito = false;

        _syncCAN.EventoSyncCANProgreso += this.EventoSyncCANProgreso;

        try
        {
            //Log
            if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Actualizando BD Local, con status S...", ref Log);

            var NumRegs = _syncCAN.MovtosCANMovil();


            //Ejecuto la Lógica de MovtosCan

            if (NumRegs > 0) // Verifica que existan más de un registro para poder 
            {
                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Registros por procesar: " + NumRegs, ref Log);
                EventoSyncCANProgreso("Movtos CAN: " + NumRegs, 0);
                Thread.Sleep(1500);
                if (!_syncCAN.MovTosCAN(_SqlConServer, IPActual, versionSistema, ref _Globales))
                {
                    EventoSyncCANProgreso("Error en Lógica de movtoscanSync ", 0);
                    Thread.Sleep(1500);
                    //
                    Thread.Sleep(2000);

                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Error en lógica de movtoscan" + NumRegs, ref Log);

                    exito = false;
                }
                else
                {
                    exito = true;
                }

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Registros locales actualizados: " + NumRegs, ref Log);

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Eliminando los registros de movtoscanx del servidor...", ref Log);
            }
            else
            {//No hay registros suficientes para sincronizar
                EventoSyncCANProgreso("No hay MovtosCan para sincronizar ", 0);
                Thread.Sleep(1500);
                _syncCAN.CodigoDescarga = _syncCAN.FalloCodigoDescarga;

                exito = true;
            }

            //Si no es servidor Alterno, sólo subo los datos del móvil al server, si no es alterno, hago toda la sincronización
            //Si no sincroniza en alterno, hago esto, si sí sincroniza en alterno, hago como si fuera local o central    <----- No es mi redacción, y tampoco le entiendo sólo copié ByRed
            if (ServerAlterno && !(bool)ParametrosInicio.SincronizaEnAlterno)
            {
                //Log "Finalizó sincronización en Servidor Alterno, Código
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Finalizó sincronización en Servidor Alterno,  CodigoDescarga: " + _syncCAN.CodigoDescarga, ref Log);

                //Si finalizó sincronización correctamente (es decir, que tenga código de descarga), entonces grabo como última sincronización

                if (_syncCAN.CodigoDescarga != _syncCAN.FalloCodigoDescarga)
                {
                    //UltimaDescarga *Se omite ésta variable

                    EventoSyncCANProgreso("Finalizó Sincronización" + Environment.NewLine + "en Servidor Alterno" + Environment.NewLine + "Codigo de Descarga" + Environment.NewLine + _syncCAN.CodigoDescarga, 0);
                    this.CodigoDescarga = _syncCAN.CodigoDescarga;
                    Thread.Sleep(TiempoEsperaMensajes);

                }
            }
            else
            {// si sí se debe de circular en Servidor Alterno:

                //LOG
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Sincronizando Tablas de Servidor...", ref Log);

                //Mostramos el proceso en la ventana de sincronización
                EventoSyncCANProgreso("Sincronizando Datos" + Environment.NewLine + "de Servidor...", 0);

                //Sleep 250

                //ejecuto el método encargado de la lógica de la actualización de catálogos
                if (!_syncCAN.DescargarTablas(_SqlConServer))
                {
                    exito = false;
                }

                //Continua flujo de CargaSecuencia

                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Cambio Meta, por si cambió los datos de la tabla...", ref Log);

                //Cargo de nuevo el Rendimiento Meta, por si se actualizó la tabla de can_catmetasregionros
                CargaMetaCAN();

                EventoSyncCANProgreso("Buscando Secuencia" + Environment.NewLine + "Espere por favor...", 0);

                //Sleep(250)

                if (_Secuencia.CargaSecuencia())
                {
                    EventoSyncCANProgreso("Finalizó Sincronz.  Secuencia Cargada..." + Environment.NewLine + "Código de Descarga:" + Environment.NewLine + _syncCAN.CodigoDescarga, 0);

                    _Secuencia.AsignaSecuenciaCAN();
                }
                else
                {
                    EventoSyncCANProgreso("Finalizó Sincroniz.  Secuencia NO Cargada..." + Environment.NewLine + "Código de Descarga:" + Environment.NewLine + _syncCAN.CodigoDescarga, 0);
                }

                this.CodigoDescarga = _syncCAN.CodigoDescarga;
            }

            if ((bool)ParametrosInicio.LogSincronizacionBien || (bool)ParametrosInicio.LogSincronizacionMal) _syncCAN.AgregarLogSync("Finalizó sincronización, Código Descarga: " + _syncCAN.CodigoDescarga + ", Autobús: " + ParametrosInicio.Autobus, ref Log);


            //Sleep(3000)

            //Mensaje final de los logs
            //EventoSyncCANProgreso("CAN - ", 1);
        }
        catch (Exception ex)
        {
            return false;
        }

        return exito;

    }

    /// <summary>
    /// interface para la sincronización de CAN
    /// Método NUBE (Esquema VMD8)
    /// </summary>
    /// <param name="WSCAN"></param>
    /// <param name="versionSistema"></param>
    /// <param name="IPActual"></param>
    /// <param name="Log"></param>
    /// <returns></returns>
    public bool Sincronizar(ServiceClient WSCAN, string versionSistema, string IPActual, ref List<string> Log)
    {
        _syncCAN = new SyncCAN();
        bool exito = false;

        _syncCAN.EventoSyncCANProgreso += this.EventoSyncCANProgreso;

        try
        {
            //Log
            if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Actualizando BD Local, con status S...", ref Log);

            var NumRegs = _syncCAN.MovtosCANMovil();

            //Ahora si viene lo shido - Luisito Comunica : ByRED 2019
            //Lógica de MovtosCAN

            if (NumRegs > 0) //Verifica que existan más de un registro para poder continuar
            {
                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Registros por procesar: " + NumRegs, ref Log);
                EventoSyncCANProgreso("Movtos CAN: " + NumRegs, 0);
                Thread.Sleep(1500);

                if (!_syncCAN.MovTosCANCloud(WSCAN, IPActual, versionSistema, ref _Globales))
                {
                    EventoSyncCANProgreso("Error en Lógica de movtoscanSync ", 0);
                    Thread.Sleep(1500);
                    //
                    Thread.Sleep(2000);

                    //Log
                    if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Error en lógica de movtoscan" + NumRegs, ref Log);

                    exito = false;
                }

                EventoSyncCANProgreso("Registros Sincronizados", 0);

                exito = true;

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Registros locales actualizados: " + NumRegs, ref Log);

                //Log
                if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Eliminando los registros de movtoscanx del servidor...", ref Log);
            }
            else
            {//No hay registros suficientes para Sincronizar
                //No hay registros suficientes para sincronizar
                EventoSyncCANProgreso("No hay MovtosCan para sincronizar ", 0);
                Thread.Sleep(1500);
                _syncCAN.CodigoDescarga = _syncCAN.FalloCodigoDescarga;

                exito = true;
            }


            //LOG
            if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Sincronizando Tablas de Servidor...", ref Log);

            //Mostramos el proceso en la ventana de sincronización
            EventoSyncCANProgreso("Sincronizando Datos" + Environment.NewLine + "de Servidor...", 0);

            //Sleep 250

            //ejecuto el método encargado de la lógica de la actualización de catálogos
            if (!_syncCAN.DescargarTablasCloud(WSCAN))
            {
                exito = false;
                EventoSyncCANProgreso("Error al sincornizar Catalogos", 0);
            }

            //Continua flujo de CargaSecuencia

            if ((bool)ParametrosInicio.LogSincronizacionBien) _syncCAN.AgregarLogSync("Cambio Meta, por si cambió los datos de la tabla...", ref Log);

            //Cargo de nuevo el Rendimiento Meta, por si se actualizó la tabla de can_catmetasregionros
            CargaMetaCAN();

            EventoSyncCANProgreso("Buscando Secuencia" + Environment.NewLine + "Espere por favor...", 0);

            //Sleep(250)

            if (_Secuencia.CargaSecuencia())
            {
                EventoSyncCANProgreso("Finalizó Sincronz.  Secuencia Cargada..." + Environment.NewLine + "Código de Descarga:" + Environment.NewLine + _syncCAN.CodigoDescarga, 0);

                this.CodigoDescarga = _syncCAN.CodigoDescarga;

                _Secuencia.AsignaSecuenciaCAN();
            }
            else
            {
                EventoSyncCANProgreso("Finalizó Sincroniz.  Secuencia NO Cargada..." + Environment.NewLine + "Código de Descarga:" + Environment.NewLine + _syncCAN.CodigoDescarga, 0);
                this.CodigoDescarga = _syncCAN.CodigoDescarga;
            }

            if ((bool)ParametrosInicio.LogSincronizacionBien || (bool)ParametrosInicio.LogSincronizacionMal) _syncCAN.AgregarLogSync("Finalizó sincronización, Código Descarga: " + _syncCAN.CodigoDescarga + ", Autobús: " + ParametrosInicio.Autobus, ref Log);

        }
        catch (Exception ex)
        {
            return false;
        }

        return exito;

    }
    #endregion

    #region "Eventos"
    /// <summary>
    /// Envía los datos por medio de un Evento a SAM
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="caso"></param>
    private void EventoSyncCANProgreso(string mensaje, int caso)
    { 
            EventoSyncCAN(mensaje, caso);  
    }

    /// <summary>
    /// Se encarga de checar el conductor
    /// Proviene de AsignaSecuencia()
    /// </summary>
    /// <param name="ClvConductor"></param>
    private void ValidarConductor(string ClvConductor)
    {
        _Conductor.ChecaConductor(ClvConductor);
    }

    /// <summary>
    /// Manda a avisar que hay viaje abierto
    /// prviene de AsignaSecuencia()
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="operador"></param>
    private void DatosViaje(string operador, string nom_operador)
    {
        AvisarViaje(operador, nom_operador);
    }
    #endregion

    #region "Timers"

    /// <summary>
    /// Se encarga de estar leyendo los datos del protocolo
    /// CAN y procesar la información recibida
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerProcesaCAN_Tick(object sender, EventArgs e)
    {
        timerProcesaCAN.Stop();
        //Obtengo los datos del protocolo CAN
        _ProtocoloCAN.ProcesaCAN((bool)ParametrosInicio.CAN, ref _Globales);

        bool res = _ProtocoloCAN.ValidaDatos();

        if (!res == Protocolo)
        {
            Protocolo = res;
        }

        //Protocolo = true; ///////////////

        timerProcesaCAN.Start();
    }

    /// <summary>
    /// Se encarga de grabar los registros de CAN a cada segundo
    /// si es que se cumplen las condiciones
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerGrabaCAN_Tick(object sender, EventArgs e)
    {
        timerGrabaCAN.Stop();

        _Bitacora.GrabaCAN();

        timerGrabaCAN.Start();
    }
    #endregion

    #region "utils"

    /// <summary>
    /// Se encarga de obtener el nombre del Mes
    /// respecto a la fecha ingrasada
    /// </summary>
    /// <param name="Fecha"></param>
    /// <returns></returns>
    private string GetMes(int Fecha)
    {
        string mes = string.Empty;

        switch (Fecha)
        {

            case 1:
                mes = "ENERO";
                break;

            case 2:
                mes = "FEBRERO";
                break;

            case 3:
                mes = "MARZO";
                break;

            case 4:
                mes = "ABRIL";
                break;

            case 5:
                mes = "MAYO";
                break;

            case 6:
                mes = "JUNIO";
                break;

            case 7:
                mes = "JULIO";
                break;

            case 8:
                mes = "AGOSTO";
                break;

            case 9:
                mes = "SEPTIEMBRE";
                break;

            case 10:
                mes = "OCTUBRE";
                break;

            case 11:
                mes = "NOVIEMBRE";
                break;

            case 12:
                mes = "DICIEMBRE";
                break;

        }

        return mes;
    }
    
    /// <summary>
    /// Se encarga de truncar los valores de CAN para
    /// ser mostrados en el Front
    /// </summary>
    /// <param name="Valor"></param>
    /// <returns></returns>
    private double Truncar(double Valor, double potencia)
    {
        //int temporal = (int)(Valor * 100);

        //return temporal / 100.0;

        int temporal = (int)(Valor * Math.Pow(10,potencia));
        return temporal / Math.Pow(10, potencia);
    }

    /// <summary>
    /// Sirve para obtener el valor de DG para Kms y lts
    /// </summary>
    /// <param name="Valor"></param>
    /// <returns></returns>
    private double ValorDG(double ValorA, double ValorB)
    {
        //int temporal = (int)((Truncar(ValorA, 2) - Truncar(ValorB, 2)) * 100);

        var temporal = ((Truncar(ValorA, 2) - Truncar(ValorB, 2)) * 100);

        var temp = temporal / 100;

        //return (temporal / 100);

        return temp;
    }

    /// <summary>
    /// Sirve para calcular el Factor de Rendimiento para DG
    /// </summary>
    /// <param name="ValorA"></param>
    /// <param name="ValorB"></param>
    /// <param name="ValorC"></param>
    /// <param name="ValorD"></param>
    /// <returns></returns>
    private double ValorDG2(double ValorA, double ValorB, double ValorC, double ValorD)
    {
        double Temp1 = Truncar(ValorA, 2) - Truncar(ValorB, 2);
        double temp2 = Truncar(ValorC, 2) - Truncar(ValorD, 2);

        //int temp3 = (int)((Temp1 / temp2) * 100);

        var temp3 = ((Temp1 / temp2) * 100);

        var temp4 = temp3 / 100;

        //return (temp3 / 100);

        return temp4;
    }

    #endregion
}
