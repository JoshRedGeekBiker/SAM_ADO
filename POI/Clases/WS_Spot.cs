using InterfazSistema.ModelosBD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

class WS_Spot
{
    //Contexto de base de datos
    public poiEntities POI_BD { get; set; }
    //Flags
    public Boolean isError { get; set; }
    public String msgError { get; set; }
    //Constructor principal
    public WS_Spot()
    {
        POI_BD = new poiEntities();
        isError = false;
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
            msgError = "No fue posible recuperar los paramtros de configuracion de POI. Causa: " + ex.Message;
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
            List<testigo_poi> tes = (from x in POI_BD.testigo_poi where x.enviado == false orderby x.id_testigo_poi ascending select x).ToList();
            foreach (testigo_poi x in tes)
            {
                t = new Testigo();
                //String id = x.detalles_poiReference.EntityKey.EntityKeyValues[0].Value.ToString();
                //String id = x.detalles_poiReference.EntityKey.EntityKeyValues[0].Value.ToString();
                String id = "";
                t.spotId = Convert.ToInt32(id);
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
                long id = (long)y.spotId;
                t = (from x in POI_BD.testigo_poi where x.detalles_poi.spotId == id select x).FirstOrDefault();
                t.enviado = true;
            }
            res = POI_BD.SaveChanges() > 0;
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
            //POI_BD.AddTotestigo_poi(t);
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
                        updateTestigo(t);
                    }
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
            List<detalles_poi> det = (from x in POI_BD.detalles_poi where x.activo == true orderby x.spotId ascending select x).ToList();
            Response_spot spot = null;
            Parada p = null;
            Coordenadas c = null;
            foreach (detalles_poi a in det)
            {
                spot = new Response_spot();
                spot.spotId = (int)a.spotId;
                spot.orientacionInicial = (int)a.orientacionInicial;
                spot.orientacionFinal = (int)a.orientacionFinal;
                spot.programacionHoraria = a.programacionHoraria;
                spot.duracion = (int)a.duracion;
                spot.tiempoEspera = (int)a.tiempoEspera;
                spot.activo = a.activo;
                spot.fechaCreacion = a.fechaCreacion;
                cat_poi cat = (from y in POI_BD.cat_poi where y.detalles_poi.spotId == a.spotId && y.activo == true orderby y.id_cat_poi ascending select y).FirstOrDefault();
                p = new Parada();
                p.paradaId = (int)cat.paradaId;
                p.nombre = cat.nombre;
                p.activo = cat.activo;
                p.clave = cat.clave;
                p.fechaCreacion = cat.fechaCreacion;
                spot.parada = p;
                List<coor_polygon> cor = (from w in POI_BD.coor_polygon where w.cat_poi.id_cat_poi == cat.id_cat_poi && w.active == true orderby w.sequence ascending select w).ToList();
                List<Coordenadas> listC = new List<Coordenadas>();
                foreach (coor_polygon co in cor)
                {
                    c = new Coordenadas();
                    c.paradaId = (int)co.paradaId;
                    c.active = co.active;
                    c.latitud = co.latitud;
                    c.longitud = co.longitud;
                    c.sequence = (int)co.sequence;
                    listC.Add(c);
                }

                spot.parada.coordenadas = listC;
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
            if (!isError)
            {
                List<Response_spot> response = call_ws_spot(url, request);
                if (response.Count > 0)
                {
                    res = insert_spot_db(response);
                }
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
                    spot = new Response_spot();
                    spot = JsonConvert.DeserializeObject<Response_spot>(x.ToString());
                    res.Add(spot);
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
        int registros = 0;
        try
        {
            EjecutarBatTruncate();
            POI_BD = new poiEntities();
            foreach (Response_spot response in response_list)
            {
                //registros = (from x in POI_BD.detalles_poi where x.spotId == response.spotId select x).Count();
                // if (registros == 0)
                //{
                detalles_poi det = new detalles_poi();
                det.spotId = response.spotId;
                det.orientacionInicial = response.orientacionInicial;
                det.orientacionFinal = response.orientacionFinal;
                det.programacionHoraria = response.programacionHoraria;
                det.duracion = response.duracion;
                det.tiempoEspera = response.tiempoEspera;
                det.activo = response.activo;
                det.fechaCreacion = response.fechaCreacion;
                //POI_BD.AddTodetalles_poi(det);
                POI_BD.detalles_poi.Add(det);
                cat_poi cat = new cat_poi();
                cat.detalles_poi = det;
                cat.paradaId = response.parada.paradaId;
                cat.nombre = response.parada.nombre;
                cat.activo = response.parada.activo;
                cat.clave = response.parada.clave;
                cat.fechaCreacion = response.parada.fechaCreacion;
                POI_BD.cat_poi.Add(cat);
                POI_BD.SaveChanges();
                archivo_poi archivo = new archivo_poi();
                archivo.detalles_poi = det;
                archivo.spotArchivoId = response.archivo.spotArchivoId;
                archivo.nombre = response.archivo.nombre;
                archivo.url = response.archivo.url;
                archivo.activo = response.archivo.activo;
                //POI_BD.AddToarchivo_poi(archivo);
                POI_BD.archivo_poi.Add(archivo);
                POI_BD.SaveChanges();
                tipo_archivo_poi tipo = new tipo_archivo_poi();
                tipo.archivo_poi = archivo;
                tipo.spotArchivoTipoId = response.archivo.tipo.spotArchivoTipoId;
                tipo.descripcion = response.archivo.tipo.descripcion;
                tipo.activo = response.archivo.tipo.activo;
                //POI_BD.AddTotipo_archivo_poi(tipo);
                POI_BD.tipo_archivo_poi.Add(tipo);
                POI_BD.SaveChanges();
                coor_polygon coor = null;
                foreach (Coordenadas x in response.parada.coordenadas)
                {
                    coor = new coor_polygon();
                    coor.cat_poi = cat;
                    coor.paradaId = x.paradaId;
                    coor.sequence = x.sequence;
                    coor.active = x.active;
                    coor.latitud = Utilidades.ConvertirCordenadasCANaDecimalLatitud(x.latitud);
                    coor.longitud = Utilidades.ConvertirCordenadasCANaDecimalLongitud(x.longitud);
                    POI_BD.coor_polygon.Add(coor);
                    //POI_BD.AddTocoor_polygon(coor);
                    coor = null;
                }
                res = POI_BD.SaveChanges() > 0;
                /* }
                 else
                 {
                     res = true;
                 }*/
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

    private void EjecutarBatTruncate()
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
}

