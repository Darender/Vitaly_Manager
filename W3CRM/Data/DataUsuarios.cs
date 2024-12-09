using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataUsuarios
    {
        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        /// <param name="nuevo">Entidad del usuario que se agregará.</param>
        /// <param name="mensaje">Mensaje de respuesta.</param>
        /// <returns>Booleano indicando si fue exitoso.</returns>
        public static bool Agregar(Usuario nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Verificar si el correo ya existe
                    string verificarQuery = @"SELECT 1 FROM Usuario WHERE Correo = @Correo;
";

                    using (SqlCommand verificarComando = new SqlCommand(verificarQuery, conexion))
                    {
                        verificarComando.Parameters.AddWithValue("@Correo", nuevo.Correo);
                        using (SqlDataReader reader = verificarComando.ExecuteReader())
                        {
                            if (reader.HasRows) // Si existe alguna fila, el correo ya está registrado.
                            {
                                mensaje = "El correo electrónico ya existe en la base de datos.";
                                return false;
                            }
                        }
                    }


                    // Insertar el nuevo usuario
                    string insertarQuery = @"INSERT INTO Usuario 
                                             (Nombre_Usuario,Correo, Contraseña, esAdmin) 
                                             VALUES (@Nombre, @Correo, @Contraseña, @esAdmin)";

                    using (SqlCommand insertarComando = new SqlCommand(insertarQuery, conexion))
                    {
                        insertarComando.Parameters.AddWithValue("@Nombre", nuevo.Nombre);
                       
                        insertarComando.Parameters.AddWithValue("@Correo", nuevo.Correo);
                        insertarComando.Parameters.AddWithValue("@Contraseña", nuevo.Contraseña);
                        insertarComando.Parameters.AddWithValue("@esAdmin", nuevo.esAdmin);

                        insertarComando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                mensaje = $"El usuario {nuevo.Nombre} ha sido agregado exitosamente.";
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
        /// Obtiene a todos los usuarios de la base de datos y los pone en una lista.
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano de si fue éxito o fracaso la consulta</param>
        /// <returns>Lista de usuarios</returns>
        public static List<Usuario> ListaUsuarios(out string respuesta, out bool exito)
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM Usuario", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idUsuario = lector["idUsuario"] != DBNull.Value ? Convert.ToInt32(lector["idUsuario"]) : 0;
                        string nombre = lector["nombreUsuario"] != DBNull.Value ? Convert.ToString(lector["nombreUsuario"])! : "N/A";
                        string? contrasena = lector["contrasena"] != DBNull.Value ? Convert.ToString(lector["contrasena"]) : null;
                        string? correo = lector["correo"] != DBNull.Value ? Convert.ToString(lector["correo"]) : null;
                        bool esAdmin = lector["esAdmin"] != DBNull.Value;

                        Usuario nuevo = new Usuario
                        {
                            ID_Usuario = idUsuario,
                            Nombre = nombre,
                            Correo = correo ?? string.Empty, // Proporciona un valor por defecto si es null
                            Contraseña = contrasena ?? string.Empty,
                            esAdmin = esAdmin,
                           
                        };


                        listaUsuarios.Add(nuevo);
                    }

                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaUsuarios;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<Usuario>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Usuario>();
            }
        }

        /// <summary>
        /// Eliminar al usuario de la base de datos
        /// </summary>
        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Verifica que el ID del usuario exista en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM usuario WHERE idUsuario = {id}";
                    SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                    int existe = (int)cmdExiste.ExecuteScalar();

                    if (existe == 0)
                    {
                        respuesta = "No se encontró el ID del usuario en la base de datos.";
                        return false;
                    }

                    // Elimina el usuario de la base de datos
                    string queryEliminar = $"DELETE FROM usuario WHERE IdUsuario = {id}";
                    new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();
                    conexion.Close();
                }
                respuesta = "Se eliminó exitosamente el usuario.";
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



        /// <summary>
        /// Modifica un usuario existente en la base de datos.
        /// </summary>
        /// <param name="usuarioModificado">Entidad de usuario que será modificada</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano indicando si la operación fue exitosa o fallida</returns>
        /*  public static bool Modificar(Usuario usuarioModificado, out string mensaje)
          {
              try
              {
                  using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                  {
                      conexion.Open();

                      string query = @"UPDATE Usuarios 
                               SET nombre = @Nombre, 
                                   apellidoPaterno = @ApellidoPaterno, 
                                   apellidoMaterno = @ApellidoMaterno, 
                                   correo = @Correo, 
                                   contraseña = @Contraseña, 
                                   rol = @Rol
                               WHERE idUsuario = @IdUsuario";

                      using (SqlCommand comando = new SqlCommand(query, conexion))
                      {
                          // Agregar los parámetros con valores
                          comando.Parameters.AddWithValue("@Nombre", usuarioModificado.Nombre);
                          comando.Parameters.AddWithValue("@ApellidoPaterno", usuarioModificado.ApellidoPaterno);
                          comando.Parameters.AddWithValue("@ApellidoMaterno", usuarioModificado.ApellidoMaterno ?? (object)DBNull.Value); // Opcional
                          comando.Parameters.AddWithValue("@Correo", usuarioModificado.Correo);
                          comando.Parameters.AddWithValue("@Contraseña", usuarioModificado.Contraseña);
                          comando.Parameters.AddWithValue("@Rol", usuarioModificado.Rol);
                          comando.Parameters.AddWithValue("@IdUsuario", usuarioModificado.ID_Usuario);

                          comando.ExecuteNonQuery();
                      }

                      conexion.Close();
                  }
                  mensaje = $"El usuario {usuarioModificado.Nombre} ha sido modificado exitosamente.";
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
          } */

        /// <summary>
        /// Elimina un usuario de la base de datos
        /// </summary>
        /// <param name="id">El id del usuario a eliminar</param>
        /// <param name="respuesta">Mensaje que describe el resultado de la operación</param>
        /// <returns>Un booleano que confirma si se pudo o no eliminar el usuario</returns>

    }
}

