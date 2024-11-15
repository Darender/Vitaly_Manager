using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataClientes
    {

        /*
        /// <summary>
        /// Optiene a todos los clientes de la base de datos y los pone una lista
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano de si fue exito o fracaso la consulta</param>
        /// <returns></returns>
        public static List<Cliente> ListaClientes(out string respuesta, out bool exito)
        {
            List<Cliente> listaClientes = new List<Cliente>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM cliente", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idCliente = lector["idCliente"] != DBNull.Value ? Convert.ToInt32(lector["idCliente"]) : 0;
                        string nombre = lector["nombre"] != DBNull.Value ? Convert.ToString(lector["nombre"])! : "N/A";
                        string apellidoP = lector["apellidoP"] != DBNull.Value ? Convert.ToString(lector["apellidoP"])! : "N/A";
                        string apellidoM = lector["apellidoM"] != DBNull.Value ? Convert.ToString(lector["apellidoM"])! : "N/A";
                        string? telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"]) : null;
                        string? genero = lector["genero"] != DBNull.Value ? Convert.ToString(lector["genero"]) : null;
                        string? contactoAlternativo = lector["contactoAlternativo"] != DBNull.Value ? Convert.ToString(lector["contactoAlternativo"]) : null;
                        int? edad = lector["edad"] != DBNull.Value ? Convert.ToInt32(lector["edad"]) : null;
                        DateOnly fechaRegistro = lector["fechaRegistro"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(lector["fechaRegistro"])) : DateOnly.MinValue;

                        Cliente nuevo = new Cliente
                        {
                            ID_Cliente = idCliente,
                            Nombre = nombre,
                            ApellidoP = apellidoP,
                            ApellidoM = apellidoM,
                            Telefono = telefono,
                            Genero = genero,
                            ContractoAlternativo = contactoAlternativo,
                            Edad = edad,
                            FechaRegistro = fechaRegistro
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
                return new List<Cliente>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Cliente>();
            }
        }


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
        }*/
    }
}
