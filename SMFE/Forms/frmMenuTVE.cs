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


public partial class frmMenuTVE : Form
{ 
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmMenuTVE()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmMenuTVE(bool _ModoPrueba, bool Nocturno, bool _btnoff)
    {
        InitializeComponent();
        this.lblFecha.Text = DateTime.Now.ToString();
        this.btnOff.Visible = _btnoff;

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
    //Para mandar a abrir la vista de Transferencia
    public delegate void AbrirTransferencia();
    public event AbrirTransferencia Transfer;

    //Para mandar a abrir la vista de consulta
    public delegate void AbrirConsulta();
    public event AbrirConsulta Consulta;

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

    // Se encarga de terminar con la logica de TVE
    public delegate void TerminarTVE();
    public event TerminarTVE FinTVE;
    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    #endregion

    #region "Variables"
    private DateTime UltActividad;
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
                this.lblFecha.ForeColor = Color.Gray;
                this.lblTitulo.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnOff.BackgroundImage = Resources.btn_offNoc;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;
                this.btnTranfer.BackgroundImage = Resources.btl_transparenciaNoc;
                this.btnConsulta.BackgroundImage = Resources.btl_consultaNoc;
                

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
                this.btnOff.BackgroundImage = Resources.btn_off;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESAR;
                this.btnTranfer.BackgroundImage = Resources.btl_transparencia;
                this.btnConsulta.BackgroundImage = Resources.btl_consulta;

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
    private void frmMenuTVE_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }

    #endregion

    #region "Botones"

    private void btnTranfer_Click(object sender, EventArgs e)
    {
        Transfer();
        UltActividad = DateTime.Now;
    }

    private void btnConsulta_Click(object sender, EventArgs e)
    {
        Consulta();
        UltActividad = DateTime.Now;
    }

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        Detener();
        FinTVE();
        Cerrar(this);
    }

    private void btnOff_Click(object sender, EventArgs e)
    {
        MuestraSalir(0);
        UltActividad = DateTime.Now;
    }

    private void imgADO_Click(object sender, EventArgs e)
    {
        MuestraSalir(1);
        UltActividad = DateTime.Now;
    }

    private void frmMenuTVE_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
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
