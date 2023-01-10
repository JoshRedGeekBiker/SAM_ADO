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
/// Powered ByRED 17MAR2021
/// </summary>
public partial class frmPanelMensajes : Form
{
    #region Contructores
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmPanelMensajes()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    public frmPanelMensajes(bool _ModoPrueba, bool _Nocturno)
    {
        InitializeComponent();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(621, 357);
            this.MinimumSize = new Size(621, 357);
            this.Size = new Size(621, 357);
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

    #region "Variables"
    private DateTime UltActividad;

    private int numpantallas = 0;
    private int opcionseleccionada = 0;
    private int indexPant = 0;
    private string PantallaActual = "";

    private List<Pantallas> PantMsjPred = new List<Pantallas>();
    private List<Pantallas> PantMsjRec = new List<Pantallas>();
    private List<Pantallas> PantMsjEnv = new List<Pantallas>();
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

    //Para mandar un Pop Up de error
    public delegate void MandaError(string mensaje);
    public event MandaError Error;

    /// <summary>
    /// Manda a pedir los mensajes de SIA según su tipo
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="_tipo"></param>
    /// <returns></returns>
    public delegate List<string> ObtenerMensajesSIA(int _tipo);
    public event ObtenerMensajesSIA MensajeSIA;

    /// <summary>
    /// Se encarga de mandar un mensaje
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="msj"></param>
    /// <returns></returns>
    public delegate bool MandarMensajeSIA(string msj);
    public event MandarMensajeSIA EnviarSIA;

    /// <summary>
    /// se encarga de mostrar el texto en grande
    /// Powered ByRED 19MAR2021
    /// </summary>
    /// <param name="texto"></param>
    public delegate void MostrarTexto(string texto);
    public event MostrarTexto TextoSMS;

    /// <summary>
    /// Se ocupa para mandar mostar una pantalla de confirmación de mensaje
    /// enviado
    /// Powered ByRED 19MAR2021
    /// </summary>
    public delegate void MensajeEnviado();
    public event MensajeEnviado Enviado;

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
                this.BackgroundImage = Resources.encuadremensajesAzulNoc;

                //Textos
                lblTitulo.ForeColor = Color.Gray;

                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRAMENSAJESNoc;
                btnEnviar.BackgroundImage = Resources.BotonENVIARpekeNoc;
                btnEnviados.BackgroundImage = Resources.BotonENVIADOSNoc;
                btnRecibidos.BackgroundImage = Resources.BotonRECIBIDOSpekeNoc;
                btnSubir.BackgroundImage = Resources.FlechaARRIBANoc;
                btnBajar.BackgroundImage = Resources.FlechaABAJONoc;
                btnLeer.BackgroundImage = Resources.botonLEERNoc;
                btnNuevoEnvio.BackgroundImage = Resources.BotonNUEVOenvioNoc;
                btnEnviados_2.BackgroundImage = Resources.BotonENVIADOSNoc;

                Flecha1.BackgroundImage = Resources.FlechaMENSAJESNoc;
                Flecha2.BackgroundImage = Resources.FlechaMENSAJESNoc;
                Flecha3.BackgroundImage = Resources.FlechaMENSAJESNoc;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {

                //Background
                this.BackgroundImage = Resources.encuadremensajesAzul;

                //Textos
                lblTitulo.ForeColor = Color.White;


                //Botones
                btnCerrar.BackgroundImage = Resources.BotonCERRAMENSAJES;
                btnEnviar.BackgroundImage = Resources.BotonENVIARpeke;
                btnEnviados.BackgroundImage = Resources.BotonENVIADOS;
                btnRecibidos.BackgroundImage = Resources.BotonRECIBIDOSpeke;
                btnSubir.BackgroundImage = Resources.FlechaARRIBA;
                btnBajar.BackgroundImage = Resources.FlechaABAJO;
                btnLeer.BackgroundImage = Resources.botonLEER;
                btnNuevoEnvio.BackgroundImage = Resources.BotonNUEVOenvio;
                btnEnviados_2.BackgroundImage = Resources.BotonENVIADOS;

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
    /// Se encarga de generar las vistas
    /// Powered ByRED 18MAR2021
    /// </summary>
    public void Crearlayout(string _tipoPant, int tipo)
    {
        PantallaActual = _tipoPant;

        //ocultamos los botones
        BlanquearRecursos();

        //habilitamos los recursos visuales necesarios
        RecursosPantalla();

        //Obtenemos los mensajes por mostrar
        var items = MensajeSIA(tipo);
        //Validamos cuantas pantallas necesitaremos
        int numitems = items.Count;

        if (numitems > 0)
        {
            double division = ((double)numitems / (double)3);

            var entero = Convert.ToInt32(Math.Truncate(division));

            if (division > entero)
            {//Significa que tenemos más elementos que mostrar
                numpantallas = entero + 1;
            }
            else
            {//Si no, nos quedamos con la cantidad exacta
                numpantallas = entero;
            }


            //Llenamos las pantallas correspondientes
            int item = 0;
            //PantMsjPred.Clear(); //Borramos las pantalla
            var newPant = SeleccionadordePantalla(PantallaActual);

            newPant.Clear();

            for (int numpantalla = 1; numpantalla <= numpantallas; numpantalla++)
            {

                Pantallas nuevaPantalla = new Pantallas();

                nuevaPantalla.numPantalla = numpantalla;
                nuevaPantalla.tipoPantalla = tipo;
                nuevaPantalla.Texto1 = items.ElementAt(item);

                item++;
                if (item > (numitems - 1))
                {
                    nuevaPantalla.Texto2 = "";
                }
                else
                {
                    nuevaPantalla.Texto2 = items.ElementAt(item);
                    item++;
                }
                
                
                if (item > (numitems - 1))
                {
                    nuevaPantalla.Texto3 = "";
                }
                else
                {
                    nuevaPantalla.Texto3 = items.ElementAt(item);
                    item++;
                }
                
                newPant.Add(nuevaPantalla);
            }

            //Si tenemos más pantallas las ponemos
            if(newPant.Count > 1)
            {
                HabilitarAbajo(true);
            }

            //Pintamos la primer pantalla

            GraficarPantalla(newPant, indexPant);
            //var primerpantalla = PantMsjPred.ElementAt(indexPant);

            //if (!primerpantalla.Texto1.Equals(""))
            //{
            //    lblMensaje1.Text = primerpantalla.Texto1;
            //    lblMensaje1.Visible = true;
            //    lblMensaje1.Enabled = true;
            //}

            //if (!primerpantalla.Texto2.Equals(""))
            //{
            //    lblMensaje2.Text = primerpantalla.Texto2;
            //    lblMensaje2.Visible = true;
            //    lblMensaje2.Enabled = true;
            //}

            //if (!primerpantalla.Texto3.Equals(""))
            //{
            //    lblMensaje3.Text = primerpantalla.Texto3;
            //    lblMensaje3.Visible = true;
            //    lblMensaje3.Enabled = true;
            //}

        }

    }

    /// <summary>
    /// se encarga de pintar una nueva pantalla hacia arriba
    /// </summary>
    /// <param name="tipo"></param>
    private void PintarNuevaPantallaArriba(string tipo)
    {
        //Reiniciamos los recursos
        BlanquearRecursos();

        //bajamos de index la página
        indexPant--;

        var datospantalla = SeleccionadordePantalla(tipo);
        //graficamos con los nuevos datos
        GraficarPantalla(datospantalla, indexPant);

        //Activamos el botón bajar porque veniamos de una página abajo
        HabilitarAbajo(true);

        if(!(indexPant <= 0))
        {
            HabilitarArriba(true);
        }
    }

    /// <summary>
    /// Se encarga de pintar una nueva pantalla hacia abajo
    /// </summary>
    /// <param name="tipo"></param>
    private void PintarNuevaPantallaAbajo(string tipo)
    {
        //Reiniciamos los recursos
        BlanquearRecursos();

        //subimos de index la página
        indexPant++;

        var datospantalla = SeleccionadordePantalla(tipo);
        //graficamos con los nuevos datos
        GraficarPantalla(datospantalla, indexPant);

        //Activamos el botón subir porque venimos de una página arriba
        HabilitarArriba(true);

        if(datospantalla.Count > (indexPant + 1))
        {//significa que tenemos más pantallas
            HabilitarAbajo(true);
        }
    }

    /// <summary>
    /// Se encarga de graficar la pantalla
    /// </summary>
    /// <param name="DatosPantalla"></param>
    /// <param name="_index"></param>
    private void GraficarPantalla(List<Pantallas> DatosPantalla, int _index)
    {
        var pantalla = DatosPantalla.ElementAt(_index);

        if (!pantalla.Texto1.Equals(""))
        {
            lblMensaje1.Text = pantalla.Texto1;
            lblMensaje1.Visible = true;
            lblMensaje1.Enabled = true;
        }

        if (!pantalla.Texto2.Equals(""))
        {
            lblMensaje2.Text = pantalla.Texto2;
            lblMensaje2.Visible = true;
            lblMensaje2.Enabled = true;
        }

        if (!pantalla.Texto3.Equals(""))
        {
            lblMensaje3.Text = pantalla.Texto3;
            lblMensaje3.Visible = true;
            lblMensaje3.Enabled = true;
        }
    }

    /// <summary>
    /// se encarga de habilitar la flecha hacia arriba
    /// </summary>
    /// <param name="activar"></param>
    private void HabilitarArriba(bool activar)
    {
        btnSubir.Enabled = activar;
        btnSubir.Visible = activar;

        if (activar)
        {
            if (ModoNocturno)
            {
                btnSubir.BackgroundImage = Resources.FlechaARRIBANoc;
            }
            else
            {
                btnSubir.BackgroundImage = Resources.FlechaARRIBA;
            }
        }
        else
        {
            btnSubir.BackgroundImage = null;
        }
    }

    /// <summary>
    /// se encarga de habilitar la flecha hacia abajo
    /// </summary>
    /// <param name="activar"></param>
    private void HabilitarAbajo(bool activar)
    {
        btnBajar.Enabled = activar;
        btnBajar.Visible = activar;

        if (activar)
        {
            if (ModoNocturno)
            {
                btnBajar.BackgroundImage = Resources.FlechaABAJONoc;
            }
            else
            {
                btnBajar.BackgroundImage = Resources.FlechaABAJO;
            }
        }
        else
        {
            btnBajar.BackgroundImage = null;
        }
    }

    /// <summary>
    /// se encarga de reinicar los valores para el pintado de una pantalla
    /// </summary>
    private void BlanquearRecursos()
    {
        Flecha1.Visible = false;
        lblMensaje1.Visible = false;
        lblMensaje1.Text = "";
        lblMensaje1.Enabled = false;

        Flecha2.Visible = false;
        lblMensaje2.Visible = false;
        lblMensaje2.Text = "";
        lblMensaje2.Enabled = false;

        Flecha3.Visible = false;
        lblMensaje3.Visible = false;
        lblMensaje3.Text = "";
        lblMensaje3.Enabled = false;

        HabilitarArriba(false);
        HabilitarAbajo(false);

    }

    /// <summary>
    /// Va a deseleccionar los items
    /// Powered ByRED 18MAR2021
    /// </summary>
    private void Deseleccionaritems()
    {
        opcionseleccionada = 0;
        Flecha1.Visible = false;
        Flecha2.Visible = false;
        Flecha3.Visible = false;
    }

    /// <summary>
    /// se encarga de regresar la lista según
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="tipo"></param>
    /// <returns></returns>
    private List<Pantallas> SeleccionadordePantalla(string tipo)
    {
        switch (tipo)
        {
            case "Recibidos":
                return PantMsjRec;

            case "Enviados":
                return PantMsjEnv;

            default:
                return PantMsjPred;

                //Predeterminados
        }
    }


    /// <summary>
    /// se encarga de recuperar el texto seleccionado
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    private string recuperarTexto()
    {
        string retorno = "";

        switch (opcionseleccionada)
        {
            case 1:
                retorno = lblMensaje1.Text;
                break;

            case 2:
                retorno = lblMensaje2.Text;
                break;

            default:
                retorno = lblMensaje3.Text;
                break;
        }

        return retorno;
    }

    /// <summary>
    /// Se encarga de mostrar/ocultar los botones, según la vista
    /// que se quiera presentar
    /// Powered ByRED19MAR2021
    /// </summary>
    /// <param name="tipo"></param>
    private void RecursosPantalla()
    {

        switch (PantallaActual)
        {
            case "Recibidos":

                btnLeer.Visible = true;
                btnNuevoEnvio.Visible = true;
                btnEnviados_2.Visible = true;

                btnEnviar.Visible = false;
                btnEnviados.Visible = false;
                btnRecibidos.Visible = false;

                lblTitulo.Text = "Bandeja de entrada";

                lblMensaje1.Font = new Font("verdana", 12.0f, FontStyle.Bold);
                lblMensaje2.Font = new Font("verdana", 12.0f, FontStyle.Bold);
                lblMensaje3.Font = new Font("verdana", 12.0f, FontStyle.Bold);

                break;

            case "Enviados":
                btnLeer.Visible = true;
                btnNuevoEnvio.Visible = true;
                btnEnviados_2.Visible = false;

                btnEnviar.Visible = false;
                btnEnviados.Visible = false;
                btnRecibidos.Visible = true;

                lblTitulo.Text = "Bandeja de salida";

                lblMensaje1.Font = new Font("Verdana", 12.0f, FontStyle.Bold);
                lblMensaje2.Font = new Font("Verdana", 12.0f, FontStyle.Bold);
                lblMensaje3.Font = new Font("Verdana", 12.0f, FontStyle.Bold);

                break;

            //Predeterminados
            default:
                btnLeer.Visible = false;
                btnNuevoEnvio.Visible = false;
                btnEnviados_2.Visible = false;

                btnEnviar.Visible = true;
                btnEnviados.Visible = true;
                btnRecibidos.Visible = true;

                lblTitulo.Text = "Mensajes para enviar";

                lblMensaje1.Font = new Font("Verdana", 16.0f, FontStyle.Bold);
                lblMensaje2.Font = new Font("Verdana", 16.0f, FontStyle.Bold);
                lblMensaje3.Font = new Font("Verdana", 16.0f, FontStyle.Bold);

                break;
        }
    }



    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmPanelMensajes_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 97;
        int y = punto.Y + 121;

        this.Location = new Point(x, y);

        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Evitar cierre a menos que sea por apagado del sistema
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmPanelMensajes_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    
    #endregion

    #region Botones
    private void btnCerrar_Click(object sender, EventArgs e)
    {
        Cerrar(this);
    }

    private void btnSubir_Click(object sender, EventArgs e)
    {
        PintarNuevaPantallaArriba(PantallaActual);
    }

    private void btnBajar_Click(object sender, EventArgs e)
    {
        PintarNuevaPantallaAbajo(PantallaActual);
    }

    private void btnEnviados_Click(object sender, EventArgs e)
    {
        indexPant = 0;
        Crearlayout("Enviados", 2);
    }

    private void btnRecibidos_Click(object sender, EventArgs e)
    {
        indexPant = 0;
        Crearlayout("Recibidos", 1);
    }


    private void btnLeer_Click(object sender, EventArgs e)
    {
        if (opcionseleccionada != 0)
        {
            TextoSMS(recuperarTexto());

            Deseleccionaritems();
        }
        else
        {
            Error("Seleccione un mensaje para enviar");
        }
    }

    private void btnNuevoEnvio_Click(object sender, EventArgs e)
    {
        indexPant = 0;
        Crearlayout("Predeterminados", 0);
    }

    private void btnEnviados_2_Click(object sender, EventArgs e)
    {
        indexPant = 0;
        Crearlayout("Enviados", 2);
    }

    /// <summary>
    /// Boton Leer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnEnviar_Click(object sender, EventArgs e)
    {
        if(opcionseleccionada != 0)
        {
            if(EnviarSIA(recuperarTexto()))
            {
                Deseleccionaritems();
                //Mandamos la pantalla de éxito total rotundo
                Enviado();
            }
            else
            {
                Error("Intente enviar el mensaje de nuevo");
            }
        }
        else
        {
            Error("Seleccione un mensaje para enviar");
        }
    }
    #endregion

    #region Timers

    #endregion

    #region Acciones
    private void lblMensaje1_Click(object sender, EventArgs e)
    {
        Flecha1.Visible = true;
        Flecha2.Visible = false;
        Flecha3.Visible = false;

        opcionseleccionada = 1;
    }

    private void lblMensaje2_Click(object sender, EventArgs e)
    {
        Flecha1.Visible = false;
        Flecha2.Visible = true;
        Flecha3.Visible = false;

        opcionseleccionada = 2;
    }

    private void lblMensaje3_Click(object sender, EventArgs e)
    {
        Flecha1.Visible = false;
        Flecha2.Visible = false;
        Flecha3.Visible = true;

        opcionseleccionada = 3;
    }
    #endregion

    public class Pantallas
    {
        public int numPantalla { get; set; }
        public int tipoPantalla { get; set; }
        public string Texto1 { get; set; }
        public string Texto2 { get; set; }
        public string Texto3 { get; set; }
    }

    
}