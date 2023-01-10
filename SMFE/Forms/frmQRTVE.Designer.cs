
    partial class frmQRTVE
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
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.imgADO = new System.Windows.Forms.PictureBox();
            this.btnInspector = new System.Windows.Forms.PictureBox();
            this.btnBitacora = new System.Windows.Forms.PictureBox();
            this.imgQR = new System.Windows.Forms.WebBrowser();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnInspector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBitacora)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
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
            this.lblTitulo.TabIndex = 52;
            this.lblTitulo.Text = "Tarjeta De Viaje";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
            this.lblFecha.TabIndex = 51;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // imgADO
            // 
            this.imgADO.BackColor = System.Drawing.Color.Transparent;
            this.imgADO.Image = global::SMFE.Properties.Resources.LogoADO;
            this.imgADO.Location = new System.Drawing.Point(9, 16);
            this.imgADO.Name = "imgADO";
            this.imgADO.Size = new System.Drawing.Size(119, 35);
            this.imgADO.TabIndex = 50;
            this.imgADO.TabStop = false;
            this.imgADO.Click += new System.EventHandler(this.imgADO_Click);
            // 
            // btnInspector
            // 
            this.btnInspector.BackColor = System.Drawing.Color.Transparent;
            this.btnInspector.BackgroundImage = global::SMFE.Properties.Resources.btl_conductorinspector;
            this.btnInspector.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnInspector.Location = new System.Drawing.Point(163, 182);
            this.btnInspector.Name = "btnInspector";
            this.btnInspector.Size = new System.Drawing.Size(161, 36);
            this.btnInspector.TabIndex = 53;
            this.btnInspector.TabStop = false;
            this.btnInspector.Click += new System.EventHandler(this.btnInspector_Click);
            // 
            // btnBitacora
            // 
            this.btnBitacora.BackColor = System.Drawing.Color.Transparent;
            this.btnBitacora.BackgroundImage = global::SMFE.Properties.Resources.btl_bitacora;
            this.btnBitacora.Location = new System.Drawing.Point(494, 182);
            this.btnBitacora.Name = "btnBitacora";
            this.btnBitacora.Size = new System.Drawing.Size(161, 36);
            this.btnBitacora.TabIndex = 54;
            this.btnBitacora.TabStop = false;
            this.btnBitacora.Click += new System.EventHandler(this.btnBitacora_Click);
            // 
            // imgQR
            // 
            this.imgQR.Location = new System.Drawing.Point(263, 243);
            this.imgQR.MinimumSize = new System.Drawing.Size(20, 20);
            this.imgQR.Name = "imgQR";
            this.imgQR.ScrollBarsEnabled = false;
            this.imgQR.Size = new System.Drawing.Size(278, 279);
            this.imgQR.TabIndex = 55;
            this.imgQR.Visible = false;
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 56;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // frmQRTVE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.imgQR);
            this.Controls.Add(this.btnBitacora);
            this.Controls.Add(this.btnInspector);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgADO);
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmQRTVE";
            this.Opacity = 0D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmQRTVE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmQRTVE_FormClosing);
            this.Load += new System.EventHandler(this.frmQRTVE_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnInspector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBitacora)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            this.ResumeLayout(false);

        }

    #endregion

    private System.Windows.Forms.Timer tmrFecha;
    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label lblFecha;
    private System.Windows.Forms.PictureBox imgADO;
    private System.Windows.Forms.PictureBox btnInspector;
    private System.Windows.Forms.PictureBox btnBitacora;
    private System.Windows.Forms.WebBrowser imgQR;
    private System.Windows.Forms.PictureBox btnRegresar;
}
