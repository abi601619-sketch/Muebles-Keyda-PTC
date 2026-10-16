using Modelo;
using Modelo.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Vista.Cotizaciones
{
    public partial class frmProdctosCotizacion : Form
    {
        public frmProdctosCotizacion()
        {
            InitializeComponent();
        }
        private int idCotizacion;
        private DateTime fechaCotizacion;
        private string nombreCliente;
        private string telefonoCliente;
        private string correoCliente;
        private string direccionCliente;
        private string condicionesPago;
        private string condicionesEntrega;
        private string estadoCotizacion;
        private decimal subtotal;
        private decimal iva;
        private decimal total;

        public frmProdctosCotizacion(int idCotizacion)
        {
            InitializeComponent();

            this.idCotizacion = idCotizacion;

            ConfigurarTablaProductos();
            ConfigurarBotones();
            CargarDetalleCotizacion();
            ConfigurarBotonPDF();
            ConfigurarInformacionCotizacion();
            CargarEncabezadoCotizacion();
        }
        private void CargarEncabezadoCotizacion()
        {
            try
            {
                DataTable dt = ProductosCotizacion.CargarEncabezadoCotizacion(idCotizacion);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontró la cotización seleccionada.");
                    return;
                }

                DataRow fila = dt.Rows[0];

                lblNumeroCotizacion.Text =
                    fila["IdCotizacion"].ToString();

                lblFecha.Text =
                    Convert.ToDateTime(fila["Fecha"]).ToString("dd/MM/yyyy");

                lblEstadoCotizacion.Text =
                    fila["Estado"].ToString();

                lblTotal.Text =
                    "Total de la cotización: " +
                    Convert.ToDecimal(fila["Total"]).ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los datos de la cotización: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void frmProdctosCotizacion_Load(object sender, EventArgs e)
        {

        }

        private void lblCliente_Click(object sender, EventArgs e)
        {

        }
        private void ConfigurarBotones()
        {


            btnSalir.ForeColor = Color.FromArgb(80, 45, 35);

            btnSalir.Cursor = Cursors.Hand;

            lblTotal.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(121, 75, 45);
        }


        private void ConfigurarTablaProductos()
        {
            dgvProductosCotizacion.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgvProductosCotizacion.EnableHeadersVisualStyles = false;

            dgvProductosCotizacion.BackgroundColor = Color.White;
            dgvProductosCotizacion.BorderStyle = BorderStyle.None;
            dgvProductosCotizacion.GridColor = Color.FromArgb(230, 220, 210);

            dgvProductosCotizacion.ReadOnly = true;
            dgvProductosCotizacion.AllowUserToAddRows = false;
            dgvProductosCotizacion.AllowUserToDeleteRows = false;
            dgvProductosCotizacion.AllowUserToResizeRows = false;
            dgvProductosCotizacion.RowHeadersVisible = false;
            dgvProductosCotizacion.MultiSelect = false;
            dgvProductosCotizacion.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProductosCotizacion.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Encabezado café
            dgvProductosCotizacion.ColumnHeadersHeight = 40;
            dgvProductosCotizacion.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvProductosCotizacion.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(121, 75, 45);
            dgvProductosCotizacion.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;
            dgvProductosCotizacion.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProductosCotizacion.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            // Filas
            dgvProductosCotizacion.RowTemplate.Height = 36;
            dgvProductosCotizacion.DefaultCellStyle.BackColor = Color.White;
            dgvProductosCotizacion.DefaultCellStyle.ForeColor =
                Color.FromArgb(65, 50, 40);
            dgvProductosCotizacion.DefaultCellStyle.Font =
                new Font("Segoe UI", 10);
            dgvProductosCotizacion.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(238, 215, 185);
            dgvProductosCotizacion.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(65, 50, 40);
            dgvProductosCotizacion.DefaultCellStyle.Padding =
                new Padding(5);

            // Filas alternadas beige
            dgvProductosCotizacion.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(250, 246, 240);
            dgvProductosCotizacion.AlternatingRowsDefaultCellStyle.ForeColor =
                Color.FromArgb(65, 50, 40);

            // Formato de precios
            if (dgvProductosCotizacion.Columns.Contains("PrecioUnitario"))
            {
                dgvProductosCotizacion.Columns["PrecioUnitario"]
                    .HeaderText = "Precio unitario";

                dgvProductosCotizacion.Columns["PrecioUnitario"]
                    .DefaultCellStyle.Format = "C2";
            }

            if (dgvProductosCotizacion.Columns.Contains("SubTotal"))
            {
                dgvProductosCotizacion.Columns["SubTotal"]
                    .HeaderText = "Subtotal";

                dgvProductosCotizacion.Columns["SubTotal"]
                    .DefaultCellStyle.Format = "C2";
            }
        }

        private void ConfigurarInformacionCotizacion()
        {
            // Datos principales
            lblNumeroCotizacion.Font =
                new Font("Segoe UI", 12, FontStyle.Bold);

            lblNumeroCotizacion.ForeColor =
                Color.FromArgb(121, 75, 45);

            lblFecha.Font =
                new Font("Segoe UI", 10, FontStyle.Regular);

            lblEstadoCotizacion.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            lblEstadoCotizacion.ForeColor =
                Color.FromArgb(121, 75, 45);


        }


        private void ConfigurarBotonPDF()
        {
            btnGenerarPDF.Text = "Generar PDF";
            btnGenerarPDF.FillColor = Color.FromArgb(121, 75, 45);
            btnGenerarPDF.ForeColor = Color.White;
            btnGenerarPDF.BorderRadius = 8;
            btnGenerarPDF.BorderThickness = 0;
            btnGenerarPDF.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnGenerarPDF.Cursor = Cursors.Hand;

            btnGenerarPDF.HoverState.FillColor =
                Color.FromArgb(145, 92, 58);

            btnGenerarPDF.PressedColor =
                Color.FromArgb(95, 58, 35);
        }


        private void CargarDetalleCotizacion()
        {
            try
            {
                dgvProductosCotizacion.DataSource =
                    ProductosCotizacion.CargarProductosCotizacion(idCotizacion);

                if (dgvProductosCotizacion.Columns.Contains("PrecioUnitario"))
                    dgvProductosCotizacion.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";

                if (dgvProductosCotizacion.Columns.Contains("SubTotal"))
                    dgvProductosCotizacion.Columns["SubTotal"].DefaultCellStyle.Format = "C2";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los productos de la cotización: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            decimal total = 0;

            foreach (DataGridViewRow fila in dgvProductosCotizacion.Rows)
            {
                if (fila.IsNewRow)
                    continue;

                if (fila.Cells["SubTotal"].Value != null &&
                    fila.Cells["SubTotal"].Value != DBNull.Value)
                {
                    total += Convert.ToDecimal(fila.Cells["SubTotal"].Value);
                }
            }

            lblTotal.Text = "Total: " + total.ToString("C2");
        }

        private void btnSalir_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGenerarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                if (idCotizacion <= 0)
                {
                    MessageBox.Show(
                        "No se ha seleccionado una cotización.",
                        "Generar PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                List<ProductoPDF> productos = new List<ProductoPDF>();
                foreach (DataGridViewRow row in dgvProductosCotizacion.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Cells["Mueble"].Value == null)
                        continue;

                    string descripcion = row.Cells["Mueble"].Value.ToString();

                    if (string.IsNullOrWhiteSpace(descripcion))
                        continue;

                    // La columna Medidas tiene un texto como "50 x 50 x 45 cm".
                    string medidas = row.Cells["Medidas"].Value?.ToString() ?? "";

                    string[] partes = medidas
                        .Replace(" cm", "")
                        .Split('x');

                    if (partes.Length != 3)
                        throw new Exception("Las medidas del producto no tienen el formato esperado.");

                    productos.Add(new ProductoPDF
                    {
                        Descripcion = descripcion,
                        Largo = Convert.ToInt32(decimal.Parse(partes[0].Trim())),
                        Ancho = Convert.ToInt32(decimal.Parse(partes[1].Trim())),
                        Alto = Convert.ToInt32(decimal.Parse(partes[2].Trim())),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        PrecioUnitario = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value),
                        SubTotal = Convert.ToDecimal(row.Cells["SubTotal"].Value)
                    });
                }

                if (productos.Count == 0)
                {
                    MessageBox.Show(
                        "Esta cotización no tiene productos.",
                        "Generar PDF",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Crear carpeta para guardar las cotizaciones
                string carpeta = Path.Combine(
                    Application.StartupPath,
                    "Cotizaciones");

                Directory.CreateDirectory(carpeta);

                string rutaPDF = Path.Combine(
                    carpeta,
                    $"Cotizacion_{idCotizacion}.pdf");

                // Estos datos deben cargarse desde la cotización seleccionada.
                // Reemplaza los nombres por los controles o variables reales.

                CotizacionDocumentoPDF.Generar(
                    rutaPDF,
                    idCotizacion,
                    fechaCotizacion,
                    nombreCliente,
                    telefonoCliente,
                    correoCliente,
                    direccionCliente,
                    condicionesPago,
                    condicionesEntrega,
                    estadoCotizacion,
                    subtotal,
                    iva,
                    total,
                    productos);

                if (!File.Exists(rutaPDF))
                {
                    MessageBox.Show(
                        "No se pudo crear el archivo PDF.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaPDF,
                    UseShellExecute = true
                });

                MessageBox.Show(
                    "PDF generado correctamente.\n\nGuardado en:\n" + rutaPDF,
                    "Generar PDF",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al generar el PDF.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
