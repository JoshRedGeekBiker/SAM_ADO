partial class frmCargadorDePautas
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Pauta1"}, 0, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "USB"}, 1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCargadorDePautas));
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.imgAdo = new System.Windows.Forms.PictureBox();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblPauta = new System.Windows.Forms.Label();
            this.lblTitPauta = new System.Windows.Forms.Label();
            this.lblNomPauta = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.PictureBox();
            this.BtnCancelar = new System.Windows.Forms.PictureBox();
            this.lblTitPauta1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tmrProgreso = new System.Windows.Forms.Timer(this.components);
            this.lblCopia = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancelar)).BeginInit();
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
            this.lblTitulo.Size = new System.Drawing.Size(316, 33);
            this.lblTitulo.TabIndex = 45;
            this.lblTitulo.Text = "Cargador De pautas";
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
            this.lblFecha.TabIndex = 43;
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
            this.imgAdo.TabIndex = 42;
            this.imgAdo.TabStop = false;
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.Transparent;
            this.btnRegresar.BackgroundImage = global::SMFE.Properties.Resources.BotonREGRESAR;
            this.btnRegresar.Location = new System.Drawing.Point(672, 448);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(106, 81);
            this.btnRegresar.TabIndex = 46;
            this.btnRegresar.TabStop = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // listView1
            // 
            this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.HideSelection = false;
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(22, 190);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(341, 298);
            this.listView1.TabIndex = 47;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "imgPauta.gif");
            this.imageList1.Images.SetKeyName(1, "imgUSB.gif");
            // 
            // lblPauta
            // 
            this.lblPauta.BackColor = System.Drawing.Color.Transparent;
            this.lblPauta.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPauta.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPauta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPauta.Location = new System.Drawing.Point(16, 142);
            this.lblPauta.Name = "lblPauta";
            this.lblPauta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPauta.Size = new System.Drawing.Size(347, 33);
            this.lblPauta.TabIndex = 48;
            this.lblPauta.Text = "Pautas en Disco:";
            // 
            // lblTitPauta
            // 
            this.lblTitPauta.BackColor = System.Drawing.Color.Transparent;
            this.lblTitPauta.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitPauta.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitPauta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitPauta.Location = new System.Drawing.Point(400, 151);
            this.lblTitPauta.Name = "lblTitPauta";
            this.lblTitPauta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitPauta.Size = new System.Drawing.Size(361, 41);
            this.lblTitPauta.TabIndex = 53;
            this.lblTitPauta.Text = "Pauta Seleccionada:";
            this.lblTitPauta.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNomPauta
            // 
            this.lblNomPauta.BackColor = System.Drawing.Color.Transparent;
            this.lblNomPauta.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNomPauta.Font = new System.Drawing.Font("Verdana", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNomPauta.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNomPauta.Location = new System.Drawing.Point(400, 201);
            this.lblNomPauta.Name = "lblNomPauta";
            this.lblNomPauta.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNomPauta.Size = new System.Drawing.Size(336, 41);
            this.lblNomPauta.TabIndex = 55;
            this.lblNomPauta.Text = "ADO - RL - MEY";
            this.lblNomPauta.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAceptar
            // 
            this.btnAceptar.BackColor = System.Drawing.Color.Transparent;
            this.btnAceptar.BackgroundImage = global::SMFE.Properties.Resources.AceptarSincronizar;
            this.btnAceptar.Location = new System.Drawing.Point(473, 315);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(90, 84);
            this.btnAceptar.TabIndex = 56;
            this.btnAceptar.TabStop = false;
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.BackColor = System.Drawing.Color.Transparent;
            this.BtnCancelar.BackgroundImage = global::SMFE.Properties.Resources.CancelarSincronizar;
            this.BtnCancelar.Location = new System.Drawing.Point(605, 315);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(90, 84);
            this.BtnCancelar.TabIndex = 57;
            this.BtnCancelar.TabStop = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // lblTitPauta1
            // 
            this.lblTitPauta1.BackColor = System.Drawing.Color.Transparent;
            this.lblTitPauta1.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitPauta1.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitPauta1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitPauta1.Location = new System.Drawing.Point(400, 256);
            this.lblTitPauta1.Name = "lblTitPauta1";
            this.lblTitPauta1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitPauta1.Size = new System.Drawing.Size(371, 41);
            this.lblTitPauta1.TabIndex = 58;
            this.lblTitPauta1.Text = "¿Desea ingresar esta pauta?";
            this.lblTitPauta1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(43, 506);
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(286, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 59;
            // 
            // tmrProgreso
            // 
            this.tmrProgreso.Tick += new System.EventHandler(this.tmrProgreso_Tick);
            // 
            // lblCopia
            // 
            this.lblCopia.BackColor = System.Drawing.Color.Transparent;
            this.lblCopia.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCopia.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopia.ForeColor = System.Drawing.Color.Red;
            this.lblCopia.Location = new System.Drawing.Point(335, 496);
            this.lblCopia.Name = "lblCopia";
            this.lblCopia.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCopia.Size = new System.Drawing.Size(331, 33);
            this.lblCopia.TabIndex = 60;
            this.lblCopia.Text = "Copiando Archivos...";
            // 
            // frmCargadorDePautas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.lblCopia);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblTitPauta1);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblNomPauta);
            this.Controls.Add(this.lblTitPauta);
            this.Controls.Add(this.lblPauta);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgAdo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmCargadorDePautas";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CargadorDePautas";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCargadorDePautas_FormClosing);
            this.Load += new System.EventHandler(this.CargadorDePautas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAceptar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancelar)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    public System.Windows.Forms.Label lblTitulo;
    public System.Windows.Forms.Label lblFecha;
    private System.Windows.Forms.PictureBox imgAdo;
    private System.Windows.Forms.PictureBox btnRegresar;
    private System.Windows.Forms.Timer tmrFecha;
    private System.Windows.Forms.ListView listView1;
    private System.Windows.Forms.ImageList imageList1;
    public System.Windows.Forms.Label lblPauta;
    public System.Windows.Forms.Label lblTitPauta;
    public System.Windows.Forms.Label lblNomPauta;
    private System.Windows.Forms.PictureBox btnAceptar;
    private System.Windows.Forms.PictureBox BtnCancelar;
    public System.Windows.Forms.Label lblTitPauta1;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Timer tmrProgreso;
    public System.Windows.Forms.Label lblCopia;
}