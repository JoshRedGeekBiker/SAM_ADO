using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vlc.DotNet.Core.Interops.Signatures;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using WMPLib;
using SMFE.Properties;
using System.Linq;
using System.Media;
using System.Threading;

public partial class frmReproductor : Form
{

    #region "Propiedades"
    public VideoVMD VideoEnReproduccion { get; set; }
    public VideoVMD VideoAnterior { get; set; }
    public spotPOI SpotEnReproduccion { get; set; }
    public spotPOI SpotSiguiente { get; set; }
    public List<VideoVMD> ColaVideo { get; set; }
    public List<spotPOI> colaSpots { get; set; }
    public bool detenida { get; set; } = false;
    public bool EstadoInicialStop { get; set; } = false;
    public bool primeraReproduccion { get; set; } = true;
    public string Error_ { get; set; }
    public ParametrosCintillo ParamCintillo { get; set; }

    public Size TamañoPantalla { get; set; }//Powered ByRED 20ABR2021

    /// <summary>
    /// Powered ByRED2020
    /// </summary>
    public TimeSpan TiempoReal { get; set; } = TimeSpan.FromMilliseconds(0); //Powered ByRED 20JUN2020
    public string NombreVideo { get; set; } = "No Hay Película Cargada"; //Powered ByRED 20JUN2020
    public TimeSpan TiempoDuracion { get; set; } = TimeSpan.FromMilliseconds(0); //Powered ByRED 20JUN2020
    #endregion

    #region "Variables"
    public bool PlaySobrePlay;
    public int AudioInicial = 100; //Este es el audio inicial de cualquier video
    public int AudioTemp = 0; //Powered ByRED 31MAR2021
    private static System.Timers.Timer TmrActividad;
    private static System.Timers.Timer TmrRevisarReproduccion;
    private static System.Timers.Timer TmrDuracion; //Powered ByRED 26ENE2023
    public bool spotPoi = false;

    private Thread hiloBarridoCintillo;//Powered ByRED 22ABR2021

    #endregion

    #region "Constructores"
    /// <summary>
    /// Se ocupa para Modo de Inicio Normal
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="ruta"></param>
    public frmReproductor(bool _ModoPrueba, string ruta)
    {
        CheckForIllegalCrossThreadCalls = false;

        InitializeComponent(ruta);

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        ColaVideo = new List<VideoVMD>();
        colaSpots = new List<spotPOI>();
        TmrActividad = new System.Timers.Timer(1000);
        TmrRevisarReproduccion = new System.Timers.Timer(5000);
        TmrActividad.Enabled = false;
        TmrRevisarReproduccion.Enabled = false;
        TmrActividad.Elapsed += new System.Timers.ElapsedEventHandler(tmrActividad_Tick);
        TmrRevisarReproduccion.Elapsed += new System.Timers.ElapsedEventHandler(tmrRevisarVideo_Tick);

        //Powered ByRED 26ENE2023
        TmrDuracion = new System.Timers.Timer(1000);
        TmrDuracion.Enabled = false;
        TmrDuracion.Elapsed += new System.Timers.ElapsedEventHandler(tmrDuracion_Tick);

        //hiloBarridoCintillo = new Thread(new ThreadStart(BarridoCintillo));

    }

