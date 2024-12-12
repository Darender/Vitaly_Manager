using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataVenta
    {
        public static List<Venta> ListaVentas(out string respuesta, out bool exito)
        {
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
                exito = true;
                respuesta = "Consulta exitosa";
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

    }
}
