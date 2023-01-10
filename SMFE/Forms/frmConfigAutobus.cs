using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using SMFE.Properties;

public partial class frmConfigAutobus : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmConfigAutobus()
        {
            InitializeComponent();
            this.imgSenial.Visible = false;
        }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_version"></param>
    /// <param name="_ModoPrueba"></param>
    /// <param name="_Regiones"></param>
    public frmConfigAutobus(string _version, bool _ModoPrueba, List<string> _Regiones, bool Nocturno, bool _btnOff)
    {
        InitializeComponent();

        this.lblVersion.Text = _version;
        this.lblFecha.Text = DateTime.Now.ToString();
        this.imgSenial.Visible = false;
        this.listaRegion.Visible = false;
        this.btnApagar.Visible = _btnOff;

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

        CargarRegiones(_Regiones);

        ConfigurarStep1();
    }

    #endregion

    #region "Propiedades"
    public string NumAutobus { get; set; }
    public bool Cargado { get; set; } = false;

    private bool ModoNocturno { get; set; } = false;
    #endregion

    #region "Variables"

    private int step = 1;
    private string strRegion = string.Empty;

    #endregion

    #region "Eventos"

    //Sirve verificar la existencia de la region
    public delegate string ChecarRegion(long reg);
    public event ChecarRegion MandaRegion;

    /// <summary>
    /// Manda a mostrar la siguiente pantalla de configuracion
    /// </summary>
    /// <param name="autobus"></param>
    /// <param name="pantallaOrigen"></param>
    public delegate void SiguienteConfig(int pantallaOrigen);
    public event SiguienteConfig Config;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso 0:apagar, 1:salir
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    //Sirve para mostrar la siguiente pantalla
    //si ya fué abierta
    //public delegate void MostrarPantSys(string autobus);
    //public event MostrarPantSys PantSys;

    public delegate void MostrarError(string mensaje);
    public event MostrarError Error;

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
                this.BackgroundImage = Resources.FondoCONblancoNoc;

                //Textos
                lblFecha.ForeColor = Color.Gray;
                lblVersion.ForeColor = Color.Gray;
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btnApagar.BackgroundImage = Resources.btn_offNoc;
                btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                btnBorrar.BackgroundImage = Resources.BotonBorrarNoc;
                btnAtras.BackgroundImage = Resources.BotonREGRESARNoc;
                this.imgADO.BackgroundImage = Resources.LogoADONoc;

                //Numeros
                lbl0.Image = Resources.BotonNUMEROSNoc;
                lbl1.Image = Resources.BotonNUMEROSNoc;
                lbl2.Image = Resources.BotonNUMEROSNoc;
                lbl3.Image = Resources.BotonNUMEROSNoc;
                lbl4.Image = Resources.BotonNUMEROSNoc;
                lbl5.Image = Resources.BotonNUMEROSNoc;
                lbl6.Image = Resources.BotonNUMEROSNoc;
                lbl7.Image = Resources.BotonNUMEROSNoc;
                lbl8.Image = Resources.BotonNUMEROSNoc;
                lbl9.Image = Resources.BotonNUMEROSNoc;


                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {

                //Textos
                lblFecha.ForeColor = Color.White;
                lblVersion.ForeColor = Color.White;
                lblTitulo.ForeColor = Color.White;

                //Botones
                btnApagar.BackgroundImage = Resources.btn_off;
                btnEnter.BackgroundImage = Resources.BotonAdelante;
                btnBorrar.BackgroundImage = Resources.BotonBorrar;
                btnAtras.BackgroundImage = Resources.BotonREGRESAR;
                this.imgADO.BackgroundImage = Resources.LogoADO;

                //Numeros
                lbl0.Image = Resources.BotonNUMEROS;
                lbl1.Image = Resources.BotonNUMEROS;
                lbl2.Image = Resources.BotonNUMEROS;
                lbl3.Image = Resources.BotonNUMEROS;
                lbl4.Image = Resources.BotonNUMEROS;
                lbl5.Image = Resources.BotonNUMEROS;
                lbl6.Image = Resources.BotonNUMEROS;
                lbl7.Image = Resources.BotonNUMEROS;
                lbl8.Image = Resources.BotonNUMEROS;
                lbl9.Image = Resources.BotonNUMEROS;

                ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// Sirve para configurar el numero de la unidad
    /// </summary>
    private void ConfigurarStep1()
    {
        step = 1;
        lblTitulo.Text = "Configurar Autobús";
        lblMensaje.Text = "Esta unidad no cuenta con número de autobús. Teclee el número de autobus";
        lblMensaje.Visible = true;
        NumAutobus = string.Empty;
        txtDisplay.Text = "";
        listaRegion.Visible = false;
        btnAtras.Visible = false;
    }

    /// <summary>
    /// Sirva para configurar la región a la que pertenece
    /// </summary>
    public void ConfigurarStep2()
    {

        step = 2;
        lblTitulo.Text = "Configurar Region";
        imgSenial.Visible = false;
        txtDisplay.Text = "";
        //lblMensaje.Text = "Indique la region a la que pertenece";
        lblMensaje.Visible = false;
        btnAtras.Visible = true;
        listaRegion.ResetText();
        listaRegion.Text = "Seleccione una Region:";

        strRegion = "";


        listaRegion.Visible = true;
     }

    /// <summary>
    /// Se encarga de llenar el Combobox con 
    /// las regiones disponibles
    /// </summary>
    /// <param name="Regiones"></param>
    private void CargarRegiones(List<string> Regiones)
    {
        foreach(string str in Regiones)
        {
            listaRegion.Items.Add(str);
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

    /// <summary>
    /// Para evitar que se introduzca alguna letra o caracter especial
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtDisplay_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (Char.IsNumber(e.KeyChar))
        {
            e.Handled = false;
        }
        else if (Char.IsControl(e.KeyChar))
        {
            e.Handled = false;
        }
        else if (Char.IsSeparator(e.KeyChar))
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }

    private void frmConfigAutobus_FormClosing(object sender, FormClosingEventArgs e)
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

    private void frmConfigAutobus_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }

    private void listaRegion_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtDisplay.Text = RecuperarNumeroRegion(listaRegion.Items[listaRegion.SelectedIndex].ToString());
    }

    /// <summary>
    /// Se encarga de recuperar el numero de Region y guardarlo en base de Datos
    /// </summary>
    /// <param name="seleccion"></param>
    /// <returns></returns>
    private string RecuperarNumeroRegion(string seleccion)
    {
        strRegion = string.Empty;

        string[] opciones = seleccion.Split(' ');

        for (int i = 2; i < opciones.Count(); i++)
        {
            strRegion = strRegion + opciones[i] + " ";
        }

        return opciones[0];
    }
    #endregion

    #region "Botones"

    private void lbl1_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "1";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl2_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "2";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl3_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "3";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl4_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "4";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl5_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "5";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl6_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "6";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl7_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "7";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl8_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "8";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl9_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "9";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void lbl0_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "0";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

    private void btnEnter_Click(object sender, EventArgs e)
    {
        if (step == 1)
        {
            NumAutobus = txtDisplay.Text;

            if (NumAutobus.Equals(""))
            {
                if (ModoNocturno)
                {
                    imgSenial.BackgroundImage = Resources.ConductorIncorrectoNoc;
                }
                else
                {
                    imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
                }

                imgSenial.Visible = true;

                lblMensaje.Text = "Ingrese un número de autobus";

                return;
            }

            if (ModoNocturno)
            {
                imgSenial.BackgroundImage = Resources.ConductorValidoNoc;
            }
            else
            {
                imgSenial.BackgroundImage = Resources.ConductorVALIDO;
            }
            
            imgSenial.Visible = true;

            lblMensaje.Text = "Número de unidad: " + NumAutobus;

            Application.DoEvents();

            Thread.Sleep(2000);


            //Configuramos el Step 2
            ConfigurarStep2();
            
        }
        else if (step == 2)
        {
            if (txtDisplay.Text.Equals(""))
            {
                Error("Seleccione o introduzca una región");
                return;
            }

            strRegion = MandaRegion(Convert.ToInt64(txtDisplay.Text));

            if (strRegion == "")
            {
                Error("La Region especificada no es valida");
                txtDisplay.Text = "";
            }
            else
            {
                imgSenial.BackgroundImage = Resources.ConductorVALIDO;
                imgSenial.Visible = true;
                listaRegion.Visible = false;
                lblMensaje.Visible = true;
                    
                lblMensaje.Text = "Region " + strRegion + " seleccionada.";

                Application.DoEvents();

                Thread.Sleep(2000);

                //Cambio de lógica por MetasPorRegionCAN
                //Powered ByRED 10/SEP/2020

                Config(1);

                //if (SegundaPantalla)
                //{
                //    //Manda evento para segundapantalla cargada
                //    PantSys(NumAutobus);
                //}
                //else
                //{
                //    Config(NumAutobus);
                //}
            }


            //if (MandaRegion(Convert.ToInt64(txtDisplay.Text)))
            //{
            //    imgSenial.BackgroundImage = Resources.ConductorVALIDO;
            //    imgSenial.Visible = true;
            //    listaRegion.Visible = false;
            //    lblMensaje.Visible = true;
            //    if (strRegion.Equals(""))
            //    {
            //        lblMensaje.Text = "Region " + txtDisplay.Text + " seleccionada.";
            //    }
            //    else
            //    {
            //        lblMensaje.Text = strRegion + " seleccionada.";
            //    }

            //    Application.DoEvents();

            //    Thread.Sleep(2000);


            //    if (SegundaPantalla)
            //    {
            //        //Manda evento para segundapantalla cargada
            //        PantSys(NumAutobus);
            //    }
            //    else
            //    {
            //        Config(NumAutobus);
            //    }
            //}
            //else
            //{

            //    Error("La Region especificada no es valida");
            //    txtDisplay.Text = "";
            //    //lblMensaje.Text = "La region especificada no es valida, especifique una nueva. Se conservará la última región";

            //    //txtDisplay.Text = "";
            //    //if (ModoNocturno)
            //    //{
            //    //    imgSenial.BackgroundImage = Resources.ConductorIncorrectoNoc;
            //    //}
            //    //else
            //    //{
            //    //    imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
            //    //}

            //    //imgSenial.Visible = true;

            //    //Application.DoEvents();
            //}
        }
    }

    private void btnBorrar_Click(object sender, EventArgs e)
    {
        if (txtDisplay.TextLength > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Count() - 1);
            txtDisplay.SelectionStart = txtDisplay.TextLength;
        }
    }

    private void btnAtras_Click(object sender, EventArgs e)
    {
        ConfigurarStep1();
    }

    private void btnApagar_Click(object sender, EventArgs e)
    {
        MuestraSalir(0);
    }

    #endregion

    #region  "Timers"
    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }

    #endregion
}
