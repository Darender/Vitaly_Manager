using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataLoteProducto
    {
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
                        comando.Parameters.AddWithValue("@IVA", nuevo.IVA);
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
        }



        // LISTA PARA CONSULTA
        public static List<LoteProducto> ListaLoteProducto(out bool exito, out string mensaje)
        {
            List<LoteProducto> listaLoteProducto = new List<LoteProducto>();
            exito = true;  // Si la consulta es exitosa
            mensaje = string.Empty;  // Mensaje vacío por defecto

            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"SELECT idCatalogoProd, IVA, esMaterial, fechaIngreso, fechaVencimiento, cantidad, precioVenta, precioCompra, margenGanancia 
                             FROM LoteProducto";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();

                        while (lector.Read())
                        {
                            LoteProducto lote = new LoteProducto
                            {
                                ID_CatalogoProducto = Convert.ToInt32(lector["idCatalogoProd"]),
                                IVA = Convert.ToDecimal(lector["IVA"]),
                                EsMaterial = Convert.ToBoolean(lector["esMaterial"]),
                                Fecha_Ingreso = Convert.ToDateTime(lector["fechaIngreso"]),
                                Fecha_Vencimiento = Convert.ToDateTime(lector["fechaVencimiento"]),
                                Cantidad = Convert.ToInt32(lector["cantidad"]),
                                Precio_Venta = Convert.ToDecimal(lector["precioVenta"]),
                                Precio_Compra = Convert.ToDecimal(lector["precioCompra"]),
                                Margen_Ganancia = Convert.ToDecimal(lector["margenGanancia"])
                            };

                            listaLoteProducto.Add(lote);
                        }
                    }

                    mensaje = "Consulta realizada exitosamente.";
                }
            }
            catch (SqlException ex)
            {
                exito = false;
                mensaje = "Error en la base de datos: " + ex.Message;
            }
            catch (Exception ex)
            {
                exito = false;
                mensaje = "Error inesperado: " + ex.Message;
            }

            return listaLoteProducto;
        }


    }
}
