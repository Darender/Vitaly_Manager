using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataTipoUnidad
    {
        private static List<TipoUnidad> _cacheTipoUnidades = new List<TipoUnidad>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Lista los tipos de unidades desde la base de datos o el caché si está vigente.
        /// </summary>
        public static List<TipoUnidad> ListaTiposUnidades(out string respuesta, out bool exito)
        {
            if (_cacheTipoUnidades.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheTipoUnidades;
            }

            List<TipoUnidad> listaTipoUnidades = new List<TipoUnidad>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM TipoUnidad", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        TipoUnidad tipoUnidad = new TipoUnidad
                        {
                            IdTipoUnidad = lector["idTipoUnidad"] != DBNull.Value ? Convert.ToInt32(lector["idTipoUnidad"]) : 0,
                            NombreTipoUnidad = lector["nombreTipoUnidad"] != DBNull.Value ? Convert.ToString(lector["nombreTipoUnidad"])! : string.Empty,
                            Abreviatura = lector["abreviatura"] != DBNull.Value ? Convert.ToString(lector["abreviatura"])! : string.Empty
                        };

                        listaTipoUnidades.Add(tipoUnidad);
                    }

                    lector.Close();
                }

                // Actualiza el caché
                _cacheTipoUnidades = listaTipoUnidades;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return listaTipoUnidades;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<TipoUnidad>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<TipoUnidad>();
            }
        }

        /// <summary>
        /// Agrega un nuevo tipo de unidad.
        /// </summary>
        public static bool Agregar(TipoUnidad nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO TipoUnidad (nombreTipoUnidad, abreviatura)
                                     OUTPUT INSERTED.idTipoUnidad
                                     VALUES (@NombreTipoUnidad, @Abreviatura)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreTipoUnidad", nuevo.NombreTipoUnidad);
                        comando.Parameters.AddWithValue("@Abreviatura", nuevo.Abreviatura);
                        nuevo.IdTipoUnidad = (int)comando.ExecuteScalar();
                    }

                    conexion.Close();
                }

                // Actualizar el caché
                _cacheTipoUnidades.Add(nuevo);

                mensaje = "Tipo de unidad agregado exitosamente.";
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
        /// Modifica un tipo de unidad existente.
        /// </summary>
        public static bool Modificar(TipoUnidad modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"UPDATE TipoUnidad
                                     SET nombreTipoUnidad = @NombreTipoUnidad,
                                         abreviatura = @Abreviatura
                                     WHERE idTipoUnidad = @IdTipoUnidad";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreTipoUnidad", modificado.NombreTipoUnidad);
                        comando.Parameters.AddWithValue("@Abreviatura", modificado.Abreviatura);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", modificado.IdTipoUnidad);
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                // Actualizar en el caché
                var tipoUnidadEnCache = _cacheTipoUnidades.FirstOrDefault(tu => tu.IdTipoUnidad == modificado.IdTipoUnidad);
                if (tipoUnidadEnCache != null)
                {
                    tipoUnidadEnCache.NombreTipoUnidad = modificado.NombreTipoUnidad;
                    tipoUnidadEnCache.Abreviatura = modificado.Abreviatura;
                }

                mensaje = "Tipo de unidad modificado exitosamente.";
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
        /// Elimina un tipo de unidad.
        /// </summary>
        public static bool Eliminar(int id, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"DELETE FROM TipoUnidad
                                     WHERE idTipoUnidad = @IdTipoUnidad";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoUnidad", id);
                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            mensaje = "No se encontró el tipo de unidad a eliminar.";
                            return false;
                        }
                    }

                    conexion.Close();
                }

                // Eliminar del caché
                _cacheTipoUnidades.RemoveAll(tu => tu.IdTipoUnidad == id);

                mensaje = "Tipo de unidad eliminado exitosamente.";
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
