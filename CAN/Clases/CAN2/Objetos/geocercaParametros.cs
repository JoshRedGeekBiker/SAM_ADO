using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class geocercaParametros
{

    public  geocercaParametros(){}

    public int ParametroId { get; set; }
    public int geocercaId { get; set; }
    public string NombreParametro { get; set; }
    public double ValorParametro { get; set; }
    public string ValorReal { get; set; }
    public double MargenParametro { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaVigenciaInicio { get; set; }
    public DateTime FechaVigenciaFin { get; set; }
    public int orientacionInicial { get; set; }
    public int orientacionFinal { get; set; }
    public Boolean in_poligone { get; set; } = false;


}

