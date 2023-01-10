using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterfazSistema;

public class PLAT : ISistema, IBDContext
{
    #region "Propiedades"

    #endregion

    #region "Propiedades Heredadas
    public int PuertoSocket { get; set; }
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }
    public Sistema Sistema { get { return Sistema.PLAT; } }
    public string GetVersionSistema { get; }
    public vmdEntities VMD_BD { get; }
    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    #endregion

    #region "Variables"
    private String Ruta_Ejecucion;

    Process p;

    public IPEndPoint Dir;
    #endregion

    #region "Variables de evento"
    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSyncPlat;
    #endregion

    #region "Constructores"
    public PLAT()
    {

        if (Environment.Is64BitOperatingSystem)
        {
            Ruta_Ejecucion = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\SIIAB\Cliente_plataforma_movil\Cliente_PM_SIIAB_2.exe";
        }
        else
        {
            Ruta_Ejecucion = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\SIIAB\Cliente_plataforma_movil\Cliente_PM_SIIAB_2.exe";
        }

    }
    #endregion

    #region "Métodos Publicos"

    #endregion

    #region "Métodos Privados"

    #endregion

    #region "Métodos Heredados"
    public void Actualizar()
    {

    }

    public string Eventos()
    {
        throw new NotImplementedException();
    }

    public void Finalizar()
    {
        //throw new NotImplementedException();
    }

    public void Inicializar()
    {
        //PlatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Dir = new IPEndPoint(IPAddress.Parse("127.0.0.1"), PuertoSocket);
        p = new Process();
        p.StartInfo.FileName = Ruta_Ejecucion;

    }

    public bool Sincronizar()
    {
        FileInfo Plat = new FileInfo(Ruta_Ejecucion);

        if (Plat.Exists)
        {
            p.Start();

            EventoSyncPlat("Buscando actualizaciones de los sistemas...", 0);
            while (!p.HasExited)
            {
                //Esperamos
            }
            EventoSyncPlat("No hay actualizaciones para los sistemas.", 0);

            return true;
        }
        else
        {
            EventoSyncPlat("No se encuentra Aplicativo Plataforma Movil", 0);
            return false;
        }
    }

    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"

    #endregion
}


