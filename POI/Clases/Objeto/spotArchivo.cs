using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class spotArchivo
{
    public int spotArchivoId { get; set; }
    public String nombre { get; set; }
    public String url { get; set; }
    public Boolean activo { get; set; }
    public DateTime fechaVigenciaInicio { get; set; }
    public DateTime fechaVigenciaFin { get; set; }
    public int tipo_spot { get; set; } = 0;
    public Tipo tipo { get; set; }
}
