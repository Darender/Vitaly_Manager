using Microsoft.CodeAnalysis.Differencing;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataCatalogoProducto
    {
        private static List<CatalogoProducto> _cacheCatalogoProductos = new List<CatalogoProducto>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static List<CatalogoProducto> ListaCatalogoProductos(out string respuesta, out bool exito)
        {
            if (_cacheCatalogoProductos.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheCatalogoProductos;
            }

            List<CatalogoProducto> listaCatalogoProductos = new List<CatalogoProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM CatalogoProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        CatalogoProducto nuevo = new CatalogoProducto
                        {
                            IdCatalogoProducto = lector["idCatalogoProducto"] != DBNull.Value ? Convert.ToInt32(lector["idCatalogoProducto"]) : 0,
                            IdTipoProducto = lector["idTipoProducto"] != DBNull.Value ? Convert.ToInt32(lector["idTipoProducto"]) : 0,
                            NombreProducto = lector["nombreProducto"] != DBNull.Value ? Convert.ToString(lector["nombreProducto"])! : string.Empty,
                            IdProveedor = lector["idProveedor"] != DBNull.Value ? Convert.ToInt32(lector["idProveedor"]) : 0,
                            PaginaWebProducto = lector["paginaWebProducto"] != DBNull.Value ? Convert.ToString(lector["paginaWebProducto"]) : null,
                            Descripcion = lector["descripcion"] != DBNull.Value ? Convert.ToString(lector["descripcion"]) : null,
                            IdTipoUnidad = lector["idTipoUnidad"] != DBNull.Value ? Convert.ToInt32(lector["idTipoUnidad"]) : 0,
                            CantidadPorUnidad = lector["cantidadPorUnidad"] != DBNull.Value ? Convert.ToDecimal(lector["cantidadPorUnidad"]) : null
                        };

                        listaCatalogoProductos.Add(nuevo);
                    }

                    lector.Close();
                }

                _cacheCatalogoProductos = listaCatalogoProductos;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return listaCatalogoProductos;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<CatalogoProducto>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<CatalogoProducto>();
            }
        }

        public static bool Agregar(CatalogoProducto nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO CatalogoProducto 
                            (idTipoProducto, nombreProducto, idProveedor, paginaWebProducto, descripcion, idTipoUnidad, cantidadPorUnidad)
                            OUTPUT INSERTED.idCatalogoProducto
                            VALUES (@IdTipoProducto, @NombreProducto, @IdProveedor, @PaginaWebProducto, @Descripcion, @IdTipoUnidad, @CantidadPorUnidad)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoProducto", nuevo.IdTipoProducto);
                        comando.Parameters.AddWithValue("@NombreProducto", nuevo.NombreProducto);
                        comando.Parameters.AddWithValue("@IdProveedor", nuevo.IdProveedor);
                        comando.Parameters.AddWithValue("@PaginaWebProducto", nuevo.PaginaWebProducto ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Descripcion", nuevo.Descripcion ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", nuevo.IdTipoUnidad);
                        comando.Parameters.AddWithValue("@CantidadPorUnidad", nuevo.CantidadPorUnidad.HasValue ? nuevo.CantidadPorUnidad.Value : (object)DBNull.Value);

                        nuevo.IdCatalogoProducto = (int)comando.ExecuteScalar();
                    }

                    conexion.Close();
                }

                _cacheCatalogoProductos.Add(nuevo);

                mensaje = $"El producto {nuevo.NombreProducto} ha sido agregado exitosamente.";
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

        public static bool Modificar(CatalogoProducto modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"UPDATE CatalogoProducto 
                            SET idTipoProducto = @IdTipoProducto, 
                                nombreProducto = @NombreProducto, 
                                idProveedor = @IdProveedor, 
                                paginaWebProducto = @PaginaWebProducto, 
                                descripcion = @Descripcion, 
                                idTipoUnidad = @IdTipoUnidad, 
                                cantidadPorUnidad = @CantidadPorUnidad
                            WHERE idCatalogoProducto = @IdCatalogoProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoProducto", modificado.IdTipoProducto);
                        comando.Parameters.AddWithValue("@NombreProducto", modificado.NombreProducto);
                        comando.Parameters.AddWithValue("@IdProveedor", modificado.IdProveedor);
                        comando.Parameters.AddWithValue("@PaginaWebProducto", modificado.PaginaWebProducto ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Descripcion", modificado.Descripcion ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", modificado.IdTipoUnidad);
                        comando.Parameters.AddWithValue("@CantidadPorUnidad", modificado.CantidadPorUnidad.HasValue ? modificado.CantidadPorUnidad.Value : (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdCatalogoProducto", modificado.IdCatalogoProducto);
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                var productoEnCache = _cacheCatalogoProductos.FirstOrDefault(p => p.IdCatalogoProducto == modificado.IdCatalogoProducto);
                if (productoEnCache != null)
                {
                    productoEnCache.IdTipoProducto = modificado.IdTipoProducto;
                    productoEnCache.NombreProducto = modificado.NombreProducto;
                    productoEnCache.IdProveedor = modificado.IdProveedor;
                    productoEnCache.PaginaWebProducto = modificado.PaginaWebProducto;
                    productoEnCache.Descripcion = modificado.Descripcion;
                    productoEnCache.IdTipoUnidad = modificado.IdTipoUnidad;
                    productoEnCache.CantidadPorUnidad = modificado.CantidadPorUnidad;
                }

                mensaje = $"El producto {modificado.NombreProducto} ha sido modificado exitosamente.";
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

        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                bool encontrado = false;
                respuesta = $"El cliente es requerido en un lote: ";
                foreach (CompraLote lote in DataCompraLote.ListaCompraLotes(out _, out _))
                {
                    if (lote.IdCatalogoProducto == id)
                    {
                        encontrado = true;
                        respuesta += "\nLote:" + lote.IdCompraLote;
                    }
                }

                if (encontrado)
                {
                    return false;
                }


                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = "DELETE FROM CatalogoProducto WHERE idCatalogoProducto = @IdCatalogoProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCatalogoProducto", id);
                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            respuesta = "No se encontró el producto a eliminar.";
                            return false;
                        }
                    }

                    conexion.Close();
                }

                _cacheCatalogoProductos.RemoveAll(p => p.IdCatalogoProducto == id);

                respuesta = "El producto ha sido eliminado exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                respuesta = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                respuesta = $"Error inesperado: {ex.Message}";
                return false;
            }
        }
    }
}
