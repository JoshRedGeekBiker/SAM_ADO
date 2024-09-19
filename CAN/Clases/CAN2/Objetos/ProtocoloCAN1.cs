using System;
using InterfazSistema.ModelosBD;
using System.Linq;


public class ProtocoloCAN1 : IBDContext
{

    #region Propiedades
    private string NombreProtocolo { get; set; }
    private double FuelEconomyEstimationFE { get; set; }
    private double EngineLoad { get; set; }
    private double WheelSpeed { get; set; }
    private double TripDistanceEstimationVS { get; set; }
    private double TotalDistance { get; set; }
    private double EconomyAverageKMpL { get; set; }
    private double LiterPerHour { get; set; }
    private double TachoVehicleSpeed { get; set; }
    private double AmbientTemp { get; set; }
    private double FuelEconomyEstimationLMAF { get; set; }
    private double RealKmPerLiterHighLevel { get; set; }
    private double TripDistance { get; set; }
    private double TripDistanceEstimationWVS { get; set; }
    private double EngineForce { get; set; }
    private double MassAirFlow { get; set; }
    private double FuelLevel { get; set; }
    private double TachoShaftSpeed { get; set; }
    private double KmPerLiter { get; set; }
    private double rendimiento { get; set; }


    //Caso DG
    //public double LtsIniDG { get; set; } = 0;
    //public double KmsIniDg { get; set; } = 0;

    public double VAL_FR_META
    {
        get { return this.FR_META; }

        set { this.FR_META = value; }
    }

    public double VAL_FR_REAL
    {
        get { return this.FR_REAL; }
    }

    public int VAL_PARAMETRO_ID
    {
        get { return this.PARAMETRO_ID; }
    }


    #endregion

    #region Variables
    //Cliente CAN
    private ADO_CAN_Cliente.CAN_Cliente.ValoresCAN ADOCAN;

    private double KMS = 0.0;
    private double LTS = 0.0;
    private double FR_REAL = 0.0;
    private double FR_META = 0.0;
    private double Aceleracion = 0.0;
    private double RendimientoCAN = 0.0;
    public int operador = 0;
    public string accion = string.Empty;
    //Por si lo ocupamos para comunicar algo más adelante
    private int PARAMETRO_ID = 0;
    public vmdEntities VMD_BD { get; set; }
    #endregion

    #region Contructores
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public ProtocoloCAN1()
    {
        //Mandamos a inicizalizar el cliente
        Iniciar_ADOCAN();
    }

    #endregion

    #region Metodos Privados

    /// <summary>
    /// Se encarga de convertir el valor a sólo 3 decimales
    /// </summary>
    /// <param name="Valor"></param>
    /// <returns></returns>
    private double Truncar(double Valor)
    {
        int temporal = (int)(Valor * 1000);

        return temporal / 1000.0;

    }

    /// <summary>
    /// Se encarga de convertir el valor a sólo 3 decimales
    /// </summary>
    /// <param name="Valor"></param>
    /// <returns></returns>
    private double Truncar1(double Valor) { return Math.Truncate(100 * Valor) / 100; }


    /// <summary>
    /// Se encarga de convertir el valor a sólo 3 decimales
    /// </summary>
    /// <param name="Valor"></param>
    /// <returns></returns>
    private double Truncar2(double Valor)
    {
        long temporal = (long)(Valor * 1000);

        return temporal / 1000.0;

    }

    /// <summary>
    /// Se encarga de calcular el FR en tiempo REAL
    /// </summary>
    /// <returns></returns>
    private void CalcularFR()
    {
        this.FR_REAL = Truncar1(((Truncar1(TripDistanceEstimationVS) - Truncar1(KMS)) / (Truncar1(FuelEconomyEstimationFE) - Truncar1(LTS))));
        if (double.IsNaN(this.FR_REAL))
        {
            this.FR_REAL = 0;
        }
    }

