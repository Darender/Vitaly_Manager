using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataClientes
    {
        public static List<Cliente> ListaClientes() { 
            List<Cliente> lista = new List<Cliente>();
            using (SqlConnection coneccion = new SqlConnection(MainServidor.Servidor))
            {
                coneccion.Open();
                SqlCommand comando = new SqlCommand("SELECT * FROM cliente", coneccion);
                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int folio = Convert.ToInt32(lector["folio"]);
                    int IDcliente = Convert.ToInt32(lector["IDcliente"]);
                    DateOnly fechaVenta = DateOnly.FromDateTime(Convert.ToDateTime(lector["fechaVenta"]));

                    Venta nuevo = new Venta
                    {
                        Folio = folio,
                        ID_Cliente = IDcliente,
                        Fecha_Venta = fechaVenta,
                    };

                    listaVentas.Add(nuevo);
                }
                lector.Close();
                return lista;
        }

        // Sin testeo
        /// <summary>
        /// Elimina un cliente de la base de datos
        /// </summary>
        /// <param name="id">El id del cliente a eliminar</param>
        /// <returns>Un boleando que confirma si se pudo o no eliminar el cliente</returns>
        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Confirma que exista el id en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM cliente WHERE ID = {id}";
                    SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                    int existe = (int)cmdExiste.ExecuteScalar();

                    if (existe == 0)
                    {
                        respuesta = "No se encontro el id del cliente en la base de datos";
                        return false;
                    }

                    // Confirma que no este siendo referenciado en otra parte necesaria de la base de datos
                    List<Venta> ListaVentas = DataVenta.ListaVentas();
                    foreach (Venta item in ListaVentas)
                    {
                        if(item.ID_Cliente == id)
                        {
                            respuesta = "No se puede eliminar un cliente que tenga una venta";
                            return false;
                        }
                    }

                    string queryEliminar = $"DELETE FROM cliente WHERE ID = {id}";
                    new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();
                }
                respuesta = "Se elimino exitosamente el cliente";
                return true;
            }
            catch (SqlException ex)
            {
                respuesta = $"Error en la base de datos (SqlException): {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                respuesta = $"Error inesperado (Exception): {ex.Message}";
                return false;
            }
        }
    }
}
