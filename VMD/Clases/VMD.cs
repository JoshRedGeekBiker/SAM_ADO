using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using InterfazSistema.ModelosBD;
using System.Data.SqlClient;
using InterfazSistema.WSCAN;
using System.IO;
using Ionic.Zip;
using System.Threading.Tasks;
using System.Reflection;

public class VMD : ISistema, IBDContext
{

    #region "Variables"
    private List<string> ListaPautas = new List<string>(); //PoweredByRED 15JUN2020
    private string UsbSeleccionado = ""; //Powered BYRED 16JUN2020

    //Logicas
    private Utils MyUtils;
    private Spots _Spots; //Powered ByRED 16ABR2021
    #endregion

    #region "Propiedades"
    public int OrdenLoad { get; set; }
    public int OrdenDescarga { get; set; }
    public Sistema Sistema { get { return Sistema.VMD; } }
    public string GetVersionSistema { get; }
    public vmdEntities VMD_BD { get; set; }
    public bool ModoPrueba { get; set; }
    public bool ModoNocturno { get; set; }
    public string error_ { get; set; }
    public autobuses Bus;
    public int PosActRandom { get; set; }
    public int ProgramacionAct { get; set; }
    public int SecuenciaAct { get; set; }
    public int ConsecProgAct { get; set; }
    public DateTime FechaProgAct { get; set; }
    //public int IdPelicula { get; set; }
    public int VolumenActual { get; set; }
    public bool HorarioValido_ { get; set; }
    public int PausaMs { get; set; }
    public bool ipRenovada { get; set; }
    public string[,] ColaVideo { get; set; }
    int ArchivoAct { get; set; }
    public bool detenida { get; set; } = false;

    

    public can_parametrosinicio parametros { get; set; }

    //Powered ByRED 16JUL2020
    public parametros_vmd parametrosVMD { get; set; }

    //Powered ByRED 15ENE2020
    public string VersionDLL { get; set; } = "";
    #endregion

    #region variables en inicio de proyecto original
    public bool ListandoMensPred;
    #endregion

    /// <summary>
    /// Constructor principal
    /// </summary>
    public VMD()
    {
        ColaVideo = new string[10, 10];
        //VMD_BD = new vmdEntities();
        //Bus = VMD_BD.autobuses.FirstOrDefault();
        //parametros = VMD_BD.can_parametrosinicio.FirstOrDefault();
        //parametrosVMD = VMD_BD.parametros_vmd.FirstOrDefault();

        RecargaBDVMD(); //Powered ByRED 15JUN2021
        MyUtils = new Utils();

        //Powered ByRED 15ENE2020
        VersionDLL = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        ReportarVersion();
    }

    #region "Metodos Publicos"
    public void Actualizar()
    {
        //throw new NotImplementedException();
    }
    public string Eventos()
    {
        throw new NotImplementedException();
    }
    public void Finalizar()
    {
        //throw new NotImplementedException();
    }
    public void Inicializar()
    {
        // if (VMD_BD != null)
        // {
        //     if (!VMD_Inicializado)
        //     {
        //         inicializaVMD();
        //         VMD_Inicializado = true;
        //     }
        //     //ParametrosVMD.Autobus = "6666";
        //     
        //     //BD.SaveChanges();
        // }

        ListaPautas = MyUtils.RecuperarScripts(parametros.CarpetaVideos);
    }
    public bool Sincronizar()
    {
        throw new Exception("NO SE HACE SINCRONIZACIÓN SIN PARÁMETROS");
    }

    /// <summary>
    /// Método para sincronizar VMD por el método tradicional de VMD 7
    /// </summary>
    /// <param name="_SqlConServer"></param>
    /// <param name="Log"></param>
    /// <returns></returns>
    public bool Sincronizar(SqlConnection _SqlConServer, ref List<string> Log)
    {

        bool exito = false;
        try
        {
            var AllLogProgramacion = VMD_BD.logprogramacion.ToList();
            var QueryInsert = string.Empty;
            foreach (var e in AllLogProgramacion)
            QueryInsert += "Insert Into Logprogramacion ( " +
                          "Id" +
                          ",rutavideo" +
                          ",idvideo" +
                          ",minutosmax" +
                          ",ejecucion" +
                          ",error" +
                          ",latitud" +
                          ",longitud" +
                          ",autobus" +
                          ",fechahora" +
                          ",secuencia" +
                          ",consecsecuencia" +
                          ",idprogramacion" +
                          ",fechaprogramacion) Values (" +
                          "'" + e.Id + "'" +
                          ",'" + e.rutavideo + "'" +
                          ",'" + e.idvideo + "'" +
                          ",'" + e.minutosmax + "'" +
                          ",'" + (e.ejecucion == true ? "1" : "0") + "'" +
                          ",'" + e.error + "'" +
                          ",'" + e.latitud + "'" +
                          ",'" + e.longitud + "'" +
                          ",'" + e.autobus + "'" +
                          ",'" + e.fechahora.ToString("yyyy-MM-ddTHH:mm:ss") + "'" +
                          ",'" + e.secuencia + "'" +
                          ",'" + e.consecsecuencia + "'" +
                          ",'" + e.idprogramacion + "'" +
                          ",'" + e.fechaprogramacion.ToString("yyyy-MM-ddTHH:mm:ss") + "' ) ; \n\r";

           
            var comando = new SqlCommand();
            comando.Connection = _SqlConServer;
            comando.CommandText = QueryInsert;
            var Insert = comando.ExecuteNonQuery();
            
            //Powered ByRED 16ABRL2021 Por nueva logica de spots
            //if (Insert > 0)
            //{
            //    var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
            //    objCtx.ExecuteStoreCommand("truncate table logprogramacion");
            //    //evMensajeSincronizacion("Sincronización lograda", 1);
            //    return true;
            //};
            //this.error_ = "La sincronización no se logró, revisar fallo en Transacción SQL";
            //evMensajeSincronizacion(this.error_, 0);
            //return false;


            if (Insert > 0)
            {
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
                objCtx.ExecuteStoreCommand("truncate table logprogramacion");
                exito = true;
            }
            else
            {
                this.error_ = "La sincronización no se logró, revisar fallo en Transacción SQL";
                evMensajeSincronizacion(this.error_, 0);
            }

            
            //Preguntamos si mandamos a sincronizar Spots
            //Powered ByRED 20OCT2022
            if ((bool)parametros.SPOTS)
            {
                //Mandamos sincronizar Spots
                //Powered ByRED 16ABR2021
                SincronizarSpots();
            }
            
            return exito;
        }
        catch (Exception oe)
        {
            this.error_ = oe.Message;
            evMensajeSincronizacion(this.error_, 0);
            return exito;
        }

    }


