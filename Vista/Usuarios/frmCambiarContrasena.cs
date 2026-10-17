using Modelo.Entidades;
using System;
using System.Drawing;
using System.Windows.Forms;
using Vista.Login;

namespace Vista.Usuarios
{
    // VENTANA PARA QUE CADA USUARIO (ADMINISTRADOR O SECRETARIO) CAMBIE SU PROPIA CONTRASEÑA.
    // SE ARMA POR CODIGO, SIN DISEÑADOR, Y SE ABRE DESDE EL BOTON "Mi contraseña" DEL MENU DE CADA DASHBOARD.
    public class frmCambiarContrasena : Form
    {
        private static readonly Color ColorCafe = Color.FromArgb(121, 75, 45);
        private static readonly Color ColorFondo = Color.FromArgb(255, 238, 200);

        private readonly TextBox txtActual = new TextBox();
        private readonly TextBox txtNueva = new TextBox();
        private readonly TextBox txtConfirmar = new TextBox();
        private readonly CheckBox chkMostrar = new CheckBox();
        private readonly Button btnGuardar = new Button();
        private readonly Button btnCancelar = new Button();
        private readonly ErrorProvider errorProvider = new ErrorProvider();

        public frmCambiarContrasena()
        {
            Text = "Cambiar mi contraseña";
            ClientSize = new Size(400, 340);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = ColorFondo;
            Font = new Font("Times New Roman", 10F);

            Controls.Add(new Label
            {
                Text = "Cambiar mi contraseña",
                Font = new Font("Times New Roman", 15F, FontStyle.Bold),
                ForeColor = ColorCafe,
                AutoSize = true,
                Location = new Point(20, 15)
            });

            Controls.Add(new Label
            {
                Text = "Usuario: " + SesionActual.Usuario,
                AutoSize = true,
                Location = new Point(22, 50)
            });

            AgregarCampo("Contraseña actual:", txtActual, 85);
            AgregarCampo("Nueva contraseña (mínimo " + DbUsuarios.LongitudMinimaContraseña + " caracteres):", txtNueva, 140);
            AgregarCampo("Confirmar la nueva contraseña:", txtConfirmar, 195);

            chkMostrar.Text = "Mostrar contraseñas";
            chkMostrar.AutoSize = true;
            chkMostrar.Location = new Point(22, 255);
            chkMostrar.CheckedChanged += (s, e) =>
            {
                bool ocultar = !chkMostrar.Checked;
                txtActual.UseSystemPasswordChar = ocultar;
                txtNueva.UseSystemPasswordChar = ocultar;
                txtConfirmar.UseSystemPasswordChar = ocultar;
            };
            Controls.Add(chkMostrar);

            ConfigurarBoton(btnGuardar, "Guardar", new Point(20, 290), ColorCafe, Color.White);
            ConfigurarBoton(btnCancelar, "Cancelar", new Point(210, 290), Color.FromArgb(235, 205, 170), Color.Black);
            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;
        }

        private void AgregarCampo(string etiqueta, TextBox caja, int y)
        {
            Controls.Add(new Label { Text = etiqueta, AutoSize = true, Location = new Point(22, y) });

            caja.Location = new Point(20, y + 20);
            caja.Size = new Size(360, 26);
            caja.Font = new Font("Times New Roman", 12F);
            caja.UseSystemPasswordChar = true;
            caja.ShortcutsEnabled = false;
            caja.MaxLength = 100;
            caja.TextChanged += (s, e) => errorProvider.SetError(caja, "");
            Controls.Add(caja);
        }

        private void ConfigurarBoton(Button boton, string texto, Point posicion, Color fondo, Color letra)
        {
            boton.Text = texto;
            boton.Location = posicion;
            boton.Size = new Size(170, 34);
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = ColorCafe;
            boton.BackColor = fondo;
            boton.ForeColor = letra;
            boton.Font = new Font("Times New Roman", 10.5F, FontStyle.Bold);
            boton.UseVisualStyleBackColor = false;
            boton.Cursor = Cursors.Hand;
            Controls.Add(boton);
        }

