
    partial class frmConfiguracion
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
            this.imgADO = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblInidicaciones = new System.Windows.Forms.Label();
            this._lblOpcion_0 = new System.Windows.Forms.Label();
            this._lblOpcion_1 = new System.Windows.Forms.Label();
            this._lblOpcion_2 = new System.Windows.Forms.Label();
            this._lblOpcion_3 = new System.Windows.Forms.Label();
            this._lblDescripcion_0 = new System.Windows.Forms.Label();
            this._lblDescripcion_1 = new System.Windows.Forms.Label();
            this._lblDescripcion_2 = new System.Windows.Forms.Label();
            this._lblDescripcion_3 = new System.Windows.Forms.Label();
            this.btnArriba = new System.Windows.Forms.PictureBox();
            this.btnAbajo = new System.Windows.Forms.PictureBox();
            this.btnEnter = new System.Windows.Forms.PictureBox();
            this.btnAtras = new System.Windows.Forms.PictureBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArriba)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbajo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAtras)).BeginInit();
            this.SuspendLayout();
            // 
            // imgADO
            // 
            this.imgADO.BackColor = System.Drawing.Color.Transparent;
            this.imgADO.BackgroundImage = global::SMFE.Properties.Resources.LogoADO;
            this.imgADO.Location = new System.Drawing.Point(8, 8);
            this.imgADO.Name = "imgADO";
            this.imgADO.Size = new System.Drawing.Size(119, 35);
            this.imgADO.TabIndex = 25;
            this.imgADO.TabStop = false;
            this.imgADO.Click += new System.EventHandler(this.imgADO_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(8, 16);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(785, 41);
            this.lblTitulo.TabIndex = 26;
            this.lblTitulo.Text = "Configurar";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblInidicaciones
            // 
            this.lblInidicaciones.BackColor = System.Drawing.Color.Transparent;
            this.lblInidicaciones.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblInidicaciones.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInidicaciones.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblInidicaciones.Location = new System.Drawing.Point(8, 80);
            this.lblInidicaciones.Name = "lblInidicaciones";
            this.lblInidicaciones.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblInidicaciones.Size = new System.Drawing.Size(745, 33);
            this.lblInidicaciones.TabIndex = 27;
            this.lblInidicaciones.Text = "¿Que tipo de autobus es?";
            // 
            // _lblOpcion_0
            // 
            this._lblOpcion_0.BackColor = System.Drawing.Color.Transparent;
            this._lblOpcion_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblOpcion_0.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblOpcion_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblOpcion_0.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this._lblOpcion_0.Location = new System.Drawing.Point(8, 128);
            this._lblOpcion_0.Name = "_lblOpcion_0";
            this._lblOpcion_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblOpcion_0.Size = new System.Drawing.Size(90, 90);
            this._lblOpcion_0.TabIndex = 28;
            this._lblOpcion_0.Text = ">";
            this._lblOpcion_0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblOpcion_0.Click += new System.EventHandler(this._lblOpcion_0_Click);
            // 
            // _lblOpcion_1
            // 
            this._lblOpcion_1.BackColor = System.Drawing.Color.Transparent;
            this._lblOpcion_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblOpcion_1.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblOpcion_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblOpcion_1.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this._lblOpcion_1.Location = new System.Drawing.Point(8, 232);
            this._lblOpcion_1.Name = "_lblOpcion_1";
            this._lblOpcion_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblOpcion_1.Size = new System.Drawing.Size(90, 90);
            this._lblOpcion_1.TabIndex = 29;
            this._lblOpcion_1.Text = ">";
            this._lblOpcion_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblOpcion_1.Click += new System.EventHandler(this._lblOpcion_1_Click);
            // 
            // _lblOpcion_2
            // 
            this._lblOpcion_2.BackColor = System.Drawing.Color.Transparent;
            this._lblOpcion_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblOpcion_2.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblOpcion_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblOpcion_2.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this._lblOpcion_2.Location = new System.Drawing.Point(8, 336);
            this._lblOpcion_2.Name = "_lblOpcion_2";
            this._lblOpcion_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblOpcion_2.Size = new System.Drawing.Size(90, 90);
            this._lblOpcion_2.TabIndex = 30;
            this._lblOpcion_2.Text = ">";
            this._lblOpcion_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblOpcion_2.Click += new System.EventHandler(this._lblOpcion_2_Click);
            // 
            // _lblOpcion_3
            // 
            this._lblOpcion_3.BackColor = System.Drawing.Color.Transparent;
            this._lblOpcion_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblOpcion_3.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblOpcion_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblOpcion_3.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this._lblOpcion_3.Location = new System.Drawing.Point(8, 440);
            this._lblOpcion_3.Name = "_lblOpcion_3";
            this._lblOpcion_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblOpcion_3.Size = new System.Drawing.Size(90, 90);
            this._lblOpcion_3.TabIndex = 31;
            this._lblOpcion_3.Text = ">";
            this._lblOpcion_3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._lblOpcion_3.Click += new System.EventHandler(this._lblOpcion_3_Click);
            // 
            // _lblDescripcion_0
            // 
            this._lblDescripcion_0.BackColor = System.Drawing.Color.Transparent;
            this._lblDescripcion_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblDescripcion_0.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDescripcion_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblDescripcion_0.Location = new System.Drawing.Point(120, 160);
            this._lblDescripcion_0.Name = "_lblDescripcion_0";
            this._lblDescripcion_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblDescripcion_0.Size = new System.Drawing.Size(521, 25);
            this._lblDescripcion_0.TabIndex = 32;
            this._lblDescripcion_0.Text = "Seleccione la opción que corresponda";
            // 
            // _lblDescripcion_1
            // 
            this._lblDescripcion_1.BackColor = System.Drawing.Color.Transparent;
            this._lblDescripcion_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblDescripcion_1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDescripcion_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblDescripcion_1.Location = new System.Drawing.Point(120, 264);
            this._lblDescripcion_1.Name = "_lblDescripcion_1";
            this._lblDescripcion_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblDescripcion_1.Size = new System.Drawing.Size(521, 25);
            this._lblDescripcion_1.TabIndex = 33;
            this._lblDescripcion_1.Text = "Seleccione la opción que corresponda";
            // 
            // _lblDescripcion_2
            // 
            this._lblDescripcion_2.BackColor = System.Drawing.Color.Transparent;
            this._lblDescripcion_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblDescripcion_2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDescripcion_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblDescripcion_2.Location = new System.Drawing.Point(120, 368);
            this._lblDescripcion_2.Name = "_lblDescripcion_2";
            this._lblDescripcion_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblDescripcion_2.Size = new System.Drawing.Size(545, 25);
            this._lblDescripcion_2.TabIndex = 34;
            this._lblDescripcion_2.Text = "Seleccione la opción que corresponda";
            // 
            // _lblDescripcion_3
            // 
            this._lblDescripcion_3.BackColor = System.Drawing.Color.Transparent;
            this._lblDescripcion_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._lblDescripcion_3.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDescripcion_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._lblDescripcion_3.Location = new System.Drawing.Point(120, 472);
            this._lblDescripcion_3.Name = "_lblDescripcion_3";
            this._lblDescripcion_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lblDescripcion_3.Size = new System.Drawing.Size(521, 25);
            this._lblDescripcion_3.TabIndex = 35;
            this._lblDescripcion_3.Text = "Seleccione la opción que corresponda";
            // 
            // btnArriba
            // 
            this.btnArriba.BackgroundImage = global::SMFE.Properties.Resources.FlechaARRIBA;
            this.btnArriba.Location = new System.Drawing.Point(480, 128);
            this.btnArriba.Name = "btnArriba";
            this.btnArriba.Size = new System.Drawing.Size(49, 47);
            this.btnArriba.TabIndex = 36;
            this.btnArriba.TabStop = false;
            this.btnArriba.Click += new System.EventHandler(this.btnArriba_Click);
            // 
            // btnAbajo
            // 
            this.btnAbajo.BackgroundImage = global::SMFE.Properties.Resources.FlechaABAJO;
            this.btnAbajo.Location = new System.Drawing.Point(480, 464);
            this.btnAbajo.Name = "btnAbajo";
            this.btnAbajo.Size = new System.Drawing.Size(49, 47);
            this.btnAbajo.TabIndex = 37;
            this.btnAbajo.TabStop = false;
            this.btnAbajo.Click += new System.EventHandler(this.btnAbajo_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.Transparent;
            this.btnEnter.BackgroundImage = global::SMFE.Properties.Resources.BotonAdelante;
            this.btnEnter.Location = new System.Drawing.Point(688, 336);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(86, 86);
            this.btnEnter.TabIndex = 60;
            this.btnEnter.TabStop = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnAtras
            // 
            this.btnAtras.BackColor = System.Drawing.Color.Transparent;
            this.btnAtras.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnAtras.Location = new System.Drawing.Point(672, 448);
            this.btnAtras.Name = "btnAtras";
            this.btnAtras.Size = new System.Drawing.Size(106, 81);
            this.btnAtras.TabIndex = 62;
            this.btnAtras.TabStop = false;
            this.btnAtras.Click += new System.EventHandler(this.btnAtras_Click);
            // 
            // lblFecha
            // 
            this.lblFecha.BackColor = System.Drawing.Color.Transparent;
            this.lblFecha.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFecha.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.ForeColor = System.Drawing.Color.White;
            this.lblFecha.Location = new System.Drawing.Point(473, 24);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFecha.Size = new System.Drawing.Size(320, 33);
            this.lblFecha.TabIndex = 64;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // frmConfiguracion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.fondoCVpoblacion;
            this.ClientSize = new System.Drawing.Size(802, 601);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.btnAtras);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.btnAbajo);
            this.Controls.Add(this.btnArriba);
            this.Controls.Add(this._lblDescripcion_3);
            this.Controls.Add(this._lblDescripcion_2);
            this.Controls.Add(this._lblDescripcion_1);
            this.Controls.Add(this._lblDescripcion_0);
            this.Controls.Add(this._lblOpcion_3);
            this.Controls.Add(this._lblOpcion_2);
            this.Controls.Add(this._lblOpcion_1);
            this.Controls.Add(this._lblOpcion_0);
            this.Controls.Add(this.lblInidicaciones);
            this.Controls.Add(this.imgADO);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(818, 640);
            this.MinimumSize = new System.Drawing.Size(818, 640);
            this.Name = "frmConfiguracion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmConfiguracion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfiguracion_FormClosing);
            this.Load += new System.EventHandler(this.frmConfiguracion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnArriba)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAbajo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAtras)).EndInit();
            this.ResumeLayout(false);

        }

    #endregion

    private System.Windows.Forms.PictureBox imgADO;
    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label lblInidicaciones;
    public System.Windows.Forms.Label _lblOpcion_0;
    public System.Windows.Forms.Label _lblOpcion_1;
    public System.Windows.Forms.Label _lblOpcion_2;
    public System.Windows.Forms.Label _lblOpcion_3;
    public System.Windows.Forms.Label _lblDescripcion_0;
    public System.Windows.Forms.Label _lblDescripcion_1;
    public System.Windows.Forms.Label _lblDescripcion_2;
    public System.Windows.Forms.Label _lblDescripcion_3;
    private System.Windows.Forms.PictureBox btnArriba;
    private System.Windows.Forms.PictureBox btnAbajo;
    private System.Windows.Forms.PictureBox btnEnter;
    private System.Windows.Forms.PictureBox btnAtras;
    public System.Windows.Forms.Label lblFecha;
    private System.Windows.Forms.Timer tmrFecha;
}
