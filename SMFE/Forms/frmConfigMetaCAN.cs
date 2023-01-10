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

public partial class frmMetaCAN : Form
{
    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmMetaCAN()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    /// <param name="Nocturno"></param>
    public frmMetaCAN(bool _ModoPrueba, bool Nocturno)
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

        //CrearLayout(Items);

        lblFecha.Text = DateTime.Now.ToString();

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    public string MetaSeleccionada { get; set; } = "";
    public bool Cargado { get; set; } = false;
    #endregion

    #region "Variables"

    #endregion

    #region "Eventos"

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;


    /// <summary>
    /// Para mostrar algun Error
    /// </summary>
    /// <param name="mensaje"></param>
    public delegate void MandarError(string mensaje);
    public event MandarError MandaError;

    /// <summary>
    /// Manda a mostrar la siguiente pantalla de configuracion
    /// </summary>
    /// <param name="autobus"></param>
    /// <param name="pantallaOrigen"></param>
    public delegate void SiguienteConfig(int pantallaOrigen);
    public event SiguienteConfig Config;

    //Muestra la pantalla anterior
    public delegate void MostrarPantBus(int NumeroPantalla);
    public event MostrarPantBus PantallaAnterior;

    /// <summary>
    /// Se encarga mandar la instrucción de apagado del sistema SAM
    /// por un apagado de equipo
    /// Powered ByRED 10DIC2020
    /// </summary>
    public delegate void ApagarEquipoPorSistema();
    public event ApagarEquipoPorSistema ApagarPorSistema;
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

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CargadorDePautas_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        this.TopMost = true;
    }

    /// <summary>
    /// Se encarga de crear el espacio de trabajo
    /// </summary>
    /// <param name="Items"></param>
    public void CrearLayout(List<string> Items)
    {

        listView1.Clear();

        PrepararPrimerInicio();

        var index = 0;

        foreach (string item in Items)
        {
            ListViewItem nuevoitem = new ListViewItem(item, index);
            nuevoitem.Font = new Font(new FontFamily("Microsoft Sans Serif"),10.0f, FontStyle.Bold);
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
        if(listView1.SelectedItems.Count > 0)
        {
            ListViewItem item = listView1.SelectedItems[0];
            SeleccionDeMeta(item.Text);
        }
    }

    /// <summary>
    /// se encarga de preparar el primer inicio del form
    /// </summary>
    private void PrepararPrimerInicio()
    {

        listView1.SelectedItems.Clear();
      
        this.lblNomMeta.Text = "";
        this.lblNomMeta.Visible = false;

        MetaSeleccionada = "";
    }

    /// <summary>
    /// Se encarga de ajustar la pantalla para una pauta seleccionada
    /// </summary>
    /// <param name="_nombrePauta"></param>
    private void SeleccionDeMeta(string _nombreMeta)
    {
        MetaSeleccionada = _nombreMeta;

        this.lblNomMeta.Text = MetaSeleccionada;
        this.lblNomMeta.Visible = true;
    }

    private void frmMetaCAN_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Preguntamos si la aplicación está siendo cerrada por el sistema
        if (e.CloseReason == CloseReason.WindowsShutDown)
        {
            //Mandamos el comando para cerrar SAM
            ApagarPorSistema();
        }
        else
        {
            //Cancelamos el cierre
            e.Cancel = true;
        }
    }
    #endregion

    #region "Botones"

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        PantallaAnterior(2);
    }

    private void btnAceptar_Click(object sender, EventArgs e)
    {
        if (!MetaSeleccionada.Equals(""))
        {
            Config(2);
        }
        else
        {
            MandaError("Seleccione una meta");
        }
        
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
