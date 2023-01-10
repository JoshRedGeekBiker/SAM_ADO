partial class frmEnviandoMensaje
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.PictureBox();
            this.imgCorrecto = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCorrecto)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(12, 18);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(239, 29);
            this.lblTitulo.TabIndex = 13;
            this.lblTitulo.Text = "Mensaje Enviado";
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrar.BackgroundImage = global::SMFE.Properties.Resources.BotonCERRAR;
            this.btnCerrar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCerrar.Location = new System.Drawing.Point(258, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(60, 59);
            this.btnCerrar.TabIndex = 14;
            this.btnCerrar.TabStop = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // imgCorrecto
            // 
            this.imgCorrecto.BackColor = System.Drawing.Color.Transparent;
            this.imgCorrecto.BackgroundImage = global::SMFE.Properties.Resources.ConductorVALIDO;
            this.imgCorrecto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imgCorrecto.Location = new System.Drawing.Point(115, 77);
            this.imgCorrecto.Name = "imgCorrecto";
            this.imgCorrecto.Size = new System.Drawing.Size(85, 81);
            this.imgCorrecto.TabIndex = 15;
            this.imgCorrecto.TabStop = false;
            // 
            // frmEnviandoMensaje
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.EncuadreENVIANDOMENSAJE;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(325, 190);
            this.Controls.Add(this.imgCorrecto);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(341, 229);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(341, 229);
            this.Name = "frmEnviandoMensaje";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "frmEnviandoMensaje";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEnviandoMensaje_FormClosing);
            this.Load += new System.EventHandler(this.frmEnviandoMensaje_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnCerrar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCorrecto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.PictureBox btnCerrar;
    private System.Windows.Forms.PictureBox imgCorrecto;
}