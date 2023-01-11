using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;

public class ProtocoloCAN : IBDContext, IMessage
{


    public vmdEntities VMD_BD { get; }
    public clsMessage AdvMsg { get; set; }

    //Parametros
    private can_parametrosinicio ParametrosInicio;

    //Propiedades
    public string NombreProtocolo { get; set; }
    public double FuelEconomyEstimationFE { get; set; }
    public double EngineLoad { get; set; }
    public double WheelSpeed { get; set; }
    public double TripDistanceEstimationVS { get; set; }
    public double TotalDistance { get; set; }
    public double EconomyAverageKMpL { get; set; }
    public double LiterPerHour { get; set; }
    public double TachoVehicleSpeed { get; set; }
    public double AmbientTemp { get; set; }
    public double FuelEconomyEstimationLMAF { get; set; }
    public double RealKmPerLiterHighLevel { get; set; }
    public double TripDistance { get; set; }
    public double TripDistanceEstimationWVS { get; set; }
    public double EngineForce { get; set; }
    public double MassAirFlow { get; set; }
    public double FuelLevel { get; set; }
    public double TachoShaftSpeed { get; set; }
    public double KmPerLiter { get; set; }
    public double rendimiento { get; set; }


    //Caso DG
    public double LtsIniDG { get; set; } = 0;
    public double KmsIniDg { get; set; } = 0;

    //Cliente CAN
    private ADO_CAN_Cliente.CAN_Cliente.ValoresCAN ADOCAN;


