using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataVenta
    {
        private static List<Venta> _cacheVentas = new List<Venta>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Obtiene la lista de ventas desde el caché o la base de datos si el caché está desactualizado.
        /// </summary>
        public static List<Venta> ListaVentas(out string respuesta, out bool exito)
        {
            if (_cacheVentas.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheVentas;
            }

            List<Venta> listaVentas = new List<Venta>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM Venta", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int folioVenta = lector["folioVenta"] != DBNull.Value ? Convert.ToInt32(lector["folioVenta"]) : 0;
                        int idCliente = lector["idCliente"] != DBNull.Value ? Convert.ToInt32(lector["idCliente"]) : 0;
                        decimal ingresoTotal = lector["ingresoTotal"] != DBNull.Value ? Convert.ToDecimal(lector["ingresoTotal"]) : 0;
                        DateTime fechaRealizado = lector["fechaRealizado"] != DBNull.Value ? Convert.ToDateTime(lector["fechaRealizado"]) : DateTime.MinValue;

                        Venta nuevo = new Venta
                        {
                            FolioVenta = folioVenta,
                            IdCliente = idCliente,
                            IngresoTotal = ingresoTotal,
                            FechaRealizado = fechaRealizado
                        };

                        listaVentas.Add(nuevo);
                    }

                    lector.Close();
                }

                // Actualiza el caché
                _cacheVentas = listaVentas;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return listaVentas;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<Venta>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Venta>();
            }
        }

        /// <summary>
        /// Agrega una nueva venta al sistema y al caché.
        /// </summary>
        public static bool AgregarVenta(Venta nuevaVenta, out string mensaje, out int idVentaGenerada)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO Venta (idCliente, ingresoTotal, fechaRealizado) 
                                     OUTPUT INSERTED.folioVenta 
                                     VALUES (@IdCliente, @IngresoTotal, @FechaRealizado)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCliente", nuevaVenta.IdCliente);
                        comando.Parameters.AddWithValue("@IngresoTotal", nuevaVenta.IngresoTotal);
                        comando.Parameters.AddWithValue("@FechaRealizado", nuevaVenta.FechaRealizado);

                        // Obtener el folio generado
                        nuevaVenta.FolioVenta = (int)comando.ExecuteScalar();
                        idVentaGenerada = nuevaVenta.FolioVenta;
                    }

                    conexion.Close();
                }

                // Agregar al caché
                _cacheVentas.Add(nuevaVenta);

                mensaje = "Venta agregada exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                idVentaGenerada = 0;
                mensaje = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                idVentaGenerada = 0;
                mensaje = $"Error inesperado: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Elimina una venta del sistema y del caché.
        /// </summary>
        public static bool EliminarVenta(int folioVenta, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = "DELETE FROM Venta WHERE folioVenta = @FolioVenta";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@FolioVenta", folioVenta);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            mensaje = "No se encontró la venta a eliminar.";
                            return false;
                        }
                    }

                    conexion.Close();
                }

                // Eliminar del caché
                _cacheVentas.RemoveAll(v => v.FolioVenta == folioVenta);

                mensaje = "Venta eliminada exitosamente.";
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
