using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataCatalogoProducto
    {
        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Confirma que exista el id en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM catalogoProducto WHERE idCatalogoProd = {id}";
                    SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                    int existe = (int)cmdExiste.ExecuteScalar();

                    if (existe == 0)
                    {
                        respuesta = "No se encontro el id del producto en la base de datos";
                        return false;
                    }

                    string queryEliminar = $"DELETE FROM CatalogoProducto WHERE idCatalogoProd = {id}";
                    new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();
                    conexion.Close();
                }
                respuesta = "Se elimino exitosamente el producto";
                return true;
            }
            catch (SqlException ex)
            {
                respuesta = $"Error en la base de datos (SqlException): {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                respuesta = $"Error inesperado (Exception): {ex.Message}";
                return false;
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
                            (nombreProducto, cantidadUnidades, paginaProducto, idProveedor, idTipoUnidad, idTipoProd) 
                            VALUES (@NombreProducto, @CantidadUnidades, @PaginaProducto, @IdProveedor, @IdTipoUnidad, @IdTipoProd)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreProducto", nuevo.Nombre_Producto);
                        comando.Parameters.AddWithValue("@CantidadUnidades", nuevo.Cantidad_Unidades);
                        comando.Parameters.AddWithValue("@PaginaProducto", nuevo.Pagina_Producto ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdProveedor", nuevo.ID_Proveedor);
                        comando.Parameters.AddWithValue("@IdTipoUnidad", nuevo.ID_TipoUnidad);
                        comando.Parameters.AddWithValue("@IdTipoProd", nuevo.ID_TipoProducto);

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                mensaje = $"El cliente {nuevo.Nombre_Producto} ha sido agregado exitosamente.";
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
        /// Optiene todos los productos de la base de datos y los pone una lista
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano de si fue exito o fracaso la consulta</param>
        /// <returns>Lista de catalogo de productos</returns>
        public static List<CatalogoProducto> ListaCatalogoProductos(out string respuesta, out bool exito)
        {
            List<CatalogoProducto> listaCatalogoProductos = new List<CatalogoProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM catalogoProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idProducto = lector["idCatalogoProd"] != DBNull.Value ? Convert.ToInt32(lector["idCatalogoProd"]) : 0;
                        string nombreProducto = lector["nombreProducto"] != DBNull.Value ? Convert.ToString(lector["nombreProducto"])! : "N/A";
                        int unidades = lector["cantidadUnidades"] != DBNull.Value ? Convert.ToInt32(lector["cantidadUnidades"]) : 0;
                        string? paginaProducto = lector["paginaProducto"] != DBNull.Value ? Convert.ToString(lector["paginaProducto"])! : null;
                        int idProveedor = lector["idProveedor"] != DBNull.Value ? Convert.ToInt32(lector["idProveedor"]) : 0;
                        int idTipoUnidad = lector["idTipoUnidad"] != DBNull.Value ? Convert.ToInt32(lector["idTipoUnidad"]) : 0;
                        int idTipoProducto = lector["idTipoProd"] != DBNull.Value ? Convert.ToInt32(lector["idTipoProd"]) : 0;

                        CatalogoProducto nuevo = new CatalogoProducto
                        {
                            ID_CatalogoProducto = idProducto,
                            Nombre_Producto = nombreProducto,
                            Cantidad_Unidades = unidades,
                            Pagina_Producto = paginaProducto,
                            ID_Proveedor = idProveedor,
                            ID_TipoUnidad = idTipoUnidad,
                            ID_TipoProducto = idTipoProducto
                        };

                        listaCatalogoProductos.Add(nuevo);
                    }

                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
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

    }
}
