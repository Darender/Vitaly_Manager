using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataTipoUnidad
    {
        public static List<TipoUnidad> ListaTiposUnidades(out string respuesta, out bool exito)
        {
            List<TipoUnidad> listaTiposProductos = new List<TipoUnidad>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM TipoUnidad", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idTipo = lector["idTipoUnidad"] != DBNull.Value ? Convert.ToInt32(lector["idTipoUnidad"]) : 0;
                        string tipo = lector["nombreTipoUnidad"] != DBNull.Value ? Convert.ToString(lector["nombreTipoUnidad"])! : "N/A";

                        TipoUnidad nuevo = new TipoUnidad
                        {
                            ID_TipoUnidad = idTipo,
                            Unidad_Medida = tipo,
                        };

                        listaTiposProductos.Add(nuevo);
                    }

                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaTiposProductos;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<TipoUnidad>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<TipoUnidad>();
            }
        }
    }
}
