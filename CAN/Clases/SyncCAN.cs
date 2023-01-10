using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.Threading;
using Ionic.Zip;
using InterfazSistema.WSCAN;

public class SyncCAN : IBDContext
{
    public vmdEntities VMD_BD { get; }
    can_parametrosinicio ParametrosInicio;

    //Constantes
    private int timeOutComand = 600;
    private int TiempoEsperaMensajes = 500;

    //Para el progreso de la sincronización hacia Front
    public delegate void ActualizaSync(string mensaje, int final);
    public event ActualizaSync EventoSyncCANProgreso;

    public string CodigoDescarga { get; set; } = string.Empty;

    public string FalloCodigoDescarga = "Sin Registros Que Descargar";

    //public SyncCAN(int _versionServer, string _versionesSistemas, string _IpActual, ref Globales _Globales)
    public SyncCAN()
    {
        VMD_BD = new vmdEntities();
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();

    }

    /// <summary>
    /// Sirve para Contar y flajear los movtosCAN para la sincronización
    /// </summary>
    /// <returns></returns>
    public int MovtosCANMovil()
    {
        int NumReg = 0;

        int ultimoMovTosCan = (from x in VMD_BD.can_movtoscan
                               orderby x.NumReg descending
                               select x.NumReg).FirstOrDefault();

        if (ultimoMovTosCan > 0)
        {
            var movTosCAN = (from x in VMD_BD.can_movtoscan
                             where x.NumReg < ultimoMovTosCan
                             select x).ToList();

            if (movTosCAN != null)
            {
                foreach (can_movtoscan mov in movTosCAN)
                {
                    mov.status = "S";
                }

                VMD_BD.SaveChanges();

                NumReg = movTosCAN.Count();
            }
            else
            {
                NumReg = 0;
            }

        }
        return NumReg;
    }


