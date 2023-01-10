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
    private String _bdProviderName = String.Empty;
    public String BDProviderName
    {
        get
        {
            return _bdProviderName;
        }

        set
        {
            _bdProviderName = value;
        }
    }

    private String _bdServer = String.Empty;
    public String BDServer
    {
        get
        {
            return _bdServer;
        }
        set
        {
            _bdServer = value;
        }
    }

    private String _bdName = String.Empty;
    public String BDName
    {
        get
        {
            return _bdName;
        }
        set
        {
            _bdName = value;
        }
    }

    private String _bdUser = String.Empty;
    public String BDUser
    {
        get
        {
            return _bdUser;
        }
        set
        {
            _bdUser = value;
        }
    }

    private String _bdPass = String.Empty;
    public String BDPass
    {
        get
        {
            return _bdPass;
        }
        set
        {
            _bdPass = value;
        }
    }

    private Boolean _modoPrueba = false;
    public Boolean ModoPrueba
    {
        get
        {
            return _modoPrueba;
        }

        set
        {
            _modoPrueba = value;
        }
    }

    private Boolean _reinicioAutomatico = false;
    public Boolean ReinicioAutomatico
    {
        get
        {
            return _reinicioAutomatico;
        }
        set
        {
            _reinicioAutomatico = value;
        }
    }

    private Boolean _adoCAN = false;
    public Boolean ADOCAN
    {
        get
        {
            return _adoCAN;
        }
        set
        {
            _adoCAN = value;
        }
    }

    private Boolean _adoCONDUSAT = false;
    public Boolean ADOCONDUSAT
    {
        get
        {
            return _adoCONDUSAT;
        }
        set
        {
            _adoCONDUSAT = value;
        }
    }

    private int _tiempoADOCAN = 0;
    public int TiempoADOCAN
    {
        get
        {
            return _tiempoADOCAN;
        }
        set
        {
            _tiempoADOCAN = value;
        }
    }

    private int _tiempoADOGPS = 0;
    public int TiempoADOGPS
    {
        get
        {
            return _tiempoADOGPS;
        }
        set
        {
            _tiempoADOGPS = value;
        }
    }

    private int _tiempoCONDUSAT = 0;
    public int TiempoCONDUSAT
    {
        get
        {
            return _tiempoCONDUSAT;
        }
        set
        {
            _tiempoCONDUSAT = value;
        }
    }

    private int _tiempoSIA = 0;
    public int TiempoSIA
    {
        get
        {
            return _tiempoSIA;
        }
        set
        {
            _tiempoSIA = value;
        }
    }

    private string _puertoWS = String.Empty;
    public string PuertoWS
    {
        get
        {
            return _puertoWS;
        }
        set
        {
            _puertoWS = value;
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

        xmlListaNodos = ArchivoXML.SelectNodes("/Configuracion/SAM");


        foreach (XmlElement Node in xmlListaNodos)
        {
            switch (Node.Attributes.GetNamedItem("id").Value)
            {
                case "BDProviderName":
                    xml.BDProviderName = Node.Attributes.GetNamedItem("valor").Value;
                    break;

                case "BDServer":
                    xml.BDServer = Node.Attributes.GetNamedItem("valor").Value;
                    break;

                case "BDName":
                    xml.BDName = Node.Attributes.GetNamedItem("valor").Value;
                    break;

                case "BDUser":
                    xml.BDUser = Node.Attributes.GetNamedItem("valor").Value;
                    break;

                case "BDPass":
                    xml.BDPass = Node.Attributes.GetNamedItem("valor").Value;
                    break;

                case "ModoPrueba":

                    if (ParSAM.ModoDeveloper)
                    {
                        xml.ModoPrueba = true;
                    }
                    else
                    {
                        if ((Node.Attributes.GetNamedItem("valor").Value).Equals("1"))
                        {
                            xml.ModoPrueba = true;
                        }
                        else
                        {
                            xml.ModoPrueba = false;
                        }
                    }

                    break;

                case "ReinicioAutomatico":
                    if ((Node.Attributes.GetNamedItem("valor").Value).Equals("1"))
                    {
                        xml.ReinicioAutomatico = true;
                    }
                    else
                    {
                        xml.ReinicioAutomatico = false;
                    }

                    break;

                case "ADOCAN":
                    if ((Node.Attributes.GetNamedItem("valor").Value).Equals("1"))
                    {
                        xml.ADOCAN = true;
                    }
                    else
                    {
                        xml.ADOCAN = false;
                    }
                    break;

                case "ADOCONDUSAT":
                    if ((Node.Attributes.GetNamedItem("valor").Value).Equals("1"))
                    {
                        xml.ADOCONDUSAT = true;
                    }
                    else
                    {
                        xml.ADOCONDUSAT = false;
                    }
                    break;

                case "TiempoADOCAN":

                    try
                    {
                        xml.TiempoADOCAN = Convert.ToInt32(Node.Attributes.GetNamedItem("valor").Value);
                    }
                    catch
                    {
                        xml.TiempoADOCAN = 10000;
                    }
                    break;

                case "TiempoADOGPS":

                    try
                    {
                        xml.TiempoADOGPS = Convert.ToInt32(Node.Attributes.GetNamedItem("valor").Value);
                    }
                    catch
                    {
                        xml.TiempoADOGPS = 5000;
                    }
                    break;

                case "TiempoCONDUSAT":

                    try
                    {
                        xml.TiempoCONDUSAT = Convert.ToInt32(Node.Attributes.GetNamedItem("valor").Value);
                    }
                    catch
                    {
                        xml.TiempoCONDUSAT = 5000;
                    }
                    break;

                case "TiempoSIA":

                    try
                    {
                        xml.TiempoSIA = Convert.ToInt32(Node.Attributes.GetNamedItem("valor").Value);
                    }
                    catch
                    {
                        xml.TiempoSIA = 5000;
                    }
                    break;

                case "PuertoWS":
                    xml.PuertoWS = Node.Attributes.GetNamedItem("valor").Value;
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
            ruta = "C:\\Program Files\\SIIAB - ADO\\SIIAB - ADOSAM\\";

            if (Directory.Exists(ruta) == false)
            {
                //Para ruta en español
                ruta = "C:\\Archivos de Programa\\SIIAB - ADO\\SIIAB - ADOSAM\\";

                if (Directory.Exists(ruta) == false)
                {
                    ruta = AppDomain.CurrentDomain.BaseDirectory;
                }
            }

        }

        //Se compone toda la ruta de XML
        ruta = ruta + "ADOSAM.xml";

        FileInfo Archivo = new FileInfo(ruta);

        if (!Archivo.Exists)
        {
            CrearXML();
        }

    }

    public static Boolean CrearXML(int ReinicioAutomatico = 0)
    {
        try
        {
            XDocument xmlNuevo = new XDocument(new XElement("Configuracion",
          new XElement("SAM", new XAttribute("id", "BDProviderName"), new XAttribute("valor", "MySql.Data.MySql.Client")),
          new XElement("SAM", new XAttribute("id", "BDServer"), new XAttribute("valor", "localhost")),
          new XElement("SAM", new XAttribute("id", "BDName"), new XAttribute("valor", "vmd")),
          new XElement("SAM", new XAttribute("id", "BDUser"), new XAttribute("valor", "root")),
          new XElement("SAM", new XAttribute("id", "BDPass"), new XAttribute("valor", "root")),
          new XElement("SAM", new XAttribute("id", "ModoPrueba"), new XAttribute("valor", "0")),
          new XElement("SAM", new XAttribute("id", "ReinicioAutomatico"), new XAttribute("valor", ReinicioAutomatico)),
          new XElement("SAM", new XAttribute("id", "ADOCAN"), new XAttribute("valor", "1")),
          new XElement("SAM", new XAttribute("id", "ADOCONDUSAT"), new XAttribute("valor", "1")),
          new XElement("SAM", new XAttribute("id", "TiempoADOCAN"), new XAttribute("valor", "10000")),
          new XElement("SAM", new XAttribute("id", "TiempoADOGPS"), new XAttribute("valor", "5000")),
          new XElement("SAM", new XAttribute("id", "TiempoCONDUSAT"), new XAttribute("valor", "5000")),
          new XElement("SAM", new XAttribute("id", "TiempoSIA"), new XAttribute("valor", "5000")),
          new XElement("SAM", new XAttribute("id", "PuertoWS"), new XAttribute("valor", "8080"))
          ));

            xmlNuevo.Save(ruta);

            return true;
        }
        catch
        {
            return false;
        }

    }

    /// <summary>
    /// Se encarga de planchar el XML en caso de hayan existido cambios durante
    /// la ejecución
    /// </summary>
    /// <param name="confxml"></param>
    /// <returns></returns>
    public static Boolean CrearXML(XML confxml)
    {
        try
        {
            XDocument xmlNuevo = new XDocument(new XElement("Configuracion",
          new XElement("SAM", new XAttribute("id", "BDProviderName"), new XAttribute("valor", confxml.BDProviderName)),
          new XElement("SAM", new XAttribute("id", "BDServer"), new XAttribute("valor", confxml.BDServer)),
          new XElement("SAM", new XAttribute("id", "BDName"), new XAttribute("valor", confxml.BDName)),
          new XElement("SAM", new XAttribute("id", "BDUser"), new XAttribute("valor", confxml.BDUser)),
          new XElement("SAM", new XAttribute("id", "BDPass"), new XAttribute("valor", confxml.BDPass)),
          new XElement("SAM", new XAttribute("id", "ModoPrueba"), new XAttribute("valor", Convert.ToInt32(confxml.ModoPrueba))),
          new XElement("SAM", new XAttribute("id", "ReinicioAutomatico"), new XAttribute("valor", Convert.ToInt32(confxml.ReinicioAutomatico))),
          new XElement("SAM", new XAttribute("id", "ADOCAN"), new XAttribute("valor", Convert.ToInt32(confxml.ADOCAN))),
          new XElement("SAM", new XAttribute("id", "ADOCONDUSAT"), new XAttribute("valor", Convert.ToInt32(confxml.ADOCONDUSAT))),
          new XElement("SAM", new XAttribute("id", "TiempoADOCAN"), new XAttribute("valor", confxml.TiempoADOCAN)),
          new XElement("SAM", new XAttribute("id", "TiempoADOGPS"), new XAttribute("valor", confxml.TiempoADOGPS)),
          new XElement("SAM", new XAttribute("id", "TiempoCONDUSAT"), new XAttribute("valor", confxml.TiempoCONDUSAT)),
          new XElement("SAM", new XAttribute("id", "TiempoSIA"), new XAttribute("valor", confxml.TiempoSIA)),
          new XElement("SAM", new XAttribute("id", "PuertoWS"), new XAttribute("valor", "8080"))
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

