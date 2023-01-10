using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Utils
{

    #region "Propiedades"
    public double ProgresoCopiado { get; set; } = 0;
    public string PautaActual { get; set; } = string.Empty; //Powered ByRED 16JUL2020

    #endregion

    #region "Variables"
    private List<string> Carpetas = new List<string>();//PoweredByRED 13JUL2020
    #endregion

    #region "Constructores"
    public Utils()
    {
        //Agregamos el nombre de las carpetas para copiar sus archivos para caso USB
        //PoweredByRED 13JUL2020
        Carpetas.Add("\\SPOTS");
        Carpetas.Add("\\FILMS");
        Carpetas.Add("\\DOCUM");
    }


    #endregion

    #region "Métodos Públicos"
    /// <summary>
    /// Se encarga de recuperar el nombre de las carpetas que contienen un arhivo de script
    /// para pauta de VMD del Disco de Películas
    /// </summary>
    /// <returns></returns>
    public List<string> RecuperarScripts(string RutaCarpeta)
    {
        List<String> ListaRetorno = new List<string>();

        var RutaScripts = RutaCarpeta + "\\PAUTA";

        //Para prueba

        //RutaScripts = "C:\\MOVIES\\PAUTA";


        if (Directory.Exists(RutaScripts))
        {
            var Subcarpetas = Directory.GetDirectories(RutaScripts);

            foreach (string dir in Subcarpetas)
            {
                if (File.Exists(dir + "\\Script.sql"))
                {
                    ListaRetorno.Add(dir.Split('\\').Last());
                }
            }

        }
        return ListaRetorno;
    }

    /// <summary>
    /// Se encarga de obtener la letra de unidad
    /// de los discos removibles
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerDiscos()
    {
        List<string> ListaRetorno = new List<string>();
        var drives = DriveInfo.GetDrives();

        foreach (var drive in drives)
        {
            if (drive.DriveType == DriveType.Removable)
            {
                ListaRetorno.Add(drive.Name);
            }
        }
        return ListaRetorno;
    }

    /// <summary>
    /// Se encarga de insertar la pauta requerida
    /// </summary>
    /// <param name="NombrePauta"></param>
    /// <returns></returns>
    public Task<bool> InsertarNuevaPauta(string Ruta)
    {
        return Task<bool>.Run(
            async () =>
            {
                await Task.Delay(1);

                try
                {
                    //Validamos la existencia del directorio en caso de una
                    //desconexión de disco
                    if (!Directory.Exists(Ruta))
                    {
                        return false;
                    }

                    var RutaScript = Ruta + "\\Script.sql";
                    var RutaEjecutor = Ruta + "\\ejecutor.bat";
                    //Para pruebas
                    //RutaScript = "C:\\MOVIES\\PAUTA\\" + NombrePauta + "\\Script.sql";
                    //RutaEjecutor = "C:\\MOVIES\\PAUTA\\" + NombrePauta + "\\ejecutor.bat";

                    if (File.Exists(RutaEjecutor))
                    {
                        File.Delete(RutaEjecutor);
                    }

                    string RutaMySQL;
                    string result;

                    if (Environment.Is64BitOperatingSystem)
                    {
                        RutaMySQL = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MySQL\\MySQL Server 5.0\\bin\\mysql.exe";
                    }
                    else
                    {
                        RutaMySQL = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\MySQL\\MySQL Server 5.0\\bin\\mysql.exe";
                    }

                    if (File.Exists(RutaMySQL))
                    {

                        using (FileStream fs = File.Create(RutaEjecutor))
                        {
                            byte[] arguments = new UTF8Encoding(true).GetBytes("\"" + RutaMySQL + "\" -h localhost -uroot -proot vmd <\"" + RutaScript + "\"");
                            fs.Write(arguments, 0, arguments.Length);
                        }

                        ProcessStartInfo start = new ProcessStartInfo();

                        start.UseShellExecute = false;
                        start.RedirectStandardOutput = true;
                        start.FileName = RutaEjecutor;

                        using (Process script = Process.Start(start))
                        {
                            using (StreamReader reader = script.StandardOutput)
                            {
                                result = reader.ReadToEnd();
                            }
                        }

                        var hola = result;
                    }

                    return true;
                }
                catch
                {
                    return false;
                }

            });
    }


    /// <summary>
    /// Se encarga de validar las versiones de las pautas
    /// Powered ByRED 16/JUL/2020
    /// </summary>
    /// <param name="Ruta"></param>
    /// <returns></returns>
    public Task<bool> ValidarVersionPauta(string Ruta, string versionPauta)
    {
        return Task<bool>.Run(
            async () =>
            {
                await Task.Delay(1);

                try
                {
                    //Validamos la existencia del directorio en caso de una
                    //desconexión de disco
                    if (!Directory.Exists(Ruta))
                    {
                        return false;
                    }

                    var RutaParametros = Ruta + "\\ParametrosScript.txt";
                    

                    if (File.Exists(RutaParametros))
                    {
                        var versionNueva = Convert.ToInt32(JsonConvert.DeserializeObject<ParametrosPauta>(File.ReadAllText(RutaParametros)).version);

                        var versionMovil = Convert.ToInt32(versionPauta);

                        if (versionNueva >= versionMovil)
                        {
                            PautaActual = versionNueva.ToString();
                            return true;
                        }
                        else
                        {
                            PautaActual = versionMovil.ToString();
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            });
    }

    /// <summary>
    /// Se encargará de transferir los archivos de las carpetas definidas en el constructor
    /// para caso USB
    /// Powered ByRED 13JUL2020
    /// </summary>
    /// <param name="rutaCarpeta"></param>
    /// <returns></returns>
    public Task<bool> TransferirArchivos(string RutaDestino, string RutaOrigen)
    {
        return Task<bool>.Run(
            async () =>
            {
                await Task.Delay(1);

                try
                {
                    var resultado = false;

                    //Iteramos las carpetas definidas
                    foreach (string strCarpeta in Carpetas)
                    {
                        //Reformulamos las rutas de destino y origen
                        var RutaOrigenTemp = RutaOrigen + strCarpeta;
                        var RutaDestinoTemp = RutaDestino + strCarpeta;

                        if (Directory.Exists(RutaOrigenTemp)) //Validamos la existencia de la ruta de origen
                        {
                            if (Directory.Exists(RutaDestinoTemp))// Validamos la existencia de la ruta de Destino
                            {
                                //Leemos todos los archivos del origen
                                DirectoryInfo di = new DirectoryInfo(RutaOrigenTemp);
                                var ListaArchivos = new List<string>();

                                foreach (var fi in di.GetFiles())
                                {
                                    ListaArchivos.Add(fi.Name);
                                }

                                double sumando = 0;

                                try
                                {
                                    sumando = 100 / ListaArchivos.Count;
                                }
                                catch
                                {
                                    sumando = 10;
                                }

                                //Validamos que no existan esos archivos en la carpeta Destino
                                //De ser así los eliminamos

                                foreach (var file in ListaArchivos)
                                {

                                    var ruta = RutaDestinoTemp + "\\" + file;

                                    if (File.Exists(ruta))
                                    {
                                        File.Delete(ruta);
                                    }

                                    //Ahora copiamos los archivos de origen a la destino
                                    File.Copy(RutaOrigenTemp + "\\" + file, ruta);


                                    //Actualizamos el progeso
                                    ProgresoCopiado = ProgresoCopiado + sumando;

                                }

                                ProgresoCopiado = 0;

                                resultado = true;
                            }
                        }
                    }
                    return resultado;
                }
                catch
                {
                    return false;
                }

            });
    }

    #endregion

    #region "Métodos Privados"


    #endregion
}
