using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;

public class DatosRegistros
    {


    /// <summary>
    /// Constructor Principal
    /// </summary>
    public DatosRegistros()
    {

    }


    public string VerCuadrosCONDUSAT(condusatEntities CONDUSAT_BD)
    {
        string Version = string.Empty;

        Version = (from x in CONDUSAT_BD.versiones
                   where x.tabla == "cuadro"
                   select x.version).FirstOrDefault().ToString();

        if  (Version != null || Version.Equals(""))
        {
            return Version;
        }
        else
        {
            return "NA";
        }
    }
}

