
    partial class frmReproductor
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
        private void InitializeComponent(string rutaVLC)
        {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReproductor));
        this.vlcControl1 = new Vlc.DotNet.Forms.VlcControl();
        this.Mp3 = new Vlc.DotNet.Forms.VlcControl();
        this.BackImage = new System.Windows.Forms.Panel();
        this.img_PISIA = new System.Windows.Forms.PictureBox();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.pnl_Cintillo = new System.Windows.Forms.Panel();
        this.lbl_Cintillo = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).BeginInit();
        this.BackImage.SuspendLayout();
        this.pnl_Cintillo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.img_PISIA)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.Mp3)).BeginInit();
        this.SuspendLayout();
        // 
        // vlcControl1
        // 
        this.vlcControl1.BackColor = System.Drawing.Color.Black;
        //this.vlcControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.vlcControl1.Location = new System.Drawing.Point(0, 0);
        this.vlcControl1.Name = "vlcControl1";
        this.vlcControl1.Size = new System.Drawing.Size(784, 561);
        this.vlcControl1.Spu = -1;
        this.vlcControl1.TabIndex = 0;
        this.vlcControl1.VlcLibDirectory = new System.IO.DirectoryInfo(@rutaVLC);
        this.vlcControl1.VlcMediaplayerOptions = null;
        this.vlcControl1.Rate = 24.0f;
        //
        // Mp3
        //
        this.Mp3.VlcLibDirectory = new System.IO.DirectoryInfo(@rutaVLC);
        this.Mp3.TabIndex = 0;
        this.Mp3.Name = "Mp3";
        
        // 
        // BackImage
        // 
        this.BackImage.BackColor = System.Drawing.Color.Transparent;
        this.BackImage.Controls.Add(this.pnl_Cintillo);
        this.BackImage.Controls.Add(this.img_PISIA);
        this.BackImage.Controls.Add(this.pictureBox1);
        //this.BackImage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.BackImage.Location = new System.Drawing.Point(0, 0);
        this.BackImage.Name = "BackImage";
        this.BackImage.Size = new System.Drawing.Size(784, 561);
        this.BackImage.TabIndex = 1;
        // 
        // pictureBox1
        // 
        //this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
        this.pictureBox1.Image = global::SMFE.Properties.Resources.Logo;
        this.pictureBox1.Location = new System.Drawing.Point(0, 0);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(784, 561);
        this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.pictureBox1.TabIndex = 0;
        this.pictureBox1.TabStop = false;
        // 
        // pnl_Cintillo
        // 
        this.pnl_Cintillo.Controls.Add(this.lbl_Cintillo);
        this.pnl_Cintillo.Location = new System.Drawing.Point(0, 0);
        this.pnl_Cintillo.Name = "pnl_Cintillo";
        this.pnl_Cintillo.Size = new System.Drawing.Size(1, 1);
        this.pnl_Cintillo.TabIndex = 0;
        
        // 
        // lbl_Cintillo
        // 
        //this.lbl_Cintillo.AutoSize = true;
        this.lbl_Cintillo.Location = new System.Drawing.Point(0, 0);
        this.lbl_Cintillo.Name = "lbl_Cintillo";
        this.lbl_Cintillo.Size = new System.Drawing.Size(1, 1);
        this.lbl_Cintillo.TabIndex = 0;
        this.lbl_Cintillo.Text = "";
        this.lbl_Cintillo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.lbl_Cintillo.Margin = new System.Windows.Forms.Padding(0);
        // 
        // img_PISIA
        // 
        this.img_PISIA.BackColor = System.Drawing.Color.Transparent;
        this.img_PISIA.Location = new System.Drawing.Point(508, 74);
        this.img_PISIA.Name = "img_PISIA";
        this.img_PISIA.Size = new System.Drawing.Size(297, 456);
        this.img_PISIA.TabIndex = 1;
        this.img_PISIA.TabStop = false;
        this.img_PISIA.Visible = false;
        // 
        // frmReproductor
        // 
        this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(784, 561);
        this.Controls.Add(this.BackImage);
        this.Controls.Add(this.vlcControl1);
        this.Controls.Add(this.Mp3);
        //this.Location = new System.Drawing.Point(801, 0);
        this.MaximizeBox = false;
        this.MinimumSize = new System.Drawing.Size(800, 600);
        this.Name = "frmReproductor";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "frmReproductor";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmReproductor_FormClosing);
        ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).EndInit();
        this.BackImage.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.img_PISIA)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.Mp3)).EndInit();
        this.pnl_Cintillo.ResumeLayout(false);
        this.pnl_Cintillo.PerformLayout();
        this.ResumeLayout(false);
    }

    /// <summary>
    /// Se ocupa para modo condiguración
    /// </summary>
    private void InitializeComponent()
    {
            this.BackImage = new System.Windows.Forms.Panel();
            this.img_PISIA = new System.Windows.Forms.PictureBox();
            this.pnl_Cintillo = new System.Windows.Forms.Panel();
            this.lbl_Cintillo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BackImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.img_PISIA)).BeginInit();
            this.pnl_Cintillo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BackImage
            // 
            this.BackImage.BackColor = System.Drawing.Color.Transparent;
            this.BackImage.Controls.Add(this.img_PISIA);
            this.BackImage.Controls.Add(this.pnl_Cintillo);
            this.BackImage.Controls.Add(this.pictureBox1);
            this.BackImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackImage.Location = new System.Drawing.Point(0, 0);
            this.BackImage.Name = "BackImage";
            this.BackImage.Size = new System.Drawing.Size(784, 561);
            this.BackImage.TabIndex = 1;
            // 
            // img_PISIA
            // 
            this.img_PISIA.BackColor = System.Drawing.Color.Transparent;
            this.img_PISIA.InitialImage = null;
            this.img_PISIA.Location = new System.Drawing.Point(454, 57);
            this.img_PISIA.Name = "img_PISIA";
            this.img_PISIA.Size = new System.Drawing.Size(297, 456);
            this.img_PISIA.TabIndex = 1;
            this.img_PISIA.TabStop = false;
            this.img_PISIA.Visible = false;
            // 
            // pnl_Cintillo
            // 
            this.pnl_Cintillo.Controls.Add(this.lbl_Cintillo);
            this.pnl_Cintillo.Location = new System.Drawing.Point(0, 519);
            this.pnl_Cintillo.Name = "pnl_Cintillo";
            this.pnl_Cintillo.Size = new System.Drawing.Size(784, 34);
            this.pnl_Cintillo.TabIndex = 2;
            // 
            // lbl_Cintillo
            // 
            this.lbl_Cintillo.AutoSize = true;
            this.lbl_Cintillo.Location = new System.Drawing.Point(27, 9);
            this.lbl_Cintillo.Margin = new System.Windows.Forms.Padding(0);
            this.lbl_Cintillo.Name = "lbl_Cintillo";
            this.lbl_Cintillo.Size = new System.Drawing.Size(86, 13);
            this.lbl_Cintillo.TabIndex = 0;
            this.lbl_Cintillo.Text = "Texto Del Cintillo";
            this.lbl_Cintillo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::SMFE.Properties.Resources.Logo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(784, 561);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // frmReproductor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.BackImage);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmReproductor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmReproductor";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmReproductor_FormClosing);
            this.Load += new System.EventHandler(this.frmReproductor_Load);
            this.BackImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.img_PISIA)).EndInit();
            this.pnl_Cintillo.ResumeLayout(false);
            this.pnl_Cintillo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    public Vlc.DotNet.Forms.VlcControl vlcControl1;
    public Vlc.DotNet.Forms.VlcControl Mp3;
    public System.Windows.Forms.Panel BackImage;
    public System.Windows.Forms.PictureBox pictureBox1;
    public System.Windows.Forms.PictureBox img_PISIA;
    public System.Windows.Forms.Panel pnl_Cintillo;
    public System.Windows.Forms.Label lbl_Cintillo;
}
