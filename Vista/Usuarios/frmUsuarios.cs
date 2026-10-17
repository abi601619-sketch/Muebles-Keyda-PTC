using Modelo.Entidades;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Vista.Responsive;

namespace Vista.Usuarios
{
    public partial class frmUsuarios : Form
    {
        public frmUsuarios()
        {
            InitializeComponent();
            ResponsiveHelper.Apply(this);
            ConfigurarTooltips();
            ConfigurarBotonesUsuarios();

        }

        // VARIABLES PARA LA PAGINACIÓN
        private DataTable dtUsuarios;
        private int paginaActual = 1;
        private int registrosPorPagina = 20;
        private int totalPaginas = 0;

        // ID DEL USUARIO QUE SE ESTA EDITANDO (0 = SE REGISTRA UNO NUEVO)
        private int idUsuarioSeleccionado = 0;
        private const string TextoGuardar = "Guardar";
        private const string TextoActualizar = "Actualizar";

        private Guna.UI2.WinForms.Guna2Elipse elipseContenedor;
        private Guna.UI2.WinForms.Guna2Elipse elipsePedidoDatos;
        private Guna.UI2.WinForms.Guna2Elipse elipsePedidosRegistrados;




        private void ConfigurarBotonesUsuarios()
        {
            ConfigurarBotonCafe(btnNuevoUsuario);
            ConfigurarBotonCafe(btnGuardarUsuario);
            ConfigurarBotonCafe(btnDesactivarUsuario);
        }

        private void ConfigurarBotonCafe(Guna.UI2.WinForms.Guna2TileButton boton)
        {
            boton.FillColor = Color.FromArgb(235, 218, 198);
            boton.ForeColor = Color.FromArgb(121, 75, 45);

            boton.BorderColor = Color.FromArgb(121, 75, 45);
            boton.BorderThickness = 1;
            boton.BorderRadius = 10;

            boton.HoverState.FillColor = Color.FromArgb(220, 190, 160);
            boton.HoverState.ForeColor = Color.FromArgb(121, 75, 45);
            boton.HoverState.BorderColor = Color.FromArgb(121, 75, 45);

            boton.PressedColor = Color.FromArgb(205, 175, 145);

            boton.Cursor = Cursors.Hand;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void ConfigurarTooltips()
        {
            ToolTip toolTip1 = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 200,
                ShowAlways = true
            };

            toolTip1.SetToolTip(txtUsuario, "Ingrese el nombre de usuario.");
            toolTip1.SetToolTip(txtCorreo, "Ingrese el correo electrónico del usuario.");
            toolTip1.SetToolTip(txtContrasena, "Ingrese la contraseña del usuario. Al editar un usuario, déjela vacía para conservar la actual o escriba una nueva para cambiarla.");
            toolTip1.SetToolTip(cmbRol, "Seleccione el rol que tendrá el usuario.");
            toolTip1.SetToolTip(chkEstado, "Indica si el usuario se encuentra activo.");
            toolTip1.SetToolTip(btnNuevoUsuario, "Limpia el formulario para registrar un nuevo usuario.");
            toolTip1.SetToolTip(btnGuardarUsuario, "Guarda el nuevo usuario. Si seleccionaste uno con doble clic, guarda los cambios de ese usuario.");
            toolTip1.SetToolTip(btnDesactivarUsuario, "Desactiva el usuario seleccionado.");
            toolTip1.SetToolTip(dgvUsuariosRegistrados, "Muestra los usuarios registrados. Haz doble clic en un usuario para editarlo o cambiar su contraseña.");
        }

