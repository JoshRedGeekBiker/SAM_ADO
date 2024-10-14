using InterfazSistema.ModelosBD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Threading;
using System.Timers;

using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Clase que se encargará de englobar la lógica del colisionador de geocercas para CAN V2
/// 22/AGO/24
/// </summary>
public class CANV2 : IGPS, IBDContext, IBDConextCAN2
{
    #region Variables
    private bool InDebug = false;
    private int bandera = 0;
    private int Operador = 0;
    private int CambioManos = 0;


    //Logicas
    public GPSData Datos_GPS { get; set; }
    private ProtocoloCAN1 objProtocolo;

    public int geocercaActual = 0;//Para llevar el control con la que colisionó
    public int geocercaAnterior = 0;//Para llevar el control con la que colisionó
    private int IdGeocerca = 0; //Con la que estaremos llevando el FR_META
    private bool Angulo = false;
    private bool flagColision = false; //Con la que estaremos llevando el FR_META
    private double FR_META_DEF = 0.00;
    public double Aceleracion = 0.00;

    private double ultimaAceleracion = 0;
    private DateTime tiempoUltimaAceleracion = DateTime.Now;

    private int IdFR = 0;
    private int IdAceleracion = 0;
    private List<geocercaParametros> geos_temp = new List<geocercaParametros>();
    //ContextosBD
    public vmdEntities VMD_BD { get; set; }
    public Can2Entities CAN2_BD { get; set; }

    //Timers
    private System.Timers.Timer timerActualiza = new System.Timers.Timer();
    private System.Timers.Timer timerPrueba = new System.Timers.Timer();

    //Parametros
    private System.String RutaMaestra = "";
    private int long_negativo = 0;
    private can_parametrosinicio ParametrosInicioCAN;


    //Flags
    private bool EsperandoSync = false;

    //Hilos
    private Thread HiloDeteccionColision;
    private Thread HiloPrueba;


    //Catalogos
    private List<Response_Geocerca> geocercas = new List<Response_Geocerca>();

    //Se encargan de ayudarnos para el calculo de la colisión
    private POINT posicion_Actual = new POINT();
    private POINT posicion_Anterior = new POINT();



    #endregion
    #region Eventos
    /// <summary>
    /// Se encarga de enviar un mensaje hacia el front que nos sirve como Loggeo
    /// </summary>
    /// <param name="mensaje"></param>
    public delegate void Reportar(string mensaje);
    public event Reportar EnviarMensaje;

    /// <summary>
    /// Se encarga de enviar un mensaje hacia el front del estatus del sync
    /// </summary>
    /// <param name="mensaje"></param>
    public delegate void _ReportarSync(string mensaje);
    public event _ReportarSync MensajeSync;

    /// <summary>
    /// Se encarga de mandar a pedir los Datos de GPS
    /// Powered ByRED 21AGO24
    /// </summary>
    /// <returns></returns>
    public delegate GPSData _DatosGPS();
    public event _DatosGPS DataGPS;

    /// <summary>
    /// Se encarga de mandarle los parametros al Socket
    /// </summary>
    /// <param name="_FR_META"></param>
    /// <param name="_FR_REAL"></param>
    /// <param name="_PARAMETROS_ID"></param>
    public delegate void _EnviarSocket(string _FR_META, string _FR_REAL, string _PARAMETROS_ID);
    public event _EnviarSocket EnviarSocket;

    /// <summary>
    /// Se encarga de mandar a reiniciar la aplicación
    /// </summary>
    public delegate void Reiniciar();
    public event Reiniciar ReiniciarAPP;

    /// <summary>
    /// Se encarga de avisar que la aplicación tiene que terminar
    /// </summary>
    public delegate void Cerrar();
    public event Cerrar CerrarAPP;

    /// <summary>
    /// Se encarga de traer el dato de aceleración
    /// Powered ByRED 09SEP2024
    /// </summary>
    /// <returns></returns>
    public delegate string _Aceleracion();
    public event _Aceleracion Aceler;

    /// <summary>
    /// Se encarga de mandar la alerta de aceleracion
    /// Powered ByRED 04OCT2024
    /// </summary>
    /// <returns></returns>
    public delegate void _AlertaAcel(string _Valor, string _Color);
    public event _AlertaAcel AlertaAcel;

