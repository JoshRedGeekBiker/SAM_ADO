
    partial class frmConfigAutobus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfigAutobus));
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.imgADO = new System.Windows.Forms.PictureBox();
            this.imgSenial = new System.Windows.Forms.PictureBox();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl5 = new System.Windows.Forms.Label();
            this.lbl0 = new System.Windows.Forms.Label();
            this.lbl8 = new System.Windows.Forms.Label();
            this.lbl6 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl9 = new System.Windows.Forms.Label();
            this.lbl7 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.btnApagar = new System.Windows.Forms.PictureBox();
            this.btnEnter = new System.Windows.Forms.PictureBox();
            this.btnBorrar = new System.Windows.Forms.PictureBox();
            this.btnAtras = new System.Windows.Forms.PictureBox();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.listaRegion = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSenial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApagar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAtras)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(8, 88);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(785, 41);
            this.lblTitulo.TabIndex = 4;
            this.lblTitulo.Text = "Configurar";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
            this.lblVersion.TabIndex = 16;
            this.lblVersion.Text = "Version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFecha
            // 
            this.lblFecha.BackColor = System.Drawing.Color.Transparent;
            this.lblFecha.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFecha.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.ForeColor = System.Drawing.Color.White;
            this.lblFecha.Location = new System.Drawing.Point(473, 9);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFecha.Size = new System.Drawing.Size(320, 33);
            this.lblFecha.TabIndex = 17;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblMensaje
            // 
            this.lblMensaje.BackColor = System.Drawing.Color.Transparent;
            this.lblMensaje.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblMensaje.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMensaje.Location = new System.Drawing.Point(344, 152);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMensaje.Size = new System.Drawing.Size(434, 76);
            this.lblMensaje.TabIndex = 21;
            this.lblMensaje.Text = "Esta unidad no cuenta con número de autobus. Teclee el número de autobus.";
            // 
            // txtDisplay
            // 
            this.txtDisplay.AcceptsReturn = true;
            this.txtDisplay.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtDisplay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDisplay.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisplay.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtDisplay.Location = new System.Drawing.Point(24, 160);
            this.txtDisplay.MaxLength = 0;
            this.txtDisplay.Multiline = true;
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtDisplay.Size = new System.Drawing.Size(225, 57);
            this.txtDisplay.TabIndex = 22;
            this.txtDisplay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDisplay_KeyPress);
            // 
            // imgADO
            // 
            this.imgADO.BackColor = System.Drawing.Color.Transparent;
            this.imgADO.BackgroundImage = global::SMFE.Properties.Resources.LogoADO;
            this.imgADO.Location = new System.Drawing.Point(16, 16);
            this.imgADO.Name = "imgADO";
            this.imgADO.Size = new System.Drawing.Size(119, 35);
            this.imgADO.TabIndex = 23;
            this.imgADO.TabStop = false;
            // 
            // imgSenial
            // 
            this.imgSenial.BackColor = System.Drawing.Color.Transparent;
            this.imgSenial.BackgroundImage = global::SMFE.Properties.Resources.ConductorVALIDO;
            this.imgSenial.Location = new System.Drawing.Point(256, 152);
            this.imgSenial.Name = "imgSenial";
            this.imgSenial.Size = new System.Drawing.Size(76, 76);
            this.imgSenial.TabIndex = 24;
            this.imgSenial.TabStop = false;
            // 
            // lbl1
            // 
            this.lbl1.BackColor = System.Drawing.Color.Transparent;
            this.lbl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl1.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl1.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl1.Location = new System.Drawing.Point(136, 248);
            this.lbl1.Name = "lbl1";
            this.lbl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl1.Size = new System.Drawing.Size(98, 90);
            this.lbl1.TabIndex = 48;
            this.lbl1.Text = "1";
            this.lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl1.Click += new System.EventHandler(this.lbl1_Click);
            // 
            // lbl5
            // 
            this.lbl5.BackColor = System.Drawing.Color.Transparent;
            this.lbl5.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl5.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl5.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl5.Location = new System.Drawing.Point(344, 248);
            this.lbl5.Name = "lbl5";
            this.lbl5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl5.Size = new System.Drawing.Size(90, 90);
            this.lbl5.TabIndex = 57;
            this.lbl5.Text = "5";
            this.lbl5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl5.Click += new System.EventHandler(this.lbl5_Click);
            // 
            // lbl0
            // 
            this.lbl0.BackColor = System.Drawing.Color.Transparent;
            this.lbl0.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl0.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl0.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl0.Location = new System.Drawing.Point(552, 344);
            this.lbl0.Name = "lbl0";
            this.lbl0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl0.Size = new System.Drawing.Size(90, 90);
            this.lbl0.TabIndex = 56;
            this.lbl0.Text = "0";
            this.lbl0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl0.Click += new System.EventHandler(this.lbl0_Click);
            // 
            // lbl8
            // 
            this.lbl8.BackColor = System.Drawing.Color.Transparent;
            this.lbl8.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl8.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl8.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl8.Location = new System.Drawing.Point(448, 344);
            this.lbl8.Name = "lbl8";
            this.lbl8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl8.Size = new System.Drawing.Size(90, 90);
            this.lbl8.TabIndex = 55;
            this.lbl8.Text = "8";
            this.lbl8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl8.Click += new System.EventHandler(this.lbl8_Click);
            // 
            // lbl6
            // 
            this.lbl6.BackColor = System.Drawing.Color.Transparent;
            this.lbl6.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl6.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl6.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl6.Location = new System.Drawing.Point(344, 344);
            this.lbl6.Name = "lbl6";
            this.lbl6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl6.Size = new System.Drawing.Size(90, 90);
            this.lbl6.TabIndex = 54;
            this.lbl6.Text = "6";
            this.lbl6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl6.Click += new System.EventHandler(this.lbl6_Click);
            // 
            // lbl4
            // 
            this.lbl4.BackColor = System.Drawing.Color.Transparent;
            this.lbl4.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl4.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl4.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl4.Location = new System.Drawing.Point(240, 344);
            this.lbl4.Name = "lbl4";
            this.lbl4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl4.Size = new System.Drawing.Size(90, 90);
            this.lbl4.TabIndex = 53;
            this.lbl4.Text = "4";
            this.lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl4.Click += new System.EventHandler(this.lbl4_Click);
            // 
            // lbl2
            // 
            this.lbl2.BackColor = System.Drawing.Color.Transparent;
            this.lbl2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl2.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl2.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl2.Location = new System.Drawing.Point(136, 344);
            this.lbl2.Name = "lbl2";
            this.lbl2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl2.Size = new System.Drawing.Size(90, 90);
            this.lbl2.TabIndex = 52;
            this.lbl2.Text = "2";
            this.lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl2.Click += new System.EventHandler(this.lbl2_Click);
            // 
            // lbl9
            // 
            this.lbl9.BackColor = System.Drawing.Color.Transparent;
            this.lbl9.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl9.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl9.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl9.Location = new System.Drawing.Point(552, 248);
            this.lbl9.Name = "lbl9";
            this.lbl9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl9.Size = new System.Drawing.Size(90, 90);
            this.lbl9.TabIndex = 51;
            this.lbl9.Text = "9";
            this.lbl9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl9.Click += new System.EventHandler(this.lbl9_Click);
            // 
            // lbl7
            // 
            this.lbl7.BackColor = System.Drawing.Color.Transparent;
            this.lbl7.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl7.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl7.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl7.Location = new System.Drawing.Point(448, 248);
            this.lbl7.Name = "lbl7";
            this.lbl7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl7.Size = new System.Drawing.Size(90, 90);
            this.lbl7.TabIndex = 50;
            this.lbl7.Text = "7";
            this.lbl7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl7.Click += new System.EventHandler(this.lbl7_Click);
            // 
            // lbl3
            // 
            this.lbl3.BackColor = System.Drawing.Color.Transparent;
            this.lbl3.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl3.Font = new System.Drawing.Font("Verdana", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl3.Image = global::SMFE.Properties.Resources.BotonNUMEROS;
            this.lbl3.Location = new System.Drawing.Point(240, 248);
            this.lbl3.Name = "lbl3";
            this.lbl3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl3.Size = new System.Drawing.Size(90, 90);
            this.lbl3.TabIndex = 49;
            this.lbl3.Text = "3";
            this.lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl3.Click += new System.EventHandler(this.lbl3_Click);
            // 
            // btnApagar
            // 
            this.btnApagar.BackColor = System.Drawing.Color.Transparent;
            this.btnApagar.BackgroundImage = global::SMFE.Properties.Resources.btn_off;
            this.btnApagar.Location = new System.Drawing.Point(8, 456);
            this.btnApagar.Name = "btnApagar";
            this.btnApagar.Size = new System.Drawing.Size(75, 75);
            this.btnApagar.TabIndex = 58;
            this.btnApagar.TabStop = false;
            this.btnApagar.Click += new System.EventHandler(this.btnApagar_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.BackColor = System.Drawing.Color.Transparent;
            this.btnEnter.BackgroundImage = global::SMFE.Properties.Resources.BotonAdelante;
            this.btnEnter.Location = new System.Drawing.Point(280, 448);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(86, 86);
            this.btnEnter.TabIndex = 59;
            this.btnEnter.TabStop = false;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.BackColor = System.Drawing.Color.Transparent;
            this.btnBorrar.BackgroundImage = global::SMFE.Properties.Resources.BotonBorrar;
            this.btnBorrar.Location = new System.Drawing.Point(416, 448);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(86, 86);
            this.btnBorrar.TabIndex = 60;
            this.btnBorrar.TabStop = false;
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // btnAtras
            // 
            this.btnAtras.BackColor = System.Drawing.Color.Transparent;
            this.btnAtras.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnAtras.Location = new System.Drawing.Point(672, 448);
            this.btnAtras.Name = "btnAtras";
            this.btnAtras.Size = new System.Drawing.Size(106, 81);
            this.btnAtras.TabIndex = 61;
            this.btnAtras.TabStop = false;
            this.btnAtras.Click += new System.EventHandler(this.btnAtras_Click);
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // listaRegion
            // 
            this.listaRegion.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.listaRegion.DropDownHeight = 300;
            this.listaRegion.DropDownWidth = 300;
            this.listaRegion.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaRegion.FormattingEnabled = true;
            this.listaRegion.IntegralHeight = false;
            this.listaRegion.ItemHeight = 39;
            this.listaRegion.Location = new System.Drawing.Point(256, 170);
            this.listaRegion.MaxDropDownItems = 20;
            this.listaRegion.Name = "listaRegion";
            this.listaRegion.Size = new System.Drawing.Size(534, 47);
            this.listaRegion.TabIndex = 64;
            this.listaRegion.TabStop = false;
            this.listaRegion.Text = "Seleccione una Region:";
            this.listaRegion.SelectedIndexChanged += new System.EventHandler(this.listaRegion_SelectedIndexChanged);
            // 
            // frmConfigAutobus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(802, 601);
            this.Controls.Add(this.listaRegion);
            this.Controls.Add(this.btnAtras);
            this.Controls.Add(this.btnBorrar);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.btnApagar);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.lbl5);
            this.Controls.Add(this.lbl0);
            this.Controls.Add(this.lbl8);
            this.Controls.Add(this.lbl6);
            this.Controls.Add(this.lbl4);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.lbl9);
            this.Controls.Add(this.lbl7);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.imgSenial);
            this.Controls.Add(this.imgADO);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(818, 640);
            this.MinimumSize = new System.Drawing.Size(818, 640);
            this.Name = "frmConfigAutobus";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmConfigAutobus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfigAutobus_FormClosing);
            this.Load += new System.EventHandler(this.frmConfigAutobus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSenial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnApagar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEnter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBorrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAtras)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label lblVersion;
    public System.Windows.Forms.Label lblFecha;
    public System.Windows.Forms.Label lblMensaje;
    public System.Windows.Forms.TextBox txtDisplay;
    private System.Windows.Forms.PictureBox imgADO;
    private System.Windows.Forms.PictureBox imgSenial;
    public System.Windows.Forms.Label lbl1;
    public System.Windows.Forms.Label lbl5;
    public System.Windows.Forms.Label lbl0;
    public System.Windows.Forms.Label lbl8;
    public System.Windows.Forms.Label lbl6;
    public System.Windows.Forms.Label lbl4;
    public System.Windows.Forms.Label lbl2;
    public System.Windows.Forms.Label lbl9;
    public System.Windows.Forms.Label lbl7;
    public System.Windows.Forms.Label lbl3;
    private System.Windows.Forms.PictureBox btnApagar;
    private System.Windows.Forms.PictureBox btnEnter;
    private System.Windows.Forms.PictureBox btnBorrar;
    private System.Windows.Forms.PictureBox btnAtras;
    private System.Windows.Forms.Timer tmrFecha;
    private System.Windows.Forms.ComboBox listaRegion;
}
