

using System.Linq;
using InterfazSistema.ModelosBD;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Text;

public class WS : IBDConextCAN2
{
    #region Variables
    //Contexto de bases de datos
    public Can2Entities CAN2_BD { get; set; }
    public telematicsEntities TEL_BD { get; set; }

    //Flags
    public string msgError { get; set; }
    #endregion

    #region Constructores

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public WS()
    {
    }
    /// <summary>
    /// constructor para sincronizar
    /// </summary>
    /// <param name="sync"></param>
    public WS(bool sync)
    {

        CAN2_BD = new Can2Entities();
    }

    #endregion

    #region SYNC TESTIGOS
    /// <summary>
    /// Se encargará de Sincronizar los Testigos
    /// </summary>
    /// <param name="numeroEconomico"></param>
    /// <returns></returns>
    public Boolean syncTestigo(string numeroEconomico)
    {
        Boolean res = false;
        try
        {
            var url = getParam("ws_testigo");

            //Paso 1: Preparamos los registros de testigos
            PrepararTestigos(numeroEconomico);

            //Paso 2: Recuperamos los registros de testigos que no han sido enviados
            List<can2_testigo> request = GetListTestigos(numeroEconomico);
            if (request.Count > 0)
            {
                //Paso 3: Mandamos los testigos hacia el WS
                List<can2_testigo> t = Call_ws_testigo(url, request);
                if (t.Count > 0)
                {
                    //Paso 4: Actualizamos los testigos que fueron enviados a WS
                    updateTestigo(t);
                }
            }


        }
        catch (Exception ex)
        {

            Console.Write(ex.Message);
        }
        return res;


    }
    /// <summary>
    /// Se encarga de recuperar el parametro desde bd
    /// </summary>
    /// <returns></returns>
    public int RecuperarIdParametro(string nombParam)
    {
        var res = 0;

        try
        {
            TEL_BD = new telematicsEntities();

            res = (int)(from x in TEL_BD.cat_codigo where x.Descripcion == nombParam select x.PK_ID).FirstOrDefault();
        }
        catch (Exception)
        {

            throw;
        }
        return res;
    }
    /// <summary>
    /// Se encarga de generar los registros de testigos, de acuerdo a los diversos parametros manejados en ejecucion
    /// por ahora sólo tenemos valor de FR
    /// </summary>
    /// <param name="numeroEconomico"></param>
    /// <returns></returns>
    public int PrepararTestigos(string numeroEconomico)
    {
        int res = 0;
        try
        {
            List<can2_parametros> can2fr = (from x in CAN2_BD.can2_parametros select x).ToList();

            foreach (can2_parametros c in can2fr)
            {
                if ((c.lat_Fin != 0 && c.long_Fin != 0))
                {
                    can2_testigo testigoSubida = new can2_testigo()
                    {
                        numeroEconomico = numeroEconomico,
                        geocercaId = c.geocerca_id,
                        parametroid = c.parametroId,
                        ValorParametro = c.valor_real,
                        fechaEvento = c.Fecha_Fin,
                        latitud = c.lat_Fin,
                        longitud = c.long_Fin,
                        enviado = false
                    };
                    CAN2_BD.can2_testigo.Add(testigoSubida);
                }

            }


            // Guarda los cambios en la base de datos
            CAN2_BD.SaveChanges();

            EjecutarBat("fr");

            return can2fr.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return res;
    }

    /// <summary>
    /// Obtiene los registros de testigos que no han sido enviados
    /// </summary>
    /// <param name="numeroEconomico"></param>
    /// <returns></returns>
    private List<can2_testigo> GetListTestigos(System.String numeroEconomico)
    {
        List<can2_testigo> res = null;
        try
        {

            res = (from x in CAN2_BD.can2_testigo
                   where x.enviado == false
                   orderby x.id_testigo ascending
                   select x).ToList();
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }

    /// <summary>
    /// Se encarga de mandar llamar el WS de Testigos para enviarle la informacion
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private List<can2_testigo> Call_ws_testigo(string url, List<can2_testigo> request)
    {
        List<can2_testigo> res = null;
        try
        {
            List<Testigo_Subida> ts = new List<Testigo_Subida>();
            res = new List<can2_testigo>();


            foreach (var r in request)
            {
                ts.Add(new Testigo_Subida()
                {
                    id_testigo = r.id_testigo,
                    numeroEconomico = r.numeroEconomico,
                    geocercaId = Convert.ToInt32(r.geocercaId),
                    parametroId = Convert.ToInt32(r.parametroid),
                    valorParametro = float.Parse(r.ValorParametro.ToString()),
                    fechaEvento = r.fechaEvento.ToString("yyyy-MM-ddTHH:mm:ss"),
                    latitud = float.Parse(r.latitud.ToString()),
                    longitud = float.Parse(r.longitud.ToString()),
                });
                res.Add(r);
            }

            //hay que mandar el objeto a la web como TS no como res
            //en que proyecto estas papi
            try
            {
                // Crear una instancia de WebClient
                using (WebClient webClient = new WebClient())
                {
                    // Establecer el tipo de contenido en la cabecera de la solicitud
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";


                    string json = JsonConvert.SerializeObject(ts, Formatting.Indented);

                    // Convertir el JSON a un arreglo de bytes para enviarlo
                    byte[] requestBody = Encoding.UTF8.GetBytes(json);

                    // Hacer la solicitud POST y recibir la respuesta en formato byte[]
                    byte[] responseBytes = webClient.UploadData(url, "POST", requestBody);

                    // Convertir la respuesta a string
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    // Procesar la respuesta JSON
                    JObject results = JObject.Parse(responseString);
                    List<int> enviados = new List<int>();

                    foreach (var x in results["data"])
                    {
                        can2_testigo t = null;
                        try
                        {
                            t = new can2_testigo();
                            t = JsonConvert.DeserializeObject<can2_testigo>(x.ToString());
                            res.Add(t);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR CALL WS FORMAT JSON: " + ex.Message + " " + ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine(EX);
                throw;
            }



            // Ruta del archivo en el que quieres guardar el JSON
            //string filePath = "C:/Users/danch/Desktop/TRABAJO/JSONSubida.txt";
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    writer.Write(json);
            //}


            return request;
        }
        catch (Exception ex)
        {
            msgError = "Fallo en la cosulta al WS Testigo. Causa: " + ex.Message;
            Console.Write(ex.Message);
        }
        return res;
    }


    /// <summary>
    /// Metodo que hace update al registro del testigo enviado y registrado con exito en el WS
    /// </summary>
    /// <param name="tlist"></param>
    /// <returns></returns>
    private Boolean updateTestigo(List<can2_testigo> tlist)
    {

        Boolean res = false;
        try
        {
            IList<can2_testigo> t = null;
            foreach (can2_testigo y in tlist)
            {
                t = (from x in CAN2_BD.can2_testigo where x.id_testigo == y.id_testigo select x).ToList();
                foreach (can2_testigo te in t)
                {
                    te.enviado = true;
                }

            }
            CAN2_BD.SaveChanges();
            res = true;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }

    #endregion


    #region SYNC GEOCERCAS


    /// <summary>
    /// proceso de sincronización de las geocercas
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Boolean syncGeocercas(Request_geocerca request)
    {
        Boolean res = false;
        try
        {
            var url = getParam("ws_geocercas");

            //Paso 1:Mandamos a consultar el WS en búsqueda de obtener el catálogo de geocercas
            List<Response_Geocerca> response = Call_ws_geocercas(url, request);
            if (response.Count > 0)
            {
                //Paso 2: Si recibimos geocercas, insertarlas
                res = insert_Geocerca(response);
            }

        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;

    }


    /// <summary>
    /// Se encarga de mandar a llamar el WS de las geocercas, para almacenarla en BD
    /// </summary>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private List<Response_Geocerca> Call_ws_geocercas(string url, Request_geocerca request)
    {
        List<Response_Geocerca> res = new List<Response_Geocerca>();
        List<Request_geocerca> reslist = new List<Request_geocerca>();

        try
        {
            // Crear una instancia de WebClient
            using (WebClient webClient = new WebClient())
            {
                // Establecer el tipo de contenido en la cabecera de la solicitud
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                // Formatear la fecha en el request
                DateTime d = Convert.ToDateTime(request.fecha_modificacion);
                request.fecha_modificacion = d.ToString("yyyy-MM-dd HH:mm:ss");
                reslist.Add(request);
                // Serializar el objeto request a JSON
                string json = JsonConvert.SerializeObject(reslist, Formatting.Indented);

                // Convertir el JSON a un arreglo de bytes para enviarlo
                byte[] requestBody = Encoding.UTF8.GetBytes(json);

                // Hacer la solicitud POST y recibir la respuesta en formato byte[]
                byte[] responseBytes = webClient.UploadData(url, "POST", requestBody);

                // Convertir la respuesta a string
                string responseString = Encoding.UTF8.GetString(responseBytes);

                // Procesar la respuesta JSON
                JArray results = JArray.Parse(responseString);
                Response_Geocerca Geocerca = null;

                foreach (var x in results)
                {
                    try
                    {
                        Geocerca = JsonConvert.DeserializeObject<Response_Geocerca>(x.ToString());
                        res.Add(Geocerca);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR CALL WS FORMAT JSON: " + ex.Message + " " + ex.StackTrace);
                    }
                }
            }
        }
        catch (WebException ex)
        {
            // Manejar errores relacionados con la solicitud web
            Console.WriteLine("Error en la solicitud web: " + ex.Message);
            if (ex.Response != null)
            {
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    string responseError = reader.ReadToEnd();
                    Console.WriteLine("Detalle del error: " + responseError);
                }
            }
        }
        catch (Exception ex)
        {
            // Manejar otros tipos de errores
            Console.WriteLine($"Error: {ex.Message}");
        }

        return res;
    }




    /// <summary>
    /// funcion que recibe como parametros la lista de resultados obtenida del json y la convierte en los distintos objetos 
    /// de la clase Response_Geocoord
    /// </summary>
    /// <param name="response_list"></param>
    /// <returns></returns>
    private bool insert_Geocerca(List<Response_Geocerca> response_list)
    {
        bool res = false;
        try
        {



            foreach (Response_Geocerca response in response_list)
            {

                var geocerca = (from x in CAN2_BD.can2_geocerca
                                where x.geocercaListaId == response.geocercaId
                                select x).FirstOrDefault();

                if (geocerca == null)
                {
                    can2_geocerca parada = new can2_geocerca()
                    {
                        activo = response.activo,
                        clave = response.clave,
                        fechaCreacion = response.fechaCreacion,
                        nombre = response.nombre,
                        geocercaListaId = response.geocercaId
                    };
                    CAN2_BD.can2_geocerca.Add(parada);
                    //CAN2_BD.SaveChanges();


                    foreach (CoordenadasCan2 CoordenadasResponse in response.Coordenadas)
                    {
                        //CoordenadasCan2 CoordenadaParaBD = new CoordenadasCan2();

                        can2_coordenadas can2_coordenadas = new can2_coordenadas();
                        can2_coordenadas.geocercaId = response.geocercaId;
                        can2_coordenadas.sequence = CoordenadasResponse.sequence;
                        can2_coordenadas.active = CoordenadasResponse.active;
                        can2_coordenadas.latitud = CoordenadasResponse.latitud;
                        can2_coordenadas.latitudCan = CoordenadasResponse.latitudCan;
                        can2_coordenadas.longitud = CoordenadasResponse.longitud;
                        can2_coordenadas.longitudCan = CoordenadasResponse.longitudCan;

                        CAN2_BD.can2_coordenadas.Add(can2_coordenadas);

                        //CAN2_BD.SaveChanges();

                    }
                    foreach (geocercaParametros GPResponse in response.geocercaParametros)
                    {
                        can2_geocercaparametros can2_geoparam = new can2_geocercaparametros();
                        //can2_geoparam.Id = response.geocercaId;
                        can2_geoparam.geocercaId = response.geocercaId;
                        can2_geoparam.parametroId = GPResponse.ParametroId;
                        can2_geoparam.nombreParametro = GPResponse.NombreParametro;
                        can2_geoparam.ValorParametro = GPResponse.ValorParametro;
                        can2_geoparam.margenParametro = GPResponse.MargenParametro;
                        can2_geoparam.activo = GPResponse.Activo;
                        can2_geoparam.fechaCreacion = GPResponse.FechaCreacion;
                        can2_geoparam.fechaVigenciaFin = GPResponse.FechaVigenciaFin;
                        can2_geoparam.orientacionInicial = GPResponse.orientacionInicial;
                        can2_geoparam.orientacionFinal = GPResponse.orientacionFinal;
                        CAN2_BD.can2_geocercaparametros.Add(can2_geoparam);
                        //CAN2_BD.SaveChanges();

                    }
                }
                else
                {

                    geocerca.activo = response.activo;
                    geocerca.clave = response.clave;
                    geocerca.fechaCreacion = response.fechaCreacion;

                    List<can2_coordenadas> coordenadaExistente = (from x in CAN2_BD.can2_coordenadas
                                                                  where x.geocercaId == response.geocercaId
                                                                  select x).ToList();
                    foreach (can2_coordenadas item in coordenadaExistente)
                    {
                        CAN2_BD.can2_coordenadas.Remove(item);
                    }
                    CAN2_BD.SaveChanges();
                    foreach (CoordenadasCan2 CoordenadasResponse in response.Coordenadas)
                    {

                        can2_coordenadas can2_coordenadas = new can2_coordenadas();
                        can2_coordenadas.geocercaId = response.geocercaId;
                        can2_coordenadas.sequence = CoordenadasResponse.sequence;
                        can2_coordenadas.active = CoordenadasResponse.active;
                        can2_coordenadas.latitud = CoordenadasResponse.latitud;
                        can2_coordenadas.latitudCan = CoordenadasResponse.latitudCan;
                        can2_coordenadas.longitud = CoordenadasResponse.longitud;
                        can2_coordenadas.longitudCan = CoordenadasResponse.longitudCan;
                        CAN2_BD.can2_coordenadas.Add(can2_coordenadas);

                    }
                    CAN2_BD.SaveChanges();

                    List<can2_geocercaparametros> parametrosExistentes = new List<can2_geocercaparametros>();
                    foreach (geocercaParametros c in response.geocercaParametros)
                    {
                        var cambio = (from x in CAN2_BD.can2_geocercaparametros
                                      where x.geocercaId == response.geocercaId
                                      && x.parametroId == c.ParametroId
                                      select x).FirstOrDefault();
                        if (cambio != null)
                            parametrosExistentes.Add(cambio);
                    }

                    foreach (var item in parametrosExistentes)
                    {
                        CAN2_BD.can2_geocercaparametros.Remove(item);
                    }
                    CAN2_BD.SaveChanges();
                    foreach (geocercaParametros GPResponse in response.geocercaParametros)
                    {
                        can2_geocercaparametros can2_geoparam = new can2_geocercaparametros();
                        //can2_geoparam.Id = response.geocercaId;
                        can2_geoparam.geocercaId = response.geocercaId;
                        can2_geoparam.parametroId = GPResponse.ParametroId;
                        can2_geoparam.nombreParametro = GPResponse.NombreParametro;
                        can2_geoparam.ValorParametro = GPResponse.ValorParametro;
                        can2_geoparam.margenParametro = GPResponse.MargenParametro;
                        can2_geoparam.activo = GPResponse.Activo;
                        can2_geoparam.fechaCreacion = GPResponse.FechaCreacion;
                        can2_geoparam.fechaVigenciaFin = GPResponse.FechaVigenciaFin;
                        can2_geoparam.orientacionInicial = GPResponse.orientacionInicial;
                        can2_geoparam.orientacionFinal = GPResponse.orientacionFinal;
                        CAN2_BD.can2_geocercaparametros.Add(can2_geoparam);
                        //CAN2_BD.SaveChanges();

                    }
                }
            }

            CAN2_BD.SaveChanges();
            EjecutarBat("ID");
            Thread.Sleep(1000);
            CAN2_BD = new Can2Entities();
            res = true;
        }
        catch (Exception ex)
        {

            Console.WriteLine("ERRO insert_spot_db" + ex.Message + " " + ex.StackTrace);
        }
        return res;
    }
    //cambio para creacion de rama
    //esta es la rama main
    #endregion

    /// <summary>
    /// Se encarga de recuperar los parametros que se requieren
    /// </summary>
    /// <param name="cve_param"></param>
    /// <returns></returns>
    private string getParam(System.String cve_param)
    {
        string res = null;
        TEL_BD = new telematicsEntities();
        try
        {
            res = (from x in TEL_BD.parametro
                   where x.cve_parametro == cve_param && x.estatus == 1
                   select x.valor_parametro).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
        return res;
    }


    private bool RealizarPingV2(string server)
    {
        try
        {
            Ping MyPing = new Ping();

            //Verificamos la respuesta del Servidor
            if (MyPing.Send(server, 10).Status == IPStatus.Success) { return true; } else { return false; }

        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            return false;
        }
    }

    private void sincronizarInternet()
    {
        string pingvalue = (from x in TEL_BD.parametro where x.cve_parametro == "ping_server" select x.valor_parametro).FirstOrDefault();

        if (RealizarPingV2(pingvalue))
        {

        }
    }

    /// <summary>
    /// Se encarga de mandar a ejecutar un .bat para truncar la table de
    /// Cat_codigos, porque por la versión de framework y entity, no es posible
    /// ocupar el contexto de la base de datos para mandar a truncar la tabla
    /// </summary>
    private void EjecutarBat(string nombre_BAT)
    {
        try
        {

            Process Truncar = new Process();
            Truncar.StartInfo.UseShellExecute = false;
            Truncar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Truncar.StartInfo.RedirectStandardOutput = true;
            Truncar.StartInfo.CreateNoWindow = true;

            string RutaBAT = AppDomain.CurrentDomain.BaseDirectory + @"Archivos\" + nombre_BAT + ".bat";

            if (System.IO.File.Exists(RutaBAT))
            {
                Truncar.StartInfo.FileName = RutaBAT;

                Truncar.Start();

                //EnviarMensaje("Truncando tabla: " + nombre_BAT);
            }
            else
            {
                //EnviarMensaje("No se encuentra el BAT en: " + RutaBAT);
            }
        }
        catch
        {
            //EnviarMensaje("Error al querer ejecutar el BAT de trunqueo para: " + nombre_BAT);
        }
    }


}
