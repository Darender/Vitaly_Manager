using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataServicioGeneral
    {
        private static List<ServicioGeneral> _cacheProveedores = new List<ServicioGeneral>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Obtiene la lista de proveedores desde el caché o la base de datos si el caché está desactualizado.
        /// </summary>
        public static List<ServicioGeneral> ListaServicioGeneral(out string respuesta, out bool exito)
        {
            if (_cacheProveedores.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheProveedores;
            }

            List<ServicioGeneral> listaProveedores = new List<ServicioGeneral>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT * FROM ServicioGeneral", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idServGeneral = lector["idServGeneral"] != DBNull.Value ? Convert.ToInt32(lector["idServGeneral"]) : 0;
                        int idCliente = lector["idCliente"] != DBNull.Value ? Convert.ToInt32(lector["idCliente"]) : 0;
                        decimal ingresoTotal = lector["ingresoTotal"] != DBNull.Value ? Convert.ToDecimal(lector["ingresoTotal"]) : 0m;
                        DateTime fechaRealizado = lector["fechaRealizado"] != DBNull.Value ? Convert.ToDateTime(lector["fechaRealizado"]) : DateTime.MinValue;

                        ServicioGeneral nuevo = new ServicioGeneral
                        {
                            IdServGeneral = idServGeneral,
                            IdCliente = idCliente,
                            IngresoTotal = ingresoTotal,
                            FechaRealizado = fechaRealizado,
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
                return new List<ServicioGeneral>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<ServicioGeneral>();
            }
        }

    }
}