        private void ConfigurarTablaUsuarios()
        {
            // Configuración general
            dgvUsuariosRegistrados.AutoGenerateColumns = true;
            dgvUsuariosRegistrados.EnableHeadersVisualStyles = false;
            dgvUsuariosRegistrados.AllowUserToAddRows = false;
            dgvUsuariosRegistrados.AllowUserToDeleteRows = false;
            dgvUsuariosRegistrados.AllowUserToResizeRows = false;
            dgvUsuariosRegistrados.AllowUserToResizeColumns = false;
            dgvUsuariosRegistrados.ReadOnly = true;
            dgvUsuariosRegistrados.MultiSelect = false;
            dgvUsuariosRegistrados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuariosRegistrados.RowHeadersVisible = false;
            dgvUsuariosRegistrados.BorderStyle = BorderStyle.None;
            dgvUsuariosRegistrados.BackgroundColor = Color.White;
            dgvUsuariosRegistrados.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvUsuariosRegistrados.GridColor = Color.FromArgb(225, 225, 225);

            // Encabezado
            dgvUsuariosRegistrados.ColumnHeadersHeight = 40;
            dgvUsuariosRegistrados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(121, 75, 45);
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(121, 75, 45);
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsuariosRegistrados.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            // Filas
            dgvUsuariosRegistrados.RowTemplate.Height = 36;

            dgvUsuariosRegistrados.DefaultCellStyle.BackColor = Color.White;
            dgvUsuariosRegistrados.DefaultCellStyle.ForeColor = Color.FromArgb(55, 55, 55);
            dgvUsuariosRegistrados.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvUsuariosRegistrados.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvUsuariosRegistrados.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 215, 185);
            dgvUsuariosRegistrados.DefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 45, 35);
            dgvUsuariosRegistrados.DefaultCellStyle.Padding = new Padding(5);

