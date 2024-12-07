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
                    string verificarQuery = @"SELECT COUNT(*) FROM Usuario 
                                              WHERE Correo = @Correo";

                    using (SqlCommand verificarComando = new SqlCommand(verificarQuery, conexion))
                    {
                        verificarComando.Parameters.AddWithValue("@Correo", nuevo.Correo);

                        int count = (int)verificarComando.ExecuteScalar();
                        if (count > 0)
                        {
                            mensaje = "El correo electrónico ya existe en la base de datos.";
                            return false;
                        }
                    }

                    // Insertar el nuevo usuario
                    string insertarQuery = @"INSERT INTO Usuario 
                                             (Nombre_Usuario, Apellido_Paterno, Apellido_Materno, Correo, Contraseña, Rol) 
                                             VALUES (@Nombre, @ApellidoPaterno, @ApellidoMaterno, @Correo, @Contraseña, @Rol)";

                    using (SqlCommand insertarComando = new SqlCommand(insertarQuery, conexion))
                    {
                        insertarComando.Parameters.AddWithValue("@Nombre", nuevo.Nombre);
                        insertarComando.Parameters.AddWithValue("@ApellidoPaterno", nuevo.ApellidoPaterno);
                        insertarComando.Parameters.AddWithValue("@ApellidoMaterno", nuevo.ApellidoMaterno ?? (object)DBNull.Value);
                        insertarComando.Parameters.AddWithValue("@Correo", nuevo.Correo);
                        insertarComando.Parameters.AddWithValue("@Contraseña", nuevo.Contraseña);
                        insertarComando.Parameters.AddWithValue("@Rol", nuevo.Rol);

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
                        string apellidoP = lector["apellidoPaterno"] != DBNull.Value ? Convert.ToString(lector["apellidoPaterno"])! : "N/A";
                        string apellidoM = lector["apellidoMaterno"] != DBNull.Value ? Convert.ToString(lector["apellidoMaterno"])! : "N/A";
                        string? contraseña = lector["contraseña"] != DBNull.Value ? Convert.ToString(lector["contraseña"]) : null;
                        string? correo = lector["correo"] != DBNull.Value ? Convert.ToString(lector["correo"]) : null;
                        string? rol = lector["rol"] != DBNull.Value ? Convert.ToString(lector["rol"]) : null;
                        DateTime fechaRegistro = lector["fechaRegistro"] != DBNull.Value ? Convert.ToDateTime(lector["fechaRegistro"]) : DateTime.Now;

                        Usuario nuevo = new Usuario
                        {
                            ID_Usuario = idUsuario,
                            Nombre = nombre,
                            ApellidoPaterno = apellidoP,
                            ApellidoMaterno = apellidoM,
                            Correo = correo ?? string.Empty, // Proporciona un valor por defecto si es null
                            Contraseña = contraseña ?? string.Empty,
                            Rol = rol ?? string.Empty,
                            FechaRegistro = DateTime.Now // Inicializa FechaRegistro o usa el valor real de la base de datos si está disponible
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
        public static bool Eliminar(int id, out string respuesta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Verifica que el ID del usuario exista en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM usuario WHERE IdUsuario = {id}";
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
    }
}

