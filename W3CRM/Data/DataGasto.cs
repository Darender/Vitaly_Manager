using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataGasto
    {
        public bool AgregarGasto(Gasto gasto, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO Gasto 
                            (concepto, monto, fecha) 
                            VALUES (@Concepto, @Monto, @Fecha)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Concepto", gasto.concepto);
                        comando.Parameters.AddWithValue("@Monto", gasto.monto);
                        comando.Parameters.AddWithValue("@Fecha", gasto.fecha);
                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                mensaje = $"El gasto ha sido agregado exitosamente.";
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

        public void ModificarGasto()
        {

        }

        public void EliminarGasto()
        {

        }

        public void ConsultarGastos()
        {

        }

        
    }
}
