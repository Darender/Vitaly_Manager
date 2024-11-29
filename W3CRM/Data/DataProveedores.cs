using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataProveedores
    {
        /// <summary>
        /// Obtiene la lista de proveedores desde la base de datos.
        /// </summary>
        /// <param name="respuesta">Devuelve un mensaje de la operación.</param>
        /// <param name="exito"></param>
        /// <returns>Lista de objetos Proveedor/>.</returns>
        /// 
        public static List<Proveedor> ListaProveedores(out string respuesta, out bool exito)
        {

            // Inicializa la lista de proveedores para almacenar los datos.
            List<Proveedor> listaProveedores = new List<Proveedor>();
            try
            {

                // Establece la conexión con la base de datos usando el MainServidor en Data
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    //Comando para consultar la tabla de proveedores
                    SqlCommand comando = new SqlCommand("SELECT * FROM Proveedor", conexion);
                    // Ejecuta el comando y obtiene los datos mediante un lector (SqlDataReader).
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        //Obtiene y valida cada campo en el registro, 
                        int idProveedor = lector["idProveedor"] != DBNull.Value ? Convert.ToInt32(lector["idProveedor"]) : 0;
                        string nombreProveedor = lector["nombreProvedor"] != DBNull.Value ? Convert.ToString(lector["nombreProvedor"])! : "N/A";
                        string? telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"]) : null;
                        string? paginaContacto = lector["PaginaContacto"] != DBNull.Value ? Convert.ToString(lector["PaginaContacto"]) : null;

                        //Cree un objeto proveedor con los datos para agregarse a la listaProveedor
                        Proveedor nuevo = new Proveedor
                        {
                            ID_Proveedor = idProveedor,
                            Nombre_Proveedor = nombreProveedor,
                            Telefono = telefono,
                            Pagina_Contacto = paginaContacto,
                        };

                        listaProveedores.Add(nuevo);
                    }
                    //Cierra el lector al procesarse los registros 
                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaProveedores;
            }
            // Captura errores relacionados con la base de datos (SqlException).
            catch (SqlException ex)
            { 
                //indica si la accion fallo y manda un mensaje de error
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<Proveedor>();
            }
            // Captura errores generales
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Proveedor>();
            }
        }
      
        public static void EliminarProveedor(int idProveedor, out string respuesta, out bool exito)
        {
            try
            {
                Console.WriteLine($"Intentando eliminar proveedor con ID: {idProveedor}");
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("DELETE FROM Proveedor WHERE idProveedor = @idProveedor", conexion);
                    comando.Parameters.AddWithValue("@idProveedor", idProveedor);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        exito = true;
                        respuesta = "Proveedor eliminado correctamente.";
                    }
                    else
                    {
                        exito = false;
                        respuesta = "No se encontró el proveedor.";
                    }
                }
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";

    }
}
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
