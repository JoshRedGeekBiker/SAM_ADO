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

public partial class frmMovtosCAN : Form
{
    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmMovtosCAN()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_datos"></param>
    /// <param name="_ModoPrueba"></param>
    public frmMovtosCAN(List<string> _datos, bool _ModoPrueba, bool Nocturno)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(533, 305);
            this.MinimumSize = new Size(533, 305);
            this.Size = new Size(533, 305);
            Cursor.Hide();
        }

        ChecarHorario();

        if (_datos.Count() == 4)
        {
            this.lblMtosCan.Text = _datos.ElementAt(0);
            this.lblCvleAutobus.Text = _datos.ElementAt(1);
            this.lblVersion.Text = this.lblVersion.Text + " " + _datos.ElementAt(2);
            this.lblVerCondusat.Text = "SIIAB-CONDUSAT " + _datos.ElementAt(3);
        }
        else
        {
            this.lblMtosCan.Text = "";
            this.lblCvleAutobus.Text = "";
            this.lblVersion.Text = "";
            this.lblVerCondusat.Text = "";
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
                this.BackgroundImage = Resources.EncuadreINBOXNoc;

                //Textos
                this.lblTitulo.ForeColor = Color.Gray;

                //Botones
                this.imgCerrar.BackgroundImage = Resources.BotonCERRARNoc;

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
                this.lblTitulo.ForeColor = Color.White;

                //Botones
                this.imgCerrar.BackgroundImage = Resources.BotonCERRAR;

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
    /// Se encarga de verificar si hay horario Nocturo
    /// </summary>
    private void ChecarHorario()
    {

    }


    /// <summary>
    /// Se encarga de detener los procesos internos
    /// del form
    /// </summary>
    private void Detener()
    {
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmMovtosCAN_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 125;
        int y = punto.Y + 128;

        this.Location = new Point(x, y);

        UltActividad = DateTime.Now;
    }

    private void frmMovtosCAN_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }
    #endregion

    #region "Botones"
    private void imgCerrar_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
    }
    #endregion

    #region "Timers"

    #endregion

    
}