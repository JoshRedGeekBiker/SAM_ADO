using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SMFE.Properties;


public partial class frmSync : Form
{   
    #region "Constructores"

    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmSync()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_ModoPrueba"></param>
    public frmSync(bool _ModoPrueba, bool Nocturno)
    {
        InitializeComponent();
        MensajeSync = "";
        MensajeLog = "";
        imgAceptar.Visible = false;
        imgCancelar.Visible = false;

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(649, 402);
            this.MinimumSize = new Size(649, 402);
            this.Size = new Size(649, 402);
            Cursor.Hide();
        }

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }
    #endregion

    #region "Propiedades"
    public string MensajeSync { get; set; }
    public string MensajeLog { get; set; }
    public string MinutosApagado { get; set; }

    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    #endregion

    #region "Variables"
    private bool Exitoso = false;
    private TimeSpan tiempo;

    private string mensajeFinal = string.Empty;
    private string mensajeFinalTemp = string.Empty;

    #endregion

    #region "Eventos"
    //Avisa al front para que cierre correctamente el form
    public delegate void CerrarForm(Form vista);
    public event CerrarForm Cerrar;

    /// <summary>
    /// Avisa que tiene que apaga1
    /// </summary>
    public delegate void MandarApagar();
    public event MandarApagar Apagar;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;

    #endregion

    #region "Metodos"

    /// <summary>
    /// Se encarga de verificar si ya pasó el tiempo de 
    /// actividad del form
    /// </summary>
    /// <returns></returns>
    public bool VerificaActividad(int TiempoEspera)
    {
        //Siempre retorna verdadero, para evitar que
        //el administrador de vista lo cierre por falta
        //de actividad
        return true;
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
                this.BackgroundImage = Resources.EncuadreSincronizarNoc;

                //Textos
                this.lblTitulo.ForeColor = Color.Gray;

                //Botones
                this.imgAceptar.BackgroundImage = Resources.AceptarSincronizarNoc;
                this.imgCancelar.BackgroundImage = Resources.CancelarSincronizarNOC;

                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {
                //Background
                this.BackgroundImage = Resources.EncuadreSincronizar;

                //Textos
                this.lblTitulo.ForeColor = Color.White;

                //Botones
                this.imgAceptar.BackgroundImage = Resources.AceptarSincronizar;
                this.imgCancelar.BackgroundImage = Resources.CancelarSincronizar;

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
        this.tmActualiza.Stop();
    }

    /// <summary>
    /// Se encarga de mostrar el mensaje en pantalla
    /// cada vez que es llamado, proviene desde el Front
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="final"></param>
    public void EventoSync(string mensaje, int final)
    {
        if (Convert.ToBoolean(final))
        {
            //logica Anterior

            //mensajeFinal = mensajeFinal + mensaje + txtLog.Text + Environment.NewLine;
            //lblMensajeFinal.Text = mensajeFinal;
            ////lblMensajeFinal.Text = lblMensajeFinal.Text + "\n" + mensaje + txtLog.Text;
            //txtLog.Text = "";
            //Thread.Sleep(5000);

            //Nueva logica

            mensajeFinal = mensajeFinal + mensaje + Environment.NewLine;
            lblMensajeFinal.Text = mensajeFinal;
            txtLog.Text = "";
            Thread.Sleep(5000);

        }
        else
        {
            txtLog.Text = mensaje;
        }

        Application.DoEvents();
    }

    /// <summary>
    /// Se encarga de recibir si la sincronización fué
    /// exitosa o no
    /// </summary>
    /// <param name="exitoso"></param>
    public void SyncOK(bool _exitoso)
    {

        Exitoso = _exitoso;

        if (Exitoso)
        {
            imgCancelar.Visible = true;
            txtLog.Text = "";
            try
            {
                if(MinutosApagado.Length > 1)
                {
                    tiempo = TimeSpan.Parse("00:0" + MinutosApagado + ":00");
                }
                else
                {
                    tiempo = TimeSpan.Parse("00:" + MinutosApagado + ":00");
                }

                
            }
            catch
            {
                tiempo = TimeSpan.Parse("00:02:00");
            }
            

            tmrApagado.Enabled = true;
            tmrApagado.Start();
        }
        imgAceptar.Visible = true;

        mensajeFinal = string.Empty;

        Application.DoEvents();
        tmActualiza.Stop();
    }

    /// <summary>
    /// Evitar que se cierre el Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSync_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    //public void EventoSyncCAN(string mensaje, int final)
    //{
    //    if (Convert.ToBoolean(final))
    //    {
    //        lblMensajeFinal.Text = mensaje;
    //    }
    //    else
    //    {
    //        txtLog.AppendText(" \n" + mensaje);
    //    }

    //    Application.DoEvents();
    //}

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSync_Load(object sender, EventArgs e)
    {
        Point punto = Ubicacion();

        int x = punto.X + 68;
        int y = punto.Y + 79;

        this.Location = new Point(x, y);
    }
    #endregion

    #region "Botones"
    private void imgAceptar_Click(object sender, EventArgs e)
    {
        //Si la sincronización fué exitosa
        //mandamos a apagar el equipo, si no, 
        //cerramos el form
        if (Exitoso)
        {
            tmrApagado.Stop();
            Apagar();
        }
        else
        {
            //Avisamos al front que cerramos vista
            Detener();
            Cerrar(this);
        }
    }

    /// <summary>
    /// Se ocupa para cancelar el apagado del equipo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void imgCancelar_Click(object sender, EventArgs e)
    {
        tmrApagado.Stop();
        tmrApagado.Enabled = false;
        Detener();
        Cerrar(this);
    }
    #endregion

    #region "Timers"

    private void tmActualiza_Tick(object sender, EventArgs e)
    {
        txtLog.AppendText(MensajeLog);

        lblMensajeFinal.Text = lblMensajeFinal.Text + " " + MensajeSync; ;
    }


    /// <summary>
    /// Se encarga de llevar la cuenta regresiva para
    /// apagar el equipo después de una sincronización
    /// exitosa
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrApagado_Tick(object sender, EventArgs e)
    {
        tiempo = tiempo.Subtract(new TimeSpan(0,0, 1));
        txtLog.Text = "El equipo se apagará en:"+ Environment.NewLine + tiempo.Minutes.ToString("D2") + ":" + tiempo.Seconds.ToString("D2");

        if (tiempo.Minutes == 0 && tiempo.Seconds == 0)
        {
            tmrApagado.Stop();

            Apagar();
        }
    }
    #endregion
}

