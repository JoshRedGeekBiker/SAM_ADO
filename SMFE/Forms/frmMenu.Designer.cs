
    partial class frmMenu
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
            this.components = new System.ComponentModel.Container();
            this.imgAdo = new System.Windows.Forms.PictureBox();
            this.btnRelevo = new System.Windows.Forms.PictureBox();
            this.btnAbrirViaje = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.btnCerrarViaje = new System.Windows.Forms.PictureBox();
            this.btnDatosViaje = new System.Windows.Forms.PictureBox();
            this.btnDatosGPS = new System.Windows.Forms.PictureBox();
            this.btnMovCAN = new System.Windows.Forms.PictureBox();
            this.btnSincronizar = new System.Windows.Forms.PictureBox();
            this.btnDatosTelemetria = new System.Windows.Forms.PictureBox();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.lblCvlAutobus = new System.Windows.Forms.Label();
            this.lblNombreOperador = new System.Windows.Forms.Label();
            this.lblCvlOperador = new System.Windows.Forms.Label();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.btnTVE = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRelevo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbrirViaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrarViaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosViaje)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosGPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMovCAN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSincronizar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosTelemetria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTVE)).BeginInit();
            this.SuspendLayout();
            // 
            // imgAdo
            // 
            this.imgAdo.BackColor = System.Drawing.Color.Transparent;
            this.imgAdo.Image = global::SMFE.Properties.Resources.LogoADO;
            this.imgAdo.Location = new System.Drawing.Point(9, 16);
            this.imgAdo.Name = "imgAdo";
            this.imgAdo.Size = new System.Drawing.Size(119, 35);
            this.imgAdo.TabIndex = 0;
            this.imgAdo.TabStop = false;
            this.imgAdo.Click += new System.EventHandler(this.imgSalir_Click);
            // 
            // btnRelevo
            // 
            this.btnRelevo.BackColor = System.Drawing.Color.Transparent;
            this.btnRelevo.BackgroundImage = global::SMFE.Properties.Resources.BotonRELEVO;
            this.btnRelevo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRelevo.Location = new System.Drawing.Point(32, 280);
            this.btnRelevo.Name = "btnRelevo";
            this.btnRelevo.Size = new System.Drawing.Size(212, 55);
            this.btnRelevo.TabIndex = 32;
            this.btnRelevo.TabStop = false;
            this.btnRelevo.Click += new System.EventHandler(this.btnRelevo_Click);
            // 
            // btnAbrirViaje
            // 
            this.btnAbrirViaje.BackColor = System.Drawing.Color.Transparent;
            this.btnAbrirViaje.BackgroundImage = global::SMFE.Properties.Resources.BotonABRIRVIAJE;
            this.btnAbrirViaje.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAbrirViaje.Location = new System.Drawing.Point(32, 208);
            this.btnAbrirViaje.Name = "btnAbrirViaje";
            this.btnAbrirViaje.Size = new System.Drawing.Size(212, 55);
            this.btnAbrirViaje.TabIndex = 31;
            this.btnAbrirViaje.TabStop = false;
            this.btnAbrirViaje.Click += new System.EventHandler(this.btnAbrirViaje_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitulo.Location = new System.Drawing.Point(24, 144);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(233, 33);
            this.lblTitulo.TabIndex = 30;
            this.lblTitulo.Text = "Menú General";
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(544, 96);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblVersion.Size = new System.Drawing.Size(249, 25);
            this.lblVersion.TabIndex = 34;
            this.lblVersion.Text = "Version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFecha
            // 
            this.lblFecha.BackColor = System.Drawing.Color.Transparent;
            this.lblFecha.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFecha.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.ForeColor = System.Drawing.Color.White;
            this.lblFecha.Location = new System.Drawing.Point(440, 8);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFecha.Size = new System.Drawing.Size(361, 41);
            this.lblFecha.TabIndex = 33;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnCerrarViaje
            // 
            this.btnCerrarViaje.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrarViaje.BackgroundImage = global::SMFE.Properties.Resources.BotonCERRARVIAJE;
            this.btnCerrarViaje.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCerrarViaje.Location = new System.Drawing.Point(32, 360);
            this.btnCerrarViaje.Name = "btnCerrarViaje";
            this.btnCerrarViaje.Size = new System.Drawing.Size(212, 55);
            this.btnCerrarViaje.TabIndex = 35;
            this.btnCerrarViaje.TabStop = false;
            this.btnCerrarViaje.Click += new System.EventHandler(this.btnCerrarViaje_Click);
            // 
            // btnDatosViaje
            // 
            this.btnDatosViaje.BackColor = System.Drawing.Color.Transparent;
            this.btnDatosViaje.BackgroundImage = global::SMFE.Properties.Resources.BotonDATOSVIAJE;
            this.btnDatosViaje.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDatosViaje.Location = new System.Drawing.Point(280, 208);
            this.btnDatosViaje.Name = "btnDatosViaje";
            this.btnDatosViaje.Size = new System.Drawing.Size(212, 55);
            this.btnDatosViaje.TabIndex = 36;
            this.btnDatosViaje.TabStop = false;
            this.btnDatosViaje.Click += new System.EventHandler(this.btnDatosViaje_Click);
            // 
            // btnDatosGPS
            // 
            this.btnDatosGPS.BackColor = System.Drawing.Color.Transparent;
            this.btnDatosGPS.BackgroundImage = global::SMFE.Properties.Resources.BotonDATOSGPS;
            this.btnDatosGPS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDatosGPS.Location = new System.Drawing.Point(280, 280);
            this.btnDatosGPS.Name = "btnDatosGPS";
            this.btnDatosGPS.Size = new System.Drawing.Size(212, 55);
            this.btnDatosGPS.TabIndex = 37;
            this.btnDatosGPS.TabStop = false;
            this.btnDatosGPS.Click += new System.EventHandler(this.btnDatosGPS_Click);
            // 
            // btnMovCAN
            // 
            this.btnMovCAN.BackColor = System.Drawing.Color.Transparent;
            this.btnMovCAN.BackgroundImage = global::SMFE.Properties.Resources.BotonMOVCAN;
            this.btnMovCAN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnMovCAN.Location = new System.Drawing.Point(280, 360);
            this.btnMovCAN.Name = "btnMovCAN";
            this.btnMovCAN.Size = new System.Drawing.Size(212, 55);
            this.btnMovCAN.TabIndex = 38;
            this.btnMovCAN.TabStop = false;
            this.btnMovCAN.Click += new System.EventHandler(this.btnMovCAN_Click);
            // 
            // btnSincronizar
            // 
            this.btnSincronizar.BackColor = System.Drawing.Color.Transparent;
            this.btnSincronizar.BackgroundImage = global::SMFE.Properties.Resources.BotonoSINCRONIZAR;
            this.btnSincronizar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSincronizar.Location = new System.Drawing.Point(536, 280);
            this.btnSincronizar.Name = "btnSincronizar";
            this.btnSincronizar.Size = new System.Drawing.Size(212, 55);
            this.btnSincronizar.TabIndex = 39;
            this.btnSincronizar.TabStop = false;
            this.btnSincronizar.Click += new System.EventHandler(this.btnSincronizar_Click);
            // 
            // btnDatosTelemetria
            // 
            this.btnDatosTelemetria.BackColor = System.Drawing.Color.Transparent;
            this.btnDatosTelemetria.BackgroundImage = global::SMFE.Properties.Resources.btn_telemetria;
            this.btnDatosTelemetria.Location = new System.Drawing.Point(172, 468);
            this.btnDatosTelemetria.Name = "btnDatosTelemetria";
            this.btnDatosTelemetria.Size = new System.Drawing.Size(161, 36);
            this.btnDatosTelemetria.TabIndex = 40;
            this.btnDatosTelemetria.TabStop = false;
            this.btnDatosTelemetria.Click += new System.EventHandler(this.btnDatosTelemetria_Click);
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 42;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // lblCvlAutobus
            // 
            this.lblCvlAutobus.BackColor = System.Drawing.Color.Transparent;
            this.lblCvlAutobus.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCvlAutobus.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCvlAutobus.ForeColor = System.Drawing.Color.White;
            this.lblCvlAutobus.Location = new System.Drawing.Point(640, 560);
            this.lblCvlAutobus.Name = "lblCvlAutobus";
            this.lblCvlAutobus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCvlAutobus.Size = new System.Drawing.Size(161, 41);
            this.lblCvlAutobus.TabIndex = 45;
            this.lblCvlAutobus.Text = "CvlAutobus";
            this.lblCvlAutobus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblNombreOperador
            // 
            this.lblNombreOperador.BackColor = System.Drawing.Color.Transparent;
            this.lblNombreOperador.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNombreOperador.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreOperador.ForeColor = System.Drawing.Color.White;
            this.lblNombreOperador.Location = new System.Drawing.Point(168, 560);
            this.lblNombreOperador.Name = "lblNombreOperador";
            this.lblNombreOperador.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNombreOperador.Size = new System.Drawing.Size(473, 41);
            this.lblNombreOperador.TabIndex = 44;
            this.lblNombreOperador.Text = "Nombre del Operador";
            this.lblNombreOperador.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblCvlOperador
            // 
            this.lblCvlOperador.BackColor = System.Drawing.Color.Transparent;
            this.lblCvlOperador.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCvlOperador.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCvlOperador.ForeColor = System.Drawing.Color.White;
            this.lblCvlOperador.Location = new System.Drawing.Point(8, 560);
            this.lblCvlOperador.Name = "lblCvlOperador";
            this.lblCvlOperador.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCvlOperador.Size = new System.Drawing.Size(153, 33);
            this.lblCvlOperador.TabIndex = 43;
            this.lblCvlOperador.Text = "Cvl Operador";
            this.lblCvlOperador.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // btnTVE
            // 
            this.btnTVE.BackColor = System.Drawing.Color.Transparent;
            this.btnTVE.BackgroundImage = global::SMFE.Properties.Resources.btl_tarjetadeviaje;
            this.btnTVE.Location = new System.Drawing.Point(393, 468);
            this.btnTVE.Name = "btnTVE";
            this.btnTVE.Size = new System.Drawing.Size(161, 36);
            this.btnTVE.TabIndex = 46;
            this.btnTVE.TabStop = false;
            this.btnTVE.Click += new System.EventHandler(this.btnTVE_Click);
            // 
            // frmMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.btnTVE);
            this.Controls.Add(this.lblCvlAutobus);
            this.Controls.Add(this.lblNombreOperador);
            this.Controls.Add(this.lblCvlOperador);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.btnDatosTelemetria);
            this.Controls.Add(this.btnSincronizar);
            this.Controls.Add(this.btnMovCAN);
            this.Controls.Add(this.btnDatosGPS);
            this.Controls.Add(this.btnDatosViaje);
            this.Controls.Add(this.btnCerrarViaje);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.btnRelevo);
            this.Controls.Add(this.btnAbrirViaje);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.imgAdo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmMenu";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMenu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenu_FormClosing);
            this.Load += new System.EventHandler(this.frmMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRelevo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbrirViaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrarViaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosViaje)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosGPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMovCAN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSincronizar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDatosTelemetria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTVE)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgAdo;
        private System.Windows.Forms.PictureBox btnRelevo;
        private System.Windows.Forms.PictureBox btnAbrirViaje;
        public System.Windows.Forms.Label lblTitulo;
        public System.Windows.Forms.Label lblVersion;
        public System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.PictureBox btnCerrarViaje;
        private System.Windows.Forms.PictureBox btnDatosViaje;
        private System.Windows.Forms.PictureBox btnDatosGPS;
        private System.Windows.Forms.PictureBox btnMovCAN;
        private System.Windows.Forms.PictureBox btnSincronizar;
        private System.Windows.Forms.PictureBox btnDatosTelemetria;
        private System.Windows.Forms.PictureBox btnRegresar;
        public System.Windows.Forms.Label lblCvlAutobus;
        public System.Windows.Forms.Label lblNombreOperador;
        public System.Windows.Forms.Label lblCvlOperador;
    private System.Windows.Forms.Timer tmrFecha;
    private System.Windows.Forms.PictureBox btnTVE;
}
