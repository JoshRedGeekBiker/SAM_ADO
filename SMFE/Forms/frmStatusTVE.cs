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

public partial class frmStatusTVE : Form
{
    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmStatusTVE()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor productivo
    /// </summary>
    public frmStatusTVE(bool _ModoPrueba, bool _Exitoso, bool Nocturno)
    {
        InitializeComponent();

        this.lblFecha.Text = DateTime.Now.ToString();

        this.Exitoso = _Exitoso;

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

    /// <summary>
    /// Se encarga de generar los QR de Consulta
    /// </summary>
    /// <returns></returns>
    public delegate Task<bool> ConsultaQR();
    public event ConsultaQR GenerarQR;

    /// <summary>
    /// Se encarga de obtener la informacion de la corrida
    /// </summary>
    /// <returns></returns>
    public delegate string ObtenerInfoCorrida();
    public event ObtenerInfoCorrida InfoCorrida;
    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    #endregion

    #region "Variables"
    private bool Exitoso = false;
    private DateTime UltActividad;
    #endregion

    #region  "Metodos"
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

                if (Exitoso)
                {
                    imgAntena.BackgroundImage = Resources.img_antena_verdeNoc;
                    imgStatus.BackgroundImage = Resources.img_recibidosatisfactoriamenteNoc;
                }
                else
                {
                    imgAntena.BackgroundImage = Resources.img_antena_rojaNoc;
                    imgStatus.BackgroundImage = Resources.img_intentedenuevoNoc;
                }

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

                if (Exitoso)
                {
                    imgAntena.BackgroundImage = Resources.img_antena_verde;
                    imgStatus.BackgroundImage = Resources.img_recibidosatisfactoriamente;
                }
                else
                {
                    imgAntena.BackgroundImage = Resources.img_antena_roja;
                    imgStatus.BackgroundImage = Resources.img_intentedenuevo;
                }

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
    private async void frmStatusTVE_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        lblCorrida.Text = "";

        if (Exitoso)
        {
            imgAntena.BackgroundImage = Resources.img_antena_verde;
            imgStatus.BackgroundImage = Resources.img_recibidosatisfactoriamente;

            try
            {
                await GenerarQR();
                lblCorrida.Text = "Corrida: " + InfoCorrida();

            }
            catch(Exception ex)
            {
                var error = ex.ToString();
            }
        }
        else
        {
            imgAntena.BackgroundImage = Resources.img_antena_roja;
            imgStatus.BackgroundImage = Resources.img_intentedenuevo;
        }
    }

    private void frmStatusTVE_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region "Botones"

    private void imgADO_Click(object sender, EventArgs e)
    {
        MuestraSalir(0);
    }

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        this.Detener();
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
