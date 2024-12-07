using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataLoteProducto
    {/*
        /// <summary>
        /// Agrega un nuevo lote de producto a la base de datos
        /// </summary>
        /// <param name="nuevo">Entidad de lote de producto que se agregará</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano que indica si la operación fue exitosa o fallida</returns>
        public static bool Agregar(LoteProducto nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO LoteProducto 
                            (idCatalogoProd, IVA, esMaterial, fechaIngreso, fechaVencimiento, cantidad, precioVenta, precioCompra, margenGanancia) 
                            VALUES (@ID_CatalogoProd, @IVA, @EsMaterial, @FechaIngreso, @FechaVencimiento, @Cantidad, @PrecioVenta, @PrecioCompra, @MargenGanancia)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID_CatalogoProd", nuevo.ID_CatalogoProducto);
                        comando.Parameters.AddWithValue("@IVA", nuevo.ID_Parametros);
                        comando.Parameters.AddWithValue("@EsMaterial", nuevo.EsMaterial ? 1 : 0);
                        comando.Parameters.AddWithValue("@FechaIngreso", nuevo.Fecha_Ingreso);
                        comando.Parameters.AddWithValue("@FechaVencimiento", nuevo.Fecha_Vencimiento);
                        comando.Parameters.AddWithValue("@Cantidad", nuevo.Cantidad);
                        comando.Parameters.AddWithValue("@PrecioVenta", nuevo.Precio_Venta);
                        comando.Parameters.AddWithValue("@PrecioCompra", nuevo.Precio_Compra);
                        comando.Parameters.AddWithValue("@MargenGanancia", nuevo.Margen_Ganancia);

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                mensaje = "Lote de producto agregado exitosamente.";
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
        }*/
    }
}
