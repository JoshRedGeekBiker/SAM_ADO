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



public partial class frmTransferTVE : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmTransferTVE()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmTransferTVE(bool _ModoPrueba, bool Nocturno, string _NumBus)
    {
        InitializeComponent();

        this.lblFecha.Text = DateTime.Now.ToString();
        this.lblNumeroBus.Text = "Código QR: A" + _NumBus;

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
            this.imgStatusCon.BackgroundImage = Resources.img_statusconexion_rojoNoc;
        }

    }
    #endregion

    #region "Eventos"

    //Para mandar a mostrar el status de la transferencia
    public delegate void MandarStatusTVE();
    public event MandarStatusTVE StatusTVE;

    //para validar la transferencia
    public delegate Task<bool> ValidarTransfer();
    public event ValidarTransfer Transfer;

    //Para obtener el estado de la conexión con TVE
    public delegate Task<bool> EstadoConexionTVE();
    public event EstadoConexionTVE ConexionTVE;

    //Para mandar apagar a el Wifi
    public delegate Task<bool> AdminWifi(bool encender, bool transferencia);
    public event AdminWifi Wifi;

    //Para encender/apagar el ethernet
    //Powered ByRED 27MAY2021
    public delegate Task<bool> AdminEthernet(bool encender);
    public event AdminEthernet ethernet;

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

    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    #endregion

    #region "Variables"

    private bool ConTVEAnt = true;
    private bool ConTVE = false;
    private int contador = 0;
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
                this.BackgroundImage = Resources.FondoCONblancoNoc;

                //Textos
                this.lblFecha.ForeColor = Color.Gray;
                this.lblTitulo.ForeColor = Color.Gray;

                //Botones
                this.imgADO.BackgroundImage = Resources.LogoADONoc;
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESARNoc;
                
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
                this.btnRegresar.BackgroundImage = Resources.BotonREGRESAR;

                ModoNocturno = false;
            }
        }
    }

    /// <summary>
    /// Se encarga del render de la imagen
    /// del estado de la conexión
    /// </summary>
    private void CambiarAntena()
    {
        if (ConTVE != ConTVEAnt)
        {
            ConTVEAnt = ConTVE;

            if (ModoNocturno)
            {
                if (ConTVE)
                {
                    this.imgStatusCon.BackgroundImage = Resources.img_statusconexion_verdeNoc;
                }
                else
                {
                    this.imgStatusCon.BackgroundImage = Resources.img_statusconexion_rojoNoc;
                }
            }
            else
            {
                if (ConTVE)
                {
                    this.imgStatusCon.BackgroundImage = Resources.img_statusconexion_verde;
                }
                else
                {
                    this.imgStatusCon.BackgroundImage = Resources.img_statusconexion_rojo;
                }
            }
        }
    }
    /// <summary>
    /// Se encarga de detener los procesos internos
    /// del form
    /// </summary>
    private void Detener()
    {
        this.tmrConexion.Stop();
        this.tmrStatus.Stop();
        this.tmrFecha.Stop();
        this.tmrWiFi.Stop();
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmTransferTVE_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();

        CambiarAntena();

        //tmrWiFi.Enabled = true;

        //Mandamos a apagar el ethernet en caso de estar encendido
        //Powered ByRED 27MAY2021
        ethernet(false);

        //Mandamos a encender el wifi e insertamos perfil de Wifi
        Wifi(true, true);

        //Encendemos el timer que dará tiempo al wifi de conectarse
        tmrWiFi.Start();
    }

    private void frmTransferTVE_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }
    #endregion

    #region "Botones"

    private void imgADO_Click(object sender, EventArgs e)
    {
        MuestraSalir(1);
    }

    private void btnRegresar_Click(object sender, EventArgs e)
    {
        ethernet(true);//Powered ByRED 27MAY2021
        Wifi(false, false);
        this.Detener();
        Cerrar(this);
    }

    #endregion

    #region "Timers"
    private void tmrFecha_Tick(object sender, EventArgs e)
    {
        tmrFecha.Stop();
        this.lblFecha.Text = DateTime.Now.ToString();
        tmrFecha.Start();
    }

    /// <summary>
    /// Se encarga de poner la imagen de conexión
    /// de acuerdo al status de la misma
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void tmrConexion_Tick(object sender, EventArgs e)
    {
        tmrConexion.Stop();

        ConTVE = await ConexionTVE();

        CambiarAntena();

        if (ConTVE)
        {
            tmrStatus.Enabled = true;
            //Para que no se vuelva a activar el timer
            return;
        }
        else
        {
            //if (contador >= 20)
            //{
            //    await Wifi(false);
            //    tmrConexion.Enabled = false;
            //    tmrWiFi.Enabled = true;
            //    //Para que no vuelva a activar el timer
            //    return;
            //}
            //else
            //{
            //    contador += 1;
            //}

        }
        tmrConexion.Start();
    }

    

    /// <summary>
    /// Se encarga de preguntar a logica de TVE si la transferencia fué exitosa
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void tmrStatus_Tick(object sender, EventArgs e)
    {
        tmrStatus.Stop();

        if (await Transfer())
        {
            await ethernet(true);//Powered ByRED 27MAY2021
            await Wifi(false, false);

            this.Detener();

            this.Cerrar(this);

            //Mandamos a mostrar el estado de la transferencia
            StatusTVE();

            return;
        }

        tmrStatus.Start();
    }

    /// <summary>
    /// Se encarga de darle tiempo al tmr de conexión
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrWiFi_Tick(object sender, EventArgs e)
    {
        tmrWiFi.Stop();

        //await Wifi(true);

        //contador = 0;

        tmrWiFi.Enabled = false;

        tmrConexion.Enabled = true;
        tmrConexion.Start();
    }
    #endregion

    
}

