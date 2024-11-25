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
                        string nombreProveedor = lector["nombreProvedor"] != DBNull.Value ? Convert.ToString(lector["nombreProvedor"])! : "N/A";
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


    }
}