    /// <summary>
    /// Se encarga de recuperar los valores del protocolo CAN
    /// </summary>
    private void RecuperarValoresProtocolo()
    {
        if (ADOCAN == null) { return; }

        try { FuelLevel = Convert.ToDouble(ADOCAN.VAL_FuelLevel); } catch { FuelLevel = 0; }

        try { TachoShaftSpeed = Convert.ToDouble(ADOCAN.VAL_RPMs); } catch { TachoShaftSpeed = 0; }

        try { KmPerLiter = Convert.ToDouble(ADOCAN.VAL_KmPerLiter); } catch { KmPerLiter = 0; }

        try { TachoVehicleSpeed = Convert.ToDouble(ADOCAN.VAL_Velocidad); } catch { TachoVehicleSpeed = 0; }

        try { LiterPerHour = Truncar(Convert.ToDouble(ADOCAN.VAL_LiterPerHour)); } catch { LiterPerHour = 0; }

        try { AmbientTemp = Convert.ToDouble(ADOCAN.VAL_AmbientTemp); } catch { AmbientTemp = 0; }

        try { EconomyAverageKMpL = Convert.ToDouble(ADOCAN.VAL_EconomyAverageKMpL); } catch { EconomyAverageKMpL = 0; }

        try { FuelEconomyEstimationLMAF = Convert.ToDouble(ADOCAN.VAL_FuelEconomyEstimationLMAF); } catch { FuelEconomyEstimationLMAF = 0; }

        try { FuelEconomyEstimationFE = Truncar2(ADOCAN.VAL_Combustible); } catch { FuelEconomyEstimationFE = 0; } // consumo de combustible

        try { RealKmPerLiterHighLevel = Convert.ToDouble(ADOCAN.VAL_Rendimiento); } catch { RealKmPerLiterHighLevel = 0; }

        try { TotalDistance = Convert.ToDouble(ADOCAN.VAL_TotalDistance); } catch { TotalDistance = 0; }

        try { TripDistance = Truncar(Convert.ToDouble(ADOCAN.VAL_TotalDistance)); } catch { TripDistance = 0; }

        try { TripDistanceEstimationVS = Truncar2(ADOCAN.VAL_Kilometros); } catch { TripDistanceEstimationVS = 0; } // km recorridos

        try { TripDistanceEstimationWVS = Convert.ToDouble(ADOCAN.VAL_TripDistanceEstimationWVS); } catch { TripDistanceEstimationWVS = 0; }

        try { WheelSpeed = Convert.ToDouble(ADOCAN.VAL_WheelSpeed); } catch { WheelSpeed = 0; }

        try { EngineForce = Convert.ToDouble(ADOCAN.VAL_EngineForce); } catch { EngineForce = 0; }

        try { EngineLoad = Convert.ToDouble(ADOCAN.VAL_EngineLoad); } catch { EngineLoad = 0; }

        try { MassAirFlow = Convert.ToDouble(ADOCAN.VAL_MassAirFlow); } catch { MassAirFlow = 0; }

        try { rendimiento = Convert.ToDouble(ADOCAN.VAL_Rendimiento); } catch { rendimiento = 0; }

        if (ADOCAN.VAL_Protocolo != null)
        {
            if (ADOCAN.VAL_Protocolo.Equals("INICIO"))
            {
                NombreProtocolo = "DG_DIDCOM_J1708VO";
            }
            else
            {
                NombreProtocolo = ADOCAN.VAL_Protocolo;
            }
        }
        else
        {
            NombreProtocolo = string.Empty;
        }


       
        //Cargamos las variables si es que tienen un valor de Cero

        if (KMS == 0) { KMS = TripDistanceEstimationVS; }

        if (LTS == 0) { LTS = FuelEconomyEstimationFE; }

        if (RendimientoCAN == 0) { RendimientoCAN = rendimiento; }

    }
    public void RecuperarDatosBD()
    {
        VMD_BD = new vmdEntities();

        var autobus = (from x in VMD_BD.can_parametrosinicio select x.Autobus).FirstOrDefault();

        var movTosCAN = (from x in VMD_BD.can_movtoscan
                         where x.autobus == autobus
                         orderby x.NumVuelta descending, x.NumReg descending, x.fechahora descending
                         select new { x.fechaviaje, x.fechahora, x.operador, x.accion, x.TotalDistance, x.LiterPerHour, x.NumVuelta }).FirstOrDefault();


        if (movTosCAN != null)
        {
            KMS = movTosCAN.TotalDistance; //Sólo en el cáso de DG serviría éste campo y está variable
            LTS = movTosCAN.LiterPerHour; //Sólo en el cáso de DG serviría éste campo y está variable
            operador = movTosCAN.operador;
            accion = movTosCAN.accion;


            //13/05/2014
            //if ((!movTosCAN.accion.Equals("VC") && KMS == 0) || (!movTosCAN.accion.Equals("VC") && LTS == 0))
            //{
            //    //Tiene que ser por evento
            //    CargaKmsLts("VA", movTosCAN.fechaviaje);
            //}
            //else
            //{
            //    //Tiene que ser por evento
            //    CargaKmsLts("T", movTosCAN.fechaviaje);
            //}
        }
        else
        {
            KMS = TripDistanceEstimationVS;
            LTS = FuelEconomyEstimationFE;
        }
    }


