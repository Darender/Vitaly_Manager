using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataTipoProducto
    {
        private static List<TipoProducto> _cacheTipoProductos = new List<TipoProducto>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Lista los tipos de productos desde la base de datos o el caché si está vigente.
        /// </summary>
        public static List<TipoProducto> ListaTiposProductos(out string respuesta, out bool exito)
        {
            if (_cacheTipoProductos.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheTipoProductos;
            }

            List<TipoProducto> listaTipoProductos = new List<TipoProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM TipoProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        TipoProducto tipoProducto = new TipoProducto
                        {
                            IdTipoProducto = lector["idTipoProducto"] != DBNull.Value ? Convert.ToInt32(lector["idTipoProducto"]) : 0,
                            NombreTipoProducto = lector["nombreTipoProducto"] != DBNull.Value ? Convert.ToString(lector["nombreTipoProducto"])! : string.Empty
                        };

                        listaTipoProductos.Add(tipoProducto);
                    }

                    lector.Close();
                }

                // Actualiza el caché
                _cacheTipoProductos = listaTipoProductos;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return listaTipoProductos;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<TipoProducto>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<TipoProducto>();
            }
        }

        /// <summary>
        /// Agrega un nuevo tipo de producto.
        /// </summary>
        public static bool Agregar(TipoProducto nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO TipoProducto (nombreTipoProducto)
                                     OUTPUT INSERTED.idTipoProducto
                                     VALUES (@NombreTipoProducto)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreTipoProducto", nuevo.NombreTipoProducto);
                        nuevo.IdTipoProducto = (int)comando.ExecuteScalar();
                    }

                    conexion.Close();
                }

                // Actualizar el caché
                _cacheTipoProductos.Add(nuevo);

                mensaje = "Tipo de producto agregado exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                mensaje = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Modifica un tipo de producto existente.
        /// </summary>
        public static bool Modificar(TipoProducto modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"UPDATE TipoProducto
                                     SET nombreTipoProducto = @NombreTipoProducto
                                     WHERE idTipoProducto = @IdTipoProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreTipoProducto", modificado.NombreTipoProducto);
                        comando.Parameters.AddWithValue("@IdTipoProducto", modificado.IdTipoProducto);
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                // Actualizar en el caché
                var tipoProductoEnCache = _cacheTipoProductos.FirstOrDefault(tp => tp.IdTipoProducto == modificado.IdTipoProducto);
                if (tipoProductoEnCache != null)
                {
                    tipoProductoEnCache.NombreTipoProducto = modificado.NombreTipoProducto;
                }

                mensaje = "Tipo de producto modificado exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                mensaje = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Elimina un tipo de producto.
        /// </summary>
        public static bool Eliminar(int id, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"DELETE FROM TipoProducto
                                     WHERE idTipoProducto = @IdTipoProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoProducto", id);
                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            mensaje = "No se encontró el tipo de producto a eliminar.";
                            return false;
                        }
                    }

                    conexion.Close();
                }

                // Eliminar del caché
                _cacheTipoProductos.RemoveAll(tp => tp.IdTipoProducto == id);

                mensaje = "Tipo de producto eliminado exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                mensaje = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = $"Error inesperado: {ex.Message}";
                return false;
            }
        }
    }
}
