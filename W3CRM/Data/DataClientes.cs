using System.Data.SqlClient;

namespace Vitaly_Manager.Data
{
    public class DataClientes
    {
        /// <summary>
        /// Elimina un cliente de la base de datos
        /// </summary>
        /// <param name="id">El id del cliente a eliminar</param>
        /// <returns>Un boleando que confirma si se pudo o no eliminar el cliente</returns>
        public static bool Eliminar(int id)
        {
            using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
            {
                conexion.Open();

                string queryExiste = $"SELECT COUNT(1) FROM Clientes WHERE ID = {id}";
                SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                int existe = (int)cmdExiste.ExecuteScalar();

                if (existe == 0) return false;

                string queryEliminar = $"DELETE FROM Clientes WHERE ID = {id}";
                new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();
            }
            return true;
        }
    }
}
