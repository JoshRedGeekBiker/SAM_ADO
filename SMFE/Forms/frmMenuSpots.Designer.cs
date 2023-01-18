
    partial class frmMenuSpots
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
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.imgAdo = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnVideo = new System.Windows.Forms.PictureBox();
            this.btnAudio = new System.Windows.Forms.PictureBox();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAudio)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 54;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(231, 83);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(307, 33);
            this.lblTitulo.TabIndex = 53;
            this.lblTitulo.Text = "Menú Spots";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(540, 94);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblVersion.Size = new System.Drawing.Size(249, 25);
            this.lblVersion.TabIndex = 52;
            this.lblVersion.Text = "Version";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblFecha
            // 
            this.lblFecha.BackColor = System.Drawing.Color.Transparent;
            this.lblFecha.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFecha.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.ForeColor = System.Drawing.Color.White;
            this.lblFecha.Location = new System.Drawing.Point(436, 6);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFecha.Size = new System.Drawing.Size(361, 41);
            this.lblFecha.TabIndex = 51;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // imgAdo
            // 
            this.imgAdo.BackColor = System.Drawing.Color.Transparent;
            this.imgAdo.Image = global::SMFE.Properties.Resources.LogoADO;
            this.imgAdo.Location = new System.Drawing.Point(5, 11);
            this.imgAdo.Name = "imgAdo";
            this.imgAdo.Size = new System.Drawing.Size(119, 35);
            this.imgAdo.TabIndex = 50;
            this.imgAdo.TabStop = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(340, 41);
            this.label2.TabIndex = 43;
            this.label2.Text = "Menú spots";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btnVideo);
            this.groupBox1.Controls.Add(this.btnAudio);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(22, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 319);
            this.groupBox1.TabIndex = 55;
            this.groupBox1.TabStop = false;
            // 
            // btnVideo
            // 
            this.btnVideo.BackColor = System.Drawing.Color.Transparent;
            this.btnVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnVideo.Image = global::SMFE.Properties.Resources.btl_videos;
            this.btnVideo.Location = new System.Drawing.Point(20, 153);
            this.btnVideo.Name = "btnVideo";
            this.btnVideo.Size = new System.Drawing.Size(360, 55);
            this.btnVideo.TabIndex = 46;
            this.btnVideo.TabStop = false;
            this.btnVideo.Click += new System.EventHandler(this.btnVideo_Click);
            // 
            // btnAudio
            // 
            this.btnAudio.BackColor = System.Drawing.Color.Transparent;
            this.btnAudio.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAudio.Image = global::SMFE.Properties.Resources.btl_audios;
            this.btnAudio.Location = new System.Drawing.Point(20, 72);
            this.btnAudio.Name = "btnAudio";
            this.btnAudio.Size = new System.Drawing.Size(360, 55);
            this.btnAudio.TabIndex = 45;
            this.btnAudio.TabStop = false;
            this.btnAudio.Click += new System.EventHandler(this.btnAudio_Click);
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // frmMenuSpots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgAdo);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmMenuSpots";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMenuSpots";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMenuSpots_FormClosing);
            this.Load += new System.EventHandler(this.frmMenuSpots_Load);
            this.Click += new System.EventHandler(this.frmMenuSpots_Click);
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAudio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox btnRegresar;
        public System.Windows.Forms.Label lblTitulo;
        public System.Windows.Forms.Label lblVersion;
        public System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.PictureBox imgAdo;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox btnVideo;
        private System.Windows.Forms.PictureBox btnAudio;
    private System.Windows.Forms.Timer tmrFecha;
}
