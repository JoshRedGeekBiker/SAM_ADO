public class GPSData
{
    //Propiedades

    public string Latitud { get; set; }
    public string LatitudNS { get; set; }
    public string Longitud { get; set; }
    public string LongitudWE { get; set; }
    public string Altitud { get; set; }
    public string Satelites { get; set; }
    public double Velocidad { get; set; }
    public string Precision { get; set; }
    public string Sentido { get; set; }
    public int EstadoGPS { get; set; }

    public GPSData()
    {
        Latitud = "0.0";
        LatitudNS = "N";
        Longitud = "0.0";
        LongitudWE = "W";
        Altitud = "0.0";
        Satelites = "0";
        Velocidad = 0;
        Precision = "0.0";
        Sentido = "D";
        EstadoGPS = -1;
    }
}

