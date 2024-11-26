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

    }
}