        private void Guardar()
        {
            errorProvider.Clear();

            if (txtActual.Text.Length == 0)
            {
                Avisar(txtActual, "Ingrese su contraseña actual.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNueva.Text))
            {
                Avisar(txtNueva, "Ingrese la nueva contraseña.");
                return;
            }

            if (txtNueva.Text.Length < DbUsuarios.LongitudMinimaContraseña)
            {
                Avisar(txtNueva, "La nueva contraseña debe tener al menos " + DbUsuarios.LongitudMinimaContraseña + " caracteres.");
                return;
            }

            if (txtNueva.Text != txtConfirmar.Text)
            {
                Avisar(txtConfirmar, "Las contraseñas no coinciden.");
                return;
            }

            if (txtNueva.Text == txtActual.Text)
            {
                Avisar(txtNueva, "La nueva contraseña debe ser distinta de la actual.");
                return;
            }

            if (!SesionActual.Activa)
            {
                MessageBox.Show("No se pudo identificar al usuario actual. Cierre sesión e inicie sesión de nuevo.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string error = DbUsuarios.CambiarContraseñaPropia(SesionActual.IdUsuario, txtActual.Text, txtNueva.Text);

            if (error != null)
            {
                MessageBox.Show(error, "No se pudo cambiar la contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (error.Contains("actual es incorrecta"))
                {
                    txtActual.Clear();
                    txtActual.Focus();
                }

                return;
            }

            MessageBox.Show("Tu contraseña se cambió correctamente. Úsala la próxima vez que inicies sesión.", "Contraseña cambiada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Avisar(Control campo, string mensaje)
        {
            errorProvider.SetError(campo, mensaje);
            campo.Focus();
        }

        // ABRE LA VENTANA PARA EL USUARIO QUE INICIO SESION
        public static void Abrir(IWin32Window dueño)
        {
            if (!SesionActual.Activa)
            {
                MessageBox.Show("No se pudo identificar al usuario actual. Cierre sesión e inicie sesión de nuevo.", "Cambiar contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (frmCambiarContrasena formulario = new frmCambiarContrasena())
            {
                formulario.ShowDialog(dueño);
            }
        }

        // AGREGA AL FINAL DEL MENU LATERAL UN BOTON "Mi contraseña" CON EL MISMO ESTILO QUE LOS DEMAS (plantilla = btnAyuda).
        // SE LLAMA ANTES DE ResponsiveHelper.Apply PARA QUE SE ADAPTE AL TAMAÑO IGUAL QUE LOS OTROS BOTONES.
        public static void AgregarBotonAlMenu(Panel menu, Button plantilla, Form dueño)
        {
            Button boton = new Button
            {
                Name = "btnCambiarContrasena",
                Text = "Mi contraseña",
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = plantilla.BackColor,
                ForeColor = plantilla.ForeColor,
                Font = plantilla.Font,
                Height = plantilla.Height,
                Margin = plantilla.Margin,
                Cursor = plantilla.Cursor,
                TextImageRelation = plantilla.TextImageRelation,
                UseVisualStyleBackColor = false
            };

            boton.FlatAppearance.BorderColor = plantilla.FlatAppearance.BorderColor;
            boton.FlatAppearance.BorderSize = plantilla.FlatAppearance.BorderSize;
            boton.FlatAppearance.MouseOverBackColor = plantilla.FlatAppearance.MouseOverBackColor;
            boton.FlatAppearance.MouseDownBackColor = plantilla.FlatAppearance.MouseDownBackColor;

            ToolTip ayuda = new ToolTip { ShowAlways = true };
            ayuda.SetToolTip(boton, "Cambia tu propia contraseña");
            boton.Tag = ayuda;      // se conserva la referencia para que el ToolTip siga vivo

            boton.Click += (s, e) => Abrir(dueño);

            menu.Controls.Add(boton);
            boton.BringToFront();   // al frente = ultimo del menu (los botones Dock.Top se apilan de atras hacia adelante)
        }
    }
}
