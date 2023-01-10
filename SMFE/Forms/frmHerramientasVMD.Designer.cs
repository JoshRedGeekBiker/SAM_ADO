partial class frmHerramientasVMD
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.imgAdo = new System.Windows.Forms.PictureBox();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.gpBox1 = new System.Windows.Forms.GroupBox();
            this.btnUSB = new System.Windows.Forms.PictureBox();
            this.btnDiscoPel = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            this.gpBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnUSB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDiscoPel)).BeginInit();
            this.SuspendLayout();
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
            this.lblTitulo.TabIndex = 41;
            this.lblTitulo.Text = "Herramientas VMD";
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
            this.lblVersion.TabIndex = 40;
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
            this.lblFecha.TabIndex = 39;
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
            this.imgAdo.TabIndex = 38;
            this.imgAdo.TabStop = false;
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 43;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // gpBox1
            // 
            this.gpBox1.BackColor = System.Drawing.Color.Transparent;
            this.gpBox1.Controls.Add(this.btnUSB);
            this.gpBox1.Controls.Add(this.btnDiscoPel);
            this.gpBox1.Controls.Add(this.label1);
            this.gpBox1.Location = new System.Drawing.Point(12, 143);
            this.gpBox1.Name = "gpBox1";
            this.gpBox1.Size = new System.Drawing.Size(340, 224);
            this.gpBox1.TabIndex = 44;
            this.gpBox1.TabStop = false;
            // 
            // btnUSB
            // 
            this.btnUSB.BackColor = System.Drawing.Color.Transparent;
            this.btnUSB.BackgroundImage = global::SMFE.Properties.Resources.btn_medioextraibleusb;
            this.btnUSB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnUSB.Location = new System.Drawing.Point(64, 150);
            this.btnUSB.Name = "btnUSB";
            this.btnUSB.Size = new System.Drawing.Size(212, 55);
            this.btnUSB.TabIndex = 45;
            this.btnUSB.TabStop = false;
            this.btnUSB.Click += new System.EventHandler(this.btnUSB_Click);
            // 
            // btnDiscoPel
            // 
            this.btnDiscoPel.BackColor = System.Drawing.Color.Transparent;
            this.btnDiscoPel.BackgroundImage = global::SMFE.Properties.Resources.btn_discopeliculas;
            this.btnDiscoPel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDiscoPel.Location = new System.Drawing.Point(64, 72);
            this.btnDiscoPel.Name = "btnDiscoPel";
            this.btnDiscoPel.Size = new System.Drawing.Size(212, 55);
            this.btnDiscoPel.TabIndex = 44;
            this.btnDiscoPel.TabStop = false;
            this.btnDiscoPel.Click += new System.EventHandler(this.btnDiscoPel_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(340, 41);
            this.label1.TabIndex = 43;
            this.label1.Text = "Cargar Pauta Desde:";
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // frmHerramientasVMD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.gpBox1);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgAdo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmHerramientasVMD";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmHerramientasVMD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmHerramientasVMD_FormClosing);
            this.Load += new System.EventHandler(this.frmHerramientasVMD_Load);
            this.Click += new System.EventHandler(this.frmHerramientasVMD_Click);
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            this.gpBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnUSB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDiscoPel)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label lblVersion;
    public System.Windows.Forms.Label lblFecha;
    private System.Windows.Forms.PictureBox imgAdo;
    private System.Windows.Forms.PictureBox btnRegresar;
    private System.Windows.Forms.GroupBox gpBox1;
    public System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox btnDiscoPel;
    private System.Windows.Forms.PictureBox btnUSB;
    private System.Windows.Forms.Timer tmrFecha;
}