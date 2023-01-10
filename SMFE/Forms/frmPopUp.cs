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

public partial class frmPopUp : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmPopUp()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    public frmPopUp(bool _ModoPrueba, bool Nocturno)
    {
      InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(504, 234);
            this.MinimumSize = new Size(504, 234);
            this.Size = new Size(504, 234);
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
    private DateTime tiempo;

    #endregion

    #region "Eventos"
    public delegate List<string> TraerDatos();
    public event TraerDatos Datos;

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
                this.BackgroundImage = Resources.EncuadreAVISOSNoc;

                //Textos
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btn_Cerrar.BackgroundImage = Resources.BotonBorrarNoc;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {

                //Background
                this.BackgroundImage = Resources.EncuadreAVISOS;

                //Textos
                lblTitulo.ForeColor = Color.White;

                //Botones
                btn_Cerrar.BackgroundImage = Resources.BotonBorrar;

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
    /// Se encarga de lanzar el delay para cerrar la vista
    /// </summary>
    private void TiempoCerrar()
    {
        tiempo = DateTime.Now;
        tmrCerrar.Enabled = true;
        tmrCerrar.Start();
    }

    /// <summary>
    /// Se encarga de detener los procesos internos
    /// del form
    /// </summary>
    private void Detener()
    {
        this.tmrActualiza.Stop();
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmPopUp_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 139;
        int y = punto.Y + 163;

        this.Location = new Point(x, y);

        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Se encarga de configurar los componentes de acuerdo al 
    /// tipo de vista que se quiere ver
    /// </summary>
    public void ConfigurarForm(string tipo)
    {
        var _datos = Datos();
        switch (tipo)
        {

            case "GPS":

                this.lblTitulo.Text = "Datos GPS";

                if (_datos.Count == 5)
                {
                    this.lblPrimer.Text = "Lat.:" + _datos.ElementAt(0) + " (" + _datos.ElementAt(1) + ")";

                    this.lblSegundo.Text = "Long.:" + _datos.ElementAt(2) + " (" + _datos.ElementAt(3) + ")";

                    this.lblTercero.Text = "Vel.:" + _datos.ElementAt(4) + " Km/h";

                    //Encender el Timer para la actualización de los datos GPS
                    tmrActualiza.Start();

                }
                else
                {
                    this.lblPrimer.Text = "Lat.:  ()";

                    this.lblSegundo.Text = "Long.:()";

                    this.lblTercero.Text = "Vel.: 0 Km/h";
                }

                this.lblTercero.TextAlign = ContentAlignment.TopLeft;

                break;


            case "VIAJE":

                if (_datos.Count == 3)
                {
                    this.lblPrimer.Text = "Viaje:          " + _datos.ElementAt(0);
                    this.lblSegundo.Text = "Conductor: " + _datos.ElementAt(1);
                    this.lblTercero.Text = _datos.ElementAt(2);
                }
                else
                {
                    this.lblPrimer.Text = "Viaje:     Cerrado";

                    this.lblSegundo.Text = "Conductor:     000000";

                    this.lblTercero.Text = "Nombre del conductor";
                }

                this.lblTitulo.Text = "Datos del viaje";

                this.lblTercero.TextAlign = ContentAlignment.MiddleCenter;

                break;

            default:

                if (_datos.Count > 0)
                {
                    this.lblTitulo.Text = "";
                    this.btn_Cerrar.Visible = false;
                    this.lblPrimer.Text = "";
                    this.lblSegundo.Font = new Font("Verdana", 18, FontStyle.Bold);
                    this.lblSegundo.Size = new Size(452, 60);
                    this.lblSegundo.Text = _datos.ElementAt(0);
                    this.lblTercero.Text = "";
                    TiempoCerrar();

                }

                tmrSiempreAdelante.Enabled = true;
                this.Opacity = 1.0;
                tmrSiempreAdelante.Start();

                break;
        }

        UltActividad = DateTime.Now;
    }

    private void frmPopUp_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    #endregion

    #region "Botones"

    private void btn_Cerrar_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
    }

    #endregion

    #region "Timers"

    private void tmrActualiza_Tick(object sender, EventArgs e)
    {

        tmrActualiza.Stop();

        var _datos = Datos();

        if (_datos.Count == 5)
        {
            this.lblPrimer.Text = "Lat.:" + _datos.ElementAt(0) + " (" + _datos.ElementAt(1) + ")";

            this.lblSegundo.Text = "Long.:" + _datos.ElementAt(2) + " (" + _datos.ElementAt(3) + ")";

            this.lblTercero.Text = "Vel.:" + _datos.ElementAt(4) + " Km/h";

            //Encender el Timer

        }
        else
        {
            this.lblPrimer.Text = "Lat.:  ()";

            this.lblSegundo.Text = "Long.:()";

            this.lblTercero.Text = "Vel.: 0 Km/h";
        }

        tmrActualiza.Start();

    }

    /// <summary>
    /// Se encarga de mantener el form siempre adelante cuando esté en modo configuración
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrSiempreAdelante_Tick(object sender, EventArgs e)
    {
        tmrSiempreAdelante.Stop();
        this.TopMost = true;
        tmrSiempreAdelante.Start();
    }

    private void tmrCerrar_Tick(object sender, EventArgs e)
    {
        tmrCerrar.Stop();

        if ((DateTime.Now - tiempo).TotalSeconds >= 3)
        {
            this.Close();
            this.Dispose();
            return;
        }

        tmrCerrar.Start();
    }

    #endregion
}

