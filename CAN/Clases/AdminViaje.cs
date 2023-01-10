using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;

public class AdminViaje : IBDContext
{
    #region "Variables"
    //Heredado de la interface ISistema
    public vmdEntities VMD_BD { get; }
    private can_parametrosinicio Parametros_Inicio; //sólo de lectura

    //Variable de logicas
    private Conductor ObjCondu;
    public Globales IGlobales;
    public ProtocoloCAN IProtocoloCAN;
    private Secuencia ISecuencia;

    //Flags
    private bool DatosCANIniciados = false;
    private bool CargandoViajeAnterior = false;

    //Eventos
    public delegate void GrabaCAN(string accion, can_poblaciones CANPob = null);
    public event GrabaCAN RegistroCAN;

    public delegate void FirmarCONDUSAT(string _autobus, string _operador, string _tipo, string _fechaapertura, string _fechacierre, string _cambioManos);
    public event FirmarCONDUSAT RegistroCondusat;

    public delegate void GrabaCANTemp(int IdSecuencia, int IdDetSecuencia, int IdConductor);
    public event GrabaCANTemp RegistroCANTemp;

    //Avisa a los datos de viaje
    public delegate void AvisaDeViajeASAM(can_poblaciones _canPOB = null);
    public event AvisaDeViajeASAM ViajeSAM;

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_Globales"></param>
    /// <param name="_ProtocoloCAN"></param>
    public AdminViaje(ref Globales _Globales, ref ProtocoloCAN _ProtocoloCAN)
    {
        VMD_BD = new vmdEntities();

        Parametros_Inicio = (from x in VMD_BD.can_parametrosinicio
                             select x).FirstOrDefault();

        IGlobales = _Globales;
        IProtocoloCAN = _ProtocoloCAN;
        ISecuencia = new Secuencia();
    }

    #region "Métodos Públicos"


