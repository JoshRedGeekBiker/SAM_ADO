//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InterfazSistema.ModelosBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class can_movtoscan
    {
        public string idregion { get; set; }
        public string idmarca { get; set; }
        public string idzona { get; set; }
        public int servicio { get; set; }
        public int numerotramo { get; set; }
        public int idsecuencia { get; set; }
        public int origenviaje { get; set; }
        public int destinoviaje { get; set; }
        public int via { get; set; }
        public System.DateTime fechaviaje { get; set; }
        public int operador { get; set; }
        public string autobus { get; set; }
        public System.DateTime fechahora { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string ns { get; set; }
        public string we { get; set; }
        public float gps_velocidad { get; set; }
        public string accion { get; set; }
        public string status { get; set; }
        public float FuelLevel { get; set; }
        public float TachoShaftSpeed { get; set; }
        public float KmPerLiter { get; set; }
        public float TachoVehicleSpeed { get; set; }
        public double LiterPerHour { get; set; }
        public float AmbientTemp { get; set; }
        public float EconomyAverageKMpL { get; set; }
        public float FuelEconomyEstimationLMAF { get; set; }
        public float FuelEconomyEstimationFE { get; set; }
        public float RealKmPerLiterHighLevel { get; set; }
        public double TotalDistance { get; set; }
        public float TripDistance { get; set; }
        public float TripDistanceEstimationVS { get; set; }
        public float TripDistanceEstimationWVS { get; set; }
        public float WheelSpeed { get; set; }
        public float EngineForce { get; set; }
        public float EngineLoad { get; set; }
        public float MassAirFlow { get; set; }
        public string Protocolo { get; set; }
        public int IDDireccion { get; set; }
        public int IDRegionOperativa { get; set; }
        public int NumVuelta { get; set; }
        public int NumReg { get; set; }
        public int NumVueltaServer { get; set; }
        public int NumRegServer { get; set; }
        public long id { get; set; }
    }
}