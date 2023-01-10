
    partial class frmPopUp
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
            this.btn_Cerrar = new System.Windows.Forms.PictureBox();
            this.lblTercero = new System.Windows.Forms.Label();
            this.lblSegundo = new System.Windows.Forms.Label();
            this.lblPrimer = new System.Windows.Forms.Label();
            this.tmrActualiza = new System.Windows.Forms.Timer(this.components);
            this.tmrSiempreAdelante = new System.Windows.Forms.Timer(this.components);
            this.tmrCerrar = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.btn_Cerrar)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitulo.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblTitulo.Location = new System.Drawing.Point(16, 8);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitulo.Size = new System.Drawing.Size(161, 33);
            this.lblTitulo.TabIndex = 4;
            this.lblTitulo.Text = "Datos GPS";
            // 
            // btn_Cerrar
            // 
            this.btn_Cerrar.BackColor = System.Drawing.Color.Transparent;
            this.btn_Cerrar.BackgroundImage = global::SMFE.Properties.Resources.BotonCERRAR;
            this.btn_Cerrar.Location = new System.Drawing.Point(392, 48);
            this.btn_Cerrar.Name = "btn_Cerrar";
            this.btn_Cerrar.Size = new System.Drawing.Size(76, 76);
            this.btn_Cerrar.TabIndex = 5;
            this.btn_Cerrar.TabStop = false;
            this.btn_Cerrar.Click += new System.EventHandler(this.btn_Cerrar_Click);
            // 
            // lblTercero
            // 
            this.lblTercero.BackColor = System.Drawing.Color.Transparent;
            this.lblTercero.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTercero.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTercero.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTercero.Location = new System.Drawing.Point(25, 152);
            this.lblTercero.Name = "lblTercero";
            this.lblTercero.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTercero.Size = new System.Drawing.Size(452, 33);
            this.lblTercero.TabIndex = 8;
            this.lblTercero.Text = "Vel.: 0 Km/h";
            // 
            // lblSegundo
            // 
            this.lblSegundo.BackColor = System.Drawing.Color.Transparent;
            this.lblSegundo.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSegundo.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSegundo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSegundo.Location = new System.Drawing.Point(25, 96);
            this.lblSegundo.Name = "lblSegundo";
            this.lblSegundo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSegundo.Size = new System.Drawing.Size(452, 41);
            this.lblSegundo.TabIndex = 7;
            this.lblSegundo.Text = "Long.:()";
            // 
            // lblPrimer
            // 
            this.lblPrimer.BackColor = System.Drawing.Color.Transparent;
            this.lblPrimer.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPrimer.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrimer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPrimer.Location = new System.Drawing.Point(25, 48);
            this.lblPrimer.Name = "lblPrimer";
            this.lblPrimer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPrimer.Size = new System.Drawing.Size(361, 41);
            this.lblPrimer.TabIndex = 6;
            this.lblPrimer.Text = "Lat.: ()";
            // 
            // tmrActualiza
            // 
            this.tmrActualiza.Interval = 1000;
            this.tmrActualiza.Tick += new System.EventHandler(this.tmrActualiza_Tick);
            // 
            // tmrSiempreAdelante
            // 
            this.tmrSiempreAdelante.Tick += new System.EventHandler(this.tmrSiempreAdelante_Tick);
            // 
            // tmrCerrar
            // 
            this.tmrCerrar.Tick += new System.EventHandler(this.tmrCerrar_Tick);
            // 
            // frmPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SMFE.Properties.Resources.EncuadreAVISOS;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(504, 234);
            this.Controls.Add(this.btn_Cerrar);
            this.Controls.Add(this.lblTercero);
            this.Controls.Add(this.lblSegundo);
            this.Controls.Add(this.lblPrimer);
            this.Controls.Add(this.lblTitulo);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(520, 273);
            this.MinimumSize = new System.Drawing.Size(520, 273);
            this.Name = "frmPopUp";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPopUp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPopUp_FormClosing);
            this.Load += new System.EventHandler(this.frmPopUp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btn_Cerrar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.PictureBox btn_Cerrar;
        public System.Windows.Forms.Label lblTercero;
        public System.Windows.Forms.Label lblSegundo;
        public System.Windows.Forms.Label lblPrimer;
    private System.Windows.Forms.Timer tmrActualiza;
    private System.Windows.Forms.Timer tmrSiempreAdelante;
    private System.Windows.Forms.Timer tmrCerrar;
}
