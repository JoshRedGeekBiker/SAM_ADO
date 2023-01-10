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

public partial class frmError : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmError()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="Mensaje"></param>
    /// <param name="_ModoPrueba"></param>
    public frmError(string Mensaje, bool _ModoPrueba, bool Nocturno)
    {
        this.mensaje = Mensaje;
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

        //tmrSalir.Start();
    }
    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    #endregion

    #region "Variables"
    private string mensaje = string.Empty;
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

                //botones
                this.btnSalir.BackgroundImage = Resources.BotonBorrarNoc;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {
                //Background
                this.BackgroundImage = Resources.EncuadreAVISOS;

                //botones
                this.btnSalir.BackgroundImage = Resources.BotonBorrar;

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
        //tmrCerrar.Stop();
        //tmrSalir.Stop();
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmError_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 148;
        int y = punto.Y + 185;

        this.Location = new Point(x, y);

        lblMensaje.Text = this.mensaje;
        this.TopMost = true;
        UltActividad = DateTime.Now;
    }

    private void frmError_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }
    #endregion

    #region "Botones"
    private void btnSalir_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
    }
    #endregion

    #region "Timers"

    //if (!PrimerInicio)
    //{
    //    tmrSalir.Stop();
    //    this.Close();
    //    this.Dispose();
    //}
    //else
    //{
    //    this.PrimerInicio = false;
    //}


    //    if (Actividad)
    //        {
    //            Actividad = false;
    //            UltActividad = DateTime.Now;
    //        }

    //        if ((DateTime.Now - UltActividad).TotalSeconds >= 10)
    //        {
    //            Detener();
    //Cerrar(this);
    //        }
    #endregion


}

