using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataProveedores
    {
        /// <summary>
        /// Agrega un nuevo cliente a la base de datos
        /// </summary>
        /// <param name="nuevo">Entidad de cliente que se agregara</param>
        /// <param name="mensaje">Mensaje de respuesta</param>
        /// <returns>Un booleano de si fue exito o fracaso la operacion</returns>
        public static bool Agregar(Proveedor nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    // Verificar si el nombre o el teléfono ya existen
                    string verificarQuery = @"SELECT COUNT(*) FROM Proveedor 
                                      WHERE nombreProvedor = @NombreProvedor 
                                      OR telefono = @Telefono";

                    using (SqlCommand verificarComando = new SqlCommand(verificarQuery, conexion))
                    {
                        verificarComando.Parameters.AddWithValue("@NombreProvedor", nuevo.Nombre_Proveedor);
                        verificarComando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);

                        int count = (int)verificarComando.ExecuteScalar(); // Obtiene el número de registros coincidentes

                        if (count > 0)
                        {
                            mensaje = "El nombre del proveedor o el teléfono ya existen en la base de datos.";
                            return false;
                        }
                    }

                    // Si no existen, proceder con la inserción
                    string insertarQuery = @"INSERT INTO Proveedor 
                                     (nombreProvedor, telefono, paginaContacto) 
                                     VALUES (@NombreProvedor, @Telefono, @PaginaContacto)";

                    using (SqlCommand insertarComando = new SqlCommand(insertarQuery, conexion))
                    {
                        insertarComando.Parameters.AddWithValue("@NombreProvedor", nuevo.Nombre_Proveedor);
                        insertarComando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                        insertarComando.Parameters.AddWithValue("@PaginaContacto", nuevo.Pagina_Contacto ?? (object)DBNull.Value);

                        insertarComando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                mensaje = $"El Proveedor {nuevo.Nombre_Proveedor} ha sido agregado exitosamente.";
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

