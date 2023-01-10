
    partial class frmTransferTVE
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
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.imgQRCon = new System.Windows.Forms.WebBrowser();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.imgStatusCon = new System.Windows.Forms.PictureBox();
            this.tmrConexion = new System.Windows.Forms.Timer(this.components);
            this.tmrStatus = new System.Windows.Forms.Timer(this.components);
            this.lblNumeroBus = new System.Windows.Forms.Label();
            this.tmrWiFi = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusCon)).BeginInit();
            this.SuspendLayout();
            // 
            // imgADO
            // 
            this.imgADO.BackColor = System.Drawing.Color.Transparent;
            this.imgADO.Image = global::SMFE.Properties.Resources.LogoADO;
            this.imgADO.Location = new System.Drawing.Point(9, 16);
            this.imgADO.Name = "imgADO";
            this.imgADO.Size = new System.Drawing.Size(119, 35);
            this.imgADO.TabIndex = 29;
            this.imgADO.TabStop = false;
            this.imgADO.Click += new System.EventHandler(this.imgADO_Click);
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
            this.lblFecha.TabIndex = 30;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblTitulo.Location = new System.Drawing.Point(8, 80);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(785, 41);
            this.lblTitulo.TabIndex = 48;
            this.lblTitulo.Text = "Vincular con Abordaje Electrónico";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(10, 147);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(785, 41);
            this.label1.TabIndex = 49;
            this.label1.Text = "Escanear código QR para establecer conexión con el equipo abordo.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // imgQRCon
            // 
            this.imgQRCon.Location = new System.Drawing.Point(44, 192);
            this.imgQRCon.MinimumSize = new System.Drawing.Size(20, 20);
            this.imgQRCon.Name = "imgQRCon";
            this.imgQRCon.ScrollBarsEnabled = false;
            this.imgQRCon.Size = new System.Drawing.Size(400, 304);
            this.imgQRCon.TabIndex = 50;
            this.imgQRCon.Url = new System.Uri("file:///C:/xampp/htdocs/phpqrcode/QRFILES/qrcode.html", System.UriKind.Absolute);
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 51;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // imgStatusCon
            // 
            this.imgStatusCon.BackColor = System.Drawing.Color.Transparent;
            this.imgStatusCon.BackgroundImage = global::SMFE.Properties.Resources.img_statusconexion_rojo;
            this.imgStatusCon.Location = new System.Drawing.Point(475, 185);
            this.imgStatusCon.Name = "imgStatusCon";
            this.imgStatusCon.Size = new System.Drawing.Size(254, 257);
            this.imgStatusCon.TabIndex = 52;
            this.imgStatusCon.TabStop = false;
            // 
            // tmrConexion
            // 
            this.tmrConexion.Interval = 1000;
            this.tmrConexion.Tick += new System.EventHandler(this.tmrConexion_Tick);
            // 
            // tmrStatus
            // 
            this.tmrStatus.Interval = 1000;
            this.tmrStatus.Tick += new System.EventHandler(this.tmrStatus_Tick);
            // 
            // lblNumeroBus
            // 
            this.lblNumeroBus.AutoSize = true;
            this.lblNumeroBus.BackColor = System.Drawing.Color.Transparent;
            this.lblNumeroBus.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroBus.Location = new System.Drawing.Point(118, 504);
            this.lblNumeroBus.Name = "lblNumeroBus";
            this.lblNumeroBus.Size = new System.Drawing.Size(254, 25);
            this.lblNumeroBus.TabIndex = 53;
            this.lblNumeroBus.Text = "Código QR: Anúmero";
            // 
            // tmrWiFi
            // 
            this.tmrWiFi.Interval = 10000;
            this.tmrWiFi.Tick += new System.EventHandler(this.tmrWiFi_Tick);
            // 
            // frmTransferTVE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.lblNumeroBus);
            this.Controls.Add(this.imgStatusCon);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.imgQRCon);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgADO);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmTransferTVE";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTransferTVE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTransferTVE_FormClosing);
            this.Load += new System.EventHandler(this.frmTransferTVE_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStatusCon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

    private System.Windows.Forms.PictureBox imgADO;
    public System.Windows.Forms.Label lblFecha;
    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label label1;
    private System.Windows.Forms.WebBrowser imgQRCon;
    private System.Windows.Forms.Timer tmrFecha;
    private System.Windows.Forms.PictureBox btnRegresar;
    private System.Windows.Forms.PictureBox imgStatusCon;
    private System.Windows.Forms.Timer tmrConexion;
    private System.Windows.Forms.Timer tmrStatus;
    private System.Windows.Forms.Label lblNumeroBus;
    private System.Windows.Forms.Timer tmrWiFi;
}
