using Microsoft.CodeAnalysis.Differencing;
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
                             SET nombre = @NombreCliente, 
                                 apellidos = @Apellidos, 
                                 telefono = @Telefono, 
                                 genero = @Genero, 
                                 contactoAlternativo = @Contacto_Alternativo, 
                                 edad = @Edad
                             WHERE idCliente = @idCliente";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreCliente", clienteModificado.Nombre);
                        comando.Parameters.AddWithValue("@Apellidos", clienteModificado.Apellidos);
                        comando.Parameters.AddWithValue("@Telefono", clienteModificado.Telefono);
                        comando.Parameters.AddWithValue("@Genero", clienteModificado.Genero ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", clienteModificado.ContactoAlternativo ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Edad", clienteModificado.Edad.HasValue ? (object)clienteModificado.Edad.Value : DBNull.Value);
                        comando.Parameters.AddWithValue("@IdCliente", clienteModificado.IdCliente); // Asegúrate de tener el Id del cliente

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }
                _cacheClientes.RemoveAll(item => item.IdCliente == clienteModificado.IdCliente);
                _cacheClientes.Add(clienteModificado);
                mensaje = $"El cliente {clienteModificado.Nombre} ha sido modificado exitosamente.";
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

                    string query = @"
                INSERT INTO Cliente 
                (nombre, apellidos, telefono, genero, contactoAlternativo, edad) 
                OUTPUT INSERTED.idCliente
                VALUES (@NombreCliente, @Apellidos, @Telefono, @Genero, @Contacto_Alternativo, @Edad)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreCliente", nuevo.Nombre);
                        comando.Parameters.AddWithValue("@Apellidos", nuevo.Apellidos);
                        comando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                        comando.Parameters.AddWithValue("@Genero", nuevo.Genero ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", nuevo.ContactoAlternativo ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Edad", nuevo.Edad.HasValue ? (object)nuevo.Edad.Value : DBNull.Value);

                        // Obtener el ID generado automáticamente
                        nuevo.IdCliente = (int)comando.ExecuteScalar();
                    }

                    conexion.Close();
                }

                // Agregar el nuevo cliente al caché
                _cacheClientes.Add(nuevo);

                mensaje = $"El cliente {nuevo.Nombre} ha sido agregado exitosamente con el ID {nuevo.IdCliente}.";
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



        private static List<Cliente> _cacheClientes = new List<Cliente>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Obtiene todos los clientes desde la base de datos o el caché si está vigente.
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano indicando si fue exitoso o no</param>
        /// <returns>Lista de clientes</returns>
        public static List<Cliente> ListaClientes(out string respuesta, out bool exito)
        {
            if (_cacheClientes.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheClientes;
            }

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
                        string nombre = lector["nombre"] != DBNull.Value ? Convert.ToString(lector["nombre"])! : "N/A";
                        string apellidos = lector["apellidos"] != DBNull.Value ? Convert.ToString(lector["apellidos"])! : "N/A";
                        string telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"])! : "N/A";
                        string? genero = lector["genero"] != DBNull.Value ? Convert.ToString(lector["genero"]) : null;
                        string? contactoAlternativo = lector["contactoAlternativo"] != DBNull.Value ? Convert.ToString(lector["contactoAlternativo"]) : null;
                        int? edad = lector["edad"] != DBNull.Value ? Convert.ToInt32(lector["edad"]) : null;

                        Cliente nuevo = new Cliente
                        {
                            IdCliente = idCliente,
                            Nombre = nombre,
                            Apellidos = apellidos,
                            Telefono = telefono,
                            Genero = genero,
                            ContactoAlternativo = contactoAlternativo,
                            Edad = edad,
                        };

                        listaClientes.Add(nuevo);
                    }

                    lector.Close();
                }

                // Actualizamos el caché
                _cacheClientes = listaClientes;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
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
        public static bool EliminarCliente(int id, out string respuesta)
        {
            try
            {
                bool encontrado = false;
                respuesta = $"El cliente es requerido en una venta o servicio general: ";
                foreach (Venta venta in DataVenta.ListaVentas(out _, out _))
                {
                    if (venta.IdCliente == id)
                    {
                        encontrado = true;
                        respuesta += "\nVenta:" + venta.FolioVenta;
                    }
                }

                if (encontrado)
                {
                    return false;
                }


                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Confirma que exista el id en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM cliente WHERE IdCliente = {id}";
                    SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                    int existe = (int)cmdExiste.ExecuteScalar();

                    if (existe == 0)
                    {
                        respuesta = "No se encontro el id del cliente en la base de datos";
                        return false;
                    }

                    string queryEliminar = $"DELETE FROM cliente WHERE IdCliente = {id}";
                    new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();
                    conexion.Close();
                }

                _cacheClientes.RemoveAll(item => item.IdCliente == id);
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
