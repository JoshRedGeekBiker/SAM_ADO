using SMFE.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


public partial class frmMenuSpots : Form
{

    #region "Constructores"
    public frmMenuSpots()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_modoPrueba"></param>
    /// <param name="Nocturno"></param>
    public frmMenuSpots(bool _ModoPrueba, bool Nocturno, String version)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        lblFecha.Text = DateTime.Now.ToString();
        lblVersion.Text += ": "+version;

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
    #endregion

    #region "Porpiedades"
    private bool ModoNocturno { get; set; } = false;

    #endregion

    #region "Variables"
    private DateTime UltActividad;

    #endregion

    #region "Eventos"
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;
    /// <summary>
    /// Se encargará de mandar a llamar el cargador de Spots
    /// </summary>
    public delegate void VMDCargadorSpots(String tipo);
    public event VMDCargadorSpots CargadorSpots;
    #endregion

    #region "Métodos"
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
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;
                btnAudio.BackgroundImage = Resources.btl_audiosNoc;
                btnVideo.BackgroundImage = Resources.btl_videosNoc;

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
                lblTitulo.ForeColor = Color.White;

                //Botones
                btnRegresar.BackgroundImage = Resources.BotonREGRESAR;
                btnAudio.BackgroundImage = Resources.btl_audios;
                btnVideo.BackgroundImage = Resources.btl_videos;

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
    private void frmMenuSpots_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        UltActividad = DateTime.Now;
        this.TopMost = true;
    }
    private void frmMenuSpots_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;

    }
    private void frmMenuSpots_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByToto ENERO2023
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region "Botones"
    private void btnAudio_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
        CargadorSpots("audio");
    }

    private void btnVideo_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
        CargadorSpots("video");

    }


    private void btnRegresar_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
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

