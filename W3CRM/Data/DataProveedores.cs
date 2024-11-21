using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataProveedores
    {

        public static List<Proveedor> ListaProveedores(out string respuesta, out bool exito)
        {
            List<Proveedor> listaProveedores = new List<Proveedor>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM Proveedor", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idProveedor = lector["idProveedor"] != DBNull.Value ? Convert.ToInt32(lector["idProveedor"]) : 0;
                        string nombreProveedor = lector["nombreProveedor"] != DBNull.Value ? Convert.ToString(lector["nombreProveedor"])! : "N/A";
                        string? telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"]) : null;
                        string? paginaContacto = lector["PaginaContacto"] != DBNull.Value ? Convert.ToString(lector["PaginaContacto"]) : null;

                        Proveedor nuevo = new Proveedor
                        {
                            ID_Proveedor = idProveedor,
                            Nombre_Proveedor = nombreProveedor,
                            Telefono = telefono,
                            Pagina_Contacto = paginaContacto,
                        };

                        listaProveedores.Add(nuevo);
                    }

                    lector.Close();
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaProveedores;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<Proveedor>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Proveedor>();
            }
        }
        public static bool EliminarProveedor(int idProveedor, out string respuesta)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("DELETE FROM Proveedor WHERE idProveedor = @idProveedor", conexion);
                    comando.Parameters.AddWithValue("@idProveedor", idProveedor);

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        respuesta = "Proveedor eliminado correctamente.";
                        return true;
                    }
                    else
                    {
                        respuesta = "No se encontró un proveedor con el ID especificado.";
                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                respuesta = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                respuesta = $"Error inesperado: {ex.Message}";
                return false;
            }
        }
    }
}