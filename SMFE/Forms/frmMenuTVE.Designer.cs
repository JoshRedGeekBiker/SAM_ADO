
    partial class frmMenuTVE
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
            this.lblFecha = new System.Windows.Forms.Label();
            this.imgADO = new System.Windows.Forms.PictureBox();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.btnOff = new System.Windows.Forms.PictureBox();
            this.btnTranfer = new System.Windows.Forms.PictureBox();
            this.btnConsulta = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTranfer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConsulta)).BeginInit();
            this.SuspendLayout();
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
            this.lblFecha.TabIndex = 27;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // imgADO
            // 
            this.imgADO.BackColor = System.Drawing.Color.Transparent;
            this.imgADO.BackgroundImage = global::SMFE.Properties.Resources.LogoADO;
            this.imgADO.Location = new System.Drawing.Point(9, 16);
            this.imgADO.Name = "imgADO";
            this.imgADO.Size = new System.Drawing.Size(119, 35);
            this.imgADO.TabIndex = 28;
            this.imgADO.TabStop = false;
            this.imgADO.Click += new System.EventHandler(this.imgADO_Click);
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 43;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // btnOff
            // 
            this.btnOff.BackColor = System.Drawing.Color.Transparent;
            this.btnOff.BackgroundImage = global::SMFE.Properties.Resources.btn_off;
            this.btnOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOff.Location = new System.Drawing.Point(32, 454);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(75, 75);
            this.btnOff.TabIndex = 44;
            this.btnOff.TabStop = false;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // btnTranfer
            // 
            this.btnTranfer.BackColor = System.Drawing.Color.Transparent;
            this.btnTranfer.BackgroundImage = global::SMFE.Properties.Resources.btl_transparencia;
            this.btnTranfer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnTranfer.Location = new System.Drawing.Point(224, 196);
            this.btnTranfer.Name = "btnTranfer";
            this.btnTranfer.Size = new System.Drawing.Size(352, 57);
            this.btnTranfer.TabIndex = 45;
            this.btnTranfer.TabStop = false;
            this.btnTranfer.Click += new System.EventHandler(this.btnTranfer_Click);
            // 
            // btnConsulta
            // 
            this.btnConsulta.BackColor = System.Drawing.Color.Transparent;
            this.btnConsulta.BackgroundImage = global::SMFE.Properties.Resources.btl_consulta;
            this.btnConsulta.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnConsulta.Location = new System.Drawing.Point(224, 334);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(352, 57);
            this.btnConsulta.TabIndex = 46;
            this.btnConsulta.TabStop = false;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
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
            this.lblTitulo.TabIndex = 47;
            this.lblTitulo.Text = "Tarjeta de Viaje";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // frmMenuTVE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.btnConsulta);
            this.Controls.Add(this.btnTranfer);
            this.Controls.Add(this.btnOff);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.imgADO);
            this.Controls.Add(this.lblFecha);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmMenuTVE";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMenuTVE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuTVE_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuTVE_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgADO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTranfer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnConsulta)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.PictureBox imgADO;
        private System.Windows.Forms.PictureBox btnRegresar;
        private System.Windows.Forms.PictureBox btnOff;
        private System.Windows.Forms.PictureBox btnTranfer;
        private System.Windows.Forms.PictureBox btnConsulta;
        public System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.Timer tmrFecha;
}
