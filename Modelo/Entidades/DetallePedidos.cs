using Modelo.Conexión_DB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modelo.Entidades
{
    public class DetallePedidos
    {
        private int IdDetallePedido;
        private int Cantidad;
        private double Largo;
        private double Ancho;
        private double Alto;
        private string MuebleARealizar;
        private string Descripcion;

        public DetallePedidos(int idDetallePedido, int cantidad, double largo, double ancho, double alto, string muebleARealizar, string descripcion)
        {
            IdDetallePedido = idDetallePedido;
            Cantidad = cantidad;
            Largo = largo;
            Ancho = ancho;
            Alto = alto;
            MuebleARealizar = muebleARealizar;
            Descripcion = descripcion;
        }

        public int IdDetallePedido1 { get => IdDetallePedido; set => IdDetallePedido = value; }
        public int Cantidad1 { get => Cantidad; set => Cantidad = value; }
        public double Largo1 { get => Largo; set => Largo = value; }
        public double Ancho1 { get => Ancho; set => Ancho = value; }
        public double Alto1 { get => Alto; set => Alto = value; }
        public string MuebleARealizar1 { get => MuebleARealizar; set => MuebleARealizar = value; }
        public string Descripcion1 { get => Descripcion; set => Descripcion = value; }

        public static DataTable CargarDetallesPorPedido(int idPedido)
        {
            try
            {
                SqlConnection conectar = Conexion.Conectar();

                string comando = "SELECT * FROM DetallePedido WHERE IdPedido = @IdPedido;";
                SqlDataAdapter adapter = new SqlDataAdapter(comando, conectar);

                adapter.SelectCommand.Parameters.AddWithValue("@IdPedido", idPedido);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                return dt;
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
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 2627:
                        MessageBox.Show("Registro duplicado por clave primaria/UNIQUE.", "ERR-SQL-005", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 2601:
                        MessageBox.Show("Registro duplicado por índice UNIQUE.", "ERR-SQL-006", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 547:
                        MessageBox.Show("Violación de FK/CHECK.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 515:
                        MessageBox.Show("Campo NOT NULL sin valor.", "ERR-SQL-008", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 245:
                        MessageBox.Show("Conversión o formato de datos incorrecto.", "ERR-SQL-009", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 8115:
                        MessageBox.Show("Desbordamiento numérico.", "ERR-SQL-010", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 8152:
                        MessageBox.Show("Datos demasiado largos para la columna.", "ERR-SQL-011", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
            catch (Exception)
            {
                MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
        }


        public static bool EliminarDetalle(int idPedido, int idDetalle, bool confirmarCancelacion)
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                using (SqlCommand cmd = new SqlCommand(
                    "dbo.sp_EliminarDetallePedido", conexion))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IdPedido", SqlDbType.Int).Value =
                        idPedido;

                    cmd.Parameters.Add("@IdDetallePedido", SqlDbType.Int).Value =
                        idDetalle;

                    cmd.Parameters.Add("@ConfirmarCancelacion", SqlDbType.Bit).Value =
                        confirmarCancelacion;

                    object resultado = cmd.ExecuteScalar();

                    return resultado != null &&
                           resultado != DBNull.Value &&
                           Convert.ToBoolean(resultado);
                }
            }
            catch (SqlException ex)
            {
                string mensaje;

                switch (ex.Number)
                {
                    case 50001:
                        mensaje = "El pedido no existe.";
                        break;

                    case 50002:
                        mensaje = "Este es el último producto. Confirme la cancelación.";
                        break;

                    case 50003:
                        mensaje = "El producto no existe en este pedido.";
                        break;

                    case 50004:
                        mensaje = "No se pueden modificar pedidos finalizados o cancelados.";
                        break;

                    case 53:
                        mensaje = "No se puede conectar al servidor SQL.";
                        break;

                    case 4060:
                        mensaje = "No se puede acceder a la base de datos.";
                        break;

                    case -2:
                        mensaje = "Tiempo de espera agotado.";
                        break;

                    case 208:
                        mensaje = "No se encontró una tabla o un procedimiento almacenado.";
                        break;

                    case 547:
                        mensaje = "No se puede completar la operación por una relación entre registros.";
                        break;

                    default:
                        mensaje = "Error SQL (" + ex.Number + "): " + ex.Message;
                        break;
                }

                MessageBox.Show(
                    mensaje,
                    "Error al eliminar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error inesperado: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }
        public static DataTable CargarDetallesPedidos()
        {
            try
            {
                SqlConnection conectar = Conexion.Conectar();

                string comando = "SELECT * FROM DetallePedido;";
                SqlDataAdapter adapter = new SqlDataAdapter(comando, conectar);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                return dt;
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
                        MessageBox.Show("Tiempo de espera agotado.", "ERR-SQL-003", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 208:
                        MessageBox.Show("Tabla, vista o procedimiento no encontrado.", "ERR-SQL-004", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 2627:
                        MessageBox.Show("Registro duplicado por clave primaria/UNIQUE.", "ERR-SQL-005", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 2601:
                        MessageBox.Show("Registro duplicado por índice UNIQUE.", "ERR-SQL-006", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 547:
                        MessageBox.Show("Violación de FK/CHECK.", "ERR-SQL-007", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 515:
                        MessageBox.Show("Campo NOT NULL sin valor.", "ERR-SQL-008", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 245:
                        MessageBox.Show("Conversión o formato de datos incorrecto.", "ERR-SQL-009", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 8115:
                        MessageBox.Show("Desbordamiento numérico.", "ERR-SQL-010", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 8152:
                        MessageBox.Show("Datos demasiado largos para la columna.", "ERR-SQL-011", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
            catch (Exception)
            {
                MessageBox.Show("Error inesperado de SQL.", "ERR-SQL-999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
        }
    }
}