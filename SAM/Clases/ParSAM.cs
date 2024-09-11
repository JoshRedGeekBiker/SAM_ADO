using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ParSAM
{
    /// <summary>
    /// Se ocupa para preparar el sistema en caso de que
    /// nos encontremos en modo developer
    /// si se pone en falso, la aplicación se comportaria
    /// cómo si estuvieramos en producción
    /// </summary>
    public static bool ModoDeveloper { get; set; } = false;

    /// <summary>
    /// Se encarga de leer el archivo
    /// </summary>
    public static void LeerArchivo()
    {
        var ruta = AppDomain.CurrentDomain.BaseDirectory;

        //Se compone toda la ruta de XML
        ruta = ruta + "keygen.SAM";

        FileInfo Archivo = new FileInfo(ruta);

        if (Archivo.Exists)
        {

            string fechaarchivo = File.GetLastWriteTime(ruta).ToString();
            //Para sistemas W10
            string fecha = "30/10/1993";

            //Comentamos las siguientes lineas, ya que el cabecita de algodon quitó el horario de verano y viene retrasado, también la hora
            //XD Cotorrea...


            //string fecha = "30/10/1993 12:30:00 a. m.";

            //if (fecha.Contains(fechaarchivo))
            //{
            //    ModoDeveloper = true;
            //}else
            //{
            //    //Para sistemas W7
            //    //fecha = "30/10/1993 12:30:00 a.m.";
            //    fecha = "30/10/1993";

            //    if (fecha.Contains(fechaarchivo))
            //    {
            //        ModoDeveloper = true;
            //    }
            //}
            if (fechaarchivo.Contains(fecha))
            {
                ModoDeveloper = true;
            }
            
        }
        else
        {
            ModoDeveloper = false;
        }
    }
}
