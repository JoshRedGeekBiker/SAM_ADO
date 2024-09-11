using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Response_spot
{
    public int spotListaId { get; set; }
    public Boolean in_poligone { get; set; } = false;
    public String nombre { get; set; }
    public int orientacionInicial { get; set; }
    public int orientacionFinal { get; set; }
    public Boolean activo { get; set; }
    public DateTime fechaCreacion { get; set; }
    public DateTime fechaVigenciaInicio { get; set; }
    public DateTime fechaVigenciaFin { get; set; }
    public Parada parada { get; set; }
    public List<spotListaDetalles> spotListaDetalles { get; set; }
    public List<POINT> points { get; set; }
    public String longitudes { get; set; } = "";
    public String latitudes { get; set; } = "";
    public int orientacionInicial2 { get; set; }
    public int orientacionFinal2 { get; set; }
    public List<POINT> Points { get; set; }
    public Boolean isPuntoDoble { get; set; } = false;
    public Response_spot punto2 { get; set; }

}
