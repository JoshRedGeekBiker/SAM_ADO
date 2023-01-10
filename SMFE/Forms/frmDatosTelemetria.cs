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

public partial class frmDatosTelemetria : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmDatosTelemetria()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="Nocturno"></param>
    /// <param name="Datos"></param>
    public frmDatosTelemetria(bool _ModoPrueba, bool Nocturno, List<string>Datos)
    {
        InitializeComponent();

        lblFecha.Text = DateTime.Now.ToString();

        if (Datos.Count >= 9)
        {
            //Para Códigos
            lblCodigos.Text = "Recibidos: " + Datos.ElementAt(0);
            lblCodigosEnviados.Text = "Enviados: " + Datos.ElementAt(1);
            lblUltLote.Text = Datos.ElementAt(2);
            lblNomLote.Text = Datos.ElementAt(3);

            //Para fallas
            lblFallas.Text = "Recibidas: " + Datos.ElementAt(4);
            lblFallasEnviadas.Text = "Enviadas: " + Datos.ElementAt(5);
            lblUltFalla.Text = Datos.ElementAt(6);
            lblFallaMod.Text = "Módulo: " + Datos.ElementAt(7);
            lblFallaCod.Text = "Código: " + Datos.ElementAt(8);
            lblVersion.Text = "Versión Telemetria: " + Datos.ElementAt(9);
        }

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
    private void frmDatosTelemetria_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        UltActividad = DateTime.Now;
        this.TopMost = true;
    }


    private void frmDatosTelemetria_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    private void frmDatosTelemetria_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
    }

    #endregion

    #region "Botones"
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