    /// <summary>
    /// Se encarga de la lógica para el MovtosCan
    /// 
    /// IMPORTANTE: Se deja de utilizar ésta lógica porque no es necesario validar la versión del Server
    /// 
    /// </summary>
    public void MovTosCAN_Anterior(SqlConnection SqlConServer, int VersionServer, string IpActual, string VersionesSistemas, ref Globales IGlobales)
    {

        //Obtener los MovTosCan a transferir del movil y los convierte al diseño de movtoscan del server (1)
        List<clsMovTosCan> movtos = ConvertirMovilaServer();

        this.CodigoDescarga = FalloCodigoDescarga;

        //Primero Elimino todos los registros que pudieran existir de éste autobús, en la tabla temporal del server
        EliminarRegistrosServer(SqlConServer);

        //si hay datos locales, los envío al server, a la tabla temporal

        if (movtos.Count > 0)
        {

            int caso = 0;

            /////////////////////////////Para pruebas, quitar la siguiente linea"/////////////////
            //VersionServer = 10;

            //Verifico la versión para saber cómo se deben de enviar los datos
            if (VersionServer == 8 || VersionServer == 9)
            {
                caso = 1;
            }
            else if (VersionServer <= 10)
            {
                caso = 2;
            }


            switch (caso)
            {

                case 1:
                    if (CopiaRS2(movtos, SqlConServer))
                    {
                        //Si el proceso fue correcto, traemos el código de descarga
                        var _codigoDescarga = ObtenerCodigoDescarga(SqlConServer, ref IGlobales, IpActual, VersionesSistemas);


                        //Se verifica que el código de descarga no sea nulo
                        if (_codigoDescarga != string.Empty)
                        {
                            this.CodigoDescarga = _codigoDescarga;

                            //Copiamos los registros de movtoscanx a movtoscan en el servidor
                            CopiarMovtosx(SqlConServer);

                            //Eliminarmos los datos locales que ya se subieron, sólo si se devuelve el código de descarga
                            EliminarRegistrosMovil();

                        }
                    }


                    break;

                case 2:
                    if (CopiaRS3(movtos, SqlConServer))
                    {
                        //Si el proceso fue correcto, traemos el código de descarga
                        var _codigoDescarga = ObtenerCodigoDescarga(SqlConServer, ref IGlobales, IpActual, VersionesSistemas);


                        //Se verifica que el código de descarga no sea nulo
                        if (_codigoDescarga != string.Empty)
                        {
                            this.CodigoDescarga = _codigoDescarga;

                            //Copiamos los registros de movtoscanx a movtoscan en el servidor
                            CopiarMovtosx(SqlConServer);

                            //Eliminarmos los datos locales que ya se subieron, sólo si se devuelve el código de descarga
                            EliminarRegistrosMovil();

                        }
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// Se encarga de la lógica de MovtosCAN, bajo el esquema tradicional VMD 7
    /// </summary>
    /// <param name="SqlConServer"></param>
    /// <param name="IpActual"></param>
    /// <param name="VersionesSistemas"></param>
    /// <param name="IGlobales"></param>
    public bool MovTosCAN(SqlConnection SqlConServer, string IpActual, string VersionesSistemas, ref Globales IGlobales)
    {
        try
        {
            //Obtener los MovTosCan a transferir del movil y los convierte al diseño de movtoscan del server (1)
            List<clsMovTosCan> movtos = ConvertirMovilaServer();

            this.CodigoDescarga = FalloCodigoDescarga;

            //Primero Elimino todos los registros que pudieran existir de éste autobús, en la tabla temporal del server
            if (EliminarRegistrosServer(SqlConServer))
            {
                //si hay datos locales, los envío al server, a la tabla temporal

                if (movtos.Count > 0)
                {
                    if (CopiaRS3(movtos, SqlConServer))
                    {
                        //Si el proceso fue correcto, traemos el código de descarga
                        var _codigoDescarga = ObtenerCodigoDescarga(SqlConServer, ref IGlobales, IpActual, VersionesSistemas);


                        //Se verifica que el código de descarga no sea nulo
                        if (_codigoDescarga != string.Empty)
                        {
                            this.CodigoDescarga = _codigoDescarga;

                            //Copiamos los registros de movtoscanx a movtoscan en el servidor
                            if (CopiarMovtosx(SqlConServer))
                            {
                                //Eliminarmos los datos locales que ya se subieron, sólo si se devuelve el código de descarga
                                if (!EliminarRegistrosMovil())
                                {
                                    //Error
                                    this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                                    EventoSyncCANProgreso("Error al borrar registros del movil", 0);
                                    Thread.Sleep(2000);
                                }
                            }
                            else
                            {
                                //Mandar error
                                this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                                EventoSyncCANProgreso("Error al copiar Movtoscanx a movtoscan", 0);
                                Thread.Sleep(2000);
                            }
                        }
                        else
                        {
                            //Manda Error
                            this.FalloCodigoDescarga = "Error al Obtener Codigo Descarga";
                            EventoSyncCANProgreso("Error al obtener codigo", 0);
                            ////////
                            this.CodigoDescarga = "Error en la sincronización";
                            return false;
                        }
                    }
                    else
                    {//Hubo error al querer insertar los registros al servidor

                        this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                        EventoSyncCANProgreso("Error al insertar movtoscan al server", 0);
                        Thread.Sleep(2000);
                        return false;
                    }
                }
            }
            else
            {
                this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                EventoSyncCANProgreso("Error al borrar registros de server", 0);
                Thread.Sleep(2000);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Se encarga de la lógica de MovTosCAN, bajo el nuevo esquema de NUBE VMD 8
    /// </summary>
    /// <param name="_Globales"></param>
    /// <returns></returns>
    public bool MovTosCANCloud(ServiceClient WSCAN, string IpActual, string VersionesSistemas, ref Globales IGlobales)
    {
        try
        {
            string Protocolo = string.Empty;
            int IdDireccion = 0;
            int IdRegionOperativa = 0;

            //Obtener los MovTosCan a transferir del movil y los convierte al diseño de movtoscan del server (1)
            List<clsMovTosCANCloud> movtos = ConvertirMovilACloud(IGlobales.Corrida.Region, IGlobales.Corrida.Marca, IGlobales.Corrida.Zona, IGlobales.Corrida.Servicio, ParametrosInicio.Autobus, ref Protocolo, ref IdDireccion, ref IdRegionOperativa);

            EventoSyncCANProgreso("Preparando Paquete" + Environment.NewLine + "de datos...", 0);

            string nombre = "MovTosCAN";

            //Pasamos los movtos a Texto
            MovtosATexto(movtos, nombre);

            //Comprimimos el archivo
            Comprimir(nombre);

            this.CodigoDescarga = FalloCodigoDescarga;


            EventoSyncCANProgreso("Enviando Paquete", 0);
            //Mandamos a ejecutar el WebService
            if (WSCAN.RecibeMovtos(IGlobales.Corrida.Region, IGlobales.Corrida.Marca, IGlobales.Corrida.Zona, IGlobales.Corrida.Servicio, ParametrosInicio.Autobus, Protocolo, IdDireccion, IdRegionOperativa, System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + nombre + ".zip"), movtos.Count).Equals("Correcto"))
            {

                EventoSyncCANProgreso("Procesando Datos", 0);

                var _codigoDescarga = WSCAN.ObtenerCodigoDescarga(IGlobales.Corrida.Region, IGlobales.Corrida.Marca, IGlobales.Corrida.Zona, IGlobales.Corrida.Servicio, ParametrosInicio.Autobus, IGlobales.Corrida.ConductorActualID, IGlobales.UltLat, IGlobales.UltLon, IGlobales.UltLatNS,
                    IGlobales.UltLonWE, IGlobales.UltVel, IpActual, VersionesSistemas);
                
                //Se verifica que el código de descarga no sea nulo
                if (_codigoDescarga != string.Empty)
                {
                    this.CodigoDescarga = _codigoDescarga;

                    //Copiamos los registros de movtoscanx a movtoscan en el servidor
                    if (WSCAN.CopiarMovtosCanX(ParametrosInicio.Autobus))
                    {
                        //Eliminarmos los datos locales que ya se subieron, sólo si se devuelve el código de descarga
                        if (!EliminarRegistrosMovil())
                        {
                            //Error
                            this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                            EventoSyncCANProgreso("Error al borrar registros del movil", 0);
                        }
                    }
                    else
                    {
                        //Mandar error
                        this.FalloCodigoDescarga = "Error al sincronizar MovtosCAN";
                        EventoSyncCANProgreso("Error al Procesar MovtosCAN", 0);
                    }
                }
                else
                {
                    this.FalloCodigoDescarga = "Error al obtener Codigo";
                    EventoSyncCANProgreso("Error al obtener Codigo", 0);
                }
            }
            else
            {
                EventoSyncCANProgreso("Error al enviar paquete", 0);
                return false;
            }

            
            return true;
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Se encarga de pasar los registros de MovtosCAN a arhivo de .txt
    /// </summary>
    private void MovtosATexto(List<clsMovTosCANCloud> movtos, string _nombre)
    {
        string ruta = AppDomain.CurrentDomain.BaseDirectory + _nombre + ".txt";

        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }

        using (StreamWriter sw = File.CreateText(ruta))
        {
            foreach (clsMovTosCANCloud mov in movtos)
            {

                string Linea =
                    mov.numerotramo.ToString() + "|" +
                    mov.idsecuencia.ToString() + "|" +
                    mov.origenviaje.ToString() + "|" +
                    mov.destinoviaje.ToString() + "|" +
                    mov.via.ToString() + "|" +
                    mov.fechaviaje.ToString("yyyy-MM-dd HH:mm:ss") + "|" +
                    mov.operador.ToString() + "|" +
                    mov.fechahora.ToString("yyyy-MM-dd HH:mm:ss") + "|" +
                    mov.latitud.ToString() + "|" +
                    mov.longitud.ToString() + "|" +
                    mov.ns + "|" +
                    mov.we + "|" +
                    mov.gps_velocidad.ToString() + "|" +
                    mov.accion + "|" +
                    mov.status + "|" +
                    mov.FuelLevel.ToString() + "|" +
                    mov.TachoShaftSpeed.ToString() + "|" +
                    mov.KmPerLiter.ToString() + "|" +
                    mov.TachoVehicleSpeed.ToString() + "|" +
                    mov.LiterPerHour.ToString() + "|" +
                    mov.AmbientTemp.ToString() + "|" +
                    mov.EconomyAverageKMpL.ToString() + "|" +
                    mov.FuelEconomyEstimationLMAF.ToString() + "|" +
                    mov.FuelEconomyEstimationFE.ToString() + "|" +
                    mov.RealKmPerLiterHighLevel.ToString() + "|" +
                    mov.TotalDistance.ToString() + "|" +
                    mov.TripDistance.ToString() + "|" +
                    mov.TripDistanceEstimationVS.ToString() + "|" +
                    mov.TripDistanceEstimationWVS.ToString() + "|" +
                    mov.WheelSpeed.ToString() + "|" +
                    mov.EngineForce.ToString() + "|" +
                    mov.EngineLoad.ToString() + "|" +
                    mov.MassAirFlow.ToString() + "|" +
                    mov.NumVuelta.ToString() + "|" +
                    mov.NumReg.ToString() + "|" +
                    mov.NumVueltaServer.ToString() + "|" +
                    mov.NumRegServer.ToString();

                sw.WriteLine(Linea);
            }
        }
    }

    /// <summary>
    /// Se encarga de comprimir el archivo de texto de los MovTos
    /// </summary>
    /// <param name="_nombre"></param>
    public void Comprimir(string _nombre)
    {
        using (ZipFile zip = new ZipFile())
        {
            zip.AddFile(_nombre + ".txt");
            zip.Save(_nombre + ".zip");
        }
    }

    /// <summary>
    /// Elimina los registros de la tabla temporal movtoscanx del servidor
    /// antes de insertar los que se van a sincronizar
    /// </summary>
    private bool EliminarRegistrosServer(SqlConnection sqlCon)
    {
        string strQuery = "Delete from movtoscanx where autobus ='" + ParametrosInicio.Autobus + "';";

        try
        {
            using (SqlCommand com = new SqlCommand(strQuery, sqlCon))
            {
                com.CommandType = CommandType.Text;
                com.CommandTimeout = this.timeOutComand;

                //sqlCon.Open();

                com.ExecuteNonQuery();

                //sqlCon.Close();
            }

            return true;

        }
        catch(Exception ex)
        {
            var hola = ex.ToString();

            return false;
        }
    }

    /// <summary>
    /// Manda a llamar el storedProcedure del servidor para obtener el código de Descarga
    /// </summary>
    /// <returns></returns>
    private string ObtenerCodigoDescarga(SqlConnection SqlConServer, ref Globales IGlobales, string IpActual, string VersionesSistemas)
    {
        string _codigoDescarga = string.Empty;

        try
        {
            using (SqlCommand com = new SqlCommand("CodigoDescarga", SqlConServer))
            {
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add("@IdRegion", SqlDbType.VarChar).Value = IGlobales.Corrida.Region;
                com.Parameters.Add("@IdMarca", SqlDbType.VarChar).Value = IGlobales.Corrida.Marca;
                com.Parameters.Add("@IdZona", SqlDbType.VarChar).Value = IGlobales.Corrida.Zona;
                com.Parameters.Add("@servicio", SqlDbType.Int).Value = IGlobales.Corrida.Servicio;
                com.Parameters.Add("@autobus", SqlDbType.VarChar).Value = ParametrosInicio.Autobus;
                com.Parameters.Add("@Operador", SqlDbType.Int).Value = IGlobales.Corrida.ConductorActualID;
                com.Parameters.Add("@Lat", SqlDbType.Float).Value = IGlobales.UltLat;
                com.Parameters.Add("@Lon", SqlDbType.Float).Value = IGlobales.UltLon;
                com.Parameters.Add("@Ns", SqlDbType.VarChar).Value = IGlobales.UltLatNS;
                com.Parameters.Add("@We", SqlDbType.VarChar).Value = IGlobales.UltLonWE;
                com.Parameters.Add("@Vel", SqlDbType.Float).Value = IGlobales.UltVel;
                com.Parameters.Add("@Ip", SqlDbType.VarChar).Value = IpActual;
                com.Parameters.Add("@versionapp", SqlDbType.VarChar).Value = VersionesSistemas;

                using (SqlDataReader rd = com.ExecuteReader())
                {

                    while (rd.Read())
                    {
                        _codigoDescarga = rd["CodigoDescarga"].ToString();
                    }

                    //if (rd.HasRows)
                    //{
                    //    //_codigoDescarga = rd["CodigoDescarga"].ToString();
                    //    _codigoDescarga = rd[0].ToString();
                    //}
                }
            }
            return _codigoDescarga;
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();

            return _codigoDescarga;
        }


    }

    /// <summary>
    /// Envía los movtoscan al server si es version 8 o 9 del servidor SQL
    ///
    /// IMPORTANTE: Se deja fuera de la jugada, ya que es el mismo proceso que el de RS3
    /// 
    /// </summary>
    /// <param name="movtos"></param>
    /// <returns></returns>
    private bool CopiaRS2(List<clsMovTosCan> movtos, SqlConnection SqlConServer)
    {
        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = movtos.Count;

        try
        {
            //Abrimos la conexión
            // SqlCon.Open();

            //Recorremos la lista de los MovTosCan para generar un query por registro
            foreach (clsMovTosCan mov in movtos)
            {
                cuenta++;

                #region "Lógica para descartar"

                ////No sé para que rayos, pero se pone
                //if ((cont % 100) == 0)
                //{
                //    //MuestraAvanceRegistrosCAN EVENTO
                //    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cont + " de " + movtos.Count + Environment.NewLine + Environment.NewLine +"Espere por Favor...", 0);
                //    Thread.Sleep(TiempoEsperaMensajes);

                //}

                #endregion

                //Mandamos insertar el movimientoCAN
                InsertarMovtosServer(mov, SqlConServer);

                //Sirve para mostrar en front cada 50 registros para mejorar el performance
                if (cuenta == total)
                {
                    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                }
                else if (i >= 100)
                {
                    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                    i = 1;
                }
                else
                {
                    i++;
                }

            }

            //MuestraMensajeCAN(procesando Datos)
            EventoSyncCANProgreso("Procesando Datos" + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            Thread.Sleep(TiempoEsperaMensajes);

            //SqlCon.Close();

            return true;
        }
        catch
        {
            return false;
        }

    }

    /// <summary>
    /// Envía los movtoscan al server si es version 10 del servidor SQL
    /// </summary>
    /// <param name="movtos"></param>
    /// <returns></returns>
    private bool CopiaRS3(List<clsMovTosCan> movtos, SqlConnection SqlConServer)
    {
        try
        {
            //Abrimos la conexión
            //SqlCon.Open();

            //Llevará la cuenta de los registros
            int cuenta = 0;
            //Contador para mandar cada 50 regs actualizar el front
            int i = 1;
            //Total de registros a procesar
            int total = movtos.Count;


            //Recorremos la lista de los MovTosCan para generar un query por registro
            foreach (clsMovTosCan mov in movtos)
            {
                cuenta++;

                #region "Logica para descartar"

                ////No sé para que rayos, pero se pone
                /////Se cree que ésta lógica no va
                //if ((cont % 50) == 0)
                //{
                //    //Mandamos insertar el movimientoCAN
                //    InsertarMovtosServer(mov, SqlConServer);
                //    //MuestraAvanceRegistroCAN
                //    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cont + " de  " + movtos.Count + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);

                //}
                //else if ((cont - movtos.Count) == 0)
                //{
                //    //Mandamos insertar el movimientoCAN
                //    InsertarMovtosServer(mov, SqlConServer);

                //    //MuestraAvanceEnvioRegistroCAN
                //    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cont + " de  " + movtos.Count + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);

                //}
                //else
                //{
                //    // Valores = Valores ?????
                //}

                #endregion

                //Mandamos insertar el movCAN
                InsertarMovtosServer(mov, SqlConServer);

                //Sirve para mostrar en front cada 100 registros para mejorar el performance
                if (cuenta == total)
                {
                    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                }
                else if (i >= 100)
                {
                    EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                    i = 1;
                }
                else
                {
                    i++;
                }

            }

            EventoSyncCANProgreso("Procesando Datos" + Environment.NewLine + "Espere por Favor...", 0);
            Thread.Sleep(TiempoEsperaMensajes);

            //Cerramos la conexión
            //SqlCon.Close();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene los registros del movil y los convierte a la estructura
    /// de movtoscan del servidor (FechaDescarga , CodigoDescarga)
    /// </summary>
    /// <returns></returns>
    private List<clsMovTosCan> ConvertirMovilaServer()
    {
        //Lista que se regresará para subir al server
        List<clsMovTosCan> movtoscan = new List<clsMovTosCan>();


        //Lista obtenida del movil
        List<can_movtoscan> movtosmovil = (from x in VMD_BD.can_movtoscan
                                           where x.status == "S"
                                           orderby x.NumVuelta, x.NumReg
                                           select x).ToList();

        clsMovTosCan NuevoMov;

        //Se verifica que la lista no esté vacia
        if (movtosmovil.Count > 0)
        {
            
            DateTime Elahora = DateTime.Now;

            foreach (can_movtoscan mov in movtosmovil)
            {
                NuevoMov = new clsMovTosCan();

                NuevoMov.idregion = mov.idregion;
                NuevoMov.idmarca = mov.idmarca;
                NuevoMov.idzona = mov.idzona;
                NuevoMov.servicio = mov.servicio;
                NuevoMov.numerotramo = mov.numerotramo;
                NuevoMov.idsecuencia = mov.idsecuencia;
                NuevoMov.origenviaje = mov.origenviaje;
                NuevoMov.destinoviaje = mov.destinoviaje;
                NuevoMov.via = mov.via;
                NuevoMov.fechaviaje = mov.fechaviaje;
                NuevoMov.operador = mov.operador;
                NuevoMov.autobus = mov.autobus;
                NuevoMov.fechahora = mov.fechahora;
                NuevoMov.latitud = mov.latitud;
                NuevoMov.longitud = mov.longitud;
                NuevoMov.ns = mov.ns;
                NuevoMov.we = mov.we;
                NuevoMov.gps_velocidad = mov.gps_velocidad;
                NuevoMov.accion = mov.accion;
                NuevoMov.status = mov.status;
                NuevoMov.FuelLevel = mov.FuelLevel;
                NuevoMov.TachoShaftSpeed = mov.TachoShaftSpeed;
                NuevoMov.KmPerLiter = mov.KmPerLiter;
                NuevoMov.TachoVehicleSpeed = mov.TachoVehicleSpeed;
                NuevoMov.LiterPerHour = (float)mov.LiterPerHour;
                NuevoMov.AmbientTemp = mov.AmbientTemp;
                NuevoMov.EconomyAverageKMpL = mov.EconomyAverageKMpL;
                NuevoMov.FuelEconomyEstimationLMAF = mov.FuelEconomyEstimationLMAF;
                NuevoMov.FuelEconomyEstimationFE = mov.FuelEconomyEstimationFE;
                NuevoMov.RealKmPerLiterHighLevel = mov.RealKmPerLiterHighLevel;
                NuevoMov.TotalDistance = (float)mov.TotalDistance;
                NuevoMov.TripDistance = mov.TripDistance;
                NuevoMov.TripDistanceEstimationVS = mov.TripDistanceEstimationVS;
                NuevoMov.TripDistanceEstimationWVS = mov.TripDistanceEstimationWVS;
                NuevoMov.WheelSpeed = mov.WheelSpeed;
                NuevoMov.EngineForce = mov.EngineForce;
                NuevoMov.EngineLoad = mov.EngineLoad;
                NuevoMov.MassAirFlow = mov.MassAirFlow;
                NuevoMov.Protocolo = mov.Protocolo;
                NuevoMov.IDDireccion = mov.IDDireccion;
                NuevoMov.IDRegionOperativa = mov.IDRegionOperativa;
                NuevoMov.FechaDescarga = Elahora;
                NuevoMov.CodigoDescarga = "''";
                NuevoMov.NumVuelta = mov.NumVuelta;
                NuevoMov.NumReg = mov.NumReg;
                NuevoMov.NumVueltaServer = mov.NumVueltaServer;
                NuevoMov.NumRegServer = mov.NumRegServer;

                //Se añade el nuevo movimiento a la lista para el server
                movtoscan.Add(NuevoMov);

                NuevoMov = null;
            }
        }

        return movtoscan;
        //v movtos = (from x in VMD_BD.can_movtoscan
        //              where x.status == "S"
        //              orderby x.NumVuelta, x.NumReg
        //              select new
        //              {
        //                  x.idregion,
        //                  x.idmarca,
        //                  x.idzona,
        //                  x.servicio,
        //                  x.numerotramo,
        //                  x.idsecuencia,
        //                  x.origenviaje,
        //                  x.destinoviaje,
        //                  x.via,
        //                  x.fechaviaje,
        //                  x.operador,
        //                  x.autobus,
        //                  x.fechahora,
        //                  x.latitud,
        //                  x.longitud,
        //                  x.ns,
        //                  x.we,
        //                  x.gps_velocidad,
        //                  x.accion,
        //                  x.status,
        //                  x.FuelLevel,
        //                  x.TachoShaftSpeed,
        //                  x.KmPerLiter,
        //                  x.TachoVehicleSpeed,
        //                  x.LiterPerHour,
        //                  x.AmbientTemp,
        //                  x.EconomyAverageKMpL,
        //                  x.FuelEconomyEstimationLMAF,
        //                  x.FuelEconomyEstimationFE,
        //                  x.RealKmPerLiterHighLevel,
        //                  x.TotalDistance,
        //                  x.TripDistance,
        //                  x.TripDistanceEstimationVS,
        //                  x.TripDistanceEstimationWVS,
        //                  x.WheelSpeed,
        //                  x.EngineForce,
        //                  x.MassAirFlow,
        //                  x.Protocolo,
        //                  x.IDDireccion,
        //                  x.IDRegionOperativa,
        //                  FechaDescarga = DateTime.Now,
        //                  CodigoDescarga = "",
        //                  x.NumVuelta,
        //                  x.NumReg,
        //                  x.NumVueltaServer,
        //                  x.NumRegServer
        //              }).ToList();

    }

    /// <summary>
    /// Obtiene los registros del móvil y los convierte a la estructura
    /// de movtoscan de NUBE SIIAB
    /// </summary>
    /// <param name="_idRegion"></param>
    /// <param name="_idMarca"></param>
    /// <param name="_idZona"></param>
    /// <param name="_servicio"></param>
    /// <param name="_autobus"></param>
    /// <param name="_protocolo"></param>
    /// <param name="_idDireccion"></param>
    /// <param name="_idRegionOper"></param>
    /// <returns></returns>
    private List<clsMovTosCANCloud> ConvertirMovilACloud(string _idRegion, string _idMarca, string _idZona, int _servicio, string _autobus, ref string _protocolo, ref int _idDireccion, ref int _idRegionOper)
    {
        //Lista que se regresará para subir al server
        List<clsMovTosCANCloud> movtoscan = new List<clsMovTosCANCloud>();


        //Lista obtenida del movil
        List<can_movtoscan> movtosmovil = (from x in VMD_BD.can_movtoscan
                                           where x.status == "S"
                                           orderby x.NumVuelta, x.NumReg
                                           select x).ToList();

        clsMovTosCANCloud NuevoMov;

        //Se verifica que la lista no esté vacia
        if (movtosmovil.Count > 0)
        {

            //Obtengo un registro para recuperar los datos que requiere el webservice
            can_movtoscan movTemp = (from x in movtosmovil
                                     select x).FirstOrDefault();

            _protocolo = movTemp.Protocolo;
            _idDireccion = movTemp.IDDireccion;
            _idRegionOper = movTemp.IDRegionOperativa;

            movTemp = null;

            //DateTime Elahora = DateTime.Now;

            foreach (can_movtoscan mov in movtosmovil)
            {
                NuevoMov = new clsMovTosCANCloud();

                NuevoMov.numerotramo = mov.numerotramo;
                NuevoMov.idsecuencia = mov.idsecuencia;
                NuevoMov.origenviaje = mov.origenviaje;
                NuevoMov.destinoviaje = mov.destinoviaje;
                NuevoMov.via = mov.via;
                NuevoMov.fechaviaje = mov.fechaviaje;
                NuevoMov.operador = mov.operador;
                NuevoMov.fechahora = mov.fechahora;
                NuevoMov.latitud = mov.latitud;
                NuevoMov.longitud = mov.longitud;
                NuevoMov.ns = mov.ns;
                NuevoMov.we = mov.we;
                NuevoMov.gps_velocidad = mov.gps_velocidad;
                NuevoMov.accion = mov.accion;
                NuevoMov.status = mov.status;
                NuevoMov.FuelLevel = mov.FuelLevel;
                NuevoMov.TachoShaftSpeed = mov.TachoShaftSpeed;
                NuevoMov.KmPerLiter = mov.KmPerLiter;
                NuevoMov.TachoVehicleSpeed = mov.TachoVehicleSpeed;
                NuevoMov.LiterPerHour = (float)mov.LiterPerHour;
                NuevoMov.AmbientTemp = mov.AmbientTemp;
                NuevoMov.EconomyAverageKMpL = mov.EconomyAverageKMpL;
                NuevoMov.FuelEconomyEstimationLMAF = mov.FuelEconomyEstimationLMAF;
                NuevoMov.FuelEconomyEstimationFE = mov.FuelEconomyEstimationFE;
                NuevoMov.RealKmPerLiterHighLevel = mov.RealKmPerLiterHighLevel;
                NuevoMov.TotalDistance = (float)mov.TotalDistance;
                NuevoMov.TripDistance = mov.TripDistance;
                NuevoMov.TripDistanceEstimationVS = mov.TripDistanceEstimationVS;
                NuevoMov.TripDistanceEstimationWVS = mov.TripDistanceEstimationWVS;
                NuevoMov.WheelSpeed = mov.WheelSpeed;
                NuevoMov.EngineForce = mov.EngineForce;
                NuevoMov.EngineLoad = mov.EngineLoad;
                NuevoMov.MassAirFlow = mov.MassAirFlow;
                NuevoMov.NumVuelta = mov.NumVuelta;
                NuevoMov.NumReg = mov.NumReg;
                NuevoMov.NumVueltaServer = mov.NumVueltaServer;
                NuevoMov.NumRegServer = mov.NumRegServer;

                //Se añade el nuevo movimiento a la lista para el server

                movtoscan.Add(NuevoMov);

                NuevoMov = null;
            }
        }

        return movtoscan;
    }

    /// <summary>
    /// Sirve para insertar movtoscan Uno por uno
    /// </summary>
    /// <param name="mov"></param>
    /// <param name="SqlCon"></param>
    private void InsertarMovtosServer(clsMovTosCan mov, SqlConnection SqlCon)
    {
        try
        {
            string queryInsertar = "insert Into movtoscanx values ('" + mov.idregion + "','" + mov.idmarca + "','" + mov.idzona + "'," + mov.servicio + "," +
                        mov.numerotramo + "," + mov.idsecuencia + "," + mov.origenviaje + "," + mov.destinoviaje + "," + mov.via + ", @fechaviaje," +
                        mov.operador + ",'" + mov.autobus + "', @fechahora," + mov.latitud + "," + mov.longitud + ",'" + mov.ns + "','" + mov.we + "'," +
                        mov.gps_velocidad + ",'" + mov.accion + "','" + mov.status + "'," + mov.FuelLevel + "," + mov.TachoShaftSpeed + "," + mov.KmPerLiter + "," +
                        mov.TachoVehicleSpeed + "," + mov.LiterPerHour + "," + mov.AmbientTemp + "," + mov.EconomyAverageKMpL + "," + mov.FuelEconomyEstimationLMAF + "," +
                        mov.FuelEconomyEstimationFE + "," + mov.RealKmPerLiterHighLevel + "," + mov.TotalDistance + "," + mov.TripDistance + "," + mov.TripDistanceEstimationVS + "," +
                        mov.TripDistanceEstimationWVS + "," + mov.WheelSpeed + "," + mov.EngineForce + "," + mov.EngineLoad + "," + mov.MassAirFlow + ",'" +
                        mov.Protocolo + "'," + mov.IDDireccion + "," + mov.IDRegionOperativa + ",@fechaDescarga," + mov.CodigoDescarga + "," +
                        mov.NumVuelta + "," + mov.NumReg + "," + mov.NumVueltaServer + "," + mov.NumRegServer + ")";


            using (SqlCommand com = new SqlCommand(queryInsertar, SqlCon))
            {
                com.CommandType = CommandType.Text;
                com.CommandTimeout = this.timeOutComand;
                com.Parameters.AddWithValue("@fechaviaje", mov.fechaviaje);
                com.Parameters.AddWithValue("@fechahora", mov.fechahora);
                com.Parameters.AddWithValue("@fechaDescarga", mov.FechaDescarga);
                com.ExecuteNonQuery();
            }
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();
        }
    }

    /// <summary>
    /// Elimina los registros de movtoscan del móvil
    /// </summary>
    private bool EliminarRegistrosMovil()
    {
        try
        {
            var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
            objCtx.ExecuteStoreCommand("Delete from can_movtoscan where status = 'S'");

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Copiara los registros de la tabla temporal movtoscanx hacia la tabla movtoscan
    /// </summary>
    private bool CopiarMovtosx(SqlConnection SqlConServer)
    {
        string strQuery = "insert into movtoscan select * from movtoscanx where autobus = '" + ParametrosInicio.Autobus + "'";
        try
        {
            using (SqlCommand com = new SqlCommand(strQuery, SqlConServer))
            {
                com.CommandType = CommandType.Text;
                com.CommandTimeout = this.timeOutComand;

                com.ExecuteNonQuery();
            }
            return true;
        }
        catch
        {
            return false;
        }
        
    }

    /// <summary>
    /// Descarga el contenido de las tablas para actualizar los catalogos
    /// </summary>
    /// <param name="SqlConServer"></param>
    public bool DescargarTablas(SqlConnection SqlConServer)
    {
        try
        {
            //Traemos el nombre de las tablas que vamos a descargar del server
            List<CAN_versiones> tablasServer = ObtenerTablasVerServer(SqlConServer);

            //Traemos las tablas del móvil para comparar
            List<can_versiones> tablasMovil = ObtenerTablaDesVerMovil();


            foreach (CAN_versiones tablaSer in tablasServer)
            {
                EventoSyncCANProgreso("Sincronizando tabla:" + Environment.NewLine + tablaSer.NomTabla, 0);

                Thread.Sleep(TiempoEsperaMensajes);

                //Checo si esta tabla existe en versiones de la BD local
                var tablaLocal = tablasMovil.Where(x => x.nomtabla.ToLower() == tablaSer.NomTabla.ToLower()).FirstOrDefault();

                if (tablaLocal != null)
                {
                    //Si hay registro de tabla en movil entonces comparo las versiones

                    if (tablaLocal.version < tablaSer.version) //Si la versión de la tabla local es menos que la del servidor, entonces:
                    {//Traemos los datos del servidor para pasarlos al movil

                        //Llamamos al método para actualizar la tabla en cuestión
                        if (ActualizarTabla(tablaSer.NomTabla, SqlConServer))
                        {
                            //Cambiamos la versión de la tabla
                            CambiarVersion(tablaSer.NomTabla, tablaSer.version);
                        }
                    }
                    else
                    {//la versión de la tabla local no es menos que la del servidor
                     //Según ésto no hace nada
                        continue;
                    }

                }
                else
                {//Si no hay registro Entonces inserto la tabla

                    #region "Validar si éste proceso va, por la cuestión de que no hay dinamismo
                    ////Inserto la versión en la tabla de versiones
                    //InsertarVersionMovil(tablaSer.NomTabla, 0);
                    ////De todos modos inserto la tabla cawn
                    //ActualizarTabla(tablaSer.NomTabla, SqlConServer);

                    ////Cambiamos la versión del catálogo en la tabla de versiones
                    //CambiarVersion(tablaSer.NomTabla, tablaSer.version);

                    #endregion
                }
            }// foreach de las tablas

            return true;
        }
        catch(Exception ex)
        {
            var hola = ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Obtiene la tabla de versiones del server, para poder compararlos con las del móvil
    /// </summary>
    /// <param name="SqlConServer"></param>
    /// <returns></returns>
    private List<CAN_versiones> ObtenerTablasVerServer(SqlConnection SqlConServer)
    {
        string StrQuery = "Select * From versiones order by nomtabla";

        List<CAN_versiones> Tablas = new List<CAN_versiones>();

        CAN_versiones nuevaTabla;

        using (SqlCommand com = new SqlCommand(StrQuery, SqlConServer))
        {
            com.CommandType = CommandType.Text;
            com.CommandTimeout = this.timeOutComand;

            using (SqlDataReader dr = com.ExecuteReader())
            {
                while (dr.Read())
                {
                    nuevaTabla.NomTabla = dr["NomTabla"].ToString().ToLower();
                    nuevaTabla.version = Convert.ToInt32(dr["version"]);

                    Tablas.Add(nuevaTabla);
                }
            }
        }

        return Tablas;
    }

    /// <summary>
    /// Obtiene la tabla de las versiones del móvil
    /// </summary>
    /// <returns></returns>
    private List<can_versiones> ObtenerTablaDesVerMovil()
    {

        List<can_versiones> Tablas = (from x in VMD_BD.can_versiones
                                      select x).ToList();

        return Tablas;

    }

    /// <summary>
    /// Nos sirve para ingresar una nueva versión de tabla a la BD del móvil
    /// </summary>
    /// <param name="NomTabla"></param>
    /// <param name="Version"></param>
    private void InsertarVersionMovil(string NomTabla, int Version)
    {
        can_versiones nuevaVersion = new can_versiones();

        nuevaVersion.nomtabla = NomTabla;
        nuevaVersion.version = Version;

        VMD_BD.can_versiones.Add(nuevaVersion);
        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Se encarga de cambiar la version del catalogo
    /// en la tabla de versiones
    /// </summary>
    /// <param name="NomTabla"></param>
    /// <param name="Version"></param>
    private void CambiarVersion(string NomTabla, int Version)
    {

        can_versiones Registro = (from x in VMD_BD.can_versiones
                                  where x.nomtabla == NomTabla
                                  select x).FirstOrDefault();

        Registro.version = Version;

        VMD_BD.SaveChanges();

    }

    /// <summary>
    /// Obtiene los registros de una tabla del servidor alterno según un query
    /// </summary>
    /// <param name="SqlConServer"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    private DataTable RegsServer(SqlConnection SqlConServer, string query)
    {

        SqlCommand cmd = new SqlCommand(query, SqlConServer);

        DataTable Registros = new DataTable();

        Registros.Load(cmd.ExecuteReader());

        return Registros;

    }

    /// <summary>
    /// Actualiza el contenido de la tabla, con su debido proceso
    /// de eliminiar primero  su contenido, copiar los nuevos datos y actualizar
    /// la versión en la tabla de versiones
    /// </summary>
    /// <param name="Nombretabla"></param>
    /// <param name="SqlConServer"></param>
    private bool ActualizarTabla(string Nombretabla, SqlConnection SqlConServer)
    {
        DataTable RegistrosServer;

        try
        {
            //Verificamos si es la tabla de Parametros Inicio para agregarle un where y descargar la del autobus
            if (Nombretabla.Equals("can_ParametrosInicio"))
            {
                RegistrosServer = RegsServer(SqlConServer, "select * from " + Nombretabla + " AS CAN_" + Nombretabla + " where autobus ='" + ParametrosInicio.Autobus + "'");
            }
            else
            {
                RegistrosServer = RegsServer(SqlConServer, "select * from " + Nombretabla + " AS CAN_" + Nombretabla + "");

            }


            //Verificamos que haya traido registros
            if (RegistrosServer != null)
            {
                //Copiamos los registros del servidor en la tabla del móvil
                if(SelectorDeTabla(Nombretabla, RegistrosServer))
                {
                    return true;
                }
                else { return false; }

            }
            else { return false; }
        }
        catch
        {
            return false;
        }
        

    }

    /// <summary>
    /// Elimina los registros de la tabla movil a través de un query (se queda por si se ocupa)
    /// </summary>
    /// <param name="NombreTabla"></param>
    private bool EliminarRegistros(string NombreTabla)
    {
        try
        {   ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
            var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
            objCtx.ExecuteStoreCommand("truncate table can_" + NombreTabla);
            return true;
        }
        catch
        {
            return false;
        }

    }

    /// <summary>
    /// Se encarga de descargar los catalogos bajo el esquema de NUBE VMD 8
    /// </summary>
    /// <returns></returns>
    public bool DescargarTablasCloud(ServiceClient WSCAN)
    {
        try
        {
            //Traemos las versiones de las tablas del server
            List<CAN_versiones> tablasServer = RecuperarVersiones(WSCAN.ObtenerTablasVerServer());

            //Traemos las tablas del móvil para comparar
            List<can_versiones> tablasMovil = ObtenerTablaDesVerMovil();

            foreach (CAN_versiones tablaSer in tablasServer)
            {
                var tablaLocal = tablasMovil.Where(x => x.nomtabla.ToLower() == tablaSer.NomTabla.ToLower()).FirstOrDefault();

                if (tablaLocal != null)
                {
                    
                    //Si hay registro de tabla en movil entonces, compraro las versiones
                    if (tablaLocal.version < tablaSer.version)
                    {//Traemos los datos del servidor para pasarlos al movil

                        EventoSyncCANProgreso("Sincronizando " + tablaLocal.nomtabla + " ...", 0);
                        //Llamamos al método para actualizar la tabla en cuestión
                        if (ActualizarTablaCloud(tablaSer.NomTabla, WSCAN))
                        {
                            //Cambiamos la versión de la tabla
                            CambiarVersion(tablaSer.NomTabla, tablaSer.version);
                        }
                    }
                    else
                    {//la versión de la tabla local no es menos que la del servidor
                     //Según esto no hace nada
                        continue;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            return false;
        }
    }

    /// <summary>
    /// Se encarga de recuperar las versiones de formato String
    /// </summary>
    /// <param name="tabla"></param>
    /// <returns></returns>
    private List<CAN_versiones> RecuperarVersiones(string[] tabla)
    {
        List<CAN_versiones> versiones = new List<CAN_versiones>();
        CAN_versiones nuevaversion;
        try
        {
            foreach (string str in tabla)
            {

                string[] strsplit = str.Split('|');

                nuevaversion = new CAN_versiones
                {
                    NomTabla = strsplit.GetValue(0).ToString(),
                    version = Convert.ToInt32(strsplit.GetValue(1))
                };

                versiones.Add(nuevaversion);
            }
        }
        catch
        {

        }
        return versiones;
    }


    /// <summary>
    /// Actualiza el contenido de la tabla, con su debido proceso
    /// de eliminiar primero  su contenido, copiar los nuevos datos y actualizar
    /// la versión en la tabla de versiones
    /// </summary>
    /// <param name="Nombretabla"></param>
    /// <param name="SqlConServer"></param>
    private bool ActualizarTablaCloud(string Nombretabla, ServiceClient WSCAN)
    {
        DataTable RegistrosServer = new DataTable();

        try
        {
            //Verificamos si es la tabla de Parametros Inicio para agregarle un where y descargar la del autobus
            if (Nombretabla.Equals("can_ParametrosInicio"))
            {

                RegistrosServer = WSCAN.RegsServer("select * from " + Nombretabla + " AS CAN_" + Nombretabla + " where autobus ='" + ParametrosInicio.Autobus + "'");
            }
            else
            {
                RegistrosServer = WSCAN.RegsServer("select * from " + Nombretabla + " AS CAN_" + Nombretabla + "");
            }


            //Verificamos que haya traido registros
            if (RegistrosServer != null)
            {
                //Copiamos los registros del servidor en la tabla del móvil
                if (SelectorDeTabla(Nombretabla, RegistrosServer))
                {
                    return true;
                }
                else { return false; }
            }
            else { return false; }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            return false;
        }
    }

    #region "Pinche desmadre"

    /// <summary>
    /// Se encarga de enviar el Datatable, según a la tabla local correspondiente
    /// Es un desmadre, pero por tiempo luego lo optimizo
    /// </summary>
    /// <param name="NombreTabla"></param>
    /// <param name="RegistrosServer"></param>
    private bool SelectorDeTabla(string NombreTabla, DataTable RegistrosServer)
    {
        try
        {
            //Se desactiva la propiedad para acelerar el proceso de insercción
            VMD_BD.Configuration.AutoDetectChangesEnabled = false;

            switch (NombreTabla)
            {
                case "catmetasregionros":
                    EliminarRegistros(NombreTabla);
                    CatalogosMetas(RegistrosServer);
                    return true;

                case "operadores":
                    EliminarRegistros(NombreTabla);
                    CatalogosOperadores(RegistrosServer);
                    return true;

                case "redes":
                    EliminarRegistros(NombreTabla);
                    CatalogosRedes(RegistrosServer);
                    return true;

                case "terminales":
                    EliminarRegistros(NombreTabla);
                    CatalogosTerminales(RegistrosServer);
                    return true;

                case "zonasdescarga":
                    EliminarRegistros(NombreTabla);
                    CatalogosZonasDescarga(RegistrosServer);
                    return true;

                case "zonasdescarga2":
                    EliminarRegistros(NombreTabla);
                    CatalogosZonasDescarga2(RegistrosServer);
                    return true;

                case "poblaciones":
                    EliminarRegistros(NombreTabla);
                    Catalogospoblaciones(RegistrosServer);
                    return true;

                default: return false;
            }
        }
        catch(Exception e)
        {
            var hola = e.ToString();
            //Se vuelve a activar la propiedad
            return false;
        }
        finally
        {
            //Se vuelve a activar la propiedad
            VMD_BD.Configuration.AutoDetectChangesEnabled = true;
        }
    }

    /// <summary>
    /// Actualiza la tabla can_CatMetasRegionROS
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosMetas(DataTable Registros)
    {
        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_catmetasregionros");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        foreach (DataRow row in Registros.Rows)
        {
            var nuevameta = new can_catmetasregionros();

            cuenta++;

            nuevameta.Clasificacion = row["Clasificacion"].ToString();
            nuevameta.Tipo = row["Tipo"].ToString();
            nuevameta.Consejo = float.Parse(row["Consejo"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
            nuevameta.Odometro = float.Parse(row["Odometro"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
            nuevameta.Mes = row["Mes"].ToString();
            nuevameta.Year = Convert.ToInt32(row["Year"]);
            nuevameta.Region = row["Region"].ToString();

            VMD_BD.can_catmetasregionros.Add(nuevameta);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            nuevameta = null;

        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();

    }

    /// <summary>
    /// Actualiza la tabla can_operadores
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosOperadores(DataTable Registros)
    {
        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_operadores");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {

            var NuevoOperador = new can_operadores();

            cuenta++;


            NuevoOperador.cveemp = Convert.ToInt32(row["cveemp"]);
            NuevoOperador.nombre = row["nombre"].ToString();
            NuevoOperador.status = row["status"].ToString();

            VMD_BD.can_operadores.Add(NuevoOperador);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if(i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevoOperador = null;
        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Actualiza la tabla can_redes
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosRedes(DataTable Registros)
    {

        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_redes");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {
            var NuevaRed = new can_redes();

            cuenta++;

            NuevaRed.Id = Convert.ToInt32(row["Id"]);
            NuevaRed.Descripcion = row["Descripcion"].ToString();
            NuevaRed.Sufijo = Convert.ToInt32(row["Sufijo"]);
            NuevaRed.IdZonaDescarga = Convert.ToInt32(row["IdZonaDescarga"]);
            
            VMD_BD.can_redes.Add(NuevaRed);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevaRed = null;

        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();

    }

    /// <summary>
    /// Actualiza la tabla can_terminales
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosTerminales(DataTable Registros)
    {

        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_terminales");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {
            var NuevaTerminal = new can_terminales();

            cuenta++;

            NuevaTerminal.Id = Convert.ToInt32(row["Id"]);
            NuevaTerminal.Descripcion = row["Descripcion"].ToString();
            NuevaTerminal.IdPob = Convert.ToInt32(row["IdPob"]);
            NuevaTerminal.Lat = float.Parse(row["Lat"].ToString());
            NuevaTerminal.NS = row["NS"].ToString();
            NuevaTerminal.Lon = float.Parse(row["Lon"].ToString());
            NuevaTerminal.WE = row["WE"].ToString();
            NuevaTerminal.IdZonaDescarga = long.Parse(row["IdZonaDescarga"].ToString());

            VMD_BD.can_terminales.Add(NuevaTerminal);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevaTerminal = null;
        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Actualiza la tabla can_zonasdescarga
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosZonasDescarga(DataTable Registros)
    {

        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_zonasdescarga");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {
            var NuevaZona = new can_zonasdescarga();

            cuenta++;

            NuevaZona.Id = Convert.ToInt32(row["Id"]);
            NuevaZona.Descripcion = row["Descripcion"].ToString();
            NuevaZona.RutaDescarga = row["RutaDescarga"].ToString();

            VMD_BD.can_zonasdescarga.Add(NuevaZona);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevaZona = null;
        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Actualiza la tabla can_zonasdescarga2
    /// </summary>
    /// <param name="Registros"></param>
    private void CatalogosZonasDescarga2(DataTable Registros)
    {

        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_zonasdescarga");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {
            var NuevaZona = new can_zonasdescarga2();

            cuenta++;

            NuevaZona.Id = Convert.ToInt32(row["Id"]);
            NuevaZona.Descripcion = row["Descripcion"].ToString();
            NuevaZona.RutaDescarga = row["RutaDescarga"].ToString();
            NuevaZona.Nube = Convert.ToBoolean(row["Nube"]);

            VMD_BD.can_zonasdescarga2.Add(NuevaZona);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevaZona = null;
        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();
    }

    /// <summary>
    /// Actualiza la tabla can_poblaciones
    /// </summary>
    /// <param name="Registros"></param>
    private void Catalogospoblaciones(DataTable Registros)
    {

        ///Borramos la lista, mandamos un truncate para que se reestablezca el auto INC del ID de Registro
        //var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        //objCtx.ExecuteStoreCommand("truncate table can_terminales");

        //Llevará la cuenta de los registros
        int cuenta = 0;
        //Contador para mandar cada 50 regs actualizar el front
        int i = 1;
        //Total de registros a procesar
        int total = Registros.Rows.Count;

        //Se desactiva la propiedad para acelerar el proceso de insercción
        VMD_BD.Configuration.AutoDetectChangesEnabled = false;

        foreach (DataRow row in Registros.Rows)
        {
            var NuevaPoblacion = new can_poblaciones();

            cuenta++;

            NuevaPoblacion.idpob = Convert.ToInt32(row["idpob"]);
            NuevaPoblacion.despob = row["despob"].ToString();
            NuevaPoblacion.latitud = Convert.ToInt32(row["latitud"]);
            NuevaPoblacion.ns = row["ns"].ToString();
            NuevaPoblacion.longitud = Convert.ToInt32(row["longitud"]);
            NuevaPoblacion.we = row["we"].ToString();
            NuevaPoblacion.IDTIPOPUNTO = Convert.ToInt32(row["idtipopunto"]);
            NuevaPoblacion.CVEPOB = row["cvepon"].ToString();

            VMD_BD.can_poblaciones.Add(NuevaPoblacion);

            if (cuenta == total)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
            }
            else if (i >= 50)
            {
                EventoSyncCANProgreso("Registros Procesados" + Environment.NewLine + cuenta + " de " + total + Environment.NewLine + Environment.NewLine + "Espere por Favor...", 0);
                i = 1;
            }
            else
            {
                i++;
            }

            NuevaPoblacion = null;
        }

        //Se guarda en la base de datos
        VMD_BD.SaveChanges();
    }

    #endregion  

    #region "Utilidades"

    /// <summary>
    /// Obtenemos el nombre del disco de peliculas (Label)
    /// </summary>
    /// <returns></returns>
    private string VolumeLabel()
    {
        string Label = string.Empty;

        DriveInfo[] allDrives = DriveInfo.GetDrives();

        foreach (DriveInfo d in allDrives)
        {
            if (d.Name.Equals(ParametrosInicio.RutaDiscoMBR + "\\"))
            {
                Label = d.VolumeLabel;
                break;
            }
        }
        return Label;
    }

    /// <summary>
    /// Recibe el mensaje que queremos que vaya en el Log
    /// </summary>
    /// <param name="evento"></param>
    public void AgregarLogSync(string evento, ref List<string> log)
    {
        log.Add(DateTime.Now.ToString() + " / " + evento);
    }

    #endregion

    private struct CAN_versiones
    {
        public string NomTabla;

        public int version;
    }
}