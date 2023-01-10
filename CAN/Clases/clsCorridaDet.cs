using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsCorridaDet
{

    public int IDDetSecuencia { get; set; }

    public int Conductor1 { get; set; }

    public int Conductor2 { get; set; }

    public int OrigenID { get; set; }

    public string OrigenDes { get; set; }

    public int DestinoID {get; set;}

    public string DestinoDes { get; set; }

    public string ViaDes { get; set; }

    public DateTime FechaHora { get; set; }

    public int NumTramo { get; set; }

    public int ViaID { get; set; }

    public bool Confirmado { get; set; }

}