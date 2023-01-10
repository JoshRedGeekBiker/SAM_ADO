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
using System.IO;

public partial class frmQRTVE : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmQRTVE()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmQRTVE(bool _ModoPrueba, bool Nocturno)
    {
        InitializeComponent();

        this.lblFecha.Text = DateTime.Now.ToString();
        this.imgQR.Visible = false;

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

    #region "Eventos"

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

    //Para mandar a llamar la vista del pass
    public delegate void IntroducirPass();
    public event IntroducirPass Pass;

    //Para mostrar la ventana de login
    public delegate void Mostrarlogin();
    public event Mostrarlogin Login;

    //Para mandar a mostrar algún error
    public delegate void MostrarError(string Mensaje);
    public event MostrarError Error;

    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    public bool EstadoLogin { get; set; } = false;
    #endregion

    #region "Variables"
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
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;
                this.btnInspector.BackgroundImage = Resources.btl_conductorinspectorNoc;
                this.btnBitacora.BackgroundImage = Resources.btl_bitacoraNoc;

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
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESAR;
                this.btnInspector.BackgroundImage = Resources.btl_conductorinspector;
                this.btnBitacora.BackgroundImage = Resources.btl_bitacora;

                ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// Se encarga de validar si el inicio es correcto, de lo contrario cerramos la pantalla
    /// </summary>
    public void ValidarLogin()
    {
        if (EstadoLogin)
        {
            this.imgQR.Visible = true;
        }
        else
        {
            this.Detener();
            this.Cerrar(this);
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
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmQRTVE_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }

    private void frmQRTVE_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }
    #endregion

    #region"Botones"

    private void imgADO_Click(object sender, EventArgs e)
    {
        MuestraSalir(1);
    }

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
    }

    private void btnInspector_Click(object sender, EventArgs e)
    {
        this.btnBitacora.Visible = false;

        var path = @"C:\xampp\htdocs\phpqrcode\QRFILES\qrInspector.html";
        if (File.Exists(path))
        {
            //mandamos a mostrar el login
            this.imgQR.Navigate(path);
            Login();

        }
        else
        {
            //Error
        }
    }

    private void btnBitacora_Click(object sender, EventArgs e)
    {
        this.btnInspector.Visible = false;
        var path = @"C:\xampp\htdocs\phpqrcode\QRFILES\qrsecure.html";
        if (System.IO.File.Exists(path))
        {
            this.imgQR.Navigate(path);
            this.imgQR.Visible = true;
        }
        else
        {
            //error
        }
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