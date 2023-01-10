using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFE.Properties;


public partial class frmConfiguracion : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmConfiguracion()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmConfiguracion(bool _ModoPrueba, bool Nocturno)
    {
        InitializeComponent();
        
        lblFecha.Text = DateTime.Now.ToString();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
        }

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
    #endregion

    #region "Propiedades"

    private bool ModoNocturno { get; set; } = false;
    public bool Cargado { get; set; } = false;
    #endregion

    #region "Variables"
    private List<Label> Botones;
    private List<Label> Opciones;
    private int step = 1;
    private int TarjetaCAN = 0;
    private bool segundaPaginaSistemas = false;
    private bool segundaPaginaAutobus = false;
    private bool seleccionSistema = false;
    private Sistemas Catsistemas;
    #endregion

    #region "Eventos"
    //Para mandar un Pop Up de error
    public delegate void MandaError(string mensaje);
    public event MandaError Error;

    //Para finalizar la configuración y enviar los datos recabados
    public delegate void MandaCerrarConfig(int TarjetaCAN, Sistemas Catsistemas);
    public event MandaCerrarConfig CerrarConfig;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso 0:apagar, 1:salir
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    //Muestra la pantalla anterior
    public delegate void MostrarPantBus(int NumeroPantalla);
    public event MostrarPantBus PantallaAnterior;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

    /// <summary>
    /// Se encarga mandar la instrucción de apagado del sistema SAM
    /// por un apagado de equipo
    /// Powered ByRED 10DIC2020
    /// </summary>
    public delegate void ApagarEquipoPorSistema();
    public event ApagarEquipoPorSistema ApagarPorSistema;
    #endregion

    #region "Metodos"

    /// <summary>
    /// Srive para activar/desactivar el modo
    /// nocturno en la vista
    /// </summary>
    /// <param name="Activar"></param>
    public void ActivarModonocturno(bool Activar)
    {
        if (Activar)
        {
            if (!ModoNocturno)
            {
                //Backgrounds
                this.BackgroundImage = Resources.fondoCVPoblacionNoc;

                //Textos
                this.lblTitulo.ForeColor = Color.Gray;
                this.lblFecha.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESARNoc;



                //Selecciones
                this._lblOpcion_0.Image = Resources.BotonNUMEROSNoc;
                this._lblOpcion_1.Image = Resources.BotonNUMEROSNoc;
                this._lblOpcion_2.Image = Resources.BotonNUMEROSNoc;
                this._lblOpcion_3.Image = Resources.BotonNUMEROSNoc;


                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {
                //Backgrounds
                this.BackgroundImage = Resources.fondoCVpoblacion;

                //Textos
                this.lblTitulo.ForeColor = Color.White;
                this.lblFecha.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnEnter.BackgroundImage = Resources.BotonAdelante;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESAR;

                //Selecciones
                this._lblOpcion_0.Image = Resources.BotonNUMEROS;
                this._lblOpcion_1.Image = Resources.BotonNUMEROS;
                this._lblOpcion_2.Image = Resources.BotonNUMEROS;
                this._lblOpcion_3.Image = Resources.BotonNUMEROS;

                ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// Se encarga de reiniciar las variables
    /// </summary>
    public void ReiniciarSys()
    {
        Catsistemas.InicializarVariables();
    }


    private void frmConfiguracion_Load(object sender, EventArgs e)
    {

        this.Location = Ubicacion();

        Botones = new List<Label>();
        Botones.Add(_lblOpcion_0);
        Botones.Add(_lblOpcion_1);
        Botones.Add(_lblOpcion_2);
        Botones.Add(_lblOpcion_3);

        BlanquearBotones();

        Opciones = new List<Label>();
        Opciones.Add(_lblDescripcion_0);
        Opciones.Add(_lblDescripcion_1);
        Opciones.Add(_lblDescripcion_2);
        Opciones.Add(_lblDescripcion_3);

        BlanquearOpciones();

        //Creamos el control de sistemas para activar
        Catsistemas = new Sistemas();
        Catsistemas.InicializarVariables();

        //Configuramos para seleccion el tipo de autobus
        SeleccionAutobus();
    }

    /// <summary>
    /// Configura el form para seleccionar
    /// el tipo de tarjeta CAN de acuerdo al 
    /// modelo de bus pagina 1
    /// </summary>
    private void SeleccionAutobus(int numCAN = 0)
    {
        step = 1;
        btnArriba.Visible = false;
        btnAbajo.Visible = true;

        segundaPaginaAutobus = false;

        lblInidicaciones.Text = "Seleccione el tipo de autobús";

        _lblDescripcion_0.Text = "Mercedes Benz";
        _lblDescripcion_1.Text = "Volvo";
        _lblDescripcion_2.Text = "Scania i6";
        _lblDescripcion_3.Text = "DIDCOM VERSION 1";

        //Mostramos los textos
        _lblDescripcion_0.Visible = true;
        _lblDescripcion_1.Visible = true;   
        _lblDescripcion_2.Visible = true;
        _lblDescripcion_3.Visible = true;

       
        if (numCAN > 0 & numCAN < 5)
        {
            SeleccionAnteriorCAN(numCAN);
        }
        else
        {
            BlanquearBotones();
        }

        TarjetaCAN = numCAN;
    }

    /// <summary>
    /// Configura el form para seleccionar el tipo de tarjeta CAN
    /// de acuerdo al modelo de bus Página 2
    /// </summary>
    private void SeleccionAutobus2(int numCAN = 0)
    {
        step = 1;
        btnArriba.Visible = true;
        btnAbajo.Visible = false;

        segundaPaginaAutobus = true;

        lblInidicaciones.Text = "Seleccione el tipo de autobús";

        _lblDescripcion_0.Text = "DIDCOM VERSION 2";

        //Mostramos los textos
        _lblDescripcion_0.Visible = true;
        _lblDescripcion_1.Visible = false;
        _lblDescripcion_2.Visible = false;
        _lblDescripcion_3.Visible = false;


        if (numCAN > 4)
        {
            SeleccionAnteriorCAN(numCAN);
        }
        else
        {
            BlanquearBotones();
            
            //Se ocultan por que sólo se tiene una opción
            _lblOpcion_1.Visible = false;
            _lblOpcion_2.Visible = false;
            _lblOpcion_3.Visible = false;
        }

        TarjetaCAN = numCAN;
    }

    /// <summary>
    /// Inicializa la primer pagina para
    /// la seleccion de los sistemas
    /// </summary>
    private void PrimerPaginaSistemas()
    {
        step = 2;

        BlanquearBotones();

        BlanquearOpciones();

        btnAbajo.Visible = true;
        btnArriba.Visible = false;

        segundaPaginaSistemas = false;

        lblInidicaciones.Text = "Seleccione los sistemas que desea activar";

        _lblDescripcion_0.Text = "CAN";
        _lblDescripcion_1.Text = "VMD";
        _lblDescripcion_2.Text = "CONDUSAT";
        _lblDescripcion_3.Text = "SIA";

        CargarSeleccionSistemas();

    }

    /// <summary>
    /// Inicializa la segunda pagina para la
    /// seleccion de los sistemas
    /// </summary>
    private void SegundaPaginaSistemas()
    {
        btnArriba.Visible = true;
        btnAbajo.Visible = false;

        segundaPaginaSistemas = true;

        lblInidicaciones.Text = "Seleccione los sistemas que desea activar";

        BlanquearBotones();

        BlanquearOpciones();


        _lblDescripcion_0.Text = "Antivirus";
        _lblDescripcion_1.Text = "PLAT";
        _lblDescripcion_2.Text = "SIA DLL";
        _lblDescripcion_3.Text = "SIIAB TELEMATICS";
        //_lblOpcion_3.Visible = false;

        CargarSeleccionSistemas();
    }

    /// <summary>
    /// Se encarga de Borrar las selecciones de los botones
    /// </summary>
    private void BlanquearBotones()
    {
        foreach (Label l in Botones)
        {
            l.Visible = true;
            l.Text = "";
        }
    }

    /// <summary>
    /// Se encarga de borrar el texto de las opciones
    /// </summary>
    private void BlanquearOpciones()
    {
        foreach (Label l in Opciones)
        {
            l.Visible = true;
            l.Text = "";
        }
    }

    /// <summary>
    /// se encarga de recuperar la selección anterior de la tarjeta
    /// de CAN de acuerdo 
    /// </summary>
    /// <param name="tipoCAN"></param>
    private void SeleccionAnteriorCAN(int tipoCAN)
    {
        switch (tipoCAN)
        {
            case 1:
                _lblOpcion_0_Click(this._lblOpcion_0, null);
                break;

            case 2:
                _lblOpcion_1_Click(this._lblOpcion_1, null);
                break;

            case 3:
                _lblOpcion_2_Click(this._lblOpcion_2, null);
                break;

            case 4:
                _lblOpcion_3_Click(this._lblOpcion_3, null);
                break;

            case 5:
                if (segundaPaginaAutobus)
                {
                    _lblOpcion_0_Click(this._lblOpcion_0, null);
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// Se encarga de cargar las selecciones
    /// de los sistemas
    /// </summary>
    private void CargarSeleccionSistemas()
    {
        seleccionSistema = false;

        if (!segundaPaginaSistemas)
        {
            if (Catsistemas.CAN)
            {
                _lblOpcion_0.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_0.Text = "";
            }

            if (Catsistemas.VMD)
            {
                _lblOpcion_1.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_1.Text = "";
            }

            if (Catsistemas.CONDUSAT)
            {
                _lblOpcion_2.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_2.Text = "";
            }

            if (Catsistemas.SIA)
            {
                _lblOpcion_3.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_3.Text = "";
            }
        }
        else //Para las selecciones de la segunda página
        {
            if (Catsistemas.Antivirus)
            {
                _lblOpcion_0.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_0.Text = "";
            }

            if (Catsistemas.PLAT)
            {
                _lblOpcion_1.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_1.Text = "";
            }

            if (Catsistemas.SIADLL)
            {
                _lblOpcion_2.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_2.Text = "";
            }

            if (Catsistemas.TELEMETRIA)
            {
                _lblOpcion_3.Text = "<";
                seleccionSistema = true;
            }
            else
            {
                _lblOpcion_3.Text = "";
            }
        }  
    }

    /// <summary>
    /// Se encarga de detener los procesos internos
    /// del form
    /// </summary>
    private void Detener()
    {
        this.tmrFecha.Stop();
    }


    private void frmConfiguracion_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Preguntamos si la aplicación está siendo cerrada por el sistema
        if (e.CloseReason == CloseReason.WindowsShutDown)
        {
            //Mandamos el comando para cerrar SAM
            ApagarPorSistema();
        }
        else
        {
            //Cancelamos el cierre
            e.Cancel = true;
        }
    }

    #endregion

    #region "Botones"

    private void _lblOpcion_0_Click(object sender, EventArgs e)
    {
        switch (step)
        {
            case 1: //Para seleccionar tarjeta de CAN
                BlanquearBotones();

                if (!segundaPaginaAutobus)
                {
                    TarjetaCAN = 1; //Para Mercedes
                }
                else
                {
                    _lblOpcion_1.Visible = false;
                    _lblOpcion_2.Visible = false;
                    _lblOpcion_3.Visible = false;

                    TarjetaCAN = 5; //Para DIDCOM V2
                }

                _lblOpcion_0.Text = ">";
                break;

            case 2: //Para los sistemas
                if (!segundaPaginaSistemas)
                {

                    if (!Catsistemas.CAN)
                    {
                        Catsistemas.CAN = true;
                    }
                    else
                    {
                        Catsistemas.CAN = false;
                    }
                }
                else
                {

                    if (!Catsistemas.Antivirus)
                    {
                        Catsistemas.Antivirus = true;
                    }
                    else
                    {
                        Catsistemas.Antivirus = false;
                    }
                }
                CargarSeleccionSistemas();
                break;
        }

        //if (step == 1) //Para Tarjeta de CAN
        //{
        //    BlanquearBotones();

        //    if (!segundaPaginaAutobus)
        //    {
        //        TarjetaCAN = 1; //Para Mercedes
        //    }
        //    else
        //    {
        //        _lblOpcion_1.Visible = false;
        //        _lblOpcion_2.Visible = false;
        //        _lblOpcion_3.Visible = false;

        //        TarjetaCAN = 5; //Para DIDCOM V2
        //    }

        //    _lblOpcion_0.Text = ">";
        //}
        //else //Para los sistemas
        //{
        //    if (!segundaPaginaSistemas)
        //    {

        //        if (!Catsistemas.CAN)
        //        {
        //            Catsistemas.CAN = true;
        //        }
        //        else
        //        {
        //            Catsistemas.CAN = false;
        //        }
        //    }
        //    else
        //    {

        //        if (!Catsistemas.Antivirus)
        //        {
        //            Catsistemas.Antivirus = true;
        //        }
        //        else
        //        {
        //            Catsistemas.Antivirus = false;
        //        }
        //    }
        //    CargarSeleccionSistemas();
        //}

    }

    private void _lblOpcion_1_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                BlanquearBotones();
                _lblOpcion_1.Text = ">";
                TarjetaCAN = 2; // Para Volvo
                break;

            case 2://Para los sistemas
                if (!segundaPaginaSistemas)
                {

                    if (!Catsistemas.VMD)
                    {
                        Catsistemas.VMD = true;
                    }
                    else
                    {
                        Catsistemas.VMD = false;
                    }
                }
                else
                {

                    if (!Catsistemas.PLAT)
                    {
                        Catsistemas.PLAT = true;
                    }
                    else
                    {
                        Catsistemas.PLAT = false; ;
                    }
                }
                CargarSeleccionSistemas();
                break;

            case 3://Para la seleccion de meta por region

                break;
        }

        //if (step == 1) //Para tarjeta de CAN
        //{
        //    BlanquearBotones();
        //    _lblOpcion_1.Text = ">";
        //    TarjetaCAN = 2; // Para Volvo
        //}
        //else
        //{

        //    if (!segundaPaginaSistemas)
        //    {

        //        if (!Catsistemas.VMD)
        //        {
        //            Catsistemas.VMD = true;
        //        }
        //        else
        //        {
        //            Catsistemas.VMD = false;
        //        }
        //    }
        //    else
        //    {

        //        if (!Catsistemas.PLAT)
        //        {
        //            Catsistemas.PLAT = true;
        //        }
        //        else
        //        {
        //            Catsistemas.PLAT = false; ;
        //        }
        //    }
        //    CargarSeleccionSistemas();
        //}
    }

    private void _lblOpcion_2_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                BlanquearBotones();
                _lblOpcion_2.Text = ">";

                TarjetaCAN = 3; //Para Sacania i6
                break;

            case 2://Para los sistemas
                if (!segundaPaginaSistemas)
                {

                    if (!Catsistemas.CONDUSAT)
                    {
                        Catsistemas.CONDUSAT = true;
                    }
                    else
                    {
                        Catsistemas.CONDUSAT = false;
                    }
                }
                else
                {
                    if (!Catsistemas.SIADLL)
                    {
                        Catsistemas.SIADLL = true;
                    }
                    else
                    {
                        Catsistemas.SIADLL = false;
                    }
                }

                CargarSeleccionSistemas();
                break;

            case 3://Para la seleccion de meta por region

                break;
        }

        //if (step == 1) //Para tarjeta de CAN
        //{

        //    BlanquearBotones();
        //    _lblOpcion_2.Text = ">";

        //    TarjetaCAN = 3; //Para Sacania i6

        //}
        //else
        //{
        //    if (!segundaPaginaSistemas)
        //    {

        //        if (!Catsistemas.CONDUSAT)
        //        {
        //            Catsistemas.CONDUSAT = true;
        //        }
        //        else
        //        {
        //            Catsistemas.CONDUSAT = false;
        //        }
        //    }
        //    else
        //    {
        //        if (!Catsistemas.SIADLL)
        //        {
        //            Catsistemas.SIADLL = true;
        //        }
        //        else
        //        {
        //            Catsistemas.SIADLL = false;
        //        }
        //    }

        //    CargarSeleccionSistemas();
        //}
    }

    private void _lblOpcion_3_Click(object sender, EventArgs e)
    {
        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                BlanquearBotones();
                _lblOpcion_3.Text = ">";

                TarjetaCAN = 4; // Para DIDCOM
                break;

            case 2://Para los sistemas
                if (!segundaPaginaSistemas)
                {
                    if (!Catsistemas.SIA)
                    {
                        Catsistemas.SIA = true;
                    }
                    else
                    {
                        Catsistemas.SIA = false;
                    }
                }
                else
                {
                    if (!Catsistemas.TELEMETRIA)
                    {
                        Catsistemas.TELEMETRIA = true;
                    }
                    else
                    {
                        Catsistemas.TELEMETRIA = false;
                    }
                }

                CargarSeleccionSistemas();
                break;

            case 3://Para la seleccion de meta por region

                break;
        }

        //if (step == 1) // Para tarjeta de CAN
        //{
        //    BlanquearBotones();
        //    _lblOpcion_3.Text = ">";

        //    TarjetaCAN = 4; // Para DIDCOM
        //}
        //else
        //{
        //    if (!segundaPaginaSistemas)
        //    {
        //        if (!Catsistemas.SIA)
        //        {
        //            Catsistemas.SIA = true;
        //        }
        //        else
        //        {
        //            Catsistemas.SIA = false;
        //        }
        //    }
        //    else
        //    {
        //        if (!Catsistemas.TELEMETRIA)
        //        {
        //            Catsistemas.TELEMETRIA = true;
        //        }
        //        else
        //        {
        //            Catsistemas.TELEMETRIA = false;
        //        }
        //    }

        //    CargarSeleccionSistemas();
        //}
    }

    private void btnEnter_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                if (TarjetaCAN != 0)
                {
                    ReiniciarSys();

                    PrimerPaginaSistemas();
                }
                else
                {
                    Error("Se debe seleccionar al menos una opción");
                }
                break;

            case 2://Para los sistemas
                if (seleccionSistema)
                {
                    btnEnter.Enabled = false;
                    btnAtras.Enabled = false;
                    btnArriba.Enabled = false;
                    btnAbajo.Enabled = false;
                    imgADO.Enabled = false;
                    _lblOpcion_0.Enabled = false;
                    _lblOpcion_1.Enabled = false;
                    _lblOpcion_2.Enabled = false;
                    _lblOpcion_3.Enabled = false;

                    CerrarConfig(TarjetaCAN, Catsistemas); /// <--------------
                }
                else
                {
                    Error("Se debe seleccionar al menos una opción");
                }
                break;

            case 3://Para la seleccion de meta por region

                break;
        }

        //if (step == 1)
        //{
        //    if (TarjetaCAN != 0)
        //    {
        //        ReiniciarSys();

        //        PrimerPaginaSistemas();
        //    }
        //    else
        //    {
        //        Error("Se debe seleccionar al menos una opción");
        //    }
        //}
        //else if (step == 2)
        //{
        //    if (seleccionSistema)
        //    {
        //        btnEnter.Enabled = false;
        //        btnAtras.Enabled = false;
        //        btnArriba.Enabled = false;
        //        btnAbajo.Enabled = false;
        //        imgADO.Enabled = false;
        //        _lblOpcion_0.Enabled = false;
        //        _lblOpcion_1.Enabled = false;
        //        _lblOpcion_2.Enabled = false;
        //        _lblOpcion_3.Enabled = false;

        //        CerrarConfig(TarjetaCAN, Catsistemas);
        //    }
        //    else
        //    {
        //        Error("Se debe seleccionar al menos una opción");
        //    }
        //}
    }

    private void btnArriba_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                SeleccionAutobus(TarjetaCAN);
                break;

            case 2://Para los sistemas
                PrimerPaginaSistemas();
                break;

            case 3://Para la seleccion de meta por region

                break;
        }
        //if (step == 1)
        //{
        //    SeleccionAutobus(TarjetaCAN);
        //}
        //else
        //{
        //    PrimerPaginaSistemas();
        //}
    }

    private void btnAbajo_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                SeleccionAutobus2(TarjetaCAN);
                break;

            case 2://Para los sistemas
                SegundaPaginaSistemas();
                break;

            case 3://Para la seleccion de meta por region

                break;
        }
        //if (step == 1)
        //{
        //    SeleccionAutobus2(TarjetaCAN);
        //}
        //else
        //{
        //    SegundaPaginaSistemas();
        //}
    }

    private void btnAtras_Click(object sender, EventArgs e)
    {

        switch (step)
        {
            case 1://Para seleccionar tarjeta de CAN
                PantallaAnterior(3);
                this.Hide();
                break;

            case 2://Para los sistemas
                SeleccionAutobus(TarjetaCAN);
                break;

            case 3://Para la seleccion de meta por region

                break;
        }
        //if (step == 1)
        //{
        //    PantBus();
        //    this.Hide();
        //    //this.TopMost;
        //}
        //else
        //{
        //    SeleccionAutobus(TarjetaCAN);
        //}
    }

    /// <summary>
    /// Podemos ocuparlo para mandar un mensaje al usuario
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void imgADO_Click(object sender, EventArgs e)
    {

    }

    #endregion

    #region "Timers"
    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }



    #endregion
}

