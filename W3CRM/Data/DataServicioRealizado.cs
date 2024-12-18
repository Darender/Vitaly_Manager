using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataServicioRealizado
    {
        private static List<ServicioRealizado> _cacheServicios = new List<ServicioRealizado>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static ServicioRealizado? ObtenerPorId(int idServicioRealizado, out string mensaje)
        {
            try
            {
                // Obtiene la lista de servicios en memoria
                bool exito;
                var listaServicios = ListaServiciosRealizados(out mensaje, out exito);

                if (!exito)
                {
                    mensaje = $"Error al obtener la lista de servicios: {mensaje}";
                    return null;
                }

                // Busca el servicio específico por ID
                var servicio = listaServicios.Find(s => s.IdServicioRealizado == idServicioRealizado);

                if (servicio != null)
                {
                    mensaje = "Servicio encontrado exitosamente.";
                    return servicio;
                }

                mensaje = "Servicio no encontrado en la lista.";
                return null;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al buscar el servicio por ID: {ex.Message}";
                return null;
            }
        }

        public static List<ServicioRealizado> ListaServiciosRealizados(out string respuesta, out bool exito)
        {
            if (_cacheServicios.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheServicios;
            }

            List<ServicioRealizado> lista = new List<ServicioRealizado>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM ServicioRealizado";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            ServicioRealizado servicio = new ServicioRealizado
                            {
                                IdServicioRealizado = Convert.ToInt32(lector["idServicioRealizado"]),
                                IdServicioGeneral = Convert.ToInt32(lector["idServicioGeneral"]),
                                IdCatalogoServicio = Convert.ToInt32(lector["idCatalogoServicio"]),
                                PrecioServicioCalculado = Convert.ToDecimal(lector["precioServicioCalculado"])
                            };
                            lista.Add(servicio);
                        }
                    }
                }

                _cacheServicios = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = "Error: " + ex.Message;
                return new List<ServicioRealizado>();
            }
        }

        public static bool Agregar(ServicioRealizado nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO ServicioRealizado 
                            (idServicioGeneral, idCatalogoServicio, precioServicioCalculado)
                             VALUES (@IdServicioGeneral, @IdCatalogoServicio, @PrecioServicioCalculado);
                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioGeneral", nuevo.IdServicioGeneral);
                        comando.Parameters.AddWithValue("@IdCatalogoServicio", nuevo.IdCatalogoServicio);
                        comando.Parameters.AddWithValue("@PrecioServicioCalculado", nuevo.PrecioServicioCalculado);

                        // Obtener el ID generado automáticamente
                        object result = comando.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int generatedId))
                        {
                            nuevo.IdServicioRealizado = generatedId; // Asignar el ID generado
                        }
                        else
                        {
                            mensaje = "Error al obtener el ID generado.";
                            return false;
                        }
                    }
                }

                // Agregar al caché con el ID generado
                _cacheServicios.Add(nuevo);
                mensaje = "Servicio realizado agregado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Modificar(ServicioRealizado modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE ServicioRealizado
                                     SET idServicioGeneral = @IdServicioGeneral,
                                         idCatalogoServicio = @IdCatalogoServicio,
                                         precioServicioCalculado = @PrecioServicioCalculado
                                     WHERE idServicioRealizado = @IdServicioRealizado";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioRealizado", modificado.IdServicioRealizado);
                        comando.Parameters.AddWithValue("@IdServicioGeneral", modificado.IdServicioGeneral);
                        comando.Parameters.AddWithValue("@IdCatalogoServicio", modificado.IdCatalogoServicio);
                        comando.Parameters.AddWithValue("@PrecioServicioCalculado", modificado.PrecioServicioCalculado);

                        comando.ExecuteNonQuery();
                    }
                }

                var servicioEnCache = _cacheServicios.Find(s => s.IdServicioRealizado == modificado.IdServicioRealizado);
                if (servicioEnCache != null)
                {
                    _cacheServicios.Remove(servicioEnCache);
                    _cacheServicios.Add(modificado);
                }
                mensaje = "Servicio realizado modificado exitosamente.";
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
                    string query = "DELETE FROM ServicioRealizado WHERE idServicioRealizado = @IdServicioRealizado";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdServicioRealizado", id);
                        comando.ExecuteNonQuery();
                    }
                }
                _cacheServicios.RemoveAll(s => s.IdServicioRealizado == id);
                mensaje = "Servicio realizado eliminado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static List<ServicioRealizado> ObtenerCache()
        {
            return _cacheServicios;
        }
    }
}
