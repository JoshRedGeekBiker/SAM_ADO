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

/// <summary>
/// Powered ByRED 19MAR2021
/// </summary>
public partial class frmEnviandoMensaje : Form
{


    #region Constructores
    /// <summary>
    /// Constructor Productivo
    /// </summary>
    public frmEnviandoMensaje()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="_Nocturno"></param>
    public frmEnviandoMensaje(bool _ModoPrueba, bool _Nocturno)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(325, 189);
            this.MinimumSize = new Size(325, 189);
            this.Size = new Size(325, 189);
            Cursor.Hide();
        }

        if (_Nocturno)
        {
            ActivarModonocturno(_Nocturno);
        }

    }

    #endregion

    #region Propiedades
    private bool ModoNocturno { get; set; } = false;
    #endregion

    #region Variables
    private DateTime UltActividad;
    #endregion

    #region Eventos
    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

    //Para mandar un Pop Up de error
    public delegate void MandaError(string mensaje);
    public event MandaError Error;
    #endregion

    #region Métodos
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
                this.BackgroundImage = Resources.EncuadreENVIANDOMENSAJENoc;

                //Textos
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRARNoc;
                btnCerrar.BackgroundImageLayout = ImageLayout.Stretch;
                imgCorrecto.BackgroundImage = Resources.ConductorValidoNoc;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {

                //Background
                this.BackgroundImage = Resources.EncuadreENVIANDOMENSAJE;

                //Textos
                lblTitulo.ForeColor = Color.White;


                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRAR;
                btnCerrar.BackgroundImageLayout = ImageLayout.Stretch;
                imgCorrecto.BackgroundImage = Resources.ConductorVALIDO;

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
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmEnviandoMensaje_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 236;
        int y = punto.Y + 199;

        this.Location = new Point(x, y);

        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Para evitar que se cierre el form por el usuario
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmEnviandoMensaje_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }


    #endregion

    #region Botones
    /// <summary>
    /// Cerrar el form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCerrar_Click(object sender, EventArgs e)
    {
        Cerrar(this);
    }


    #endregion

    #region Timers

    #endregion

    #region Acciones

    #endregion
}
