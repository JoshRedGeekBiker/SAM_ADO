using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class POINT
{
    #region "Propiedades"
    public float Latitud { get; set; }
    public float Longitud { get; set; }
    public int Secuencia { get; set; }
    public Response_spot item { get; set; }
    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor basico
    /// </summary>
    public POINT()
    {
        this.Latitud = 0.0f;
        this.Longitud = 0.0f;
    }

    public POINT(String latitud, String longitud)
    {
        this.Latitud = (float)Convert.ToDouble(latitud);
        this.Longitud = (float)Convert.ToDouble(longitud);
    }

    /// <summary>
    /// Constructor con una coordenada
    /// </summary>
    /// <param name="latitud"></param>
    /// <param name="longitud"></param>
    public POINT(float latitud, float longitud, int secuencia)
    {
        this.Latitud = latitud;
        this.Longitud = longitud;
        this.Secuencia = secuencia;
    }

    public POINT(float latitud, float longitud, int secuencia, Response_spot spot)
    {
        this.Latitud = latitud;
        this.Longitud = longitud;
        this.Secuencia = secuencia;
        this.item = spot;
    }

    public POINT(float latitud, float longitud)
    {
        this.Latitud = latitud;
        this.Longitud = longitud;
    }
    #endregion
}
