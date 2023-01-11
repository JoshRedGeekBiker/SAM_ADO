using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;

public class Conductor : IBDContext, IDisposable
{
    public vmdEntities VMD_BD { get; }
    private can_parametrosinicio ParametrosInicio;

    private Globales _Globales;
    private Bitacora _Bitacora;
    private ProtocoloCAN _ProtocoloCAN;
    private Secuencia _Secuencia;

    //Evento para avisar el viaje
    public delegate void Viaje(string operador, string nom_operador);
    public event Viaje AvisarViajeaFront;


    /// <summary>
    /// Constructor principal
    /// </summary>
    /// <param name="IGlobales"></param>
    /// <param name="IBitacora"></param>
    /// <param name="IProtocoloCAN"></param>
    /// <param name="ISecuencia"></param>
    public Conductor(ref Globales IGlobales, ref Bitacora IBitacora, ref ProtocoloCAN IProtocoloCAN, ref Secuencia ISecuencia)
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();

        _Globales = IGlobales;
        _Bitacora = IBitacora;
        _ProtocoloCAN = IProtocoloCAN;
        _Secuencia = ISecuencia;
    }

    /// <summary>
    /// Valida la clave del conductor
    /// </summary>
    /// <param name="ClvCondu"></param>
    /// <returns></returns>
    public string ValidarConductor(string ClvCondu)
    {
        try
        {
            if (ClvCondu.Length > 0) //Verificamos que la cadena no esté vacia
            {
                int ClvConduInt = Int32.Parse(ClvCondu);

                if(ClvConduInt > 0)//verificamos que la clave sea mayor a cero
                {
                    var conductor = (from x in VMD_BD.can_operadores
                                     where x.cveemp == ClvConduInt && x.status == "A"
                                     select x).FirstOrDefault();

                    if(conductor != null)//se verifica la existencia del registro del conductor
                    {

                        return conductor.nombre;
                    }
                    else
                    {
                        return "NE";
         
                    }
                }
                else
                {
                    return "NE";
         
                }
            }
            else
            {
                return "NE";
         
            }
        }
        catch
        {
            return "NE";
         
        }
    }

    /// <summary>
    /// Checa el numero del conductor y determina la acción a realizar
    /// </summary>
    /// <param name="ClaveCond"></param>
    public void ChecaConductor(string ClaveCond)
    {
        try
        {
            int ClaveCondInt = Int32.Parse(ClaveCond);

            if (ClaveCond.Length > 0 && ClaveCondInt > 0)
            {
                var operador = (from x in VMD_BD.can_operadores
                                where x.cveemp == ClaveCondInt && x.status == "A"
                                select x).FirstOrDefault();

                if (operador != null)
                {
                    if (_Globales.ViajeAbierto)
                    {

                        if (_Globales.EsperaFinViaje)//Cerrar viaje
                        {
                            _Globales.ReiniciarFLagsViaje();
                            _Globales.ViajeAbierto = false;
                            _Globales.Corrida.SecuenciaAbierta = false;
                            //LimpiaSecuenciaTemp()***********************************
                            //MuestraSecuencia()************************************
                            _Bitacora.GrabaRegistroCAN("VC");

                            //En el caso de que fuera DG, grabo los datos iniciales
                            _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                            _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;
                            _Globales.conductor = 0;
                            //PantallaCAN CONDUCTOR VALIDO
                            //MuestraModo();

                            //Checo si hay más viajes, si no, marco secuencia como finalizada

                            if (_Globales.Corrida.ViajeActual == _Globales.Corrida.NumViajes)
                            {
                                can_secuencia sec = (from x in VMD_BD.can_secuencia
                                                     where x.idsecuencia == _Globales.Corrida.IDSecuencia
                                                     select x).FirstOrDefault();
                                sec.confirmado = true;
                                VMD_BD.SaveChanges();

                            }

                            _Globales.ClearViajes();
                            _Globales.Corrida.ConductorActualID = 0;
                            _Globales.Corrida.ConductorActualNom = "";

                            //Checo si hay una pauta en proceso y la finalizo
                            //************************************************************************


                            //Inicializo las varibales de la tarjeta de CAN
                            _ProtocoloCAN.InicializarVariablesCAN();

                        }
                        else if (_Globales.EsperaCambioManos) //Cambio de manos
                        {
                            if (ClaveCondInt != _Globales.conductor)
                            {
                                _Globales.conductor = ClaveCondInt;
                                _Globales.ReiniciarFLagsViaje();

                                if (_Globales.Corrida.ViajeActual > 0)
                                {
                                    _Globales.Corrida.SecuenciaAbierta = true;
                                    _Globales.Corrida.ConductorActualID = _Globales.conductor;
                                    _Globales.Corrida.ConductorActualNom = operador.nombre;

                                    //Asigno los datos del nuevo conductor
                                    _Globales.Viajes[_Globales.Corrida.NumViajes].FechaHora = DateTime.Now;
                                    _Globales.Viajes[_Globales.Corrida.NumViajes].Conductor1 = _Globales.conductor;

                                    //GrabaSecuenciaTemp();*****************************
                                }

                                //MuestraSecuencia();******************************
                                _Bitacora.GrabaRegistroCAN("CM");

                                //En el caso de que fuera DG, grabo los datos iniciales

                                _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                                _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;
                                //PantallaCAN CONDUCTOR VALIDO
                                //MuestraModo();

                                //inicializo las variables de la tarjeta de can
                                _ProtocoloCAN.InicializarVariablesCAN();

                            }
                            else
                            {
                                _Globales.ReiniciarFLagsViaje();
                                //PantallaCAN VIAJE ABIERTO CONTINUA VIAJE MISMO CONDUCTOR
                                //MuestraModo();
                            }

                        }
                    }
                    else if (_Globales.EsperaIniciaViaje) //Abrir Viaje
                    {
                        int IdSecx;
                        //Checo si hay viajes cargados, osea una secuencia, si no, creo una en blanco
                        if (_Globales.Corrida.NumViajes <= 0)
                        {
                            IdSecx = _Secuencia.GetIdSecuenciaTemp();

                            //Obtengo los datos para la venta, de origen y destino final, de la corrida
                            _Globales.Corrida.IDSecuencia = IdSecx;
                            _Globales.Corrida.Region = ParametrosInicio.Region.ToString();
                            _Globales.Corrida.Marca = ParametrosInicio.Marca.ToString();
                            _Globales.Corrida.Zona = ParametrosInicio.Zona;
                            _Globales.Corrida.Servicio = Int32.Parse(ParametrosInicio.Servicio);
                            _Globales.Corrida.Autobus = ParametrosInicio.Autobus;
                            _Globales.Corrida.VersionSecuencia = 0;

                            _Globales.ClearViajes();

                            //Agrego un viaje en blanco

                            _Globales.AddViaje();

                            //pasamos por aquí debido a que se trabaja con listas

                            var NumViajes = _Globales.Corrida.NumViajes - 1;

                            _Globales.Viajes[NumViajes].FechaHora = DateTime.Now;
                            _Globales.Viajes[NumViajes].NumTramo = 0;
                            _Globales.Viajes[NumViajes].OrigenID = 0;
                            _Globales.Viajes[NumViajes].DestinoID = 0;
                            _Globales.Viajes[NumViajes].ViaID = 0;
                            _Globales.Viajes[NumViajes].Conductor1 = ClaveCondInt;
                            _Globales.Viajes[NumViajes].Conductor2 = 0;
                            _Globales.Viajes[NumViajes].OrigenDes = "O.L.";
                            _Globales.Viajes[NumViajes].DestinoDes = "D.L.";
                            _Globales.Viajes[NumViajes].ViaDes = "V.L.";
                            _Globales.Viajes[NumViajes].IDDetSecuencia = IdSecx;
                            _Globales.Viajes[NumViajes].Confirmado = false;
                            //No está confirmado, lo asigno como actual
                            _Globales.Corrida.ViajeActual = 1;
                            _Globales.Corrida.SecuenciaAbierta = true;
                            _Globales.ViajeAbierto = true;
                            _Globales.Corrida.ConductorActualID = ClaveCondInt;
                            _Globales.Corrida.ConductorActualNom = operador.nombre;
                            _Globales.conductor = ClaveCondInt;

                            AvisarViajeaFront(operador.cveemp.ToString(), operador.nombre);

                            //PantallaCAN = VIAJE ABIERTO CONDUCTOR
                            //MuestraModo;


                        }

                        _Globales.conductor = ClaveCondInt;
                        _Globales.ReiniciarFLagsViaje();
                        _Globales.ViajeAbierto = true;

                        if (_Globales.Corrida.ViajeActual > 0)
                        {
                            _Globales.Corrida.SecuenciaAbierta = true;
                            _Globales.Corrida.ConductorActualID = _Globales.conductor;
                            _Globales.Corrida.ConductorActualNom = operador.nombre;

                            //GrabasecuenciaTemp;*********************
                        }

                        //MuestraSecuenciaTemp***********************

                        //Si no es un proceso automatico de carga de secuencia anterior (viaje avierto previamente)

                        if (!_Globales.CargandoViajeAnterior)
                        {
                            _Bitacora.GrabaRegistroCAN("VA");

                            //En el caso de que fuera DG, grabo los datos iniciales al momento de abrir el viaje,
                            //si  es ST estas variables
                            _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                            _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;

                            //Inicializo las variables de la tarjeta de CAN
                            _ProtocoloCAN.InicializarVariablesCAN();
                        }

                        //PantallaCAN = CONDUCTOR VALIDO"
                        //MuestraModo();

                    }
                    else
                    {//Conductor No valido
                        //Si no se valida la clave de conductor ni secuencia, entonces dejo pasar cualquier clave
                        if (!(bool)ParametrosInicio.ValidarCveCondSec)
                        {
                            if (_Globales.ViajeAbierto)
                            {
                                if (_Globales.EsperaFinViaje)//cerrar viaje
                                {
                                    _Globales.ReiniciarFLagsViaje();
                                    _Globales.ViajeAbierto = false;
                                    _Globales.Corrida.SecuenciaAbierta = false;
                                    _Globales.Corrida.ViajeActual = 0;
                                    //LimpiaSecuenciaTemp
                                    //MuestraSecuencia()
                                    _Bitacora.GrabaRegistroCAN("VC");

                                    //En el caso de que fuera DG, grabo los datos iniciales

                                    _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                                    _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;

                                    _Globales.conductor = 0;
                                    //PantallaCAN = Conductor Valido;
                                    //MuestraModo();

                                    //Checo si hay más viajes, si no, marco secuencia como finalizada

                                    if (_Globales.Corrida.ViajeActual == _Globales.Corrida.NumViajes)
                                    {
                                        can_secuencia sec = (from x in VMD_BD.can_secuencia
                                                             where x.idsecuencia == _Globales.Corrida.IDSecuencia
                                                             select x).FirstOrDefault();

                                        if (sec != null)
                                        {
                                            sec.confirmado = true;
                                            VMD_BD.SaveChanges();
                                        }

                                        //inicializo las variables de la tarjeta de CAN
                                        _ProtocoloCAN.InicializarVariablesCAN();
                                    }

                                }
                                else if (_Globales.EsperaCambioManos)
                                {
                                    if (ClaveCondInt != _Globales.conductor)
                                    {
                                        _Globales.conductor = ClaveCondInt;
                                        _Globales.ReiniciarFLagsViaje();
                                        if (_Globales.Corrida.ViajeActual > 0)
                                        {
                                            _Globales.Corrida.SecuenciaAbierta = true;
                                            _Globales.Corrida.ConductorActualID = _Globales.conductor;
                                            _Globales.Corrida.ConductorActualNom = _Globales.ConductorPredeterminado;

                                            //grabaSecuenciaTemp();
                                        }

                                        //MuestraSecuencia();
                                        _Bitacora.GrabaRegistroCAN("CM");

                                        //En el caso de DG, grabo los datos iniciales

                                        _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                                        _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;
                                        //PantallaCAN CONDUCTOR VALIDO
                                        //MuestraModo();

                                        //Inicializo las variables de la tarjeta de CAN
                                        _ProtocoloCAN.InicializarVariablesCAN();

                                    }
                                    else
                                    {
                                        _Globales.ReiniciarFLagsViaje();
                                        //PantallaCAN VIAJE ABIERTO CONTINUA VIAJE MISMO CONDUCTOR
                                        //MuestraModo();

                                    }
                                }

                            }
                            else if (_Globales.EsperaIniciaViaje)// Abrir Viaje
                            {
                                //Checo si hay viajes cargados, o sea, secuencia, si no, creo una en blanco
                                if (_Globales.Corrida.NumViajes <= 0)
                                {
                                    int IdSecx = _Secuencia.GetIdSecuenciaTemp();

                                    //Obtengo los datos para la vetna, de Origen y Destino Final, de la corrida

                                    _Globales.Corrida.IDSecuencia = IdSecx;
                                    _Globales.Corrida.Region = ParametrosInicio.Region.ToString();
                                    _Globales.Corrida.Marca = ParametrosInicio.Marca.ToString();
                                    _Globales.Corrida.Zona = ParametrosInicio.Zona;
                                    _Globales.Corrida.Servicio = Int32.Parse(ParametrosInicio.Servicio);
                                    _Globales.Corrida.Autobus = ParametrosInicio.Autobus;
                                    _Globales.Corrida.VersionSecuencia = 0;

                                    _Globales.ClearViajes();

                                    //Agrego un viaje en blanco

                                    _Globales.AddViaje();

                                    var numViaje = _Globales.Corrida.NumViajes;

                                    _Globales.Viajes[numViaje].FechaHora = DateTime.Now;
                                    _Globales.Viajes[numViaje].NumTramo = 0;
                                    _Globales.Viajes[numViaje].OrigenID = 0;
                                    _Globales.Viajes[numViaje].DestinoID = 0;
                                    _Globales.Viajes[numViaje].ViaID = 0;
                                    _Globales.Viajes[numViaje].Conductor1 = ClaveCondInt;
                                    _Globales.Viajes[numViaje].Conductor2 = 0;
                                    _Globales.Viajes[numViaje].OrigenDes = "O.L.";
                                    _Globales.Viajes[numViaje].DestinoDes = "D.L.";
                                    _Globales.Viajes[numViaje].ViaDes = "V.L.";
                                    _Globales.Viajes[numViaje].IDDetSecuencia = IdSecx;
                                    _Globales.Viajes[numViaje].Confirmado = false;

                                    //No está confirmado, lo asgino como actual

                                    _Globales.Corrida.ViajeActual = 1;
                                    _Globales.Corrida.SecuenciaAbierta = true;
                                    _Globales.ViajeAbierto = true;
                                    _Globales.Corrida.ConductorActualID = ClaveCondInt;
                                    _Globales.Corrida.ConductorActualNom = _Globales.ConductorPredeterminado;
                                    //PantallaCAN VIAJE ABIERTO
                                    //MuestraModo();
                                }

                                _Globales.conductor = ClaveCondInt;

                                _Globales.ReiniciarFLagsViaje();

                                _Globales.ViajeAbierto = true;

                                if (_Globales.Corrida.ViajeActual > 0)
                                {
                                    _Globales.Corrida.SecuenciaAbierta = true;
                                    _Globales.Corrida.ConductorActualID = _Globales.conductor;
                                    _Globales.Corrida.ConductorActualNom = _Globales.ConductorPredeterminado;

                                    //GrabaSecuenciaTemp();

                                }

                                //MuestraSecuencia();

                                //Si no es un proceso automatico de carga de secuencia anterior (Viaje Abierto previmanete)

                                if (!_Globales.CargandoViajeAnterior)
                                {
                                    _Bitacora.GrabaRegistroCAN("VA");

                                    //En el caso de que fuera DG, grabo los datos iniciales
                                    _ProtocoloCAN.KmsIniDg = _ProtocoloCAN.TripDistanceEstimationVS;
                                    _ProtocoloCAN.LtsIniDG = _ProtocoloCAN.FuelEconomyEstimationFE;
                                }

                                //PantallaCAN "Conductor valido"
                                //MuestraModo();

                                //Si no es un proceso automatico de carga de secuencia anterior (Viaje Abierto previmanete)

                                if (!_Globales.CargandoViajeAnterior)
                                {
                                    //Inicializa las vaiables de la tarjeta de Can
                                    _ProtocoloCAN.InicializarVariablesCAN();
                                }

                            }
                        }
                        else
                        {
                            //PantallaCAN CONDUCTOR NO VALIDO
                            //MuestraModo();
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();
            //MuestraError("ChecaConductor");
        }
    }

    public void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);

    }
}

