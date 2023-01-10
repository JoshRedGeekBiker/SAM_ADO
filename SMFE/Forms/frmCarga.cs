using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class frmCarga : Form
{
    #region "Constructores"

    /// <summary>
    /// Constructor principal
    /// </summary>
    public frmCarga()
    {
        InitializeComponent();

        Cursor.Hide();
    }

    #endregion

    #region "Variables"
    private DateTime tiempo;
    #endregion

    #region "Eventos"
    /// <summary>
    /// Se encarga de preguntar
    /// si ya se debe de cerrar la ventana
    /// </summary>
    /// <returns></returns>
    public delegate bool FlagCerrar();
    public event FlagCerrar Cerrar;

    /// <summary>
    /// Se encarga de recuperar la ubicación de la pantalla principal
    /// </summary>
    /// <returns></returns>
    public delegate Point ObtenerUbicacion();
    public event ObtenerUbicacion Ubicacion;
    #endregion

    #region "Metodos"

    /// <summary>
    /// Load del form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void frmCarga_Load(object sender, EventArgs e)
    {
        CheckForIllegalCrossThreadCalls = false;

        this.Location = Ubicacion();

        this.Opacity = 0.0;

        await incrementarOpacidad();
    }

    /// <summary>
    /// Se encarga darle el efecto de "aparecer"
    /// al inicio de la vista
    /// </summary>
    /// <returns></returns>
    private Task<bool> incrementarOpacidad()
    {
         return Task<bool>.Run(
            async() =>{
            double incremental = 0.0;

            do
            {
                this.Opacity = incremental;
                incremental += 0.01;

                    await Task.Delay(1);

                } while (incremental < 1);

                return true;
        });

    }

    /// <summary>
    /// Se encarga de lanzar el delay para cerrar la vista
    /// </summary>
    private void TiempoCerrar()
    {
        tiempo = DateTime.Now;
        tmrCerrar.Enabled = true;
        tmrCerrar.Start();
    }

    #endregion

    #region "Timers"

    /// <summary>
    /// Se encarga de mantener el form hasta adelante
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrSiempreAdelante_Tick(object sender, EventArgs e)
    {
        tmrSiempreAdelante.Stop();
        this.TopMost = true;
        //Cursor.Hide();

        if (!Cerrar())
        {
            TiempoCerrar();
            return;
        }
        tmrSiempreAdelante.Start();
    }


    /// <summary>
    /// Se encarga de darle unos segundos antes de cerrar la ventana
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tmrCerrar_Tick(object sender, EventArgs e)
    {
        tmrCerrar.Stop();

        if ((DateTime.Now - tiempo).TotalSeconds >= 2)
        {
            this.Close();
            this.Dispose();
            //Cursor.Show();
            return;
        }
        
        tmrCerrar.Start();
    }

    #endregion
}
