partial class frmDatosTelemetria
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
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnRegresar = new System.Windows.Forms.PictureBox();
            this.lblTitulo1 = new System.Windows.Forms.Label();
            this.lblTitulo2 = new System.Windows.Forms.Label();
            this.lblCodigosEnviados = new System.Windows.Forms.Label();
            this.lblUltLoteTit = new System.Windows.Forms.Label();
            this.lblTitLote = new System.Windows.Forms.Label();
            this.lblFallasEnviadas = new System.Windows.Forms.Label();
            this.lblUltFallaTit = new System.Windows.Forms.Label();
            this.lblFallaMod = new System.Windows.Forms.Label();
            this.lblCodigos = new System.Windows.Forms.Label();
            this.lblFallas = new System.Windows.Forms.Label();
            this.lblNomLote = new System.Windows.Forms.Label();
            this.lblFallaCod = new System.Windows.Forms.Label();
            this.tmrFecha = new System.Windows.Forms.Timer(this.components);
            this.lblUltLote = new System.Windows.Forms.Label();
            this.lblUltFalla = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).BeginInit();
            this.SuspendLayout();
            // 
            // imgAdo
            // 
            this.imgAdo.BackColor = System.Drawing.Color.Transparent;
            this.imgAdo.Image = global::SMFE.Properties.Resources.LogoADO;
            this.imgAdo.Location = new System.Drawing.Point(9, 16);
            this.imgAdo.Name = "imgAdo";
            this.imgAdo.Size = new System.Drawing.Size(119, 35);
            this.imgAdo.TabIndex = 1;
            this.imgAdo.TabStop = false;
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
            this.lblVersion.TabIndex = 36;
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
            this.lblFecha.TabIndex = 35;
            this.lblFecha.Text = "dd/MM/yyyy HH:MM";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(231, 80);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(307, 33);
            this.lblTitulo.TabIndex = 37;
            this.lblTitulo.Text = "SIIAB TELEMATICS";
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
            // lblTitulo1
            // 
            this.lblTitulo1.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo1.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo1.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitulo1.Location = new System.Drawing.Point(22, 150);
            this.lblTitulo1.Name = "lblTitulo1";
            this.lblTitulo1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo1.Size = new System.Drawing.Size(361, 41);
            this.lblTitulo1.TabIndex = 44;
            this.lblTitulo1.Text = "Datos Motor";
            // 
            // lblTitulo2
            // 
            this.lblTitulo2.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo2.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitulo2.Location = new System.Drawing.Point(417, 150);
            this.lblTitulo2.Name = "lblTitulo2";
            this.lblTitulo2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo2.Size = new System.Drawing.Size(361, 41);
            this.lblTitulo2.TabIndex = 45;
            this.lblTitulo2.Text = "Codigos de Fallas";
            // 
            // lblCodigosEnviados
            // 
            this.lblCodigosEnviados.BackColor = System.Drawing.Color.Transparent;
            this.lblCodigosEnviados.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCodigosEnviados.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigosEnviados.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCodigosEnviados.Location = new System.Drawing.Point(22, 268);
            this.lblCodigosEnviados.Name = "lblCodigosEnviados";
            this.lblCodigosEnviados.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCodigosEnviados.Size = new System.Drawing.Size(361, 41);
            this.lblCodigosEnviados.TabIndex = 46;
            this.lblCodigosEnviados.Text = "Enviados:";
            // 
            // lblUltLoteTit
            // 
            this.lblUltLoteTit.BackColor = System.Drawing.Color.Transparent;
            this.lblUltLoteTit.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblUltLoteTit.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUltLoteTit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUltLoteTit.Location = new System.Drawing.Point(22, 331);
            this.lblUltLoteTit.Name = "lblUltLoteTit";
            this.lblUltLoteTit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblUltLoteTit.Size = new System.Drawing.Size(361, 41);
            this.lblUltLoteTit.TabIndex = 47;
            this.lblUltLoteTit.Text = "Último Lote Enviado:";
            // 
            // lblTitLote
            // 
            this.lblTitLote.BackColor = System.Drawing.Color.Transparent;
            this.lblTitLote.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitLote.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitLote.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitLote.Location = new System.Drawing.Point(22, 418);
            this.lblTitLote.Name = "lblTitLote";
            this.lblTitLote.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitLote.Size = new System.Drawing.Size(361, 41);
            this.lblTitLote.TabIndex = 48;
            this.lblTitLote.Text = "Nombre:";
            // 
            // lblFallasEnviadas
            // 
            this.lblFallasEnviadas.BackColor = System.Drawing.Color.Transparent;
            this.lblFallasEnviadas.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFallasEnviadas.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFallasEnviadas.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFallasEnviadas.Location = new System.Drawing.Point(417, 268);
            this.lblFallasEnviadas.Name = "lblFallasEnviadas";
            this.lblFallasEnviadas.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFallasEnviadas.Size = new System.Drawing.Size(361, 41);
            this.lblFallasEnviadas.TabIndex = 49;
            this.lblFallasEnviadas.Text = "Enviadas:";
            // 
            // lblUltFallaTit
            // 
            this.lblUltFallaTit.BackColor = System.Drawing.Color.Transparent;
            this.lblUltFallaTit.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblUltFallaTit.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUltFallaTit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUltFallaTit.Location = new System.Drawing.Point(417, 331);
            this.lblUltFallaTit.Name = "lblUltFallaTit";
            this.lblUltFallaTit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblUltFallaTit.Size = new System.Drawing.Size(361, 41);
            this.lblUltFallaTit.TabIndex = 50;
            this.lblUltFallaTit.Text = "Última Falla Enviada:";
            // 
            // lblFallaMod
            // 
            this.lblFallaMod.BackColor = System.Drawing.Color.Transparent;
            this.lblFallaMod.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFallaMod.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFallaMod.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFallaMod.Location = new System.Drawing.Point(416, 418);
            this.lblFallaMod.Name = "lblFallaMod";
            this.lblFallaMod.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFallaMod.Size = new System.Drawing.Size(361, 41);
            this.lblFallaMod.TabIndex = 51;
            this.lblFallaMod.Text = "Módulo:";
            // 
            // lblCodigos
            // 
            this.lblCodigos.BackColor = System.Drawing.Color.Transparent;
            this.lblCodigos.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCodigos.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigos.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCodigos.Location = new System.Drawing.Point(22, 215);
            this.lblCodigos.Name = "lblCodigos";
            this.lblCodigos.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCodigos.Size = new System.Drawing.Size(361, 41);
            this.lblCodigos.TabIndex = 52;
            this.lblCodigos.Text = "Recibidos:";
            // 
            // lblFallas
            // 
            this.lblFallas.BackColor = System.Drawing.Color.Transparent;
            this.lblFallas.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFallas.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFallas.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFallas.Location = new System.Drawing.Point(417, 215);
            this.lblFallas.Name = "lblFallas";
            this.lblFallas.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFallas.Size = new System.Drawing.Size(361, 41);
            this.lblFallas.TabIndex = 53;
            this.lblFallas.Text = "Recibidas:";
            // 
            // lblNomLote
            // 
            this.lblNomLote.BackColor = System.Drawing.Color.Transparent;
            this.lblNomLote.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNomLote.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNomLote.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNomLote.Location = new System.Drawing.Point(22, 459);
            this.lblNomLote.Name = "lblNomLote";
            this.lblNomLote.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNomLote.Size = new System.Drawing.Size(361, 41);
            this.lblNomLote.TabIndex = 54;
            this.lblNomLote.Text = "10737-30-10-93_0.zip";
            // 
            // lblFallaCod
            // 
            this.lblFallaCod.BackColor = System.Drawing.Color.Transparent;
            this.lblFallaCod.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFallaCod.Font = new System.Drawing.Font("Verdana", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFallaCod.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFallaCod.Location = new System.Drawing.Point(417, 459);
            this.lblFallaCod.Name = "lblFallaCod";
            this.lblFallaCod.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblFallaCod.Size = new System.Drawing.Size(361, 41);
            this.lblFallaCod.TabIndex = 55;
            this.lblFallaCod.Text = "Código:";
            // 
            // tmrFecha
            // 
            this.tmrFecha.Enabled = true;
            this.tmrFecha.Interval = 1000;
            this.tmrFecha.Tick += new System.EventHandler(this.tmrFecha_Tick);
            // 
            // lblUltLote
            // 
            this.lblUltLote.BackColor = System.Drawing.Color.Transparent;
            this.lblUltLote.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblUltLote.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUltLote.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUltLote.Location = new System.Drawing.Point(25, 372);
            this.lblUltLote.Name = "lblUltLote";
            this.lblUltLote.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblUltLote.Size = new System.Drawing.Size(361, 41);
            this.lblUltLote.TabIndex = 56;
            this.lblUltLote.Text = "0000-00-00 00:00:00";
            // 
            // lblUltFalla
            // 
            this.lblUltFalla.BackColor = System.Drawing.Color.Transparent;
            this.lblUltFalla.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblUltFalla.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUltFalla.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUltFalla.Location = new System.Drawing.Point(420, 372);
            this.lblUltFalla.Name = "lblUltFalla";
            this.lblUltFalla.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblUltFalla.Size = new System.Drawing.Size(361, 41);
            this.lblUltFalla.TabIndex = 57;
            this.lblUltFalla.Text = "0000-00-00 00:00:00";
            // 
            // frmDatosTelemetria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.FondoCONblanco;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.lblUltFalla);
            this.Controls.Add(this.lblUltLote);
            this.Controls.Add(this.btnRegresar);
            this.Controls.Add(this.lblFallaCod);
            this.Controls.Add(this.lblNomLote);
            this.Controls.Add(this.lblFallas);
            this.Controls.Add(this.lblCodigos);
            this.Controls.Add(this.lblFallaMod);
            this.Controls.Add(this.lblUltFallaTit);
            this.Controls.Add(this.lblFallasEnviadas);
            this.Controls.Add(this.lblTitLote);
            this.Controls.Add(this.lblUltLoteTit);
            this.Controls.Add(this.lblCodigosEnviados);
            this.Controls.Add(this.lblTitulo2);
            this.Controls.Add(this.lblTitulo1);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.imgAdo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(816, 640);
            this.MinimumSize = new System.Drawing.Size(816, 640);
            this.Name = "frmDatosTelemetria";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmDatosTelemetria";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDatosTelemetria_FormClosing);
            this.Load += new System.EventHandler(this.frmDatosTelemetria_Load);
            this.Click += new System.EventHandler(this.frmDatosTelemetria_Click);
            ((System.ComponentModel.ISupportInitialize)(this.imgAdo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnRegresar)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox imgAdo;
    public System.Windows.Forms.Label lblVersion;
    public System.Windows.Forms.Label lblFecha;
    public System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.PictureBox btnRegresar;
    public System.Windows.Forms.Label lblTitulo1;
    public System.Windows.Forms.Label lblTitulo2;
    public System.Windows.Forms.Label lblCodigosEnviados;
    public System.Windows.Forms.Label lblUltLoteTit;
    public System.Windows.Forms.Label lblTitLote;
    public System.Windows.Forms.Label lblFallasEnviadas;
    public System.Windows.Forms.Label lblUltFallaTit;
    public System.Windows.Forms.Label lblFallaMod;
    public System.Windows.Forms.Label lblCodigos;
    public System.Windows.Forms.Label lblFallas;
    public System.Windows.Forms.Label lblNomLote;
    public System.Windows.Forms.Label lblFallaCod;
    private System.Windows.Forms.Timer tmrFecha;
    public System.Windows.Forms.Label lblUltLote;
    public System.Windows.Forms.Label lblUltFalla;
}