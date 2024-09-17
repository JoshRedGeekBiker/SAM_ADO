using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InterfazSistema;
using InterfazSistema.ModelosBD;
using ADO_CAN_Utilerias;
using ADO_CAN_Cliente_Telemetría;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Ionic.Zip;
using System.Net.NetworkInformation;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Timers;

public class TELEMETRIA : ISistema, IBDContext, IBDContextTs, IGPS
{

    #region "Propiedades"
    public int Clave_operador { get; set; } = 0;
    public string Nombre_Operador { get; set; } = "";
    public bool EnViaje { get; set; } = false;
    public DateTime FechaViaje { get; set; } = DateTime.Now;
    public long IdPob { get; set; } = 0;
    public string DescPob { get; set; } = "";
    public string CvePob { get; set; } = "";
    public string VersionDLL { get; set; } = "";
    #endregion

    #region "Propiedades Heredadas"
    public int OrdenDescarga { get; set; }
    public int OrdenLoad { get; set; }

    public Sistema Sistema { get { return Sistema.TELEMETRIA; } }

    public string GetVersionSistema { get; }

    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }

    //Contexto de Base de datos
    public vmdEntities VMD_BD { get; }
    public telematicsEntities TELEMATICS_BD { get; }
    public GPSData Datos_GPS { get; set; }

    #endregion

    #region "Variables"

    //Dll Cliente de ADOCAN Telemetria
    private ADO_CAN_Cliente_Telemetria Telematics;

    //ContextoAuxiliar
    private telematicsEntities TELEMATICS_BD1;
    private telematicsEntities TELEMATICS_BD2;
    private telematicsEntities TELEMATICS_BD3;

    //Hilos
    private Thread HiloProcesamientoCodigos;

    private Thread HiloRecepcionCodigos;
    private Thread HiloProceso;
    private Thread HiloEnvioPrioritario;
    private Thread HiloEnvioXLote;
    private Thread HiloSyncLote;

    //Parametros
    public can_parametrosinicio ParametrosInicio;
    public parametrostelematics Parametros;


    //Timers
    private System.Windows.Forms.Timer timerEnvio = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerProceso = new System.Windows.Forms.Timer();
    private System.Timers.Timer timerTransponder = new System.Timers.Timer();//Powered ByRED 24JUL2024

    //transponder
    private bool EnviarTransponder = false; //Powered ByRED 24JUL2024


    //Flags
    private bool EnviarLote = false;

    //Para calculo de frenadas y aceleraciones
    private double velAnt = 0;
    private codigo codigoPasado;
    private DateTime ultAcele = DateTime.Now;
    private DateTime ultFren = DateTime.Now;

    //Para las alertas hacia el conductor
    private List<string> AlertasPorMostrar;

    //Obtenemos el catalogo en memoria para no saturar la peticion a BD
    private List<cat_codigo> Catalogo_Codigos;

    //Obtenemos la lista de fallas en memoria para no saturar la petición a la BD
    private List<codigo> Fallas_Mem;

    //Obtenemos la lista de los codigos ya existentes
    private List<codigo> Codigos_motor;

    //obtiene el ultimoIDCodigo
    private long ultimoIDCodigo = 0;

    //Fabian VSP 19ENE2022
    private long ultimoIDFallaEnvio = 0;

    //Obtiene la region que viene configurado el equipo
    private can_referenciaregion region = new can_referenciaregion();

    //Nos sirve para llevar la cuenta del protocolo Telemetria
    private int contadorSinCodigos = 0;

    //Para llevar el control del indicador del Front
    private int EstadoAnterior = 0;

    //Variables en memoria para reporte
    private int CodigosRecibidos = 0;
    private int FallasRecibidas = 0;
    private int CodigosEnviados = 0;
    private int FallasEnviadas = 0;
    private string ultLoteEnviadoNOM = "Sin paquete";
    private string ultLoteEnviadoFech = "0000-00-00 00:00:00";
    private string ultFallEnviada = "Sin registro";
    private string ultFallaModulo = "0;";
    private string ultFallaCode = "0";


    //Powered ByRED 19ENE2022
    private string FirmwareLocal = string.Empty;
    private string ProtocoloLocal = string.Empty;

    public List<Telemetria_Codigo> codigos_temporales = new List<Telemetria_Codigo>();


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
    public TELEMETRIA()
    {
        VMD_BD = new vmdEntities();
        TELEMATICS_BD = new telematicsEntities();
        TELEMATICS_BD1 = new telematicsEntities();
        TELEMATICS_BD2 = new telematicsEntities();
        TELEMATICS_BD3 = new telematicsEntities();

        //Para obtener la versión de la DLL Powered ByRED 15ENE2021
        VersionDLL = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //Reportamos versión a BD
        //ReportarVersion();//Se pasa a método Inicializar Powered ByRED 19ENE2022
    }

    #endregion

    #region "Metodos Publicos"

    /// <summary>
    /// Se encarga de enviar los codigos pendientes de telemetria
    /// antes de realizar una sincronización del móvil
    /// </summary>
    public void CierreDeCodigos()
    {
        try
        {
            if (HiloEnvioXLote != null)
            {
                while (HiloEnvioXLote.IsAlive)
                {
                    //Sólo trabamos el ciclo aquí para evitar dobleProceso
                }

                //Configuramos el hilo
                HiloSyncLote = new Thread(new ThreadStart(CorteDeCodigos));
                //Lanzamos el hilo, para que se genere el lote en segundo plano
                HiloSyncLote.Start();
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// ***Está lógica estaba en el inicio de los sistemas (SAM) pero no debería de estar allí
    /// asi que lo trajimos para acá***
    /// Se encarga de preguntar si tiene que guardar los lotes
    /// de lo contrario los elimina
    /// 21May2020 Powered ByRED
    /// </summary>
    public void VerificarLotes(long GuardarLote)
    {
        if (GuardarLote == 0)
        {
            //Recuperamos los archivos para ver si se tiene que borrar algo
            try
            {
                DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var fi in di.GetFiles("*.zip"))
                {
                    //Validamos si la fecha de creación del archivo es mayor a 2 días
                    if ((DateTime.Now - fi.LastWriteTime).Days >= 2)
                    {
                        //De ser así, elimino la información
                        if (File.Exists(fi.Name))
                        {
                            File.Delete(fi.Name);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// Se encarga de extraer los datos para el reporte
    /// Powered ByRED 15JUN2020
    /// </summary>
    /// <returns></returns>
    public List<string> GenerarReporte()
    {
        List<string> Reporte = new List<string>();

        //Para codigos
        Reporte.Add(CodigosRecibidos.ToString());
        Reporte.Add(CodigosEnviados.ToString());
        Reporte.Add(ultLoteEnviadoFech);
        Reporte.Add(ultLoteEnviadoNOM);

        //Para fallas
        Reporte.Add(FallasRecibidas.ToString());
        Reporte.Add(FallasEnviadas.ToString());
        Reporte.Add(ultFallEnviada);
        Reporte.Add(ultFallaModulo);
        Reporte.Add(ultFallaCode);

        //Version 
        Reporte.Add(Assembly.GetExecutingAssembly().GetName().Version.ToString());


        return Reporte;
    }

    /// <summary>
    /// Se encarga de recuperar el valor de aceleración para CANV2
    /// 09SEP2024 Powered ByRED
    /// </summary>
    /// <returns></returns>
    public string ValorAceleracion()
    {
        var aceler = string.Empty;

        try
        {
            if (codigos_temporales != null)
            {
                aceler = (from x in codigos_temporales
                          where x.LLave == "91"
                          select x.Valor).FirstOrDefault();
            }
        }
        catch (Exception)
        {

            throw;
        }

        return aceler;
    }

    #endregion

    #region "Metodos Privados"

    /// <summary>
    /// Se encarga de generar un signo de vida que será enviada a nube para determinar si está vivo o no.
    /// Powered ByRED 24JUL2024
    /// </summary>
    private void GenerarTransponder()
    {
        try
        {
            ultimoIDCodigo++;
            codigo nuevocodigo = new codigo();

            var Elahora = DateTime.Now;

            nuevocodigo.PK_ID = ultimoIDCodigo;

            nuevocodigo.Modulo = "99";
            nuevocodigo.Codigo1 = "9997";
            nuevocodigo.Valor = "1";

            //Para ser enviado por streaming:
            nuevocodigo.Type_Codigo_Id = 1;
            nuevocodigo.FechaHora_Inicio = Elahora;
            nuevocodigo.FechaHora_Fin = Elahora; //Igualamos la hora, para no tener problemas al guardar
            nuevocodigo.Autobus = ParametrosInicio.Autobus;

            if (Datos_GPS != null)
            {
                nuevocodigo.Lat = Convert.ToDouble(Datos_GPS.Latitud);
                nuevocodigo.Lng = Convert.ToDouble(Datos_GPS.Longitud);
                nuevocodigo.NS = Datos_GPS.LatitudNS;
                nuevocodigo.WE = Datos_GPS.LongitudWE;
            }
            else
            {
                nuevocodigo.Lat = 0;
                nuevocodigo.Lng = 0;
                nuevocodigo.NS = "";
                nuevocodigo.WE = "";
            }

            nuevocodigo.Marca_Id = ParametrosInicio.Marca.ToString();
            nuevocodigo.Region_Id = ParametrosInicio.Region.ToString();
            nuevocodigo.Region_Operativa_Id = ParametrosInicio.IDRegionOperativa.ToString();
            nuevocodigo.Clave_Operador = this.Clave_operador;
            nuevocodigo.Nombre_Operador = (this.Nombre_Operador).Equals("NE") ? "" : this.Nombre_Operador;
            nuevocodigo.Fecha_Evento_Viaje = this.FechaViaje;
            nuevocodigo.Tipo_Viaje = this.EnViaje ? "V" : "T";
            nuevocodigo.Status_Id = 0;

            //Se pone procesado en uno, por que en teoria no tendrían que tener ningún otro proceso
            nuevocodigo.Procesado = 1;
            nuevocodigo.Enviado = 0;
            nuevocodigo.Lote = null;

            //04MAY2020 ByRED
            nuevocodigo.TipoLectura = "0";
            nuevocodigo.Contador = "1";

            //Powered ByRED 13OCT2021
            nuevocodigo.Protocolo = this.ProtocoloLocal;
            nuevocodigo.Firmware = this.FirmwareLocal;
            TELEMATICS_BD1.codigo.Add(nuevocodigo);
        }
        catch
        {

        }
    }
    /// <summary>
    /// Se encarga de reportar la versión a la tabla de Plat_versiones
    /// Powered ByRED 15ENE2021
    /// UPGRADE: Powered ByRED 19ENE2022 - Se plancha la versión de Firmware de la tarjeta
    /// </summary>
    private void ReportarVersion()
    {
        try
        {

            //Obtenemos los datos
            this.ProtocoloLocal = Parametros.Protocolo;
            this.FirmwareLocal = Parametros.Firmware;

            var version = string.Empty;

            if (this.FirmwareLocal != "")
            {
                version = this.VersionDLL + " Firmware " + this.FirmwareLocal;
            }
            else
            {
                version = this.VersionDLL;
            }

            //Mandamos a planchar la version de SAM en la tabla Versiones de PLAT

            var plat_versiones = (from x in VMD_BD.plat_versiones
                                  select x).ToList();

            if (plat_versiones != null)
            {
                var version_telemetria = plat_versiones.Where(x => x.Sistemas.Equals("SAM - Telemetria")).FirstOrDefault();

                //No existe el registro en la tabla, lo agregamos
                if (version_telemetria == null)
                {
                    plat_versiones nuevaVersion = new plat_versiones();

                    nuevaVersion.Sistemas = "SAM - Telemetria";
                    nuevaVersion.Versiones = version;

                    VMD_BD.plat_versiones.Add(nuevaVersion);

                }
                else//sólo planchamos la version
                {
                    version_telemetria.Versiones = version;
                }

                VMD_BD.SaveChanges();
            }
        }
        catch
        {

        }
    }


    /// <summary>
    /// Configuramos los timers/hilos
    /// </summary>
    private void PreparaTimers()
    {
        //Prueba
        HiloProcesamientoCodigos = new Thread(new ThreadStart(ProcesoTelemetria));

        //Se encarga de la recepción de los códigos
        HiloRecepcionCodigos = new Thread(new ThreadStart(RecepcionCodigos));

        //Se encarga de procesar los codigos
        HiloProceso = new Thread(new ThreadStart(ProcesamientoCodigos));

        //se encarga del envió prioritario de los códigos
        HiloEnvioPrioritario = new Thread(new ThreadStart(EnviarCodigosFallas));

        //Se encarga del envio por lote
        HiloEnvioXLote = new Thread(new ThreadStart(CodigosPorLote));

        //Se encarga de que en la sincronización se genere el archivo por lote
        HiloSyncLote = new Thread(new ThreadStart(GenerarArchivoLote));

        //Cada 10 Minutos para el envio de paquetes
        try
        {
            timerEnvio.Interval = Convert.ToInt32(Parametros.Tiempo_Envio);
        }
        catch
        {
            //Por default ponemos 10 minutos
            timerEnvio.Interval = 600000;
        }
        timerEnvio.Enabled = true;
        timerEnvio.Tick += new EventHandler(EnviarPaquete_Tick);
        timerEnvio.Start();

        //timer para debbug
        timerProceso.Interval = 10000;
        timerProceso.Enabled = true;
        timerProceso.Tick += new EventHandler(Debuggear);
        timerProceso.Start();

        //Powered ByRED 24JUL2024

        timerTransponder.Interval = Convert.ToDouble(Parametros.Mins_Transponder * 60000);
        timerTransponder.Enabled = true;
        timerTransponder.Elapsed += Transponder_Tick;
        timerTransponder.Start();
    }

    /// <summary>
    /// Se encarga de recuperar el catalogo de codigos en memoria
    /// </summary>
    private void CatalogosCodigos()
    {
        Catalogo_Codigos = new List<cat_codigo>();

        Catalogo_Codigos = (from x in TELEMATICS_BD.cat_codigo
                             select x).ToList();
    }

    /// <summary>
    /// recupera las fallas que ya existen en la BD
    /// Sirve como buffer para evitar hacer peticiones en exceso
    /// Modificación: 21May2020 ByRED - se incluyó la parametrización del filtro
    /// </summary>
    private void Fallas()
    {
        //21May2020 Powered ByRED
        if (Parametros.FiltroFallas)
        {
            Fallas_Mem = new List<codigo>();

            //Ya no se cargan en el búffer
            //Fallas_Mem = (from x in TELEMATICS_BD.codigo
            //              where x.Type_Codigo_Id == 1
            //              select x).ToList();
        }
    }

    /// <summary>
    /// Recupera los codigos de motor que ya existen en la BD
    /// Sirve como búffer para evitar almacenar datos duplicados
    /// </summary>
    private void CodigosMotor()
    {
        Codigos_motor = new List<codigo>();

        //Codigos_motor = (from x in TELEMATICS_BD.codigo
                         //where x.Type_Codigo_Id == 2
                         //select distin x).ToList();
    }

    /// <summary>
    /// Timer que sirve para debuggear
    /// </summary>
    private void Debuggear(Object sender, EventArgs e)
    {
        timerProceso.Stop();

        //Aquí enuncia el método que quieres debbugear

        //CodigosPorLote();
    }

    /// <summary>
    /// Es el método principal para el procesamiento de de Telemetria
    /// Tarjeta de circulación vencida********
    /// </summary>
    private void ProcesoTelemetria()
    {
        //Se encarga de recibir los codigos
        RecepcionCodigos();

        //Se encarga de procesar los codigos
        ProcesamientoCodigos();

        //Se encarga de enviar los codigos prioritarios
        //EnviarCodigoFalla();

        //Verificamos si tenemos que enviar lote
        if (EnviarLote)
        {
            //EnviarCodigosPorLote();
            EnviarLote = false;
        }
    }

    /// <summary>
    /// Se encarga de Recibir los codigos de parametros y/o fallas
    /// </summary>
    private void RecepcionCodigos()
    {
        RecibirCodigosV2();
        RecibirFallas();

        if(contadorSinCodigos == 0)
        {
            //Reportamos
            ReportarStatusAFront(1);
        }else if (contadorSinCodigos >= 100)
        {
            ReportarStatusAFront(0);
            //Para evitar acomulado de muchos codigos
            contadorSinCodigos = 1;
        }
    }

    /// <summary>
    /// Se encarga de Recibir, completar y almacenar los codigos de parametros
    /// Tarjeta de Circulación Vencida
    /// </summary>
    private void RecibirCodigos()
    {
        try
        {
            //Obtenemos la lista de codigos
            List<Telemetria_Codigo> codigos = new List<Telemetria_Codigo>();
            codigos.AddRange(Telematics.Lista_Codigos);

            var num_codigos = codigos.Count();

            //Verificamos si trajimos codigos o no
            if (num_codigos > 0)
            {
                var version = (from x in codigos
                               select x.Version).FirstOrDefault();

                int i = 0;

                codigo nuevocodigo;

                foreach (Telemetria_Codigo codigo in codigos)
                {

                    var types = RecuperarTypeCodigo2(codigo.LLave);

                    //Validamos si trajimos más de 1 type
                    if (types.Count > 1)
                    {
                        //Para agregarlo a la cuenta
                        num_codigos = num_codigos + (types.Count - 1);
                    }

                    foreach (string type in types)
                    {
                        nuevocodigo = new codigo();
                        var tipo = Convert.ToInt64(type);
                        ultimoIDCodigo++;


                        nuevocodigo.PK_ID = ultimoIDCodigo;
                        nuevocodigo.Modulo = "";
                        nuevocodigo.Codigo1 = codigo.LLave;
                        nuevocodigo.Valor = codigo.Valor;
                        nuevocodigo.Type_Codigo_Id = tipo;
                        nuevocodigo.FechaHora_Inicio = codigo.Fecha_Recepcion;
                        nuevocodigo.Autobus = ParametrosInicio.Autobus;

                        if (Datos_GPS != null)
                        {
                            nuevocodigo.Lat = Convert.ToDouble(Datos_GPS.Latitud);
                            nuevocodigo.Lng = Convert.ToDouble(Datos_GPS.Longitud);
                            nuevocodigo.NS = Datos_GPS.LatitudNS;
                            nuevocodigo.WE = Datos_GPS.LongitudWE;
                        }
                        else
                        {
                            nuevocodigo.Lat = 0;
                            nuevocodigo.Lng = 0;
                            nuevocodigo.NS = "";
                            nuevocodigo.WE = "";
                        }

                        nuevocodigo.Marca_Id = ParametrosInicio.Marca.ToString();
                        nuevocodigo.Region_Id = ParametrosInicio.Region.ToString();
                        nuevocodigo.Region_Operativa_Id = ParametrosInicio.IDRegionOperativa.ToString();
                        nuevocodigo.Clave_Operador = this.Clave_operador;
                        nuevocodigo.Nombre_Operador = (this.Nombre_Operador).Equals("NE") ? "" : this.Nombre_Operador;
                        nuevocodigo.Fecha_Evento_Viaje = this.FechaViaje;
                        nuevocodigo.Tipo_Viaje = this.EnViaje ? "V" : "T";
                        nuevocodigo.Status_Id = 0;

                        //Validamos si es envio por paquete
                        if (tipo == 2)
                        {//Sólo si es tipo envio por paquete se flagea como procesado
                            nuevocodigo.Procesado = 1;
                        }
                        else
                        {//De lo contrario, no
                            nuevocodigo.Procesado = 0;
                        }

                        nuevocodigo.Enviado = 0;
                        nuevocodigo.Lote = null;

                        TELEMATICS_BD.codigo.Add(nuevocodigo);

                        i++;
                    }
                }

                //Comparamos los codigos obtenidos versus los procesados
                if (i == num_codigos)
                {
                    //Si todo sale bien limpio la lista del cliente y guardo mis cambios en Base de datos
                    Telematics.Limpiar_Telemetria_Codigos(version);
                    TELEMATICS_BD.SaveChanges();
                }
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de Recibir, completar y almacenar los codigos de parametros
    /// evitando que se guarden los mismos codigos si tienen el mismo valor anterior
    /// **PARA PRODUCCION**
    /// Versión 2
    /// Modificación: 25May2020 ByRED - se incluyó la parametrización del filtro
    /// Modificación: 24AGO2021 ByRED - Se recibe los dos nuevos parametros: Protocolo y Firmware
    /// </summary>
    private void RecibirCodigosV2()
    {
        try
        {
            //Obtenemos la lista de codigos
            List<Telemetria_Codigo> codigos = new List<Telemetria_Codigo>();
            codigos.AddRange(Telematics.Lista_Codigos);

            var num_codigos = codigos.Count();

            //Verificamos si trajimos codigos o no
            if (num_codigos > 0)
            {
                
                contadorSinCodigos = 0;


                var version = (from x in codigos
                               select x.Version).FirstOrDefault();

                int i = 0;

                codigo nuevocodigo;

                foreach (Telemetria_Codigo codigo in codigos)
                {

                    //Obtenemos la version de Firmware y el nombre de Protocolo
                    //Powered ByRED 19ENE2022
                    if (codigo.Protocolo != this.ProtocoloLocal || codigo.Firmware != this.FirmwareLocal)
                    {
                        updateParTelematics(codigo.Protocolo, codigo.Firmware);
                    }

                    //Logica de diserción

                    //var existente = (from x in Codigos_motor
                    //                 where x.Codigo1 == codigo.LLave
                    //                 select x).FirstOrDefault();

                    //Si ya existe, entonces...
                    //if (existente != null)
                    //{

                    //    if (codigo.Valor == existente.Valor)
                    //    {//si el valor del codigo es igual, entonces...

                    //        sólo aparento que lo tomé en cuenta
                    //        i++;

                    //        Saltamos al siguente codigo por procesar
                    //        continue;
                    //    }
                    //    else
                    //    {//Si el valor del código no es igual, entonces...

                    //        Elimino el codigo de la lista temporal
                    //        Codigos_motor.RemoveAll(c => c.Codigo1 == codigo.LLave);

                    //    }
                    //}

                    //**Sección Filtro codigo**
                    //Powered ByRED 25May2020
                    if (Parametros.FiltroCodigos)
                    {
                        //Logica de diserción por filtro

                        var existente = (from x in Codigos_motor
                                         where x.Codigo1 == codigo.LLave
                                         select x).FirstOrDefault();

                        //Si ya existe, entonces...
                        if (existente != null)
                        {

                            if (codigo.Valor == existente.Valor)
                            {//si el valor del codigo es igual, entonces...

                                //sólo aparento que lo tomé en cuenta
                                i++;

                                //Saltamos al siguente codigo por procesar
                                continue;
                            }
                            else
                            {//Si el valor del código no es igual, entonces...

                                //Elimino el codigo de la lista temporal
                                Codigos_motor.RemoveAll(c => c.Codigo1 == codigo.LLave);

                            }
                        }
                    }

                    //Si no existe o si existia pero fué diferente, continuamos...

                    var types = RecuperarTypeCodigo2(codigo.LLave);

                    //Validamos si trajimos más de 1 type
                    if (types.Count > 1)
                    {
                        //Para agregarlo a la cuenta
                        num_codigos = num_codigos + (types.Count - 1);
                    }

                    foreach (string type in types)
                    {
                        nuevocodigo = new codigo();
                        var tipo = Convert.ToInt64(type);
                        ultimoIDCodigo++;


                        nuevocodigo.PK_ID = ultimoIDCodigo;
                        nuevocodigo.Modulo = "";

                        nuevocodigo.Codigo1 = codigo.LLave;
                        nuevocodigo.Valor = codigo.Valor;
                        nuevocodigo.Type_Codigo_Id = tipo;
                        nuevocodigo.FechaHora_Inicio = codigo.Fecha_Recepcion;
                        nuevocodigo.FechaHora_Fin = codigo.Fecha_Recepcion; //Campo nuevo
                        nuevocodigo.Autobus = ParametrosInicio.Autobus;

                        if (Datos_GPS != null)
                        {
                            nuevocodigo.Lat = Convert.ToDouble(Datos_GPS.Latitud);
                            nuevocodigo.Lng = Convert.ToDouble(Datos_GPS.Longitud);
                            nuevocodigo.NS = Datos_GPS.LatitudNS;
                            nuevocodigo.WE = Datos_GPS.LongitudWE;
                        }
                        else
                        {
                            nuevocodigo.Lat = 0;
                            nuevocodigo.Lng = 0;
                            nuevocodigo.NS = "";
                            nuevocodigo.WE = "";
                        }

                        nuevocodigo.Marca_Id = ParametrosInicio.Marca.ToString();
                        nuevocodigo.Region_Id = ParametrosInicio.Region.ToString();
                        nuevocodigo.Region_Operativa_Id = ParametrosInicio.IDRegionOperativa.ToString();
                        nuevocodigo.Clave_Operador = this.Clave_operador;
                        nuevocodigo.Nombre_Operador = (this.Nombre_Operador).Equals("NE") ? "" : this.Nombre_Operador;
                        nuevocodigo.Fecha_Evento_Viaje = this.FechaViaje;
                        nuevocodigo.Tipo_Viaje = this.EnViaje ? "V" : "T";
                        nuevocodigo.Status_Id = 0;

                        //Validamos si es envio por paquete
                        if (tipo == 2)
                        {//Sólo si es tipo envio por paquete se flagea como procesado
                            nuevocodigo.Procesado = 1;
                        }
                        else
                        {//De lo contrario, no
                            nuevocodigo.Procesado = 0;
                        }

                        nuevocodigo.Enviado = 0;
                        nuevocodigo.Lote = null;

                        //Nuevos campos
                        nuevocodigo.TipoLectura = "0";
                        nuevocodigo.Contador = "1";

                        //Powered ByRED 24AGO2021
                        //nuevocodigo.Protocolo = codigo.Protocolo;
                        //nuevocodigo.Firmware = codigo.Firmware; //Lógica Anterior

                        //Powered ByRED 19ENE2022
                        nuevocodigo.Protocolo = this.ProtocoloLocal;
                        nuevocodigo.Firmware = this.FirmwareLocal;
                       

                        TELEMATICS_BD.codigo.Add(nuevocodigo);

                        //Agregamos el codigo al búffer
                        if (Parametros.FiltroCodigos)
                        {
                            Codigos_motor.Add(nuevocodigo);
                        }

                        i++;
                    }
                }

                //Comparamos los codigos obtenidos versus los procesados
                if (i == num_codigos)
                {
                    //Si todo sale bien limpio la lista del cliente y guardo mis cambios en Base de datos
                    Telematics.Limpiar_Telemetria_Codigos(version);
                    TELEMATICS_BD.SaveChanges();

                    //Llevamos la sumatoria para el reporte
                    CodigosRecibidos = CodigosRecibidos + num_codigos;
                }
            }
            else
            {
                //Incrementamos el contador
                contadorSinCodigos++;
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de flagear para que se genere el codigo de transponder
    /// Powered ByRED 24JUL2024
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Transponder_Tick(object sender, ElapsedEventArgs e)
    {
        timerTransponder.Stop();

        EnviarTransponder = true;

        timerTransponder.Start();
    }

    /// <summary>
    /// Se encarga de Recibir, completar y almacenar los codigos de fallas
    /// Modificación: 25May2020 Powered ByRED
    /// Modificación: 24AGO2021 ByRED - Se recibe los dos nuevos parametros: Protocolo y Firmware
    /// </summary>
    private void RecibirFallas()
    {
        try
        {
            //Obtenemos la lista de Fallas
            List<Telemetria_Falla> fallas = new List<Telemetria_Falla>();
            fallas.AddRange(Telematics.Lista_Fallas);
            var num_codigos = fallas.Count();

            if (num_codigos > 0)
            {

                contadorSinCodigos = 0;

                var version = (from x in fallas
                               select x.Version).FirstOrDefault();

                //int i = 0;

                codigo nuevocodigo;

                //Inicializamos una lista Temporal
                List<codigo> Fallas_Mem_Temp = new List<codigo>();

                foreach (Telemetria_Falla codigo in fallas)
                {
                    ultimoIDCodigo++;
                    nuevocodigo = new codigo();

                    //string strcadena = JsonConvert.SerializeObject(codigo);
                    nuevocodigo.PK_ID = ultimoIDCodigo;

                    //Lineas Productivas
                    nuevocodigo.Modulo = codigo.LLave;
                    nuevocodigo.Codigo1 = codigo.Valor;
                    nuevocodigo.Valor = "";
                    nuevocodigo.Type_Codigo_Id = 1;
                    nuevocodigo.FechaHora_Inicio = codigo.Fecha_Recepcion;
                    nuevocodigo.FechaHora_Fin = codigo.Fecha_Recepcion; //Igualamos la hora, para no tener problemas
                    nuevocodigo.Autobus = ParametrosInicio.Autobus;

                    if (Datos_GPS != null)
                    {
                        nuevocodigo.Lat = Convert.ToDouble(Datos_GPS.Latitud);
                        nuevocodigo.Lng = Convert.ToDouble(Datos_GPS.Longitud);
                        nuevocodigo.NS = Datos_GPS.LatitudNS;
                        nuevocodigo.WE = Datos_GPS.LongitudWE;
                    }
                    else
                    {
                        nuevocodigo.Lat = 0;
                        nuevocodigo.Lng = 0;
                        nuevocodigo.NS = "";
                        nuevocodigo.WE = "";
                    }

                    nuevocodigo.Marca_Id = ParametrosInicio.Marca.ToString();
                    nuevocodigo.Region_Id = ParametrosInicio.Region.ToString();
                    nuevocodigo.Region_Operativa_Id = ParametrosInicio.IDRegionOperativa.ToString();
                    nuevocodigo.Clave_Operador = this.Clave_operador;
                    nuevocodigo.Nombre_Operador = (this.Nombre_Operador).Equals("NE") ? "" : this.Nombre_Operador;
                    nuevocodigo.Fecha_Evento_Viaje = this.FechaViaje;
                    nuevocodigo.Tipo_Viaje = this.EnViaje ? "V" : "T";
                    nuevocodigo.Status_Id = 0;

                    //Se pone procesado en uno, por que en teoria no tendrían que tener ningún otro proceso
                    nuevocodigo.Procesado = 1;

                    nuevocodigo.Enviado = 0;

                    nuevocodigo.Lote = null;

                    nuevocodigo.TipoLectura = codigo.TipoLectura;
                    nuevocodigo.Contador = codigo.Contador;

                    //Powered ByRED 24AGO2021
                    nuevocodigo.Protocolo = codigo.Protocolo;
                    nuevocodigo.Firmware = codigo.Firmware;

                    ////Se valida si no se habia mandado la falla
                    //if (!EvitarFallaDuplicada(nuevocodigo))
                    //{
                    //    nuevocodigo.Enviado = 0;
                    //    //Se agrega al buffer de fallas
                    //    Fallas_Mem.Add(nuevocodigo);
                    //}
                    //else
                    //{//Si ya existia esa falla, no se envía
                    //    nuevocodigo.Enviado = 1;
                    //}
                    //TELEMATICS_BD.codigo.Add(nuevocodigo);
               
                    Fallas_Mem_Temp.Add(nuevocodigo);

                    //i++;
                }


                ////Comparamos los codigos obtenidos versus los procesados
                //if (i == num_codigos)
                //{
                //    //Si todo sale bien limpio la lista del cliente y guardo mis cambios en Base de datos
                //    Telematics.Limpiar_Telemetria_Fallas(version);
                //    TELEMATICS_BD.SaveChanges();
                //}

                //Inserto en el contexto de la base de datos las fallas resultantes del filtro (Si estuviera activo)
                //Powered ByRED 11may2020
                foreach (codigo code in FiltroFallas(Fallas_Mem_Temp))
                {
                    TELEMATICS_BD.codigo.Add(code);
                    //La sumatoria para el reporte Powered ByRED 16JUN2020
                    FallasRecibidas++;
                }

                //limpio la lista del cliente y guardo mis cambios en Base de datos
                Telematics.Limpiar_Telemetria_Fallas(version);
                TELEMATICS_BD.SaveChanges();
                Telematics.PeticionDeFallas(); //Mandamos mensaje
                Fallas_Mem_Temp.Clear(); //Borramos la lista de las fallas ingresadas
            }
            else
            {
                //Incrementamos el contador
                contadorSinCodigos++;
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }


    /// <summary>
    /// Se encarga de guardar la falla en BD en caso de que no haya podido ser enviada
    /// Powered ByRED 20OCT2021
    /// </summary>
    /// <param name="JSON"></param>
    private bool GuardarFallaBD(string _JSON, string _codigo)
    {
        try
        {
            ultimoIDFallaEnvio++;

            falla_envio nuevafallaBD = new falla_envio();

            nuevafallaBD.PK_ID = ultimoIDFallaEnvio;
            nuevafallaBD.JSON = _JSON;
            nuevafallaBD.Codigo = _codigo;
            nuevafallaBD.Enviado = 0;
            TELEMATICS_BD2.falla_envio.Add(nuevafallaBD);
            return true;

        }
        catch (Exception ex)
        {

            return false;
        }
    }
    /// <summary>
    /// Se encarga de enviar las fallas rezagadas
    /// Powered ByRED 20OCT2021
    /// </summary>
    private void EnviarFallaBD()
    {
        try
        {
            falla_envio codigo_Falla = (from x in TELEMATICS_BD2.falla_envio
                                        where x.Enviado == 0
                                        orderby x.PK_ID ascending
                                        select x).FirstOrDefault();

            if (codigo_Falla != null)
            {
                //Consumimos el WebService
                if (JSONWS(codigo_Falla.JSON) > 0)
                {
                    codigo_Falla.Enviado = 1;

                    //Powered ByRED 20OCT2021
                    TELEMATICS_BD2.SaveChanges();

                }
            }
            //else
            //{
            //    //Podriamos mandar a truncar la tabla

            //    if(ultimoIDFallaEnvio != 0)
            //    {
            //        //Truncamos
            //        EjecutarBatTruncate("falla_enviada");

            //        //y reiniciamos el contador de ID
            //        ultimoIDFallaEnvio = 0;
            //    }
            //}
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// Se encarga de filtrar las fallas entrantes
    /// Creado: 25May2020 Powered ByRED
    /// </summary>
    private List<codigo> FiltroFallas(List<codigo> FallasRecibidas)
    {
        //Valido la ejecución del filtro
        if (!Parametros.FiltroFallas)
        {//Si no lo tengo activo, retorno las fallas tal cúal me llegaron, viene con status Enviado en '0'
            return FallasRecibidas;
        }
        else
        {//Ejecuto la lógica de las fallas
            //Lista de Retorno
            List<codigo> FallasXInsertar = new List<codigo>();

            //Primero vemos qué pedo...Separamos las fallas

            //**Fallas iguales
            //Recorro las fallas recibidas en busca de las iguales
            foreach (codigo FallaRecibida in FallasRecibidas)
            {
                //Buscaremos la falla en el búffer para actualizarla
                codigo falla = (from x in Fallas_Mem
                                where x.Modulo == FallaRecibida.Modulo && x.Codigo1 == FallaRecibida.Codigo1
                                select x).FirstOrDefault();

                if (falla != null)
                {//Le actualizamos el contador

                    //Elimino del búffer la falla para actualizarla
                    Fallas_Mem.RemoveAll(c => c.Modulo == falla.Modulo && c.Codigo1 == falla.Codigo1);

                    //Actualizamos los datos de ambas fallas (búffer y recibida) 

                    //Flageamos cómo enviado
                    falla.Enviado = 1;
                    FallaRecibida.Enviado = 1;
                    long cont;
                    try
                    {//Trabajamos con el contador del búffer
                        cont = Convert.ToInt64(falla.Contador);
                        cont++;
                    }
                    catch
                    {
                        cont = 1;
                    }

                    //Planchamos el contador
                    falla.Contador = cont.ToString();
                    FallaRecibida.Contador = cont.ToString();

                    //Y agrego la falla modificada de nuevo al búffer
                    Fallas_Mem.Add(falla);

                    FallasXInsertar.Add(FallaRecibida);
                }
            }

            //**Obtenemos las fallas QUE YA NO ESTÁN EN EL BÚFFER, Para finalizarlos**
            List<codigo> FallasXFinalizar = (from b in Fallas_Mem where !(from r in FallasRecibidas select r.Modulo).Contains(b.Modulo) || !(from r in FallasRecibidas select r.Codigo1).Contains(b.Codigo1) select b).ToList();
            //Recorremos las fallas finalizadas para quitarlas
            foreach (codigo CodigoFinalizar in FallasXFinalizar)
            {
                //Eliminamos el código del búffer
                Fallas_Mem.RemoveAll(c => c.Modulo == CodigoFinalizar.Modulo && c.Codigo1 == CodigoFinalizar.Codigo1);

                //Cambiamos el PK_ID
                ultimoIDCodigo++;

                codigo nuevaFalla = new codigo();

                nuevaFalla.PK_ID = ultimoIDCodigo;
                nuevaFalla.Modulo = CodigoFinalizar.Modulo;
                nuevaFalla.Codigo1 = CodigoFinalizar.Codigo1;
                nuevaFalla.Valor = CodigoFinalizar.Valor;
                nuevaFalla.Type_Codigo_Id = CodigoFinalizar.Type_Codigo_Id;
                nuevaFalla.FechaHora_Inicio = CodigoFinalizar.FechaHora_Inicio;
                //Copletamos el código con la fecha fin
                nuevaFalla.FechaHora_Fin = DateTime.Now;
                nuevaFalla.Autobus = CodigoFinalizar.Autobus;
                nuevaFalla.Lat = CodigoFinalizar.Lat;
                nuevaFalla.Lng = CodigoFinalizar.Lng;
                nuevaFalla.NS = CodigoFinalizar.NS;
                nuevaFalla.WE = CodigoFinalizar.WE;
                nuevaFalla.Marca_Id = CodigoFinalizar.Marca_Id;
                nuevaFalla.Region_Id = CodigoFinalizar.Region_Id;
                nuevaFalla.Region_Operativa_Id = CodigoFinalizar.Region_Operativa_Id;
                nuevaFalla.Clave_Operador = CodigoFinalizar.Clave_Operador;
                nuevaFalla.Nombre_Operador = CodigoFinalizar.Nombre_Operador;
                nuevaFalla.Fecha_Evento_Viaje = CodigoFinalizar.Fecha_Evento_Viaje;
                nuevaFalla.Tipo_Viaje = CodigoFinalizar.Tipo_Viaje;
                nuevaFalla.Status_Id = CodigoFinalizar.Status_Id;
                nuevaFalla.Procesado = CodigoFinalizar.Procesado;
                nuevaFalla.Enviado = 0;
                nuevaFalla.Lote = CodigoFinalizar.Lote;
                nuevaFalla.TipoLectura = CodigoFinalizar.TipoLectura;
                nuevaFalla.Contador = CodigoFinalizar.Contador;


                //y agregamos a lista XAgregar
                FallasXInsertar.Add(nuevaFalla);
            }

            //**Obtenemos las fallas nuevas, PARA AGREGARLAS**
            List<codigo> FallasNuevas = (from r in FallasRecibidas where !(from b in Fallas_Mem select b.Modulo).Contains(r.Modulo) || !(from b in Fallas_Mem select b.Codigo1).Contains(r.Codigo1) select r).ToList();
            //Recorremos las fallas nuevas
            foreach (codigo CodigoNuevo in FallasNuevas)
            {
                //Validamos si es del tipo "Recuperado"
                if (!CodigoNuevo.TipoLectura.Equals("1"))
                {//De no ser así...
                    //La agrego al búffer
                    Fallas_Mem.Add(CodigoNuevo);

                    //Iniciamos su contador
                    CodigoNuevo.Contador = "1";
                }
                //La agrego a la BD
                FallasXInsertar.Add(CodigoNuevo);
            }

            //Al final regreso el resultado del filtro
            return FallasXInsertar;
        }
    }

    /// <summary>
    /// Se encarga de procesar los codidos de acuerdo a su typeAction
    /// </summary>
    private void ProcesamientoCodigos()
    {
        try
        {
            //Obtememos los más antiguos
            List<codigo> CodigosxProcesar = (from x in TELEMATICS_BD1.codigo
                                             where x.Procesado == 0
                                             orderby x.FechaHora_Inicio ascending
                                             select x).Take(250).ToList();
            //Si tenemos al menos un código entramos al procesamiento
            if(CodigosxProcesar.Count > 0)
            {
                foreach (codigo code in CodigosxProcesar)
                {
                    switch (code.Type_Codigo_Id)
                    {
                        //Envio Prioritario
                        case 1:

                            //Se deja por si en un futuo se tiene que realizar algun proceso

                            ////Validamos de que no exista el mismo codigo
                            //if (!EvitarFallaDuplicada(code))
                            //{
                            //    //PrepararEnvio(code, ref ultimoIDEnvio);
                            //    code.Procesado = 1;
                            //    Fallas_Mem.Add(code);
                            //}
                            //else
                            //{//Si ya existe lo flageamos de que ya lo enviados
                            //    code.Procesado = 1;
                            //    code.Enviado = 1;
                            //}
                            break;

                        //Envio por paquete
                        case 2:
                            //Se deja por que en un futuro se tiene que realizarle
                            //algun tipo de proceso
                            
                            break;

                        //Guardar
                        case 3:
                            code.Procesado = 1;
                            break;

                        //Mandar al proceso de revisión de velocidad
                        case 4:
                            try
                            {
                                if(Parametros.CalculoAceleracion == 1)
                                {
                                    CalculoAceleración(code);
                                }
                                code.Procesado = 1;
                            }
                            catch(Exception ex)
                            {
                                var error = ex.ToString();
                            }
                            break;

                       //Mandar mensaje al conductor
                        case 5:
                            break;

                        default:
                            //Si no tenemos un TypeID por ahora
                            // lo tratamos como un codigo de lote
                            code.Type_Codigo_Id = 2;
                            code.Procesado = 1;
                            break;
                    }
                }
                //Generamos un codigo del tipo transponder para rpeortar a nube la ejecución de telematics
                //Powered ByRED 24JUL2024
                if (EnviarTransponder)
                {
                    EnviarTransponder = false;

                    GenerarTransponder();
                }
                TELEMATICS_BD1.SaveChanges();
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de recuperar el type de acuerdo al catalogo de codigos
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private List<string> RecuperarTypeCodigo(string code)
    {

        List<string> types = new List<string>();
        try
        {
            string descomponer = (from x in TELEMATICS_BD.cat_codigo
                                  where x.Codigo == code
                                  select x.Type_Action_Id).FirstOrDefault();
            if (descomponer != null)
            {
                //Si sólo tenemos un type lo mandamos tal cual
                if (descomponer.Length == 1)
                {
                    types.Add(descomponer);
                }
                else if (descomponer.Length > 1)//Si tenemos más de uno, se separan por comas
                {
                    string[] type = descomponer.Split(',');

                    foreach (string caracter in type)
                    {
                        types.Add(caracter);
                    }
                }
                else
                {
                    types.Add("0");
                }
            }
            else
            {
                types.Add("0");
            }

            
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
            types.Add("0");
        }

        return types;

    }

    /// <summary>
    /// Se encarga de recuperar el type de acuerdo al catalogo de codigos
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private List<string> RecuperarTypeCodigo2(string code)
    {

        List<string> types = new List<string>();

        try
        {
            string descomponer = (from x in Catalogo_Codigos
                                  where x.Codigo == code
                                  select x.Type_Action_Id).FirstOrDefault();
            if (descomponer != null)
            {
                //Si sólo tenemos un type lo mandamos tal cual
                if (descomponer.Length == 1)
                {
                    types.Add(descomponer);
                }
                else if (descomponer.Length > 1)//Si tenemos más de uno, se separan por comas
                {
                    string[] type = descomponer.Split(',');

                    foreach (string caracter in type)
                    {
                        types.Add(caracter);
                    }
                }
                else
                {
                    types.Add("0");
                }
            }
            else
            {
                types.Add("0");
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            types.Add("0");
        }

        return types;

    }

    /// <summary>
    /// Se encarga de evitar mandar una falla que ya se haya mostrado anteriormente
    /// </summary>
    /// <param name="code"></param>
    private bool EvitarFallaDuplicada(codigo code)
    {
        try
        {
            codigo falla = (from x in Fallas_Mem
                                  where x.Modulo == code.Modulo && x.Codigo1 == code.Codigo1
                                  select x).FirstOrDefault();
            return falla == null ? false : true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Se encarga de preparar el código para el envío Proritario
    /// Priority: 1 
    /// Tarjeta de Ciruclación Vencida
    /// </summary>
    /// <param name="code"></param>
    private void PrepararEnvio(codigo code, ref long ultimoID)
    {
        try
        {
            codigo_envio nuevo_codigo = new codigo_envio();

            nuevo_codigo.PK_ID = ultimoID++;
            nuevo_codigo.Modulo = code.Modulo;
            nuevo_codigo.Codigo = code.Codigo1;
            nuevo_codigo.Valor = code.Valor;
            nuevo_codigo.Type_Envio_Id = code.Type_Codigo_Id;
            nuevo_codigo.FechaHora = code.FechaHora_Inicio;
            nuevo_codigo.Autobus = code.Autobus;
            nuevo_codigo.Lat = code.Lat;
            nuevo_codigo.Lng = code.Lng;
            nuevo_codigo.NS = code.NS;
            nuevo_codigo.WE = code.WE;
            nuevo_codigo.Marca_Id = code.Marca_Id;
            nuevo_codigo.Region_Id = code.Region_Id;
            nuevo_codigo.Region_Operativa_Id = code.Region_Operativa_Id;
            nuevo_codigo.Clave_Operador = code.Clave_Operador;
            nuevo_codigo.Nombre_Operador = code.Nombre_Operador;
            nuevo_codigo.Fecha_Evento_Viaje = code.Fecha_Evento_Viaje;
            nuevo_codigo.Tipo_Viaje = code.Tipo_Viaje;
            nuevo_codigo.Status_Id = 0;
            nuevo_codigo.Enviado = 0;

            //Si tenemos el código de falla lo agregamos al nuestra lista en memoria
            if (code.Type_Codigo_Id == 1)
            {
                //Fallas_Mem.Add(nuevo_codigo);
            }

            TELEMATICS_BD1.codigo_envio.Add(nuevo_codigo);
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encargará de enviar los codigos de fallas en envió prioritario
    /// Modificación 25May2020 Powered ByRED: se agregaron los ultimos campos de la definición de código
    /// Modificación 24AGO2021 Powered ByRED: Se agregan dos nuevos campos al final de la definición de código: Protocolo - Firmware
    /// </summary>
    private void EnviarCodigoFalla()
    {
        try
        {
            var Elahora = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");

            codigo codigo_Falla = (from x in TELEMATICS_BD2.codigo
                                         where x.Type_Codigo_Id == 1 && x.Procesado == 1 && x.Enviado == 0
                                         orderby x.FechaHora_Inicio ascending
                                         select x).FirstOrDefault();
            if (codigo_Falla != null)
            {
                string json;

                if (this.EnViaje)
                {
                    var tipo_viaje = "V";
                    if (region != null)
                    {//Con viaje y con region
                        json = "{\"Code\":[{" +

                    "\"TYPE\":\"FaultData\"," +
                    "\"Id\":\"" + codigo_Falla.PK_ID + "\"," +
                    "\"Fecha_Hora\":\"" + codigo_Falla.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    "\"Autobus\":\"" + codigo_Falla.Autobus + "\"," +
                    "\"Modulo\":\"" + codigo_Falla.Modulo + "\"," +
                    "\"Codigo\":\"" + codigo_Falla.Codigo1 + "\"," +
                    "\"Valor\":null," +
                    "\"Lat\":" + codigo_Falla.Lat.ToString() + "," +
                    "\"Lng\":" + codigo_Falla.Lng.ToString() + "," +
                    "\"Operador_Cve\":" + codigo_Falla.Clave_Operador.ToString() + "," +
                    "\"Posicion_N\":\"" + codigo_Falla.NS + "\"," +
                    "\"Posicion_W\":\"" + codigo_Falla.WE + "\"," +
                    "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                    "\"Origen_Id\":" + this.IdPob.ToString() + "," +
                    "\"Origen_Desc\":\"" + this.DescPob + "\"," +
                    "\"Destino_Id\":null," +
                    "\"Destino_Desc\":null," +
                    "\"FechaViajeIni\":\"" + codigo_Falla.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    "\"FechaViajeFin\":null," +
                    "\"Region_Id\":" + region.IdRegion + "," +
                    "\"RegionClave\":null," +
                    "\"RegionDesc\":\"" + region.Region + "\"," +
                    "\"RegionOperId\":null," +
                    "\"RegionOperClave\":null," +
                    "\"RegionOperDesc\":null," +
                    "\"Marca_Comercial_Id\":\"" + region.Marca + "\"," +
                    "\"MarcaComercialClave\":null," +
                    "\"Marca_Comercial_Desc\":null," +
                    "\"Zona_Id\":null," +
                    "\"ZonaClave\":\"" + region.Zona + "\"," +
                    "\"Zona_Desc\":null," +
                    "\"Clase_Id\":null," +
                    "\"ClaseClave\":null," +
                    "\"Clase_Desc\":null," +
                    "\"SubModeloId\":null," +
                    "\"SubModeloDesc\":null," +
                    "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                    "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                    //25May2020 Powered ByRED
                    "\"TipoLectura\":\"" + codigo_Falla.TipoLectura + "\"," +
                    "\"Contador\":\"" + codigo_Falla.Contador + "\"," +
                    "\"Fecha_Hora_Fin\":\"" + codigo_Falla.FechaHora_Fin.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    //24AGO2021 Powered ByRED
                    "\"Protocolo\":\"" + codigo_Falla.Protocolo + "\"," +
                    "\"Firmware\":\"" + codigo_Falla.Firmware + "\"}]}";
                    }
                    else
                    {//Con viaje sin region

                        json = "{\"Code\":[{" +

                   "\"TYPE\":\"FaultData\"," +
                   "\"Id\":\"" + codigo_Falla.PK_ID + "\"," +
                   "\"Fecha_Hora\":\"" + codigo_Falla.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                   "\"Autobus\":\"" + codigo_Falla.Autobus + "\"," +
                   "\"Modulo\":\"" + codigo_Falla.Modulo + "\"," +
                   "\"Codigo\":\"" + codigo_Falla.Codigo1 + "\"," +
                   "\"Valor\":null," +
                   "\"Lat\":" + codigo_Falla.Lat.ToString() + "," +
                   "\"Lng\":" + codigo_Falla.Lng.ToString() + "," +
                   "\"Operador_Cve\":" + codigo_Falla.Clave_Operador.ToString() + "," +
                   "\"Posicion_N\":\"" + codigo_Falla.NS + "\"," +
                   "\"Posicion_W\":\"" + codigo_Falla.WE + "\"," +
                   "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                   "\"Origen_Id\":" + this.IdPob.ToString() + "," +
                   "\"Origen_Desc\":\"" + this.DescPob + "\"," +
                   "\"Destino_Id\":null," +
                   "\"Destino_Desc\":null," +
                   "\"FechaViajeIni\":\"" + codigo_Falla.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                   "\"FechaViajeFin\":null," +
                   "\"Region_Id\":null," +
                   "\"RegionClave\":null," +
                   "\"RegionDesc\":null," +
                   "\"RegionOperId\":null," +
                   "\"RegionOperClave\":null," +
                   "\"RegionOperDesc\":null," +
                   "\"Marca_Comercial_Id\":null," +
                   "\"MarcaComercialClave\":null," +
                   "\"Marca_Comercial_Desc\":null," +
                   "\"Zona_Id\":null," +
                   "\"ZonaClave\":null," +
                   "\"Zona_Desc\":null," +
                   "\"Clase_Id\":null," +
                   "\"ClaseClave\":null," +
                   "\"Clase_Desc\":null," +
                   "\"SubModeloId\":null," +
                   "\"SubModeloDesc\":null," +
                   "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                   "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                    //25May2020 Powered ByRED
                    "\"TipoLectura\":\"" + codigo_Falla.TipoLectura + "\"," +
                    "\"Contador\":\"" + codigo_Falla.Contador + "\"," +
                    "\"Fecha_Hora_Fin\":\"" + codigo_Falla.FechaHora_Fin.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    //24AGO2021 Powered ByRED
                    "\"Protocolo\":\"" + codigo_Falla.Protocolo + "\"," +
                    "\"Firmware\":\"" + codigo_Falla.Firmware + "\"}]}";
                    }
                }
                else
                {
                    var tipo_viaje = "T";

                    if (region != null)
                    {//Sin viaje con region

                        json = "{\"Code\":[{" +

                     "\"TYPE\":\"FaultData\"," +
                     "\"Id\":\"" + codigo_Falla.PK_ID + "\"," +
                     "\"Fecha_Hora\":\"" + codigo_Falla.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Autobus\":\"" + codigo_Falla.Autobus + "\"," +
                     "\"Modulo\":\"" + codigo_Falla.Modulo + "\"," +
                     "\"Codigo\":\"" + codigo_Falla.Codigo1 + "\"," +
                     "\"Valor\":null," +
                     "\"Lat\":" + codigo_Falla.Lat.ToString() + "," +
                     "\"Lng\":" + codigo_Falla.Lng.ToString() + "," +
                     "\"Operador_Cve\":" + codigo_Falla.Clave_Operador.ToString() + "," +
                     "\"Posicion_N\":\"" + codigo_Falla.NS + "\"," +
                     "\"Posicion_W\":\"" + codigo_Falla.WE + "\"," +
                     "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                     "\"Origen_Id\":null," +
                     "\"Origen_Desc\":null," +
                     "\"Destino_Id\":" + this.IdPob.ToString() + "," +
                     "\"Destino_Desc\":\"" + this.DescPob + "\"," +
                     "\"FechaViajeIni\":null," +
                     "\"FechaViajeFin\":\"" + codigo_Falla.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Region_Id\":" + region.IdRegion + "," +
                     "\"RegionClave\":null," +
                     "\"RegionDesc\":\"" + region.Region + "\"," +
                     "\"RegionOperId\":null," +
                     "\"RegionOperClave\":null," +
                     "\"RegionOperDesc\":null," +
                     "\"Marca_Comercial_Id\":\"" + region.Marca + "\"," +
                     "\"MarcaComercialClave\":null," +
                     "\"Marca_Comercial_Desc\":null," +
                     "\"Zona_Id\":null," +
                     "\"ZonaClave\":\"" + region.Zona + "\"," +
                     "\"Zona_Desc\":null," +
                     "\"Clase_Id\":null," +
                     "\"ClaseClave\":null," +
                     "\"Clase_Desc\":null," +
                     "\"SubModeloId\":null," +
                     "\"SubModeloDesc\":null," +
                     "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                     "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                    //25May2020 Powered ByRED
                    "\"TipoLectura\":\"" + codigo_Falla.TipoLectura + "\"," +
                    "\"Contador\":\"" + codigo_Falla.Contador + "\"," +
                    "\"Fecha_Hora_Fin\":\"" + codigo_Falla.FechaHora_Fin.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    //24AGO2021 Powered ByRED
                    "\"Protocolo\":\"" + codigo_Falla.Protocolo + "\"," +
                    "\"Firmware\":\"" + codigo_Falla.Firmware + "\"}]}";
                    }
                    else
                    {//Sin viaje y sin Region

                        json = "{\"Code\":[{" +

                     "\"TYPE\":\"FaultData\"," +
                     "\"Id\"\":" + codigo_Falla.PK_ID + "\"," +
                     "\"Fecha_Hora\":\"" + codigo_Falla.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Autobus\":\"" + codigo_Falla.Autobus + "\"," +
                     "\"Modulo\":\"" + codigo_Falla.Modulo + "\"," +
                     "\"Codigo\":\"" + codigo_Falla.Codigo1 + "\"," +
                     "\"Valor\":null," +
                     "\"Lat\":" + codigo_Falla.Lat.ToString() + "," +
                     "\"Lng\":" + codigo_Falla.Lng.ToString() + "," +
                     "\"Operador_Cve\":" + codigo_Falla.Clave_Operador.ToString() + "," +
                     "\"Posicion_N\":\"" + codigo_Falla.NS + "\"," +
                     "\"Posicion_W\":\"" + codigo_Falla.WE + "\"," +
                     "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                     "\"Origen_Id\":null," +
                     "\"Origen_Desc\":null," +
                     "\"Destino_Id\":" + this.IdPob.ToString() + "," +
                     "\"Destino_Desc\":\"" + this.DescPob + "\"," +
                     "\"FechaViajeIni\":null," +
                     "\"FechaViajeFin\":\"" + codigo_Falla.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Region_Id\":null," +
                     "\"RegionClave\":null," +
                     "\"RegionDesc\":null," +
                     "\"RegionOperId\":null," +
                     "\"RegionOperClave\":null," +
                     "\"RegionOperDesc\":null," +
                     "\"Marca_Comercial_Id\":null," +
                     "\"MarcaComercialClave\":null," +
                     "\"Marca_Comercial_Desc\":null," +
                     "\"Zona_Id\":null," +
                     "\"ZonaClave\":null," +
                     "\"Zona_Desc\":null," +
                     "\"Clase_Id\":null," +
                     "\"ClaseClave\":null," +
                     "\"Clase_Desc\":null," +
                     "\"SubModeloId\":null," +
                     "\"SubModeloDesc\":null," +
                     "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                     "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                    //25May2020 Powered ByRED
                    "\"TipoLectura\":\"" + codigo_Falla.TipoLectura + "\"," +
                    "\"Contador\":\"" + codigo_Falla.Contador + "\"," +
                    "\"Fecha_Hora_Fin\":\"" + codigo_Falla.FechaHora_Fin.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    //24AGO2021 Powered ByRED
                    "\"Protocolo\":\"" + codigo_Falla.Protocolo + "\"," +
                    "\"Firmware\":\"" + codigo_Falla.Firmware + "\"}]}";
                    }
                }

                //Consumimos el WebService
                if (JSONWS(json) > 0)
                {
                    //Flageamos de que se proceso por envio por streaming
                    codigo_Falla.Status_Id = 1;

                    codigo_Falla.Enviado = 1;

                   // TELEMATICS_BD2.SaveChanges();

                    //Reportamos...
                    ReportarStatusAFront(3);
                    //Añadimos al contador Powered ByRED 15JUN2020
                    FallasEnviadas++;
                    ultFallEnviada = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ultFallaModulo = codigo_Falla.Modulo;
                    ultFallaCode = codigo_Falla.Codigo1;

                }
                else
                {
                    {   //Powered ByRED 20OCT2021
                        //Lo Almacenamos en Base de datos, para ser enviado posteriormente
                        if (GuardarFallaBD(json, codigo_Falla.Codigo1))
                        {
                            codigo_Falla.Status_Id = 1;
                            codigo_Falla.Enviado = 2;
                        }
                    }
                }
                TELEMATICS_BD2.SaveChanges();

                //Para Prueba de Lógica
                //if (true)
                //{
                //    //Flageamos de que se proceso por envio por streaming
                //    codigo_Falla.Status_Id = 1;

                //    codigo_Falla.Enviado = 1;

                //    TELEMATICS_BD.SaveChanges();
                //}
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }


    /// <summary>
    /// Se encarga de enviar fallas resguardadas en BD y las que apenas se generaron
    /// </summary>
    private void EnviarCodigosFallas()
    {
        try
        {
            //Enviamos los codigos resagados por falta de internet
            EnviarFallaBD();

            //Enviamos Fallas recien horneadas
            EnviarCodigoFalla();

        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de consumir el web services enviandole el JSON
    /// Modificación 25May2020 Powered ByRED: se le agregan las credenciales para consumir el webservice
    /// </summary>
    /// <param name="JSON"></param>
    /// <returns></returns>
    private int JSONWS(string JSON)
    {
        string resultado = "";
        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Parametros.WSSTREAM + "?autobus=" + ParametrosInicio.Autobus);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "POST";
            //25May2020 PoweredByRED
            httpWebRequest.Credentials = new System.Net.NetworkCredential(Parametros.UserWS, Parametros.PassWS);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JSON);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                resultado = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            return 0;
        }

        return Convert.ToInt32(resultado);
    }

    /// <summary>
    /// Se encarga de enviar los codigos por lote
    /// </summary>
    private void CodigosPorLote()
    {
        //Generamos un nuevo archivo de codigos por lote
        GenerarArchivoLote();

        //enviamos todos los archivos de lote pendientes por enviar
        EnviarArchivosLote();
    }

    /// <summary>
    /// Se encarga de generar el archivo de codigos por lote
    /// </summary>
    private void GenerarArchivoLote()
    {
        try
        {
            List<codigo> codigosxpaquete = (from x in TELEMATICS_BD3.codigo
                                            //where x.Type_Codigo_Id == 2 && x.Lote.Equals("")
                                            where x.Type_Codigo_Id == 2 && x.Lote == null
                                            select x).ToList();

            if (codigosxpaquete != null && codigosxpaquete.Count > 0)
            {

                //Reportamos...
                ReportarStatusAFront(2);

                var rutaApp = AppDomain.CurrentDomain.BaseDirectory;

                var nombreArchivo = ParametrosInicio.Autobus + "_" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "T" +
                            DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_0.zip";

                //Generamos los archivos necesarios
                CodigosATexto(rutaApp, GeneradorJsonMasivo(codigosxpaquete), codigosxpaquete.Count);

                //Comprimimos los archivos y lo depositamos en la carpeta de enviospendientes
                Comprimir(nombreArchivo);

                foreach (codigo code in codigosxpaquete)
                {
                    //Flageamos de que se proceso en envió por lote
                    code.Status_Id = 2;
                    //Flageamos de que ya fué procesado 
                    code.Procesado = 1;
                    //Escribimos el nombre del lote en el que se generó
                    code.Lote = nombreArchivo;
                }

                TELEMATICS_BD3.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// se encarga de enviar los archivos en lote que se encuentran en cola
    /// </summary>
    private void EnviarArchivosLote()
    {
        try
        {
            //Primero realizamos un ping, para ver si tenemos conexíón a internet
            if (RealizarPing(Parametros.ServerPing))
            {
                var rutaApp = AppDomain.CurrentDomain.BaseDirectory;

                DirectoryInfo di = new DirectoryInfo(rutaApp);

                foreach (var fi in di.GetFiles("*" + ParametrosInicio.Autobus + "*"))
                {
                    var respuesta = Convert.ToInt32(SendZIPWS(rutaApp + fi.Name));

                    if (respuesta > 0)
                    {
                        //nos aseguramos de que flaggear como enviados los codigos pertenecientes a éste lote

                        List<codigo> codigoslote = (from x in TELEMATICS_BD3.codigo
                                                    where x.Lote.Equals(fi.Name)
                                                    select x).ToList();

                        //Flageamos al codigo como enviado
                        foreach (codigo code in codigoslote)
                        {
                            code.Enviado = 1;
                            //Sumamos al contador, para el reporte Powered ByRED 15JUN2020
                            CodigosEnviados++;
                        }

                        //Guardamos cambios
                        TELEMATICS_BD3.SaveChanges();

                        //Eliminamos el archivo
                        ElimnarArchivo(rutaApp + fi.Name);

                        //Lo guardamos para el reporte Powered ByRED 15JUN2020
                        ultLoteEnviadoNOM = fi.Name;
                        ultLoteEnviadoFech = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de generar la estructura de un Json Masivo
    /// de codigos
    /// Modificación 24AGO2021 Powered ByRED: Se agregan dos nuevos campos al final de la definición de código: Protocolo - Firmware
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private string GeneradorJsonMasivo(List<codigo> codigos)
    {
        var PrimerEjecucion = true;
        var Elahora = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
        var JSON = "{\"Code\": [";

        //Iteramos para generar la estructura del JSON
        foreach (codigo codigo in codigos)
        {
            if (!PrimerEjecucion)
            {
                JSON += "," + Environment.NewLine;
            }
            else
            {
                PrimerEjecucion = false;    
            }

            if(this.EnViaje)
            {
                var tipo_viaje = "V";
                if (region != null)
                {
                    //Si hay Viaje y Region

                    JSON += "{\"TYPE\":\"CustomDataMotor\"," +
                    "\"Id\":\"" + codigo.PK_ID + "\"," +
                    "\"Fecha_Hora\":\"" + codigo.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    "\"Autobus\":\"" + codigo.Autobus + "\"," +
                    "\"Modulo\":null," +
                    "\"Codigo\":" + codigo.Codigo1 + "," +
                    "\"Valor\":\"" + codigo.Valor + "\"," +
                    "\"Lat\":" + codigo.Lat.ToString() + "," +
                    "\"Lng\":" + codigo.Lng.ToString() + "," +
                    "\"Operador_Cve\":" + codigo.Clave_Operador.ToString() + "," +
                    "\"Posicion_N\":\"" + codigo.NS + "\"," +
                    "\"Posicion_W\":\"" + codigo.WE + "\"," +
                    //"\"Fecha_Hora_Descarga\":\"" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                    "\"Origen_Id\":" + this.IdPob.ToString() + "," +
                    "\"Origen_Desc\":\"" + this.DescPob + "\"," +
                    "\"Destino_Id\":null," +
                    "\"Destino_Desc\":null," +
                    "\"FechaViajeIni\":\"" + codigo.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                    "\"FechaViajeFin\":null," +
                    "\"Region_Id\":" + region.IdRegion + "," +
                    "\"RegionClave\":null," +
                    "\"RegionDesc\":\"" + region.Region + "\"," +
                    "\"RegionOperId\":null," +
                    "\"RegionOperClave\":null," +
                    "\"RegionOperDesc\":null," +
                    "\"Marca_Comercial_Id\":\"" + region.Marca + "\"," +
                    "\"MarcaComercialClave\":null," +
                    "\"Marca_Comercial_Desc\":null," +
                    "\"Zona_Id\":null," +
                    "\"ZonaClave\":\"" + region.Zona + "\"," +
                    "\"Zona_Desc\":null," +
                    "\"Clase_Id\":null," +
                    "\"ClaseClave\":null," +
                    "\"Clase_Desc\":null," +
                    "\"ModeloId\":null," +
                    "\"ModeloDesc\":null," +
                    "\"SubModeloId\":null," +
                    "\"SubModeloDesc\":null," +
                    "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                    "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                    //24AGO2021 Powered ByRED
                    "\"Protocolo\":\"" + codigo.Protocolo + "\"," +
                    "\"Firmware\":\"" + codigo.Firmware + "\"}";
                }
                else
                {//si hay Viaje y no hay Region

                    JSON += "{\"TYPE\":\"CustomDataMotor\"," +
                   "\"Id\":\"" + codigo.PK_ID + "\"," +
                   "\"Fecha_Hora\":\"" + codigo.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                   "\"Autobus\":\"" + codigo.Autobus + "\"," +
                   "\"Modulo\":null," +
                   "\"Codigo\":" + codigo.Codigo1 + "," +
                   "\"Valor\":\"" + codigo.Valor + "\"," +
                   "\"Lat\":" + codigo.Lat.ToString() + "," +
                   "\"Lng\":" + codigo.Lng.ToString() + "," +
                   "\"Operador_Cve\":" + codigo.Clave_Operador.ToString() + "," +
                   "\"Posicion_N\":\"" + codigo.NS + "\"," +
                   "\"Posicion_W\":\"" + codigo.WE + "\"," +
                   //"\"Fecha_Hora_Descarga\":\"" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                   "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                   "\"Origen_Id\":" + this.IdPob.ToString() + "," +
                   "\"Origen_Desc\":\"" + this.DescPob + "\"," +
                   "\"Destino_Id\":null," +
                   "\"Destino_Desc\":null," +
                   "\"FechaViajeIni\":\"" + codigo.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                   "\"FechaViajeFin\":null," +
                   "\"Region_Id\":null," +
                   "\"RegionClave\":null," +
                   "\"RegionDesc\":null," +
                   "\"RegionOperId\":null," +
                   "\"RegionOperClave\":null," +
                   "\"RegionOperDesc\":null," +
                   "\"Marca_Comercial_Id\":null," +
                   "\"MarcaComercialClave\":null," +
                   "\"Marca_Comercial_Desc\":null," +
                   "\"Zona_Id\":null," +
                   "\"ZonaClave\":null," +
                   "\"Zona_Desc\":null," +
                   "\"Clase_Id\":null," +
                   "\"ClaseClave\":null," +
                   "\"Clase_Desc\":null," +
                   "\"ModeloId\":null," +
                    "\"ModeloDesc\":null," +
                   "\"SubModeloId\":null," +
                   "\"SubModeloDesc\":null," +
                   "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                   "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                   //24AGO2021 Powered ByRED
                   "\"Protocolo\":\"" + codigo.Protocolo + "\"," +
                   "\"Firmware\":\"" + codigo.Firmware + "\"}";
                }
            }
            else
            {
                var tipo_viaje = "T";
                if (region != null)
                {//Si no hay viaje y si hay Region
                    JSON += "{\"TYPE\":\"CustomDataMotor\"," +
                     "\"Id\":\"" + codigo.PK_ID + "\"," +
                     "\"Fecha_Hora\":\"" + codigo.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Autobus\":\"" + codigo.Autobus + "\"," +
                     "\"Modulo\":null," +
                     "\"Codigo\":" + codigo.Codigo1 + "," +
                     "\"Valor\":\"" + codigo.Valor + "\"," +
                     "\"Lat\":" + codigo.Lat.ToString() + "," +
                     "\"Lng\":" + codigo.Lng.ToString() + "," +
                     "\"Operador_Cve\":" + codigo.Clave_Operador.ToString() + "," +
                     "\"Posicion_N\":\"" + codigo.NS + "\"," +
                     "\"Posicion_W\":\"" + codigo.WE + "\"," +
                     //"\"Fecha_Hora_Descarga\":\"" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                     "\"Origen_Id\":null," +
                     "\"Origen_Desc\":null," +
                     "\"Destino_Id\":" + this.IdPob.ToString() + "," +
                     "\"Destino_Desc\":\"" + this.DescPob + "\"," +
                     "\"FechaViajeIni\":null," +
                     "\"FechaViajeFin\":\"" + codigo.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Region_Id\":" + region.IdRegion + "," +
                     "\"RegionClave\":null," +
                     "\"RegionDesc\":\"" + region.Region + "\"," +
                     "\"RegionOperId\":null," +
                     "\"RegionOperClave\":null," +
                     "\"RegionOperDesc\":null," +
                     "\"Marca_Comercial_Id\":\"" + region.Marca + "\"," +
                     "\"MarcaComercialClave\":null," +
                     "\"Marca_Comercial_Desc\":null," +
                     "\"Zona_Id\":null," +
                     "\"ZonaClave\":\"" + region.Zona + "\"," +
                     "\"Zona_Desc\":null," +
                     "\"Clase_Id\":null," +
                     "\"ClaseClave\":null," +
                     "\"Clase_Desc\":null," +
                     "\"ModeloId\":null," +
                    "\"ModeloDesc\":null," +
                     "\"SubModeloId\":null," +
                     "\"SubModeloDesc\":null," +
                     "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                     "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                     //24AGO2021 Powered ByRED
                     "\"Protocolo\":\"" + codigo.Protocolo + "\"," +
                     "\"Firmware\":\"" + codigo.Firmware + "\"}";
                }
                else
                {//Si no hay Viaje pero tampoco Region

                    JSON += "{\"TYPE\":\"CustomDataMotor\"," +
                     "\"Id\"\":" + codigo.PK_ID + "\"," +
                     "\"Fecha_Hora\":\"" + codigo.FechaHora_Inicio.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Autobus\":\"" + codigo.Autobus + "\"," +
                     "\"Modulo\":null," +
                     "\"Codigo\":" + codigo.Codigo1 + "," +
                     "\"Valor\":\"" + codigo.Valor + "\"," +
                     "\"Lat\":" + codigo.Lat.ToString() + "," +
                     "\"Lng\":" + codigo.Lng.ToString() + "," +
                     "\"Operador_Cve\":" + codigo.Clave_Operador.ToString() + "," +
                     "\"Posicion_N\":\"" + codigo.NS + "\"," +
                     "\"Posicion_W\":\"" + codigo.WE + "\"," +
                     //"\"Fecha_Hora_Descarga\":\"" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Fecha_Hora_Descarga\":\"" + Elahora + "\"," +
                     "\"Origen_Id\":null," +
                     "\"Origen_Desc\":null," +
                     "\"Destino_Id\":" + this.IdPob.ToString() + "," +
                     "\"Destino_Desc\":\"" + this.DescPob + "\"," +
                     "\"FechaViajeIni\":null," +
                     "\"FechaViajeFin\":\"" + codigo.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                     "\"Region_Id\":null," +
                     "\"RegionClave\":null," +
                     "\"RegionDesc\":null," +
                     "\"RegionOperId\":null," +
                     "\"RegionOperClave\":null," +
                     "\"RegionOperDesc\":null," +
                     "\"Marca_Comercial_Id\":null," +
                     "\"MarcaComercialClave\":null," +
                     "\"Marca_Comercial_Desc\":null," +
                     "\"Zona_Id\":null," +
                     "\"ZonaClave\":null," +
                     "\"Zona_Desc\":null," +
                     "\"Clase_Id\":null," +
                     "\"ClaseClave\":null," +
                     "\"Clase_Desc\":null," +
                     "\"ModeloId\":null," +
                    "\"ModeloDesc\":null," +
                     "\"SubModeloId\":null," +
                     "\"SubModeloDesc\":null," +
                     "\"Tipo_Viaje\":\"" + tipo_viaje + "\"," +
                     "\"Operador_Desc\":\"" + this.Nombre_Operador + "\"," +
                     //24AGO2021 Powered ByRED
                     "\"Protocolo\":\"" + codigo.Protocolo + "\"," +
                     "\"Firmware\":\"" + codigo.Firmware + "\"}";
                }
            }   
        }

        //Terminamos de completar la trama
        JSON += "]}";

        return JSON;
    }

    /// <summary>
    /// Se encarga de generar los archivos necesarios para enviar por paquete
    /// </summary>
    /// <param name="JSON"></param>
    /// <param name="numCodigos"></param>
    private void CodigosATexto(string rutaApp, string JSON, int numCodigos)
    {
        var ruta1 = rutaApp + "codigos.txt";

        if (File.Exists(ruta1))
        {
            File.Delete(ruta1);
        }

        using (StreamWriter sw1 = File.CreateText(ruta1))
        {
            sw1.Write(JSON);
        }

        var ruta2 = rutaApp + "parametros.txt";

        if (File.Exists(ruta2))
        {
            File.Delete(ruta2);
        }

        using(StreamWriter sw2 = File.CreateText(ruta2))
        {
            sw2.Write("{\"checksum\":\"" + numCodigos + "\"}");
        }
    }

    /// <summary>
    /// Mandamos a comprimir los archivos
    /// se guardaria en la carpeta de ejecucion de la aplicacion
    /// con el nombre del autobus
    /// </summary>
    private void Comprimir(string nombre)
    {
        using (ZipFile zip = new ZipFile())
        {
            zip.AddFile("codigos.txt");
            zip.AddFile("parametros.txt");
            zip.Save(nombre);
        }
    }

    /// <summary>
    /// Se encarga de eliminar algún archivo
    /// </summary>
    /// <param name="RutaArchivo"></param>
    private void ElimnarArchivo(string RutaArchivo)
    {
        try
        {
            if (File.Exists(RutaArchivo))
            {
                File.Delete(RutaArchivo);
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de enviar el archivo ZIP a NUBE de Telemetria
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private string SendZIPWS(string filePath)
    {
        WebResponse response = null;
        try
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(Parametros.WSPACKAGE + "?autobus=" + ParametrosInicio.Autobus);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            //wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Credentials = new System.Net.NetworkCredential(Parametros.UserWS, Parametros.PassWS);
            Stream stream = wr.GetRequestStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            stream.Write(boundarybytes, 0, boundarybytes.Length);
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(filePath);
            stream.Write(formitembytes, 0, formitembytes.Length);
            stream.Write(boundarybytes, 0, boundarybytes.Length);
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", Path.GetFileName(filePath), Path.GetExtension(filePath));
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            stream.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                stream.Write(buffer, 0, bytesRead);
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            stream.Write(trailer, 0, trailer.Length);
            stream.Close();

            response = wr.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string responseData = streamReader.ReadToEnd();
            return responseData;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        finally
        {
            if (response != null)
                response.Close();
        }
    }

    /// <summary>
    /// Generador de prueba2
    /// </summary>
    /// <param name="codigos"></param>
    /// <returns></returns>
    private string GeneradorJsonMasivo2(List<codigo_envio> codigos)
    {
        var PrimerEjecucion = true;
        var Elahora = DateTime.Now.ToUniversalTime();
        var region = (from x in VMD_BD.can_referenciaregion
                      where x.IdRegion == ParametrosInicio.Region
                      select x).FirstOrDefault();
        var JSON = "{\"Code\": [";

        //Iteramos para generar la estructura del JSON
        foreach (codigo_envio codigo in codigos)
        {
            if (!PrimerEjecucion)
            {
                JSON += ",";
            }
            else
            {
                PrimerEjecucion = false;
            }

            var tipo_viaje = "T";

            JSON += "[1," +
                codigo.PK_ID + "," +
                "\"" + codigo.FechaHora.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                "\"" + codigo.Autobus + "\"," +
                "null," +
                codigo.Codigo + "," +
                codigo.Valor + "," +
                codigo.Lat.ToString() + "," +
                codigo.Lng.ToString() + "," +
                codigo.Clave_Operador.ToString() + "," +
                "\"" + codigo.NS + "\"," +
                "\"" + codigo.WE + "\"," +
                "\"" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                this.IdPob.ToString() + "," +
                "\"" + this.DescPob + "\"," +
                "null," +
                "null," +
                "\"" + codigo.Fecha_Evento_Viaje.ToString("yyy-MM-dd HH:mm:ss") + "\"," +
                "null," +
                region.IdRegion + "," +
                "null," +
                "\"" + region.Region + "\"," +
                "null," +
                "null," +
                "null," +
                "\"" + region.Marca + "\"," +
                "null," +
                "null," +
                "null," +
                "\"" + region.Zona + "\"," +
                "null," +
                "null," +
                "null," +
                "null," +
                "null," +
                "null," +
                "null," +
                "null," +
                "\"" + tipo_viaje + "\"," +
                "\"" + this.Nombre_Operador + "\"]";
        }

        //Terminamos de completar la trama
        JSON += "]}";

        return JSON;
    }

    /// <summary>
    /// se encarga de detectar una aceleración que cobre los niveles permitidos
    /// </summary>
    private void CalculoAceleración(codigo _code)
    {
        try
        {
            var velocidad = Convert.ToDouble(_code.Valor);

            //Por si hubo reset del equipo, no se calcula nada.
            if (velAnt == 0 || codigoPasado == null)
            {
                velAnt = velocidad;
                codigoPasado = _code;
                return;
            }
            else
            {
                //Si las fechas y tiempso son iguales, descarto el codigo y evito el calculo
                if (_code.FechaHora_Inicio == codigoPasado.FechaHora_Inicio)
                {
                    return;
                }

                //Si la diferencia entre el tiempo de los registros supera los 10 segundos, no lo pasamos por el calculo, para evitar
                //registros de evento erroneos
                if((_code.FechaHora_Inicio - codigoPasado.FechaHora_Inicio).TotalSeconds > 10)
                {
                    //Pero si actualizamos los datos
                    velAnt = velocidad;
                    codigoPasado = _code;
                    return;
                }


                //Calculamos el factor de aceleración
                var aceleracion = velocidad - velAnt;

                if (aceleracion > 0)
                {//Significa que hubo aceleracion

                    //Validamos si ésta pasó el nivel permitido
                    if (aceleracion >= Parametros.FactorAceleracion)
                    {
                        //se pone el codigo de aceleración por default
                        //Si hay otros factores que nos ayude a determinar la aceleración se cambiaria el codigo //Fuera de circulacion
                        string codigoGenerado = "9999";

                        //if (Parametros.CalculoAcelerador == 1)
                        //{
                        //    //Validamos que haya sido por pedal de aceleracion
                        //    //Obtenemos la posición del acelerador
                        //    var acelerador = (from x in Codigos_motor
                        //                      where x.Codigo1 == "91"
                        //                      select x).FirstOrDefault();

                        //    if (acelerador != null)
                        //    {
                        //        var posicionAcelerador = Convert.ToDouble(acelerador.Valor);

                        //        if (posicionAcelerador == 0)
                        //        {//Significa que está tomando velocidad, sin acelerar, que pachou allí?

                        //            codigoGenerado = "9996";

                        //        }
                        //        else if (posicionAcelerador < Parametros.PorcentajeAcelerador)
                        //        {//Si es meno que el factor de Acelerador

                        //            //se genera un codigo de aceleracion preventiva
                        //            codigoGenerado = "9997";
                        //        }
                        //    }
                        //}

                        //generamos alerta de aceleracion, CON o SIN parametro de Acelerador

                        //Validamos si no tiene menos de 5 segundos que hicimos el ultimo evento
                        if ((_code.FechaHora_Inicio - ultAcele).TotalSeconds > 5)
                        {
                            ultAcele = _code.FechaHora_Inicio;
                            GenerarAcelFren("99", codigoGenerado, velocidad.ToString(), _code.FechaHora_Inicio);
                        }
                        else
                        {
                            var hola = "esto es una prueba de flujo";
                        }
                    }
                }
                else
                {//significa que hubo desaceleracion
                    aceleracion = aceleracion * -1;

                    //Validamos si ésta pasó el nivel permitido
                    if (aceleracion >= Parametros.FactorDesaceleracion)
                    {
                        //generamos alerta de desaceleracion
                        //Validamos si no tiene menos de 5 segundos que hicimos el ultimo evento
                        if ((_code.FechaHora_Inicio - ultFren).TotalSeconds > 5)
                        {
                            GenerarAcelFren("99", "9998", velocidad.ToString(), _code.FechaHora_Inicio);
                            ultFren = _code.FechaHora_Inicio;
                        }
                        else
                        {
                            var hola = "esto es una prueba de flujo";
                        }

                    }
                }
                velAnt = velocidad;
                codigoPasado = _code;
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de generar un nuevo registro de falla en la BD
    /// Modificado 25May2020: se sumaron los campos nuevos fechahora_fin, contador, historico
    /// </summary>
    /// <param name="_modulo"></param>
    /// <param name="_codigo"></param>
    /// <param name="_valor"></param>
    private void GenerarAcelFren(string _modulo, string _codigo, string _valor, DateTime _fechahora)
    {
        ultimoIDCodigo++;
        codigo nuevocodigo = new codigo();

        nuevocodigo.PK_ID = ultimoIDCodigo;

        //Lineas Productivas
        nuevocodigo.Modulo = _modulo;
        nuevocodigo.Codigo1 = _codigo;
        nuevocodigo.Valor = _valor;

        //Para ser enviado por streaming:
        nuevocodigo.Type_Codigo_Id = 1;
        nuevocodigo.FechaHora_Inicio = _fechahora;
        nuevocodigo.FechaHora_Fin = _fechahora; //Igualamos la hora, para no tener problemas al guardar
        nuevocodigo.Autobus = ParametrosInicio.Autobus;

        if (Datos_GPS != null)
        {
            nuevocodigo.Lat = Convert.ToDouble(Datos_GPS.Latitud);
            nuevocodigo.Lng = Convert.ToDouble(Datos_GPS.Longitud);
            nuevocodigo.NS = Datos_GPS.LatitudNS;
            nuevocodigo.WE = Datos_GPS.LongitudWE;
        }
        else
        {
            nuevocodigo.Lat = 0;
            nuevocodigo.Lng = 0;
            nuevocodigo.NS = "";
            nuevocodigo.WE = "";
        }

        nuevocodigo.Marca_Id = ParametrosInicio.Marca.ToString();
        nuevocodigo.Region_Id = ParametrosInicio.Region.ToString();
        nuevocodigo.Region_Operativa_Id = ParametrosInicio.IDRegionOperativa.ToString();
        nuevocodigo.Clave_Operador = this.Clave_operador;
        nuevocodigo.Nombre_Operador = (this.Nombre_Operador).Equals("NE") ? "" : this.Nombre_Operador;
        nuevocodigo.Fecha_Evento_Viaje = this.FechaViaje;
        nuevocodigo.Tipo_Viaje = this.EnViaje ? "V" : "T";
        nuevocodigo.Status_Id = 0;

        //Se pone procesado en uno, por que en teoria no tendrían que tener ningún otro proceso
        nuevocodigo.Procesado = 1;
        nuevocodigo.Enviado = 0;
        nuevocodigo.Lote = null;

        nuevocodigo.TipoLectura = "0";
        nuevocodigo.Contador = "1";

        ////Se valida si no se habia mandado la falla
        //if (!EvitarFallaDuplicada(nuevocodigo))
        //{
        //    nuevocodigo.Enviado = 0;
        //    //Se agrega al buffer de fallas
        //    Fallas_Mem.Add(nuevocodigo);
        //}
        //else
        //{//Si ya existia esa falla, no se envía
        //    nuevocodigo.Enviado = 1;
        //}
        TELEMATICS_BD1.codigo.Add(nuevocodigo);
    }

    /// <summary>
    /// Se encargará de enviar las alertas al conductor
    /// </summary>
    private void EnviarAlertasConductor()
    {
        if (AlertasPorMostrar.Count > 0)
        {
            var alerta = AlertasPorMostrar.ElementAt(0);

            AlertaSAM(alerta);

            AlertasPorMostrar.RemoveAt(0);
        }

        //AlertaSAM("Esto es una prueba Carnal");
    }

    /// <summary>
    /// Se encarga de realizar un Ping para
    /// comprobar la conectividad de internet
    /// Modificación: 21May2020 Powered ByRED - Se pide en parametro la dirección del server, para homologar versiones con XP
    /// </summary>
    /// <returns></returns>
    private bool RealizarPing(string server)
    {
        //Validamos que no se encuentra el blanco el Parametro
        if (!server.Equals(""))
        {
            Ping MyPing = new Ping();

            //Verificamos la respuesta del Servidor
            if (MyPing.Send(server, 10).Status == IPStatus.Success) { return true; } else{ return false; }
        }
        else
        {
            //Mandamos verdadero, para que intente realizar el envío sin PING
            return true;
        }
    }

    /// <summary>
    /// Se encarga de generar el último paquete de códigos cuando se
    /// sincroniza y vacía la tabla de codigos para ser llenada nuevamente
    /// </summary>
    /// <returns></returns>
    private void CorteDeCodigos()
    {
        //Generemos el último lote
        GenerarArchivoLote();

        //mandamos a borrar la tabla
        try
        {
            var objctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)TELEMATICS_BD3).ObjectContext;

            //Boramos la tabla de codigos de telemetria
            objctx.ExecuteStoreCommand("Truncate Table codigo");

            //Borramos búffer de fallas
            Fallas_Mem.Clear();

            //Borramos búffer de códigos
            Codigos_motor.Clear();
        }
        catch
        {

        }
    }


    ////De aquí para abajo, vienen lógicas traspasadas de Satelite de Telemetria, para empate de lógicas apartir del 21 de Mayo 2020 Powered ByRED

    /// <summary>
    /// Se encarga de mantener sincronizados los catalogos con Nube
    /// 21May2020 Powered ByRED2020
    /// </summary>
    private void SincronizarCatalogos()
    {
        try
        {
            //Primero vamos a preguntar la versión de los catalogos por sincronizar
            var Version = ObtenerVersiones();

            //Validamos si pudimos traer algo, de lo contrario  no entramos en el flujo
            if (!(Version > 0))
            {
                //Significa que no trajo nada o existio un error interno o de conexión
                return;
            }
            //Recuperamos las tablas y sus versiones del móvil      
            List<version_tabla> versionesTablas = (from x in TELEMATICS_BD.version_tabla select x).ToList();

            //Recorremos la lista
            foreach (version_tabla tabla in versionesTablas)
            {
                switch (tabla.NombreTabla)
                {
                    case "cat_codigo":
                        //Comparamos la versión obtenido vs. la versión de tabla del móvil
                        if (Version > Convert.ToInt32(tabla.Version))
                        {
                            //Si la versión del móvil es menor, actualizaremos la tabla
                            if (ActualizarCatCodigos())
                            {
                                //Si no hubo errores, planchamos la nueva versión
                                tabla.Version = Version;
                            }
                        }

                        break;

                    default: break;
                }
            }

            TELEMATICS_BD.SaveChanges();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de consumir un WS para obtener las versines de los catalogos
    /// 21May2020 Powered ByRED
    /// </summary>
    /// <returns></returns>
    private int ObtenerVersiones()
    {
        string resultado = "";

        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Parametros.WSVersion);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new System.Net.NetworkCredential(Parametros.UserWS, Parametros.PassWS);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                resultado = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            return 0;
        }

        return Convert.ToInt32(resultado);
    }

    /// <summary>
    /// Se encarga de recibir la tabla y plancharla en el móvil
    /// 21May2020 Powered ByRED
    /// </summary>
    /// <returns></returns>
    private bool ActualizarCatCodigos()
    {
        try
        {
            //Obtenemos la tabla en string en formato JSON del WS
            var TablaString = ObtenerTablaWS(Parametros.WSCatalogo);


            //Transformamos el JSON en un objeto del tipo Cat_Codigo
            var Result = JsonConvert.DeserializeObject<Nuevocat_codigo>(TablaString);

            //Validamos que la conversión haya exitosa y al menos tengamos un registro
            if (Result.CatalogoCodigos.Count >= 1)
            {

                ////Mandamos a borrar el contenido de la tabla, creando un respaldo en memoria del contenido
                var objctx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)TELEMATICS_BD).ObjectContext;

                //Boramos la tabla de codigos de telemetria
                objctx.ExecuteStoreCommand("Truncate Table cat_codigo");



                //ahora procedemos a llenar de nuevo la tabla

                foreach (cat_codigo nuevaentrada in Result.CatalogoCodigos)
                {
                    TELEMATICS_BD.cat_codigo.Add(nuevaentrada);
                }

                TELEMATICS_BD.SaveChanges();

            }
            else
            {
                return false;
            }

            return true;
        }
        catch(DbEntityValidationException e)
        {

            foreach (var eve in e.EntityValidationErrors)
            {
                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }



            //var error = ex.ToString();
            return false;
        }

    }

    /// <summary>
    /// Se encarga de Mandar a pedir la tabla a un WS
    /// Retorna un String en formato JSON
    /// 21May2020 Powered ByRED
    /// </summary>
    /// <param name="WS"></param>
    /// <returns></returns>
    private string ObtenerTablaWS(string WS)
    {
        string resultado = "";
        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(WS);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new System.Net.NetworkCredential(Parametros.UserWS, Parametros.PassWS);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                resultado = streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
            //EnviarMensaje(error);
            return resultado;
        }

        return resultado;
    }

    /// <summary>
    /// Se encarga de reportar el estado del sistema al front
    /// Powered ByRED 15JUN2020
    /// </summary>
    private void ReportarStatusAFront(int estadoNuevo)
    {
        //Mandamos actualizar sólo si el estado es diferente
        if(estadoNuevo != EstadoAnterior)
        {
            IndicadorLed(estadoNuevo);
            EstadoAnterior = estadoNuevo;
        }
    }

    /// <summary>
    /// Se encarga de actualizar los parametros en los parametros de telemetria
    /// Powered ByRED 19ENE2022
    /// </summary>
    /// <param name="_protocolo"></param>
    /// <param name="_firmware"></param>
    private void updateParTelematics(string _protocolo, string _firmware)
    {
        try
        {
            Parametros.Protocolo = _protocolo;
            Parametros.Firmware = _firmware;

            //Salvamos los cambios
            TELEMATICS_BD.SaveChanges();

            //Actualizamos los parametros
            ParametrosTelematics();

            //Reportamos los cambios a la version
            ReportarVersion();

        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de recargar el contenido de la tabla parametros telematics
    /// Powered ByRED 19ENE2022
    /// </summary>
    private void ParametrosTelematics()
    {
        Parametros = (from x in TELEMATICS_BD.parametrostelematics select x).FirstOrDefault();
    }
    #endregion


    #region "Métodos Heredados"

    /// <summary>
    /// Se encarga de desencadenar los hilos para el procesamiento de los codigos
    /// </summary>
    public void Actualizar()
    {
        try
        {
            if (!EnviarLote)
            {
                if (HiloRecepcionCodigos != null)
                {
                    if (!HiloRecepcionCodigos.IsAlive)
                    {
                        HiloRecepcionCodigos = new Thread(new ThreadStart(RecepcionCodigos));
                        HiloRecepcionCodigos.Start();
                    }
                }

                if (HiloProceso != null)
                {
                    if (!HiloProceso.IsAlive)
                    {
                        HiloProceso = new Thread(new ThreadStart(ProcesamientoCodigos));
                        HiloProceso.Start();
                    }
                }

                if (HiloEnvioPrioritario != null)
                {
                    if (!HiloEnvioPrioritario.IsAlive)
                    {
                        HiloEnvioPrioritario = new Thread(new ThreadStart(EnviarCodigosFallas));
                        HiloEnvioPrioritario.Start();
                    }
                }
            }
            else
            {
                EnviarLote = false;

                if (HiloEnvioXLote != null)
                {
                    if (!HiloEnvioXLote.IsAlive)
                    {
                        HiloEnvioXLote = new Thread(new ThreadStart(CodigosPorLote));
                        HiloEnvioXLote.Start();
                    }
                }
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de inicializar los componentes necesarios del sistema
    /// </summary>
    public void Inicializar()
    {
        //Iniciamos los parametros generales y de telemtria
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio select x).FirstOrDefault();
        //Parametros = (from x in TELEMATICS_BD.parametrostelematics select x).FirstOrDefault();//Lógica Anterior
        
        //Powered ByRED 19ENE2021
        ParametrosTelematics();
        ReportarVersion();//Powered ByRED 19ENE2022

        //21May2020 Powered ByRED 
        //Para mandar a sincronizar los catalogos
        SincronizarCatalogos();

        //Recuperamos el catalogo de codigos
        CatalogosCodigos();

        //Recuperaoms los codigos de motor ya existentes
        CodigosMotor();

        //Recuperamos codigos de fallas ya enviadas
        Fallas();

        ultimoIDCodigo = (from x in TELEMATICS_BD.codigo
                          orderby x.PK_ID descending
                          select x.PK_ID).FirstOrDefault();


        region = (from x in VMD_BD.can_referenciaregion
                  where x.IdRegion == ParametrosInicio.Region
                  select x).FirstOrDefault();

        //Se inicializa las fallas por mostrar al conductor
        AlertasPorMostrar = new List<string>();

        //Configuramos el cliente
        Telematics = new ADO_CAN_Cliente_Telemetría.ADO_CAN_Cliente_Telemetria();

        Telematics.Servidor = Parametros.Servidor;
        Telematics.Puerto = Convert.ToInt32(Parametros.Puerto);
        Telematics.Tiempo_Envio_Evento_Codigo = Convert.ToInt32(Parametros.Tiempo_Envio);
        Telematics.Limite_Cola_Fallas = Convert.ToInt32(Parametros.Limite_Cola_Fallas);

        //Iniciamos el cliente
        Telematics.Iniciar_Cliente();


        //Timers
        PreparaTimers();

        //enviamos todos los archivos de lote pendientes por enviar
        EnviarArchivosLote();
    }

    /// <summary>
    /// Se encarga de finalizar los componentes del sistema
    /// </summary>
    public void Finalizar()
    {
        timerEnvio.Stop();

        if (HiloRecepcionCodigos.IsAlive)
        {
            HiloRecepcionCodigos.Abort();
        }

        if (HiloProceso.IsAlive)
        {
            HiloProceso.Abort();
        }

        if (HiloEnvioPrioritario.IsAlive)
        {
            HiloEnvioPrioritario.Abort();
        }

        if (HiloEnvioXLote.IsAlive)
        {
            HiloEnvioXLote.Abort();
        }

        //Esperamos a que termine de realizar el "corte de caja"
        //De los codigos
        while (HiloSyncLote.IsAlive)
        {
            //Sólo trabamos el proceso aquí para esperar a que termine
            //la creación del archivo de lote
        }

        if (Telematics != null)
        {
            Telematics.Detener_Cliente();
        }
    }

    /// <summary>
    /// Se encarga de sincronizar el sistema
    /// por ahora para la primera versión no existirá
    /// </summary>
    /// <returns></returns>
    public bool Sincronizar()
    {
        return true;
    }

    /// <summary>
    /// Se encarga de iniciar o detener el cliente, segun se requiera
    /// Para evitar que se acomule información en el buffer cuando se
    /// sincronice
    /// </summary>
    /// <param name="Iniciar"></param>
    public void ClienteTelemetria(bool Iniciar)
    {
        if (Iniciar)
        {
            timerEnvio.Start();
            Telematics.Iniciar_Cliente();
        }
        else
        {
            timerEnvio.Stop();
            Telematics.Detener_Cliente();
        }
    }

    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"

    /// <summary>
    /// Se encarga de ejecutar el proceso para el envió de paquetes
    /// </summary>
    private void EnviarPaquete_Tick(object sender, EventArgs e)
    {
        timerEnvio.Stop();

        //Dispara el flag para que envie por lote
        EnviarLote = true;
        
        timerEnvio.Start();
    }
    #endregion
}