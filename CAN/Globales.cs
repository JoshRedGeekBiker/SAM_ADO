using System;
using System.Collections.Generic;

    public class Globales
    {

    public clsCorrida Corrida;
    public List<clsCorridaDet> Viajes;

    //Flags
    public bool ViajeAbierto = false;
    public bool EsperaFinViaje = false;
    public bool EsperaCambioManos = false;
    public bool MostrandoDatosCAN = false; //?
    public bool MostrandoDatosGPS = false;
    public bool CargandoViajeAnterior = false;
    public bool EnViajePrueba = false;
    public bool GrabandoCAN = false;
    public bool DatosCANIniciados = false;
    public bool SincronizandoCAN = false;
    public int conductor;
    public DateTime UltimoCANGrabado = DateTime.Now;
    public bool EsperaIniciaViaje;
    public bool ValidaDatosCAN = false;
    

    //Parametros
    public string ConductorPredeterminado = "COND.PREDETERMINADO";
    public double Param_CAN_MetaRendimiento = 0.0;
    public string Param_CAN_DatosViaje = "VIAJE";
    public string Param_CAN_DatosGPS = "DATOSGPS";
    public string Param_CAN_MovtosCAN = "MOVTOSCAN";
    public double Param_CAN_MetaRendimientoPrueba = 2;

    //Variables de GPS

    public string UltLat = string.Empty, UltLon = string.Empty, UltLatNS = string.Empty, UltLonWE = string.Empty;
    public double UltVel = 0;

    public Globales()
    {
        Corrida = new clsCorrida();
        Corrida.Marca = "0";
        Corrida.Region = "0";
        Corrida.Zona = "0";

        Viajes = new List<clsCorridaDet>();
    }



    /// <summary>
    /// Pone en falso las banderas
    /// </summary>
    public void ReiniciarFLagsViaje()
    {
        EsperaIniciaViaje = false;
        EsperaCambioManos = false;
        EsperaFinViaje = false;
    }

    /// <summary>
    /// Añade un viaje
    /// </summary>
    public void AddViaje()
    {
        Corrida.AddViaje();
        Viajes.Add(new clsCorridaDet());

    }

    /// <summary>
    /// Borra los viajes
    /// </summary>
    /// <returns></returns>
    public void ClearViajes()
    {
        Corrida.ClearViajes();
        Corrida.ViajeActual = 0;
        Viajes.Clear();
    }

    public void AddViajeBlanco(int NumViajes, int NumTramo, int OrigenID, int DestinoID, int ViaID ,int Conductor1, int conductor2, string OrigenDes, string DestinoDes, string ViaDes, int IDDetSecuencia, bool Confirmado)
    {
        AddViaje();

        Viajes[NumViajes].FechaHora = DateTime.Now;
        Viajes[NumViajes].NumTramo = NumTramo;
        Viajes[NumViajes].OrigenID = OrigenID;
        Viajes[NumViajes].DestinoID = DestinoID;
        Viajes[NumViajes].ViaID = ViaID;
        Viajes[NumViajes].Conductor1 = Conductor1;
        Viajes[NumViajes].Conductor2 = conductor2;
        Viajes[NumViajes].OrigenDes = OrigenDes;
        Viajes[NumViajes].DestinoDes = DestinoDes;
        Viajes[NumViajes].ViaDes = ViaDes;
        Viajes[NumViajes].IDDetSecuencia = IDDetSecuencia;
        Viajes[NumViajes].Confirmado = Confirmado;

    }
}