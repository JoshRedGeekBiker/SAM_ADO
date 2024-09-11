using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using InterfazSistema.ModelosBD;
using InterfazSistema;
/// <summary>
/// Powered ByRED 23DIC2022
/// </summary>
public class POI : IBDContextPOI, IBDContext, ISistema, IGPS
{
    #region "Propiedades"


    //Contexto Base de Datos
    public vmdEntities VMD_BD { get; }
    public poiEntities POI_BD { get; }

    #endregion

    #region "Propiedades Heredadas"
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }

    public Sistema Sistema { get { return Sistema.POI; } }

    public string GetVersionSistema { get; }

    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    public GPSData Datos_GPS { get; set; }

    #endregion

    #region "Variables"

    //Parametros
    private can_parametrosinicio ParametrosInicioCAN;
    private String RutaMaestra = "";
    private int long_negativo = 0;

    //Flags
    private bool EsperandoSync = false;
    private bool InDev = false;

    //Hilos
    private Thread HiloDeteccionColision;
    private Thread HiloPrueba;


    //Catalogos
    //private List<List<POINT>> lPoint = null;
    private List<Response_spot> point = null;

    //Se encargan de ayudarnos para el calculo de la colisión
    private POINT punto_actual = new POINT();
    private POINT punto_anterior = new POINT();

    //Para SYNC
    private bool Sincronizando = false;
    private List<Response_spot> listSpots = new List<Response_spot>();
    #endregion

    #region "Eventos"
    /// <summary>
    /// Se encarga de enviar un mensaje de lo que está haciendo enla sincronización
    /// </summary>
    /// <param name="mensaje"></param>
    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSyncPOI;

    /// <summary>
    /// Se encarga de enviarle al reproductor el spot que tiene que reproducir
    /// </summary>
    public delegate Boolean EnviarSpot(Response_spot sp);
    public event EnviarSpot NuevoSpot;
    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public POI(bool _developer)
    {
        this.InDev = _developer;

        //Iniciamos los parametros generales
        VMD_BD = new vmdEntities();
        ParametrosInicioCAN = (from x in VMD_BD.can_parametrosinicio select x).FirstOrDefault();
        POI_BD = new poiEntities();
        RutaMaestra = ParametrosInicioCAN.CarpetaVideos;
        String cv_param = "long_negativo";
        long_negativo = Convert.ToInt32((from x in POI_BD.parametros_poi where x.cve_parametro == cv_param select x.valor_parametro).FirstOrDefault());
    }
    #endregion

    #region "Pruebas"
    public void metodo_pruebas(String numeroEconomico)
    {
        WS_Spot spot = new WS_Spot(RutaMaestra);
        spot.delete_lanzador(numeroEconomico);
    }
    #endregion

    #region "Métodos Privados"

    /// <summary>
    /// Se ecarga deconfigurar los timers e hilos
    /// </summary>
    private void PreparaTimersEHilos()
    {
        //Hilos
        HiloDeteccionColision = new Thread(new ThreadStart(Colisionador));
    }

    /// <summary>
    /// Se encarga de detener los timers e hilos para un cierre correcto del programa
    /// </summary>
    private void DetieneTimersEHilos()
    {

    }
    /// <summary>
    /// Método que sirve para debuggear
    /// </summary>
    private void Debuggear(Object sender, ElapsedEventArgs e)
    {

    }

    /// <summary>
    /// Se encarga de cargar los catálogos en memoria para evitar hacer peticiones en exceso a la BD
    /// </summary>
    private void CargarCatalogos()
    {
        try
        {
            WS_Spot ws = new WS_Spot(RutaMaestra);
            point = ws.getListSpots();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Se encarga de detectar las colisiones
    /// </summary>
    private void Colisionador()
    {
        try
        {
            //Paso 1: Detectar colisión
            Response_spot Colision = DetectarColisiones();
            Boolean res = false;
            if (Colision != null)
            {
                //Paso 2: Se analiza con las reglas de negocio establecidas
                if (Procesar_colision(Colision))
                {
                    //Paso 3: Crear Registro en Base de datos para llevar a cabo el evento
                    GenerarEvento(Colision);
                }
            }
            //Paso 4 Verificar si hay spots en memoria para madnarlos a reproducir
            if (listSpots.Count > 0)
            {
                //foreach (Response_spot Rs in listSpots)
                //{
                //    res = NuevoSpot(Rs);
                //    if (res)
                //    {
                //        listSpots.Remove(Rs);
                //    }
                //}
                for (int i = 0; i < listSpots.Count; i++)
                {
                    res = NuevoSpot(listSpots.ElementAt(i));
                    if (res)
                    {
                        listSpots.RemoveAt(i);
                        i--;
                    }
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    /// <summary>
    /// Paso 1: Se encarga de detectar las colisiones
    /// </summary>
    //private Response_spot DetectarColisiones()
    //{
    //    Response_spot res = null;
    //    try
    //    {
    //        punto_anterior = punto_actual;
    //        //punto_actual = new POINT(Datos_GPS.Latitud, Datos_GPS.Longitud);
    //        Double longG = Convert.ToDouble(Datos_GPS.Longitud) * -1;
    //        punto_actual = new POINT(Datos_GPS.Latitud, longG.ToString());
    //        foreach (Response_spot sp in point)
    //        {
    //            POLYGON pol = new POLYGON(sp.points.ToArray());
    //            if (pol.in_poligone(punto_actual, false))
    //            {
    //                float angle_bus = pol.angulo_orientacion(punto_anterior, punto_actual);
    //                if (in_angle(sp.orientacionInicial, angle_bus, sp.orientacionFinal))
    //                {
    //                    res = sp;
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //    return res;
    //}


    /// <summary>
    /// Proceso crucial del colisionador
    /// </summary>
    private Response_spot DetectarColisiones()
    {
        Response_spot res = null;
        try
        {
            
            punto_anterior = punto_actual;
            Double longG = long_negativo == 1 ? Convert.ToDouble(Datos_GPS.Longitud) * -1 : Convert.ToDouble(Datos_GPS.Longitud);
            punto_actual = new POINT(Datos_GPS.Latitud, longG.ToString());

            //Se agrega esta linea, para evitar que el calculo de angulo de dirección sea erroneo por punto anterior y actual iguales Powered ByRED 25OCT2023
            if (punto_anterior.Latitud == punto_actual.Latitud && punto_anterior.Longitud == punto_actual.Longitud) { return res; }

            if (InDev) { Console.WriteLine("PUNTO ACTUAL: " + Datos_GPS.Latitud + " " + longG); }
            foreach (Response_spot sp in point)
            {
                POLYGON pol = new POLYGON(sp.points.ToArray());
                if (pol.in_poligone(punto_actual, false))
                {
                    if (InDev) { Console.WriteLine("ENTRO: " + sp.parada.clave); }
                    
                    float angle_bus = pol.CalculeAngle(punto_anterior, punto_actual);//Powered ByRED 24SEP2023
                    if (InDev) { Console.WriteLine("ESTA EN EL POLIGONO ANGLE BUS: " + angle_bus); }
                    if (sp.isPuntoDoble)
                    {
                        if (in_angle(sp.orientacionInicial, angle_bus, sp.orientacionFinal))
                        {
                            
                            if (!sp.in_poligone)
                            {
                                if (InDev)
                                {
                                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<**********COLISION EN: " + sp.nombre + "***********>>>>>>>>>>>>>>>>>>>>");
                                    Console.WriteLine("---------->>>>>COORDENADAS: " + Datos_GPS.Latitud + ", " + longG + " EN EL ANGULO: " + angle_bus);
                                }

                                res = sp;
                                sp.in_poligone = true;
                            }
                            break;
                        }
                        else if (in_angle(sp.orientacionInicial2, angle_bus, sp.orientacionFinal2))
                        {
                            //Console.WriteLine("ESTA DENTRO DEL ANGULO PROGRAMADO 2...");
                            if (!sp.in_poligone)
                            {
                                if (InDev)
                                {
                                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<**********COLISION EN: " + sp.punto2.nombre + "***********>>>>>>>>>>>>>>>>>>>>");
                                    Console.WriteLine("---------->>>>>COORDENADAS: " + Datos_GPS.Latitud + ", " + longG + " EN EL ANGULO: " + angle_bus);
                                }

                                res = sp.punto2;
                                sp.in_poligone = true;
                            }
                            break;
                        }
                    }
                    else
                    {                        
                        if (in_angle(sp.orientacionInicial, angle_bus, sp.orientacionFinal))
                        {
                            if (!sp.in_poligone)
                            {
                                if (InDev)
                                {
                                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<**********COLISION EN: " + sp.nombre + "***********>>>>>>>>>>>>>>>>>>>>");
                                    Console.WriteLine("---------->>>>>COORDENADAS: " + Datos_GPS.Latitud + ", " + longG + " EN EL ANGULO: " + angle_bus);
                                }
                                
                                res = sp;
                                sp.in_poligone = true;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    if (sp.in_poligone)
                    {
                        sp.in_poligone = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (InDev)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return res;
    }
    
    /// <summary>
    /// Paso 2: Se encarga de procesar la colisión detectada: Horario, vigencia, etc.
    /// Powered ByRED 24OCT2023
    /// </summary>
    /// <param name="spot"></param>
    private bool Procesar_colision(Response_spot spot)
    {
        Boolean res = false;
        try
        {
            DateTime fecha_actual = DateTime.Now;

            if ((fecha_actual >= spot.fechaVigenciaInicio) && (fecha_actual <= spot.fechaVigenciaFin))
            {
                res = true;
            }
        }
        catch (Exception ex)
        {
            if (InDev) { Console.WriteLine(ex.ToString()); }
        }
        return res;
    }

    /// <summary>
    /// Paso 3: Se encarga de generar el evento para que sea reproducido por VMD
    /// </summary>
    private void GenerarEvento(Response_spot spot)
    {
        WS_Spot ws = new WS_Spot(RutaMaestra);
        foreach (spotListaDetalles x in spot.spotListaDetalles)
        {
            if (verificaCaducidad(x.spotArchivo.fechaVigenciaInicio, x.spotArchivo.fechaVigenciaFin))
            {
                x.reproducir = true;
            }
        }
        listSpots.Add(spot);
    }


    /// <summary>
    /// Powered ByRED 24OCT2023
    /// </summary>
    /// <param name="fi"></param>
    /// <param name="ff"></param>
    /// <returns></returns>
    private bool verificaCaducidad(DateTime fi, DateTime ff)
    {
        Boolean res = false;
        try
        {
            DateTime fecha_actual = DateTime.Now;
            if ((fecha_actual >= fi) && (fecha_actual <= ff))
            {
                res = true;
            }
        }
        catch (Exception ex)
        {
            
        }
        return res;
    }

    /// <summary>
    /// Detecta que el autobus se encuentre dentro del agunlo
    /// Revisado Powered ByRED 24OCT2023
    /// </summary>
    /// <param name="comienzo"></param>
    /// <param name="punto"></param>
    /// <param name="final"></param>
    /// <returns></returns>
    private Boolean in_angle(float comienzo, float punto, float final)
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

    /// <summary>
    /// Se encarga de obtener el nombre y ruta de los archivos que podrán ser reproducidos en spot
    /// </summary>
    private void ObtenerListaSpots()
    {

        /*List<archivo_poi> list_archivo_bd = (from x in POI_BD.archivo_poi select x).ToList();
         foreach (archivo_poi item in list_archivo_bd) {
             String ruta = ParametrosInicioCAN.CarpetaVideos+item.url;
             if (System.IO.File.Exists(ruta))
             {
                 cat_media_disponible media = new cat_media_disponible();
                 media.url = ruta;
                 media.nombre = item.nombre;
                 media.id_spot = item.spotArchivoId;
                 media.status = true;
                 POI_BD.AddTocat_media_disponible(media);
             }else {

             }
         }*/
        POI_BD.SaveChanges();

    }

    #endregion

    #region "Métodos públicos"
    /// <summary>
    /// Se encarga de ir a insertar el testigo del spot que se reprodujo
    /// Powered byToto♫♫
    /// Update Powered ByRED 25OCT2023 || Se adicionan reglas de negocio
    /// </summary>
    /// <param name="listaId"></param>
    /// <param name="secuencia"></param>
    /// <param name="idSPOT"></param>
    public void CrearTestigo(int listaId, int secuencia, int idSPOT)
    { 
        WS_Spot spot = new WS_Spot(RutaMaestra);
        //spot.insert_testigo(listaId, secuencia, idSPOT, ParametrosInicioCAN.Autobus,
          // (float)Convert.ToDouble(Datos_GPS.Latitud), (float)Convert.ToDouble(Datos_GPS.Longitud));
        //Powered ByRED 25OCT2023
        spot.insert_testigo(listaId, secuencia, idSPOT, ParametrosInicioCAN.Autobus,
           (float)Convert.ToDouble(Datos_GPS.Latitud), (float)(long_negativo == 1 ? Convert.ToDouble(Datos_GPS.Longitud) * -1 : Convert.ToDouble(Datos_GPS.Longitud)));

    }
    /// <summary>
    /// Se encarga de pedir el nombre de los Spots
    /// </summary>
    /// <returns></returns>
    public List<cat_media_disponible> ObtenerSpots(String tipo)
    {
        List<cat_media_disponible> ListaSpots = new List<cat_media_disponible>();

        WS_Spot ws = new WS_Spot(RutaMaestra);
        ListaSpots = ws.obtenerSpotsConductor(tipo);
        return ListaSpots;
    }
    #endregion

    #region "Métodos de Heredados"

    /// <summary>
    /// Se encarga de actualizar el programa
    /// </summary>
    public void Actualizar()
    {
        try
        {
            if (EsperandoSync) { return; }

            if (HiloDeteccionColision != null)
            {
                if (!HiloDeteccionColision.IsAlive)
                {
                    HiloDeteccionColision = new Thread(new ThreadStart(Colisionador));
                    HiloDeteccionColision.Start();
                }
            }

        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }


    /// <summary>
    /// Se encarga de inicializar los componentes necesarios del sistema
    /// </summary>
    public void Inicializar()
    {
        CargarCatalogos();
        WS_Spot spot = new WS_Spot(RutaMaestra);
        spot.syncFileSpot();
        PreparaTimersEHilos();

        //DetectarColisiones();
    }

    /// <summary>
    /// Se encarga de finalizar los procesos para un cierre correcto del programa
    /// </summary>
    public void Finalizar()
    {
        DetieneTimersEHilos();
    }

    /// <summary>
    /// Se encarga de sincronizar los testigos y los catálogos de POI
    /// </summary>
    /// <returns></returns>
    public Boolean Sincronizar()
    {
        Boolean res = false;

        EsperandoSync = true;
        try
        {
            EventoSyncPOI("Iniciando SIIAB-POI", 0);

            //Creamos Objeto para sincronizar
            WS_Spot ws = new WS_Spot(RutaMaestra);

            //Paso 1:Sincronizacion de testigos
            res = ws.syncTestigo(ParametrosInicioCAN.Autobus);

            EventoSyncPOI("Sincronizacion Testigos POI: " + (res ? true : false), 0);

            //Paso 2: Sincronización de los catálogos de POI
            Request_spot request = new Request_spot();
            request.numeroEconomico = ParametrosInicioCAN.Autobus;
            request.latitud = (float)Convert.ToDouble(Datos_GPS.Latitud);
            //request.longitud = (float)Convert.ToDouble(Datos_GPS.Longitud);
            request.longitud = (float)(long_negativo == 1 ? Convert.ToDouble(Datos_GPS.Longitud) * -1 : Convert.ToDouble(Datos_GPS.Longitud)); //Powered ByRED 25OCT2023
            res = ws.syncSpots(request);
            EventoSyncPOI("Sincronizacion de POI: " + (res ? true : false), 0);

            //Termino de sincronización
            EventoSyncPOI("Finalizando SIIAB-POI", 0);
        }
        catch (Exception ex)
        {
            EventoSyncPOI("Error: " + ex.ToString(), 0);
        }
        finally { EsperandoSync = false; }
        return res;
    }

    #endregion

    #region "Timers"

    #endregion
}