    /// <summary>
    /// Abre un Nuevo Viaje
    /// </summary>
    /// <param name="ClvConductor"></param>
    /// <param name="CANPob"></param>
    /// <returns></returns>
    public bool AbrirViaje(string ClvConductor, can_poblaciones CANPob = null)
    {
        try
        {
            int ClvConductorint = Int32.Parse(ClvConductor);
            int IdSecx;

            if (ClvConductor.Length > 0 && ClvConductorint > 0)
            {
                var operador = (from x in VMD_BD.can_operadores
                                where x.cveemp == ClvConductorint && x.status == "A"
                                select x).FirstOrDefault();

                if (operador != null)
                {//Checo si hay vajes cargados, o sea, secuencia, si no, creo una en blanco

                    if(IGlobales.Corrida.NumViajes <= 0)
                    {
                        IdSecx = ISecuencia.GetIdSecuenciaTemp();

                        //Obtengo los datos para la venta, De Origen y Destino Final, de la corrida

                        IGlobales.Corrida.IDSecuencia = IdSecx;
                        IGlobales.Corrida.Region = Parametros_Inicio.Region.ToString();
                        IGlobales.Corrida.Marca = Parametros_Inicio.Marca.ToString();
                        IGlobales.Corrida.Zona = Parametros_Inicio.Zona;
                        IGlobales.Corrida.Servicio = Int32.Parse(Parametros_Inicio.Servicio);
                        IGlobales.Corrida.Autobus = Parametros_Inicio.Autobus;
                        IGlobales.Corrida.VersionSecuencia = 0;

                        IGlobales.ClearViajes();

                        //agrego un viaje en Blanco
                        IGlobales.AddViajeBlanco(IGlobales.Corrida.NumViajes, 0, 0, 0, 0, ClvConductorint, 0, "O.L.", "D.L.", "V.L.", IdSecx, false);

                        IGlobales.Corrida.ViajeActual = 1;
                        IGlobales.Corrida.SecuenciaAbierta = true;
                        IGlobales.ViajeAbierto = true;

                        IGlobales.Corrida.ConductorActualID = ClvConductorint;
                        IGlobales.Corrida.ConductorActualNom = operador.nombre;


                    }
                    //Punto de interrupcion

                    IGlobales.conductor = ClvConductorint;

                    IGlobales.ReiniciarFLagsViaje();
                    IGlobales.ViajeAbierto = true;

                    if (IGlobales.Corrida.ViajeActual > 0)
                    {
                        IGlobales.Corrida.SecuenciaAbierta = true;
                        IGlobales.Corrida.ConductorActualID = IGlobales.conductor;
                        IGlobales.Corrida.ConductorActualNom = operador.nombre;

                        //GrabaSecuenciaTemp();
                        //RegistroCANTemp(IGlobales.Corrida.IDSecuencia, IGlobales.Viajes[IGlobales.Corrida.ViajeActual].IDDetSecuencia, IGlobales.Corrida.ConductorActualID);

                        //¿Se ocupa la secuencia temporal?
                        
                    }

                    //MuestraSecuencia
                    //Si no es un proceso automatico de carga de secuencia anterior (Viaje abierto previmanete)

                    if (!IGlobales.CargandoViajeAnterior)
                    {
                        if(CANPob != null)
                        {
                            //GrabaRegistroCAN("VA", CANPob)
                            RegistroCAN("VA", CANPob);
                        }
                        else
                        {
                            //GrabaRegistroCAN("VA");
                            RegistroCAN("VA");
                        }

                        //PantallaCAN Conductor valido
                        //MuestraModo();

                        IProtocoloCAN.InicializarVariablesCAN();

                    }

                    //Reiniciamos las variables del protocolo tipo DG
                    IProtocoloCAN.KmsIniDg = IProtocoloCAN.TripDistanceEstimationVS;
                    IProtocoloCAN.LtsIniDG = IProtocoloCAN.FuelEconomyEstimationFE;

                    if ((bool)Parametros_Inicio.CONDUSAT)
                    {
                        try
                        {
                            //Mandamos a cerrar el viaje a CONDUSAT
                            RegistroCondusat(Parametros_Inicio.Autobus, IGlobales.Corrida.ConductorActualID.ToString(), "Operador", RecuperarUltimaFechaCAN(), "0", "0");
                        }
                        catch
                        {

                        }
                        
                    }

                    //Mandamos los datos hacia SAM
                   if (CANPob != null)
                    {
                        ViajeSAM(CANPob);
                    }
                    else
                    {
                        ViajeSAM();
                    }
                    

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Realiza un cambio de manos
    /// </summary>
    /// <param name="ClvConductor"></param>
    /// <param name="CANPob"></param>
    /// <returns></returns>
    public bool CambioDeManos(string ClvConductor, can_poblaciones CANPob = null)
    {
        try
        {
            int ClvConductorint = Int32.Parse(ClvConductor);

            if( ClvConductor.Length > 0 && ClvConductorint > 0)
            {
                var operador = (from x in VMD_BD.can_operadores
                                where x.cveemp == ClvConductorint && x.status == "A"
                                select x).FirstOrDefault();

                if (operador != null)
                {
                    if( ClvConductorint != IGlobales.conductor)
                    {
                        IGlobales.conductor = ClvConductorint;
                        IGlobales.ReiniciarFLagsViaje();

                        //pasamos por aquí debido a que se trabaja con listas
                        int numviaje = IGlobales.Corrida.NumViajes - 1;

                        if (IGlobales.Corrida.ViajeActual > 0)
                        {
                            IGlobales.Corrida.SecuenciaAbierta = true;
                            IGlobales.Corrida.ConductorActualID = IGlobales.conductor;
                            IGlobales.Corrida.ConductorActualNom = operador.nombre;

                            //Asigno los datos del nuevo conductor

                            
                            IGlobales.Viajes[numviaje].FechaHora = DateTime.Now;
                            IGlobales.Viajes[numviaje].Conductor1 = IGlobales.conductor;

                            //GrabasecuenciaTemp();

                        //Se ocupa la secuencia Temporal?????
                        }

                        //MuestraSecuencia

                        //GrabaRegistroCAN("V");
                        RegistroCAN("V");
                        //thread.Sleep(1000); ¿Neta?

                        if (CANPob != null)
                        {
                            ///GrabaRegistroCAN("CM", CANPob);
                            RegistroCAN("CM", CANPob);
                        }
                        else
                        {
                            ///GrabaRegistroCAN("CM");
                            RegistroCAN("CM");
                        }

                        //PantallaCAN CONDUCTOR VALIDO
                        //MuestraModo();

                        IProtocoloCAN.InicializarVariablesCAN();

                        //Reiniciamos las variables del protocolo tipo DG
                        IProtocoloCAN.KmsIniDg = IProtocoloCAN.TripDistanceEstimationVS;
                        IProtocoloCAN.LtsIniDG = IProtocoloCAN.FuelEconomyEstimationFE;

                        if ((bool)Parametros_Inicio.CONDUSAT)
                        {
                            try
                            {
                                //pasamos por aquí debido a que se trabaja con listas
                                var numviaje_2 = IGlobales.Corrida.ViajeActual - 1;
                                string fecha = IGlobales.Viajes[numviaje_2].FechaHora.ToString();

                                //Mandamos a cerrar el viaje a CONDUSAT
                                RegistroCondusat(Parametros_Inicio.Autobus, IGlobales.Corrida.ConductorActualID.ToString(), "Operador", fecha, "0", "1");
                            }
                            catch
                            {

                            }
                            
                        }

                        //Mandamos los datos hacia SAM
                        if (CANPob != null)
                        {
                            ViajeSAM(CANPob);
                        }
                        else
                        {
                            ViajeSAM();
                        }

                        return true;

                    }
                    else
                    {//No es diferente conductor
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Relaiza un cierre de viaje
    /// </summary>
    /// <param name="ClvConductor"></param>
    /// <param name="CANPob"></param>
    /// <returns></returns>
    public bool CerrarViaje(string ClvConductor, can_poblaciones CANPob = null)
    {
        try
        {
            int ClvConductorint = Int32.Parse(ClvConductor);

            if( ClvConductor.Length > 0 && ClvConductorint > 0)
            {

                var operador = (from x in VMD_BD.can_operadores
                                where x.cveemp == ClvConductorint && x.status == "A"
                                select x).FirstOrDefault();

                if (operador != null)
                {
                    if(ClvConductorint == IGlobales.conductor)
                    {
                        if (IGlobales.ViajeAbierto)
                        {

                            IGlobales.ReiniciarFLagsViaje();

                            IGlobales.ViajeAbierto = false;
                            IGlobales.Corrida.SecuenciaAbierta = false;
                            //LimpiaSecTemp()
                            //MuestraSecuencia();

                            if (CANPob != null)
                            {
                                //GrabaRegistroCAN("VC", CANPob);
                                RegistroCAN("VC", CANPob);
                            }
                            else
                            {
                                //GrabaRegistroCAN("VC");
                                RegistroCAN("VC");
                            }

                            IGlobales.conductor = 0;

                            //PantallaCAn Conductor valido, viaje cerrado
                            //MuestraModo

                            //Checo si hay mas viajes, si no marco la secuencia como finalizada

                            if(IGlobales.Corrida.ViajeActual == IGlobales.Corrida.NumViajes)
                            {
                                can_secuencia sec = (from x in VMD_BD.can_secuencia
                                                     where x.idsecuencia == IGlobales.Corrida.IDSecuencia
                                                     select x).FirstOrDefault();

                                if (sec != null)
                                {
                                    sec.confirmado = true;
                                    VMD_BD.SaveChanges();
                                }
                            }

                            IGlobales.ClearViajes();
                            IGlobales.Corrida.ConductorActualID = 0;
                            IGlobales.Corrida.ConductorActualNom = "";

                            //Mando el evento de detener pelicula
                            //Ya se encuentra el evento en el front

                            //Inicializo las varibales de tarjeta de CAN
                            IProtocoloCAN.InicializarVariablesCAN();

                            //GrabaRegistroCAN("T");
                            RegistroCAN("T");

                            //Reiniciamos las variables del protocolo tipo DG
                            IProtocoloCAN.KmsIniDg = IProtocoloCAN.TripDistanceEstimationVS;
                            IProtocoloCAN.LtsIniDG = IProtocoloCAN.FuelEconomyEstimationFE;

                            if ((bool)Parametros_Inicio.CONDUSAT)
                            {
                                try
                                {
                                    //Mandamos a cerrar el viaje a CONDUSAT
                                    RegistroCondusat(Parametros_Inicio.Autobus, IGlobales.Corrida.ConductorActualID.ToString(), "Operador", "0", RecuperarUltimaFechaCAN(), "0");
                                }
                                catch
                                {

                                }
                            }

                            //Mandamos los datos hacia SAM
                            if (CANPob != null)
                            {
                                ViajeSAM(CANPob);
                            }
                            else
                            {
                                ViajeSAM();
                            }

                            return true;

                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
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
    }

    #endregion

    #region "Métodos Privados"

    /// <summary>
    /// Se encarga de recuperar la ultima fecha de un evento de viaje de CAN
    /// </summary>
    /// <returns></returns>
    private string RecuperarUltimaFechaCAN()
    {
        can_movtoscan movcan = (from x in VMD_BD.can_movtoscan
                                where x.accion.Equals("VC")
                                orderby x.fechahora descending
                                select x).FirstOrDefault();

        string fechafirma = string.Empty;

        if (movcan != null)
        {
            fechafirma = movcan.fechahora.ToString();
        }

        return fechafirma;
    }
    #endregion

}