    public ProtocoloCAN()
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();
    }

    /// <summary>
    /// Inicia El socket de CAN
    /// </summary>
    public void Iniciar_ADOCAN()
    {
        ADOCAN = new ADO_CAN_Cliente.CAN_Cliente.ADO_CAN_Cliente();
        ADOCAN.Iniciar_Cliente();
        //Lanza evento al form principal activa un timer
    }

    /// <summary>
    /// Detiene el socket de CAN
    /// </summary>
    public void Detener_ADOCAN()
    {
        ADOCAN.Detener_Cliente();
        //lanza evento al form principal desactiva el timer
    }

    /// <summary>
    /// Manda inicializar las variables de la tarjeta CAN, en especial caso ST
    /// </summary>
    public void InicializarVariablesCAN()
    {
        try
        {
            ADOCAN.ST_Reinicio_Variables();
        }
        catch
        {
            //MuestraError("InicializaVariablesCAN");
        }
    }

    /// <summary>
    /// Se encarga de configurar el servidor de ADOCAN
    /// de acuerdo al tipo de tarjeta
    /// </summary>
    /// <param name="Tarjeta"></param>
    public void Configura_ADOCAN(string Tarjeta)
    {
        try
        {
            ADOCAN.CONFIGURACION_Cargar_Prederminado(Tarjeta);
        }
        catch
        {

        }
    }

    /// <summary>
    /// Obtiene el primer registro de Viaje con valores diferentes de cero
    /// en caso de que la acción haya iniciado con datos iguales a cero
    /// </summary>
    /// <returns></returns>
    /// //Tabla que ocupa MovtosCAN
    public void CargaKmsLts(string accion, DateTime fechaviaje)
    {
        if (accion.Equals("VA") || accion.Equals("CM") || accion.Equals("VC"))
        {
            accion = "V";
        }
        else
        {
            accion = "T";
        }

        //Cargamos los litros que se tengan para el viaje

        if (LtsIniDG == 0)
        {
            var RsKml = (from x in VMD_BD.can_movtoscan
                         where (x.accion == accion && x.fechaviaje > fechaviaje && x.autobus == ParametrosInicio.Autobus && x.LiterPerHour > 0)
                         orderby x.NumVuelta descending, x.NumReg ascending, x.fechahora ascending
                         select new { x.TotalDistance, x.LiterPerHour }).FirstOrDefault();

            if (RsKml != null)
            {
                LtsIniDG = RsKml.LiterPerHour; //Sólo en el caso de DG serviria éste campo y ésta variable
            }
            else
            {
                //Si no hay registros en can_movtosCAN se dejan como iniciales los valores enviados por el ADORELAY
                LtsIniDG = FuelEconomyEstimationFE;
            }

            RsKml = null;

            //Cargamos los kilómetros que se tengan para el viaje
        }

        if (KmsIniDg == 0)
        {
            var RsKml = (from x in VMD_BD.can_movtoscan
                         where (x.accion == accion && x.fechaviaje > fechaviaje && x.autobus == ParametrosInicio.Autobus && x.TotalDistance > 0)
                         orderby x.NumVuelta descending, x.NumReg ascending, x.fechahora ascending
                         select new { x.TotalDistance, x.LiterPerHour }).FirstOrDefault();

            if (RsKml != null)
            {
                KmsIniDg = RsKml.TotalDistance; //Sólo en caso de DG serviría éste campo y variable
            }
            else
            {
                //Si no hay registros en can_movtosCAN se dejan como iniciales los valores envidados el ADORelay
                KmsIniDg = TripDistanceEstimationVS;
            }

            RsKml = null;

        }
    }

    /// <summary>
    /// Se encarga de procesar los datos del protocolo CAN
    /// </summary>
    /// <param name="Param_ADOCAN"></param>
    /// <param name="AdvMsg"></param>
    public void ProcesaCAN(bool Param_ADOCAN, ref Globales iGlobales, clsMessage AdvMsg = null)
    {
        try
        {
            if (Param_ADOCAN)
            {
                try { FuelLevel = Convert.ToDouble(ADOCAN.VAL_FuelLevel); } catch { FuelLevel = 0; }

                try { TachoShaftSpeed = Convert.ToDouble(ADOCAN.VAL_RPMs); } catch { TachoShaftSpeed = 0; }

                try { KmPerLiter = Convert.ToDouble(ADOCAN.VAL_KmPerLiter); } catch { KmPerLiter = 0; }

                try { TachoVehicleSpeed = Convert.ToDouble(ADOCAN.VAL_Velocidad); } catch { TachoVehicleSpeed = 0; }

                try { LiterPerHour = Truncar(Convert.ToDouble(ADOCAN.VAL_LiterPerHour)); } catch { LiterPerHour = 0; }

                try { AmbientTemp = Convert.ToDouble(ADOCAN.VAL_AmbientTemp); } catch { AmbientTemp = 0; }

                try { EconomyAverageKMpL = Convert.ToDouble(ADOCAN.VAL_EconomyAverageKMpL); } catch { EconomyAverageKMpL = 0; }

                try { FuelEconomyEstimationLMAF = Convert.ToDouble(ADOCAN.VAL_FuelEconomyEstimationLMAF); } catch { FuelEconomyEstimationLMAF = 0; }

                try { FuelEconomyEstimationFE = Truncar(ADOCAN.VAL_Combustible); } catch { FuelEconomyEstimationFE = 0; }

                try { RealKmPerLiterHighLevel = Convert.ToDouble(ADOCAN.VAL_Rendimiento); } catch { RealKmPerLiterHighLevel = 0; }

                try { TotalDistance = Convert.ToDouble(ADOCAN.VAL_TotalDistance); } catch { TotalDistance = 0; }

                try { TripDistance = Truncar(Convert.ToDouble(ADOCAN.VAL_TotalDistance)); } catch { TripDistance = 0; }

                try { TripDistanceEstimationVS = Truncar(ADOCAN.VAL_Kilometros); } catch { TripDistanceEstimationVS = 0; }

                try { TripDistanceEstimationWVS = Convert.ToDouble(ADOCAN.VAL_TripDistanceEstimationWVS); } catch { TripDistanceEstimationWVS = 0; }

                try { WheelSpeed = Convert.ToDouble(ADOCAN.VAL_WheelSpeed); } catch { WheelSpeed = 0; }

                try { EngineForce = Convert.ToDouble(ADOCAN.VAL_EngineForce); } catch { EngineForce = 0; }

                try { EngineLoad = Convert.ToDouble(ADOCAN.VAL_EngineLoad); } catch { EngineLoad = 0; }

                try { MassAirFlow = Convert.ToDouble(ADOCAN.VAL_MassAirFlow); } catch { MassAirFlow = 0; }

                if (ADOCAN.VAL_Protocolo != null)
                {
                    if (ADOCAN.VAL_Protocolo.Equals("INICIO"))
                    {
                        NombreProtocolo = string.Empty;

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
            }
            else
            {

                FuelLevel = Convert.ToDouble(AdvMsg.GetValuebyKey("FuelLevel"));

                TachoShaftSpeed = Convert.ToDouble(AdvMsg.GetValuebyKey("TachoShaftSpeed"));

                KmPerLiter = Convert.ToDouble(AdvMsg.GetValuebyKey("kmPerLiter"));

                TachoVehicleSpeed = Convert.ToDouble(AdvMsg.GetValuebyKey("TachoVehicleSpeed"));

                LiterPerHour = Math.Truncate((Convert.ToDouble(AdvMsg.GetValuebyKey("LiterPerHour")) * 1000) / 1000);

                AmbientTemp = Convert.ToDouble(AdvMsg.GetValuebyKey("AmbientTemp"));

                EconomyAverageKMpL = Convert.ToDouble(AdvMsg.GetValuebyKey("EconomyAverageKmpl"));

                FuelEconomyEstimationLMAF = Convert.ToDouble(AdvMsg.GetValuebyKey("FuelEconomyEstimationLMAF"));

                FuelEconomyEstimationFE = Math.Truncate((Convert.ToDouble(AdvMsg.GetValuebyKey("FuelEconomyEstimationFE")) * 1000) / 1000);

                RealKmPerLiterHighLevel = Convert.ToDouble(AdvMsg.GetValuebyKey("RealKmPerLiterHighLevel"));

                TotalDistance = Convert.ToDouble(AdvMsg.GetValuebyKey("TotalDistance"));

                TripDistance = Math.Truncate(((Convert.ToDouble(AdvMsg.GetValuebyKey("TripDistance"))) * 1000) / 1000);

                TripDistanceEstimationVS = Math.Truncate((Convert.ToDouble(AdvMsg.GetValuebyKey("TripDistanceEstimationVS")) * 1000) / 1000);

                TripDistanceEstimationWVS = Convert.ToDouble(AdvMsg.GetValuebyKey("TripDistanceEstimacionWVS"));

                WheelSpeed = Convert.ToDouble(AdvMsg.GetValuebyKey("WheelSpeed"));

                EngineForce = Convert.ToDouble(AdvMsg.GetValuebyKey("EngineForce"));

                EngineLoad = Convert.ToDouble(AdvMsg.GetValuebyKey("EngineLoad"));

                MassAirFlow = Convert.ToDouble(AdvMsg.GetValuebyKey("MassAirFlow"));

                NombreProtocolo = AdvMsg.GetValuebyKey("Protocolo").ToString();
            }


            //Se quitará los segundos de muestra
            if (!iGlobales.SincronizandoCAN
               //&& ((DateTime.Now - iGlobales.UltimoCANGrabado).TotalSeconds >= ParametrosInicio.SegsMuestra)
               && (!iGlobales.EsperaCambioManos)
               && (!iGlobales.EsperaFinViaje)
               && (!iGlobales.EsperaIniciaViaje))
            {

                if (!iGlobales.MostrandoDatosCAN)
                {
                    //MuestraDatosCAN EVENTO
                }

                //iGlobales.UltimoCANGrabado = DateTime.Now;

                if (NombreProtocolo != string.Empty && NombreProtocolo.Substring(0, 2).Equals("DG"))
                {
                    if ((FuelEconomyEstimationFE - LtsIniDG) > 0)
                    {
                        rendimiento = ((TripDistanceEstimationVS - KmsIniDg) / (FuelEconomyEstimationFE - LtsIniDG));
                    }
                    else
                    {
                        rendimiento = 0;
                    }
                }
                else
                {
                    rendimiento = RealKmPerLiterHighLevel;
                }


                //Validar si son necesarias las siguientes lineas ****************

                //double rendimientoCompar = Convert.ToDouble(Math.Truncate(((rendimiento * 100) / 100)));
                //if (iGlobales.EnViajePrueba)
                //{
                //    if (rendimientoCompar < iGlobales.Param_CAN_MetaRendimientoPrueba)
                //    {
                //        //ApagarExternoCAN();
                //        //EncenderExternoCAN();
                //    }
                //    else if(rendimientoCompar >= iGlobales.Param_CAN_MetaRendimientoPrueba)
                //    {
                //        //ApagarExternoCAN();
                //        //EncerderExternoCAN();
                //    }
                //}
                //else
                //{
                //    if (rendimientoCompar < iGlobales.Param_CAN_MetaRendimiento)
                //    {
                //        //ApagarExternoCAN();
                //        //EncerderExternoCAN();

                //    }
                //    else if (rendimientoCompar >= iGlobales.Param_CAN_MetaRendimiento)
                //    {

                //        //ApagarExternoCAN();
                //        //EncerderExternoCAN();
                //    }

                //}
            }
            //Se encarga de prender el led amarillo, azul, ambar que indica que se está acercando a la velocidad máxima permitida

            if (TachoVehicleSpeed >= ParametrosInicio.MaxVel)
            {//Validamos que la pantalla touch esté activa

                if (!true)//PantallaTouch bool
                {
                    //Encernder externo
                }
                else
                {
                    //15/05/2013
                    //Presentamos la velocidad real en el protector de pantalla
                    //frmprotectorPantalla.lblReal.Caption = Format(CInt(TachoVehicleSpeed), "0")

                    //Validamos que el semáforo sólo se active cuando Condusat no se encuentre encendido

                    if (!(bool)ParametrosInicio.CONDUSAT)
                    {
                        //frmSistemas.imgESemaforo.left = 9840;*******************
                        /////frmSistemas.imgESemaforo.Picture = Nothing*******************
                        //frmSistemas.ImgESemaforo.Image = "imagenes/letAMARILLOpeke.gif")*******************
                        //Sleep3(1000);*******************
                        //frmSistemas.imgESemeaforo.Visible = false;*******************
                        //frmSistemas.imgESemaforo.left = 5640;*******************
                    }
                    else
                    {
                        //17/04/2013
                        EnviarVelCANaCondusat(Param_ADOCAN, iGlobales);
                        //DatosCondusat();
                    }
                }

            }
            else
            {//Validamos que la pantalla touch esté activa
                if (!true)//PantallaTouch bool
                {
                    //ApagarExternoCAN(Ledamarillo);
                }
                else
                {
                    //15/05/2013
                    //Presentamos la velocidad real en el protector de pantalla
                    //frmprotectorPantalla.lblReal.Caption = Format(CInt(TachoVehicleSpeed), "0")

                    //Validamos que el semáforo sólo se active cuando Condusat no se encuentre encendido

                    if (!(bool)ParametrosInicio.CONDUSAT)
                    {
                        //frmSistemas.imgESemaforo.left = 9840;*******************
                        /////frmSistemas.imgESemaforo.Picture = Nothing*******************
                        //frmSistemas.ImgESemaforo.Image = "imagenes/LetVerdepeke.gif")*******************
                        //Sleep3(1000);*******************
                        //frmSistemas.imgESemeaforo.Visible = false;*******************
                        //frmSistemas.imgESemaforo.left = 5640;*******************
                    }
                    else
                    {
                        //17/04/2013
                        EnviarVelCANaCondusat(Param_ADOCAN, iGlobales);
                        //DatosCondusat();*******************
                    }
                }
            }

            //24/10/2013
            //Cargamos las variables de DG si es que éstas no han sido inicializadas

            if (KmsIniDg == 0)
            {
                KmsIniDg = TripDistanceEstimationVS;
            }

            if (LtsIniDG == 0)
            {
                LtsIniDG = FuelEconomyEstimationFE;
            }
        }
        catch (Exception ex)
        {

        }




    }


    /// <summary>
    /// Enviamos la velocidad hacia CONDUSAT
    /// </summary>
    public void EnviarVelCANaCondusat(bool Param_ADOCAN, Globales iGlobales)
    {
        try
        {
            if (Param_ADOCAN)
            {
                return;
            }

            if ((bool)ParametrosInicio.CAN)
            {//17/04/2013
             //Le enviamos la velocidad CAN a Condusat

                if (iGlobales.ValidaDatosCAN)
                {
                    //frmSistemas.lblVreal.Text = TachoVehicleSpeed + " Km/h";

                    //Datos para el botón
                    //frmSistemas.lblBVelocidad.Caption = TachoVehicleSpeed + "/" + "VelocidadMáxima

                    //Enviar evento de actualización de CONDUSAT ************************************
                    //if (fmrPrincipal.SerialCondusat.CtlState = MSWinsocklib.StateConstats.sckConnected)
                    //{
                    //    fmrPrincipal.SerialCondusat.SendData(TachoVehicleSpeed);
                    //    Sleep2(300);
                    //}
                }
                else
                {
                    //frmSistemas.lblVReal.Text = Math.Round(iGlobales.UltVel, 2);
                    //if (fmrPrincipal.SerialCondusat.CtlState = MSWinsocklib.StateConstats.sckConnected)
                    //{
                    //    fmrPrincipal.SerialCondusat.SendData(Math.Round( iGlobales.UltVel, 2);
                    //    Sleep2(300);
                    //}
                }
            }
        }
        catch
        {
            //MsgBox "Error información";
        }
    }

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
    /// Se encarga de verificar la existencia de datos del protocolo de CAN
    /// </summary>
    /// <returns></returns>
    public bool ValidaDatos()
    {
        bool Resultado = true;

        try
        {
            if (ADOCAN.VAL_Protocolo != null)
            {
                if (ADOCAN.VAL_Protocolo.Contains("DG"))
                {
                    if (TachoShaftSpeed == 0 || FuelEconomyEstimationFE == 0 || TripDistanceEstimationVS == 0)
                    {
                        Resultado = false;
                    }
                }
                else
                {
                    if (TachoShaftSpeed == 0)
                    {
                        Resultado = false;
                    }
                }
            }
            else
            {
                Resultado = false;
            }
        }
        catch
        {
            Resultado = false;
        }

        return Resultado;
    }

    /// <summary>
    /// Se ponen para saber que es lo que hacian
    /// </summary>
    /// <param name="posicion"></param>
    #region "Ya no se ocupan"

    public void ApagarExternoCAN(int posicion)
    {
        //If fmrPrincipal.SerialPant.PortOpen Then

        //    fmrPrincipal.SerialPant.Output = Chr(254) & Chr(86) & Chr(Posicion)

        //End If
    }

    public void EcenderExternoCAN(int posicion)
    {
        //If fmrPrincipal.SerialPant.PortOpen Then

        //    fmrPrincipal.SerialPant.Output = Chr(254) & Chr(87) & Chr(Posicion)

        //End If
    }

    #endregion


}

