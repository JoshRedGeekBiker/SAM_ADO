using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Response_Geocerca
{
    public bool isPuntoDoble;

    public int geocercaId { get; set; }
    public int parametroId { get; set; }
    public long sequence { get; set; }
    public string nombre { get; set; }
    public string clave { get; set; }
    public bool activo { get; set; }
    public string numeroEconomico { get; set; }
    public float latitud { get; set; }
    public float longitud { get; set; }
    public Coordenadas Geocercas { get; set; }
    public DateTime fechaCreacion { get; set; }
    public List<CoordenadasCan2> Coordenadas { get; set; }
    public List<geocercaParametros> geocercaParametros { get; set; }
    public List<POINT> points { get; set; }


}

