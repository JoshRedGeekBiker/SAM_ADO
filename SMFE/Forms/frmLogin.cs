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


public partial class frmLogin : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmLogin()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Constructor para Front Sistemas
    /// </summary>
    /// <param name="_pass"></param>
    /// <param name="_version"></param>
    /// <param name="_ModoPrueba"></param>
    public frmLogin(string _pass, string _version, bool _ModoPrueba, bool Nocturno, bool _btnoff)
    {
        InitializeComponent();

        Password = _pass;
        this.lblVersion.Text = _version;
        this.lblFecha.Text = DateTime.Now.ToString();
        this.btnOff.Visible = _btnoff;

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }

    /// <summary>
    /// Constructor para Front Configuración
    /// </summary>
    /// <param name="_pass"></param>
    /// <param name="_version"></param>
    /// <param name="_modoPrueba"></param>
    /// <param name="_Config"></param>
    public frmLogin(string _pass, string _version, bool _ModoPrueba, bool _Config, bool Nocturno, bool _btnoff)
    {
        InitializeComponent();

        Password = _pass;
        this.lblVersion.Text = _version;
        enConfiguracion = _Config;

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
    
    #endregion

    #region "Variables"

    private string Password = string.Empty;
    private bool enConfiguracion = false;

    #endregion

    #region "Eventos"

    //Para mandar mostrar el form de apagar
    public delegate void MandarApagar(int Tipo);
    public event MandarApagar Apagar;

    //Para mandar un mensaje de error
    public delegate void MandarError(string mensaje);
    public event MandarError Error;

    //Para continuar con la configuracion
    public delegate void ContinuaConfig(int pantallaOrigen);
    public event ContinuaConfig Config;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso 0:apagar, 1:salir
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    //Avisa al Front que hubo un Login Correcto
    public delegate void Login();
    public event Login LoginOK;

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
                //Background
                this.BackgroundImage = Resources.FondoCONblancoNoc;

                //textos
                this.lblVersion.ForeColor = Color.Gray;
                this.lblVersion.ForeColor = Color.Gray;
                this.lblFecha.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnOff.BackgroundImage = Resources.btn_offNoc;
                this.btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrarNoc;

                //Numeros
                this.lbl0.Image = Resources.BotonNUMEROSNoc;
                this.lbl1.Image = Resources.BotonNUMEROSNoc;
                this.lbl2.Image = Resources.BotonNUMEROSNoc;
                this.lbl3.Image = Resources.BotonNUMEROSNoc;
                this.lbl4.Image = Resources.BotonNUMEROSNoc;
                this.lbl5.Image = Resources.BotonNUMEROSNoc;
                this.lbl6.Image = Resources.BotonNUMEROSNoc;
                this.lbl7.Image = Resources.BotonNUMEROSNoc;
                this.lbl8.Image = Resources.BotonNUMEROSNoc;
                this.lbl9.Image = Resources.BotonNUMEROSNoc;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {
                //Backgound
                this.BackgroundImage = Resources.FondoCONblanco;

                //Textos
                this.lblVersion.ForeColor = Color.White;
                this.lblFecha.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnOff.BackgroundImage = Resources.btn_off;
                this.btnEnter.BackgroundImage = Resources.BotonAdelante;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrar;

                //Numeros
                this.lbl0.Image = Resources.BotonNUMEROS;
                this.lbl1.Image = Resources.BotonNUMEROS;
                this.lbl2.Image = Resources.BotonNUMEROS;
                this.lbl3.Image = Resources.BotonNUMEROS;
                this.lbl4.Image = Resources.BotonNUMEROS;
                this.lbl5.Image = Resources.BotonNUMEROS;
                this.lbl6.Image = Resources.BotonNUMEROS;
                this.lbl7.Image = Resources.BotonNUMEROS;
                this.lbl8.Image = Resources.BotonNUMEROS;
                this.lbl9.Image = Resources.BotonNUMEROS;

                
                ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmLogin_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }
    
    /// <summary>
    /// Evitamos que se cierre el formulario 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Preguntamos si la aplicación está siendo cerrada por el sistema
        if (e.CloseReason == CloseReason.WindowsShutDown)
        {
            if (this.enConfiguracion)
            {
                //Mandamos el comando para cerrar SAM
                ApagarPorSistema();
            }
        }
        else
        {
            //Cancelamos el cierre
            e.Cancel = true;
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

    #endregion

    #region  "Botones"

    private void lbl0_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "0";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
    }

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

    private void btnEnter_Click(object sender, EventArgs e)
    {
        if (txtDisplay.Text.Equals(Password))
        {
            if (enConfiguracion)
            {
                Config(0);
            }
            else
            {
                //Avisamos al front que el Log fué Correcto
                LoginOK();
                //Cerrar(this);
            }
        }
        else
        {
            txtDisplay.Text = "";

            Error("La clave no es válida");
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

    private void btnOff_Click(object sender, EventArgs e)
    {
        Apagar(0);
    }

    #endregion

    #region "Timers"

    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }

    /// <summary>
    /// Timer que se encarga de retrasar unas milesimas de segundo
    /// la visibilidad del form para que no se vea ralentizado mientras 
    /// se carga.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mostrar_Tick(object sender, EventArgs e)
    {
        //if (!primer_inicio)
        //{
        //    this.TopMost = true;
        //    mostrar.Stop();
        //    mostrar.Enabled = false;
        //}
        //else
        //{
        //    primer_inicio = false;
        //}
    }
    #endregion

   
}