using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataCatalogoProducto
    {
        private static List<CatalogoProducto> _cacheCatalogoProductos = new List<CatalogoProducto>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static CatalogoProducto? ObtenerPorId(int idCatalogoProducto, out string mensaje)
        {
            try
            {
                bool exito;
                var listaProductos = ListaCatalogoProductos(out mensaje, out exito);

                if (!exito)
                {
                    mensaje = $"Error al obtener la lista de productos: {mensaje}";
                    return null;
                }

                var producto = listaProductos.FirstOrDefault(p => p.IdCatalogoProducto == idCatalogoProducto);

                mensaje = producto != null ? "Producto encontrado exitosamente." : "Producto no encontrado.";
                return producto;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al buscar el producto por ID: {ex.Message}";
                return null;
            }
        }

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
                            IdCatalogoProducto = Convert.ToInt32(lector["idCatalogoProducto"]),
                            IdTipoProducto = Convert.ToInt32(lector["idTipoProducto"]),
                            NombreProducto = Convert.ToString(lector["nombreProducto"])!,
                            IdProveedor = Convert.ToInt32(lector["idProveedor"]),
                            PaginaWebProducto = lector["paginaWebProducto"] != DBNull.Value ? Convert.ToString(lector["paginaWebProducto"]) : null,
                            Descripcion = lector["descripcion"] != DBNull.Value ? Convert.ToString(lector["descripcion"]) : null,
                            IdTipoUnidad = Convert.ToInt32(lector["idTipoUnidad"]),
                            CantidadPorUnidad = lector["cantidadPorUnidad"] != DBNull.Value ? Convert.ToDecimal(lector["cantidadPorUnidad"]) : null,
                            EsMaterial = lector["esMaterial"] != DBNull.Value && Convert.ToBoolean(lector["esMaterial"]) // Nuevo campo
                        };

                        listaCatalogoProductos.Add(nuevo);
                    }
                }

                _cacheCatalogoProductos = listaCatalogoProductos;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return listaCatalogoProductos;
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
                            (idTipoProducto, nombreProducto, idProveedor, paginaWebProducto, descripcion, idTipoUnidad, cantidadPorUnidad, esMaterial)
                            OUTPUT INSERTED.idCatalogoProducto
                            VALUES (@IdTipoProducto, @NombreProducto, @IdProveedor, @PaginaWebProducto, @Descripcion, @IdTipoUnidad, @CantidadPorUnidad, @EsMaterial)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoProducto", nuevo.IdTipoProducto);
                        comando.Parameters.AddWithValue("@NombreProducto", nuevo.NombreProducto);
                        comando.Parameters.AddWithValue("@IdProveedor", nuevo.IdProveedor);
                        comando.Parameters.AddWithValue("@PaginaWebProducto", nuevo.PaginaWebProducto ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Descripcion", nuevo.Descripcion ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", nuevo.IdTipoUnidad);
                        comando.Parameters.AddWithValue("@CantidadPorUnidad", nuevo.CantidadPorUnidad ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EsMaterial", nuevo.EsMaterial); // Nuevo campo

                        nuevo.IdCatalogoProducto = (int)comando.ExecuteScalar();
                    }
                }

                _cacheCatalogoProductos.Add(nuevo);

                mensaje = $"El producto {nuevo.NombreProducto} ha sido agregado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al agregar el producto: {ex.Message}";
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
                                cantidadPorUnidad = @CantidadPorUnidad,
                                esMaterial = @EsMaterial
                            WHERE idCatalogoProducto = @IdCatalogoProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdTipoProducto", modificado.IdTipoProducto);
                        comando.Parameters.AddWithValue("@NombreProducto", modificado.NombreProducto);
                        comando.Parameters.AddWithValue("@IdProveedor", modificado.IdProveedor);
                        comando.Parameters.AddWithValue("@PaginaWebProducto", modificado.PaginaWebProducto ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Descripcion", modificado.Descripcion ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", modificado.IdTipoUnidad);
                        comando.Parameters.AddWithValue("@CantidadPorUnidad", modificado.CantidadPorUnidad ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EsMaterial", modificado.EsMaterial);
                        comando.Parameters.AddWithValue("@IdCatalogoProducto", modificado.IdCatalogoProducto);

                        comando.ExecuteNonQuery();
                    }
                }

                var productoEnCache = _cacheCatalogoProductos.FirstOrDefault(p => p.IdCatalogoProducto == modificado.IdCatalogoProducto);
                if (productoEnCache != null)
                {
                    productoEnCache.EsMaterial = modificado.EsMaterial; // Actualiza el campo
                }

                mensaje = $"El producto {modificado.NombreProducto} ha sido modificado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar el producto: {ex.Message}";
                return false;
            }
        }

        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                bool encontrado = false;
                respuesta = "El producto no se puede eliminar porque está siendo utilizado en los siguientes registros:";

                // Validar si el producto está asociado a algún lote de compra
                foreach (CompraLote lote in DataCompraLote.ListaCompraLotes(out _, out _))
                {
                    if (lote.IdCatalogoProducto == id)
                    {
                        encontrado = true;
                        respuesta += $"\n - Lote ID: {lote.IdCompraLote}";
                    }
                }

                // Si se encuentra asociado, no permitir la eliminación
                if (encontrado)
                {
                    return false;
                }

                // Eliminar el producto de la base de datos
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
                }

                // Eliminar el producto del caché
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
