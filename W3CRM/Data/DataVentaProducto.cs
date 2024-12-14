using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataVentaProducto
    {
        private static List<VentaProducto> _cacheVentasProductos = new List<VentaProducto>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static VentaProducto? ObtenerPorId(int idCompraLote, int folioVenta, out string mensaje)
        {
            try
            {
                // Obtiene la lista de ventas en memoria
                bool exito;
                var listaVentas = ListaVentasProductos(out mensaje, out exito);

                if (!exito)
                {
                    mensaje = $"Error al obtener la lista de ventas: {mensaje}";
                    return null;
                }

                // Busca la venta específica por ID
                var venta = listaVentas.Find(v => v.IdCompraLote == idCompraLote && v.FolioVenta == folioVenta);

                if (venta != null)
                {
                    mensaje = "Venta encontrada exitosamente.";
                    return venta;
                }

                // Si no encuentra la venta
                mensaje = "Venta no encontrada en la lista.";
                return null;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al buscar la venta por ID: {ex.Message}";
                return null;
            }
        }

        public static List<VentaProducto> ListaVentasProductos(out string respuesta, out bool exito)
        {
            if (_cacheVentasProductos.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheVentasProductos;
            }

            List<VentaProducto> lista = new List<VentaProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM VentaProducto";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            VentaProducto venta = new VentaProducto
                            {
                                IdCompraLote = Convert.ToInt32(lector["idCompraLote"]),
                                FolioVenta = Convert.ToInt32(lector["folioVenta"]),
                                CantidadVendida = Convert.ToInt32(lector["cantidadVendida"])
                            };
                            lista.Add(venta);
                        }
                    }
                }

                _cacheVentasProductos = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = "Error: " + ex.Message;
                return new List<VentaProducto>();
            }
        }

        public static bool Agregar(VentaProducto nueva, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO VentaProducto 
                            (idCompraLote, folioVenta, cantidadVendida)
                             VALUES (@IdCompraLote, @FolioVenta, @CantidadVendida);";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCompraLote", nueva.IdCompraLote);
                        comando.Parameters.AddWithValue("@FolioVenta", nueva.FolioVenta);
                        comando.Parameters.AddWithValue("@CantidadVendida", nueva.CantidadVendida);

                        comando.ExecuteNonQuery();
                    }
                }

                // Agregar al caché
                _cacheVentasProductos.Add(nueva);
                mensaje = "Venta de producto agregada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Modificar(VentaProducto modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE VentaProducto
                                     SET cantidadVendida = @CantidadVendida
                                     WHERE idCompraLote = @IdCompraLote AND folioVenta = @FolioVenta";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCompraLote", modificado.IdCompraLote);
                        comando.Parameters.AddWithValue("@FolioVenta", modificado.FolioVenta);
                        comando.Parameters.AddWithValue("@CantidadVendida", modificado.CantidadVendida);

                        comando.ExecuteNonQuery();
                    }
                }
                var ventaEnCache = _cacheVentasProductos.Find(v => v.IdCompraLote == modificado.IdCompraLote && v.FolioVenta == modificado.FolioVenta);
                if (ventaEnCache != null)
                {
                    _cacheVentasProductos.Remove(ventaEnCache);
                    _cacheVentasProductos.Add(modificado);
                }
                mensaje = "Venta de producto modificada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Eliminar(int idCompraLote, int folioVenta, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "DELETE FROM VentaProducto WHERE idCompraLote = @IdCompraLote AND folioVenta = @FolioVenta";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCompraLote", idCompraLote);
                        comando.Parameters.AddWithValue("@FolioVenta", folioVenta);
                        comando.ExecuteNonQuery();
                    }
                }
                _cacheVentasProductos.RemoveAll(v => v.IdCompraLote == idCompraLote && v.FolioVenta == folioVenta);
                mensaje = "Venta de producto eliminada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static List<VentaProducto> ObtenerCache()
        {
            return _cacheVentasProductos;
        }
    }
}
