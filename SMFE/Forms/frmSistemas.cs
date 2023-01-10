using SMFE.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class frmSistemas : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmSistemas()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmSistemas(bool _ModoPrueba, bool Nocturno, bool _CAN, bool _VMD, bool _CONDUSAT, bool _SIA, bool _TELEMETRIA, bool _GPS,bool _btnOff, bool _btnPanico)
    {
        InitializeComponent();
        //ModoPrueba = _ModoPrueba;

        //Llenamos las posiciones donde se pintarán los botones
        posicionesBotones = new List<int>();
        posicionesBotones.Add(104);
        posicionesBotones.Add(200);
        posicionesBotones.Add(296);
        posicionesBotones.Add(392);

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        this.CAN = _CAN;
        this.VMD = _VMD;
        this.CONDUSAT = _CONDUSAT;
        this.SIA = _SIA;
        this.Telemetria = _TELEMETRIA;
        this.GPS = _GPS;

        //Acomodar vistas
        CrearLayout();


        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }

        //Asignamos valores por default
        lblVReal.Text = "0";
        lblVMax.Text = "100";
        lblRealFR.Text = "0.00";
        lblMetaFR.Text = "3.00";

        btnOff.Visible = _btnOff;

        //Powered ByRED 08JUN2021
        if (this.SIA)
        {//Ya que es una función de SIA
            btn_Panico.Visible = _btnPanico;
        }
        else
        {//Powered ByRED 15JUN2021
            btn_Panico.Visible = false;
        }
    }
    #endregion

    #region "Eventos"

    public delegate void VistaMenu();
    public event VistaMenu MuestraMenu;

    //Para mostrar algun Error
    public delegate void MandarError(string mensaje);
    public event MandarError MandaError;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Manda a bloquear la pantalla
    //(Mostrar la pantalla de Login)
    public delegate void BloquearPantalla();
    public event BloquearPantalla LockPant;

    //Avisa a Front que la carga del sistema se concluyo
    public delegate void TerminarCarga();
    public event TerminarCarga Carga;

    //Para iniciar la lógica de VMD
    public delegate void IniciaRepro();
    public event IniciaRepro Repro;

    public delegate void VMDReiniciar();
    public event VMDReiniciar EReiniciar;

    public delegate void VMDReiniciarPauta(bool DetenerVideo);
    public event VMDReiniciarPauta EReiniciarPauta;

    public delegate int VMDControlVolumen(SMFE.Forms.frmVolumen.TipoDeVolumen Tipo);
    public event VMDControlVolumen EControlVolumen;

    public delegate void VMDDetenerPelicula();
    public event VMDDetenerPelicula EDetenerPelicula;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

    /// <summary>
    /// Se encarga de mandar a mostrar el menu de herramientas
    /// Powered ByRED 16JUN2020
    /// </summary>
    public delegate void VistaHerramientasVMD();
    public event VistaHerramientasVMD MuestraHerramientasVMD;

    /// <summary>
    /// Se encarga mandar la instrucción de apagado del sistema SAM
    /// por un apagado de equipo
    /// Powered ByRED 10DIC2020
    /// </summary>
    public delegate void ApagarEquipoPorSistema();
    public event ApagarEquipoPorSistema ApagarPorSistema;

    /// <summary>
    /// Se encarga de mandar a mostrar la vista de los mensajes de SIA
    /// Powered ByRED 17MAR2021
    /// </summary>
    public delegate void VistaMensajesSIA();
    public event VistaMensajesSIA MuestraMensajesSIA;

    /// <summary>
    /// Se encarga de actualizar el led de gps en caso de cambio
    /// por Horario Nocturno
    /// Powered ByRED 27MAY2021
    /// </summary>
    public delegate void ActualizaLedGPS();
    public event ActualizaLedGPS LedGPSView;

    /// <summary>
    /// Se encarga de detonar el evento para enviar mensaje de Robo, a través de SIA
    /// </summary>
    public delegate void EjecutaRobo();
    public event EjecutaRobo Robo;

    #endregion

    #region "Propiedades"

    public string NumAutobus { get; set; }
    public string Version { get; set; }
    //public bool ModoPrueba { get; set; }
    public  bool ModoNocturno { get; set; } = false;
    public bool CAN { get; set; }
    public bool VMD { get; set; }
    public bool CONDUSAT { get; set; }
    public bool SIA { get; set; }
    public bool VMD_Inicializado { get; set; } = false;
    public bool Telemetria { get; set; } = false;
    public bool GPS { get; set; } = false;

    #endregion

    #region "Variables"

    private int BotonChicoX = 180;
    private int BotonChicoY = 83;
    private int BotonGrandeX = 191;
    private int BotonGrandeY = 85;

    private List<int> posicionesBotones;

    private string PrioridadSistema = string.Empty;
    private string formActual = string.Empty;
    private DateTime UltActividad;

    //Powered ByRED 08JUN2021
    private bool EnRobo = false;
    private int contadorRobo = 0;

    #endregion

    #region "Eventos Vista CAN"
    private void btnOpciones_Click(object sender, EventArgs e)
    {
        //LanzarVistaMenú
        MuestraMenu();
    }

    #endregion

    #region "Eventos Vista CONDUSAT"

    #endregion

    #region  "Eventos Vista VMD"
    public bool ModoOcupado;

    private void btnReiniciar_Click(object sender, EventArgs e)
    {
        EReiniciar();
    }

    /// <summary>
    /// Tarjeta de Circulación vencida
    /// </summary>
    /// <param name="NombreVideo"></param>
    /// <param name="TiempoVideo"></param>
    public void InfoVideo(string NombreVideo, TimeSpan TiempoVideo)
    {
        var Tiempo = TiempoVideo.ToString(@"hh\:mm\:ss");
        lblDuracion.Text = Tiempo + " Duración total" ;
        lblTituloPelicula.Text = NombreVideo;

    }

    /// <summary>
    /// Tarjeta de Circulación Vencida
    /// </summary>
    /// <param name="Tiempo"></param>
    public void PosicionVideo(TimeSpan Tiempo)
    {
        lblREproduccion.Text = Tiempo.ToString(@"hh\:mm\:ss") + " Tiempo de reproducción";
    }

    /// <summary>
    /// Se encarga de cambiar el icono del estado de la reproducción
    /// Powered ByRED2020
    /// </summary>
    /// <param name="estado"></param>
    public void Func_EstablecerEstadoReproductor(string estado)
    {
        switch (estado.ToUpper())
        {
            case "PLAY":
                if (ModoNocturno) this.imgStatusPelicula.BackgroundImage = global::SMFE.Properties.Resources.letavanzando;
                else this.imgStatusPelicula.BackgroundImage = global::SMFE.Properties.Resources.Avanzando;
                break;
            default:
                if (ModoNocturno) this.imgStatusPelicula.BackgroundImage = global::SMFE.Properties.Resources.letdetenida;
                else this.imgStatusPelicula.BackgroundImage = global::SMFE.Properties.Resources.Detenida;
                break;
        }
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        Repro();
        //EBuscaPauta(); ESTA NECESITA REINICIAR, POR ENDE SE TOMA MEJOR INICIALIZA VMD
        UltActividad = DateTime.Now;
    }

    private void btnNuevaPauta_Click(object sender, EventArgs e)
    {
        EReiniciarPauta(false);
    }

    private void btnVolumeUp_Click(object sender, EventArgs e)
    {
        var Tipo = SMFE.Forms.frmVolumen.TipoDeVolumen.Mas;
        var Result = EControlVolumen(Tipo);
        if (Result == -1) return;
        var Mensaje = new SMFE.Forms.frmVolumen(SMFE.Forms.frmVolumen.TipoDeVolumen.Mas, Result, ModoNocturno);
        Mensaje.ShowDialog();

        UltActividad = DateTime.Now;
    }

    private void btnVolumeDown_Click(object sender, EventArgs e)
    {
        var Tipo = SMFE.Forms.frmVolumen.TipoDeVolumen.Menos;
        var Result = EControlVolumen(Tipo);
        if (Result == -1) return;
        var Mensaje = new SMFE.Forms.frmVolumen(SMFE.Forms.frmVolumen.TipoDeVolumen.Menos, Result, ModoNocturno);
        Mensaje.ShowDialog();

        UltActividad = DateTime.Now;
    }

    private void btnDetener_Click(object sender, EventArgs e)
    {
        EDetenerPelicula();

        UltActividad = DateTime.Now;

        MandaError("Pelicula Detenida por el conductor");
    }

    /// <summary>
    /// Se encarga de mandar a mostrar el menu de herramientas VMD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnHerramientasVMD_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
        MuestraHerramientasVMD();
    }

    #endregion

    #region "Eventos Vista SIA"
    private void btnMensajes_Click(object sender, EventArgs e)
    {
        UltActividad = DateTime.Now;
        MuestraMensajesSIA();
    }
    #endregion

    #region "Metodos"

    /// <summary>
    /// Se encarga de verificar si ya pasó el tiempo de 
    /// actividad del form
    /// </summary>
    /// <returns></returns>
    public bool VerificaActividad(int TiempoEspera)
    {
        //Se verifica que se encuentre en un form diferente
        //porque si no, no tiene caso calcular el tiempo
        if (PrioridadSistema.Equals(formActual))
        {
            return true;
        }else
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
    }

    /// <summary>
    /// Se encarga de regresarle el foco al sistema que tiene prioridad de vista
    /// </summary>
    public void SistemaFocus()
    {
        BotonFocus(PrioridadSistema);
    }

    /// <summary>
    /// Se encarga de generar la plantilla con la que el sistema
    /// estará trabajando, de acuerdo a los subsistemas activados
    /// CAN, VMD, CONDUSAT, SIA
    /// </summary>
    private void CrearLayout()
    {
        if (CAN)
        {
            btnCAN.Enabled = true;
            btnCAN.Visible = true;
            frmCAN.Visible = true;

            btnCAN.Location = new Point(48, posicionesBotones.ElementAt(0));
            posicionesBotones.RemoveAt(0);

            PrioridadSistema = "btnCAN";

        }
        else
        {
            btnCAN.Enabled = false;
            btnCAN.Visible = false;
            frmCAN.Visible = false;
        }

        //Powered ByRED 16JUN2020
        if (Telemetria)
        {
            LedTelemetria.Visible = true;
        }
        else
        {
            LedTelemetria.Visible = false;
        }

        if (VMD)
        {
            btnVMD.Enabled = true;
            btnVMD.Visible = true;
            frmVMD.Visible = true;

            btnVMD.Location = new Point(48, posicionesBotones.ElementAt(0));
            posicionesBotones.RemoveAt(0);

            //CAN tiene prioridad sobre VMD, si no tenemos CAN activo
            //Dejamos VMD
            if (!CAN)
            {
                PrioridadSistema = "btnVMD";
            }
        }
        else
        {
            btnVMD.Enabled = false;
            btnVMD.Visible = false;
            frmVMD.Visible = false;
        }

        if (CONDUSAT)
        {
            btnCONDUSAT.Enabled = true;
            btnCONDUSAT.Visible = true;
            frmCondusat.Visible = true;

            btnCONDUSAT.Location = new Point(48, posicionesBotones.ElementAt(0));
            posicionesBotones.RemoveAt(0);
            PrioridadSistema = "btnCONDUSAT";
        }
        else
        {
            btnCONDUSAT.Enabled = false;
            btnCONDUSAT.Visible = false;
            frmCondusat.Visible = false;
        }

        if (SIA)
        {
            btnSIA.Enabled = true;
            btnSIA.Visible = true;
            //Powered ByRED 17MAR2021
            frmSIA.Visible = true;

            btnSIA.Location = new Point(48, posicionesBotones.ElementAt(0));
            posicionesBotones.RemoveAt(0);

            //Si aún no se ha llenado éste campo significa que sólo se activo SÏA
            //De lo contrario, dejamos como prioridad los 3 sistemas pasados
            if (PrioridadSistema.Equals(""))
            {
                PrioridadSistema = "btnSIA";
            }
            
        }
        else
        {
            btnSIA.Enabled = false;
            btnSIA.Visible = false;
            //Powered ByRED 17MAR2021
            frmSIA.Visible = false;
        }

        //Powered ByRED 01/JUL/2020
        if (!this.GPS)
        {
            this.ledGPS.Visible = false;
        }

        BotonFocus(PrioridadSistema);
    }

    /// <summary>
    /// Se encarga de darle el foco a la vista del sistema
    /// </summary>
    /// <param name="boton"></param>
    private void BotonFocus(string  Nombreboton)
    {
        if (CAN)
        {
            if (Nombreboton.Equals("btnCAN"))
            {
                if (ModoNocturno)
                {
                    btnCAN.BackgroundImage = Resources.cuadroGRANDENoc;
                    btnCAN.Image = Resources.LogoCANNoc;
                }
                else
                {
                    btnCAN.BackgroundImage = Resources.cuadroGRANDE;
                    btnCAN.Image = Resources.LogoCAN;
                }

                btnCAN.Size = new Size(BotonGrandeX, BotonGrandeY);

                //frmCAN.Show();
                //frmCAN.BringToFront();
                frmCAN.Visible = true;

                formActual = "btnCAN";
            }
            else
            {
                if (ModoNocturno)
                {
                    btnCAN.BackgroundImage = Resources.cuadroPEQUEÑONoc;
                    btnCAN.Image = Resources.LogoCANChicoNoc;
                }
                else
                {
                    btnCAN.BackgroundImage = Resources.cuadroPEQUEÑO;
                    btnCAN.Image = Resources.LogoCANChico;
                }
                
                btnCAN.Size = new Size(BotonChicoX, BotonChicoY);

                frmCAN.Visible = false;
                //frmCAN.Hide();
            }
        }

        if (VMD)
        {
            if (Nombreboton.Equals("btnVMD"))
            {
                if (ModoNocturno)
                {
                    btnVMD.BackgroundImage = Resources.cuadroGRANDENoc;
                    btnVMD.Image = Resources.LogoVMDNoc;
                }
                else
                {
                    btnVMD.BackgroundImage = Resources.cuadroGRANDE;
                    btnVMD.Image = Resources.LogoVMD;
                }

                btnVMD.Size = new Size(BotonGrandeX, BotonGrandeY);

                frmVMD.Visible = true;
                //frmVMD.Show();
                //frmVMD.BringToFront();

                formActual = "btnVMD";
            }
            else
            {
                if (ModoNocturno)
                {
                    btnVMD.BackgroundImage = Resources.cuadroPEQUEÑONoc;
                    btnVMD.Image = Resources.LogoVMDChicoNoc;
                }
                else
                {
                    btnVMD.BackgroundImage = Resources.cuadroPEQUEÑO;
                    btnVMD.Image = Resources.LogoVMDChico;
                }

                btnVMD.Size = new Size(BotonChicoX, BotonChicoY);

                frmVMD.Visible = false;
                //frmVMD.Hide();   
            }
        }

        if (CONDUSAT)
        {
            if (Nombreboton.Equals("btnCONDUSAT"))
            {
                if (ModoNocturno)
                {
                    btnCONDUSAT.BackgroundImage = Resources.cuadroGRANDENoc;
                    btnCONDUSAT.Image = Resources.LogoCONDUSATNoc;
                }
                else
                {
                    btnCONDUSAT.BackgroundImage = Resources.cuadroGRANDE;
                    btnCONDUSAT.Image = Resources.LogoCONDUSAT;
                }

                btnCONDUSAT.Size = new Size(BotonGrandeX, BotonGrandeY);

                frmCondusat.Visible = true;
                //frmCondusat.Show();
                //frmCondusat.BringToFront();

                formActual = "btnCONDUSAT";
            }
            else
            {
                if (ModoNocturno)
                {
                    btnCONDUSAT.BackgroundImage = Resources.cuadroPEQUEÑONoc;
                    btnCONDUSAT.Image = Resources.LogoCONDUSATChicoNoc;
                }
                else
                {
                    btnCONDUSAT.BackgroundImage = Resources.cuadroPEQUEÑO;
                    btnCONDUSAT.Image = Resources.LogoCONDUSATChico;
                }

                btnCONDUSAT.Size = new Size(BotonChicoX, BotonChicoY);

                frmCondusat.Visible = false;
                //frmCondusat.Hide();
            }
        }

        if (SIA)
        {
            if (Nombreboton.Equals("btnSIA"))
            {
                if (ModoNocturno)
                {
                    btnSIA.BackgroundImage = Resources.cuadroGRANDENoc;
                    btnSIA.Image = Resources.LogoSIANoc;
                }
                else
                {
                    btnSIA.BackgroundImage = Resources.cuadroGRANDE;
                    btnSIA.Image = Resources.LogoSIA;
                }

                btnSIA.Size = new Size(BotonGrandeX, BotonGrandeY);
                
                //Powered ByRED 17MAR2021
                frmSIA.Visible = true;

                formActual = "btnSIA";

            }
            else
            {
                if (ModoNocturno)
                {
                    btnSIA.BackgroundImage = Resources.cuadroPEQUEÑONoc;
                    btnSIA.Image = Resources.LogoSIAChicoNoc;
                }
                else
                {
                    btnSIA.BackgroundImage = Resources.cuadroPEQUEÑO;
                    btnSIA.Image = Resources.LogoSIAChico;
                }

                btnSIA.Size = new Size(BotonChicoX, BotonChicoY);

                //Powered ByRED 17MAR2021
                frmSIA.Visible = false;

            }
        }

        UltActividad = DateTime.Now;

    }

    #region ModoNocturno
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
                ModoNocturno = true;

                //Background
                this.BackgroundImage = Resources.FondoRECUADROSNoc;

                //Textos
                this.lblFecha.ForeColor = Color.Gray;
                this.lblECAN.ForeColor = Color.Gray;
                this.lblEFR.ForeColor = Color.Gray;
                this.lblEFRMeta.ForeColor = Color.Gray;
                this.lblMetaFR.ForeColor = Color.Gray;
                this.lblCvlOperador.ForeColor = Color.Gray;
                this.lblNombreOperador.ForeColor = Color.Gray;
                this.lblCvlAutobus.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnOff.BackgroundImage = Resources.btn_offNoc;
                this.btnbloquear.BackgroundImage = Resources.BotonBloquearNoc;

                //Indicadores
                ledGPS.BackColor = Color.Black;
                LedTelemetria.BackColor = Color.Black;


                BotonFocus(PrioridadSistema);

                //Cambiamos los recursos de las vistas de los sistemas
                Func_ModoNocturnoCAN(ModoNocturno);
                Func_ModoNocturnoVMD(ModoNocturno);
                Func_ModoNocturnoCONDUSAT(ModoNocturno);
                Func_ModonocturnoSIA(ModoNocturno);
            }
        }
        else
        {
            if (ModoNocturno)
            {
                ModoNocturno = false;

                //Background
                this.BackgroundImage = Resources.FondoRECUADROS;

                //Textos
                this.lblFecha.ForeColor = Color.White;
                this.lblECAN.ForeColor = Color.White;
                this.lblEFR.ForeColor = Color.White;
                this.lblEFRMeta.ForeColor = Color.White;
                this.lblMetaFR.ForeColor = Color.White;
                this.lblCvlOperador.ForeColor = Color.White;
                this.lblNombreOperador.ForeColor = Color.White;
                this.lblCvlAutobus.ForeColor = Color.White;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADO;
                this.btnOff.BackgroundImage = Resources.btn_off;
                this.btnbloquear.BackgroundImage = Resources.BotonBloquear;
                //Indicadores
                ledGPS.BackColor = Color.Transparent;
                LedTelemetria.BackColor = Color.Transparent;



                BotonFocus(PrioridadSistema);

                //Cambiamos los recursos de las vistas de los sistemas
                Func_ModoNocturnoCAN(ModoNocturno);
                Func_ModoNocturnoVMD(ModoNocturno);
                Func_ModoNocturnoCONDUSAT(ModoNocturno);
                Func_ModonocturnoSIA(ModoNocturno);
            }
        }
        //Actualizar Led de GPS
        //Powered ByRED 27MAY2021
        LedGPSView();
    }

    /// <summary>
    /// Se encarga de cambiar los recursos de la vista de CAN
    /// según sea modo nocturno o no
    /// </summary>
    /// <param name="Activar"></param>
    private void Func_ModoNocturnoCAN(bool Activar)
    {

        if (Activar)
        {
            //Background
            frmCAN.BackColor = Color.Black;

            //Textos
            lblFRMeta.ForeColor = Color.Gray;
            lblVFRMeta.ForeColor = Color.Gray;
            lblFR.ForeColor = Color.Gray;
            lblVFR.ForeColor = Color.Gray;
            lblKm.ForeColor = Color.Gray;
            lblVKm.ForeColor = Color.Gray;
            lblLitro.ForeColor = Color.Gray;
            lblVlitros.ForeColor = Color.Gray;

            //Botones
            btnOpciones.BackgroundImage = Resources.BotonOpcionesNoc;
        }
        else
        {
            //Background
            frmCAN.BackColor = Color.Transparent;

            //Textos
            lblFRMeta.ForeColor = Color.White;
            lblVFRMeta.ForeColor = Color.White;
            lblFR.ForeColor = Color.White;
            lblVFR.ForeColor = Color.White;
            lblKm.ForeColor = Color.White;
            lblVKm.ForeColor = Color.White;
            lblLitro.ForeColor = Color.White;
            lblVlitros.ForeColor = Color.White;

            //Botones
            btnOpciones.BackgroundImage = Resources.BotonOpciones;
        }
    }

    /// <summary>
    /// Se encarga de cambiar los recursos de la vista de VMD
    /// según sea modo nocturno o no
    /// </summary>
    /// <param name="Activar"></param>
    private void Func_ModoNocturnoVMD(bool Activar)
    {
        if (Activar)
        {
            frmVMD.BackColor = Color.Black;

            this.imgStatusInternet.BackgroundImage = global::SMFE.Properties.Resources.letdetenida;
            this.btnDetener.BackgroundImage = global::SMFE.Properties.Resources.BotonDetenerNoc;
            this.btnPlay.BackgroundImage = global::SMFE.Properties.Resources.BotonIniciarNoc;
            this.btnReiniciar.BackgroundImage = global::SMFE.Properties.Resources.BotonReiniciarNoc;
            this.btnNuevaPauta.BackgroundImage = global::SMFE.Properties.Resources.BotonPautaNoc;
            this.btnVolumeUp.BackgroundImage = global::SMFE.Properties.Resources.BotonVolumenMASNoc;
            this.btnVolumeDown.BackgroundImage = global::SMFE.Properties.Resources.BotonVolumenMENOSNoc;
            this.btnHerramientasVMD.BackgroundImage = global::SMFE.Properties.Resources.BotonHerramientasNoc;

            this.lblDuracion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.lblREproduccion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));

        }
        else
        {
            frmVMD.BackColor = Color.Transparent;

            this.imgStatusInternet.BackgroundImage = global::SMFE.Properties.Resources.Detenida;
            this.btnDetener.BackgroundImage = global::SMFE.Properties.Resources.BotonDetener;
            this.btnPlay.BackgroundImage = global::SMFE.Properties.Resources.BotonIniciar;
            this.btnReiniciar.BackgroundImage = global::SMFE.Properties.Resources.BotonReiniciar;
            this.btnNuevaPauta.BackgroundImage = global::SMFE.Properties.Resources.BotonPauta;
            this.btnVolumeUp.BackgroundImage = global::SMFE.Properties.Resources.BotonVolumenMAS;
            this.btnVolumeDown.BackgroundImage = global::SMFE.Properties.Resources.BotonVolumenMENOS;
            this.btnHerramientasVMD.BackgroundImage = global::SMFE.Properties.Resources.BotonHerramientas;

            this.lblDuracion.ForeColor = System.Drawing.Color.Black;
            this.lblREproduccion.ForeColor = System.Drawing.Color.Black;
        }
    }

    /// <summary>
    /// Se encarga de cambiar los recursos de la vista de CONDUSAT
    /// según sea modo nocturno o no
    /// </summary>
    /// <param name="Activar"></param>
    private void Func_ModoNocturnoCONDUSAT(bool Activar)
    {
        if (Activar)
        {
            //Background
            frmCondusat.BackColor = Color.Black;

            //Textos
            lblVRealTitulo.ForeColor = Color.Gray;
            _lblUnidad_0.ForeColor = Color.Gray;
            lblVMaxTitulo.ForeColor = Color.Gray;
            _lblUnidad_1.ForeColor = Color.Gray;
            lblVMax.ForeColor = Color.Green;

        }
        else
        {
            //Background
            frmCondusat.BackColor = Color.Transparent;

            //textos
            lblVRealTitulo.ForeColor = Color.FromArgb(64, 64, 64);
            _lblUnidad_0.ForeColor = Color.FromArgb(64, 64, 64);
            lblVMaxTitulo.ForeColor = Color.FromArgb(64, 64, 64);
            _lblUnidad_1.ForeColor = Color.FromArgb(64, 64, 64);
            lblVMax.ForeColor = Color.FromArgb(64, 64, 64);
        }
    }

    /// <summary>
    /// Se encarga de cambiar los recursos de la vista de SIA
    /// según sea modo nocturno o no
    /// </summary>
    /// <param name="Activar"></param>
    private void Func_ModonocturnoSIA(bool Activar)
    {
        if (Activar)
        {
            btnMensajes.BackgroundImage = Resources.mensajeNoc;

            lblTituloSIA.ForeColor = Color.Silver;
            lblStatusSIA.ForeColor = Color.Black;
            lblStatusSIA.BackColor = Color.Gray;

            if (!EnRobo)
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblancoNoc;
            }
        }
        else
        {
            btnMensajes.BackgroundImage = Resources.mensaje;
            lblTituloSIA.ForeColor = Color.Black;
            lblStatusSIA.ForeColor = Color.White;
            lblStatusSIA.BackColor = Color.Black;
            if (!EnRobo)
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblanco;
            }
        }
    }
    #endregion

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSistemas_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        UltActividad = DateTime.Now;
        CheckForIllegalCrossThreadCalls = false;
        tmrFechaHora.Start();
        Carga();
    }

    /// <summary>
    /// CLOSING
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSistemas_FormClosing(object sender, FormClosingEventArgs e)
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

    #region  "Botones"

    /// <summary>
    /// Se encarga de redimensionar el menú de sistemas cuando se aprieta el de CAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCAN_Click(object sender, EventArgs e)
    {
        BotonFocus(btnCAN.Name);
    }

    /// <summary>
    /// Se encarga de redimensionar el menú de sistemas cuando se aprieta el de VMD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnVMD_Click(object sender, EventArgs e)
    {
        BotonFocus(btnVMD.Name);
    }

    /// <summary>
    /// Se encarga de redimencionar el menú de sistemas cuando se aprieta el de CONDUSAT
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCONDUSAT_Click(object sender, EventArgs e)
    { 
        BotonFocus(btnCONDUSAT.Name);
    }

    /// <summary>
    /// Se encarga de redimencionar el menú de sistemas cuando se aprieta el de SIA
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSIA_Click(object sender, EventArgs e)
    {
        BotonFocus(btnSIA.Name);
    }

    /// <summary>
    /// Al hacer click en la imagen de ADO, manda la pantalla para salir de la app
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void imgADO_Click(object sender, EventArgs e)
    {
        
        MuestraSalir(1);
        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Al hacer click en la imagen de Off, manda la pantalla para apagar el sistema
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOff_Click(object sender, EventArgs e)
    {
        MuestraSalir(0);
        UltActividad = DateTime.Now;
    }

    /// <summary>
    /// Sirve para "bloquear la aplicacion"
    /// y mostrar la pantalla de login
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnbloquear_Click(object sender, EventArgs e)
    {
        LockPant();
        UltActividad = DateTime.Now;
    }


    /// <summary>
    /// Se encarga de activar el timer
    /// Powered ByRED 08JUN2021
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btn_Panico_MouseDown(object sender, MouseEventArgs e)
    {
        tmr_Robo.Enabled = true;
    }

    /// <summary>
    /// Se encarga de validar si se tiene el tiempo necesario para mandar la alerta 3 segundos
    /// Powered ByRED 08JUN2021
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btn_Panico_MouseUp(object sender, MouseEventArgs e)
    {
        if (contadorRobo < 10)
        {
            if (contadorRobo < 3)
            {
                tmr_Robo.Stop();
                tmr_Robo.Enabled = false;
            }
        }
        else
        {
            tmr_Robo.Stop();
            tmr_Robo.Enabled = false;
            contadorRobo = 0;

            if (ModoNocturno)
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblancoNoc;
            }
            else
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblanco;
            }

        }
    }
    #endregion

    #region "Timers"
    private void tmrFechaHora_Tick(object sender, EventArgs e)
    {
        tmrFechaHora.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFechaHora.Start();
    }

    /// <summary>
    /// Se encarga de cambiar el icono de botón de pánico
    /// Powered ByRED 08JUN2021
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmr_Robo_Tick(object sender, EventArgs e)
    {

        contadorRobo++;

        if (contadorRobo == 3)
        {
            this.btn_Panico.BackgroundImage = Resources.BotonPANICOAzul;
            Robo();
        }

        if(contadorRobo >= 10)
        {
            tmr_Robo.Stop();
            tmr_Robo.Enabled = false;
            contadorRobo = 0;
            if (ModoNocturno)
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblancoNoc;
            }
            else
            {
                this.btn_Panico.BackgroundImage = Resources.BotonPANICOblanco;
            }
        }        
    }

    #endregion
}

