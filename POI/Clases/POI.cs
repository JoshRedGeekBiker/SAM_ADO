using InterfazSistema.ModelosBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

public class POI : IBDContext, IBDContextPOI, IGPS, ISistema
{
    #region "Propiedades"
    //Contexto Base de Datos
    public vmdEntities VMD_BD { get; }
    public poiEntities POI_BD { get; }
    public GPSData Datos_GPS { get; set; }

    //Para el Data de GPS
    //public Cliente ADOGPS { get; set; }
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }

    public Sistema Sistema { get { return Sistema.POI; } }

    public string GetVersionSistema => throw new System.NotImplementedException();

    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    public String status_sync { get; set; } = String.Empty;

    #endregion

    #region "Variables"
    //Parametros
    private can_parametrosinicio ParametrosInicioCAN;
    private Utilidades MyUtils;

    //Flags
    private bool EsperandoSync = false;

    //Hilos
    private Thread HiloDeteccionColision;
    private List<string> ListaSpots = new List<string>(); //PoweredByToto


    //Timers
    private System.Timers.Timer timerPrueba = new System.Timers.Timer();
    private System.Timers.Timer timerActualiza = new System.Timers.Timer();

    //Catalogos
    private List<List<POINT>> lPoint = null;
    private List<Response_spot> point = null;

    private POINT punto_actual;
    private POINT punto_anterior;
    #endregion

    #region "Eventos"

    #endregion
    #region "Variables de eventos"

    /// <summary>
    /// Para enviar una alerta de conductor hacia SAM
    /// </summary>
    /// <returns></returns>
    public delegate void MandarAlertaSAM(string alerta);
    public event MandarAlertaSAM AlertaSAM;

    /// <summary>
    /// Se encarga de 
    /// </summary>
    /// <param name="Estado"></param>
    public delegate void IndicadorSAM(int Estado);
    public event IndicadorSAM IndicadorLed;

    #endregion
    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public POI()
    {
        //Iniciamos los parametros generales
        VMD_BD = new vmdEntities();
        ParametrosInicioCAN = (from x in VMD_BD.can_parametrosinicio select x).FirstOrDefault();
        POI_BD = new poiEntities();
        punto_actual = new POINT();
        punto_anterior = new POINT();
        ListaSpots = new List<string>();
        MyUtils = new Utilidades();
    }

    #endregion

    #region "Métodos privados"
    private void PreparaTimersEHilos()
    {

        //Hilos
        HiloDeteccionColision = new Thread(new ThreadStart(colisionador));


        //Timers

        //timer para debbug
        timerPrueba.Interval = 10000;
        timerPrueba.Enabled = true;
        timerPrueba.Elapsed += Debuggear;
        timerPrueba.Start();
    }

    /// <summary>
    /// Método que sirve para debuggear
    /// </summary>
    private void Debuggear(Object sender, ElapsedEventArgs e)
    {
        timerPrueba.Stop();
    }

    /// <summary>
    /// Se encarga de cargar los catálogos en memoria para evitar hacer peticiones en exceso a la BD
    /// </summary>
    private void CargarCatalogos()
    {
        try
        {
            WS_Spot ws = new WS_Spot();
            point = ws.getListSpots();
            lPoint = new List<List<POINT>>();
            List<POINT> Point = new List<POINT>();
            foreach (Response_spot x in point)
            {
                foreach (Coordenadas y in x.parada.coordenadas)
                {
                    Point.Add(new POINT(y.latitud, y.longitud, y.sequence, x));
                }
                lPoint.Add(Point);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void colisionador()
    {
        Response_spot res = DetectarColisiones();

    }
    /// <summary>
    /// Se encarga de detectar las colisiones
    /// </summary>
    private Response_spot DetectarColisiones()
    {
        Response_spot res = null;
        try
        {
            punto_anterior = punto_actual;
            punto_actual = new POINT(Datos_GPS.Latitud, Datos_GPS.Longitud);
            foreach (List<POINT> lp in lPoint)
            {
                POLYGON pol = new POLYGON(lp.ToArray());
                if (pol.in_poligone(punto_actual, false))
                {
                    float angle_bus = pol.angulo_orientacion(punto_anterior, punto_actual);
                    if (in_angle(lp.ElementAt(0).item.orientacionInicial, angle_bus, lp.ElementAt(0).item.orientacionFinal))
                    {
                        res = lp.ElementAt(0).item;
                        break;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return res;
    }
    //Validar reglas de negocio del spot (horario,vigencia, etc)
    private void procesar_colision(Response_spot spot)
    {


    }

    /// <summary>
    /// Detecta que el autobus se encuentre dentro del agunlo
    /// </summary>
    /// <param name="comienzo"></param>
    /// <param name="punto"></param>
    /// <param name="final"></param>
    /// <returns></returns>
    public Boolean in_angle(float comienzo, float punto, float final)
    {
        Boolean res = false;
        double p = Math.Sin(30 * Math.PI / 360);

        if (comienzo >= 180 && comienzo > final)
        {

            if (punto >= 180 && punto <= 360 + final)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }



            }


            else

            {
                if (punto <= 180 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;


                }

            }


        }
        else if (comienzo >= 90 && comienzo > final)
        {

            if (punto >= 90 && punto <= 360 + final)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }

            }


            else

            {
                if (punto <= 90 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;

                }

            }


        }
        else if (comienzo >= 0 && comienzo > final)
        {

            if (punto >= 0 && punto <= 360)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }

            }
            else

            {
                if (punto <= 0 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;

                }

            }


        }
        else
        {
            double A = (Math.Cos(comienzo * Math.PI / 360)) * (Math.Sin(punto * Math.PI / 360)) - (Math.Sin(comienzo * Math.PI / 360)) * (Math.Cos(punto * Math.PI / 360));
            double Ab = (Math.Cos(comienzo * Math.PI / 360)) * (Math.Sin(final * Math.PI / 360)) - (Math.Sin(comienzo * Math.PI / 360)) * (Math.Cos(final * Math.PI / 360));
            double B = (Math.Cos(final * Math.PI / 360)) * (Math.Sin(punto * Math.PI / 360)) - (Math.Sin(final * Math.PI / 360)) * (Math.Cos(punto * Math.PI / 360));
            double Bb = (Math.Cos(final * Math.PI / 360)) * (Math.Sin(comienzo * Math.PI / 360)) - (Math.Sin(final * Math.PI / 360)) * (Math.Cos(comienzo * Math.PI / 360));

            if (A * Ab >= 0 && B * Bb >= 0)
            {

                res = true;
            }
        }
        return res;
    }

    #endregion

    #region "Métodos publicos"
    /// <summary>
    /// Se encarga de pedir el nombre de los Spots
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerSpots(int tipo)
    {
        ListaSpots = MyUtils.RecuperarSpots(ParametrosInicioCAN.CarpetaVideos, tipo);
        return ListaSpots;
    }
    #endregion

    #region "Métodos heredados"
    public void Inicializar()
    {
        //CargarCatalogos();
        //CrearGPS();
        //DetectarColisiones();
    }

    public void Finalizar()
    {
        throw new System.NotImplementedException();
    }

    public bool Sincronizar()
    {
        Boolean res = false;
        try
        {
            status_sync = "Iniciando sincronizacion SIIAP POI";
            WS_Spot ws = new WS_Spot();
            Request_spot request = new Request_spot();
            request.numeroEconomico = ParametrosInicioCAN.Autobus;
            request.latitud = (float)Convert.ToDouble(Datos_GPS.Latitud);
            request.longitud = (float)Convert.ToDouble(Datos_GPS.Longitud);
            res = ws.syncSpots(request);
            status_sync = "Sincronizacion POI: " + (res ? true : false);
            //Sincronizacion de testigos
            res = ws.syncTestigo(ParametrosInicioCAN.Autobus);
            status_sync = "Sincronizacion Testigos POI: " + (res ? true : false);
            status_sync = "Finalizando sincronizacion SIIAP POI";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }


    public void Actualizar()
    {
       
    }
    #endregion


    #region "Timers"

    /// <summary>
    /// Se encarga de desencadenar los hilos para el procesamiento de los codigos
    /// </summary>
    private void Actualizar_Tick(object sender, ElapsedEventArgs e)
    {
        timerActualiza.Stop();
        try
        {
            if (EsperandoSync) { return; }

            if (HiloDeteccionColision != null)
            {
                if (!HiloDeteccionColision.IsAlive)
                {
                    HiloDeteccionColision = new Thread(new ThreadStart(colisionador));
                    HiloDeteccionColision.Start();
                }
            }

        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }


        timerActualiza.Start();
    }

    #endregion
}