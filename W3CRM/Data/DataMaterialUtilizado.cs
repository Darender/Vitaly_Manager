using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataMaterialUtilizado
    {
        private static List<MaterialUtilizado> _cacheMateriales = new List<MaterialUtilizado>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static MaterialUtilizado? ObtenerPorId(int idServicioRealizado, int idCompraLote, out string mensaje)
        {
            try
            {
                // Obtiene la lista en memoria
                bool exito;
                var listaMateriales = ListaMaterialesUtilizados(out mensaje, out exito);

                if (!exito)
                {
                    mensaje = $"Error al obtener la lista de materiales utilizados: {mensaje}";
                    return null;
                }

                // Busca el material específico por las llaves compuestas
                var material = listaMateriales.Find(m => m.IdServicioRealizado == idServicioRealizado && m.IdCompraLote == idCompraLote);

                if (material != null)
                {
                    mensaje = "Material encontrado exitosamente.";
                    return material;
                }

                mensaje = "Material no encontrado en la lista.";
                return null;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al buscar el material por ID: {ex.Message}";
                return null;
            }
        }

        public static List<MaterialUtilizado> ListaMaterialesUtilizados(out string respuesta, out bool exito)
        {
            if (_cacheMateriales.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheMateriales;
            }

            List<MaterialUtilizado> lista = new List<MaterialUtilizado>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM MaterialUtilizado";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            MaterialUtilizado material = new MaterialUtilizado
                            {
                                IdServicioRealizado = Convert.ToInt32(lector["idServicioRealizado"]),
                                IdCompraLote = Convert.ToInt32(lector["idCompraLote"]),
                                CantidadEnvases = lector["cantidadEnvases"] != DBNull.Value ? Convert.ToDecimal(lector["cantidadEnvases"]) : null,
                                CantidadUnidades = lector["cantidadUnidades"] != DBNull.Value ? Convert.ToDecimal(lector["cantidadUnidades"]) : null
                            };
                            lista.Add(material);
                        }
                    }
                }

                _cacheMateriales = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = "Error: " + ex.Message;
                return new List<MaterialUtilizado>();
            }
        }

        public static bool Agregar(MaterialUtilizado nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO MaterialUtilizado 
                            (idServicioRealizado, idCompraLote, cantidadEnvases, cantidadUnidades)
                            VALUES (@IdServicioRealizado, @IdCompraLote, @CantidadEnvases, @CantidadUnidades)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioRealizado", nuevo.IdServicioRealizado);
                        comando.Parameters.AddWithValue("@IdCompraLote", nuevo.IdCompraLote);
                        comando.Parameters.AddWithValue("@CantidadEnvases", nuevo.CantidadEnvases ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@CantidadUnidades", nuevo.CantidadUnidades ?? (object)DBNull.Value);

                        comando.ExecuteNonQuery();
                    }
                }

                // Agregar al caché
                _cacheMateriales.Add(nuevo);
                mensaje = "Material utilizado agregado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Eliminar(int idServicioRealizado, int idCompraLote, out string mensaje)
        {
            try
            {

                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"DELETE FROM MaterialUtilizado 
                                     WHERE idServicioRealizado = @IdServicioRealizado AND idCompraLote = @IdCompraLote";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioRealizado", idServicioRealizado);
                        comando.Parameters.AddWithValue("@IdCompraLote", idCompraLote);
                        comando.ExecuteNonQuery();
                    }
                }

                // Remover del caché
                _cacheMateriales.RemoveAll(m => m.IdServicioRealizado == idServicioRealizado && m.IdCompraLote == idCompraLote);
                mensaje = "Material utilizado eliminado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al eliminar el material utilizado: {ex.Message}";
                return false;
            }
        }

        public static bool Modificar(MaterialUtilizado modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE MaterialUtilizado
                                     SET cantidadEnvases = @CantidadEnvases, cantidadUnidades = @CantidadUnidades
                                     WHERE idServicioRealizado = @IdServicioRealizado AND idCompraLote = @IdCompraLote";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioRealizado", modificado.IdServicioRealizado);
                        comando.Parameters.AddWithValue("@IdCompraLote", modificado.IdCompraLote);
                        comando.Parameters.AddWithValue("@CantidadEnvases", modificado.CantidadEnvases ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@CantidadUnidades", modificado.CantidadUnidades ?? (object)DBNull.Value);

                        comando.ExecuteNonQuery();
                    }
                }

                // Actualizar el caché
                var materialEnCache = _cacheMateriales.Find(m => m.IdServicioRealizado == modificado.IdServicioRealizado && m.IdCompraLote == modificado.IdCompraLote);
                if (materialEnCache != null)
                {
                    _cacheMateriales.Remove(materialEnCache);
                    _cacheMateriales.Add(modificado);
                }

                mensaje = "Material utilizado modificado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar el material utilizado: {ex.Message}";
                return false;
            }
        }

        public static List<MaterialUtilizado> ObtenerCache()
        {
            return _cacheMateriales;
        }
    }
}
