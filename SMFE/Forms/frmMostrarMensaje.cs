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
public partial class frmMostrarMensaje : Form
{

    #region Constructores
    /// <summary>
    /// Constructor Productivo
    /// </summary>
    public frmMostrarMensaje()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="_Nocturno"></param>
    public frmMostrarMensaje(bool _ModoPrueba, bool _Nocturno, string _textosms)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(532, 308);
            this.MinimumSize = new Size(532, 308);
            this.Size = new Size(532, 308);
            Cursor.Hide();
        }

        if (_Nocturno)
        {
            ActivarModonocturno(_Nocturno);
        }

        ProcesarTexto(_textosms);
    }
    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="_Nocturno"></param>
    public frmMostrarMensaje(bool _ModoPrueba, bool _Nocturno, string _textosms, bool POI)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(532, 308);
            this.MinimumSize = new Size(532, 308);
            this.Size = new Size(532, 308);
            Cursor.Hide();
        }

        if (_Nocturno)
        {
            ActivarModonocturno(_Nocturno);
        }

        ProcesarTextoPOI(_textosms);
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
                this.BackgroundImage = Resources.EncuadreINBOXNoc;

                //Textos
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRAMENSAJESNoc;
                

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {

                //Background
                this.BackgroundImage = Resources.EncuadreINBOX;

                //Textos
                lblTitulo.ForeColor = Color.White;


                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRAMENSAJES;
                
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
    /// Se encarga de dividir el texto
    /// </summary>
    /// <param name="texto"></param>
    public void ProcesarTexto(string texto)
    {
        try
        {
            var dividido = texto.Split('|');

            lblFecha.Text = "Fecha: " + dividido[0];
            lblMensaje1.Text = "Texto: " + dividido[1];
        }
        catch
        {
            lblMensaje1.Text = texto;
        }
    }

    /// <summary>
    /// Se encarga de dividir el texto
    /// </summary>
    /// <param name="texto"></param>
    public void ProcesarTextoPOI(string texto)
    {
        try
        {
            lblFecha.Text = "";
            lblTitulo.Text = "¡Mensaje Spots!";
            lblMensaje1.Text = texto;

        }
        catch
        {
            lblMensaje1.Text = texto;
        }
    }
    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmMostrarMensaje_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 130;
        int y = punto.Y + 140;

        this.Location = new Point(x, y);

        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Para evitar que se cierre el form por el usuario
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmMostrarMensaje_FormClosing(object sender, FormClosingEventArgs e)
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
