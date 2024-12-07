using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataTipoProducto
    {

        /// <summary>
        /// Obtiene una lista de los tipos de productos que hay en la base de datos
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano de si fue exito o fracaso la consulta</param>
        /// <returns>Debuelve una lista de tipos de productos</returns>
        public static List<TipoProducto> ListaTiposProductos(out string respuesta, out bool exito)
        {
            List<TipoProducto> listaTiposProductos = new List<TipoProducto>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT idTipoProd, nombreTipoProd FROM TipoProducto", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idTipoProd = lector["idTipoProd"] != DBNull.Value ? Convert.ToInt32(lector["idTipoProd"]) : 0;
                        string nombreTipoProd = lector["nombreTipoProd"] != DBNull.Value ? Convert.ToString(lector["nombreTipoProd"])! : "N/A";

                        TipoProducto nuevo = new TipoProducto
                        {
                            ID_TipoProducto = idTipoProd,      
                            Nombre_Tipo_Producto = nombreTipoProd 
                        };

                        listaTiposProductos.Add(nuevo); 
                    }

                    lector.Close(); 
                }
                exito = true;
                respuesta = "Consulta exitosa";
                return listaTiposProductos;
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<TipoProducto>();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<TipoProducto>();
            }
        }


    }
}
