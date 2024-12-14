using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataGasto
    {
        private static List<Gasto> _cacheGastos = new List<Gasto>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static List<Gasto> ListaGastos(out string respuesta, out bool exito)
        {
            if (_cacheGastos.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheGastos;
            }

            List<Gasto> lista = new List<Gasto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM Gasto";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            Gasto gasto = new Gasto
                            {
                                IdGasto = Convert.ToInt32(lector["idGasto"]),
                                Monto = Convert.ToDecimal(lector["monto"]),
                                Descripcion = lector["descripcion"] as string,
                                IdTipoGasto = Convert.ToInt32(lector["idTipoGasto"]),
                                FechaRealizado = Convert.ToDateTime(lector["fechaRealizado"])
                            };
                            lista.Add(gasto);
                        }
                    }
                }

                _cacheGastos = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = "Error: " + ex.Message;
                return new List<Gasto>();
            }
        }

        public static bool Agregar(Gasto nuevo, out string mensaje, out int id)
        {
            id = 0; // Inicializa el ID como 0 para manejar errores si no se asigna
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO Gasto (monto, descripcion, idTipoGasto, fechaRealizado)
                             VALUES (@Monto, @Descripcion, @IdTipoGasto, @FechaRealizado);
                             SELECT SCOPE_IDENTITY();"; // Devuelve el ID generado

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Monto", nuevo.Monto);
                        comando.Parameters.AddWithValue("@Descripcion", (object)nuevo.Descripcion ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoGasto", nuevo.IdTipoGasto);
                        comando.Parameters.AddWithValue("@FechaRealizado", nuevo.FechaRealizado);

                        // Ejecuta el comando y obtiene el ID generado
                        id = Convert.ToInt32(comando.ExecuteScalar());
                    }
                }

                // Asigna el ID al objeto antes de agregarlo al caché
                nuevo.IdGasto = id;
                _cacheGastos.Add(nuevo);

                mensaje = "Gasto agregado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }


        public static bool Modificar(Gasto modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE Gasto
                                     SET monto = @Monto, descripcion = @Descripcion, idTipoGasto = @IdTipoGasto, fechaRealizado = @FechaRealizado
                                     WHERE idGasto = @IdGasto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdGasto", modificado.IdGasto);
                        comando.Parameters.AddWithValue("@Monto", modificado.Monto);
                        comando.Parameters.AddWithValue("@Descripcion", (object)modificado.Descripcion ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoGasto", modificado.IdTipoGasto);
                        comando.Parameters.AddWithValue("@FechaRealizado", modificado.FechaRealizado);

                        comando.ExecuteNonQuery();
                    }
                }
                var gastoEnCache = _cacheGastos.Find(g => g.IdGasto == modificado.IdGasto);
                if (gastoEnCache != null)
                {
                    _cacheGastos.Remove(gastoEnCache);
                    _cacheGastos.Add(modificado);
                }
                mensaje = "Gasto modificado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Eliminar(int id, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "DELETE FROM Gasto WHERE idGasto = @IdGasto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdGasto", id);
                        comando.ExecuteNonQuery();
                    }
                }
                _cacheGastos.RemoveAll(g => g.IdGasto == id);
                mensaje = "Gasto eliminado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static List<Gasto> ObtenerCache()
        {
            return _cacheGastos;
        }
    }
}
