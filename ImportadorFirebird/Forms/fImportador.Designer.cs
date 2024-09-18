namespace ImportadorFirebird
{
    partial class fImportador
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fImportador));
            this.pgbImportando = new System.Windows.Forms.ProgressBar();
            this.pcbLogoSGBR = new System.Windows.Forms.PictureBox();
            this.gpbBancoOrigem = new System.Windows.Forms.GroupBox();
            this.btnSelecionarBancoOrigem = new System.Windows.Forms.Button();
            this.txtBancoOrigem = new System.Windows.Forms.TextBox();
            this.gpbBancoDestino = new System.Windows.Forms.GroupBox();
            this.txtBancoDestino = new System.Windows.Forms.TextBox();
            this.btnSelecionarBancoDestino = new System.Windows.Forms.Button();
            this.gpbCriarBanco = new System.Windows.Forms.GroupBox();
            this.btnCriarBanco = new System.Windows.Forms.Button();
            this.btnImportar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pcbLogoSGBR)).BeginInit();
            this.gpbBancoOrigem.SuspendLayout();
            this.gpbBancoDestino.SuspendLayout();
            this.gpbCriarBanco.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgbImportando
            // 
            this.pgbImportando.Location = new System.Drawing.Point(12, 219);
            this.pgbImportando.Name = "pgbImportando";
            this.pgbImportando.Size = new System.Drawing.Size(598, 60);
            this.pgbImportando.TabIndex = 0;
            // 
            // pcbLogoSGBR
            // 
            this.pcbLogoSGBR.Image = global::ImportadorFirebird.Properties.Resources.logo_sgbr_162_78;
            this.pcbLogoSGBR.Location = new System.Drawing.Point(12, 12);
            this.pcbLogoSGBR.Name = "pcbLogoSGBR";
            this.pcbLogoSGBR.Size = new System.Drawing.Size(240, 93);
            this.pcbLogoSGBR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcbLogoSGBR.TabIndex = 1;
            this.pcbLogoSGBR.TabStop = false;
            // 
            // gpbBancoOrigem
            // 
            this.gpbBancoOrigem.Controls.Add(this.btnSelecionarBancoOrigem);
            this.gpbBancoOrigem.Controls.Add(this.txtBancoOrigem);
            this.gpbBancoOrigem.Location = new System.Drawing.Point(12, 125);
            this.gpbBancoOrigem.Name = "gpbBancoOrigem";
            this.gpbBancoOrigem.Size = new System.Drawing.Size(291, 44);
            this.gpbBancoOrigem.TabIndex = 2;
            this.gpbBancoOrigem.TabStop = false;
            this.gpbBancoOrigem.Text = "Banco origem (Firebird 2.5)";
            // 
            // btnSelecionarBancoOrigem
            // 
            this.btnSelecionarBancoOrigem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.btnSelecionarBancoOrigem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelecionarBancoOrigem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelecionarBancoOrigem.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSelecionarBancoOrigem.Location = new System.Drawing.Point(252, 17);
            this.btnSelecionarBancoOrigem.Name = "btnSelecionarBancoOrigem";
            this.btnSelecionarBancoOrigem.Size = new System.Drawing.Size(32, 20);
            this.btnSelecionarBancoOrigem.TabIndex = 7;
            this.btnSelecionarBancoOrigem.Text = "...";
            this.btnSelecionarBancoOrigem.UseVisualStyleBackColor = false;
            this.btnSelecionarBancoOrigem.Click += new System.EventHandler(this.btnSelecionarBancoOrigem_Click);
            // 
            // txtBancoOrigem
            // 
            this.txtBancoOrigem.Location = new System.Drawing.Point(6, 17);
            this.txtBancoOrigem.Name = "txtBancoOrigem";
            this.txtBancoOrigem.Size = new System.Drawing.Size(240, 20);
            this.txtBancoOrigem.TabIndex = 6;
            // 
            // gpbBancoDestino
            // 
            this.gpbBancoDestino.Controls.Add(this.txtBancoDestino);
            this.gpbBancoDestino.Controls.Add(this.btnSelecionarBancoDestino);
            this.gpbBancoDestino.Location = new System.Drawing.Point(321, 125);
            this.gpbBancoDestino.Name = "gpbBancoDestino";
            this.gpbBancoDestino.Size = new System.Drawing.Size(289, 44);
            this.gpbBancoDestino.TabIndex = 3;
            this.gpbBancoDestino.TabStop = false;
            this.gpbBancoDestino.Text = "Banco destino (Firebird 5.0)";
            // 
            // txtBancoDestino
            // 
            this.txtBancoDestino.Location = new System.Drawing.Point(6, 17);
            this.txtBancoDestino.Name = "txtBancoDestino";
            this.txtBancoDestino.Size = new System.Drawing.Size(240, 20);
            this.txtBancoDestino.TabIndex = 8;
            // 
            // btnSelecionarBancoDestino
            // 
            this.btnSelecionarBancoDestino.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.btnSelecionarBancoDestino.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelecionarBancoDestino.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelecionarBancoDestino.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSelecionarBancoDestino.Location = new System.Drawing.Point(252, 17);
            this.btnSelecionarBancoDestino.Name = "btnSelecionarBancoDestino";
            this.btnSelecionarBancoDestino.Size = new System.Drawing.Size(32, 20);
            this.btnSelecionarBancoDestino.TabIndex = 5;
            this.btnSelecionarBancoDestino.Text = "...";
            this.btnSelecionarBancoDestino.UseVisualStyleBackColor = false;
            this.btnSelecionarBancoDestino.Click += new System.EventHandler(this.btnSelecionarBancoDestino_Click);
            // 
            // gpbCriarBanco
            // 
            this.gpbCriarBanco.Controls.Add(this.btnCriarBanco);
            this.gpbCriarBanco.Location = new System.Drawing.Point(321, 12);
            this.gpbCriarBanco.Name = "gpbCriarBanco";
            this.gpbCriarBanco.Size = new System.Drawing.Size(138, 70);
            this.gpbCriarBanco.TabIndex = 4;
            this.gpbCriarBanco.TabStop = false;
            this.gpbCriarBanco.Text = "Criar banco (5.0)";
            // 
            // btnCriarBanco
            // 
            this.btnCriarBanco.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.btnCriarBanco.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCriarBanco.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCriarBanco.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCriarBanco.Location = new System.Drawing.Point(6, 19);
            this.btnCriarBanco.Name = "btnCriarBanco";
            this.btnCriarBanco.Size = new System.Drawing.Size(125, 40);
            this.btnCriarBanco.TabIndex = 8;
            this.btnCriarBanco.Text = "Criar banco";
            this.btnCriarBanco.UseVisualStyleBackColor = false;
            this.btnCriarBanco.Click += new System.EventHandler(this.btnCriarBanco_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(51)))), ((int)(((byte)(255)))));
            this.btnImportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImportar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnImportar.ForeColor = System.Drawing.Color.White;
            this.btnImportar.Location = new System.Drawing.Point(242, 175);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(138, 35);
            this.btnImportar.TabIndex = 5;
            this.btnImportar.Text = "Importar dados";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // fImportador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 291);
            this.Controls.Add(this.btnImportar);
            this.Controls.Add(this.gpbCriarBanco);
            this.Controls.Add(this.gpbBancoDestino);
            this.Controls.Add(this.gpbBancoOrigem);
            this.Controls.Add(this.pcbLogoSGBR);
            this.Controls.Add(this.pgbImportando);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "fImportador";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importador";
            ((System.ComponentModel.ISupportInitialize)(this.pcbLogoSGBR)).EndInit();
            this.gpbBancoOrigem.ResumeLayout(false);
            this.gpbBancoOrigem.PerformLayout();
            this.gpbBancoDestino.ResumeLayout(false);
            this.gpbBancoDestino.PerformLayout();
            this.gpbCriarBanco.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pgbImportando;
        private System.Windows.Forms.PictureBox pcbLogoSGBR;
        private System.Windows.Forms.GroupBox gpbBancoOrigem;
        private System.Windows.Forms.Button btnSelecionarBancoOrigem;
        public System.Windows.Forms.TextBox txtBancoOrigem;
        private System.Windows.Forms.GroupBox gpbBancoDestino;
        public System.Windows.Forms.TextBox txtBancoDestino;
        private System.Windows.Forms.Button btnSelecionarBancoDestino;
        private System.Windows.Forms.GroupBox gpbCriarBanco;
        private System.Windows.Forms.Button btnCriarBanco;
        private System.Windows.Forms.Button btnImportar;
    }
}

