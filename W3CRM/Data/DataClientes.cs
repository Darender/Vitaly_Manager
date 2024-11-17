using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataClientes
    {
        /// <summary>
        /// Modifica un cliente existente en la base de datos
        /// </summary>
        /// <param name="clienteModificado">Entidad de cliente que será modificada</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano indicando si la operación fue exitosa o fallida</returns>
        public static bool Modificar(Cliente clienteModificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"UPDATE Cliente 
                             SET nombreCliente = @NombreCliente, 
                                 apellidoP = @ApellidoP, 
                                 apellidoM = @ApellidoM, 
                                 telefono = @Telefono, 
                                 genero = @Genero, 
                                 contactoAlternativo = @Contacto_Alternativo, 
                                 edad = @Edad
                             WHERE idCliente = @IdCliente";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreCliente", clienteModificado.NombreCliente);
                        comando.Parameters.AddWithValue("@ApellidoP", clienteModificado.ApellidoP);
                        comando.Parameters.AddWithValue("@ApellidoM", clienteModificado.ApellidoM);
                        comando.Parameters.AddWithValue("@Telefono", clienteModificado.Telefono);
                        comando.Parameters.AddWithValue("@Genero", clienteModificado.Genero ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", clienteModificado.ContactoAlternativo ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Edad", clienteModificado.Edad.HasValue ? (object)clienteModificado.Edad.Value : DBNull.Value);
                        comando.Parameters.AddWithValue("@IdCliente", clienteModificado.ID_Cliente); // Asegúrate de tener el Id del cliente

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                mensaje = $"El cliente {clienteModificado.NombreCliente} ha sido modificado exitosamente.";
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


        /// <summary>
        /// Agrega un nuevo cliente a la base de datos
        /// </summary>
        /// <param name="nuevo">Entidad de cliente que se agregara</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano de si fue exito o fracaso la operacion</returns>
        public static bool Agregar(Cliente nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO Cliente 
                            (nombreCliente, apellidoP, apellidoM, telefono, genero, contactoAlternativo, edad, fechaRegistro) 
                            VALUES (@NombreCliente, @ApellidoP, @ApellidoM, @Telefono, @Genero, @Contacto_Alternativo, @Edad, @Fecha_Registro)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreCliente", nuevo.NombreCliente);
                        comando.Parameters.AddWithValue("@ApellidoP", nuevo.ApellidoP);
                        comando.Parameters.AddWithValue("@ApellidoM", nuevo.ApellidoM);
                        comando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                        comando.Parameters.AddWithValue("@Genero", nuevo.Genero ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", nuevo.ContactoAlternativo ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Edad", nuevo.Edad.HasValue ? (object)nuevo.Edad.Value : DBNull.Value);
                        comando.Parameters.AddWithValue("@Fecha_Registro", nuevo.FechaRegistro);

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                mensaje = $"El cliente {nuevo.NombreCliente} ha sido agregado exitosamente.";
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
                    SqlCommand comando = new SqlCommand("SELECT * FROM Cliente", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idCliente = lector["idCliente"] != DBNull.Value ? Convert.ToInt32(lector["idCliente"]) : 0;
                        string nombre = lector["nombreCliente"] != DBNull.Value ? Convert.ToString(lector["nombreCliente"])! : "N/A";
                        string apellidoP = lector["apellidoP"] != DBNull.Value ? Convert.ToString(lector["apellidoP"])! : "N/A";
                        string apellidoM = lector["apellidoM"] != DBNull.Value ? Convert.ToString(lector["apellidoM"])! : "N/A";
                        string? telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"]) : null;
                        string? genero = lector["genero"] != DBNull.Value ? Convert.ToString(lector["genero"]) : null;
                        string? contactoAlternativo = lector["contactoAlternativo"] != DBNull.Value ? Convert.ToString(lector["contactoAlternativo"]) : null;
                        int? edad = lector["edad"] != DBNull.Value ? Convert.ToInt32(lector["edad"]) : null;
                        DateTime fechaRegistro = Convert.ToDateTime(lector["fechaRegistro"]);

                        Cliente nuevo = new Cliente
                        {
                            ID_Cliente = idCliente,
                            NombreCliente = nombre,
                            ApellidoP = apellidoP,
                            ApellidoM = apellidoM,
                            Telefono = telefono,
                            Genero = genero,
                            ContactoAlternativo = contactoAlternativo,
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
        /*

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
