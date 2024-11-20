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

                    string query = @"INSERT INTO Proveedor 
                            (nombreProvedor, telefono, paginaContacto) 
                            VALUES (@NombreProvedor, @Telefono, @PaginaContacto)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreProvedor", nuevo.Nombre_Proveedor);
                        comando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", nuevo.Pagina_Contacto ?? (object)DBNull.Value);
                        
                        comando.ExecuteNonQuery();
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

