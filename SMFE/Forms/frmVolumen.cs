using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMFE.Forms
{
    public partial class frmVolumen : Form
    {

        public enum TipoDeVolumen
        {
            Mas = 0,
            Menos = 1
        }

        public frmVolumen()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            Cursor.Hide();

        }

        public frmVolumen(TipoDeVolumen Tipo, int AudioMostrar, bool ModoNocturno)
        {
           
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            Func_ModoGrafico(ModoNocturno, Tipo);
            this.TxtVolumen.Text = "Volumen: " + AudioMostrar.ToString();
            var hilo = new System.Threading.Thread(Func_CerrarVentana);
            hilo.IsBackground = true;
            hilo.Start();
        }

        public void Func_CerrarVentana()
        {
            System.Threading.Thread.Sleep(1000);
            this.Close();

        }


        private void Func_ModoGrafico(bool Nocturno, TipoDeVolumen Tipo)
        {
            if (Nocturno)
            {
                this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(73)))), ((int)(((byte)(90)))));
                this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(73)))), ((int)(((byte)(90)))));
                this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(73)))), ((int)(((byte)(90)))));
                this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(73)))), ((int)(((byte)(90)))));
                this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(99)))), ((int)(((byte)(99)))));
                if (Tipo == TipoDeVolumen.Mas)
                    this.pictureBox1.Image = global::SMFE.Properties.Resources.BotonVolumenMASNoc;
                else
                    this.pictureBox1.Image = global::SMFE.Properties.Resources.BotonVolumenMENOSNoc;
            }
            else
            {
                this.panel1.BackColor = System.Drawing.Color.DeepSkyBlue;
                this.panel2.BackColor = System.Drawing.Color.DeepSkyBlue;
                this.panel3.BackColor = System.Drawing.Color.DeepSkyBlue;
                this.panel4.BackColor = System.Drawing.Color.DeepSkyBlue;
                this.panel5.BackColor = Color.White;
                if (Tipo == TipoDeVolumen.Mas)
                    this.pictureBox1.Image = global::SMFE.Properties.Resources.BotonVolumenMAS;
                else
                    this.pictureBox1.Image = global::SMFE.Properties.Resources.BotonVolumenMENOS;

            }

        }

        private void frmVolumen_FormClosing(object sender, FormClosingEventArgs e)
        {

            //PoweredByRED 04Marzo2002
            //Se quita porque no dejaba cerrar el form

            ////Powered ByRED 10DIC2020
            //if (e.CloseReason != CloseReason.WindowsShutDown)
            //{
            //    e.Cancel = true;
            //}
        }
    }
}