    /// <summary>
    /// Obtiene el primer registro de Viaje con valores diferentes de cero
    /// en caso de que la acción haya iniciado con datos iguales a cero
    /// </summary>
    /// <returns></returns>
    /// //Tabla que ocupa MovtosCAN
    //private void CargaKmsLts(string accion, DateTime fechaviaje)
    //{
    //    if (accion.Equals("VA") || accion.Equals("CM") || accion.Equals("VC"))
    //    {
    //        accion = "V";
    //    }
    //    else
    //    {
    //        accion = "T";
    //    }

    //    //Cargamos los litros que se tengan para el viaje

    //    if (LTS == 0)
    //    {
    //        var RsKml = (from x in VMD_BD.can_movtoscan
    //                     where (x.accion == accion && x.fechaviaje > fechaviaje && x.autobus == autobus && x.LiterPerHour > 0)
    //                     orderby x.NumVuelta descending, x.NumReg ascending, x.fechahora ascending
    //                     select new { x.TotalDistance, x.LiterPerHour }).FirstOrDefault();

    //        if (RsKml != null)
    //        {
    //            LTS = RsKml.LiterPerHour; //Sólo en el caso de DG serviria éste campo y ésta variable
    //        }
    //        else
    //        {
    //            //Si no hay registros en can_movtosCAN se dejan como iniciales los valores enviados por el ADORELAY
    //            LTS = FuelEconomyEstimationFE;
    //        }

    //        RsKml = null;

    //        //Cargamos los kilómetros que se tengan para el viaje
    //    }

    //    if (KMS == 0)
    //    {
    //        var RsKml = (from x in VMD_BD.can_movtoscan
    //                     where (x.accion == accion && x.fechaviaje > fechaviaje && x.autobus == autobus && x.TotalDistance > 0)
    //                     orderby x.NumVuelta descending, x.NumReg ascending, x.fechahora ascending
    //                     select new { x.TotalDistance, x.LiterPerHour }).FirstOrDefault();

    //        if (RsKml != null)
    //        {
    //            KMS = RsKml.TotalDistance; //Sólo en caso de DG serviría éste campo y variable
    //        }
    //        else
    //        {
    //            //Si no hay registros en can_movtosCAN se dejan como iniciales los valores envidados el ADORelay
    //            KMS = TripDistanceEstimationVS;
    //        }

    //        RsKml = null;

    //    }
    //}

    #endregion

    #region Metodos Publicos

    /// <summary>
    /// Manda inicializar las variables de la tarjeta CAN, será nuestro nuevo Cero.
    /// </summary>
    public void InicializarVariablesCAN()
    {
        try
        {
            KMS = TripDistanceEstimationVS;
            LTS = FuelEconomyEstimationFE;
            ADOCAN.ST_Reinicio_Variables();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de inicializar el cliente
    /// </summary>
    public void Iniciar_ADOCAN()
    {
        ADOCAN = new ADO_CAN_Cliente.CAN_Cliente.ADO_CAN_Cliente();
        ADOCAN.Iniciar_Cliente();
    }

    /// <summary>
    /// Se encarga de detener el cliente
    /// </summary>
    public void Detener_ADOCAN()
    {
        if (ADOCAN != null) { ADOCAN.Detener_Cliente(); }
    }


    /// <summary>
    /// Se encarga de procesar los datos del protocolo CAN
    /// </summary>
    public void ProcesaCAN()
    {

        RecuperarValoresProtocolo();
        CalcularFR();
    }

    #endregion
}