    /// <summary>
    /// Método para sincronizar VMD por el nuevo método de NUBE VMD 8
    /// </summary>
    /// <param name="WSCAN"></param>
    /// <param name="Log"></param>
    /// <returns></returns>
    public bool Sincronizar(ServiceClient WSCAN, ref List<string> Log)
    {
        string nombre = "LogProgramacion";

        bool exito = false;

        try
        {
            ////Generamos el archivo a mandar
            int numeroLog = logProgATexto(nombre);

            //Comprimimos el archivo
            Comprimir(nombre);

            //Enviamos los logprogramación
            if (EnviarLogProgramacion(nombre, numeroLog, WSCAN))
            {
                //procedemos a borrar los registros de LogProgramación del server
                exito = true;
            }

            //Preguntamos si mandamos a sincronizar Spots
            //Powered ByRED 20OCT2022
            if ((bool)parametros.SPOTS)
            {
                //Mandamos sincronizar Spots
                //Powered ByRED 16ABR2021
                SincronizarSpots();
            }

        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }

        return exito;
    }

    /// <summary>
    /// Se encarga de pedir el nombre de las pautas
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerPautas(string tipo)
    {
        switch (tipo)
        {
            case "USB":
                return MyUtils.ObtenerDiscos();

            default:
                return ListaPautas;
        }
    }
  
    /// <summary>
    /// Se encarga de recibir el tipo y nombre de la pauta
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_nombrePauta"></param>
    /// <returns></returns>
    public Task<bool> CargarPauta(string _tipo, string _nombrePauta)
    {
        switch (_tipo)
        {
            case "USB":
                MyUtils.InsertarNuevaPauta(UsbSeleccionado + "\\PAUTA\\" + _nombrePauta);
                var resultado = MyUtils.TransferirArchivos(parametros.CarpetaVideos, UsbSeleccionado);

                parametrosVMD.VersionPauta = MyUtils.PautaActual;
                VMD_BD.SaveChanges();

                RecargaBDVMD();//Powered ByRED 15JUN2021

                return resultado;

            default:

                var resultado2 = MyUtils.InsertarNuevaPauta(parametros.CarpetaVideos + "\\PAUTA\\" + _nombrePauta);

                parametrosVMD.VersionPauta = MyUtils.PautaActual;
                VMD_BD.SaveChanges();

                RecargaBDVMD();//Powered ByRED 15JUN2021

                return resultado2;
        }
    }

    /// <summary>
    /// Se encarga de validar la versión de la Pauta, antes de insertarla
    /// Powered ByRED 16JUL2020 
    /// </summary>
    /// <param name="_nombrePauta"></param>
    /// <returns></returns>
    public Task<bool> ValidadPauta(string _tipo, string _nombrePauta)
    {
        var parametrosVMD = (from x in VMD_BD.parametros_vmd select x).FirstOrDefault();

        switch (_tipo)
        {
            case "USB":
                return MyUtils.ValidarVersionPauta(UsbSeleccionado + "\\PAUTA\\" + _nombrePauta, parametrosVMD.VersionPauta);
            default:
                return MyUtils.ValidarVersionPauta(parametros.CarpetaVideos + "\\PAUTA\\" + _nombrePauta, parametrosVMD.VersionPauta);
        }
    }

    /// <summary>
    /// Se encarga de recuperar los scripts de los medios extraibles
    /// </summary>
    /// <param name="_LetraUnidad"></param>
    /// <returns></returns>
    public List<string> RecuperarScripts(string _letraUnidad)
    {
        UsbSeleccionado = _letraUnidad + "MOVIES";
        return MyUtils.RecuperarScripts(UsbSeleccionado);
    }

    public int ProgesoCopiado()
    {
        int progreso = 0;

        try
        {
            progreso = Convert.ToInt32(MyUtils.ProgresoCopiado);
        }
        catch
        {
            
        }

        return progreso;
    }


    #endregion

    #region "Metodos Privados"

    /// <summary>
    /// Se encarga de convertir los registros de 
    /// LogProgramacion de VMD a .txt
    /// </summary>
    /// <param name="_nombre"></param>
    private int logProgATexto(string _nombre)
    {
        //para la cuenta
        int i = 0;
        string ruta = AppDomain.CurrentDomain.BaseDirectory + _nombre + ".txt";

        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }

        var AllLogProgramacion = VMD_BD.logprogramacion.ToList();

        using (StreamWriter sw = File.CreateText(ruta))
        {
            foreach (logprogramacion log in AllLogProgramacion)
            {
                string Linea = log.Id + "|" +
                    log.rutavideo + "|" +
                    log.idvideo + "|" +
                    log.minutosmax + "|" +
                    (log.ejecucion == true ? "1" : "0") + "|" +
                    log.error + "|" +
                    log.latitud + "|" +
                    log.longitud + "|" +
                    log.autobus + "|" +
                    log.fechahora.ToString("yyyy-MM-dd HH:mm:ss") + "|" +
                    log.secuencia + "|" +
                    log.consecsecuencia + "|" +
                    log.idprogramacion + "|" +
                    log.fechaprogramacion.ToString("yyyy-MM-dd HH:mm:ss");

                sw.WriteLine(Linea);
                i++;
            }
        }