    #endregion
    #region Constructores
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public CANV2(bool _Debug = false)
    {
        InDebug = _Debug; //Nos servirá por si queremos ejecutar algun proceso en modo Debug
        VMD_BD = new vmdEntities();
        CAN2_BD = new Can2Entities();
        ParametrosInicioCAN = (from x in VMD_BD.can_parametrosinicio select x).FirstOrDefault();
        this.FR_META_DEF = Convert.ToDouble((from x in CAN2_BD.can2_config where x.cve_parametro == "fr_default" select x.valor_parametro).FirstOrDefault());
        this.long_negativo = Convert.ToInt32((from x in CAN2_BD.can2_config where x.cve_parametro == "long_negativo" select x.valor_parametro).FirstOrDefault());
        this.Datos_GPS = new GPSData();

        //cargamos la lógica de CAN1
        objProtocolo = new ProtocoloCAN1();

    }

    #endregion
    #region Métodos Privados

    /// <summary>
    /// Se encargara de realizar pruebas de desarrollo
    /// </summary>
    private void Debugg()
    {
        ParametrosInicioCAN = (from x in VMD_BD.can_parametrosinicio select x).FirstOrDefault();
        Datos_GPS = new GPSData();
        VerificarEventos();
        Sincronizar("");

    }


    /// <summary>
    /// Se encarga de mandar a pedir los últimos datos del GPS
    /// Powered ByRED 21AGO2024
    /// </summary>
    private void ActualizarDatosGPS()
    {
        try
        {

            this.Datos_GPS = DataGPS();
            //EnviarMensaje("GPS REcibido" + Datos_GPS.Latitud + " " + Datos_GPS.Longitud);
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encargará de configurar los timers necesarios para el funcionamiento del sistema
    /// </summary>
    private void PreparaTimers()
    {
        //Hilos
        HiloDeteccionColision = new Thread(new ThreadStart(Colisionador));

        //Timer de actualiza
        timerActualiza.Interval = 500; //Configuramos a medio segundo
        timerActualiza.Enabled = true;
        timerActualiza.Elapsed += Actualizar_Tick;
        timerActualiza.Start();

        ////timer para debbug
        //if (InDebug)
        //{
        //    timerPrueba.Interval = 10000;
        //    timerPrueba.Enabled = true;
        //    timerPrueba.Elapsed += Debuggear;
        //    timerPrueba.Start();
        //}

    }
    /// <summary>
    /// Se encarga de detener los timers e hilos para un cierre correcto del programa
    /// </summary>
    private void DetieneTimersEHilos()
    {
        timerActualiza.Stop();
        timerActualiza.Enabled = false;

        timerPrueba.Stop();
        timerPrueba.Enabled = false;

    }

    /// <summary>
    /// Se encarga de cargar los catálogos en memoria para evitar hacer peticiones en exceso a la BD
    /// </summary>
    private void CargarCatalogos()
    {
        try
        {
            geocercas = TraerGeocercas();
            WS _ws = new WS();
            this.IdFR = _ws.RecuperarIdParametro("Rendimiento");
            this.IdAceleracion = _ws.RecuperarIdParametro("Posicion Pedal Acelerador");
        }
        catch (Exception ex)
        {
            EnviarMensaje("CargarCatalogos " + ex.Message);
        }
    }

    /// <summary>
    /// Se encarga de cargar en memoria las geocercas para ser coilisionadas
    /// </summary>
    private List<Response_Geocerca> TraerGeocercas()
    {
        List<Response_Geocerca> rs = new List<Response_Geocerca>();
        List<can2_geocerca> GeocercasActivas = new List<can2_geocerca>();
        can2_geocercaparametros IdGeocercaDeParametros = new can2_geocercaparametros();

        GeocercasActivas = (from x in CAN2_BD.can2_geocerca where x.activo == true select x).ToList();


        foreach (can2_geocerca info in GeocercasActivas)
        {

            Response_Geocerca geoCoord = new Response_Geocerca()
            {
                geocercaId = info.geocercaListaId,
                nombre = info.nombre,
                clave = info.clave
            };
            geoCoord.points = new List<POINT>();
            List<can2_coordenadas> cor = (from x in CAN2_BD.can2_coordenadas where x.geocercaId == info.geocercaListaId select x).ToList();
            foreach (can2_coordenadas c in cor)
            {
                POINT punto = new POINT();
                punto.Latitud = c.latitudCan;
                punto.Longitud = c.longitudCan;
                punto.Secuencia = c.sequence;
                geoCoord.points.Add(punto);
            }
            geoCoord.geocercaParametros = new List<geocercaParametros>();
            List<can2_geocercaparametros> gcp = (from x in CAN2_BD.can2_geocercaparametros
                                                 where x.geocercaId == info.geocercaListaId
                                                 && x.activo != false
                                                 && x.fechaVigenciaFin > DateTime.Now
                                                 && x.fechaVigenciaInicio < DateTime.Now
                                                 select x).ToList();
            foreach (var geoparam in gcp)
            {
                geocercaParametros gp = new geocercaParametros();
                gp.geocercaId = geoparam.geocercaId;
                gp.ParametroId = geoparam.parametroId;
                gp.ValorParametro = geoparam.ValorParametro;
                gp.orientacionFinal = geoparam.orientacionFinal;
                gp.orientacionInicial = geoparam.orientacionInicial;
                gp.NombreParametro = geoparam.nombreParametro;
                gp.MargenParametro = geoparam.margenParametro;
                gp.FechaVigenciaFin = geoparam.fechaVigenciaFin;
                gp.FechaVigenciaInicio = geoparam.fechaVigenciaInicio;
                geoCoord.geocercaParametros.Add(gp);
            }

            rs.Add(geoCoord);


        }

        return rs;

    }


    private void Colisionador()
    {
        try
        {
            var EventoColision = DetectarColisiones();
            if (EventoColision.Count() > 0)
            {
                CerrarEvento();
                //Mandamos a generar el evento correspondiente para desatar los parametros
                GenerarEvento(EventoColision);

            }
            else
            {
                if (!this.flagColision)
                {

                    if (this.geocercaActual != this.geocercaAnterior)
                    {
                        this.geocercaAnterior = this.geocercaActual;
                        if (Angulo)
                        {
                            this.Angulo = false;
                            CerrarEvento();
                            this.geos_temp.Clear();
                            this.IdGeocerca = 0;
                        }
                    }
                    else if (this.IdGeocerca == 0)
                    {
                        GenerarFR_default();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            EnviarMensaje(ex.Message);
        }
    }
    private double convertirparaPunto()
    {
        double res = 0.0;
        return res;
    }




    private static double ParseLongitude(string longStr)
    {
        // Split the string by the decimal point
        string[] lonParts = longStr.Split('.');

        if (lonParts.Length != 2)
        {
            throw new ArgumentException("Input string must contain a decimal point.");
        }

        string integerPart = lonParts[0];
        string decimalPart = lonParts[1];

        // Check the length of the integer part
        var integerstrin = Convert.ToDouble(integerPart);
        var integerd = integerstrin.ToString();
        // Check the length of the integer part
        int integerPartLength = integerd.Length;

        int lonDeg;
        int lonMin;
        int lonSec;

        if (integerPartLength == 4)
        {
            // Extract degrees and minutes for the format with 4 digits
            lonDeg = int.Parse(integerd.Substring(0, 2));
            lonMin = int.Parse(integerd.Substring(2, 2));
        }
        else if (integerPartLength == 5)
        {
            // Extract degrees and minutes for the format with 5 digits
            lonDeg = int.Parse(integerPart.Substring(0, 3));
            lonMin = int.Parse(integerPart.Substring(3, 2));
        }
        else
        {
            throw new ArgumentException("Invalid format for longitude.");
        }

        // Convert the decimal part of the minutes to seconds
        if (decimalPart.Length > 0)
        {
            lonSec = (int)(Double.Parse("0." + decimalPart));
        }
        else
        {
            lonSec = 0;
        }

        // Concatenate the degrees, minutes, and seconds
        string concatenatedString = $"{lonDeg:D2}{lonMin:D2}";
        double enteros = Convert.ToDouble(concatenatedString);

        double decimales = Convert.ToDouble(lonSec / 10000.0);
        double result = enteros + decimales;

        return result;

    }





    private void VerificarEventos()
    {


        try
        {
            var reg = (from x in CAN2_BD.can2_parametros
                       where x.long_Fin == 0 && x.lat_Fin == 0 && x.Fecha_Fin == DateTime.MinValue
                       select x).ToList();
            if (reg != null)
            {
                if (objProtocolo.accion.Equals("VA") || objProtocolo.accion.Equals("CM"))
                {
                    foreach (var r in reg)
                    {
                        if (r.operador == objProtocolo.operador)
                        {
                            CerrarRegistros(r, false);
                        }
                        else
                        {
                            CerrarRegistros(r, true);
                        }
                    }
                }
                else
                {
                    foreach (var r in reg)
                    {

                        CerrarRegistros(r, true);
                    }
                }

            }
        }
        catch (Exception e)
        {

        }


    }



    /// <summary>
    /// Paso 3: Se encarga de generar el evento para que sea reproducido por VMD
    /// </summary>
    private void GenerarEvento(List<geocercaParametros> geos)
    {
        try
        {
            this.geos_temp = geos;
            foreach (geocercaParametros geo in geos)
            {
                this.IdGeocerca = geo.geocercaId;
                //hay que preguntar acerca del numero de parametro a utilizar
                if (geo.ParametroId == this.IdFR)
                {
                    objProtocolo.InicializarVariablesCAN();
                    objProtocolo.VAL_FR_META = Convert.ToDouble(geo.ValorParametro);
                }
                can2_parametros parametro = new can2_parametros()
                {
                    valor_parametro = Convert.ToDouble(geo.ValorParametro),
                    geocerca_id = this.IdGeocerca,
                    Fecha_Ini = DateTime.Now,
                    lat_Ini = Convert.ToDouble(Datos_GPS.Latitud),
                    long_Ini = Convert.ToDouble(Datos_GPS.Longitud),
                    Cambio_Manos = this.CambioManos,
                    operador = this.Operador,
                    parametroId = geo.ParametroId
                };
                CAN2_BD.can2_parametros.Add(parametro);
                CAN2_BD.SaveChanges();
                Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            EnviarMensaje(ex.Message);
        }

    }
    /// <summary>
    /// se encarga de generar el evento cuando se  detona una canfirma
    /// </summary>
    private void GenerarEventoFirma()
    {
        try
        {
            CerrarEvento();
            if (this.geos_temp.Count > 0)
            {
                foreach (geocercaParametros geo in this.geos_temp)
                {
                    //hay que preguntar acerca del numero de parametro a utilizar
                    if (geo.ParametroId == this.IdFR)
                    {
                        objProtocolo.InicializarVariablesCAN();
                        objProtocolo.VAL_FR_META = Convert.ToDouble(geo.ValorParametro);
                    }
                    can2_parametros parametro = new can2_parametros()
                    {
                        valor_parametro = Convert.ToDouble(geo.ValorParametro),
                        geocerca_id = this.IdGeocerca,
                        Fecha_Ini = DateTime.Now,
                        lat_Ini = Convert.ToDouble(Datos_GPS.Latitud),
                        long_Ini = Convert.ToDouble(Datos_GPS.Longitud),
                        Cambio_Manos = this.CambioManos,
                        operador = this.Operador,
                        parametroId = geo.ParametroId
                    };
                    CAN2_BD.can2_parametros.Add(parametro);
                    CAN2_BD.SaveChanges();
                    Thread.Sleep(1000);
                }
            }
            else
            {
                GenerarFR_default();
            }

        }
        catch (Exception e)
        {

        }
    }

    /// <summary>
    /// se encarga de generar FR_DEFAULT cuando no tengamos geocerca
    /// </summary>
    private void GenerarFR_default()
    {


        try
        {
            var def = (from x in CAN2_BD.can2_parametros
                       where x.geocerca_id == 0
                            && x.valor_real == 0
                            && x.Fecha_Fin == DateTime.MinValue
                       select x).FirstOrDefault();
            if (this.FR_META_DEF == 0)
            {

                this.FR_META_DEF = (from x in CAN2_BD.can2_parametros
                                    where x.geocerca_id == 0
                                         && x.parametroId == this.IdFR
                                    select x.valor_parametro).FirstOrDefault();
            }

            if (this.Aceleracion == 0)
            {
                this.Aceleracion = (from x in CAN2_BD.can2_parametros
                                    where x.geocerca_id == 0
                                         && x.parametroId == this.IdAceleracion
                                    select x.valor_parametro).FirstOrDefault();
            }

            if (def == null)
            {
                //Reiniciamos Variables del protocolo
                objProtocolo.InicializarVariablesCAN();
                objProtocolo.VAL_FR_META = Convert.ToDouble(this.FR_META_DEF);

                can2_parametros fr = new can2_parametros()
                {
                    valor_parametro = Convert.ToDouble(this.FR_META_DEF),
                    geocerca_id = this.IdGeocerca,
                    parametroId = this.IdFR,
                    Fecha_Ini = DateTime.Now,
                    lat_Ini = Convert.ToDouble(Datos_GPS.Latitud),
                    long_Ini = Convert.ToDouble(Datos_GPS.Longitud),
                    Cambio_Manos = CambioManos,
                    operador = Operador
                };
                CAN2_BD.can2_parametros.Add(fr);
                can2_parametros Acel = new can2_parametros()
                {
                    valor_parametro = Convert.ToDouble(this.Aceleracion),
                    geocerca_id = this.IdGeocerca,
                    parametroId = this.IdAceleracion,
                    Fecha_Ini = DateTime.Now,
                    lat_Ini = Convert.ToDouble(Datos_GPS.Latitud),
                    long_Ini = Convert.ToDouble(Datos_GPS.Longitud),
                    Cambio_Manos = CambioManos,
                    operador = Operador
                };
                CAN2_BD.can2_parametros.Add(Acel);
                CAN2_BD.SaveChanges();
            }

        }
        catch
        {

        }
    }

    /// <summary>
    /// cerrar el registro, se encarga de cerrar los registros que no fueron cerrados correctamente 
    /// por apagado de equipo
    /// </summary>
    private void CerrarRegistros(can2_parametros fr, bool recuperado)
    {
        if (fr != null)
        {
            //condicion ? valor complido : valor no cumplido 

            fr.valor_real = recuperado ? 0 : objProtocolo.VAL_FR_REAL;
            fr.lat_Fin = Convert.ToDouble(Datos_GPS.Latitud);
            fr.long_Fin = Convert.ToDouble(Datos_GPS.Longitud);
            fr.Fecha_Fin = fr.Fecha_Ini;
            fr.operador = recuperado ? 0 : fr.operador;
            CAN2_BD.SaveChanges();

        }
        this.IdGeocerca = 0;

    }




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
            EnviarMensaje(ex.Message);
        }
        return res;
    }


    /// <summary>
    /// Se encarga de calcular el semaforo y de enviar el valor de la aceleración
    /// </summary>
    private void semaforizacionAceleracion(int idGeocerca)
    {
        double Margen = (from x in CAN2_BD.can2_geocercaparametros
                         where x.geocercaId == idGeocerca
                                    && x.parametroId == 19
                         select x.margenParametro).FirstOrDefault();

        if (this.Aceleracion != ultimaAceleracion)
        {
            ultimaAceleracion = this.Aceleracion;
            tiempoUltimaAceleracion = DateTime.Now;
        }

        if (this.Aceleracion < Margen)
        {
            AlertaAcel(Aceleracion.ToString(), "verde");
        }
        else if (this.Aceleracion >= Margen)
        {
            TimeSpan tiempoTranscurrido = DateTime.Now - tiempoUltimaAceleracion;

            if (tiempoTranscurrido.TotalSeconds < 15)
            {
                AlertaAcel(Aceleracion.ToString(), "Amarillo");
            }
            else
            {
                AlertaAcel(Aceleracion.ToString(), "Rojo");
            }
        }
    }


    #endregion
    #region Métodos Públicos

    /// <summary>
    /// Se encarga de inicializar el sistema
    /// </summary>
    public void Inicializar()
    {


        //Logica Productiva
        if (InDebug) { Debugg(); }

        objProtocolo.RecuperarDatosBD();
        CargarCatalogos();
        VerificarEventos();
        PreparaTimers();


    }

    /// <summary>
    /// Se encarga de finalizar los procesos del sistema
    /// </summary>
    public void Finalizar()
    {

    }

    /// <summary>
    /// Se encarga de poner el sistema en modo sincronizar
    /// </summary>
    public Boolean Sincronizar(string _mensaje)
    {
        Boolean res = false;

        try
        {
            MensajeSync("Iniciando CAN2");

            //Creamos Objeto para sincronizar
            WS _ws = new WS(true);

            //Paso 1:Sincronizacion de testigos
            //res = _ws.syncTestigo(ParametrosInicioCAN.Autobus);

            // MensajeSync("Sincronizacion de Testigos: " + (res ? true : false));

            //Paso 2: Sincronización de los catálogos de Geocercas
            Request_geocerca request = new Request_geocerca();
            request.deviceid = Convert.ToString(ParametrosInicioCAN.Autobus);

            can2_sync can2_sync = (from x in CAN2_BD.can2_sync select x).FirstOrDefault();
            //ver que onda con el formato de la fecha aqui 
            request.fecha_modificacion = DateTime.MinValue.ToString();

            //if (can2_sync != null)
            //{
            //    request.fecha_modificacion = can2_sync.UltimaSync.ToString();
            //}
            //else
            //{
            //    request.fecha_modificacion = Convert.ToString(DateTime.MinValue);
            //}
            res = _ws.syncGeocercas(request);
            MensajeSync("Sincronizacion de CAN2: " + (res ? true : false));

            //Termino de sincronización
            MensajeSync("Finalizando CAN2");
            MensajeSync("");

            can2_sync.UltimaSync = DateTime.Now;
            CAN2_BD.SaveChanges();
        }
        catch (Exception ex)
        {
            MensajeSync("Error: " + ex.ToString());
        }
        return res;

    }

    /// <summary>
    /// Se encarga de rebicir los parametros de evento de viaje
    /// </summary>
    /// <param name="Autobus"></param>
    /// <param name="Operador"></param>
    /// <param name="Tipo"></param>
    /// <param name="Fecha_Apertura"></param>
    /// <param name="Fecha_Cierre"></param>
    /// <param name="Cambio_Manos"></param>
    public void RecibirCANFirma(string _Autobus, string _Operador, string _Tipo,
                                string _Fecha_Apertura, string _Fecha_Cierre, string _Cambio_Manos)
    {


        if (InDebug)
        {
            this.Operador = Convert.ToInt32(9111119);
            this.CambioManos = Convert.ToInt32(0);
        }
        else
        {
            this.Operador = Convert.ToInt32(_Operador);
            this.CambioManos = Convert.ToInt32(_Cambio_Manos);
        }
        GenerarEventoFirma();
    }

    #endregion
    #region Colisionador

    /// <summary>
    /// Paso 1: Se encarga de detectar las colisiones
    /// </summary>
    private List<geocercaParametros> DetectarColisiones()
    {

        List<geocercaParametros> geos = new List<geocercaParametros>();

        this.geocercaActual = 0;
        try
        {
            posicion_Anterior = posicion_Actual;
            Double longG = long_negativo == 1 ? Convert.ToDouble(Datos_GPS.Longitud) * -1 : Convert.ToDouble(Datos_GPS.Longitud);





            posicion_Actual = new POINT(Datos_GPS.Latitud.ToString(), longG.ToString());

            //if (InDebug)
            //{
            //    switch (bandera)
            //    {
            //        case 0:
            //            posicion_Actual = new POINT("0000.0000", "0000.0000");

            //            break;
            //        case 1:
            //            posicion_Actual = new POINT(Datos_GPS.Latitud, longG.ToString());
            //            break;
            //        case 2:
            //            posicion_Actual = new POINT("1925.9759", "-9906.6649");
            //            break;

            //        default:
            //            break;
            //    }
            //}


            if (InDebug)
            {
                EnviarMensaje("PUNTO ACTUAL: " + Datos_GPS.Latitud + " " + longG.ToString());
            }
            if (geocercas.Count != 0)
            {
                //verificar que coincidan los puntos para que haga la colision
                foreach (Response_Geocerca sg in geocercas)
                {
                    POLYGON pol = new POLYGON(sg.points.ToArray());

                    if (pol.in_poligone(posicion_Actual, false))
                    {
                        if (InDebug)
                            Console.WriteLine("En poligono:" + sg.nombre);

                        this.flagColision = true;
                        //logica para detectar cambio de geocerca y estancia de la misma geocerca
                        this.geocercaActual = sg.geocercaId;

                        if (this.geocercaActual != this.geocercaAnterior)
                        {
                            this.geocercaAnterior = this.geocercaActual;
                            //CerrarEvento();
                        }
                        else
                        {
                            //validar logica para permanencia del bus en la misma geocerca
                            if (Angulo)
                                break;
                        }
                        if (InDebug) { EnviarMensaje("ENTRO: " + sg.clave); }
                        float angle_bus = pol.CalculeAngle(posicion_Anterior, posicion_Actual);
                        if (InDebug) { EnviarMensaje("ESTA EN EL POLIGONO ANGLE BUS: " + angle_bus); }


                        foreach (geocercaParametros parametro in sg.geocercaParametros)
                        {
                            if (in_angle(parametro.orientacionInicial, angle_bus, parametro.orientacionFinal))
                            {
                                Angulo = true;

                                if (InDebug)
                                {
                                    EnviarMensaje("<<<<<<<<<<<<<<<<<<<<**********COLISION EN: " + parametro.NombreParametro + "***********>>>>>>>>>>>>>>>>>>>>");
                                    EnviarMensaje("---------->>>>>COORDENADAS: " + Datos_GPS.Latitud + ", " + longG + " EN EL ANGULO: " + angle_bus);
                                }
                                geos.Add(parametro);
                            }
                        }
                        break;
                    }
                    else
                    {
                        this.flagColision = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EnviarMensaje(ex.Message);
        }

        semaforizacionAceleracion(geocercaActual);
        return geos;
    }
    /// <summary>
    /// Detecta que el autobus se encuentre dentro del agunlo
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
    /// se encarga de cerrar los eventos abiertos por los parametros de una geocerca en una colision 
    /// </summary>
    private void CerrarEvento()
    {

        //inicio de cierre de eventos
        try
        {
            if (InDebug)
            {
                EnviarMensaje("Cerrando Evento");

            }
            var listaparametros = (from x in CAN2_BD.can2_parametros
                                   where x.geocerca_id == this.IdGeocerca
                                    && x.Fecha_Fin == DateTime.MinValue
                                   select x).ToList();
            if (listaparametros != null)
            {
                foreach (can2_parametros item in listaparametros)
                {
                    item.Fecha_Fin = DateTime.Now;
                    item.lat_Fin = Convert.ToDouble(Datos_GPS.Latitud);
                    item.long_Fin = Convert.ToDouble(Datos_GPS.Longitud);
                    if (item.parametroId == this.IdFR)
                    {
                        item.valor_real = objProtocolo.VAL_FR_REAL;

                    }
                    else if (item.parametroId == this.IdAceleracion)
                    {
                        item.valor_real = this.Aceleracion;
                    }
                    else
                    {
                        item.valor_real = 0;
                    }

                }
                CAN2_BD.SaveChanges();
            }
        }
        catch (Exception ex)
        {

        }
    }

    #endregion
    #region Timers

    /// <summary>
    /// Se encargrá de ejecutar 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Actualizar_Tick(object sender, ElapsedEventArgs e)
    {

        timerActualiza.Stop();

        try
        {
            //Mandamos a pedir los datos de GPS para tenerlos frescos de este lado
            ActualizarDatosGPS();

            //Mandamos a traer el valor de Aceleracion
            this.Aceleracion = Convert.ToDouble(Aceler());

            //Actualizamos Datos de CAN
            if (objProtocolo != null)
            {
                objProtocolo.ProcesaCAN();
                //Enviamos los datos para el Socket
                EnviarSocket(objProtocolo.VAL_FR_META.ToString(), objProtocolo.VAL_FR_REAL.ToString(), objProtocolo.VAL_PARAMETRO_ID.ToString());
            }

            if (!HiloDeteccionColision.IsAlive)
            {
                HiloDeteccionColision = new Thread(new ThreadStart(Colisionador));
                HiloDeteccionColision.Start();
            }

        }
        catch (Exception ex)
        {

        }

        timerActualiza.Start();
    }
    #endregion
}
