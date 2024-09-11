using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;


public class XML
{

    public XML()
    {
        //Cargamos el archivo de configuracion
        ArchivoXML.ObtenerXML(this);
    }

    #region "Propiedades"
    private Boolean _Log = false;
    public Boolean Log
    {
        get
        {
            return _Log;
        }

        set
        {
            _Log = value;
        }
    }

    private int _milisegundosRetardo = 60000;

    public int MilisegundosRetardo
    {
        get
        {
            return _milisegundosRetardo;
        }
        set
        {
            _milisegundosRetardo = value;
        }
    }
    #endregion
}


class ArchivoXML
{

    private static String ruta = String.Empty;

    public static void ObtenerXML(XML xml)
    {

        //Se verifica si existe el XML, si no lo crea
        ChecarXML();

        //Procede con la lectura del archivo XML
        XmlDocument ArchivoXML = new XmlDocument();
        XmlNodeList xmlListaNodos;

        ArchivoXML.Load(ruta);

        xmlListaNodos = ArchivoXML.SelectNodes("/Configuracion/SIIAB_POI");


        foreach (XmlElement Node in xmlListaNodos)
        {
            switch (Node.Attributes.GetNamedItem("id").Value)
            {
                case "Log":

                    if ((Node.Attributes.GetNamedItem("valor").Value).Equals("1"))
                    {
                        xml.Log = true;
                    }
                    else
                    {
                        xml.Log = false;
                    }

                    break;

                case "MilisegundosRetardo":
                    try
                    {
                        xml.MilisegundosRetardo = Convert.ToInt32(Node.Attributes.GetNamedItem("valor").Value);
                    }
                    catch
                    {
                        //Le ponemos mínimo 1 minuto
                        xml.MilisegundosRetardo = 60000;
                    }

                    break;

                default:
                    break;
            }
        }

    }

    private static void ChecarXML()
    {
        //Se verifica la ruta del archivo XML
        if (ruta == String.Empty || Directory.Exists(ruta) == false)
        {
            //Para ruta en inglés
            ruta = "C:\\Program Files\\SIIAB - ADO\\SIIAB - POI\\";

            if (Directory.Exists(ruta) == false)
            {
                //Para ruta en español
                ruta = "C:\\Archivos de Programa\\SIIAB - ADO\\SIIAB - POI\\";

                if (Directory.Exists(ruta) == false)
                {
                    ruta = AppDomain.CurrentDomain.BaseDirectory;
                }
            }

        }

        //Se compone toda la ruta de XML
        ruta = ruta + "POIConfig.xml";

        FileInfo Archivo = new FileInfo(ruta);

        if (!Archivo.Exists)
        {
            CrearXML();
        }
    }

    /// <summary>
    /// Se encarga de crear el XML
    /// </summary>
    /// <returns></returns>
    public static Boolean CrearXML()
    {
        try
        {
            XDocument xmlNuevo = new XDocument(new XElement("Configuracion",
            new XElement("SIIAB_POI", new XAttribute("id", "Log"), new XAttribute("valor", "0")),
            new XElement("SIIAB_POI", new XAttribute("id", "MilisegundosRetardo"), new XAttribute("valor", "60000"))
            ));

            xmlNuevo.Save(ruta);

            return true;
        }
        catch
        {
            return false;
        }

    }

}