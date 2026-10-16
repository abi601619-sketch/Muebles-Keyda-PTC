namespace Vista.Cotizaciones
{
    partial class frmProdctosCotizacion
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblMostrarFecha = new System.Windows.Forms.Label();
            this.lblDetalleCotizacion = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlBuscarClienteSuperior = new System.Windows.Forms.Panel();
            this.guna2CustomGradientPanel1 = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.lblNumeroCotizacion = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCodigoProduccion = new System.Windows.Forms.Label();
            this.lblEstadoCotizacion = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblFecha = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dgvProductosCotizacion = new Guna.UI2.WinForms.Guna2DataGridView();
            this.lblTotal = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnGenerarPDF = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.pnlBuscarClienteSuperior.SuspendLayout();
            this.guna2CustomGradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductosCotizacion)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // lblMostrarFecha
            // 
            this.lblMostrarFecha.AutoSize = true;
            this.lblMostrarFecha.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.lblMostrarFecha.Location = new System.Drawing.Point(169, 53);
            this.lblMostrarFecha.Name = "lblMostrarFecha";
            this.lblMostrarFecha.Size = new System.Drawing.Size(52, 16);
            this.lblMostrarFecha.TabIndex = 64;
            this.lblMostrarFecha.Text = "Estado :";
            // 
            // lblDetalleCotizacion
            // 
            this.lblDetalleCotizacion.AutoSize = true;
            this.lblDetalleCotizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblDetalleCotizacion.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetalleCotizacion.ForeColor = System.Drawing.Color.White;
            this.lblDetalleCotizacion.Location = new System.Drawing.Point(15, 9);
            this.lblDetalleCotizacion.Name = "lblDetalleCotizacion";
            this.lblDetalleCotizacion.Size = new System.Drawing.Size(172, 15);
            this.lblDetalleCotizacion.TabIndex = 0;
            this.lblDetalleCotizacion.Text = "DETALLES DE COTIZACION";
            // 
            // btnSalir
            // 
            this.btnSalir.Image = global::Vista.Properties.Resources.Cerrar16px;
            this.btnSalir.Location = new System.Drawing.Point(730, 5);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(32, 25);
            this.btnSalir.TabIndex = 1;
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click_1);
            // 
            // pnlBuscarClienteSuperior
            // 
            this.pnlBuscarClienteSuperior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(62)))), ((int)(((byte)(36)))));
            this.pnlBuscarClienteSuperior.Controls.Add(this.btnSalir);
            this.pnlBuscarClienteSuperior.Controls.Add(this.lblDetalleCotizacion);
            this.pnlBuscarClienteSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBuscarClienteSuperior.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlBuscarClienteSuperior.Location = new System.Drawing.Point(0, 0);
            this.pnlBuscarClienteSuperior.Name = "pnlBuscarClienteSuperior";
            this.pnlBuscarClienteSuperior.Size = new System.Drawing.Size(775, 38);
            this.pnlBuscarClienteSuperior.TabIndex = 50;
            // 
            // guna2CustomGradientPanel1
            // 
            this.guna2CustomGradientPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.guna2CustomGradientPanel1.Controls.Add(this.lblFecha);
            this.guna2CustomGradientPanel1.Controls.Add(this.lblMostrarFecha);
            this.guna2CustomGradientPanel1.Controls.Add(this.lblEstadoCotizacion);
            this.guna2CustomGradientPanel1.Controls.Add(this.lblNumeroCotizacion);
            this.guna2CustomGradientPanel1.Controls.Add(this.lblCodigoProduccion);
            this.guna2CustomGradientPanel1.FillColor = System.Drawing.Color.Bisque;
            this.guna2CustomGradientPanel1.FillColor2 = System.Drawing.Color.SandyBrown;
            this.guna2CustomGradientPanel1.FillColor3 = System.Drawing.Color.Snow;
            this.guna2CustomGradientPanel1.Location = new System.Drawing.Point(18, 50);
            this.guna2CustomGradientPanel1.Name = "guna2CustomGradientPanel1";
            this.guna2CustomGradientPanel1.Size = new System.Drawing.Size(325, 83);
            this.guna2CustomGradientPanel1.TabIndex = 66;
            // 
            // lblNumeroCotizacion
            // 
            this.lblNumeroCotizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblNumeroCotizacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroCotizacion.Location = new System.Drawing.Point(157, 8);
            this.lblNumeroCotizacion.Name = "lblNumeroCotizacion";
            this.lblNumeroCotizacion.Size = new System.Drawing.Size(75, 26);
            this.lblNumeroCotizacion.TabIndex = 67;
            this.lblNumeroCotizacion.Text = " numero";
            // 
            // lblCodigoProduccion
            // 
            this.lblCodigoProduccion.AutoSize = true;
            this.lblCodigoProduccion.BackColor = System.Drawing.Color.Transparent;
            this.lblCodigoProduccion.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigoProduccion.Location = new System.Drawing.Point(10, 14);
            this.lblCodigoProduccion.Name = "lblCodigoProduccion";
            this.lblCodigoProduccion.Size = new System.Drawing.Size(131, 19);
            this.lblCodigoProduccion.TabIndex = 66;
            this.lblCodigoProduccion.Text = "COTIZACION N°";
            // 
            // lblEstadoCotizacion
            // 
            this.lblEstadoCotizacion.BackColor = System.Drawing.Color.Transparent;
            this.lblEstadoCotizacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblEstadoCotizacion.Location = new System.Drawing.Point(229, 51);
            this.lblEstadoCotizacion.Name = "lblEstadoCotizacion";
            this.lblEstadoCotizacion.Size = new System.Drawing.Size(72, 18);
            this.lblEstadoCotizacion.TabIndex = 68;
            this.lblEstadoCotizacion.Text = "Aprobada";
            // 
            // lblFecha
            // 
            this.lblFecha.BackColor = System.Drawing.Color.Transparent;
            this.lblFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblFecha.Location = new System.Drawing.Point(16, 51);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(46, 17);
            this.lblFecha.TabIndex = 69;
            this.lblFecha.Text = "Fecha:";
            // 
            // dgvProductosCotizacion
            // 
            dataGridViewCellStyle22.BackColor = System.Drawing.Color.White;
            this.dgvProductosCotizacion.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle22;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProductosCotizacion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle23;
            this.dgvProductosCotizacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvProductosCotizacion.DefaultCellStyle = dataGridViewCellStyle24;
            this.dgvProductosCotizacion.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvProductosCotizacion.Location = new System.Drawing.Point(21, 152);
            this.dgvProductosCotizacion.Name = "dgvProductosCotizacion";
            this.dgvProductosCotizacion.RowHeadersVisible = false;
            this.dgvProductosCotizacion.Size = new System.Drawing.Size(741, 223);
            this.dgvProductosCotizacion.TabIndex = 67;
            this.dgvProductosCotizacion.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvProductosCotizacion.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvProductosCotizacion.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductosCotizacion.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvProductosCotizacion.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // lblTotal
            // 
            this.lblTotal.BackColor = System.Drawing.Color.Transparent;
            this.lblTotal.Location = new System.Drawing.Point(21, 385);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(86, 15);
            this.lblTotal.TabIndex = 68;
            this.lblTotal.Text = "guna2HtmlLabel1";
            // 
            // btnGenerarPDF
            // 
            this.btnGenerarPDF.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGenerarPDF.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGenerarPDF.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGenerarPDF.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGenerarPDF.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGenerarPDF.ForeColor = System.Drawing.Color.White;
            this.btnGenerarPDF.Location = new System.Drawing.Point(383, 83);
            this.btnGenerarPDF.Name = "btnGenerarPDF";
            this.btnGenerarPDF.Size = new System.Drawing.Size(146, 36);
            this.btnGenerarPDF.TabIndex = 70;
            this.btnGenerarPDF.Text = "Generar PDF";
            this.btnGenerarPDF.Click += new System.EventHandler(this.btnGenerarPDF_Click);
            // 
            // frmProdctosCotizacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(221)))), ((int)(((byte)(175)))));
            this.ClientSize = new System.Drawing.Size(775, 421);
            this.Controls.Add(this.btnGenerarPDF);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvProductosCotizacion);
            this.Controls.Add(this.guna2CustomGradientPanel1);
            this.Controls.Add(this.pnlBuscarClienteSuperior);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmProdctosCotizacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmProdctosCotizacion";
            this.Load += new System.EventHandler(this.frmProdctosCotizacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.pnlBuscarClienteSuperior.ResumeLayout(false);
            this.pnlBuscarClienteSuperior.PerformLayout();
            this.guna2CustomGradientPanel1.ResumeLayout(false);
            this.guna2CustomGradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductosCotizacion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblMostrarFecha;
        private System.Windows.Forms.Panel pnlBuscarClienteSuperior;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label lblDetalleCotizacion;
        private Guna.UI2.WinForms.Guna2CustomGradientPanel guna2CustomGradientPanel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNumeroCotizacion;
        private System.Windows.Forms.Label lblCodigoProduccion;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFecha;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEstadoCotizacion;
        private Guna.UI2.WinForms.Guna2DataGridView dgvProductosCotizacion;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotal;
        private Guna.UI2.WinForms.Guna2Button btnGenerarPDF;
    }
}