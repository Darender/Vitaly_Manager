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
                                              WHERE Email = @Email";

                    using (SqlCommand verificarComando = new SqlCommand(verificarQuery, conexion))
                    {
                        verificarComando.Parameters.AddWithValue("@Email", nuevo.Email);

                        int count = (int)verificarComando.ExecuteScalar();
                        if (count > 0)
                        {
                            mensaje = "El correo electrónico ya existe en la base de datos.";
                            return false;
                        }
                    }

                    // Insertar el nuevo usuario
                    string insertarQuery = @"INSERT INTO Usuario 
                                             (Nombre_Usuario, Apellido_Paterno, Apellido_Materno, Email, Contraseña, Rol) 
                                             VALUES (@Nombre, @ApellidoPaterno, @ApellidoMaterno, @Email, @Contraseña, @Rol)";

                    using (SqlCommand insertarComando = new SqlCommand(insertarQuery, conexion))
                    {
                        insertarComando.Parameters.AddWithValue("@Nombre", nuevo.Nombre_Usuario);
                        insertarComando.Parameters.AddWithValue("@ApellidoPaterno", nuevo.Apellido_Paterno);
                        insertarComando.Parameters.AddWithValue("@ApellidoMaterno", nuevo.Apellido_Materno ?? (object)DBNull.Value);
                        insertarComando.Parameters.AddWithValue("@Email", nuevo.Email);
                        insertarComando.Parameters.AddWithValue("@Contraseña", nuevo.Contraseña);
                        insertarComando.Parameters.AddWithValue("@Rol", nuevo.Rol);

                        insertarComando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                mensaje = $"El usuario {nuevo.Nombre_Usuario} ha sido agregado exitosamente.";
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
