using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class clsTerminal
    {

    public bool Encontrado { get; set; } = false;
    public int IdTerminal { get; set; } = 0;
    public string DescripcionTerminal { get; set; } = "";
    public string IdZonaDescarga { get; set; } = "";
    public string DescipcionZona { get; set; } = "";
    public string RutaDescarga { get; set; } = "";


    public void LimpiaTerminal()
    {
        Encontrado = false;
        DescripcionTerminal = "";
        DescipcionZona = "";
        IdTerminal = 0;
        IdZonaDescarga = Convert.ToString(0);
        RutaDescarga = "";
    }



    }

