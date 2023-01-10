using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsCorrida
{

    public int ViajeActual { get; set; }


    private int vNumViajes = 0;
    public int NumViajes {
        get {
                return vNumViajes;
            }
    }

    public string Region { get; set; }

    public string Marca { get; set; }

    public string Zona { get; set; }

    public int Servicio { get; set; }

    public int IDSecuencia { get; set; }

    public int VersionSecuencia {get; set;}

    public int ConductorActualID { get; set; }

    public string ConductorActualNom { get; set; }

    public bool ConfimadoTotal { get; set; }

    public int PobActual { get; set; }

    public bool SecuenciaAbierta { get; set; }

    public string Autobus { get; set; }

    public void ClearViajes()
    {
        vNumViajes = 0;
    }

    public void AddViaje()
    {
        vNumViajes = vNumViajes + 1;
    }

    public void DelViajes( int Posicion, clsCorridaDet[] Viajes)
    {
        if (vNumViajes <= 0) return;

        if (Posicion < vNumViajes)
        {
            for (int i = 0; i == vNumViajes; i++)
            {
                Viajes[i] = Viajes[i + 1];
            }

            Viajes[vNumViajes] = null;
            vNumViajes = vNumViajes - 1;

        }
    }
 }