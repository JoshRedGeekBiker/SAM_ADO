using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;

public class Bitacora : IBDContext
{

    public vmdEntities VMD_BD { get; }
    public can_parametrosinicio ParametrosInicio;

    private bool ModoPrueba = false;

    //Variables de Lógicas

    private Globales IGlobales;
    private ProtocoloCAN IProtocoloCAN;

    public Bitacora(ref Globales _Globales, ref ProtocoloCAN _ProtocoloCAN, bool _modoPrueba)
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();
        this.ModoPrueba = _modoPrueba;

        IGlobales = _Globales;
        IProtocoloCAN = _ProtocoloCAN;
    }

    #region "Métodos públicos"

    /// <summary>
    /// Se encarga de grabar los registros en MovtosCAN
    /// </summary>
    /// <param name="accion"></param>
    /// <param name="CANPob"></param>
    public void GrabaRegistroCAN(string accion, can_poblaciones CANPob = null)
    {
        try
        {
            //Evitamos que grabe un registro mientras insertamos la población de forma manual
            if (!IGlobales.GrabandoCAN)
            {
                
                IGlobales.GrabandoCAN = true;

                int NumVuelta = 0, NumReg = 0;

                int Via, Origen, NumTramo, Destino, CualConductor, ViajeActual = 0;
                
                DateTime fechaviaje;


                if (IGlobales.Corrida.ViajeActual != 0)
                {
                    ViajeActual = IGlobales.Corrida.ViajeActual;

                    if(ViajeActual > 0)
                    {
                        ViajeActual = ViajeActual - 1;
                    }

                    NumTramo = IGlobales.Viajes[ViajeActual].NumTramo;
                    Origen = IGlobales.Viajes[ViajeActual].OrigenID;
                    Destino = IGlobales.Viajes[ViajeActual].DestinoID;
                    Via = IGlobales.Viajes[ViajeActual].ViaID;
                    fechaviaje = IGlobales.Viajes[ViajeActual].FechaHora;
                    CualConductor = IGlobales.Corrida.ConductorActualID;

                    //Si no hay acción, pero si hay viaje avierto, lo marco con una V de Viaje

                    if (accion.Length == 0)
                    {
                        accion = "V";

                    }
                    else if (accion.Equals("VA"))
                    {
                        if (CANPob != null)
                        {
                            IGlobales.Viajes[ViajeActual].OrigenID = (int)CANPob.idpob;
                            Origen = (int)CANPob.idpob;
                        }
                    }
                    else if (accion.Equals("CM"))
                    {
                        if (CANPob != null)
                        {
                            IGlobales.Viajes[ViajeActual].OrigenID = (int)CANPob.idpob;
                            Origen = (int)CANPob.idpob;
                        }

                    }
                    else if (accion.Equals("VC"))
                    {
                        if (CANPob != null)
                        {
                            IGlobales.Viajes[ViajeActual].DestinoID = (int)CANPob.idpob;
                            Destino = (int)CANPob.idpob;
                        }
                    }

                }
                else
                {
                    NumTramo = 0;
                    Origen = 0;
                    Destino = 0;
                    Via = 0;
                    fechaviaje = DateTime.Now;
                    CualConductor = 0;
                    //si no hay accion, pero si hay viaje abierto, lo marco con una T de Traslado
                    if (accion.Length == 0)
                    {
                        accion = "T";
                    }
                }

                GetNumRegistroSigt(ref NumVuelta, ref NumReg);

                //Linea a quitar en producción

                //IProtocoloCAN.NombreProtocolo = "DG_DIDCOM_";

                if(this.ModoPrueba && IProtocoloCAN.NombreProtocolo.Equals(""))
                {
                    IProtocoloCAN.NombreProtocolo = "ModoDemo";
                }else if (IProtocoloCAN.NombreProtocolo.Equals(""))
                {
                    IProtocoloCAN.NombreProtocolo = "Sin Protocolo";
                }


                if (IProtocoloCAN.NombreProtocolo.Substring(0, 2).Equals("DG"))
                {
                    ///

                    if ((IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG) > 0 && (IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg) > 0)
                    {
                        if (IProtocoloCAN.FuelEconomyEstimationFE < IProtocoloCAN.LtsIniDG)
                        {
                            return;
                        }

                        if (IProtocoloCAN.TripDistanceEstimationVS < IProtocoloCAN.KmsIniDg)
                        {
                            return;
                        }


                        if (CANPob == null)
                        {
                            //Grabo con la coordenadas del GPS

                            GrabaConGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, accion, NumVuelta, NumReg);

                        }
                        else
                        {
                            //grabo con las coordenadas de la poblacion indicada por el operador

                            GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);


                            //Grabo registros adicionales para Procesa<viajes CAN

                            if (accion.Equals("VA") || (accion.Equals("CM")))
                            {
                                accion = "V";

                            }
                            else if (accion.Equals("VC"))
                            {
                                accion = "T";
                                CualConductor = 0;
                                IProtocoloCAN.InicializarVariablesCAN();

                            }

                            for (int i = 0; i <= 2; i++)
                            {
                                if (accion.Equals("T"))
                                {
                                    fechaviaje = DateTime.Now;
                                }

                                IGlobales.GrabandoCAN = true;
                                GetNumRegistroSigt(ref NumVuelta, ref NumReg);
                                System.Threading.Thread.Sleep(1000);
                                GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);

                            }

                            IGlobales.GrabandoCAN = false;
                        }

                    }
                    else
                    {

                        if (CANPob == null)
                        {   //grabo las coordenadas del GPS
                            GrabaConGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, accion, NumVuelta, NumReg);
                        }
                        else
                        {// Grabo con las coordenadas de la poblacion indicada por el operador

                            GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);

                            //Grabo registro adicionales para Procesaviajes CAN
                            if (accion.Equals("VA") || accion.Equals("CM"))
                            {
                                accion = "V";
                            }
                            else if (accion.Equals("VC"))
                            {
                                accion = "T";
                                CualConductor = 0;
                                IProtocoloCAN.InicializarVariablesCAN();
                            }

                            for (int i = 0; i <= 2; i++)
                            {
                                if (accion.Equals("T"))
                                {
                                    fechaviaje = DateTime.Now;
                                }

                                IGlobales.GrabandoCAN = true;
                                GetNumRegistroSigt(ref NumVuelta, ref NumReg);
                                //System.Threading.Thread.Sleep(1000);
                                GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);

                            }

                            IGlobales.GrabandoCAN = false;

                        }
                    }

                    ///
                }
                else
                {
                    //Insertamos los registros para Multego y Scania

                    if (CANPob == null)
                    {
                        //grabo con las coordenadas del GPS
                        GrabaConGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, accion, NumVuelta, NumReg);

                        
                    }
                    else
                    {
                        //Grabo con las coordenadas de la poblacion indicada por el operador
                        GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);

                        //Grabo registro adicionales para Procesaviajes CAN
                        if (accion.Equals("VA") || accion.Equals("CM"))
                        {
                            accion = "V";
                        }
                        else if (accion.Equals("VC"))
                        {
                            accion = "T";
                            CualConductor = 0;
                            IProtocoloCAN.InicializarVariablesCAN();
                        }
                        int i;

                        for (i = 0; i <= 2; i++) //para el JOB de Procesa viajes CAN
                        {
                            if (accion.Equals("T"))
                            {
                                fechaviaje = DateTime.Now;
                            }

                            IGlobales.GrabandoCAN = true;
                            GetNumRegistroSigt(ref NumVuelta, ref NumReg);
                            System.Threading.Thread.Sleep(1000);
                            GrabaSinGPS(NumTramo, Origen, Destino, Via, fechaviaje, CualConductor, CANPob, accion, NumVuelta, NumReg);

                        }
                    }
                }

                IGlobales.GrabandoCAN = false;
            }
            
        }
        catch(Exception ex)
        {
            //MuestraError("GrabaResgistroCAN");
            IGlobales.GrabandoCAN = false;
        }
    }

    /// <summary>
    /// Se encarga de obtener el valor del siguiente registro
    /// </summary>
    /// <param name="NumVuelta"></param>
    /// <param name="NumReg"></param>
    public void GetNumRegistroSigt(ref int NumVuelta, ref int NumReg)
    {
        try
        {
            int VueltaTmp, RegTmp;

            can_contadores contador = (from x in VMD_BD.can_contadores
                                       select x).FirstOrDefault();

            if (contador != null)
            {//Sólo la vuelta, la leo directamente ya que el número de registro, si le sumo 1
                VueltaTmp = contador.numvuelta;
                RegTmp = contador.numreg + 1;
            }
            else
            {
                contador = new can_contadores();

                contador.numvuelta = 1;
                contador.numreg = 0;

                VMD_BD.can_contadores.Add(contador);
                VMD_BD.SaveChanges();
                VueltaTmp = 1;
                RegTmp = 1;
            }

            //Si es mayor a 1,500,000 inicializo el contador en 1, y al de vueltas le aumento 1
            if (RegTmp > 1500000)
            {
                RegTmp = 1;
                RegTmp = VueltaTmp + 1;
            }

            contador.numvuelta = VueltaTmp;
            contador.numreg = RegTmp;
            VMD_BD.SaveChanges();

            NumVuelta = VueltaTmp;
            NumReg = RegTmp;
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de grabar un registro de secuencia temporal
    /// </summary>
    /// <param name="IdSecuencia"></param>
    /// <param name="IdDetSecuencia"></param>
    /// <param name="IdConductor"></param>
    public void GrabaSecuenciaTemp(int IdSecuencia, int IdDetSecuencia, int IdConductor)
    {
        can_regsecuencia RegSec = (from x in VMD_BD.can_regsecuencia
                                   select x).FirstOrDefault();

        RegSec.idsecuencia = IdSecuencia;
        RegSec.orden = IdDetSecuencia;
        RegSec.conductor = IdConductor;

        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Validaciones de propiedades de protocolo antes de GrabarRegistroCAN
    /// </summary>
    public void GrabaCAN()
    {
        // 03/10/2013
        //Validamos si las revoluciones son mayor a cero en caso de no ser así no grabamos nada
        //27/10/2014
        //Cambio de condición para grabar ralentis en multegos y Scanias

        if( IProtocoloCAN.TachoShaftSpeed == 0 || (IProtocoloCAN.FuelEconomyEstimationFE == 0 && IProtocoloCAN.TripDistanceEstimationVS == 0))
        {
            return;
        }


        if(IProtocoloCAN.NombreProtocolo.Substring(0, 2).Equals("DG"))
        {
            if((IProtocoloCAN.FuelEconomyEstimationFE < IProtocoloCAN.LtsIniDG) || (IProtocoloCAN.LtsIniDG == 0))
            {
                return;
            }

            if ((IProtocoloCAN.TripDistanceEstimationVS < IProtocoloCAN.KmsIniDg) || (IProtocoloCAN.KmsIniDg == 0))
            {
                return;
            }

            if(((IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG)> 10000) && (((IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg) > 10000))){
                return;
            }

        }
        else
        {
            if((IProtocoloCAN.FuelEconomyEstimationFE > 10000) || (IProtocoloCAN.TripDistanceEstimationVS > 10000))
            {
                return;
            }
        }

        //Se valida que hayan pasado los segundos mínimos para cada grabado. (Se deja sólo el símbolo de Myor, para que sean más de 5 segs)
        //If Abs(DateDiff("s", UltimoGrabado, Now)) > Param_CAN_SegsGraba Then

        //Asi cómo también se valida que sii sobre pasa los RPMS o Vel de exceso (de CAN), se grabe cada vez que el timer pase por aquí

        if (((DateTime.Now - IGlobales.UltimoCANGrabado).TotalSeconds > ParametrosInicio.SegsGraba) || (IProtocoloCAN.TachoVehicleSpeed > ParametrosInicio.ExcesosVel) || (IProtocoloCAN.TachoShaftSpeed > ParametrosInicio.ExcesosRPMs))
        {
            IGlobales.UltimoCANGrabado = DateTime.Now;
            GrabaRegistroCAN("");
        }
    }

    #endregion


    #region "Métodos Privados"

    /// <summary>
    /// Inserta en MovtosCAN con la población indicada por el conductor
    /// </summary>
    /// <param name="_NumTramo"></param>
    /// <param name="_Origen"></param>
    /// <param name="_Destino"></param>
    /// <param name="_Via"></param>
    /// <param name="_FechaViaje"></param>
    /// <param name="_CualConductor"></param>
    /// <param name="CANPob"></param>
    /// <param name="accion"></param>
    /// <param name="_NumVuelta"></param>
    /// <param name="_NumReg"></param>
    private void GrabaSinGPS(int _NumTramo, int _Origen, int _Destino, int _Via, DateTime _FechaViaje, int _CualConductor, can_poblaciones CANPob, string accion, int _NumVuelta, int _NumReg)
    {
        long ultimoId = (from x in VMD_BD.can_movtoscan
                         orderby x.id descending
                         select x.id).FirstOrDefault();

        long nuevoId = ultimoId + 1;

        can_movtoscan movtos = new can_movtoscan();

        movtos.idregion = IGlobales.Corrida.Region;
        movtos.idmarca = IGlobales.Corrida.Marca;
        movtos.idzona = IGlobales.Corrida.Zona;
        movtos.servicio = IGlobales.Corrida.Servicio;
        movtos.numerotramo = _NumTramo;
        movtos.idsecuencia = IGlobales.Corrida.IDSecuencia;
        movtos.origenviaje = _Origen;
        movtos.destinoviaje = _Destino;
        movtos.via = _Via;
        movtos.fechaviaje = _FechaViaje;
        movtos.operador = _CualConductor;
        movtos.autobus = ParametrosInicio.Autobus;
        movtos.fechahora = DateTime.Now;
        movtos.latitud = Math.Abs(CANPob.latitud);
        movtos.longitud = Math.Abs(CANPob.longitud);
        movtos.ns = CANPob.ns;
        movtos.we = CANPob.we;
        movtos.gps_velocidad = Convert.ToSingle(IGlobales.UltVel);
        movtos.accion = accion;
        movtos.status = "A";
        movtos.FuelLevel = Convert.ToSingle(IProtocoloCAN.FuelLevel);
        movtos.TachoShaftSpeed = Convert.ToSingle(IProtocoloCAN.TachoShaftSpeed);
        movtos.KmPerLiter = Convert.ToSingle(IProtocoloCAN.KmPerLiter);
        movtos.TachoVehicleSpeed = Convert.ToSingle(IProtocoloCAN.TachoVehicleSpeed);
        movtos.LiterPerHour = IProtocoloCAN.FuelEconomyEstimationFE;
        movtos.AmbientTemp = Convert.ToSingle(IProtocoloCAN.AmbientTemp);
        movtos.EconomyAverageKMpL = Convert.ToSingle(IProtocoloCAN.EconomyAverageKMpL);
        movtos.FuelEconomyEstimationLMAF = Convert.ToSingle(IProtocoloCAN.FuelEconomyEstimationLMAF);
        movtos.FuelEconomyEstimationFE = Convert.ToSingle(IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG);

        var RealKmPerTemp = Convert.ToSingle((IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg) / (IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG));

        if (RealKmPerTemp.Equals(Single.NaN) || RealKmPerTemp.Equals(Single.PositiveInfinity) || RealKmPerTemp.Equals(Single.NegativeInfinity))
        {
            movtos.RealKmPerLiterHighLevel = 0;
        }
        else
        {
            movtos.RealKmPerLiterHighLevel = RealKmPerTemp;
        }

        movtos.TotalDistance = IProtocoloCAN.TripDistanceEstimationVS;
        movtos.TripDistance = Convert.ToSingle(IProtocoloCAN.TripDistance);
        movtos.TripDistanceEstimationVS = Convert.ToSingle(IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg);
        movtos.TripDistanceEstimationWVS = Convert.ToSingle(IProtocoloCAN.TripDistanceEstimationWVS);
        movtos.WheelSpeed = Convert.ToSingle(IProtocoloCAN.WheelSpeed);
        movtos.EngineForce = Convert.ToSingle(IProtocoloCAN.EngineForce);
        movtos.EngineLoad = Convert.ToSingle(IProtocoloCAN.EngineLoad);
        movtos.MassAirFlow = Convert.ToSingle(IProtocoloCAN.MassAirFlow);
        movtos.Protocolo = IProtocoloCAN.NombreProtocolo;
        movtos.IDDireccion = Convert.ToInt32(ParametrosInicio.IDDireccion);
        movtos.IDRegionOperativa = Convert.ToInt32(ParametrosInicio.IDRegionOperativa);
        movtos.NumVuelta = _NumVuelta;
        movtos.NumReg = _NumReg;
        movtos.NumVueltaServer = 0;
        movtos.NumRegServer = 0;
        movtos.id = nuevoId;

        VMD_BD.can_movtoscan.Add(movtos);
        VMD_BD.SaveChanges();

    }

    /// <summary>
    /// Inserta en MovtosCAN con los valores del GPS
    /// </summary>
    /// <param name="_NumTramo"></param>
    /// <param name="_Origen"></param>
    /// <param name="_Destino"></param>
    /// <param name="_Via"></param>
    /// <param name="_FechaViaje"></param>
    /// <param name="_CualConductor"></param>
    /// <param name="accion"></param>
    /// <param name="_NumVuelta"></param>
    /// <param name="_NumReg"></param>
    private void GrabaConGPS(int _NumTramo, int _Origen, int _Destino, int _Via, DateTime _FechaViaje, int _CualConductor, string accion, int _NumVuelta, int _NumReg)
    {
        long ultimoId = (from x in VMD_BD.can_movtoscan
                         orderby x.id descending
                         select x.id).FirstOrDefault();

        long nuevoId = ultimoId + 1;


        can_movtoscan movtos = new can_movtoscan();

        movtos.idregion = IGlobales.Corrida.Region;
        movtos.idmarca = IGlobales.Corrida.Marca;
        movtos.idzona = IGlobales.Corrida.Zona;
        movtos.servicio = IGlobales.Corrida.Servicio;
        movtos.numerotramo = _NumTramo;
        movtos.idsecuencia = IGlobales.Corrida.IDSecuencia;
        movtos.origenviaje = _Origen;
        movtos.destinoviaje = _Destino;
        movtos.via = _Via;
        movtos.fechaviaje = _FechaViaje;
        movtos.operador = _CualConductor;
        movtos.autobus = ParametrosInicio.Autobus;
        movtos.fechahora = DateTime.Now;
        movtos.latitud = Convert.ToDouble(IGlobales.UltLat);
        movtos.longitud = Convert.ToDouble(IGlobales.UltLon);
        movtos.ns = IGlobales.UltLatNS;
        movtos.we = IGlobales.UltLonWE;
        movtos.gps_velocidad = Convert.ToSingle(IGlobales.UltVel);
        movtos.accion = accion;
        movtos.status = "A";
        movtos.FuelLevel = Convert.ToSingle(IProtocoloCAN.FuelLevel);
        movtos.TachoShaftSpeed = Convert.ToSingle(IProtocoloCAN.TachoShaftSpeed);
        movtos.KmPerLiter = Convert.ToSingle(IProtocoloCAN.KmPerLiter);
        movtos.TachoVehicleSpeed = Convert.ToSingle(IProtocoloCAN.TachoVehicleSpeed);
        movtos.LiterPerHour = IProtocoloCAN.FuelEconomyEstimationFE;
        movtos.AmbientTemp = Convert.ToSingle(IProtocoloCAN.AmbientTemp);
        movtos.EconomyAverageKMpL = Convert.ToSingle(IProtocoloCAN.EconomyAverageKMpL);
        movtos.FuelEconomyEstimationLMAF = Convert.ToSingle(IProtocoloCAN.FuelEconomyEstimationLMAF);
        movtos.FuelEconomyEstimationFE = Convert.ToSingle(IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG);

        var RealKmPerTemp = Convert.ToSingle((IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg) / (IProtocoloCAN.FuelEconomyEstimationFE - IProtocoloCAN.LtsIniDG));

        if ( RealKmPerTemp.Equals(Single.NaN) || RealKmPerTemp.Equals(Single.PositiveInfinity) || RealKmPerTemp.Equals(Single.NegativeInfinity))
        {
            movtos.RealKmPerLiterHighLevel = 0;
        }
        else
        {
            movtos.RealKmPerLiterHighLevel = RealKmPerTemp;
        }
        
        movtos.TotalDistance = IProtocoloCAN.TripDistanceEstimationVS;
        movtos.TripDistance = Convert.ToSingle(IProtocoloCAN.TripDistance);
        movtos.TripDistanceEstimationVS = Convert.ToSingle(IProtocoloCAN.TripDistanceEstimationVS - IProtocoloCAN.KmsIniDg);
        movtos.TripDistanceEstimationWVS = Convert.ToSingle(IProtocoloCAN.TripDistanceEstimationWVS);
        movtos.WheelSpeed = Convert.ToSingle(IProtocoloCAN.WheelSpeed);
        movtos.EngineForce = Convert.ToSingle(IProtocoloCAN.EngineForce);
        movtos.EngineLoad = Convert.ToSingle(IProtocoloCAN.EngineLoad);
        movtos.MassAirFlow = Convert.ToSingle(IProtocoloCAN.MassAirFlow);
        movtos.Protocolo = IProtocoloCAN.NombreProtocolo;
        movtos.IDDireccion = Convert.ToInt32(ParametrosInicio.IDDireccion);
        movtos.IDRegionOperativa = Convert.ToInt32(ParametrosInicio.IDRegionOperativa);
        movtos.NumVuelta = _NumVuelta;
        movtos.NumReg = _NumReg;
        movtos.NumVueltaServer = 0;
        movtos.NumRegServer = 0;
        movtos.id = nuevoId;

        VMD_BD.can_movtoscan.Add(movtos);
        VMD_BD.SaveChanges();

    }

    #endregion
}

