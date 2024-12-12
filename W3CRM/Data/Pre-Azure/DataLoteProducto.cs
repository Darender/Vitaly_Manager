using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataLoteProducto
    {
     

        public static List<LoteProducto> ListaLoteProducto(out string respuesta, out bool exito)
        {
            List<LoteProducto> listaClientes = new List<LoteProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM LoteProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idLoteProd = lector["idLoteProd"] != DBNull.Value ? Convert.ToInt32(lector["idLoteProd"]) : 0;
                        int idCatalogoProd = lector["idCatalogoProd"] != DBNull.Value ? Convert.ToInt32(lector["idCatalogoProd"]) : 0;
                        decimal IVa = lector["IVA"] != DBNull.Value ? Convert.ToInt32(lector["IVA"]) : 0;
                        bool esMaterial = lector["esMaterial"] != DBNull.Value;
                        DateTime fechaIngreso = Convert.ToDateTime(lector["fechaIngreso"]);
                        DateTime fechaVencimiento = Convert.ToDateTime(lector["fechaVencimiento"]);
                        int cantidad = lector["cantidad"] != DBNull.Value ? Convert.ToInt32(lector["cantidad"]) : 0;
                        decimal precioVenta = lector["precioVenta"] != DBNull.Value ? Convert.ToInt32(lector["precioVenta"]) : 0;
                        decimal precioCompra = lector["precioCompra"] != DBNull.Value ? Convert.ToInt32(lector["precioCompra"]) : 0;
                        decimal margenGanancia = lector["margenGanancia"] != DBNull.Value ? Convert.ToInt32(lector["margenGanancia"]) : 0;

                        LoteProducto nuevo = new LoteProducto
                        {
                            ID_LoteProducto = idLoteProd,
                            ID_CatalogoProducto = idCatalogoProd,
                            IVA = IVa,
                            EsMaterial = esMaterial,
                            Fecha_Ingreso = fechaIngreso,
                            Fecha_Vencimiento = fechaVencimiento,
                            Cantidad = cantidad,
                            Precio_Venta = precioVenta,
                            Precio_Compra = precioCompra,
                            Margen_Ganancia = margenGanancia
                        };

                        listaClientes.Add(nuevo);
                    }

                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaClientes;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<LoteProducto>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<LoteProducto>();
            }
        }

        /// <summary>
        /// Agrega un nuevo lote de producto a la base de datos
        /// </summary>
        /// <param name="nuevo">Entidad de lote de producto que se agregara</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano de si fue exito o fragado la operacion</returns>
        public static bool Agregar(LoteProducto nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO loteProducto 
                            (idCatalogoProd, IVA, esMaterial, fechaIngreso, fechaVencimiento, cantidad, precioVenta, precioCompra, margenGanancia) 
                            VALUES (@ID_CatalogoProducto, @ID_IVA, @EsMaterial, @FechaIngreso, @FechaVencimiento, @Cantidad, @PrecioVenta, @PrecioCompra, @MargenGanancia)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@ID_CatalogoProducto", nuevo.ID_CatalogoProducto);
                        comando.Parameters.AddWithValue("@ID_IVA", DataParametros.UltimoIVA(out _, out _).IVA   );
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

    }
}
