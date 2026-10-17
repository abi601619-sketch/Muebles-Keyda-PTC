using Modelo.Conexión_DB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modelo.Entidades
{
    public class DbUsuarios
    {
        private int IdUsuario;
        private string Nombre;
        private string Usuario;
        private string Contraseña;
        private string Rol;
        private string Correo;
        private bool Estado;

        public DbUsuarios(int idUsuario, string nombre, string usuario, string contraseña, string rol, bool estado)
        {
            IdUsuario = idUsuario;
            Nombre = nombre;
            Usuario = usuario;
            Contraseña = contraseña;
            Rol = rol;
            Estado = estado;
        }

        public DbUsuarios() { }

        public int IdUsuario1 { get => IdUsuario; set => IdUsuario = value; }
        public string Nombre1 { get => Nombre; set => Nombre = value; }
        public string Usuario1 { get => Usuario; set => Usuario = value; }
        public string Contraseña1 { get => Contraseña; set => Contraseña = value; }
        public string Rol1 { get => Rol; set => Rol = value; }
        public string Correo1 { get => Correo; set => Correo = value; }
        public bool Estado1 { get => Estado; set => Estado = value; }

        public static DataTable CargarUsuarios()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string comando = "SELECT * FROM VerUsuarios;";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(comando, conexion))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La vista VerUsuarios no existe.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se puede conectar al servidor SQL.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se puede acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        public void InsertarUsuario()
        {
            try
            {
                string contraseñaHash = BCrypt.Net.BCrypt.HashPassword(Contraseña);

                string comandoSQL = @"INSERT INTO Usuario (Nombre, Usuario, Correo, Contraseña, Rol, Estado)
                    VALUES (@Nombre, @Usuario, @Correo, @Contraseña, @Rol, @Estado);";

                using (SqlConnection conexion = Conexion.Conectar())
                using (SqlCommand comando = new SqlCommand(comandoSQL, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", Nombre);
                    comando.Parameters.AddWithValue("@Usuario", Usuario);
                    comando.Parameters.AddWithValue("@Correo", Correo);
                    comando.Parameters.AddWithValue("@Contraseña", contraseñaHash);
                    comando.Parameters.AddWithValue("@Rol", Rol);
                    comando.Parameters.AddWithValue("@Estado", Estado);

                    comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        MessageBox.Show("El nombre de usuario o correo electrónico ya está registrado.", "ERR-SQL-005", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 2601:
                        MessageBox.Show("El nombre de usuario o correo electrónico ya está registrado.", "ERR-SQL-006", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 547:
                        MessageBox.Show("El rol seleccionado no existe.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 515:
                        MessageBox.Show("Hay campos obligatorios sin completar.", "ERR-SQL-008", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 53:
                        MessageBox.Show("No se puede conectar al servidor SQL.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se puede acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 8152:
                        MessageBox.Show("Datos demasiado largos para la columna.", "ERR-SQL-011", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DesactivarUsuario(int idUsuario)
        {
            try
            {
                string comandoSQL = @"UPDATE Usuario SET Estado = 0 WHERE IdUsuario = @IdUsuario;";

                using (SqlConnection conexion = Conexion.Conectar())
                using (SqlCommand comando = new SqlCommand(comandoSQL, conexion))
                {
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                    {
                        MessageBox.Show("No se encontró el usuario indicado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 547:
                        MessageBox.Show("No se puede desactivar el usuario.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se puede conectar al servidor SQL.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se puede acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool ExistenUsuarios()
        {
            try
            {
                string comandoSQL = "SELECT COUNT(*) FROM Usuario;";

                using (SqlConnection conexion = Conexion.Conectar())
                using (SqlCommand comando = new SqlCommand(comandoSQL, conexion))
                {
                    int cantidadUsuarios = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidadUsuarios > 0;
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        MessageBox.Show("No se puede conectar al servidor SQL.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        // LA BASE DE DATOS AÚN NO EXISTE (PRIMER USO): LA CONFIGURACIÓN INICIAL LA CREA
                        break;

                    case -2:
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void ActivarUsuario(int idUsuario)
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string consulta = @"
                UPDATE Usuario
                SET Estado = 1
                WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            throw new Exception("No se encontró el usuario seleccionado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al activar el usuario: " + ex.Message, ex);
            }
        }

        // ACTUALIZA EL USUARIO IdUsuario1 (NOMBRE DE USUARIO, CORREO Y ROL). SI nuevaContraseña TIENE TEXTO TAMBIEN CAMBIA LA CONTRASEÑA;
        // SI VIENE VACIA SE CONSERVA LA ACTUAL. DEVUELVE null SI TODO SALIO BIEN; SI NO, EL MENSAJE QUE SE LE DEBE MOSTRAR AL USUARIO.
        public string ActualizarUsuario(string nuevaContraseña)
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string rolActual;

                    using (SqlCommand consulta = new SqlCommand("SELECT Rol FROM Usuario WHERE IdUsuario = @IdUsuario;", conexion))
                    {
                        consulta.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = IdUsuario;

                        object resultado = consulta.ExecuteScalar();

                        if (resultado == null || resultado == DBNull.Value)
                            return "No se encontró el usuario seleccionado.";

                        rolActual = resultado.ToString();
                    }

                    // EL SISTEMA NO PUEDE QUEDAR SIN ADMINISTRADORES: UN ADMINISTRADOR SOLO PUEDE DEJAR DE SERLO SI HAY OTRO ACTIVO
                    if (rolActual == "Administrador" && Rol != "Administrador")
                    {
                        string consultaAdmins = "SELECT COUNT(*) FROM Usuario WHERE Rol = 'Administrador' AND Estado = 1 AND IdUsuario <> @IdUsuario;";

                        using (SqlCommand otros = new SqlCommand(consultaAdmins, conexion))
                        {
                            otros.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = IdUsuario;

                            if (Convert.ToInt32(otros.ExecuteScalar()) == 0)
                                return "No se puede cambiar el rol: debe existir al menos otro administrador activo.";
                        }
                    }

                    bool cambiarContraseña = !string.IsNullOrWhiteSpace(nuevaContraseña);

                    // EL NOMBRE ACOMPAÑA AL NOMBRE DE USUARIO SOLO CUANDO ERAN IGUALES (ASI SE REGISTRAN DESDE EL FORMULARIO)
                    string comandoSQL = @"UPDATE Usuario
                        SET Nombre = CASE WHEN Nombre = Usuario THEN @Usuario ELSE Nombre END,
                            Usuario = @Usuario,
                            Correo = @Correo,
                            Rol = @Rol" + (cambiarContraseña ? ",\n                            Contraseña = @Contraseña" : "") + @"
                        WHERE IdUsuario = @IdUsuario;";

                    using (SqlCommand comando = new SqlCommand(comandoSQL, conexion))
                    {
                        comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = IdUsuario;
                        comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = Usuario;
                        comando.Parameters.Add("@Correo", SqlDbType.VarChar, 150).Value = Correo;
                        comando.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = Rol;

                        if (cambiarContraseña)
                            comando.Parameters.Add("@Contraseña", SqlDbType.VarChar, 255).Value = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);

                        return comando.ExecuteNonQuery() == 0 ? "No se encontró el usuario seleccionado." : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                    case 2601:
                        return "El nombre de usuario ya está registrado.";

                    case 547:
                        return "El rol seleccionado no es válido.";

                    case 8152:
                        return "Alguno de los datos es demasiado largo.";

                    case 53:
                        return "No se puede conectar al servidor SQL.";

                    case 4060:
                        return "No se puede acceder a la base de datos.";

                    case -2:
                        return "Tiempo de espera agotado.";

                    case 208:
                        return "Tabla, vista o procedimiento no encontrado.";

                    default:
                        return "Error inesperado de SQL (" + ex.Number + "): " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                return "Error inesperado: " + ex.Message;
            }
        }

        // MINIMO DE CARACTERES PARA LA CONTRASEÑA QUE UN USUARIO ESCOGE PARA SI MISMO
        public const int LongitudMinimaContraseña = 6;

        // CAMBIA LA CONTRASEÑA DEL PROPIO USUARIO (idUsuario) VERIFICANDO ANTES SU CONTRASEÑA ACTUAL (BCRYPT).
        // DEVUELVE null SI SE CAMBIO; SI NO, EL MENSAJE QUE SE LE DEBE MOSTRAR AL USUARIO.
        public static string CambiarContraseñaPropia(int idUsuario, string contraseñaActual, string nuevaContraseña)
        {
            if (string.IsNullOrEmpty(contraseñaActual))
                return "Ingrese su contraseña actual.";

            if (string.IsNullOrWhiteSpace(nuevaContraseña))
                return "Ingrese la nueva contraseña.";

            if (nuevaContraseña.Length < LongitudMinimaContraseña)
                return "La nueva contraseña debe tener al menos " + LongitudMinimaContraseña + " caracteres.";

            if (nuevaContraseña == contraseñaActual)
                return "La nueva contraseña debe ser distinta de la actual.";

            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string hashActual;
                    bool activo;

                    using (SqlCommand consulta = new SqlCommand("SELECT Contraseña, Estado FROM Usuario WHERE IdUsuario = @IdUsuario;", conexion))
                    {
                        consulta.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                        using (SqlDataReader lector = consulta.ExecuteReader())
                        {
                            if (!lector.Read())
                                return "No se encontró el usuario.";

                            hashActual = lector["Contraseña"].ToString();
                            activo = Convert.ToBoolean(lector["Estado"]);
                        }
                    }

                    if (!activo)
                        return "Este usuario se encuentra desactivado.";

                    if (!BCrypt.Net.BCrypt.Verify(contraseñaActual, hashActual))
                        return "La contraseña actual es incorrecta.";

                    using (SqlCommand actualizar = new SqlCommand("UPDATE Usuario SET Contraseña = @Contraseña WHERE IdUsuario = @IdUsuario;", conexion))
                    {
                        actualizar.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
                        actualizar.Parameters.Add("@Contraseña", SqlDbType.VarChar, 255).Value = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);

                        actualizar.ExecuteNonQuery();
                    }

                    return null;
                }
            }
            catch (SqlException ex)
            {
                return "Error de base de datos (" + ex.Number + "): " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error inesperado: " + ex.Message;
            }
        }

        public bool CrearAdministradorInicial(string nombre, string usuario, string contraseña, string correo)
        {
            try
            {
                string contraseñaHash = BCrypt.Net.BCrypt.HashPassword(contraseña);

                string comandoSQL = @"INSERT INTO Usuario
                    (Nombre, Usuario, Contraseña,Correo, Rol, Estado)
                    VALUES (@Nombre, @Usuario, @Contraseña,@Correo ,'Administrador', 1);";

                using (SqlConnection conexion = Conexion.Conectar())
                using (SqlCommand comando = new SqlCommand(comandoSQL, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    comando.Parameters.AddWithValue("@Usuario", usuario);
                    comando.Parameters.AddWithValue("@Contraseña", contraseñaHash);
                    comando.Parameters.AddWithValue("@Correo", correo);


                    return comando.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 53:
                        MessageBox.Show("No se puede conectar al servidor SQL.", "ERR-SQL-001", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se puede acceder a la base de datos.", "ERR-SQL-002", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 2627:
                        MessageBox.Show("Registro duplicado por clave primaria o UNIQUE.", "ERR-SQL-005", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 2601:
                        MessageBox.Show("Registro duplicado por índice UNIQUE.", "ERR-SQL-006", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 547:
                        MessageBox.Show("Violación de clave foránea o restricción CHECK.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 515:
                        MessageBox.Show("Campo NOT NULL sin valor.", "ERR-SQL-008", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 245:
                        MessageBox.Show("Conversión o formato de datos incorrecto.", "ERR-SQL-009", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 8115:
                        MessageBox.Show("Desbordamiento numérico.", "ERR-SQL-010", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case 8152:
                        MessageBox.Show("Datos demasiado largos para la columna.", "ERR-SQL-011", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    default:
                        MessageBox.Show("Número de error: " + ex.Number + "\n\nMensaje: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}