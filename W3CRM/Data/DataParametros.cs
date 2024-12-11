using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataParametros
    {
        public static Parametros UltimoIVA(out string respuesta, out bool exito)
        {
            Parametros listaTiposProductos = new();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT TOP 1 * FROM Parametros ORDER BY idParametros DESC", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idIVA = lector["idParametros"] != DBNull.Value ? Convert.ToInt32(lector["idParametros"]) : 0;
                        decimal porcentaje = Convert.ToInt32(lector["IVA"]);

                        Parametros nuevo = new Parametros
                        {
                            ID_Parametros = idIVA,
                            IVA = porcentaje
                        };
                        exito = true;
                        respuesta = "Consulta exitosa";
                        return nuevo;
                    }

                    lector.Close();
                }
                exito = false;
                respuesta = "Consulta fallida ninguno encontrado";
                return new Parametros();
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new Parametros();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new Parametros();
            }
        }
    }
}
