using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Sistemas
{

    public bool CAN { get; set; }

    public bool VMD { get; set; }

    public bool CONDUSAT { get; set; }

    public bool SIA { get; set; }

    public bool Antivirus { get; set; }

    public bool PLAT { get; set; }

    public bool SIADLL { get; set; }

    public bool TELEMETRIA { get; set; }

    public void InicializarVariables()
    {
        CAN = false;
        VMD = false;
        CONDUSAT = false;
        SIA = false;
        Antivirus = false;
        PLAT = true;
        SIADLL = false;
        TELEMETRIA = false;
    }

}
