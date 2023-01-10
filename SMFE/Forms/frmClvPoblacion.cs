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


public partial class frmClvPoblacion : Form
{
    #region "Constructores"


    /// <summary>
    /// Constructor Principal
    /// </summary>
    public frmClvPoblacion()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor Productivo
    /// </summary>
    /// <param name="_tipo"></param>
    /// <param name="_clvConductor"></param>
    public frmClvPoblacion(string _tipo, string _clvConductor, bool _ModoPrueba, bool Nocturno)
    {
        this.TipoViaje = _tipo;
        this.ClaveConductor = _clvConductor;

        InitializeComponent();
        

        this.AjustarTexto();

        if (!_ModoPrueba)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new Size(800, 600);
            this.MinimumSize = new Size(800, 600);
            this.Size = new Size(800, 600);
            Cursor.Hide();
        }

        UltActividad = DateTime.Now;

        if (Nocturno)
        {
            ActivarModonocturno(Nocturno);
        }
    }

    #endregion

    #region "Propiedades"
    private bool ModoNocturno { get; set; } = false;
    private bool Nocturno { get; set; } = false;
    #endregion

    #region "Variables"

    private string ClaveConductor = string.Empty;
    private string TipoViaje = string.Empty;
    private DateTime UltActividad;
    #endregion

    #region "Eventos"

    public delegate bool RevisarPoblacion(string CVE);
    public event RevisarPoblacion ChecaPob;

    public delegate void MandarViaje(string tipo, string clvConductor);
    public event MandarViaje Viaje;

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
                //Backgrounds
                this.BackgroundImage = Resources.fondoCVPoblacionNoc;

                //Textos
                lblFecha.ForeColor = Color.Gray;
                

                //Botones
                btn_Cambio.BackgroundImage = Resources.btn_123Noc;
                btnABC.BackgroundImage = Resources.btn_abcNoc;
                btnAceptar.BackgroundImage = Resources.btn_aceptarNoc;
                btnAceptar2.BackgroundImage = Resources.btn_aceptarNoc;
                btnCancelar.BackgroundImage = Resources.btn_cancelarNoc;
                btnCancelar2.BackgroundImage = Resources.btn_cancelarNoc;
                btnBorrar.BackgroundImage = Resources.btn_borrarNoc;
                btnBorrar2.BackgroundImage = Resources.btn_borrarNoc;


                //Letras
                label_A.Image = Resources.BotonNUMEROSNoc;
                label_B.Image = Resources.BotonNUMEROSNoc;
                label_C.Image = Resources.BotonNUMEROSNoc;
                label_D.Image = Resources.BotonNUMEROSNoc;
                label_E.Image = Resources.BotonNUMEROSNoc;
                label_F.Image = Resources.BotonNUMEROSNoc;
                label_G.Image = Resources.BotonNUMEROSNoc;
                label_H.Image = Resources.BotonNUMEROSNoc;
                label_I.Image = Resources.BotonNUMEROSNoc;
                label_J.Image = Resources.BotonNUMEROSNoc;
                label_K.Image = Resources.BotonNUMEROSNoc;
                label_L.Image = Resources.BotonNUMEROSNoc;
                label_M.Image = Resources.BotonNUMEROSNoc;
                label_N.Image = Resources.BotonNUMEROSNoc;
                label_O.Image = Resources.BotonNUMEROSNoc;
                label_P.Image = Resources.BotonNUMEROSNoc;
                label_Q.Image = Resources.BotonNUMEROSNoc;
                label_R.Image = Resources.BotonNUMEROSNoc;
                label_S.Image = Resources.BotonNUMEROSNoc;
                label_T.Image = Resources.BotonNUMEROSNoc;
                label_U.Image = Resources.BotonNUMEROSNoc;
                label_V.Image = Resources.BotonNUMEROSNoc;
                label_W.Image = Resources.BotonNUMEROSNoc;
                label_X.Image = Resources.BotonNUMEROSNoc;
                label_Y.Image = Resources.BotonNUMEROSNoc;
                label_Z.Image = Resources.BotonNUMEROSNoc;


                //Numeros

                label0.Image = Resources.BotonNUMEROSNoc;
                label1.Image = Resources.BotonNUMEROSNoc;
                label2.Image = Resources.BotonNUMEROSNoc;
                label3.Image = Resources.BotonNUMEROSNoc;
                label4.Image = Resources.BotonNUMEROSNoc;
                label5.Image = Resources.BotonNUMEROSNoc;
                label6.Image = Resources.BotonNUMEROSNoc;
                label7.Image = Resources.BotonNUMEROSNoc;
                label8.Image = Resources.BotonNUMEROSNoc;
                label9.Image = Resources.BotonNUMEROSNoc;
                labelGato.Image = Resources.BotonNUMEROSNoc;


                ModoNocturno = true;
            }
        }
        else
        {
            if (ModoNocturno)
            {
                this.BackgroundImage = Resources.fondoCVpoblacion;

                //Textos
                lblFecha.ForeColor = Color.White;


                //Botones
                btn_Cambio.BackgroundImage = Resources.btn_123;
                btnABC.BackgroundImage = Resources.btn_abc;
                btnAceptar.BackgroundImage = Resources.btn_aceptar;
                btnAceptar2.BackgroundImage = Resources.btn_aceptar;
                btnCancelar.BackgroundImage = Resources.btn_cancelar;
                btnCancelar2.BackgroundImage = Resources.btn_cancelar;
                btnBorrar.BackgroundImage = Resources.btn_borrar;
                btnBorrar2.BackgroundImage = Resources.btn_borrar;


                //Letras
                label_A.Image = Resources.BotonNUMEROS;
                label_B.Image = Resources.BotonNUMEROS;
                label_C.Image = Resources.BotonNUMEROS;
                label_D.Image = Resources.BotonNUMEROS;
                label_E.Image = Resources.BotonNUMEROS;
                label_F.Image = Resources.BotonNUMEROS;
                label_G.Image = Resources.BotonNUMEROS;
                label_H.Image = Resources.BotonNUMEROS;
                label_I.Image = Resources.BotonNUMEROS;
                label_J.Image = Resources.BotonNUMEROS;
                label_K.Image = Resources.BotonNUMEROS;
                label_L.Image = Resources.BotonNUMEROS;
                label_M.Image = Resources.BotonNUMEROS;
                label_N.Image = Resources.BotonNUMEROS;
                label_O.Image = Resources.BotonNUMEROS;
                label_P.Image = Resources.BotonNUMEROS;
                label_Q.Image = Resources.BotonNUMEROS;
                label_R.Image = Resources.BotonNUMEROS;
                label_S.Image = Resources.BotonNUMEROS;
                label_T.Image = Resources.BotonNUMEROS;
                label_U.Image = Resources.BotonNUMEROS;
                label_V.Image = Resources.BotonNUMEROS;
                label_W.Image = Resources.BotonNUMEROS;
                label_X.Image = Resources.BotonNUMEROS;
                label_Y.Image = Resources.BotonNUMEROS;
                label_Z.Image = Resources.BotonNUMEROS;


                //Numeros

                label0.Image = Resources.BotonNUMEROS;
                label1.Image = Resources.BotonNUMEROS;
                label2.Image = Resources.BotonNUMEROS;
                label3.Image = Resources.BotonNUMEROS;
                label4.Image = Resources.BotonNUMEROS;
                label5.Image = Resources.BotonNUMEROS;
                label6.Image = Resources.BotonNUMEROS;
                label7.Image = Resources.BotonNUMEROS;
                label8.Image = Resources.BotonNUMEROS;
                label9.Image = Resources.BotonNUMEROS;
                labelGato.Image = Resources.BotonNUMEROS;


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
    /// Ajusta el Label de mensaje de la pantalla
    /// de acuerto al tipo de Viaje
    /// </summary>
    /// <param name="tipo"></param>
    private void AjustarTexto()
    {
        imgSenial.Visible = false;

        if (this.TipoViaje.Equals("VA") || this.TipoViaje.Equals("CM"))
        {
            this.lblMensaje.Text = "Ingrese la clave de poblacion origen (3 digitos)";
        }
        else if (this.TipoViaje.Equals("VC"))
        {
            this.lblMensaje.Text = "Ingrese la clave de poblacion destino (3 digitos)";
        }
    }

    private void frmClvPoblacion_FormClosing(object sender, FormClosingEventArgs e)
    {
        //Powered ByRED 10DIC2020
        if (e.CloseReason != CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
        }
    }

    /// <summary>
    /// LOAD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmClvPoblacion_Load(object sender, EventArgs e)
    {
        this.Location = Ubicacion();
    }

    #endregion

    #region "Botones VistaNumerica"

    private void label0_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "0";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label1_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "1";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label2_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "2";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label3_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "3";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label4_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "4";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label5_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "5";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label6_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "6";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label7_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "7";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label8_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "8";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label9_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "9";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void labelGato_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "#";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void btnABC_Click(object sender, EventArgs e)
    {
        this.GBNumeros.Visible = false;
        this.GBAlfa.Visible = true;
        UltActividad = DateTime.Now;
    }

    private void btnAceptar2_Click(object sender, EventArgs e)
    {
        //Evento de que ya estufas

        if (this.txtDisplay.Text.Length > 0)
        {
            if (ChecaPob(this.txtDisplay.Text))//Significa que es correcta y manda a viaje
            {
                //mandamos a viaje
                Viaje(this.TipoViaje, this.ClaveConductor);
                this.Detener();
                this.Cerrar(this);
            }
            else
            {//significa que no encontró clave de poblacion
                this.imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
                this.lblMensaje.Text = "Clave Incorrecta";
            }
        }
        else
        {
            this.imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
            this.lblMensaje.Text = "Clave Incorrecta";
        }
        UltActividad = DateTime.Now;
    }

    private void btnCancelar2_Click(object sender, EventArgs e)
    {
        Cerrar(this);
    }

    private void btnBorrar2_Click(object sender, EventArgs e)
    {
        if (txtDisplay.TextLength > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Count() - 1);
            txtDisplay.SelectionStart = txtDisplay.TextLength;
        }
        UltActividad = DateTime.Now;
    }

    #endregion

    #region "Botones VistaAlfabeto"
    private void label_A_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "A";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_B_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "B";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_C_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "C";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_D_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "D";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_E_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "E";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_F_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "F";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_G_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "G";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_H_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "H";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_I_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "I";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_J_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "J";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_K_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "K";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_L_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "L";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_M_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "M";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_N_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "N";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_O_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "O";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_P_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "P";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_Q_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "Q";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_R_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "R";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_S_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "S";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_T_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "T";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_U_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "U";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_V_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "V";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_W_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "W";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_X_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "X";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_Y_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "Y";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void label_Z_Click(object sender, EventArgs e)
    {
        txtDisplay.Text = txtDisplay.Text + "Z";
        txtDisplay.SelectionStart = txtDisplay.TextLength;
        UltActividad = DateTime.Now;
    }

    private void btn_Cambio_Click(object sender, EventArgs e)
    {
        this.GBAlfa.Visible = false;
        this.GBNumeros.Visible = true;
        UltActividad = DateTime.Now;
    }

    private void btnAceptar_Click(object sender, EventArgs e)
    {
        //mandar evento de que ya estufas
        
        if (this.txtDisplay.Text.Length > 0)
        {
            if (ChecaPob(this.txtDisplay.Text))//Significa que es correcta y manda a viaje
            {
                //mandamos a viaje
                Viaje(this.TipoViaje, this.ClaveConductor);
                this.Detener();
                this.Cerrar(this);
            }
            else
            {//significa que no encontró clave de poblacion
                this.imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
                this.lblMensaje.Text = "Clave Incorrecta";
                this.txtDisplay.Text = "";
            }
        }
        else
        {
            this.imgSenial.BackgroundImage = Resources.ConductorIncorrecto;
            this.lblMensaje.Text = "Clave Incorrecta";
            this.txtDisplay.Text = "";
        }

        UltActividad = DateTime.Now;

    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        Cerrar(this);
    }

    private void btnBorrar_Click(object sender, EventArgs e)
    {
        if (txtDisplay.TextLength > 0)
        {
            txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Count() - 1);
            txtDisplay.SelectionStart = txtDisplay.TextLength;
        }
        UltActividad = DateTime.Now;
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

