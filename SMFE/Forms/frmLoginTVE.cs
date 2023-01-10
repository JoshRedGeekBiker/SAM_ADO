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

public partial class frmLoginTVE : Form
{

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmLoginTVE()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    public frmLoginTVE(string _pass, bool _ModoPrueba, bool Nocturno)
    {
        InitializeComponent();

        Password = _pass;

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
    private DateTime UltActividad;
    #endregion

    #region "Eventos"

    //Para mandar un mensaje de error
    public delegate void MandarError(string mensaje);
    public event MandarError Error;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    //Para avisar que la autenticación fué correcta
    public delegate void Login(bool Correcto);
    public event Login LoginBien;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

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
                
                this.lblFecha.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrarNoc;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;

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
                this.lblFecha.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnEnter.BackgroundImage = Resources.BotonAdelante;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrar;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESAR;

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
    /// Se encarga de reiniciar la actividad, reestablece
    /// el último momento de la actividad
    /// </summary>
    /// <returns></returns>
    public void ReiniciaActividad()
    {
        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmLoginTVE_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }

    /// <summary>
    /// Evitamos que se cierre el formulario
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmLoginTVE_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
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

    #region "botones"

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
            LoginBien(true);
            Detener();
            Cerrar(this);
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

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        LoginBien(false);
        Detener();
        Cerrar(this);
    }


    #endregion

    #region "Timers"
    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }


    #endregion
}
