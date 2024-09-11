using InterfazSistema.ModelosBD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

/// <summary>
/// Jose Miguel Olguin CTC 2022 
/// </summary>
public class WS_Spot
{
    //Contexto de base de datos
    public poiEntities POI_BD { get; set; }
    private Dictionary<String, String> type_archivo = null;
    //Flags
    public Boolean isError { get; set; }
    public String msgError { get; set; }
    List<spotArchivo> response_manual = null;
    private String rutaMaster = "";
    //Constructor principal
    public WS_Spot(String ruta)
    {
        POI_BD = new poiEntities();
        isError = false;
        rutaMaster = ruta;
        getTypesAr();
    }
    /// <summary>
    /// Metodo para obtener de base de datos las url de la tabla de paramtros POI
    /// </summary>
    /// <param name="cve_param"></param>
    /// <returns></returns>
    public parametros_poi getParam(String cve_param)
    {
        parametros_poi res = null;
        try
        {
            res = (from x in POI_BD.parametros_poi where x.cve_parametro == cve_param && x.estatus == 1 select x).FirstOrDefault();
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "No fue posible recuperar los parametros de configuracion de POI. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;
    }
    /// <summary>
    /// Metodo que se encarga de de otener la lista de testigos registrados en base de datos con respecto a las coliciones 
    /// </summary>
    /// <param name="numeroEconomico"></param>
    /// <returns></returns>
    public List<Testigo> getListTestigos(String numeroEconomico)
    {
        List<Testigo> res = null;
        try
        {
            res = new List<Testigo>();
            Testigo t = null;
            List<testigo_poi> tes = (from x in POI_BD.testigo_poi where x.enviado == false && x.latitud != 0 orderby x.id_testigo ascending select x).ToList();
            foreach (testigo_poi x in tes)
            {
                t = new Testigo();
                t.spotListaId = (int)x.spotListaId;
                t.spotListaSecuencia = (int)x.spotListaSecuencia;
                t.spotArchivoId = (int)x.spotArchivoId;
                t.numeroEconomico = numeroEconomico;
                t.fechaReproduccion = x.fechaReproduccion;
                t.latitud = x.latitud;
                t.longitud = x.longitud;
                res.Add(t);
            }
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }
    /// <summary>
    /// Metodo que hace update al registro del testigo enviado y registrado con exito en el WS
    /// </summary>
    /// <param name="tlist"></param>
    /// <returns></returns>
    public Boolean updateTestigo(List<Testigo> tlist)
    {
        Boolean res = false;
        
        try
        {
            testigo_poi t = null;
            foreach (Testigo y in tlist)
            {
                t = (from x in POI_BD.testigo_poi where x.spotListaId == y.spotListaId && x.enviado == false select x).FirstOrDefault();
                t.enviado = true;
            }

            //Powered ByRED 25OCT2023
            if (POI_BD.SaveChanges() > 0) { res = true; }
            
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }


    /// <summary>
    /// Metodo que hace update al registro del testigo enviado y registrado con exito en el WS
    /// Powered ByRED 25OCT2023
    /// </summary>
    /// <param name="tlist"></param>
    /// <returns></returns>
    public Boolean updateTestigo2(List<Testigo> tlist)
    {
        Boolean res = false;

        try
        {
            List<testigo_poi> t_list = (from x in POI_BD.testigo_poi where x.enviado == false && x.latitud != 0 select x).ToList();
            testigo_poi t = null;
            foreach (Testigo y in tlist)
            {
                t = (from x in t_list where x.spotListaId == y.spotListaId && x.enviado == false && x.latitud != 0 select x).FirstOrDefault();
                t.enviado = true;
            }


            var cuenta = POI_BD.SaveChanges();

            if(cuenta > 0) { res = true; }


            //Powered ByRED 25OCT2023
            //if (POI_BD.SaveChanges() > 0) { res = true; }

        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }


    /// <summary>
    /// Metodo que se encarga de insertar en base de datos los testigos de colicion con puntos de interes
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Boolean insertTestigos(testigo_poi t)
    {
        Boolean res = false;
        try
        {
            POI_BD.testigo_poi.Add(t);
            res = POI_BD.SaveChanges() > 0;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }

    /// <summary>
    /// Metodo que se encarga de comunicarse con el web service donde se envian los testigos.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public List<Testigo> call_ws_testigo(String url, List<Testigo> request)
    {
        List<Testigo> res = null;
        try
        {
            res = new List<Testigo>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(request));
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JObject results = JObject.Parse(result);
                Testigo t = null;
                foreach (var x in results["data"])
                {
                    t = new Testigo();
                    t = JsonConvert.DeserializeObject<Testigo>(x.ToString());
                    res.Add(t);
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "Fallo en la cosulta al WS Testigo. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;
    }
    /// <summary>
    /// Metodo que se encarga de obtener la lista de testigos y enviarlas al ws de testigos.
    /// </summary>
    /// <param name="numeroEconomico"></param>
    /// <returns></returns>
    public Boolean syncTestigo(String numeroEconomico)
    {
        Boolean res = false;
        try
        {
            String url = getParam("ws_testigo").valor_parametro;
            if (!isError)
            {
                List<Testigo> request = getListTestigos(numeroEconomico);
                if (request.Count > 0)
                {
                    List<Testigo> t = call_ws_testigo(url, request);
                    if (t.Count > 0)
                    {
                        res = updateTestigo2(t);//Powered ByRED 25OCT2023
                    }
                }
                else
                {
                    //Para que no de la sensación de que hubo algun error en el proceso cuando no hay testigos por subir
                    //Powered ByRED 25OCT2023
                    res = true;
                }

            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError += "No fue posible sincronizar POI TESTIGO. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;

    }
    /// <summary>
    /// Metodo para obtener la lista de todos los puntos de interes registrados en base de datos
    /// </summary>
    /// <returns></returns>
    public List<Response_spot> getListSpots()
    {
        List<Response_spot> res = null;
        try
        {
            res = new List<Response_spot>();
            DateTime hoy = DateTime.Now;
            List<spot_poi> det = (from x in POI_BD.spot_poi where x.activo == true && (hoy >= x.fechaVigenciaInicio && hoy <= x.fechaVigenciaFin) orderby x.spotListaId ascending select x).ToList();
            Response_spot spot = null;
            Parada p = null;
            Coordenadas c = null;
            foreach (spot_poi a in det)
            {
                spot = new Response_spot();
                spot.spotListaId = (int)a.spotListaId;
                spot.orientacionInicial = (int)a.orientacionInicial;
                spot.orientacionFinal = (int)a.orientacionFinal;
                spot.activo = a.activo;
                spot.fechaCreacion = a.fechaCreacion;
                spot.fechaVigenciaInicio = a.fechaVigenciaInicio;
                spot.fechaVigenciaFin = a.fechaVigenciaFin;
                spot.nombre = a.nombre;
                parada_poi cat = (from y in POI_BD.parada_poi where y.spotListaId == a.spotListaId && y.activo == true orderby y.paradaId ascending select y).FirstOrDefault();
                p = new Parada();
                p.paradaId = (int)cat.paradaId;
                p.spotListaId = (int)cat.spotListaId;
                p.nombre = cat.nombre;
                p.activo = cat.activo;
                p.clave = cat.clave;
                p.fechaCreacion = cat.fechaCreacion;
                spot.parada = p;
                List<coordenadas_poi> cor = (from w in POI_BD.coordenadas_poi where w.paradaId == cat.spotListaId && w.active == true orderby w.sequence ascending select w).ToList();
                List<Coordenadas> listC = new List<Coordenadas>();
                List<POINT> lPoint = new List<POINT>();
                foreach (coordenadas_poi co in cor)
                {
                    c = new Coordenadas();
                    c.paradaId = (int)co.paradaId;
                    c.active = co.active;
                    c.latitud = co.latitud;
                    c.longitud = co.longitud;
                    c.latitudCan = co.latitudCan;
                    c.longitudCan = co.longitudCan;
                    c.sequence = (int)co.sequence;
                    lPoint.Add(new POINT(c.latitudCan, c.longitudCan, c.sequence));
                    listC.Add(c);
                    spot.latitudes += co.latitud;
                    spot.longitudes += co.longitud;
                }
                spot.parada.coordenadas = listC;
                spot.points = lPoint;
                List<spotlista_poi> Lspots = (from w in POI_BD.spotlista_poi where w.spotListaId == spot.spotListaId && w.activo == true orderby w.secuencia ascending select w).ToList();
                List<spotListaDetalles> spotsList = new List<spotListaDetalles>();
                spotListaDetalles item = null;
                spotArchivo archivo = null;
                Tipo tp = null;
                foreach (spotlista_poi s in Lspots)
                {
                    item = new spotListaDetalles();
                    item.secuencia = (int)s.secuencia;
                    archivo = new spotArchivo();
                    archivo.spotArchivoId = (int) s.spotArchivoId;
                    archivo.nombre = s.nombre;
                    archivo.url = s.url;
                    archivo.activo = s.activo;
                    archivo.fechaVigenciaInicio = s.fechaVigenciaInicio;
                    archivo.fechaVigenciaFin = s.fechaVigenciaFin;
                    item.spotArchivo = archivo;
                    tp = new Tipo();
                    tp.spotArchivoTipoId = (int)s.spotArchivoTipoId;
                    tp.descripcion = s.descripcion;
                    item.spotArchivo.tipo = tp;
                    spotsList.Add(item);
                }
                spot.spotListaDetalles = spotsList;
                res.Add(spot);
            }

        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }

    /// <summary>
    /// Metodo para obtener los puntos de interes del ws de Puntos de interes
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Boolean syncSpots(Request_spot request)
    {
        Boolean res = false;
        try
        {
            String url = getParam("ws_spots").valor_parametro;
            String url_manual = getParam("ws_spots_manual").valor_parametro;
            //Console.WriteLine("INICIANDO SINCRONIZACION CON WS...");
            if (!isError)
            {
                List<Response_spot> response = call_ws_spot(url, request);
                // Console.WriteLine("SE RECIBIERON :"+response.Count);
                response_manual = call_ws_spot_manual(url_manual, request);
                if (response.Count > 0)
                {
                    res = insert_spot_db(response);
                }
                syncFileSpot();
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError += "No fue posible sincronizar POI. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;

    }
    /// <summary>
    /// Metodo que se encarga de comunicarse con el ws y obtener todos los puntos manuales 
    /// agegados al autobus
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public List<spotArchivo> call_ws_spot_manual(String url, Request_spot request)
    {
        List<spotArchivo> res = null;
        try
        {
            res = new List<spotArchivo>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(request));
            }
            //Console.WriteLine("PETICION DE SERVICIO CREADA...");
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //Console.WriteLine("RECIBIENDO RESPUESTA...");
                var result = streamReader.ReadToEnd();
                //Console.WriteLine("RESPUESTA OBTENIDA: "+result);
                JObject results = JObject.Parse(result);
                spotArchivo spot = null;
                foreach (var x in results["data"])
                {
                    try
                    {
                        spot = new spotArchivo();
                        spot = JsonConvert.DeserializeObject<spotArchivo>(x.ToString());
                        spot.tipo_spot = 1;
                        spot.url = rutaMaster + spot.url.Replace("/", "\\");
                        res.Add(spot);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERRO call_ws_spot_manual FORMAT JSON" + ex.Message + " " + ex.StackTrace);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "Fallo en la cosulta al WS SPOT MANUAL. Causa: " + ex.Message;
            Console.WriteLine("ERROR CALL WS" + ex.Message + " " + ex.StackTrace);
        }
        return res;
    }
    /// <summary>
    /// Metodo que se encarga de comunicarse con el ws y obtener todos los puntos de interes 
    /// agegados al autobus
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public List<Response_spot> call_ws_spot(String url, Request_spot request)
    {
        List<Response_spot> res = null;
        try
        {
            res = new List<Response_spot>();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(request));
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JObject results = JObject.Parse(result);
                Response_spot spot = null;
                foreach (var x in results["data"])
                {
                    try
                    {
                        spot = new Response_spot();
                        spot = JsonConvert.DeserializeObject<Response_spot>(x.ToString());
                        res.Add(spot);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "Fallo en la cosulta al WS SPOT. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;
    }
    /// <summary>
    /// Metodo que se encarga de insertar todo el detalle de puntos de interes en BD
    /// </summary>
    /// <param name="response_list"></param>
    /// <returns></returns>
    public bool insert_spot_db(List<Response_spot> response_list)
    {
        bool res = false;
        try
        {
            EjecutarBatTruncate(1);
            //Console.WriteLine("TRUNCATE EJECUTADO");
            Thread.Sleep(1000);
            POI_BD = new poiEntities();
            foreach (Response_spot response in response_list)
            {
                spot_poi det = new spot_poi();
                det.spotListaId = response.spotListaId;
                det.orientacionInicial = response.orientacionInicial;
                det.orientacionFinal = response.orientacionFinal;
                det.nombre = response.nombre;
                det.activo = response.activo;
                det.fechaCreacion = response.fechaCreacion;
                det.fechaVigenciaInicio = response.fechaVigenciaInicio;
                det.fechaVigenciaFin = response.fechaVigenciaFin;
                POI_BD.spot_poi.Add(det);
                parada_poi cat = new parada_poi();
                cat.spotListaId = response.spotListaId;
                cat.paradaId = response.parada.paradaId;
                cat.nombre = response.parada.nombre;
                cat.activo = response.parada.activo;
                cat.clave = response.parada.clave;
                cat.fechaCreacion = response.parada.fechaCreacion;
                POI_BD.parada_poi.Add(cat);
                POI_BD.SaveChanges();
                spotlista_poi archivo = null;
                cat_media_disponible media = null;
                foreach (spotListaDetalles x in response.spotListaDetalles)
                {
                    archivo = new spotlista_poi();
                    archivo.spotListaId = response.spotListaId;
                    archivo.secuencia = x.secuencia;
                    archivo.spotArchivoId = x.spotArchivo.spotArchivoId;
                    archivo.nombre = x.spotArchivo.nombre;
                    archivo.url = rutaMaster + x.spotArchivo.url.Replace("/", "\\");
                    archivo.activo = x.spotArchivo.activo;
                    archivo.fechaVigenciaInicio = x.spotArchivo.fechaVigenciaInicio;
                    archivo.fechaVigenciaFin = x.spotArchivo.fechaVigenciaFin;
                    archivo.descripcion = x.spotArchivo.tipo.descripcion;
                    archivo.activo_t = x.spotArchivo.tipo.activo;
                    archivo.spotArchivoTipoId = x.spotArchivo.tipo.spotArchivoTipoId;
                    /*Boolean existe = (from c in POI_BD.cat_media_disponible where c.url == archivo.url select c).Count() > 0;
                    if (!existe)
                    {
                        media = new cat_media_disponible();
                        media.spotListaId = response.spotListaId;
                        media.spotArchivoId = x.spotArchivo.spotArchivoId;
                        media.url = rutaMaster + x.spotArchivo.url.Replace("/", "\\");
                        media.nombre = x.spotArchivo.nombre;
                        media.type = x.spotArchivo.tipo.descripcion;
                        media.status = true;
                        POI_BD.cat_media_disponible.Add(media);
                    }*/
                    POI_BD.spotlista_poi.Add(archivo);
                }
                POI_BD.SaveChanges();
                coordenadas_poi coor = null;
                foreach (Coordenadas x in response.parada.coordenadas)
                {
                    coor = new coordenadas_poi();
                    coor.paradaId = response.spotListaId;
                    coor.sequence = x.sequence;
                    coor.active = x.active;
                    coor.latitud = x.latitud;
                    coor.longitud = x.longitud;
                    coor.latitudCan = x.latitudCan;
                    coor.longitudCan = x.longitudCan;
                    POI_BD.coordenadas_poi.Add(coor);
                    coor = null;
                }
                res = POI_BD.SaveChanges() > 0;
            }
            if (response_manual.Count > 0)
            {
                spot_manual item = null;
                foreach (spotArchivo sm in response_manual)
                {
                    item = new spot_manual();
                    item.url = sm.url;
                    item.nombre = sm.nombre;
                    item.status = sm.activo;
                    item.spotArchivoId = sm.spotArchivoId;
                    item.type = "video";
                    item.fechaVigenciaInicio = sm.fechaVigenciaInicio;
                    item.fechaVigenciaFin = sm.fechaVigenciaFin;
                    POI_BD.spot_manual.Add(item);
                }
                res = POI_BD.SaveChanges() > 0;
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "Fallo en la insercion DB POI. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;
    }

    public void EjecutarBatTruncate(int tipo)
    {
        try
        {

            Process Truncar = new Process();
            Truncar.StartInfo.UseShellExecute = false;
            Truncar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Truncar.StartInfo.RedirectStandardOutput = true;
            Truncar.StartInfo.CreateNoWindow = true;

            string RutaBAT = AppDomain.CurrentDomain.BaseDirectory + (tipo == 1 ? @"Archivo\detalles.bat" : @"Archivo\media_cat.bat");

            if (System.IO.File.Exists(RutaBAT))
            {
                Truncar.StartInfo.FileName = RutaBAT;

                Truncar.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERRO EJECUTA TRUNCATE" + ex.Message + " " + ex.StackTrace);
        }
    }

    public void EjecutarBatTruncate()
    {
        try
        {

            Process Truncar = new Process();
            Truncar.StartInfo.UseShellExecute = false;
            Truncar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Truncar.StartInfo.RedirectStandardOutput = true;
            Truncar.StartInfo.CreateNoWindow = true;

            string RutaBAT = AppDomain.CurrentDomain.BaseDirectory + @"Archivo\detalles.bat";

            if (System.IO.File.Exists(RutaBAT))
            {
                Truncar.StartInfo.FileName = RutaBAT;

                Truncar.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public Boolean insert_lanzador(int id_spot, int spotListaSecuencia, int spotArchivoId)
    {
        Boolean res = false;
        try
        {
            lanzador_spot evento = new lanzador_spot();
            evento.spotListaId = id_spot;
            evento.spotListaSecuencia = spotListaSecuencia;
            evento.spotArchivoId = spotArchivoId;
            POI_BD.lanzador_spot.Add(evento);
            POI_BD.SaveChanges();
            res = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }
    public Boolean insert_testigo(int id_spot, int spotListaSecuencia, int spotArchivoId, string economico,
        float latitud, float longitud)
    {
        Boolean res = false;
        try
        {
            testigo_poi evento = new testigo_poi();
            evento.spotListaId = id_spot;
            evento.spotListaSecuencia = spotListaSecuencia;
            evento.spotArchivoId = spotArchivoId;
            evento.enviado = false;
            evento.latitud = latitud;
            evento.longitud = longitud;
            evento.numeroEconomico = economico;
            evento.fechaReproduccion = DateTime.Now;
            POI_BD.testigo_poi.Add(evento);
            POI_BD.SaveChanges();
            delete_lanzador_testigo(id_spot, spotListaSecuencia, spotArchivoId);
            res = true;
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }
    public void syncFileSpot()
    {
        cat_media_disponible m = null;
        try
        {
            //Truncar tabla cat_media_disponible
            EjecutarBatTruncate(2);
            Thread.Sleep(5000);
            POI_BD = new poiEntities();
            List<spotlista_poi> media = (from x in POI_BD.spotlista_poi where x.activo == true select x).ToList();
            List<spot_manual> media_manual = (from x in POI_BD.spot_manual where x.status == true select x).ToList();
            if (media_manual.Count > 0)
            {
                foreach (spot_manual x in media_manual)
                {
                    if (verificar_caducidad(x))
                    {
                        Boolean existe = (from c in POI_BD.cat_media_disponible where c.spotArchivoId == x.spotArchivoId select c).Count() > 0;
                        //Console.WriteLine("EVALUANDO: " + x.url+" EVALUADO: "+existe);
                        if (!existe)
                        {
                            if (System.IO.File.Exists(x.url))
                            {
                                //Console.WriteLine("INSERTANDO: " + x.url);
                                m = new cat_media_disponible();
                                m.spotListaId = 1;
                                m.spotArchivoId = x.spotArchivoId;
                                m.url = x.url;
                                m.nombre = x.nombre;
                                m.tipo_spot = 1;
                                m.type = x.type;
                                m.status = true;
                                POI_BD.cat_media_disponible.Add(m);
                                POI_BD.SaveChanges();
                            }
                        }
                        else
                        {
                            if (!System.IO.File.Exists(x.url))
                            {
                                //Console.WriteLine("NO EXISTE: "+x.url);
                                m = new cat_media_disponible();
                                m.spotListaId = 1;
                                m.spotArchivoId = x.spotArchivoId;
                                m.url = x.url;
                                m.nombre = x.nombre;
                                m.tipo_spot = 1;
                                m.type = x.type;
                                m.status = true;
                                //POI_BD.cat_media_disponible.Context.DeleteObject(m);
                                POI_BD.cat_media_disponible.Remove(m);
                                POI_BD.SaveChanges();
                            }
                        }
                    }
                }
            }
            //Console.WriteLine("---------evaluando: "+media.Count+"-----------");
            foreach (spotlista_poi x in media)
            {
                Boolean existe = (from c in POI_BD.cat_media_disponible where c.spotArchivoId == x.spotArchivoId select c).Count() > 0;
                //Console.WriteLine("EVALUANDO: " + x.url+" EVALUADO: "+existe);
                if (!existe)
                {
                    if (System.IO.File.Exists(x.url))
                    {
                        //Console.WriteLine("INSERTANDO: " + x.url);
                        m = new cat_media_disponible();
                        m.spotListaId = x.spotListaId;
                        m.spotArchivoId = x.spotArchivoId;
                        m.url = rutaMaster + x.url.Replace("/", "\\");
                        m.nombre = x.nombre;
                        m.type = whatIsFile(x.descripcion);
                        m.status = true;
                        //POI_BD.AddTocat_media_disponible(m);
                        POI_BD.cat_media_disponible.Add(m);
                        POI_BD.SaveChanges();
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(x.url))
                    {
                        //Console.WriteLine("NO EXISTE: "+x.url);
                        m = new cat_media_disponible();
                        m.spotListaId = x.spotListaId;
                        m.spotArchivoId = x.spotArchivoId;
                        m.url = x.url;
                        // m.url = rutaMaster + x.url.Replace("/", "\\");
                        m.nombre = x.nombre;
                        m.type = whatIsFile(x.descripcion);
                        m.status = true;
                        POI_BD.cat_media_disponible.Remove(m);
                        POI_BD.SaveChanges();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public String whatIsFile(String file)
    {
        String res = "";
        foreach (KeyValuePair<string, string> kvp in type_archivo)
        {
            String[] tipos = kvp.Value.Split(',');
            if (tipos.Contains(file))
            {
                res = kvp.Key;
            }
        }
        return res;
    }

    public List<cat_media_disponible> obtenerSpotsConductor(String tipo)
    {
        List<cat_media_disponible> spotL = null;
        try
        {
            List<cat_media_disponible> media = (from x in POI_BD.cat_media_disponible where x.status == true & x.tipo_spot == 1 select x).ToList();
            spotL = new List<cat_media_disponible>();
            if (media.Count > 0)
            {
                foreach (cat_media_disponible x in media)
                {
                    if (x.type == tipo)
                    {
                        spotL.Add(x);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return spotL;
    }
    public Boolean delete_lanzador(String numeroEconomico)
    {
        Boolean res = false;
        try
        {
            testigo_poi testigo = null;
            List<lanzador_spot> lanzador = (from x in POI_BD.lanzador_spot where x.status == true select x).ToList();
            foreach (lanzador_spot x in lanzador)
            {
                testigo = new testigo_poi();
                testigo.numeroEconomico = numeroEconomico;
                testigo.spotListaId = x.spotListaId;
                testigo.spotListaSecuencia = x.spotListaSecuencia;
                testigo.spotArchivoId = x.spotArchivoId;
                testigo.fechaReproduccion = x.fecha_lanzamiento;
                testigo.latitud = x.latitud;
                testigo.longitud = x.longitud;
                POI_BD.testigo_poi.Add(testigo);
                POI_BD.lanzador_spot.Remove(x);
            }
            POI_BD.SaveChanges();
            res = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }

    public Boolean delete_lanzador_testigo(int spotListaID, int spotListaSecuencia, int spotArchivoID)
    {
        Boolean res = false;
        try
        {
            lanzador_spot lanzador= (from x in POI_BD.lanzador_spot where x.spotListaId == spotListaID & x.spotListaSecuencia == spotListaSecuencia & x.spotArchivoId == spotArchivoID select x).FirstOrDefault();
            POI_BD.lanzador_spot.Attach(lanzador);
            POI_BD.lanzador_spot.Remove(lanzador);
            POI_BD.SaveChanges();
            res = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }
    public void getTypesAr()
    {
        type_archivo = new Dictionary<string, string>();
        List<parametros_poi> res = null;
        try
        {
            res = (from x in POI_BD.parametros_poi where (x.id_parametro_poi != 1 || x.id_parametro_poi != 2) select x).ToList();
            foreach (parametros_poi p in res)
            {
                type_archivo.Add(p.cve_parametro, p.valor_parametro);
            }
        }
        catch (Exception ex)
        {
            isError = true;
            msgError = "No fue posible recuperar los paramtros de configuracion de POI. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
    }
    /// <summary>
    /// Se encarga de verificar la caducidad el spot manual
    /// </summary>
    /// <param name="spot"></param>
    private bool verificar_caducidad(spot_manual spot)
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
            Console.WriteLine(ex.Message);
        }
        return res;
    }

}
