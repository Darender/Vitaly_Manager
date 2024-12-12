using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataProveedores
    {
        private static List<Proveedor> _cacheProveedores = new List<Proveedor>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Obtiene la lista de proveedores desde el caché o la base de datos si el caché está desactualizado.
        /// </summary>
        public static List<Proveedor> ListaProveedores(out string respuesta, out bool exito)
        {
            if (_cacheProveedores.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheProveedores;
            }

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
                        string nombreProveedor = lector["nombre"] != DBNull.Value ? Convert.ToString(lector["nombre"])! : "N/A";
                        string? telefono = lector["telefono"] != DBNull.Value ? Convert.ToString(lector["telefono"]) : null;
                        string? paginaContacto = lector["contactoAlternativo"] != DBNull.Value ? Convert.ToString(lector["contactoAlternativo"]) : null;

                        Proveedor nuevo = new Proveedor
                        {
                            IdProveedor = idProveedor,
                            Nombre = nombreProveedor,
                            Telefono = telefono,
                            ContactoAlternativo = paginaContacto,
                        };

                        listaProveedores.Add(nuevo);
                    }

                    lector.Close();
                }

                // Actualiza el caché
                _cacheProveedores = listaProveedores;
                _ultimoCache = DateTime.Now;

                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
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

        public static bool ModificarProveedor(Proveedor proveedorModificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"UPDATE Proveedor 
                                     SET nombre = @NombreProveedor, 
                                         telefono = @Telefono, 
                                         contactoAlternativo = @Contacto_Alternativo 
                                     WHERE idProveedor = @IdProveedor";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreProveedor", proveedorModificado.Nombre);
                        comando.Parameters.AddWithValue("@Telefono", proveedorModificado.Telefono);
                        comando.Parameters.AddWithValue("@Contacto_Alternativo", proveedorModificado.ContactoAlternativo ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@IdProveedor", proveedorModificado.IdProveedor);

                        comando.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                _cacheProveedores.RemoveAll(item => item.IdProveedor == proveedorModificado.IdProveedor);
                _cacheProveedores.Add(proveedorModificado);

                mensaje = $"El proveedor {proveedorModificado.Nombre} ha sido modificado exitosamente.";
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

        public static void EliminarProveedor(int idProveedor, out string respuesta, out bool exito)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"DELETE FROM Proveedor WHERE idProveedor = @idProveedor";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idProveedor", idProveedor);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            // Eliminar del caché
                            _cacheProveedores.RemoveAll(p => p.IdProveedor == idProveedor);

                            exito = true;
                            respuesta = "Proveedor eliminado correctamente.";
                        }
                        else
                        {
                            exito = false;
                            respuesta = "No se encontró el proveedor.";
                        }
                    }

                    conexion.Close();
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

        public static bool Agregar(Proveedor nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();

                    string query = @"INSERT INTO Proveedor 
                                     (nombre, telefono, contactoAlternativo) 
                                     OUTPUT INSERTED.idProveedor
                                     VALUES (@NombreProveedor, @Telefono, @ContactoAlternativo)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreProveedor", nuevo.Nombre);
                        comando.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                        comando.Parameters.AddWithValue("@ContactoAlternativo", nuevo.ContactoAlternativo ?? (object)DBNull.Value);

                        // Obtener el ID generado
                        nuevo.IdProveedor = (int)comando.ExecuteScalar();
                    }

                    conexion.Close();
                }

                // Agregar al caché
                _cacheProveedores.Add(nuevo);

                mensaje = $"El proveedor {nuevo.Nombre} ha sido agregado exitosamente.";
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
