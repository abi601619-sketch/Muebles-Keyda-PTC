using Modelo.Conexión_DB;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modelo.Entidades
{
    public class DbProducción
    {
        private int IdProduccion;
        private int Pedido;
        private string Cliente;
        private string Mueble;
        private DateTime FechaEntrega;
        private string Estado;
        private int Progreso;

        public DbProducción()
        {
        }

        public DbProducción(int idProduccion, int pedido, string cliente, string mueble, DateTime fechaEntrega, string estado, int progreso)
        {
            IdProduccion1 = idProduccion;
            Pedido1 = pedido;
            Cliente1 = cliente;
            Mueble1 = mueble;
            FechaEntrega1 = fechaEntrega;
            Estado1 = estado;
            Progreso1 = progreso;
        }

        public int IdProduccion1 { get => IdProduccion; set => IdProduccion = value; }
        public int Pedido1 { get => Pedido; set => Pedido = value; }
        public string Cliente1 { get => Cliente; set => Cliente = value; }
        public string Mueble1 { get => Mueble; set => Mueble = value; }
        public DateTime FechaEntrega1 { get => FechaEntrega; set => FechaEntrega = value; }
        public string Estado1 { get => Estado; set => Estado = value; }
        public int Progreso1 { get => Progreso; set => Progreso = value; }

        public static DataTable CargarProducción()
        {
            try
            {
                using (SqlConnection conectar = Conexion.Conectar())
                {
                    string comando = "SELECT * FROM VerProduccion;";

                    SqlDataAdapter adapter = new SqlDataAdapter(comando, conectar);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt;
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La vista VerProduccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Ocurrió un error al cargar las producciones.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return new DataTable();
            }
        }

        public bool ObtenerProduccion()
        {
            string comandoSQL = @"
            SELECT
                IdProduccion,
                IdPedido,
                Cliente,
                Producto,
                [Fecha de Entrega],
                Progreso,
                Estado
            FROM VerProduccion
            WHERE IdProduccion = @IdProduccion;";

            using (SqlConnection conexion = Conexion.Conectar())
            {
                using (SqlCommand comandoObjeto =
                    new SqlCommand(comandoSQL, conexion))
                {
                    comandoObjeto.Parameters.AddWithValue("@IdProduccion", IdProduccion1);

                    try
                    {
                        using (SqlDataReader reader = comandoObjeto.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IdProduccion1 = Convert.ToInt32(reader["IdProduccion"]);

                                Pedido1 = Convert.ToInt32(reader["IdPedido"]);

                                Cliente1 = reader["Cliente"].ToString();

                                Mueble1 = reader["Producto"].ToString();

                                FechaEntrega1 = Convert.ToDateTime(reader["Fecha de Entrega"]);

                                Progreso1 = Convert.ToInt32(reader["Progreso"]);

                                Estado1 = reader["Estado"].ToString();

                                return true;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        switch (ex.Number)
                        {
                            case 208:
                                MessageBox.Show("La vista VerProduccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case 53:
                                MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case 4060:
                                MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            case -2:
                                MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;

                            default:
                                MessageBox.Show("Error al consultar la producción.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }

                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrió un error inesperado.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                }
            }

            return false;
        }


        public bool ActualizarProduccion()
        {
            if (Progreso1 < 0 || Progreso1 > 100)
            {
                MessageBox.Show("El progreso debe estar entre 0 y 100.", "Progreso inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            string consulta = @"
        SET XACT_ABORT ON;

        BEGIN TRY
            BEGIN TRANSACTION;

            DECLARE @IdPedido INT;
            DECLARE @ProgresoActual INT;

            -- Consultar y bloquear la producción seleccionada
            SELECT
                @IdPedido = IdPedido,
                @ProgresoActual = Progreso
            FROM dbo.Produccion WITH (UPDLOCK, HOLDLOCK)
            WHERE IdProduccion = @IdProduccion;

            IF @IdPedido IS NULL
                THROW 50001, 'No se encontró la producción.', 1;

            -- No permitir modificar una producción finalizada
            IF @ProgresoActual = 100
                THROW 50003,
                    'Esta producción ya está finalizada al 100 % y no se puede modificar.',
                    1;

            -- Actualizar solamente la producción seleccionada
            UPDATE dbo.Produccion
            SET Progreso = @Progreso
            WHERE IdProduccion = @IdProduccion;

            -- Finalizar el pedido solo si todos sus muebles
            -- tienen una producción relacionada al 100 %
            UPDATE dbo.Pedido
            SET Estado =
                CASE
                    WHEN EXISTS (
                        SELECT 1
                        FROM dbo.DetallePedido AS d
                        WHERE d.IdPedido = @IdPedido
                    )
                    AND NOT EXISTS (
                        SELECT 1
                        FROM dbo.DetallePedido AS d
                        LEFT JOIN dbo.Produccion AS p
                            ON p.IdDetallePedido = d.IdDetallePedido
                            AND p.IdPedido = d.IdPedido
                        WHERE d.IdPedido = @IdPedido
                          AND (
                              p.IdProduccion IS NULL
                              OR p.Progreso < 100
                          )
                    )
                    THEN 'Finalizado'
                    ELSE 'En proceso'
                END
            WHERE IdPedido = @IdPedido
              AND Estado <> 'Cancelado';

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF XACT_STATE() <> 0
                ROLLBACK TRANSACTION;

            THROW;
        END CATCH;
    ";

            using (SqlConnection conexion = Conexion.Conectar())
            {
                try
                {
                    using (SqlCommand comando =
                        new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.Add(
                            "@IdProduccion", SqlDbType.Int).Value =
                            IdProduccion1;

                        comando.Parameters.Add(
                            "@Progreso", SqlDbType.Int).Value =
                            Progreso1;

                        comando.ExecuteNonQuery();
                    }

                    Estado1 = Progreso1 == 100
                        ? "Finalizado"
                        : Progreso1 == 0
                            ? "Pendiente"
                            : "En producción";

                    MessageBox.Show("El progreso del mueble se actualizó correctamente.", "Actualización exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return true;
                }
                catch (SqlException ex)
                {
                    string mensaje;

                    switch (ex.Number)
                    {
                        case 50001:
                            mensaje = "No se encontró la producción.";
                            break;

                        case 50003:
                            mensaje = ex.Message;
                            break;

                        default:
                            mensaje =
                                "No se pudo actualizar la producción.\n\n" +
                                ex.Message;
                            break;
                    }

                    MessageBox.Show(
                        mensaje,
                        "No se pudo actualizar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Ocurrió un error al actualizar la producción.\n\n" +
                        ex.Message,
                        "Error de actualización",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }
        }


        public static int ContarProduccionesTotales()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string query = "SELECT COUNT(*) FROM Produccion";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        return Convert.ToInt32(comando.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La tabla Produccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error al contar las producciones totales.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return 0;
            }
        }

        public static int ContarProduccionesPendientes()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM Produccion
                        WHERE Progreso = 0";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        return Convert.ToInt32(comando.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La tabla Produccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error al contar las producciones pendientes.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return 0;
            }
        }

        public static int ContarProduccionesEnProceso()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM Produccion
                        WHERE Progreso BETWEEN 1 AND 99";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        return Convert.ToInt32(comando.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La tabla Produccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error al contar las producciones en proceso.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return 0;
            }
        }

        public static int ContarProduccionesFinalizadas()
        {
            try
            {
                using (SqlConnection conexion = Conexion.Conectar())
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM Produccion
                        WHERE Progreso = 100";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        return Convert.ToInt32(comando.ExecuteScalar());
                    }
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 208:
                        MessageBox.Show("La tabla Produccion no existe en la base de datos.", "Error 208", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 53:
                        MessageBox.Show("No se pudo establecer conexión con el servidor.", "Error 53", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case 4060:
                        MessageBox.Show("No se pudo acceder a la base de datos.", "Error 4060", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case -2:
                        MessageBox.Show("La consulta tardó demasiado tiempo.", "Error -2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show("Error al contar las producciones finalizadas.\n" + ex.Message, "Error " + ex.Number, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                return 0;
            }
        }
    }
}