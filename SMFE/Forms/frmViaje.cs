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


public partial class frmViaje : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor principal
    /// </summary>
    public frmViaje()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="Nocturno"></param>
    public frmViaje(bool _ModoPrueba, bool Nocturno, bool _btnoff)
    {
        InitializeComponent();

        lblFecha.Text = DateTime.Now.ToString();
        this.imgOK.Visible = false;
        this.btnApagar.Visible = _btnoff;

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
    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    #endregion

    #region "Variables"

    private DateTime UltActividad;

    #endregion

    #region "Eventos"
    //Mandar la acción de un viaje
    public delegate void MandarViaje(string clvConductor);
    public event MandarViaje Viaje;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso 0:apagar, 1:salir
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

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

                //Textos
                this.lblFecha.ForeColor = Color.Gray;
                this.lblTitulo.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnApagar.BackgroundImage = Resources.btn_offNoc;
                this.btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrarNoc;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESARNoc;

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
                //Background
                this.BackgroundImage = Resources.FondoCONblanco;

                //Textos
                this.lblFecha.ForeColor = Color.White;
                this.lblTitulo.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnApagar.BackgroundImage = Resources.btn_off;
                this.btnEnter.BackgroundImage = Resources.BotonAdelante;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrar;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESAR;

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
    /// Se encarga de verificar si ya pasó el tiempo de 
    /// actividad del form
    /// </summary>
    /// <returns></returns>
    public bool VerificaActividad(int TiempoEspera)
    {
        if ((DateTime.Now - UltActividad).TotalSeconds >= TiempoEspera)
        {
            return false;
        }
        else
        {
            return true;
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
    /// Se encarga de detener los procesos internos
    /// del form
    /// </summary>
    private void Detener()
    {
        this.tmrFecha.Stop();
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmViaje_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        UltActividad = DateTime.Now;
    }

    public void indicador(string mensaje, bool correcto, bool visible)
    {

        txtDisplay.Text = "";

        if (correcto)
        {
            imgOK.BackgroundImage = Resources.ConductorVALIDO;
        }
        else
        {
            imgOK.BackgroundImage = Resources.ConductorIncorrecto;
        }

        imgOK.Visible = visible;
        lblMensaje.Visible = visible;
        lblMensaje.Text = mensaje;

    }

    private void frmViaje_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region "Botones"

    private void lbl0_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "0";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl1_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "1";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl2_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "2";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl3_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "3";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl4_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "4";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl5_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "5";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl6_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "6";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl7_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "7";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl8_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "8";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void lbl9_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "9";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void btnEnter_Click(object sender, EventArgs e)
    {
        lblMensaje.Text = "";
        lblMensaje.Visible = false;
        imgOK.Visible = false;

        Viaje(txtDisplay.Text);
        UltActividad = DateTime.Now;
    }

    private void btnBorrar_Click(object sender, EventArgs e)
    {
        if (txtDisplay.TextLength > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Count() - 1);
            txtDisplay.SelectionStart = txtDisplay.TextLength;
        }
        UltActividad = DateTime.Now;
    }

    private void btnAtras_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
        Detener();
        Cerrar(this);
    }

    private void btnApagar_Click(object sender, EventArgs e)
    {
        MuestraSalir(0);
    }

    private void imgADO_Click(object sender, EventArgs e)
    {
        MuestraSalir(1);
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
