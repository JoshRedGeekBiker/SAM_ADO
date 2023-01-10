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


    public partial class frmMenu : Form
    {
    #region "Constructores"


    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmMenu()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmMenu(bool _ModoPrueba, bool Nocturno, string _Version, bool _Telemetria)
    {
        InitializeComponent();

        if (_Telemetria)
        {
            btnDatosTelemetria.Visible = true;
        }
        else
        {
            btnDatosTelemetria.Visible = false;
        }

        lblFecha.Text = DateTime.Now.ToString();
        
        lblVersion.Text = _Version;

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        modoPrueba = _ModoPrueba;

        ReiniciarPie();

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
        
    #endregion

    #region "Eventos"

    //Para Mandar a llamar La Sync en SAM
    public delegate void MandarSincronizar();
    public event MandarSincronizar MandaSync;

    //Para mostrar algun Error
    public delegate void MandarError(string mensaje);
    public event MandarError MandaError;

    //Para Mandar a abrir Viaje
    public delegate void Viaje(string tipo);
    public event Viaje MostrarViaje;

    //Para verificar el status del viaje
    public delegate bool ChecarViaje();
    public event ChecarViaje EnViaje;

    //Para validar que se tenga protocolo
    public delegate bool ChecarProtocolo();
    public event ChecarProtocolo Protocolo;

    //Para validar la velocidad a la que va
    public delegate int ChecarVelocidad();
    public event ChecarVelocidad Velocidad;

    //Para mandar a llamar la vista de los Datos de Viaje
    public delegate void TraerDatosViaje(string tipo);
    public event TraerDatosViaje DatosViaje;

    //Para mandar a llamar la vista de Datos GPS
    public delegate void TraerDatosGPS(string tipo);
    public event TraerDatosGPS DatosGPS;

    //Para mandar a llamar la vista de MovtosCan
    public delegate void TraerDatosMovtos();
    public event TraerDatosMovtos DatosMovtos;

    //Para mandar mostrar los datos de Tarjeta de Viaje Electronica
    public delegate void MostrarTVE();
    public event MostrarTVE TVE;

    //Manda a traer la pantalla de salida o de apagado
    //Segun sea el caso 0:apagar, 1:salir
    public delegate void VistaSalir(int _tipo);
    public event VistaSalir MuestraSalir;

    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    //Pregunta a Front Engine se nos encontramos en sincronización
    public delegate bool PreguntarSync();
    public event PreguntarSync EnSync;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

    /// <summary>
    /// Manda a pedir la ventana de Telemetria
    /// Powered ByRED 15JUN2020
    /// </summary>
    public delegate void MostrarTELEMETRIA();
    public event MostrarTELEMETRIA TELEMETRIA;

    #endregion

    #region "Propiedades"    
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    
    #endregion

    #region "Variables"
    private DateTime UltActividad;
    private bool modoPrueba;

    private bool VentanaEmergente = false;

    #endregion

    #region "Metodos"

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
                this.lblVersion.ForeColor = Color.Gray;
                this.lblCvlOperador.ForeColor = Color.Gray;
                this.lblNombreOperador.ForeColor = Color.Gray;
                this.lblCvlAutobus.ForeColor = Color.Gray;

                //Botones
                this.imgAdo.BackgroundImage = Resources.LogoADONoc;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;
                this.btnAbrirViaje.BackgroundImage = Resources.BotonABRIRVIAJENoc;
                this.btnRelevo.BackgroundImage = Resources.BotonRELEVONoc;
                this.btnCerrarViaje.BackgroundImage = Resources.BotonCERRARVIAJENoc;
                this.btnDatosViaje.BackgroundImage = Resources.BotonDATOSVIAJENoc;
                this.btnDatosGPS.BackgroundImage = Resources.BotonDATOSGPSNoc;
                this.btnMovCAN.BackgroundImage = Resources.BotonMOVCANNoc;
                this.btnSincronizar.BackgroundImage = Resources.BotonoSINCRONIZARNoc;
                this.btnDatosTelemetria.BackgroundImage = Resources.btn_telemetriaNoc;
                this.btnTVE.BackgroundImage = Resources.btl_tarjetadeviajeNoc;


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
                this.lblVersion.ForeColor = Color.White;
                this.lblCvlOperador.ForeColor = Color.White;
                this.lblNombreOperador.ForeColor = Color.White;
                this.lblCvlAutobus.ForeColor = Color.White;

                //Botones
                this.imgAdo.BackgroundImage = Resources.LogoADO;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESAR;
                this.btnAbrirViaje.BackgroundImage = Resources.BotonABRIRVIAJE;
                this.btnRelevo.BackgroundImage = Resources.BotonRELEVO;
                this.btnCerrarViaje.BackgroundImage = Resources.BotonCERRARVIAJE;
                this.btnDatosViaje.BackgroundImage = Resources.BotonDATOSVIAJE;
                this.btnDatosGPS.BackgroundImage = Resources.BotonDATOSGPS;
                this.btnMovCAN.BackgroundImage = Resources.BotonMOVCAN;
                this.btnSincronizar.BackgroundImage = Resources.BotonoSINCRONIZAR;
                this.btnDatosTelemetria.BackgroundImage = Resources.btn_telemetria;
                this.btnTVE.BackgroundImage = Resources.btl_tarjetadeviaje;

                ModoNocturno = false;
            }
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
        VentanaEmergente = false;
    }
    
    /// <summary>
    /// Se encarga de reiniciar las variables
    /// </summary>
    private void ReiniciarPie()
    {
        this.lblCvlOperador.Text = string.Empty;
        this.lblNombreOperador.Text = string.Empty;
        this.lblCvlAutobus.Text = string.Empty;
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmMenu_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
        UltActividad = DateTime.Now;
        this.TopMost = true;

        //Para recuperar los datos de pie de pagina
        //en caso de que exixta un viaje
        EnViaje();
        
    }

    private void frmMenu_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
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

    #endregion

    #region "Botones"

    /// <summary>
    /// Manda a abrir un viaje de CAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAbrirViaje_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                if (EnViaje())
                {
                    MandaError("El viaje se encuentra abierto");
                }
                else
                {
                    if (modoPrueba)//Si el sistema se encuentra en modo prueba, no importa que no se tenga protocolo CAN
                    {
                        MostrarViaje("VA");
                    }
                    else
                    {
                        if (Protocolo()) //Se verifica la existencia de protocolo CAN
                        {
                            if (Velocidad() > 0) // Se verifica que la unidad no tenga velocidad
                            {
                                MandaError("La unidad debe de estar detenenida");
                            }
                            else
                            {
                                MostrarViaje("VA");
                            }
                        }
                        else
                        {
                            MandaError("La unidad se encuentra apagada o no se detectan los datos de CAN. Intente de nuevo.");
                        }
                    }
                }
                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Manda a hacer un relevo de CAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRelevo_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                if (!EnViaje())
                {
                    MandaError("El viaje se encuentra cerrado");
                }
                else
                {
                    if (modoPrueba) //Si el sistema se encuentra en modo prueba, no importa que no se tenga protocolo CAN
                    {
                        MostrarViaje("CM");
                    }
                    else
                    {
                        if (Protocolo())//Se verifica la existencia de protocolo
                        {

                            if (Velocidad() > 0)// Se verifica que la unidad no tenga velocidad
                            {
                                MandaError("La unidad debe de estar detenenida");
                            }
                            else
                            {
                                MostrarViaje("CM");
                            }
                        }
                        else
                        {
                            MandaError("La unidad se encuentra apagada o no se detectan los datos de CAN. Intente de nuevo.");
                        }
                    }
                }
                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Manda a cerrar el viaje de CAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCerrarViaje_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                if (!EnViaje())
                {
                    MandaError("El viaje se encuentra cerrado");
                }
                else
                {
                    if (modoPrueba)//Si el sistema se encuentra en modo prueba, no importa que no se tenga protocolo CAN
                    {
                        MostrarViaje("VC");
                    }
                    else
                    {
                        if (Protocolo())//Se verifica la existencia de protocolo
                        {

                            if (Velocidad() > 0)// Se verifica que la unidad no tenga velocidad
                            {
                                MandaError("La unidad debe estar detenida");
                            }
                            else
                            {
                                MostrarViaje("VC");
                            }
                        }
                        else
                        {
                            MandaError("La unidad se encuentra apagada o no se detectan los datos de CAN. Intente de nuevo.");
                        }
                    }
                }

                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Muestra los datos del viaje
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDatosViaje_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            VentanaEmergente = true;

            if (!EnSync())
            {
                DatosViaje("VIAJE");

                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Muestra los datos del GPS
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDatosGPS_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            VentanaEmergente = true;
            if (!EnSync())
            {
                DatosGPS("GPS");

                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Muestra la cantidad de MovtosCAN
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMovCAN_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            VentanaEmergente = true;

            if (!EnSync())
            {
                DatosMovtos();

                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Manda aviso a SAM para sincronizar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSincronizar_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            VentanaEmergente = true;
            if (!EnSync())
            {
                if (!EnViaje())
                {
                    MandaSync();
                }
                else
                {
                    MandaError("El viaje se encuentra abierto.");
                }

                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Manda a abrir un Viaje de prueba
    /// Powered ByRED 15JUN2020
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDatosTelemetria_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                UltActividad = DateTime.Now;

                TELEMETRIA();
            }
        }
    }

    /// <summary>
    /// Manda a mostrar la salida de la aplicacion
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void imgSalir_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                MuestraSalir(1);
                UltActividad = DateTime.Now;
            }
        }
    }

    /// <summary>
    /// Regresa a la pantalla anterior
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRegresar_Click(object sender, EventArgs e)
    {
        //Se quitó la validación de ventana emergente
        if (!EnSync())
        {
            Detener();
            Cerrar(this);
        }
    }

    /// <summary>
    /// Abrir el menú de TVE
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTVE_Click(object sender, EventArgs e)
    {
        if (!VentanaEmergente)
        {
            if (!EnSync())
            {
                UltActividad = DateTime.Now;
                TVE();
            }
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

    //if (Actividad)
    //{
    //    Actividad = false;
    //    UltActividad = DateTime.Now;
    //}

    //if ((DateTime.Now - UltActividad).TotalSeconds >= 10)
    //{
    //    Detener();
    //Cerrar(this);
    //}
    //else
    //{

    //}
    #endregion
    }