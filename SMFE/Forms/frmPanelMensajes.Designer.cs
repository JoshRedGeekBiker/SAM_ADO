partial class frmPanelMensajes
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.PictureBox();
            this.btnSubir = new System.Windows.Forms.PictureBox();
            this.btnBajar = new System.Windows.Forms.PictureBox();
            this.Flecha1 = new System.Windows.Forms.PictureBox();
            this.Flecha2 = new System.Windows.Forms.PictureBox();
            this.Flecha3 = new System.Windows.Forms.PictureBox();
            this.lblMensaje1 = new System.Windows.Forms.Label();
            this.lblMensaje2 = new System.Windows.Forms.Label();
            this.lblMensaje3 = new System.Windows.Forms.Label();
            this.btnRecibidos = new System.Windows.Forms.PictureBox();
            this.btnEnviados = new System.Windows.Forms.PictureBox();
            this.btnEnviar = new System.Windows.Forms.PictureBox();
            this.btnNuevoEnvio = new System.Windows.Forms.PictureBox();
            this.btnLeer = new System.Windows.Forms.PictureBox();
            this.btnEnviados_2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBajar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecibidos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNuevoEnvio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviados_2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(26, 21);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(338, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Mensajes para enviar";
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrar.BackgroundImage = global::SMFE.Properties.Resources.BotonCERRAMENSAJES;
            this.btnCerrar.Location = new System.Drawing.Point(537, 6);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(60, 59);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.TabStop = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnSubir
            // 
            this.btnSubir.BackColor = System.Drawing.Color.DarkGray;
            this.btnSubir.BackgroundImage = global::SMFE.Properties.Resources.FlechaARRIBA;
            this.btnSubir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSubir.Location = new System.Drawing.Point(537, 80);
            this.btnSubir.Name = "btnSubir";
            this.btnSubir.Size = new System.Drawing.Size(60, 90);
            this.btnSubir.TabIndex = 5;
            this.btnSubir.TabStop = false;
            this.btnSubir.Click += new System.EventHandler(this.btnSubir_Click);
            // 
            // btnBajar
            // 
            this.btnBajar.BackColor = System.Drawing.Color.DarkGray;
            this.btnBajar.BackgroundImage = global::SMFE.Properties.Resources.FlechaABAJO;
            this.btnBajar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBajar.Location = new System.Drawing.Point(537, 180);
            this.btnBajar.Name = "btnBajar";
            this.btnBajar.Size = new System.Drawing.Size(60, 90);
            this.btnBajar.TabIndex = 6;
            this.btnBajar.TabStop = false;
            this.btnBajar.Click += new System.EventHandler(this.btnBajar_Click);
            // 
            // Flecha1
            // 
            this.Flecha1.BackColor = System.Drawing.Color.Transparent;
            this.Flecha1.BackgroundImage = global::SMFE.Properties.Resources.FlechaMENSAJES;
            this.Flecha1.Enabled = false;
            this.Flecha1.Location = new System.Drawing.Point(43, 90);
            this.Flecha1.Name = "Flecha1";
            this.Flecha1.Size = new System.Drawing.Size(45, 38);
            this.Flecha1.TabIndex = 7;
            this.Flecha1.TabStop = false;
            // 
            // Flecha2
            // 
            this.Flecha2.BackColor = System.Drawing.Color.Transparent;
            this.Flecha2.BackgroundImage = global::SMFE.Properties.Resources.FlechaMENSAJES;
            this.Flecha2.Enabled = false;
            this.Flecha2.Location = new System.Drawing.Point(43, 152);
            this.Flecha2.Name = "Flecha2";
            this.Flecha2.Size = new System.Drawing.Size(45, 38);
            this.Flecha2.TabIndex = 8;
            this.Flecha2.TabStop = false;
            // 
            // Flecha3
            // 
            this.Flecha3.BackColor = System.Drawing.Color.Transparent;
            this.Flecha3.BackgroundImage = global::SMFE.Properties.Resources.FlechaMENSAJES;
            this.Flecha3.Enabled = false;
            this.Flecha3.Location = new System.Drawing.Point(43, 210);
            this.Flecha3.Name = "Flecha3";
            this.Flecha3.Size = new System.Drawing.Size(45, 38);
            this.Flecha3.TabIndex = 9;
            this.Flecha3.TabStop = false;
            // 
            // lblMensaje1
            // 
            this.lblMensaje1.BackColor = System.Drawing.Color.Transparent;
            this.lblMensaje1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje1.Location = new System.Drawing.Point(126, 90);
            this.lblMensaje1.Name = "lblMensaje1";
            this.lblMensaje1.Size = new System.Drawing.Size(390, 38);
            this.lblMensaje1.TabIndex = 10;
            this.lblMensaje1.Text = "Mensaje 1";
            this.lblMensaje1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje1.Click += new System.EventHandler(this.lblMensaje1_Click);
            // 
            // lblMensaje2
            // 
            this.lblMensaje2.BackColor = System.Drawing.Color.Transparent;
            this.lblMensaje2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje2.Location = new System.Drawing.Point(126, 152);
            this.lblMensaje2.Name = "lblMensaje2";
            this.lblMensaje2.Size = new System.Drawing.Size(390, 38);
            this.lblMensaje2.TabIndex = 11;
            this.lblMensaje2.Text = "Mensaje 2";
            this.lblMensaje2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje2.Click += new System.EventHandler(this.lblMensaje2_Click);
            // 
            // lblMensaje3
            // 
            this.lblMensaje3.BackColor = System.Drawing.Color.Transparent;
            this.lblMensaje3.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje3.Location = new System.Drawing.Point(126, 210);
            this.lblMensaje3.Name = "lblMensaje3";
            this.lblMensaje3.Size = new System.Drawing.Size(390, 38);
            this.lblMensaje3.TabIndex = 12;
            this.lblMensaje3.Text = "Mensaje 3";
            this.lblMensaje3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMensaje3.Click += new System.EventHandler(this.lblMensaje3_Click);
            // 
            // btnRecibidos
            // 
            this.btnRecibidos.BackColor = System.Drawing.Color.Transparent;
            this.btnRecibidos.BackgroundImage = global::SMFE.Properties.Resources.BotonRECIBIDOSpeke;
            this.btnRecibidos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRecibidos.Location = new System.Drawing.Point(413, 303);
            this.btnRecibidos.Name = "btnRecibidos";
            this.btnRecibidos.Size = new System.Drawing.Size(167, 43);
            this.btnRecibidos.TabIndex = 15;
            this.btnRecibidos.TabStop = false;
            this.btnRecibidos.Click += new System.EventHandler(this.btnRecibidos_Click);
            // 
            // btnEnviados
            // 
            this.btnEnviados.BackColor = System.Drawing.Color.Transparent;
            this.btnEnviados.BackgroundImage = global::SMFE.Properties.Resources.BotonENVIADOS;
            this.btnEnviados.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEnviados.Location = new System.Drawing.Point(224, 303);
            this.btnEnviados.Name = "btnEnviados";
            this.btnEnviados.Size = new System.Drawing.Size(150, 43);
            this.btnEnviados.TabIndex = 14;
            this.btnEnviados.TabStop = false;
            this.btnEnviados.Click += new System.EventHandler(this.btnEnviados_Click);
            // 
            // btnEnviar
            // 
            this.btnEnviar.BackColor = System.Drawing.Color.Transparent;
            this.btnEnviar.BackgroundImage = global::SMFE.Properties.Resources.BotonENVIARpeke;
            this.btnEnviar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEnviar.Location = new System.Drawing.Point(32, 296);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(150, 50);
            this.btnEnviar.TabIndex = 13;
            this.btnEnviar.TabStop = false;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // btnNuevoEnvio
            // 
            this.btnNuevoEnvio.BackColor = System.Drawing.Color.Transparent;
            this.btnNuevoEnvio.BackgroundImage = global::SMFE.Properties.Resources.BotonNUEVOenvio;
            this.btnNuevoEnvio.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNuevoEnvio.Location = new System.Drawing.Point(200, 303);
            this.btnNuevoEnvio.Name = "btnNuevoEnvio";
            this.btnNuevoEnvio.Size = new System.Drawing.Size(195, 43);
            this.btnNuevoEnvio.TabIndex = 16;
            this.btnNuevoEnvio.TabStop = false;
            this.btnNuevoEnvio.Click += new System.EventHandler(this.btnNuevoEnvio_Click);
            // 
            // btnLeer
            // 
            this.btnLeer.BackColor = System.Drawing.Color.Transparent;
            this.btnLeer.BackgroundImage = global::SMFE.Properties.Resources.botonLEER;
            this.btnLeer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnLeer.Location = new System.Drawing.Point(32, 303);
            this.btnLeer.Name = "btnLeer";
            this.btnLeer.Size = new System.Drawing.Size(150, 43);
            this.btnLeer.TabIndex = 17;
            this.btnLeer.TabStop = false;
            this.btnLeer.Click += new System.EventHandler(this.btnLeer_Click);
            // 
            // btnEnviados_2
            // 
            this.btnEnviados_2.BackColor = System.Drawing.Color.Transparent;
            this.btnEnviados_2.BackgroundImage = global::SMFE.Properties.Resources.BotonENVIADOS;
            this.btnEnviados_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEnviados_2.Location = new System.Drawing.Point(430, 303);
            this.btnEnviados_2.Name = "btnEnviados_2";
            this.btnEnviados_2.Size = new System.Drawing.Size(150, 43);
            this.btnEnviados_2.TabIndex = 18;
            this.btnEnviados_2.TabStop = false;
            this.btnEnviados_2.Click += new System.EventHandler(this.btnEnviados_2_Click);
            // 
            // frmPanelMensajes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.encuadremensajesAzul;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(621, 358);
            this.Controls.Add(this.btnEnviados);
            this.Controls.Add(this.btnEnviar);
            this.Controls.Add(this.lblMensaje3);
            this.Controls.Add(this.lblMensaje2);
            this.Controls.Add(this.lblMensaje1);
            this.Controls.Add(this.Flecha3);
            this.Controls.Add(this.Flecha2);
            this.Controls.Add(this.Flecha1);
            this.Controls.Add(this.btnBajar);
            this.Controls.Add(this.btnSubir);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.btnNuevoEnvio);
            this.Controls.Add(this.btnLeer);
            this.Controls.Add(this.btnRecibidos);
            this.Controls.Add(this.btnEnviados_2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(637, 397);
            this.MinimumSize = new System.Drawing.Size(637, 397);
            this.Name = "frmPanelMensajes";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPanelMensajes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPanelMensajes_FormClosing);
            this.Load += new System.EventHandler(this.frmPanelMensajes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSubir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBajar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Flecha3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRecibidos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnNuevoEnvio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLeer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnviados_2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.PictureBox btnCerrar;
    private System.Windows.Forms.PictureBox btnSubir;
    private System.Windows.Forms.PictureBox btnBajar;
    private System.Windows.Forms.PictureBox Flecha1;
    private System.Windows.Forms.PictureBox Flecha2;
    private System.Windows.Forms.PictureBox Flecha3;
    private System.Windows.Forms.Label lblMensaje1;
    private System.Windows.Forms.Label lblMensaje2;
    private System.Windows.Forms.Label lblMensaje3;
    private System.Windows.Forms.PictureBox btnRecibidos;
    private System.Windows.Forms.PictureBox btnEnviados;
    private System.Windows.Forms.PictureBox btnEnviar;
    private System.Windows.Forms.PictureBox btnNuevoEnvio;
    private System.Windows.Forms.PictureBox btnLeer;
    private System.Windows.Forms.PictureBox btnEnviados_2;
}