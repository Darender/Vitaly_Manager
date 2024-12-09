using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataCatalogoServicio
    {
        public static bool Agregar(CatalogoServicio nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO CatalogoServicio 
                            (nombreServ, descripcion) 
                            VALUES (@Nombre, @Descripcion)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", nuevo.NombreServ);
                        comando.Parameters.AddWithValue("@Descripcion", nuevo.Descripcion);

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                mensaje = $"El servicio {nuevo.NombreServ} ha sido agregado exitosamente.";
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
