using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataCatalogoProducto
    {
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
                    SqlCommand comando = new SqlCommand("SELECT * FROM CatalogoProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idCatalogoProd = lector["idCatalogoProd"] != DBNull.Value ? Convert.ToInt32(lector["idCatalogoProd"]) : 0;
                        string nombreProducto = lector["nombreProducto"] != DBNull.Value ? Convert.ToString(lector["nombreProducto"])! : "N/A";
                        int cantidadUnidades = lector["cantidadUnidades"] != DBNull.Value ? Convert.ToInt32(lector["cantidadUnidades"]) : 0;
                        string? paginaProducto = lector["paginaProducto"] != DBNull.Value ? Convert.ToString(lector["paginaProducto"])! : null;
                        int idProveedor = lector["idProveedor"] != DBNull.Value ? Convert.ToInt32(lector["idProveedor"]) : 0;
                        int idTipoUnidad = lector["idTipoUnidad"] != DBNull.Value ? Convert.ToInt32(lector["idTipoUnidad"]) : 0;
                        int idTipoProd = lector["idTipoProd"] != DBNull.Value ? Convert.ToInt32(lector["idTipoProd"]) : 0;

                        CatalogoProducto nuevo = new CatalogoProducto
                        {
                            ID_CatalogoProducto = idCatalogoProd,
                            Nombre_Producto = nombreProducto,
                            Cantidad_Unidades = cantidadUnidades,
                            Pagina_Producto = paginaProducto,
                            ID_Proveedor = idProveedor,
                            ID_TipoUnidad = idTipoUnidad,
                            ID_TipoProducto = idTipoProd
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
