partial class frmCarga
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
            this.tmrSiempreAdelante = new System.Windows.Forms.Timer(this.components);
            this.cargando = new System.Windows.Forms.PictureBox();
            this.tmrCerrar = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.cargando)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrSiempreAdelante
            // 
            this.tmrSiempreAdelante.Enabled = true;
            this.tmrSiempreAdelante.Tick += new System.EventHandler(this.tmrSiempreAdelante_Tick);
            // 
            // cargando
            // 
            this.cargando.Image = global::SMFE.Properties.Resources.loading_SIIAB;
            this.cargando.Location = new System.Drawing.Point(0, 0);
            this.cargando.Name = "cargando";
            this.cargando.Size = new System.Drawing.Size(800, 600);
            this.cargando.TabIndex = 0;
            this.cargando.TabStop = false;
            // 
            // tmrCerrar
            // 
            this.tmrCerrar.Interval = 1000;
            this.tmrCerrar.Tick += new System.EventHandler(this.tmrCerrar_Tick);
            // 
            // frmCarga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.cargando);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCarga";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCarga";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmCarga_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cargando)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer tmrSiempreAdelante;
    private System.Windows.Forms.PictureBox cargando;
    private System.Windows.Forms.Timer tmrCerrar;
}