    /// <summary>
    /// Se ocupara para Modo Configuración
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmReproductor(bool _ModoPrueba)
    {
        CheckForIllegalCrossThreadCalls = false;
        InitializeComponent();
        this.SendToBack();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }
        ColaVideo = new List<VideoVMD>();
        colaSpots = new List<spotPOI>();
        TmrActividad = new System.Timers.Timer(1000);
        TmrRevisarReproduccion = new System.Timers.Timer(5000);
        TmrActividad.Enabled = false;
        TmrRevisarReproduccion.Enabled = false;
        TmrActividad.Elapsed += new System.Timers.ElapsedEventHandler(tmrActividad_Tick);
        TmrRevisarReproduccion.Elapsed += new System.Timers.ElapsedEventHandler(tmrRevisarVideo_Tick);
    }

    #endregion

    #region "Eventos"
    public delegate void delActualizaActividad(double posicion, double longitud);
    public event delActualizaActividad evtActualizaActividad;
    public delegate void delActualizarUltimaVez(int Video);
    public event delActualizarUltimaVez evtActualizarUltimaVez;
    public delegate void delChecarSiguienteVideo();
    public event delChecarSiguienteVideo evtChecarSiguienteVideo;
    public delegate void delInfoVideo(string nombreVideo, TimeSpan Duracion);
    public event delInfoVideo evtInfoVideo;
    public delegate void delPosicionVideo(TimeSpan Posicion);
    public event delPosicionVideo evtPosicionVideo;
    public delegate void delEstadoVideoReproduccion(string Estado);
    public event delEstadoVideoReproduccion evtEstadoReproduccion;
    public delegate void delAgregarLog(string VideoURL, int idPelicula, int MinutosMax, bool Ejecuta);
    public event delAgregarLog evtAgregarLog;
    public delegate void delTerminarPoi();
    public event delTerminarPoi evtTerminarPoi;
    public delegate void delTestigoPoi(spotPOI sp);
    public event delTestigoPoi evtEnviarTestigo;

    public delegate void delReaudarPeliculaPOI();
    public event delReaudarPeliculaPOI ReanudarPeliculaPOI;
    public delegate void enviaMensajePOI(String msg);
    public event enviaMensajePOI enviaMSG;
    /// <summary>
    /// Sirve para trar los datos mutimedia
    /// Powered ByRED 30MAR2021
    /// </summary>
    /// <returns></returns>
    public delegate List<string> TraerMultimediaPI();
    public event TraerMultimediaPI MultimediaPI;

    /// <summary>
    /// Sirve para trar los datos de la pantalla
    /// Powered ByRED 30MAR2021
    /// </summary>
    /// <returns></returns>
    public delegate Size TamanoPant();
    public event TamanoPant Pant;

    /// <summary>
    /// Se encarga de avisar a FE que se está mostrando el cintillo
    /// </summary>
    /// <param name="Activo"></param>
    public delegate void MostrandoCintillo(bool Activo);
    public event MostrandoCintillo Cintillo;

    #endregion

    #region "Métodos"

    /// <summary>
    /// Se encarga de regresar si el reproductor está
    /// reproduciendo una pelicula o no
    /// </summary>
    /// <returns></returns>
    public bool EnReproduccion()
    {
        return vlcControl1.State == MediaStates.Playing ? true : false;
    }

    public void Func_VMDReiniciar()
    {

        if (this.VideoEnReproduccion == null) return;

        var archivo = new System.IO.FileInfo(this.VideoEnReproduccion.rutaVideo);
        this.vlcControl1.SetMedia(archivo);
        this.vlcControl1.Position = (float)0.0;
        this.vlcControl1.BringToFront();
        this.vlcControl1.Visible = true;
        this.vlcControl1.Play();

        //Avisa al Front para cambiar el ícono
        evtEstadoReproduccion("play");

        TmrActividad.Start();//Powered ByRED 20JUN2020
        TmrDuracion.Start();//Powered ByRED 24ENE2023

    }

    public void Func_VMDPlay(VideoVMD VideoEntrante, bool DetenerVideoActual)
    {
        if (PlaySobrePlay)
        {
            if (!DetenerVideoActual)
            { ColaVideo.Add(VideoEntrante); return; }
        }
        else
        {
            if (vlcControl1.State == MediaStates.Playing)
            { ColaVideo.Add(VideoEntrante); return; }
        }
        this.VideoEnReproduccion = VideoEntrante;
        //En este punto ya deja reproducir
        Func_ReproductorPlay();

    }
    public void Func_SpotPlay(spotPOI VideoEntrante)
    {

        //if (vlcControl1.State == MediaStates.Playing)
        //{
        //    colaSpots.Add(VideoEntrante);
        //    return;
        //}

        colaSpots.Add(VideoEntrante);
        if (vlcControl1.State == MediaStates.Stopped || vlcControl1.State == MediaStates.NothingSpecial)
        {
            if (primeraReproduccion)
            {
                EstadoInicialStop = vlcControl1.State == MediaStates.NothingSpecial ? true : false;
                primeraReproduccion = false;
            }
            if (vlcControl1.State != MediaStates.Opening && vlcControl1.State == MediaStates.Stopped)
            {

                Func_ReproduceColaSpots();
            }
        }
        else {
            enviaMSG("POI");
        }

        //if (primeraRepoduccion) {
        //    EstadoInicialStop = vlcControl1.State == MediaStates.NothingSpecial ? true : false;
        //    primeraRepoduccion = false;
        //}
        //this.SpotEnReproduccion = VideoEntrante;
        //En este punto ya deja reproducir
        //switch (tipo) {
        //    case 0:
        //        ReproducirMP3POI();
        //        break;
        //    case 1:
        //        Func_ReproSpots(tipo);
        //        break;
        //}
        //Func_ReproSpots();
    }
    /// <summary>
    /// Metodo que reproduce los Spots
    /// </summary>
    /// <param name="VideoEntrante"></param>
    /// <param name="DetenerVideoActual"></param>
    public void Func_ReproSpots()
    {

        var archivo = new System.IO.FileInfo(this.SpotEnReproduccion.rutaVideo);
        
        this.vlcControl1.SetMedia(archivo);
        this.vlcControl1.BringToFront();
        if (SpotEnReproduccion.tipoSpot.ToUpper().Contains("VIDEO"))
        {
            this.vlcControl1.Visible = true;
        }
        else
        {
            this.vlcControl1.Visible = false;
        }
        //this.vlcControl1.Play();
        evtEstadoReproduccion("play");//Avisa al Front para cambiar el ícono
        //var posicion = (float)this.VideoEnReproduccion.posicion; se comento por que truena aqui
        //this.vlcControl1.Position = posicion; //Debemos de poner la posición más adelante
        //this.vlcControl1.Audio.Volume = AudioInicial;

        //var hilo = new System.Threading.Thread(Func_EstablecerPosicionInicial);
        //hilo.IsBackground = true;
        //hilo.Start();//En caso de que no cambie la posición pido la ejecución desde este hilo

        this.NombreVideo = this.vlcControl1.GetCurrentMedia().Title;

        TmrActividad.Start();
        TmrDuracion.Start();//Powered ByRED 24ENE2023

        TmrRevisarReproduccion.Start(); //Inicio del ciclo de recuperaciones
       
        this.vlcControl1.Play(); //Powered ByRED 20JUN2020
        this.vlcControl1.Position = 0; //Powered ByRED 07ABR2021
        spotPoi = true;
        TmrActividad.Start();
        TmrDuracion.Start();//Powered ByRED 24ENE2023


    }
    public void Func_ReproductorPlay()
    {
        var archivo = new System.IO.FileInfo(this.VideoEnReproduccion.rutaVideo);
        this.vlcControl1.SetMedia(archivo);
        this.vlcControl1.BringToFront();
        this.vlcControl1.Visible = true;
        //this.vlcControl1.Play();
        evtEstadoReproduccion("play");//Avisa al Front para cambiar el ícono
        var posicion = (float)this.VideoEnReproduccion.posicion;
        //this.vlcControl1.Position = posicion; //Debemos de poner la posición más adelante
        this.vlcControl1.Audio.Volume = AudioInicial;

        //var hilo = new System.Threading.Thread(Func_EstablecerPosicionInicial);
        //hilo.IsBackground = true;
        //hilo.Start();//En caso de que no cambie la posición pido la ejecución desde este hilo

        this.NombreVideo = this.vlcControl1.GetCurrentMedia().Title;

        TmrActividad.Start();
        TmrDuracion.Start();//Powered ByRED 24ENE2023

        TmrRevisarReproduccion.Start(); //Inicio del ciclo de recuperaciones


        this.vlcControl1.Play(); //Powered ByRED 20JUN2020

        this.vlcControl1.Position = posicion; //Powered ByRED 07ABR2021
    }

    /// <summary>
    /// *Tarjeta de circulación vencida* Se quita ejecución desde hilo
    /// </summary>
    public void Func_EstablecerPosicionInicial()
    {
        System.Threading.Thread.Sleep(500);
        var SegundosTotales = this.vlcControl1.GetCurrentMedia().Duration.TotalSeconds;
        this.TiempoDuracion = TimeSpan.FromSeconds(SegundosTotales);
        this.NombreVideo = this.vlcControl1.GetCurrentMedia().Title;
        //var PosicionF = (float)this.VideoEnReproduccion.posicion;
        //this.vlcControl1.Position = PosicionF;
        //this.vlcControl1.Audio.Volume = AudioInicial;


        //evtInfoVideo(NombreVideo, TimeSpanMinutos);
    }

    public void Func_PararTmrActividad()
    {
        TmrActividad.Stop();
        TmrDuracion.Stop();//Powered ByRED 24ENE2023
        this.vlcControl1.Stop();
        this.vlcControl1.Visible = false;
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1)); //Espera un segundo para garantizar el término del Timer
        evtEstadoReproduccion("stop");//Avisa al Front para cambiar el ícono
    }

    private void Func_RevisaVideo()
    {
        switch (this.vlcControl1.State)
        {
            case MediaStates.Stopped: /*En códigos pasados hacen algo con este estado, aunque aún no lo encuentro*/
                evtEstadoReproduccion("stop"); //Avisa al Front para cambiar el ícono
                break;
            case MediaStates.Ended:
                evtEstadoReproduccion("stop"); //Avisa al Front para cambiar el ícono
                if (!spotPoi)
                {
                    TmrActividad.Stop();
                    TmrDuracion.Stop();//Powered ByRED 24ENE2023
                    Func_ReproduceColaVideo();
                }
                else

               {
                    evtEnviarTestigo(SpotEnReproduccion);
                    if (colaSpots.Count() == 0)
                    {
                        spotPoi = false;
                        if (!EstadoInicialStop)
                        {
                            ReanudarPeliculaPOI();

                        }
                        else {
                            Func_DetenerPeliculaPOI();
                        }
                    }
                    else
                    {
                        Func_ReproduceColaSpots();
                    }

                }
                break;
            case MediaStates.Playing:
                if (!spotPoi)
                {
                    if (VideoAnterior == null) VideoAnterior = VideoEnReproduccion;
                    if (VideoAnterior.idArchivo != VideoEnReproduccion.idArchivo)
                    {
                        evtActualizarUltimaVez(VideoEnReproduccion.idArchivo);
                        evtAgregarLog(VideoEnReproduccion.rutaVideo, VideoEnReproduccion.idArchivo, VideoEnReproduccion.MinutosMax, true);
                        VideoAnterior = VideoEnReproduccion;
                    }
                }
                else
                {

                }

                TmrRevisarReproduccion.Start();
                break;
        }

    }

    private void Func_ReproduceColaVideo()
    {
        if (ColaVideo.Count > 0)
        {
            this.VideoEnReproduccion = ColaVideo[0];
            ColaVideo.RemoveAt(0);
            Func_ReproductorPlay();
            return;
        }
        else
        {
            if (this.vlcControl1.State == MediaStates.Ended)
            {
                evtChecarSiguienteVideo();
            }
        }

    }
    /// <summary>
    /// Se encarga de asignar el siguiente spot para la reproduccion y mandar el testigo
    /// Powered byToto2023♫♫
    /// </summary>
    private void Func_ReproduceColaSpots()
    {
        if(this.vlcControl1.State != MediaStates.Playing) {
            if (colaSpots.Count > 0)
            {
                //Aqui vamos a mandar el testigo del spot reproducido
                this.SpotEnReproduccion = colaSpots[0];
                colaSpots.RemoveAt(0);
                Func_ReproSpots();
                return;
            }
        }
        
    }

    /// <summary>
    /// Se encarga de subir el Volumen
    /// Powered ByRED2020
    /// </summary>
    /// <returns></returns>
    public int Func_SubirVolumen()
    {
        //  Func_MinimizarReproductor(this.vlcControl1);  // línea depurativa
        try
        {
            var VolumenActual = this.vlcControl1.Audio.Volume;
            if (VolumenActual > 100)
                return -1;
            else if (VolumenActual == 100) return 100;
            VolumenActual += 5;
            this.vlcControl1.Audio.Volume = VolumenActual;
            AudioInicial = VolumenActual;
            return VolumenActual;
        }
        catch (Exception oe)
        {
            this.Error_ = oe.Message;
            return -1;
        }
    }

    /// <summary>
    /// Se encarga de bajar el volumen
    /// Powered ByRED2020
    /// </summary>
    /// <returns></returns>
    public int Func_BajarVolumen()
    {
        try
        {
            var VolumenActual = this.vlcControl1.Audio.Volume;
            if (VolumenActual < 0) return -1;
            else if (VolumenActual == 0) return 0;
            VolumenActual -= 5;
            this.vlcControl1.Audio.Volume = VolumenActual;
            AudioInicial = VolumenActual;
            return VolumenActual;
        }
        catch (Exception oe)
        {
            this.Error_ = oe.Message;
            return -1;
        }
    }

    /// <summary>
    /// Se encarga de detener la pelicula en reproduccion
    /// Powered ByRED2020
    /// </summary>
    public void Func_DetenerPelicula()
    {
        try
        {
            TmrRevisarReproduccion.Stop();
            Func_PararTmrActividad();
            return;
        }
        catch (Exception oe)
        {
            this.Error_ = oe.Message;
        }
    }

    /// <summary>
    /// Se encarga de detener la pelicula en reproduccion
    /// Powered ByRED2020
    /// </summary>
    public void Func_DetenerPeliculaPOI()
    {
        try
        {
            if (vlcControl1.State == MediaStates.Playing)
            {

                TmrRevisarReproduccion.Stop();
                TmrActividad.Stop();
                //TmrDuracion.Stop();//Powered ByRED 24ENE2023

                this.vlcControl1.Stop();
                this.vlcControl1.Visible = false;
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1)); //Espera un segundo para garantizar el término del Timer
                evtEstadoReproduccion("stop");//Avisa al Front para cambiar el ícono
            }
            return;
        }
        catch (Exception oe)
        {
            this.Error_ = oe.Message;
        }
    }

    /// <summary>
    /// Se encarga de recibir el texto para ser mostrado en el cintillo
    /// </summary>
    /// <param name="TextoMostrar"></param>
    public void Func_AddCintillo(string TextoMostrar)
    {
        Func_AddCintillo(this,
                         this.vlcControl1,
                         TextoMostrar,
                         ParamCintillo.PosicionCintillo,
                         ColorTranslator.FromHtml(ParamCintillo.ColorDFondo),
                         ParamCintillo.VelocidadDCintillo,
                         ParamCintillo.TamanioDFuente,
                         ParamCintillo.ColorDFuente,
                         ParamCintillo.VueltaCintillo);
    }

    /// <summary>
    /// Función Máster para completar los parametros y mostrar el cintillo
    /// Powered ByRED 14ABR2021
    /// </summary>
    /// <param name="ControlPadre"></param>
    /// <param name="ControlReproductor"></param>
    /// <param name="TextoMostrar"></param>
    /// <param name="PosicionCintillo"></param>
    /// <param name="ColorDFondo"></param>
    /// <param name="VelocidadDCintillo"></param>
    /// <param name="FuenteDTexto"></param>
    /// <param name="ColorDTexto"></param>
    /// <param name="vueltasCintillo"></param>
    private void Func_AddCintillo(Control ControlPadre,
                                  Vlc.DotNet.Forms.VlcControl ControlReproductor,
                                  String TextoMostrar,
                                  DockStyle PosicionCintillo,
                                  Color ColorDFondo,
                                  int VelocidadDCintillo,
                                  Font FuenteDTexto,
                                  string ColorDTexto,
                                  int vueltasCintillo)
    {

        Cintillo(true);

        if (PosicionCintillo != DockStyle.Top)
            if (PosicionCintillo != DockStyle.Bottom)
                throw new Exception("La posición solo puede ser TOP o BOTTOM");

        var MaxHeight = this.Height;
        ControlReproductor.Dock = PosicionCintillo == DockStyle.Top ? DockStyle.Bottom : DockStyle.Top;

        //ControlReproductor.Height = Convert.ToInt32(MaxHeight * 0.87);
        //var AlturaDecidida = Convert.ToInt32(MaxHeight * 0.13);


        ControlReproductor.Height = Convert.ToInt32(MaxHeight * 0.87);
        var AlturaDecidida = Convert.ToInt32(MaxHeight * 0.13);
        //var AlturaDecidida = Convert.ToInt32(MaxHeight * 0.13);
        //var Panel = new System.Windows.Forms.Panel();
        //Panel.BackColor = ColorDFondo;
        //Panel.Dock = PosicionCintillo;
        //Panel.Location = new System.Drawing.Point(0, AlturaDecidida);
        //Panel.Name = "Panel_Auto";
        //Panel.Size = new System.Drawing.Size(ControlPadre.Width, AlturaDecidida);
        //Panel.TabIndex = 0;
        //ControlPadre.Controls.Add(Panel);

        this.pnl_Cintillo.BackColor = ColorDFondo;
        this.pnl_Cintillo.Dock = PosicionCintillo;
        this.pnl_Cintillo.Location = new System.Drawing.Point(0, AlturaDecidida);
        this.pnl_Cintillo.Size = new System.Drawing.Size(ControlPadre.Width, AlturaDecidida);
        this.pnl_Cintillo.TabIndex = 0;

        //var Label = new System.Windows.Forms.Label();
        //Label.BackColor = ColorDFondo;
        //Label.Dock = DockStyle.None;
        //Label.AutoSize = true;
        //Label.RightToLeft = System.Windows.Forms.RightToLeft.No;
        //Label.Location = new System.Drawing.Point(0, 0);
        //Label.Name = "Label_Auto";
        //Label.TabIndex = 0;
        //Label.ForeColor = ColorTranslator.FromHtml(ColorDTexto);

        this.lbl_Cintillo.BackColor = ColorDFondo;
        this.lbl_Cintillo.Dock = DockStyle.None;
        this.lbl_Cintillo.AutoSize = true;
        this.lbl_Cintillo.RightToLeft = RightToLeft.No;
        this.lbl_Cintillo.Location = new System.Drawing.Point(0, 0);
        this.lbl_Cintillo.TabIndex = 0;
        this.lbl_Cintillo.ForeColor = ColorTranslator.FromHtml(ColorDTexto);

        //Panel.Controls.Add(Label);
        //Label.Text = TextoMostrar;
        //Label.Font = FuenteDTexto;
        //Panel.BringToFront();
        //ControlReproductor.BringToFront();

        this.lbl_Cintillo.Text = TextoMostrar;
        this.lbl_Cintillo.Font = FuenteDTexto;
        this.lbl_Cintillo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.pnl_Cintillo.BringToFront();

        this.Refresh();


        THRD_Func_CintilloMensaje(this.lbl_Cintillo,
                                         this.pnl_Cintillo.Width,
                                         PosicionCintillo == DockStyle.Top ? 9 : 40,
                                         VelocidadDCintillo,
                                         ControlPadre,
                                         ControlReproductor,
                                         vueltasCintillo);
    }

    /// <summary>
    /// Tarea que se ejcuta en 2do plano para el barrido del texto del cintillo
    /// Powered ByRED 14ABR2021
    /// </summary>
    /// <param name="lbl"></param>
    /// <param name="MaxX"></param>
    /// <param name="MaxY"></param>
    /// <param name="VelocidadCintillo"></param>
    /// <param name="ControlPadre"></param>
    /// <param name="ControlReproductor"></param>
    /// <param name="vueltasCintillo"></param>
    /// <returns></returns>
    private Task<bool> THRD_Func_CintilloMensaje(Label lbl,
                              int MaxX,
                              int MaxY,
                              int VelocidadCintillo,
                              Control ControlPadre,
                              Vlc.DotNet.Forms.VlcControl ControlReproductor,
                              int vueltasCintillo)
    {

        return Task<bool>.Run(
            async () =>
            {
                var x = MaxX;
                var FinCintillo = lbl.Width * -1;

                //Realizamos el barrido
                for (int i = 1; i <= vueltasCintillo; i++)
                {
                    while (x >= FinCintillo)
                    {
                        lbl.Location = new System.Drawing.Point(x, MaxY);
                        x = x - 1;
                        await Task.Delay(TimeSpan.FromTicks(VelocidadCintillo));
                    }
                    x = MaxX;
                }

                this.pnl_Cintillo.Size = new System.Drawing.Size(1, 1);
                this.lbl_Cintillo.Size = new System.Drawing.Size(1, 1);
                this.pnl_Cintillo.Location = new System.Drawing.Point(0, 0);
                this.lbl_Cintillo.Location = new System.Drawing.Point(0, 0);
                ControlReproductor.Dock = DockStyle.None;
                ControlReproductor.Size = this.TamañoPantalla;
                ControlReproductor.Location = new Point(0, 0);

                //Avisamos al front que dejamos de mostrar el cintillO
                Cintillo(false);
                return false;
            }
            );
    }


    private void BarridoCintillo(int MaxY, int VelocidadCintillo, Control ControlPadre, Vlc.DotNet.Forms.VlcControl ControlReproductor, int vueltasCintillo)
    {
        var x = this.pnl_Cintillo.Width;
        var FinCintillo = this.lbl_Cintillo.Width * -1;

        //Realizamos el barrido
        for (int i = 1; i <= vueltasCintillo; i++)
        {
            while (x >= FinCintillo)
            {
                this.lbl_Cintillo.Location = new System.Drawing.Point(x, MaxY);
                x = x - 1;
                Thread.Sleep(TimeSpan.FromTicks(VelocidadCintillo));
            }
            x = this.pnl_Cintillo.Width;
        }

        this.pnl_Cintillo.Size = new System.Drawing.Size(1, 1);
        this.lbl_Cintillo.Size = new System.Drawing.Size(1, 1);
        this.pnl_Cintillo.Location = new System.Drawing.Point(0, 0);
        this.lbl_Cintillo.Location = new System.Drawing.Point(0, 0);
        ControlReproductor.Dock = DockStyle.None;
        ControlReproductor.Size = this.TamañoPantalla;
        ControlReproductor.Location = new Point(0, 0);

        //Avisamos al front que dejamos de mostrar el cintillO
        Cintillo(false);
    }
    /// <summary>
    /// Se encarga de evitar que se cierre la ventana
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmReproductor_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.WindowsShutDown)
        {
            //Mandamos el comando para cerrar SAM
            //ApagarPorSistema();
        }
        else
        {
            //Cancelamos el cierre
            e.Cancel = true;
        }
    }


    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmReproductor_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Se encarga de reproducir un Mp3
    /// Powered Bytoto ENERO2023
    /// </summary>
    /// <param name="ruta"></param>
    public void ReproducirMP3POI()
    {
        try
        {
            var archivo = new System.IO.FileInfo(this.SpotEnReproduccion.rutaVideo);
            this.Mp3.SetMedia(archivo);
            this.Mp3.Visible = false;
            this.Mp3.Audio.Volume = AudioInicial;
            this.Mp3.Play();
            spotPoi = true;
            TmrActividad.Start();
            TmrDuracion.Start();//Powered ByRED 24ENE2023

            TmrRevisarReproduccion.Start(); //Inicio del ciclo de recuperaciones
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }


    /// <summary>
    /// Se encarga de reproducir un Mp3
    /// Powered ByRED 30MAR2021
    /// </summary>
    /// <param name="ruta"></param>
    public void ReproducirMP3(string ruta)
    {
        try
        {
            var archivo = new System.IO.FileInfo(ruta);
            this.Mp3.SetMedia(archivo);
            this.Mp3.Visible = false;
            this.Mp3.Audio.Volume = AudioTemp;
            this.Mp3.Play();
        }
        catch (Exception ex)
        {
            var error = ex.ToString();
        }
    }

    #endregion

    #region "Timers"

    private void tmrActividad_Tick(object source, ElapsedEventArgs e)
    {
        TmrActividad.Stop();


        // Se mueve la logica a tmr_duracion
        //try
        //{
        //    var Span = TimeSpan.FromMilliseconds(this.vlcControl1.Time);


        //    TiempoReal = Span;
        //    TiempoDuracion = TimeSpan.FromSeconds(this.vlcControl1.GetCurrentMedia().Duration.TotalSeconds);
        //}
        //catch (Exception oe)
        //{
        //    this.Error_ = oe.Message;
        //}

        try
        {
            if (vlcControl1.State == MediaStates.Playing)
            {
                var posicion = (double)this.vlcControl1.Position;
                var Longitud = (double)this.vlcControl1.Length;
                if (spotPoi) return;
                evtActualizaActividad(posicion, Longitud);
            }
            //else {
            //    if (spotPoi) {
            //        //Crear evento para avisar a Front engine  para reanudar el video
            //        //evtTerminarPoi();
            //    }
            //}
        }
        catch (Exception o)
        {
            this.Error_ = o.Message;
        }
        TmrActividad.Start();
    }

    /// <summary>
    /// Se encargará de actualizar la duración y posición de la película en reproducción
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void tmrDuracion_Tick(object source, ElapsedEventArgs e)
    {
        TmrDuracion.Stop();
        try
        {
            var Span = TimeSpan.FromMilliseconds(this.vlcControl1.Time);


            TiempoReal = Span;
            TiempoDuracion = TimeSpan.FromSeconds(this.vlcControl1.GetCurrentMedia().Duration.TotalSeconds);
        }
        catch (Exception oe)
        {
            this.Error_ = oe.Message;
        }

        TmrDuracion.Start();
    }

    private void tmrRevisarVideo_Tick(object source, ElapsedEventArgs e)
    {
        TmrRevisarReproduccion.Stop();
        Func_RevisaVideo();
    }


    #endregion

    #region  "Clases"
    public class VideoVMD
    {
        public int idArchivo { get; set; }
        public int MinutosMax { get; set; }
        public string rutaVideo { get; set; }
        public double posicion { get; set; }
    }

    public class ParametrosCintillo
    {
        public DockStyle PosicionCintillo { get; set; }
        public string ColorDFondo { get; set; }
        public int VelocidadDCintillo { get; set; }
        public Font TamanioDFuente { get; set; }
        public string ColorDFuente { get; set; }
        public int VueltaCintillo { get; set; }
    }

    #endregion

    #region "funciones para Cintillo"


    //Cintillo inicial y recibe los parámetros **Tarjeta Vencidad, fuera de circulación***
    public void Func_AddCintillo(string TextoMostrar,
                                  string PosicionCintillo,
                                  string ColorDFondo,
                                  int VelocidadDCintillo,
                                  int TamanioDFuente,
                                  string ColorDFuente,
                                  int VueltasCintillo)
    {
        var Fuente = new System.Drawing.Font("Arial",
                                            TamanioDFuente,
                                            FontStyle.Regular,
                                            GraphicsUnit.Point,
                                            ((byte)(0)));

        ParamCintillo = new ParametrosCintillo();
        ParamCintillo.PosicionCintillo = PosicionCintillo.ToUpper().Equals("T") ? DockStyle.Top : DockStyle.Bottom;
        ParamCintillo.ColorDFondo = ColorDFondo;
        ParamCintillo.VelocidadDCintillo = VelocidadDCintillo;
        ParamCintillo.TamanioDFuente = Fuente;
        ParamCintillo.ColorDFuente = ColorDFuente;
        ParamCintillo.VueltaCintillo = VueltasCintillo;

        Func_AddCintillo(this,
                         this.vlcControl1,
                         TextoMostrar,
                         ParamCintillo.PosicionCintillo,
                         ColorTranslator.FromHtml(ParamCintillo.ColorDFondo),
                         ParamCintillo.VelocidadDCintillo,
                         ParamCintillo.TamanioDFuente,
                         ParamCintillo.ColorDFuente,
                         ParamCintillo.VueltaCintillo);
    }





    #endregion
}

