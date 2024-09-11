using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using InterfazSistema.ModelosBD;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Windows.Forms;
using InterfazSistema.WSCAN;
using RegistroWifi;
using Microsoft.Win32;
using System.IO;

public class SyncSAM : ICloud
    {


    public int comTimeOut { get; set; } = 1200;
    public int conexionTimeOut { get; set; } = 180;

    public int VersionServer { get; set; } = 10; //Ponemos la versión por default, cambiará si el server devuelve otro valor
    public string strVersionesSistemas { get; set; } = string.Empty;
    public string CadenaConexion { get; set; } = string.Empty;
    public string CualServidorCAN { get; set; } = string.Empty;
    public string IpActual { get; set; } = string.Empty;
    public double AnilloRedCAN { get; set; } = 0;
    public string ZonaDescarga { get; set; } = string.Empty;
    public string FormaIdentiZona { get; set; } = string.Empty;

    //Powered ByRED 18ENE2022
    public string PuertoAPP { get; set; } = string.Empty; //Puerto aplicativo para la descarga de información

    public List<string> log = new List<string>();

    public bool ServerAlterno { get; set; } = false;

    public bool ServerNube { get; set; } = false;

    private DateTime FechaByRed = DateTime.ParseExact("1993-10-30 00:15", "yyyy-MM-dd HH:mm", null); //FechaRED =)

    //Conexión a SQL
    public SqlConnection SqlCon;

    //Conexión a NUBE
    public ServiceClient servicioWCF { get; set; }

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public SyncSAM()
    {
        
    }

    /// <summary>
    /// Se encarga de probar la conexión y establecerla
    /// para poder sincronizar
    /// </summary>
    /// <returns></returns>
    public bool PruebaSQLServer()
    {
        //string cadena = @"PROVIDER=SQLOLEDB;Data source=DESKTOP-HCC9M18\SIIABCAN;Initial Catalog=SIIABCAN;user id=sa;pwd=satrackitacccit";

        string[] str = this.CualServidorCAN.Split(';');


        string CadenaRecortada = str[1] + ";" + str [2] + ";" + str[3] + ";" + str[4];

        //Para Producción
        SqlCon = new SqlConnection(CadenaRecortada);



        ////Para pruebas Locales
        //SqlCon = new SqlConnection(@"Data source=DESKTOP-HCC9M18\SIIABCAN;Initial Catalog=SIIABCAN;user id=sa;pwd=satrackitacccit");

        if (ParSAM.ModoDeveloper)
        {
            //Para pruebas hacia server migracion oriente 2021
            //SqlCon = new SqlConnection(@"Data source=10.15.44.13;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //CQ Caribe
            //SqlCon = new SqlConnection(@"Data source=128.3.57.10;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //SDL
            //SqlCon = new SqlConnection(@"Data source=128.3.76.135;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //CUAUTLA
            //SqlCon = new SqlConnection(@"Data source=128.3.91.77;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //Poza RICA  
            SqlCon = new SqlConnection(@"Data source=128.3.138.134;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //Tuxtla
            //SqlCon = new SqlConnection(@"Data source=128.3.106.29;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

            //Taller Mérida
            //SqlCon = new SqlConnection(@"Data source=128.3.40.9;Initial Catalog=SIIABCAN;user id=siiabsa;pwd=satrackitacccit");

        }

        try
        {
            SqlCon.Open();
            return true;
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
            return false;
        }

    }

    /// <summary>
    /// Se encarga de configurar y probar el servicioWCF
    /// ***Tarjeta de Circulación Vencida***
    /// </summary>
    /// <returns></returns>
    public bool PruebaCloud(string Puerto)
    {
        try
        {
            //Creo el cliente con valores por default
            servicioWCF = new ServiceClient();

            //asigno la dirección a donde está alojado el WS
            if (ParSAM.ModoDeveloper)
            {//para pruebas en servidor de migración y no se me vaya a chispotear
                servicioWCF.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://canmxdes.ado.net:80/wscan/Service.svc");
                servicioWCF.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://canmxdes.ado.net:80/wscan/Service.svc");
            }
            else
            {
                servicioWCF.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://" + this.CualServidorCAN + ":" + Puerto + "/wscan/Service.svc");
            }

            //Hago una prueba, consiste en hacer una consulta hacia la base de datos, para validar proceso
            return servicioWCF.Prueba() != "" ? true : false;
        }
        catch(Exception ex)
        {
            var error = ex.ToString();

            return false;
        }
    }


    /// <summary>
    /// Powered ByRED 18ENE2022
    /// Se encarga de configurar y probar el servicioWCF
    /// El puerto lo toma del método CualServidorXIP
    /// </summary>
    /// <returns></returns>
    public bool PruebaCloud()
    {
        try
        {
            //Creo el cliente con valores por default
            servicioWCF = new ServiceClient();

            //asigno la dirección a donde está alojado el WS
            if (ParSAM.ModoDeveloper)
            {//para pruebas en servidor de migración y no se me vaya a chispotear
                servicioWCF.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://alt_mxote.ado.net:80/wscan/Service.svc");
            }
            else
            {
                servicioWCF.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://" + this.CualServidorCAN + ":" + this.PuertoAPP + "/wscan/Service.svc");
            }

            //Hago una prueba, consiste en hacer una consulta hacia la base de datos, para validar proceso
            return servicioWCF.Prueba() != "" ? true : false;
        }
        catch (Exception ex)
        {
            var error = ex.ToString();

            return false;
        }
    }

    /// <summary>
    /// Se encarga de terminar la conexión (si la hubiera)
    /// al término de la Sincronización
    /// </summary>
    public void CerrarConexión()
    {
        try
        {
            SqlCon.Close();
            SqlCon.Dispose();
        }
        catch
        { 
        }
    }

    /// <summary>
    /// Obtiene la versión del servidor para saber cómo enviar la información
    /// 
    /// IMPORTANTE: se deja fuera del juego, no es necesario saber Hacia qué versión de Server ando sincronizando...
    /// </summary>
    /// <returns></returns>
    public void VersionSQL()
    {
        string strQuery = "SELECT  CAST(SERVERPROPERTY('productversion') AS VARCHAR) as Version";
        try
        {
            using (SqlCommand com = new SqlCommand(strQuery, this.SqlCon))
            {
                    com.CommandType = CommandType.Text;
                    com.CommandTimeout = this.comTimeOut;

                    //this.SqlCon.Open();

                    using (SqlDataReader sqlDr = com.ExecuteReader())
                    {
                        string _version;
                        if (sqlDr.Read())
                        {
                            _version = Convert.ToString(sqlDr["Version"]);

                             VersionServer = Convert.ToInt32(_version.Split('.').First());
                            
                        }
                    }

                    //this.SqlCon.Close();
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de verificar la existencia del 
    /// archivo Log de Sincronización y borrarlo
    /// </summary>
    public void BorrarLogSync(string RutaEjecucion)
    {
        var path = RutaEjecucion + "/LogSync.txt";

        if (System.IO.File.Exists(path))
        {
            try
            {
                System.IO.File.Delete(path);
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// Recibe el mensaje que queremos que vaya en el Log
    /// </summary>
    /// <param name="evento"></param>
    public void AgregarLogSync(string evento)
    {
        log.Add(DateTime.Now.ToString() + " / " + evento);
    }

    /// <summary>
    /// Se encarga de Guardar el Log
    /// </summary>
    /// <param name="RutaEjecucion"></param>
    public void GuardarLogSync(string RutaEjecucion)
    {
        try
        {
            string path = RutaEjecucion + "/LogSync.txt";

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                foreach (string linea in log)
                {
                    writer.WriteLine(linea);
                }

                log.Clear();
            }

            CrearAccesoDirectoLog(path);
        }
        catch
        {

        }
    }

    /// <summary>
    /// se encarga de crear un acceso directo
    /// para el Log
    /// </summary>
    /// <param name="RutaLog"></param>
    private void CrearAccesoDirectoLog(string RutaLog)
    {
        object shDesktop = (object)"Desktop";
        WshShell shell = new WshShell();
        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\LogSincronizaciónSAM.lnk";
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
        shortcut.Description = "Acceso Directo para ver el Log de SAM";
        shortcut.TargetPath = RutaLog;
        shortcut.Save();
    }


    /// <summary>
    /// Se encarga de activar y crear el perfil Wifi adecuado
    /// dependiendo de para qué se necesite: Sincronización o Abordaje
    /// </summary>
    public bool Wifi(bool habilitar, string device, bool Transferencia, string numBus)
    {
        //Si estamos en Modo DIOS, salimos de la rutina
        if (ParSAM.ModoDeveloper) { return true; }

        Process wifi = new Process();
        wifi.StartInfo.UseShellExecute = false;
        wifi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        wifi.StartInfo.RedirectStandardOutput = true;
        wifi.StartInfo.CreateNoWindow = true;

        string Rutadevcon;

        if (Environment.Is64BitOperatingSystem)
        {
            Rutadevcon = AppDomain.CurrentDomain.BaseDirectory + @"Devcon\64\devcon.exe";
        }
        else
        {
            Rutadevcon = AppDomain.CurrentDomain.BaseDirectory + @"Devcon\32\devcon.exe";
        }


        if (System.IO.File.Exists(Rutadevcon))
        {
            var perfilWiFi = new Perfil();
            try
            {
                wifi.StartInfo.FileName = Rutadevcon;
                if (habilitar)
                {
                    //Esperamos unos segundos, para que se habilite la tarjeta y se conecte a la red wifi (Obtenga IP)

                    wifi.StartInfo.Arguments = @"enable " + device;
                    wifi.Start();

                    //Esperamos a que encienda bien el dispositivo
                    Thread.Sleep(10000);

                    if (Transferencia)
                    {
                        Thread.Sleep(5000);
                        perfilWiFi.CrearLlaveSAM(numBus);
                    }
                    else
                    {
                        perfilWiFi.InsertarSIIABCANSAM();
                    }

                    //Esperamos a que trate de conectar a la nueva red
                    Thread.Sleep(5000);

                    return true;
                }
                else
                {
                    perfilWiFi.BorrarWiFiSAM();
                    Thread.Sleep(2500);
                    wifi.StartInfo.Arguments = @"disable " + device;
                    wifi.Start();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Para desactivar el adaptador de Ethernet (SIA, TELEMATICS)
    /// </summary>
    /// <returns></returns>
    public bool Ethernet(bool habilitar, string device)
    {

        //Si estamos en Modo DIOS, salimos de la rutina
        if (ParSAM.ModoDeveloper) { return true; }

        Process ethernet = new Process();
        ethernet.StartInfo.UseShellExecute = false;
        ethernet.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        ethernet.StartInfo.RedirectStandardOutput = true;
        ethernet.StartInfo.CreateNoWindow = true;

        string Rutadevcon;

        if (Environment.Is64BitOperatingSystem)
        {
            Rutadevcon = AppDomain.CurrentDomain.BaseDirectory + @"Devcon\64\devcon.exe";
        }
        else
        {
            Rutadevcon = AppDomain.CurrentDomain.BaseDirectory + @"Devcon\32\devcon.exe";
        }

        if (System.IO.File.Exists(Rutadevcon))
        {
            try
            {
                ethernet.StartInfo.FileName = Rutadevcon;
                if (habilitar)
                {
                    //Esperamos unos segundos, para que se habilite la tarjeta y se conecte a la red wifi (Obtenga IP)
                    ethernet.StartInfo.Arguments = @"enable " + device;
                    ethernet.Start();

                    //Esperamos a que encienda bien el dispositivo
                    //Thread.Sleep(5000);
                    return true;
                }
                else
                {
                    ethernet.StartInfo.Arguments = @"disable " + device;
                    ethernet.Start();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Obtenemos la IP del Equipo
    /// </summary>
    /// <returns></returns>
    public string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            IpActual = ip.ToString();
        }

        return IpActual;
    }

    /// <summary>
    /// Se encarga De liberar y renovar la ip que está teniendo la conexión en ese momento
    /// </summary>
    public void ReleaseIP_Renew()
    {
        try
        {
            System.Management.ManagementClass objMC = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
            System.Management.ManagementObjectCollection objMOC = objMC.GetInstances();

            foreach (System.Management.ManagementObject objMO in objMOC)
            {
                //Need to determine which adapter here with some kind of if() statement
                objMO.InvokeMethod("ReleaseDHCPLease", null, null);
                objMO.InvokeMethod("RenewDHCPLease", null, null);
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de validar si el primer anillo de red IP, pertenece a la intranet de Mobility ADO
    /// </summary>
    /// <param name="Anillos"></param>
    /// <returns></returns>
    public bool ValidaIP(string Anillos)
    {
        //Obtengo la IP Local
        string IP = GetLocalIpAddress();

        string[] AnilloIp = IP.Split('.');

        string[] AnillosValidos = Anillos.Split(',');

        foreach (string var in AnillosValidos)
        {
            if (var.Equals(AnilloIp[0]))
            {
                //Si es válida Aprovecho para guardar el 3er anillo de red, para posteriores validaciones ByRED
                AnilloRedCAN = Convert.ToDouble(AnilloIp[2]);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Se encarga de encontrar el servidor can por la IP
    /// </summary>
    /// <param name="BD_VMD"></param>
    /// <returns></returns>
    public bool CualServidorxIP(vmdEntities BD_VMD)
    {
        //VMD 7.0.3
        //var red = (from x in BD_VMD.can_redes
        //           join zonades in BD_VMD.can_zonasdescarga on x.IdZonaDescarga equals zonades.Id
        //           where x.Sufijo == Convert.ToDouble(AnilloRedCAN)
        //           orderby x.Id, zonades.Id
        //           select new { idred = x.Id, red = x.Descripcion, idzonadescarga = zonades.Id, zonadescarga = zonades.Descripcion, zonades.RutaDescarga }).FirstOrDefault();

        if (ParSAM.ModoDeveloper)
        {
            //Para sincronizar MXD
            AnilloRedCAN = 201;

            //Para sincronizar en SDL
            //AnilloRedCAN = 76;

            //Para sincronizar en TLP (NUBE)
            //AnilloRedCAN = 3;

            //Para Cuautla
            //AnilloRedCAN = 191;

            //Para Poza RIca
            //AnilloRedCAN = 238;

            //Para NTE
            //AnilloRedCAN = 168;

            //Tuxtla
            //AnilloRedCAN = 215;

            //Taller Merida
            //AnilloRedCAN = 140;

            //AnilloRedCAN = 119;
        }

        //Powered ByRED 18ENE2022: se ocupa descarga a instancias con ayuda de una nueva tabla
        //Sólo se necesitan las siguientes columnas, validar ByRED
        var red = (from x in BD_VMD.can_redes
                   join zonades in BD_VMD.can_zonasdescargainstancias on x.IdZonaDescarga equals zonades.Id
                   where x.Sufijo == AnilloRedCAN
                   orderby x.Id, zonades.Id
                   select new { zonadescarga = zonades.Descripcion, zonades.RutaDescarga, zonades.EsquemaInstancias, zonades.PuertoAPP, zonades.RutaDescargaAPP }).FirstOrDefault();

        if (red != null)
        {
            this.ZonaDescarga = red.zonadescarga;
            this.CualServidorCAN = red.RutaDescarga;
            this.FormaIdentiZona = "(RED)";

            //Validamos si la ubicación pertenece a nube
            if (red.EsquemaInstancias)
            {
                this.ServerNube = true;
                this.PuertoAPP = red.PuertoAPP;

                //Powered ByRED 01MAR2022
                this.CualServidorCAN = red.RutaDescargaAPP;
            }
            else
            {
                this.ServerAlterno = true;
            }

            return true;
        }
        else
        {
            //No se encuentra el anillo de red, entonces, checo si por GPS se ubicó la zona de descarga
            return false;
        }

        #region "Lógica Anterior"
        //Sólo se necesitan las siguientes columnas, validar ByRED
        //var red = (from x in BD_VMD.can_redes
        //           join zonades in BD_VMD.can_zonasdescarga2 on x.IdZonaDescarga equals zonades.Id
        //           where x.Sufijo == AnilloRedCAN
        //           orderby x.Id, zonades.Id
        //           select new { zonadescarga = zonades.Descripcion, zonades.RutaDescarga, zonades.Nube }).FirstOrDefault();

        //if (red != null)
        //{
        //    this.ZonaDescarga = red.zonadescarga;
        //    this.CualServidorCAN = red.RutaDescarga;
        //    this.FormaIdentiZona = "(RED)";

        //    Validamos si la ubicación pertenece a nube
        //    if (red.Nube)
        //    {
        //        this.ServerNube = true;
        //    }
        //    else
        //    {
        //        this.ServerAlterno = true;
        //    }

        //    return true;
        //}
        //else
        //{
        //    No se encuentra el anillo de red, entonces, checo si por GPS se ubicó la zona de descarga
        //    return false;
        //}

        #endregion
    }

    /// <summary>
    /// Se encarga de encontrar el servidor CAN por punto GPS
    /// </summary>
    /// <param name="Sistema"></param>
    /// <returns></returns>
    public bool CualServidorxGPS(ISistema Sistema)
    {
        //para CONDUSAT, creo....

        //gps
        if (true)
        {
            //despues de detectar que si está por punto GPS, entonces ByRED
            //this.ZonaDescarga = Terminal.DescripcionZona;
            //this.CualServidorCAN = Terminal.ZonaDescarga;
            this.ServerAlterno = true;
            this.FormaIdentiZona = "(GPS)";
            return true;
        }
        else
        {
            this.ZonaDescarga = string.Empty;
            this.ServerAlterno = false;
            this.FormaIdentiZona = string.Empty;
            return false;
        }

        
    }

    /// <summary>
    /// Se encarga de recojer las versiones de todos los satelites de SAM
    /// </summary>
    public void SistemasVersiones(vmdEntities VMD_BD)
    {
        var plat_versiones = (from x in VMD_BD.plat_versiones
                              select x).ToList();
        var parametrosInicio = (from x in VMD_BD.can_parametrosinicio
                                select x).FirstOrDefault();

        //Para SAM
        plat_versiones versionSAM = plat_versiones.Where(X => X.Sistemas.Equals("SAM")).FirstOrDefault();

        if (versionSAM != null)
        {
            strVersionesSistemas = "SAM~" + versionSAM.Versiones;
        }
        else
        {
            strVersionesSistemas = Application.ProductVersion;
        }

        //Recuperar el nombre del disco de peliculas
        //Powered ByRED 01MAR2021
        strVersionesSistemas = strVersionesSistemas + ObtenerNombreDisco();


        //Para ADOCAN
        if ((bool)parametrosInicio.CAN)
        {
            plat_versiones versionCAN = plat_versiones.Where(x => x.Sistemas.Equals("ADO-CAN")).FirstOrDefault();

            if (versionCAN != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/ADO-CAN~" + versionCAN.Versiones;
            }

            //Powered ByRED 15ENE2021

            //Para Abordaje
            plat_versiones versionAbordaje = plat_versiones.Where(x => x.Sistemas.Equals("SAM - Abordaje")).FirstOrDefault();

            if (versionAbordaje != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/SIIAB-Abordaje~" + versionAbordaje.Versiones;
            }

            //Para Telemetria
            plat_versiones versionTelemetria = plat_versiones.Where(x => x.Sistemas.Equals("SAM - Telemetria")).FirstOrDefault();

            if (versionTelemetria != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/SIIAB-Telematics~" + versionTelemetria.Versiones;
            }

        }

        //Para GPS
        if ((bool)parametrosInicio.ADOGPS)
        {
            plat_versiones versionGPS = plat_versiones.Where(x => x.Sistemas.Equals("SIIAB-ADOGPS")).FirstOrDefault();

            if (versionGPS != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/SIIAB-ADOGPS~" + versionGPS.Versiones;
            }
        }

        //Para VMD
        if ((bool)parametrosInicio.VMD)
        {
            plat_versiones versionVMD = plat_versiones.Where(x => x.Sistemas.Equals("SAM - VMD")).FirstOrDefault();

            if (versionVMD != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/SIIAB-VMD~" + versionVMD.Versiones;
            }
        }

        //Para Condusat
        if ((bool)parametrosInicio.CONDUSAT)
        {
            plat_versiones versionCondusat = plat_versiones.Where(x => x.Sistemas.Equals("SIIAB-CONDUSAT")).FirstOrDefault();

            if (versionCondusat != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/SIIAB-CONDUSAT~" + versionCondusat.Versiones;
            }
        }

        //Powered ByRED 08ENE2021
        //Para PLAT
        if ((bool)parametrosInicio.PLAT)
        {
        
            plat_versiones versionPLAT = plat_versiones.Where(x => x.Sistemas.Equals("PLAT")).FirstOrDefault();

            if(versionPLAT != null)
            {
                strVersionesSistemas = strVersionesSistemas + "/PLAT~" + versionPLAT.Versiones;
            }
        }

        //Obtener la versión de la imagen del registro
        try
        {
            var version = Registry.GetValue(@"HKEY_CURRENT_USER\CurrentVersion", "image_desc", 0);

            strVersionesSistemas = strVersionesSistemas + "/ImageID~" + version.ToString();
        }
        catch
        {

        }
        
        //Guardamos la MAC
        strVersionesSistemas = strVersionesSistemas + "/MacAddress~" + parametrosInicio.MacAddress;
    }

    /// <summary>
    /// Se encarga de obtener el nombre del disco secundario
    /// para reportarlo en la descarga
    /// Powered ByRED 01MAR2021
    /// </summary>
    /// <returns></returns>
    private string ObtenerNombreDisco()
    {
        string nombre = "/";
        var drives = DriveInfo.GetDrives();

        foreach (var drive in drives)
        {
            //Nos aseguramos que no tome un disco extraible
            if (drive.DriveType == DriveType.Fixed)
            {
                //Nos aseguramos que no sea el disco C://
                if (drive.Name != @"C:\")
                {
                    nombre = nombre + drive.VolumeLabel;
                    break;
                }
            }
        }
        return nombre;
    }

    #region "CambiarFechaHoraSistema"

    /// <summary>
    /// Se encarga de Obtener, verificar y cambiar la Fecha y Hora con la sincronización
    /// </summary>
    public void CambiarFechaHora()
    {
        DateTime fechaServer;
        //Obtenemos FechaServidor
        if (ServerNube)
        {
            fechaServer = servicioWCF.ObtenerFechaServidor();
        }
        else
        {
            fechaServer = ObtenerFechaServidor();
        }
        
        //Obtenemos Fecha del equipo
        DateTime fechaSistema = DateTime.Now;

        //Verificamos si trajo nueva fecha del servidor
        if (DateTime.Compare(fechaServer, this.FechaByRed) != 0)
        {
            //Comparamos, si son iguales no actualizo la del sistema
            if (DateTime.Compare(fechaSistema, fechaServer) != 0)
            {
                //Si las fechas no coinciden entonces actualizamos la del equipo, con la del server
                SetTime(fechaServer);
            }
        }
    }

    /// <summary>
    /// Obtiene mediante un query la fecha del servidor al que se conectó
    /// </summary>
    /// <returns></returns>
    private DateTime ObtenerFechaServidor()
    {
        DateTime fechaServidor = FechaByRed;
        string strQuery = "Select getdate() as fechaservidor";

            using (SqlCommand com = new SqlCommand(strQuery, this.SqlCon))
            {
                com.CommandType = CommandType.Text;
                com.CommandTimeout = this.comTimeOut;

                using (SqlDataReader dr = com.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            fechaServidor = Convert.ToDateTime(dr["fechaservidor"]).ToUniversalTime();
                        
                        }
                    }
                }
            }
        return fechaServidor;
    }

    /// <summary>
    /// Obtiene la fecha actual del sistem, se necesita obtener primero para poder cambiarla después
    /// </summary>
    private void GetTime()
    {
        SYSTEMTIME stime = new SYSTEMTIME();
        GetSystemTime(ref stime);
    }

    /// <summary>
    /// Actualiza la fecha del equipo con la fecha que se obtuvo del servidor
    /// </summary>
    /// <param name="fechaserver"></param>
    public void SetTime(DateTime fechaserver)
    {
        SYSTEMTIME systime = new SYSTEMTIME();
        GetSystemTime(ref systime);

        systime.wDay = (ushort)fechaserver.Day;
        systime.wDayOfWeek = (ushort)fechaserver.DayOfWeek;
        systime.wMonth = (ushort)fechaserver.Month;
        systime.wYear = (ushort)fechaserver.Year;
        systime.wHour = (ushort)fechaserver.Hour;
        systime.wMinute = (ushort)fechaserver.Minute;
        systime.wSecond = (ushort)fechaserver.Second;
        systime.wMilliseconds = (ushort)fechaserver.Millisecond;

        SetSystemTime(ref systime);

    }

    [DllImport("kernel32.dll")]
    private extern static void GetSystemTime(ref SYSTEMTIME lpsSystemTime);

    [DllImport("kernel32.dll")]
    private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
        
    }
    #endregion
}

