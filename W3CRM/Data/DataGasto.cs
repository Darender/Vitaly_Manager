using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataGasto
    {
        //Agrega un gasto a la base de datos, toma como parametro el objeto tipo gasto que agregara
        //y regresa un booleano y un string de respuesta
        public bool AgregarGasto(Gasto gasto, out string mensaje)
        {
            try
            {
                //Se crea la conexión con la base de datos
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    //Se abre la conexion
                    conexion.Open();

                    //Se crea el query de insercion
                    string query = @"INSERT INTO Gasto 
                            (concepto, monto, fecha) 
                            VALUES (@Concepto, @Monto, @Fecha)";

                    //Se ejecuta el query para insertar los datos del objeto en la tabla gasto
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Concepto", gasto.concepto);
                        comando.Parameters.AddWithValue("@Monto", gasto.monto);
                        comando.Parameters.AddWithValue("@Fecha", gasto.fecha);
                        comando.ExecuteNonQuery();
                    }

                    //Cierre de conexion
                    conexion.Close();
                }

                //se retorna true y un mensaje de exito
                mensaje = $"El gasto ha sido agregado exitosamente.";
                return true;
            }
            catch (SqlException ex)
            {
                //Mensaje de error de Sql
                mensaje = $"Error en la base de datos: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                //Mensaje de error del sistema
                mensaje = $"Error inesperado: {ex.Message}";
                return false;
            }

        }

        public void ModificarGasto()
        {

        }

        //Elimina un gasto de la base de datos, toma como parametro la id del gasto a eliminar y
        //retorna un booleano y un string de respuesta.
        public bool EliminarGasto(int idGasto, out string respuesta)
        {
            try
            {
                //Se crea la conexión con la base de datos
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    //Se abre la conexion
                    conexion.Open();

                    // Confirma que exista el id en la base de datos
                    string queryExiste = $"SELECT COUNT(1) FROM Gasto WHERE IdGasto = {idGasto}";
                    SqlCommand cmdExiste = new SqlCommand(queryExiste, conexion);
                    int existe = (int)cmdExiste.ExecuteScalar();

                    //Si no existe, se retorna false y un mensaje notifocandolo
                    if (existe == 0)
                    {
                        respuesta = "No se encontro el gasto en la base de datos";
                        return false;
                    }

                    //Se ejecuta el query de eliminación
                    string queryEliminar = $"DELETE FROM Gasto WHERE IdGasto = {idGasto}";
                    new SqlCommand(queryEliminar, conexion).ExecuteNonQuery();

                    //Cierre de conexion
                    conexion.Close();
                }

                //se retorna true y un mensaje de exito
                respuesta = "Se elimino exitosamente el gasto";
                return true;
            }
            catch (SqlException ex)
            {
                //Mensaje de error de Sql
                respuesta = $"Error en la base de datos (SqlException): {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                //Mensaje de error del sistema
                respuesta = $"Error inesperado (Exception): {ex.Message}";
                return false;
            }

        }

        public void ConsultarGastos()
        {

        }

        
    }
}
