using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMFE.Properties;
using System.Windows.Forms;
using SMFE.Properties;


public partial class frmSalida : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmSalida()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_pass"></param>
    /// <param name="_version"></param>
    /// <param name="_ModoPrueba"></param>
    /// <param name="_tipo"></param>
    public frmSalida(string _pass, string _version, bool _ModoPrueba, int _tipo, bool Nocturno, bool _Config)
    {
        InitializeComponent();

        lblFecha.Text = DateTime.Now.ToString();

        Password = _pass;
        lblVersion.Text = _version;
        tipo = _tipo;
        Config = _Config;

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

        ConfigurarSegunTipo();
    }

    #endregion

    #region "Propiedades
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    #endregion

    #region "Variables"

    private string Password = string.Empty;
    private int tipo;
    private DateTime UltActividad;
    private bool Config = false;

    #endregion

    #region "Eventos"
    /// <summary>
    /// Avisa que tiene que apagar
    /// </summary>
    public delegate void MandarApagar();
    public event MandarApagar Apagar;

    public delegate void MandarApagarConfig();
    public event MandarApagarConfig ApagarConfig;

    public delegate void MandarSalir();
    public event MandarSalir Salir;

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
                this.lblVersion.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnEnter.BackgroundImage = Resources.BotonAdelanteNoc;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrarNoc;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESARNoc;
                this.imgSenal.BackgroundImage = Resources.ConductorValidoNoc;

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
                this.lblVersion.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnEnter.BackgroundImage = Resources.BotonAdelante;
                this.btnBorrar.BackgroundImage = Resources.BotonBorrar;
                this.btnAtras.BackgroundImage = Resources.BotonREGRESAR;
                this.imgSenal.BackgroundImage = Resources.ConductorVALIDO;

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
    private void frmSalida_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        imgSenal.Visible = false;
        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Se encarga de configurar el form de acuerdo al tipo de funcion
    /// que se requiera
    /// </summary>
    private void ConfigurarSegunTipo()
    {
        //0 Para Apagar
        //1 Para Salir

        switch(tipo)
        {

            case 0:

                this.lblTitulo.Text = "Apagar Equipo";

                break;


            case 1:

                this.lblTitulo.Text = "Salir de la aplicacion";

                break;


            default: break;
        }

        UltActividad = DateTime.Now;
    }

    private void frmSalida_FormClosing(object sender, FormClosingEventArgs e)
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
        try//Powered ByRED 20JUL2021
        {
            if (txtDisplay.Text.Equals(Password))
            {
                switch (tipo)
                {
                    case 0:
                        if (ModoNocturno)
                        {
                            this.imgSenal.BackgroundImage = Resources.ConductorValidoNoc;
                        }
                        else
                        {
                            this.imgSenal.BackgroundImage = Resources.ConductorVALIDO;
                        }

                        this.imgSenal.Visible = true;
                        this.CancelarBotones();
                        this.txtDisplay.Text = "";
                        this.lblMensaje.Text = "El sistema se está apagando \nespere un momento por favor";

                        //Poner logica de config

                        if (this.Config)
                        {
                            //Rutina para apagar el equipo en modo Config
                            ApagarConfig();
                        }
                        else
                        {
                            //Rutina para apagar el equipo en modo No CONFIG
                            Apagar();
                        }

                        break;

                    case 1:

                        Salir();

                        break;

                    default: break;
                }


                this.Close();
                this.Dispose();
            }
            else
            {
                lblMensaje.Text = "Clave incorrecta";
                lblMensaje.Visible = true;
                if (ModoNocturno)
                {
                    imgSenal.BackgroundImage = Resources.ConductorIncorrectoNoc;
                }
                else
                {
                    imgSenal.BackgroundImage = Resources.ConductorIncorrecto;
                }

                imgSenal.Visible = true;
                txtDisplay.Text = "";
            }
            UltActividad = DateTime.Now;
        }
        catch(Exception ex)
        {
            var error = ex.ToString();

            MessageBox.Show(error);
        }
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
        Detener();
        Cerrar(this);
    }

    /// <summary>
    /// Se encarga de inhabilitar los botones
    /// </summary>
    private void CancelarBotones()
    {

        this.lbl0.Enabled = false;
        this.lbl1.Enabled = false;
        this.lbl2.Enabled = false;
        this.lbl3.Enabled = false;
        this.lbl4.Enabled = false;
        this.lbl5.Enabled = false;
        this.lbl6.Enabled = false;
        this.lbl7.Enabled = false;
        this.lbl8.Enabled = false;
        this.lbl9.Enabled = false;

        this.btnEnter.Enabled = false;
        this.btnAtras.Enabled = false;
        this.btnBorrar.Enabled = false;

    }

    #endregion

    #region "Timers"
    private void tmrFechaHora_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }

    #endregion
}

