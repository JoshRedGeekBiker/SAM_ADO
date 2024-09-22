using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CoordenadasCan2
{
    public CoordenadasCan2() { }
    public int coordenadas_id { get; set; }    // Clave primaria
    public int geocercaId { get; set; }    // Clave primaria
    public int sequence { get; set; }
    public bool active { get; set; }            // Se usa byte en lugar de tinyint
    public float latitud { get; set; }
    public float latitudCan { get; set; }
    public float longitud { get; set; }
    public float longitudCan { get; set; }
}