            // Filas alternadas
            dgvUsuariosRegistrados.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 246, 240);
            dgvUsuariosRegistrados.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(55, 55, 55);
            dgvUsuariosRegistrados.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 215, 185);
            dgvUsuariosRegistrados.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 45, 35);

            // Fila seleccionada
            dgvUsuariosRegistrados.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 215, 185);
            dgvUsuariosRegistrados.RowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 45, 35);

            // Ajustar columnas
            dgvUsuariosRegistrados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // No permitir ordenar las columnas
            foreach (DataGridViewColumn columna in dgvUsuariosRegistrados.Columns)
            {
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void RegistrarUsuario()
        {
            try
            {
                string nombre = txtUsuario.Text.Trim();
                string usuarioNombre = txtUsuario.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string contraseña = txtContrasena.Text;
                bool estado = true;

                // Validar nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    errorProvider1.SetError(txtUsuario, "Debe ingresar el nombre del usuario.");
                    txtUsuario.Focus();
                    return;
                }

                // Validar nombre de usuario
                if (string.IsNullOrWhiteSpace(usuarioNombre))
                {
                    errorProvider1.SetError(txtUsuario, "Debe ingresar un nombre de usuario.");
                    txtUsuario.Focus();
                    return;
                }

                // Validar correo
                if (string.IsNullOrWhiteSpace(correo))
                {
                    errorProvider1.SetError(txtCorreo, "Debe ingresar un correo electrónico.");
                    txtCorreo.Focus();
                    return;
                }

                // Validar formato del correo
                if (!correo.Contains("@") || !correo.Contains("."))
                {
                    errorProvider1.SetError(txtCorreo, "Ingrese un correo electrónico válido.");
                    txtCorreo.Focus();
                    return;
                }

                // Validar contraseña
                if (string.IsNullOrWhiteSpace(contraseña))
                {
                    errorProvider1.SetError(txtContrasena, "Debe ingresar una contraseña.");
                    txtContrasena.Focus();
                    return;
                }

                // Validar rol
                if (cmbRol.SelectedIndex == -1)
                {
                    errorProvider1.SetError(cmbRol, "Se debe seleccionar un rol.");
                    cmbRol.Focus();
                    return;
                }

                string rol = cmbRol.Text.Trim();

                // Validar rol válido
                if (rol != "Administrador" && rol != "Secretario")
                {
                    errorProvider1.SetError(cmbRol, "Se debe seleccionar un rol válido.");
                    cmbRol.Focus();
                    return;
                }

                DbUsuarios nuevoUsuario = new DbUsuarios
                {
                    Nombre1 = nombre,
                    Usuario1 = usuarioNombre,
                    Correo1 = correo,
                    Contraseña1 = contraseña,
                    Rol1 = rol,
                    Estado1 = estado
                };
                nuevoUsuario.InsertarUsuario();

                MessageBox.Show("Usuario registrado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
                MostrarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al registrar el usuario: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ACTUALIZAR EL USUARIO SELECCIONADO (NOMBRE DE USUARIO, CORREO, ROL Y, SI SE ESCRIBE, LA CONTRASEÑA)
        private void ActualizarUsuarioSeleccionado()
        {
            try
            {
                string usuarioNombre = txtUsuario.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string nuevaContraseña = txtContrasena.Text;

                if (string.IsNullOrWhiteSpace(usuarioNombre))
                {
                    errorProvider1.SetError(txtUsuario, "Debe ingresar un nombre de usuario.");
                    txtUsuario.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(correo))
                {
                    errorProvider1.SetError(txtCorreo, "Debe ingresar un correo electrónico.");
                    txtCorreo.Focus();
                    return;
                }

                if (!correo.Contains("@") || !correo.Contains("."))
                {
                    errorProvider1.SetError(txtCorreo, "Ingrese un correo electrónico válido.");
                    txtCorreo.Focus();
                    return;
                }

                string rol = cmbRol.Text.Trim();

                if (cmbRol.SelectedIndex == -1 || (rol != "Administrador" && rol != "Secretario"))
                {
                    errorProvider1.SetError(cmbRol, "Se debe seleccionar un rol válido.");
                    cmbRol.Focus();
                    return;
                }

                // LA CONTRASEÑA ES OPCIONAL: VACIA = SE CONSERVA LA ACTUAL
                bool cambiaContraseña = !string.IsNullOrWhiteSpace(nuevaContraseña);

                string aviso = "¿Desea guardar los cambios del usuario \"" + usuarioNombre + "\"?";

                if (cambiaContraseña)
                    aviso += "\n\nTambién se cambiará su contraseña.";

                if (MessageBox.Show(aviso, "Confirmar cambios", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                DbUsuarios usuario = new DbUsuarios
                {
                    IdUsuario1 = idUsuarioSeleccionado,
                    Usuario1 = usuarioNombre,
                    Correo1 = correo,
                    Rol1 = rol
                };

                string error = usuario.ActualizarUsuario(nuevaContraseña);

                if (error != null)
                {
                    MessageBox.Show(error, "No se pudo actualizar el usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    cambiaContraseña ? "Usuario actualizado y contraseña cambiada correctamente." : "Usuario actualizado correctamente.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarFormulario();
                MostrarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al actualizar el usuario: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BOTÓN GUARDAR
        private void btnGuardarUsuario_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (idUsuarioSeleccionado > 0)
                    ActualizarUsuarioSeleccionado();
                else
                    RegistrarUsuario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DESACTIVAR USUARIO
        private void btnDesactivarUsuario_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsuariosRegistrados.CurrentRow == null)
                {
                    MessageBox.Show(
                        "Debe seleccionar un usuario.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow fila = dgvUsuariosRegistrados.CurrentRow;

                int idUsuario = Convert.ToInt32(fila.Cells["#"].Value);
                string estado = fila.Cells["Estado"].Value?.ToString() ?? "";

                DbUsuarios usuario = new DbUsuarios();

                if (estado == "Activo")
                {
                    DialogResult respuesta = MessageBox.Show(
                        "¿Está seguro de desactivar este usuario?",
                        "Confirmar desactivación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (respuesta != DialogResult.Yes)
                        return;

                    usuario.DesactivarUsuario(idUsuario);

                    MessageBox.Show(
                        "Usuario desactivado correctamente.",
                        "Información",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else if (estado == "Inactivo")
                {
                    DialogResult respuesta = MessageBox.Show(
                        "¿Desea volver a activar este usuario?",
                        "Confirmar activación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (respuesta != DialogResult.Yes)
                        return;

                    // Este método debe existir en DbUsuarios.
                    usuario.ActivarUsuario(idUsuario);

                    MessageBox.Show(
                        "Usuario activado correctamente.",
                        "Información",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "No se reconoce el estado del usuario.",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MostrarUsuarios();
                LimpiarFormulario();
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "El identificador del usuario no es válido.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al cambiar el estado del usuario: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // MOSTRAR USUARIOS
        private void MostrarUsuarios()
        {
            try
            {
                // Cargar todos los usuarios
                dtUsuarios = DbUsuarios.CargarUsuarios();

                // Volver a la primera página
                paginaActual = 1;

                // Calcular el total de páginas
                CalcularPaginasUsuarios();

                // Mostrar la primera página
                MostrarPaginaUsuarios();

                // El estado se controla desde la base de datos
                // Los nuevos usuarios siempre se registran activos
                chkEstado.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar los usuarios:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CalcularPaginasUsuarios()
        {
            try
            {
                if (dtUsuarios == null || dtUsuarios.Rows.Count == 0)
                {
                    totalPaginas = 1;
                    paginaActual = 1;
                    return;
                }

                totalPaginas = (int)Math.Ceiling((double)dtUsuarios.Rows.Count / registrosPorPagina);

                if (totalPaginas == 0)
                    totalPaginas = 1;

                if (paginaActual > totalPaginas)
                    paginaActual = totalPaginas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al calcular las páginas: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MostrarPaginaUsuarios()
        {
            try
            {
                if (dtUsuarios == null)
                    return;

                DataTable dtPagina = dtUsuarios.Clone();

                int inicio = (paginaActual - 1) * registrosPorPagina;
                int fin = Math.Min(inicio + registrosPorPagina, dtUsuarios.Rows.Count);

                for (int i = inicio; i < fin; i++)
                    dtPagina.ImportRow(dtUsuarios.Rows[i]);

                dgvUsuariosRegistrados.DataSource = null;
                dgvUsuariosRegistrados.DataSource = dtPagina;

                ConfigurarTablaUsuarios();

                lblPagina.Text = $"Página {paginaActual} de {totalPaginas}";
                btnAnterior.Enabled = paginaActual > 1;
                btnSiguiente.Enabled = paginaActual < totalPaginas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al mostrar los usuarios: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                MostrarPaginaUsuarios();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                MostrarPaginaUsuarios();
            }
        }

        // CARGAR FORMULARIO
        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                MostrarUsuarios();
                chkEstado.Visible = false;
                ConfigurarTablaUsuarios();
                CargarLogoEmpresa();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al cargar el formulario: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // DOBLE CLIC EN UN USUARIO
        private void dgvUsuariosRegistrados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                // Evitar seleccionar encabezados
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow fila = dgvUsuariosRegistrados.Rows[e.RowIndex];

                // Obtener el estado antes de utilizarlo
                string estado = fila.Cells["Estado"].Value?.ToString() ?? "";

                // El usuario queda seleccionado para editarlo
                idUsuarioSeleccionado = Convert.ToInt32(fila.Cells["#"].Value);
                btnGuardarUsuario.Text = TextoActualizar;

                // Cambiar el texto del botón según el estado
                if (estado == "Activo")
                {
                    btnDesactivarUsuario.Text = "Desactivar";
                }
                else
                {
                    btnDesactivarUsuario.Text = "Activar";
                }

                // Cargar datos del usuario
                txtUsuario.Text = fila.Cells["Usuario"].Value?.ToString() ?? "";
                txtCorreo.Text = fila.Cells["Correo"].Value?.ToString() ?? "";
                cmbRol.Text = fila.Cells["Rol"].Value?.ToString() ?? "";

                // Cargar estado
                chkEstado.Checked = estado == "Activo";

                // No mostrar la contraseña
                txtContrasena.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al seleccionar el usuario:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        // BOTÓN NUEVO USUARIO
        private void btnNuevoUsuario_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();

            txtUsuario.Focus();
        }

        // LIMPIAR FORMULARIO
        private void LimpiarFormulario()
        {
            idUsuarioSeleccionado = 0;
            btnGuardarUsuario.Text = TextoGuardar;

            txtUsuario.Clear();

            txtCorreo.Clear();

            txtContrasena.Clear();

            cmbRol.SelectedIndex = -1;

            chkEstado.Checked = false;
        }

        private void CargarLogoEmpresa()
        {
            string rutaLogo = Modelo.Properties.Settings.Default.LogoEmpresa;

            if (!string.IsNullOrWhiteSpace(rutaLogo) && File.Exists(rutaLogo))
            {
                try
                {
                    if (picLogo.Image != null)
                    {
                        picLogo.Image.Dispose();
                        picLogo.Image = null;
                    }

                    using (Image imagenOriginal = Image.FromFile(rutaLogo))
                    {
                        picLogo.Image = new Bitmap(imagenOriginal);
                    }

                    picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch
                {
                    picLogo.Image = null;
                }
            }
            else
            {
                picLogo.Image = null;
            }
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();

        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();

        }

        private void txtContrasena_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();

        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.Clear();

        }


    }
}