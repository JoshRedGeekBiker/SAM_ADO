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

public partial class frmSpots : Form
{

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmSpots()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="Nocturno"></param>
    public frmSpots(bool _ModoPrueba, bool Nocturno, List<string> Items, int _tipo)
    {
        Tipo = _tipo;
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        CrearLayout(Items);

        lblFecha.Text = DateTime.Now.ToString();

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
    #endregion
    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    private int Tipo { get; set; }
    private string PautaSeleccionada { get; set; } = "";
    private string USBSeleccionado { get; set; } = "";
    #endregion
    #region "Variables"
    private int stepUSB = 1;
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

    /// <summary>
    /// Sirve para mandar el nombre de la pauta hacia las utilidades de VMD
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    public delegate Task<bool> PlancharPauta(string tipo, string nombrepauta);
    public event PlancharPauta Pauta;

    /// <summary>
    /// Se encarga de validar la pauta
    /// Powered ByRED 16JUL2020
    /// </summary>
    /// <param name="tipo"></param>
    /// <param name="nombrepauta"></param>
    /// <returns></returns>
    public delegate Task<bool> ValidarPauta(string tipo, string nombrepauta);
    public event ValidarPauta ValidaPauta;


    //Para mostrar algun Error
    public delegate void MandarError(string mensaje);
    public event MandarError MandaError;

    /// <summary>
    /// Para mandar a pedir el progreso del copiado
    /// </summary>
    public delegate int PedirProgresoCopiado();
    public event PedirProgresoCopiado ProgresoCopiado;


    /// <summary>
    /// Se encarga de mostrar un PopUp
    /// </summary>
    /// <param name="tipo"></param>
    public delegate void MostrarPopUp(string tipo);
    public event MostrarPopUp PopUp;

    //Para iniciar la reproduccion del video
    public delegate void IniciaRepro(int Tipo);
    public event IniciaRepro Repro;
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

                listView1.BackColor = Color.Gray;

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

                listView1.BackColor = Color.White;

                //Botones
                btnRegresar.BackgroundImage = Resources.BotonREGRESAR;

                ModoNocturno = false;
            }
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
    private void CargadorDePautas_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        this.TopMost = true;
    }

    /// <summary>
    /// Se encarga de crear el espacio de trabajo
    /// </summary>
    /// <param name="Items"></param>
    private void CrearLayout(List<string> Items)
    {

        listView1.Clear();

        PrepararPrimerInicio();

        var index = 0;
        lblPauta.Text = "Spots disponibles: ";
        //if (Tipo.Equals("USB"))
        //{

        //    if (stepUSB == 1)
        //    {
        //        lblPauta.Text = "Medios Extraibles:";
        //        index = 1;
        //    }
        //    else
        //    {
        //        lblPauta.Text = "Pautas en USB";
        //    }

        //}
        //else
        //{
        //    lblPauta.Text = "Pautas en Disco:";
        //}

        foreach (string item in Items)
        {
            ListViewItem nuevoitem = new ListViewItem(item, Tipo);
            nuevoitem.Font = new Font(new FontFamily("Microsoft Sans Serif"), 12.0f, FontStyle.Bold);
            listView1.Items.Add(nuevoitem);
        }
    }


    /// <summary>
    /// Detecta la selección de un item en el listview
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listView1.SelectedItems.Count > 0)
        {
            ListViewItem item = listView1.SelectedItems[0];
            SeleccionDePauta(item.Text);

        }
    }

    /// <summary>
    /// se encarga de preparar el primer inicio del form
    /// </summary>
    private void PrepararPrimerInicio()
    {

        listView1.SelectedItems.Clear();

        switch (this.Tipo)
        {
            case 0:

                if (stepUSB == 1)
                {
                    this.lblTitPauta.Text = "Seleccione un Disco Extraíble";
                }
                else
                {
                    this.lblTitPauta.Text = "Seleccione una Pauta";
                }
                break;

            default:
                this.lblTitPauta.Text = "Seleccione una Pauta";
                break;
        }

        this.lblNomPauta.Text = "";
        this.lblNomPauta.Visible = false;

        this.lblTitPauta1.Visible = false;

        this.btnAceptar.Visible = false;
        this.BtnCancelar.Visible = false;

        PautaSeleccionada = "";
    }

    /// <summary>
    /// Se encarga de ajustar la pantalla para una pauta seleccionada
    /// </summary>
    /// <param name="_nombrePauta"></param>
    private void SeleccionDePauta(string _nombrePauta)
    {
        PautaSeleccionada = _nombrePauta;

        this.lblTitPauta.Text = "Spot Seleccionado:";

        this.lblNomPauta.Text = PautaSeleccionada;
        this.lblNomPauta.Visible = true;

        this.lblTitPauta1.Text = "¿Desea ingresar este spot?";
        this.lblTitPauta1.Visible = true;

        this.btnAceptar.Visible = true;
        this.BtnCancelar.Visible = true;
    }

    private void frmCargadorDePautas_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }
    #endregion
    #region "Botones"
    private void btnRegresar_Click(object sender, EventArgs e)
    {
        Detener();
        Cerrar(this);
    }
    private void btnAceptar_Click(object sender, EventArgs e)
    {
        PrepararPrimerInicio();
        Repro(Tipo);
    }
    #endregion
    #region "Timers"
    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }


    //private void tmrProgreso_Tick(object sender, EventArgs e)
    //{
    //    tmrProgreso.Stop();

    //    progressBar1.Visible = true;
    //    this.lblCopia.Visible = true;

    //    if (lblCopia.ForeColor == Color.Red)
    //    {
    //        lblCopia.ForeColor = Color.Yellow;
    //    }
    //    else if (lblCopia.ForeColor == Color.Yellow)
    //    {
    //        lblCopia.ForeColor = Color.Red;
    //    }

    //    var progreso = ProgresoCopiado();

    //    if (progreso <= 100)
    //    {
    //        progressBar1.Value = progreso;
    //        tmrProgreso.Start();
    //    }

    //}

    #endregion

 
}