        return i;
    }

    /// <summary>
    /// Se encarga de comprimirel archivo de texto
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
    /// se encarga de enviar el logprogramación al WSCAN
    /// </summary>
    /// <param name="_nombre"></param>
    /// <param name="numeroLog"></param>
    /// <returns></returns>
    private bool EnviarLogProgramacion(string _nombre, int numeroLog, ServiceClient WSCAN)
    {
        string ruta = AppDomain.CurrentDomain.BaseDirectory + _nombre + ".zip";

        var arreglo = System.IO.File.ReadAllBytes(ruta);

        var regreso = string.Empty;

        regreso = WSCAN.RecibeLogVMD(arreglo, numeroLog);

        return regreso.Equals("Correcto") ? true : false;
    }


    /// <summary>
    /// Se encarga de reportar la versión a la tabla de Plat_versiones
    /// Powered ByRED 15ENE2021
    /// </summary>
    private void ReportarVersion()
    {
        try
        {
            //Mandamos a planchar la version de SAM en la tabla Versiones de PLAT

            var plat_versiones = (from x in VMD_BD.plat_versiones
                                  select x).ToList();

            if (plat_versiones != null)
            {
                var version_vmd = plat_versiones.Where(x => x.Sistemas.Equals("SAM - VMD")).FirstOrDefault();

                //No existe el registro en la tabla, lo agregamos
                if (version_vmd == null)
                {
                    plat_versiones nuevaVersion = new plat_versiones();

                    nuevaVersion.Sistemas = "SAM - VMD";
                    nuevaVersion.Versiones = this.VersionDLL;

                    VMD_BD.plat_versiones.Add(nuevaVersion);

                }
                else//sólo planchamos la version
                {
                    version_vmd.Versiones = this.VersionDLL;
                }

                VMD_BD.SaveChanges();
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de crear y ejecutar la lógica de 
    /// sincronización de Spots
    /// Powered ByRED 16ABR2021
    /// </summary>
    private void SincronizarSpots()
    {
        try
        {
            if(_Spots == null)
            {
                //Creamos lógica
                _Spots = new Spots();

                //Asignamos Eventos
                _Spots.EventoSync += this.RecibeLogSyncSpots;

                //iniciamos Lógica
                _Spots.SincronizarSpots();

                //Desechamos la lógica
                _Spots = null;
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Recibe el log de sincronización de la lógica
    /// de Spots
    /// Powered ByRED 16ABR2021
    /// </summary>
    private void RecibeLogSyncSpots(string _log)
    {
        try
        {
            evMensajeSincronizacion(_log, 0);
        }
        catch
        {

        }
    }

 
    /// <summary>
    /// Se encarga de refrescar los datos del entity
    /// Powered ByRED 15JUN2021
    /// </summary>
    private void RecargaBDVMD()
    {
        VMD_BD = new vmdEntities();
        Bus = VMD_BD.autobuses.FirstOrDefault();
        parametros = VMD_BD.can_parametrosinicio.FirstOrDefault();
        parametrosVMD = VMD_BD.parametros_vmd.FirstOrDefault();
        //VMD_BD.Entry(VMD_BD).Reload();
    }
    

    #endregion

    #region "Metodos de VMD"

    public void BuscaPauta()
    {
        try
        {
            var CualPauta = parametros.PautaAutomatica.ToString();
            var IdPauta = 0;
            var IdProgPauta = 0;
            if (CualPauta.Length > 2)
            {
                IdPauta = Convert.ToInt32(CualPauta.Substring(2));
                if (IdPauta > 0)
                {
                    
                    var catpautas = VMD_BD.catpautas.Where(x => x.idpauta == IdPauta).FirstOrDefault();
                    if (catpautas != null)
                    {
                        IdProgPauta = catpautas.idprogramacion;
                        
                        var CamposInicio = VMD_BD.can_parametrosinicio.FirstOrDefault();
                        var QuerySecuencia = "insert into secuencias (idsecuencia, region, marca, zona, autobus, fechahorareal, ejecutada) values(-1,'" + CamposInicio.Region.ToString() + "','" + CamposInicio.Marca.ToString() + "','" + CamposInicio.Zona.ToString() + "','" + CamposInicio.Autobus.ToString() + "',now(),0)";
                        var QueryDetSecuencias = "insert into detsecuencias (idsecuencia, consec, idprogramacion, fecha, ejecutada) values(-1,1," + IdProgPauta + ",now(),0)";
                        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
                        objCtx.ExecuteStoreCommand(QuerySecuencia);
                        objCtx.ExecuteStoreCommand(QueryDetSecuencias);
                        inicializaVMD();
                        muestraPantalla = "Pauta " + CualPauta + "cargada \n\r" + catpautas.despauta.ToString();
                    }
                    else
                    {
                        muestraPantalla = "Pauta " +CualPauta + "no existe";
                    }
                }
            }
        }
        catch (Exception oe)
        {
            this.error_ = oe.Message;
            MuestraError(oe.Message);
        }
    }


    public void ReiniciarSecuencia(bool SeDetieneVideo)
    {
        AgregaLog("", 0, 0, true, "Se reinicia secuencia, por fin de secuencia normal: " + parametros.PautaAutomatica);
        PantallaVMD = "Reiniciando Secuencia \n\r Espere por favor...";
        
        if (System.IO.File.Exists(parametros.CarpetaVideos))
            return;
        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        objCtx.ExecuteStoreCommand("Truncate actividad;");
        objCtx.ExecuteStoreCommand("Truncate secuencias;");
        objCtx.ExecuteStoreCommand("Truncate detsecuencias;");
        if (!SeDetieneVideo)
        {
            ValidaComando(VMD_BD.can_parametrosinicio.FirstOrDefault().PautaAutomatica.ToString());
        }
    }
    public void ValidaComando(string cmd)
    {

        if (cmd.Length <= 0) return;
        if (cmd == parametros.CodigoSalir.ToString())
        { Salir(false); return; }
        if (cmd.Equals(parametros.CodigoReinicio.ToString()))
        {
            // Shell (Param_RutaVideo ), es ver quien va a invocar esta rutina SHell
            Salir(false);
            return;
        }
        if (cmd.Equals(parametros.CodigoBuscaSecuencia.ToString()))
        {
            BuscarSecuencia();
            return;
        }

        if (cmd.Equals(parametros.CodigoCargaUSB.ToString()))
        {
            CargaUSB();
            return;
        }

        if (cmd.Equals(parametros.CodigoMensajes.ToString()))
        {
            if (!ListandoMensPred)
            { ListaMensajePredefinidos(); }
            else { LimpiaMensPred(); }
            return;
        }

        if (cmd.Substring(0, 2).Equals(parametros.CodigoPautas.ToString()))
        {
            BuscaPauta();
            return;
        }
        else
        {
            this.PantallaVMD = "Comando VMD Inválido";
        }
        MuestraError(this.PantallaVMD);

        return;

    }
    public void ActualizarInfoSecuencia(double posicion, double longitud)
    {
        var StringQuery = string.Empty;
        var ElementoActivo = VMD_BD.actividad.Where(x => x.idsecuencia == SecuenciaAct).FirstOrDefault();
        if (ElementoActivo == null)
        {
            StringQuery = "Insert Into Actividad ( " +
                          " IdSecuencia, " +
                          " Idprogramacion, " +
                          " consecsecuencia, " +
                          " consec, " +
                          " posicion, " +
                          " longitud, " +
                          " posrandom) Values (" +
                          " '" + SecuenciaAct + "', " +
                          " '" + ProgramacionAct + "', " +
                          " '" + ConsecProgAct + "', " +
                          " '" + ArchivoAct + "', " +
                          " '" + posicion + "', " +
                          " '" + longitud + "', " +
                          " '" + PosActRandom + "') ";
        }
        else
        {

            StringQuery = "update actividad set idsecuencia=" + SecuenciaAct + 
                          ", idprogramacion=" + ProgramacionAct + 
                          ", consecsecuencia=" + ConsecProgAct + 
                          ", consec=" + ArchivoAct + 
                          ", posicion=" + posicion + 
                          ", longitud=" + longitud + 
                          ", posrandom=" + PosActRandom;
        }
        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        objCtx.ExecuteStoreCommand(StringQuery);
    }


    public void ChecaSigtVideo()
    {
        try
        {

        int IdReg, IdArch, Progx;
        bool DetenerArch;
        if (SecuenciaAct != 0 && ProgramacionAct > 0 && ConsecProgAct > 0 && ArchivoAct > 0)
        {
            var secuencia = VMD_BD.secuencias.OrderBy(x => x.fechahorareal).FirstOrDefault();
            if (secuencia != null)
            {
                if (!Convert.ToBoolean(secuencia.ejecutada))
                {
                    if (secuencia.idsecuencia == SecuenciaAct)
                    {
                        var detsecuencias = VMD_BD.detsecuencias.Where(x => x.idsecuencia == SecuenciaAct && x.ejecutada == 0).OrderBy(x => x.consec).FirstOrDefault();
                        if (detsecuencias != null)
                            Progx = Convert.ToInt32(detsecuencias.idprogramacion);
                        else
                            Progx = 0;
                        if (Progx != ProgramacionAct)
                        {
                            ArchivoAct = 0;
                            PosActRandom = -1;
                        }
                        var programacion = VMD_BD.programacion.Where(x => x.Id == Progx && x.Consec > ArchivoAct).OrderBy(x => x.Consec).FirstOrDefault();
                        if (programacion != null)
                        {
                            if (programacion.TipoPunto == 1)
                            {

                                if (programacion.TipoReprodRandom)
                                {
                                    IdReg = programacion.Consec;
                                    IdArch = programacion.Archivo;
                                    ArchivoAct = programacion.Consec;
                                    PosActRandom = -1;
                                    DetenerArch = programacion.Detener;
                                        var InitDateTime = new DateTime(1900, 01, 01, 00, 00, 00);
                                    var RandValue = (from rnd in VMD_BD.random
                                                     join arh in VMD_BD.archivos on rnd.idarchivo equals arh.id
                                                     where  rnd.idruta == ProgramacionAct && rnd.idreg == (IdReg - 1) && arh.UltimaVez == InitDateTime
                                                     select new { rnd.idarchivo, arh.Descripcion, arh.UltimaVez }).Count();
                                    if (RandValue == 0)
                                    {
                                        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
                                        objCtx.ExecuteStoreCommand(" update archivos as arc set arc.ultimavez = '1900-01-01 00:00:00'" + " where  arc.id in ( select ran.idArchivo from random as ran where ran.idruta = " + ProgramacionAct.ToString() + "   and ran.idreg=" + (IdReg - 1).ToString() + ")");
                                    }

                                    var RandLst2 = (from rnd in VMD_BD.random
                                                    join arh in VMD_BD.archivos on rnd.idarchivo equals arh.id
                                                    where rnd.idruta == ProgramacionAct && rnd.idreg == (IdReg - 1) && arh.UltimaVez == InitDateTime
                                                    select new { rnd.idarchivo, rnd.consec }).ToList();
                                    RandLst2.Shuffle();
                                    var RandValue2 = RandLst2.FirstOrDefault();

                                    if (RandValue2 != null)
                                    {
                                        IdArch = RandValue2.idarchivo;
                                        PosActRandom = RandValue2.consec;
                                    }

                                }
                                else
                                {
                                    IdArch = programacion.Archivo;
                                    ArchivoAct = programacion.Consec;
                                    DetenerArch = programacion.Detener;
                                }

                                var ruta = VMD_BD.archivos.Where(x => x.id == IdArch).FirstOrDefault().Ruta;
                                if (ruta != null)
                                {
                                        Play(ruta,
                                        IdArch,
                                        (int)VMD_BD.can_parametrosinicio.FirstOrDefault().TiempoUltimaVez,
                                        DetenerArch, false, true, -1);
                                }
                                else
                                { return; }
                            }
                            else
                            {
                                IdArch = programacion.Archivo;
                                ArchivoAct = programacion.Consec;
                                DetenerArch = programacion.Detener;

                            }
                        }
                        else
                        {
                            SecuenciaAct = 0;
                            ProgramacionAct = 0;
                            ConsecProgAct = 0;
                            FechaProgAct = DateTime.Now;
                            ArchivoAct = 0;
                            ReiniciarSecuencia(false);
                        }
                    }
                    else
                    {
                        DesActivaSecuencia();
                        SecuenciaAct = 0;
                        ProgramacionAct = 0;
                        ConsecProgAct = 0;
                        FechaProgAct = DateTime.Now;
                        ArchivoAct = 0;
                        PantallaVMD = "Cambio de Secuencia";
                    }
                }
                else
                {
                    PantallaVMD = "Secuencia ya se ejecuto por completo";
                    ReiniciarSecuencia(false);
                }

            }
            else
            {
                SecuenciaAct = 0;
                ProgramacionAct = 0;
                ConsecProgAct = 0;
                FechaProgAct = DateTime.Now;
                ArchivoAct = 0;
                PantallaVMD = "No se ha cargado ninguna secuencia";
            }
        }
        return;
        }
        catch (Exception oe)
        {
            var M = oe.Message;
            return;
        }

    }

    public void FinalizaPautaVMD()
    {
        DesActivaProgramacion();
        DesActivaSecuencia();
        SecuenciaAct = 0;
        ProgramacionAct = 0;
        ConsecProgAct = 0;
        FechaProgAct = DateTime.Now;
        ArchivoAct = 0;
        PantallaVMD = "Secuencia Finalizada";
        /*
    'Detengo el video
    'UPGRADE_NOTE: controls se actualizó a Ctlcontrols. Haga clic aquí para obtener más información: 'ms - help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    fmrPrincipal.Video.Ctlcontrols.stop()
    */
        detenida = true;
    }
    public void DesActivaSecuencia()
    {
        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        objCtx.ExecuteStoreCommand("update secuencias set ejecutada=1 where idsecuencia=" + SecuenciaAct);
        PantallaVMD = "Secuencia ya se ejecuto por completo";
    }
    public void DesActivaProgramacion()
    {
        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        objCtx.ExecuteStoreCommand("update detsecuencias set ejecutada=1 where idsecuencia=" + SecuenciaAct + " and idprogramacion=" + ProgramacionAct);
    }
   
    
    public bool SecuenciaActualEsMenor(int SecuenciaRemota, DateTime FechaHoraRealRemota)
    {
        bool result = false;
        var secuencia = VMD_BD.secuencias.OrderByDescending(x => x.fechahorareal).OrderByDescending(x => x.idsecuencia).FirstOrDefault();
        if (secuencia != null)
        {
            if (SecuenciaRemota > secuencia.idsecuencia || FechaHoraRealRemota > secuencia.fechahorareal)
                result = true;
        }
        else
            result = true;
        return result;
    }
    public void ControlVolumen()
    {
        var audiovmd = VMD_BD.audiovmd.Where(x => x.HORAINICIO.TimeOfDay > DateTime.Now.TimeOfDay).OrderBy(x => x.HORAINICIO).ToList();

        TimeSpan horaActual;
        TimeSpan horaAnterior;
        TimeSpan horaPosterior = new TimeSpan(23, 59, 59);
        horaActual = DateTime.Now.TimeOfDay;
        int i;
        i = 0;
        Boolean asignaFechaAnterior;
        asignaFechaAnterior = false;
        int volTiempo;
        volTiempo = VolumenActual;
        var NextId = 0;
        if (audiovmd != null)
        {
            if (audiovmd.Count > 0)
            {

                for (i = 0; i < audiovmd.Count; i++)
                {
                    if (i == 0)
                    {
                        horaPosterior = audiovmd[i].HORAINICIO.TimeOfDay;
                    }
                    else
                    {
                        volTiempo = audiovmd[i].VOLUMEN;
                        NextId = i + 1;
                        horaAnterior = horaPosterior;
                        asignaFechaAnterior = true;
                        if (i < audiovmd.Count)
                            horaPosterior = audiovmd[NextId].HORAINICIO.TimeOfDay;
                        else horaPosterior = new TimeSpan(23, 59, 59);

                        if (asignaFechaAnterior)
                        {
                            if (horaActual > horaPosterior)
                            {
                                VolumenActual = audiovmd[NextId].VOLUMEN;
                                continue;
                            }
                        }
                        else
                        {
                            if (horaAnterior < DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay < horaPosterior)
                                VolumenActual = audiovmd[NextId].VOLUMEN;
                        }
                    }

                }
                horaAnterior = horaPosterior;
                if (asignaFechaAnterior)
                {
                    if (horaActual > horaPosterior)
                    {
                        VolumenActual = audiovmd[NextId].VOLUMEN;
                    }
                }
                else
                {
                    if (horaAnterior < DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay < horaPosterior)
                        VolumenActual = audiovmd[NextId].VOLUMEN;
                }
                /*
             If fmrPrincipal.Video.Settings.volume > 100 Then Exit Sub
		fmrPrincipal.Video.Settings.volume = VolumenActual
		
		Rs.Close()    
             */
            }
        }



    }

    public void ActualizaFechaSeleccion(string idArchivo)
    {
        var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
        objCtx.ExecuteStoreCommand(" update archivos as arc set arc.ultimavez = curdate()" + " where  arc.id in ( " + idArchivo + ")");
    }
    public void BuscarSecuencia()
    {
        
        throw new NotImplementedException("Función que sincroniza, esto se trabajará después de la línea del reproductor");
    }
    
    #endregion

    #region Funciones que se encuentran en DISPLAY
    public void MuestraError(string FuncionProced) {
        var DescriptionError = string.Empty;
        if (this.error_ == null) DescriptionError = this.error_;
        AgregaLog("** Error: " + FuncionProced, -1, 0, false, DescriptionError);
    }
    public void Salir(bool ApagarEquipo) { AgregaLog("** Error: Manda Salida VMD", -1, 0, false, "Manda Salida VMD"); }
    public void ListaMensajePredefinidos() { AgregaLog("** Error: Manda mensaje predefinido", -1, 0, false, "//"); }
    public void LimpiaMensPred() { AgregaLog("** Error: Limpia mensaje predefinido", -1, 0, false, "//"); }
    public void AgregaLog(string RutaVideo, int NumReg, int MinutosMax, bool Ejecuta, string DescripError)
    {
        AgregaLog(RutaVideo, NumReg, MinutosMax, Ejecuta, DescripError, "0", "0");
    }
    public void AgregaLog(string RutaVideo, int NumReg, int MinutosMax, bool Ejecuto, string DescripError, string UltLat, string ultLon) {
        try
        {

            if (FechaProgAct.Year > 2000)
            {
                var StringQuery = "insert into logprogramacion (id,rutavideo,idvideo,minutosmax,ejecucion,error,latitud,longitud,autobus,fechahora,secuencia,consecsecuencia,idprogramacion,fechaprogramacion) values(" + GetNext("logprogramacion", "id").ToString() + ",'" + ValidarRuta(RutaVideo) + "'," + NumReg + "," + MinutosMax + "," + (Ejecuto == true ? 1.ToString() : 0.ToString()) + ",'" + DescripError + "'," + (UltLat.Length == 0 ? 0.ToString() : UltLat.ToString()) + "," + (ultLon.Length == 0 ? 0.ToString() : ultLon.ToString()) + ",'" + parametros.Autobus + "',now()," + SecuenciaAct + "," + ConsecProgAct + "," + ProgramacionAct + ",'" + FechaProgAct.ToString("yyyy-MM-ddTHH:mm:ss") + "')";
                var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)VMD_BD).ObjectContext;
                objCtx.ExecuteStoreCommand(StringQuery);
            }
        }
        catch (Exception oe)
        { this.error_ = oe.Message; }

    }
    private int GetNext(string tabla, string Campo)
    {
        try
        {
            var Query = "Select max(" + Campo + ") as maximo from " + tabla;
            var DS = GetDataSet(Query);
            if (DS == null) throw new Exception("No se tiene información");
            if (DS.Tables.Count <= 0) throw new Exception("No se tiene información (1)");
            var Max = DS.Tables[0].Rows[0][0].ToString();
            if (Max == null)
                return 1;
            else
                return Convert.ToInt32(Max) + 1;
        }
        catch (Exception oe){ this.error_ = oe.Message; return -1; }
    }
    public string PantallaVMD { get; set; }
    public string muestraPantalla { get; set; }
    public string LabelAct;
    public string lblMensaje; // FrmSincroniza 
    #endregion

    #region "Implementaciones a realizar después"
    public void CargaUSB() { throw new NotImplementedException("NO SE VA A IMPLEMENTAR DE MOMENTO"); }
    #endregion

    #region Inicializa VMD
    public void inicializaVMD()
    {
        int ConsecProg, IdSec, IdProg, IdArch = -1;
        bool DetenerArch;
        int IdReg;
        DateTime FechaProg;
        int CualRegTmp, q;
        bool HayArchivoValido;

        var contiene = VMD_BD.secuencias.Count();
        if (contiene > 0)
        {
            var IdSecuencia = VMD_BD.secuencias.OrderByDescending(x => x.fechahorareal).FirstOrDefault().idsecuencia;
            var ejecutada = VMD_BD.secuencias.OrderByDescending(x => x.fechahorareal).FirstOrDefault().ejecutada;

            if (!Convert.ToBoolean(ejecutada))
            {
                IdSec = IdSecuencia;
                var detSecuenciasElige = VMD_BD.detsecuencias.Where(x => x.idsecuencia == IdSec && x.ejecutada == 0).FirstOrDefault();
                ConsecProg = detSecuenciasElige.consec;
                IdProg = detSecuenciasElige.idprogramacion;
                FechaProg = detSecuenciasElige.fecha;
                PantallaVMD = "Secuencia Activada #:" + IdSec.ToString();
                if (!ChecaActividad(IdSec, IdProg, ConsecProg, FechaProg))
                {
                    var ExisteProgramacion = VMD_BD.programacion.Where(x => x.Id == IdProg).Count();
                    if (ExisteProgramacion > 0)
                    {
                        var Programacion = VMD_BD.programacion.Where(x => x.Id == IdProg).FirstOrDefault();

                        if (Programacion.TipoPunto == 1)
                        {
                            if (Programacion.TipoReprodRandom)
                            {
                                IdReg = Programacion.Consec;
                                SecuenciaAct = IdSec;
                                ProgramacionAct = IdProg;
                                ConsecProgAct = ConsecProg;
                                FechaProgAct = FechaProg;
                                ArchivoAct = Programacion.Consec;
                                PosActRandom = -1;
                                DetenerArch = Programacion.Detener;

                                //Este elemento debería de elegir de la lista que se busca un elemento Random, para fines de velocidad lo estoy eligiendo
                                // como el primero, pero se tiene que cambiar
                                var Random = VMD_BD.random.Where(x => x.idruta == IdSec && x.idreg == (IdReg - 1)).ToList();
                                if (Random != null)
                                {
                                    HayArchivoValido = false;
                                    for (q = 1; q <= Random.Count; q++)
                                    {
                                        Random rnd = new Random();
                                        CualRegTmp = rnd.Next(1, Random.Count);
                                        var ElementoElegidoRandom = Random[CualRegTmp];
                                        var TiempoUltimaVez = Convert.ToInt32(VMD_BD.can_parametrosinicio.FirstOrDefault().TiempoUltimaVez);
                                        if (ValidarUltimaVez(ElementoElegidoRandom.idarchivo, TiempoUltimaVez))
                                        {
                                            IdArch = ElementoElegidoRandom.idarchivo;
                                            PosActRandom = ElementoElegidoRandom.consec;
                                            HayArchivoValido = true;
                                            break;
                                        }

                                        if (!HayArchivoValido)
                                        {
                                            CualRegTmp = rnd.Next(1, Random.Count);
                                            var ElementoElegido_ = Random[CualRegTmp];
                                            IdArch = ElementoElegido_.idarchivo;
                                            PosActRandom = ElementoElegido_.consec;
                                        }
                                    }


                                }
                            }
                            else
                            {
                                IdArch = Programacion.Archivo;
                                SecuenciaAct = IdSec;
                                ProgramacionAct = IdProg;
                                ConsecProgAct = ConsecProg;
                                FechaProgAct = FechaProg;
                                ArchivoAct = Programacion.Consec;
                                PosActRandom = -1;
                                DetenerArch = Programacion.Detener;
                            }
                            if (IdArch == -1) return;
                            var RutaNueva = VMD_BD.archivos.Where(x => x.id == IdArch).FirstOrDefault().Ruta;
                            Play(RutaNueva, IdArch, Convert.ToInt32( parametros.TiempoUltimaVez), DetenerArch, false, true, (float)0.0);
                            // AQUI ES EN DONDE DEBE DE IR EL PLAY NUEVO DEL REPRODUCTOR
                        }
                        else
                        {
                            SecuenciaAct = IdSec;
                            ProgramacionAct = IdProg;
                            ConsecProgAct = ConsecProg;
                            FechaProgAct = FechaProg;
                            ArchivoAct = Programacion.Consec;
                            PosActRandom = -1;
                            DetenerArch = Programacion.Detener;
                        }
                    }
                    else
                    {
                        SecuenciaAct = 0;
                        ProgramacionAct = 0;
                        ConsecProgAct = 0;
                        FechaProgAct = DateTime.Now;
                        ArchivoAct = 0;
                        PosActRandom = -1;
                        PantallaVMD = "No hay programación en la secuencia";
                    }
                }

            }
            else
            {
                PantallaVMD = "Secuencia ya se ejecutó completo";
            }
        }
        else
        {
            SecuenciaAct = 0;
            ProgramacionAct = 0;
            ConsecProgAct = 0;
            FechaProgAct = DateTime.Now;
            ArchivoAct = 0;
            PosActRandom = -1;
            PantallaVMD = "No se ha cargado ninguna secuencia";
        }
    }


    public bool ChecaActividad(int CualSec, int CualProg, int CualConsecProg, DateTime CualFechaProg)
    {
        var resultCheck = false;
        try
        {
            var Query = "select archivos.id, archivos.ruta, (archivosrandom.id) as idr, (archivosrandom.ruta) as rutar, actividad.posrandom, programacion.consec, programacion.detener, actividad.posicion, actividad.longitud from actividad inner join programacion on (actividad.idprogramacion=programacion.id and actividad.consec=programacion.consec) left join archivos on (programacion.archivo=archivos.id) left join random on (programacion.id=random.idruta and programacion.consec=(random.idreg+1) and actividad.posrandom=random.consec) left join archivos as archivosrandom on (random.idarchivo=archivosrandom.id) where actividad.idsecuencia=" + CualSec.ToString() + " and actividad.idprogramacion=" + CualProg.ToString();
            var Result = GetDataSet(Query);
            if (Result != null)
            {
                if (Result.Tables.Count > 0)
                {
                    if (Result.Tables[0].Rows.Count > 0)
                    {
                        var Detener = false;
                        // Filtrado para el valor Detener, ya que en ciertos puntos puede llegar vacío
                        try
                        {
                            Detener = Convert.ToBoolean(Result.Tables[0].Rows[0]["detener"].ToString());
                        }
                        catch (Exception oe)
                        { Detener = false; this.error_ = oe.Message; }

                        resultCheck = true;
                        SecuenciaAct = CualSec;
                        ProgramacionAct = CualProg;
                        ConsecProgAct = CualConsecProg;
                        FechaProgAct = CualFechaProg;
                        ArchivoAct = Convert.ToInt32(Result.Tables[0].Rows[0]["consec"].ToString());
                        double Posicion = Convert.ToDouble(Result.Tables[0].Rows[0]["Posicion"].ToString());
                        double Longitud = Convert.ToDouble(Result.Tables[0].Rows[0]["longitud"].ToString());
                        if (( Longitud - Posicion) > 2)
                        {
                            var ValidoId = -1;
                            try{ ValidoId = Convert.ToInt32(Result.Tables[0].Rows[0]["Id"].ToString()); } catch { ValidoId = -1; }
                            if (ValidoId == -1)
                            {
                                PosActRandom = Convert.ToInt32(Result.Tables[0].Rows[0]["posrandom"].ToString());
                                Play(Result.Tables[0].Rows[0]["Rutar"].ToString(),
                                   Convert.ToInt32(Result.Tables[0].Rows[0]["Idr"].ToString()),
                                    Convert.ToInt32(parametros.TiempoUltimaVez),
                                     Detener, true, false, Posicion);
                            }
                            else
                            {
                                PosActRandom = -1;
                                Play(Result.Tables[0].Rows[0]["Ruta"].ToString(),
                                   Convert.ToInt32(Result.Tables[0].Rows[0]["Id"].ToString()),
                                    Convert.ToInt32(parametros.TiempoUltimaVez),
                                     Detener, true, false, Posicion);
                            }
                        }

                    }
                }
            }
        }

        catch (Exception oe)
        { MuestraError(oe.Message); }

        return resultCheck;
    }
    public bool ValidarUltimaVez(int idArchivo, int MinutosMax)
    {
        bool result = true;
        try
        {
            var Archivo_ = VMD_BD.archivos.Where(x => x.id == idArchivo).FirstOrDefault();
            if (Archivo_ != null)
            {
                var DiferenceTime = (DateTime.Now - Archivo_.UltimaVez).TotalMinutes;
                if (DiferenceTime < MinutosMax)
                    result = false;
            }
        }
        catch (Exception oe)
        {
            this.error_ = "ValidarUltimaVez" + oe.Message;
        }
        return result;
    }
    public void ActualizarUltimaVez(int idArchivo)
    {
        try
        {
            VMD_BD.archivos.Where(x => x.id == idArchivo).FirstOrDefault().UltimaVez = DateTime.Now;
            VMD_BD.SaveChanges();

        }
        catch (Exception o)
        {
            AgregaLog("NO_RUTA", idArchivo, 0, true, "Error al Actualización UVez " + o.Message);
        }
        

    }
    #endregion

    #region Play
    public void Play(string RutaVideo, int IdArchivo, int MinutosMax, bool DetenerVideoActual, bool CargarVideoInterrumpido, bool ValidarHorario, double posicion)
    {
        var Param_CarpetaVideo = parametros.CarpetaVideos;
        Param_CarpetaVideo = Param_CarpetaVideo + 
                             (Param_CarpetaVideo.PadRight(1) == @"\" ? string.Empty : @"\").ToString();
        Param_CarpetaVideo = Param_CarpetaVideo + ValidarRuta(RutaVideo);

        if (!System.IO.File.Exists(Param_CarpetaVideo))
        {
            AgregaLog(RutaVideo, IdArchivo, MinutosMax, false, "No existe el archivo en la carpeta configurada");
            ActualizaFechaSeleccion(Convert.ToString(IdArchivo));

            ChecaSigtVideo();

            return;
        }

        //if (ValidarHorario)
        //{
        //    if (!HorarioValido())
        //    {
        //        AgregaLog(RutaVideo, IdArchivo, MinutosMax, false, "Archivo se intenta ejecutar en horario no válido");
        //        ActualizaFechaSeleccion(Convert.ToString(IdArchivo));
        //        return;
        //    }
        //}

        //if (Caduco(IdArchivo))
        //{
        //    AgregaLog(RutaVideo, IdArchivo, MinutosMax, false, "La caducidad ya venció");
        //    ActualizaFechaSeleccion(Convert.ToString(IdArchivo));
        //    return;
        //}

        //if (!Vigente(IdArchivo))
        //{
        //    AgregaLog(RutaVideo, IdArchivo, MinutosMax, false, "El inicio de vigencia aún no es válido");
        //    ActualizaFechaSeleccion(Convert.ToString(IdArchivo));
        //    return;
        //}
        //
        evRepPlay(IdArchivo, Param_CarpetaVideo, MinutosMax, DetenerVideoActual, Convert.ToBoolean(parametros.PlaySobrePlay), posicion);
        //IdPelicula = IdArchivo;
    }


    public string ValidarRuta(string ruta)
    {
        if (ruta.Contains(@"\"))
            return ruta.Substring(ruta.LastIndexOf("\\", ruta.Length - 2) + 1);
        else
            return ruta;
    }
    public Boolean HorarioValido()
    {

        var StrHoraInicial = VMD_BD.can_parametrosinicio.FirstOrDefault().HoraInicial.Split(':');
        var StrHoraFinal = VMD_BD.can_parametrosinicio.FirstOrDefault().HoraFinal.Split(':'); ;
        TimeSpan CualHoraActual = DateTime.Now.TimeOfDay;
        var HoraInicial = new TimeSpan(Convert.ToInt32(StrHoraInicial[0]), Convert.ToInt32(StrHoraInicial[1]), 0);
        var HoraFinal = new TimeSpan(Convert.ToInt32(StrHoraFinal[0]), Convert.ToInt32(StrHoraFinal[1]), 0);

        if (CualHoraActual > HoraInicial)
            if (CualHoraActual < HoraFinal)
                return true;
        return false;
    }
    private Boolean Caduco(int id)
    {
        var Caducidad = VMD_BD.archivos.Where(x => x.id == id && x.caducidad >= DateTime.Now).Count();
        if (Caducidad > 0)
            return false;
        return true;
    }
    private Boolean Vigente(int id)
    {
        var Vigente = VMD_BD.archivos.Where(x => x.id == id && x.InicioVig <= DateTime.Now).Count();
        if (Vigente > 0)
            return true;
        return false;
    }
    #endregion

    #region "Eventos"
    //Para mostrar durante la sincronización
    public delegate void dPlay(int idArchivo, string rutaVideo, int MinutosMax, bool detenerVideo, bool playSobrePlay, double posicion);
    public event dPlay evRepPlay;
    //Enviar Log de eventos de sincronización
    public delegate void dMensajeSincronizacion(string mensaje, int final);
    public event dMensajeSincronizacion evMensajeSincronizacion;
    // Mensaje Inicial

    

    #endregion

    #region "Funciones para obtener DataSet de Entity Framework (Para consultas demasiado elaboradas)"
    DataSet GetDataSet(string sql)
    {
        // creates resulting dataset
        var result = new DataSet();

        // creates a data access context (DbContext descendant)
        using (var context = new vmdEntities())
        {
            // creates a Command 

            var cmd = context.Database.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            try
            {
                // executes
                context.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                // loop through all resultsets (considering that it's possible to have more than one)
                do
                {
                    // loads the DataTable (schema will be fetch automatically)
                    var tb = new DataTable();
                    tb.Load(reader);
                    result.Tables.Add(tb);

                } while (!reader.IsClosed);
            }
            finally
            {
                // closes the connection
                context.Database.Connection.Close();
            }
        }

        // returns the DataSet
        return result;
    }
    #endregion


}


static class MyExtensions
{
    private static Random rng = new Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}