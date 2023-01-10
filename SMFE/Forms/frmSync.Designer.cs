
    partial class frmSync
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
            this.Label2 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblTitulo2 = new System.Windows.Forms.Label();
            this.lblMensajeFinal = new System.Windows.Forms.Label();
            this.imgAceptar = new System.Windows.Forms.PictureBox();
            this.tmActualiza = new System.Windows.Forms.Timer(this.components);
            this.tmrApagado = new System.Windows.Forms.Timer(this.components);
            this.imgCancelar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgAceptar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCancelar)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(24, 8);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(171, 25);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Sincronizar";
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.Transparent;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(24, 48);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(369, 33);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "Acciones en proceso";
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLog.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLog.Location = new System.Drawing.Point(24, 72);
            this.txtLog.MaxLength = 0;
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtLog.Size = new System.Drawing.Size(425, 97);
            this.txtLog.TabIndex = 6;
            this.txtLog.WordWrap = false;
            // 
            // lblTitulo2
            // 
            this.lblTitulo2.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitulo2.Location = new System.Drawing.Point(24, 168);
            this.lblTitulo2.Name = "lblTitulo2";
            this.lblTitulo2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo2.Size = new System.Drawing.Size(409, 33);
            this.lblTitulo2.TabIndex = 7;
            this.lblTitulo2.Text = "Resultado de la sincronización";
            // 
            // lblMensajeFinal
            // 
            this.lblMensajeFinal.BackColor = System.Drawing.Color.Transparent;
            this.lblMensajeFinal.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblMensajeFinal.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensajeFinal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMensajeFinal.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblMensajeFinal.Location = new System.Drawing.Point(32, 200);
            this.lblMensajeFinal.Margin = new System.Windows.Forms.Padding(0);
            this.lblMensajeFinal.Name = "lblMensajeFinal";
            this.lblMensajeFinal.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMensajeFinal.Size = new System.Drawing.Size(481, 177);
            this.lblMensajeFinal.TabIndex = 8;
            // 
            // imgAceptar
            // 
            this.imgAceptar.BackColor = System.Drawing.Color.Transparent;
            this.imgAceptar.BackgroundImage = global::SMFE.Properties.Resources.AceptarSincronizar;
            this.imgAceptar.Location = new System.Drawing.Point(547, 72);
            this.imgAceptar.Name = "imgAceptar";
            this.imgAceptar.Size = new System.Drawing.Size(90, 84);
            this.imgAceptar.TabIndex = 9;
            this.imgAceptar.TabStop = false;
            this.imgAceptar.Click += new System.EventHandler(this.imgAceptar_Click);
            // 
            // tmActualiza
            // 
            this.tmActualiza.Enabled = true;
            this.tmActualiza.Tick += new System.EventHandler(this.tmActualiza_Tick);
            // 
            // tmrApagado
            // 
            this.tmrApagado.Interval = 1000;
            this.tmrApagado.Tick += new System.EventHandler(this.tmrApagado_Tick);
            // 
            // imgCancelar
            // 
            this.imgCancelar.BackColor = System.Drawing.Color.Transparent;
            this.imgCancelar.BackgroundImage = global::SMFE.Properties.Resources.CancelarSincronizar;
            this.imgCancelar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imgCancelar.Location = new System.Drawing.Point(455, 72);
            this.imgCancelar.Name = "imgCancelar";
            this.imgCancelar.Size = new System.Drawing.Size(90, 84);
            this.imgCancelar.TabIndex = 10;
            this.imgCancelar.TabStop = false;
            this.imgCancelar.Click += new System.EventHandler(this.imgCancelar_Click);
            // 
            // frmSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.EncuadreSincronizar;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(649, 402);
            this.Controls.Add(this.imgCancelar);
            this.Controls.Add(this.imgAceptar);
            this.Controls.Add(this.lblMensajeFinal);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblTitulo2);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(665, 441);
            this.MinimumSize = new System.Drawing.Size(665, 441);
            this.Name = "frmSync";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSync_FormClosing);
            this.Load += new System.EventHandler(this.frmSync_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgAceptar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgCancelar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblTitulo;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.TextBox txtLog;
        public System.Windows.Forms.Label lblTitulo2;
        public System.Windows.Forms.Label lblMensajeFinal;
        private System.Windows.Forms.PictureBox imgAceptar;
    public System.Windows.Forms.Timer tmActualiza;
    private System.Windows.Forms.Timer tmrApagado;
    private System.Windows.Forms.PictureBox imgCancelar;
}
