using Microsoft.CodeAnalysis.Differencing;
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

        //Modifica un gasto ya existente en la base de datos, toma como parametro un objeto gasto
        //con los nuevos valores y retorna un booleano y un string de respuesta
        public bool ModificarGasto(Gasto gastoModificado, out string mensaje)
        {

            try
            {
                //Se crea la conexión con la base de datos
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    //Se abre la conexion
                    conexion.Open();

                    //Se crea el query de modificacion
                    string query = 
                        @"UPDATE Gasto 
                        SET 
                        concepto = @Concepto, 
                        monto = @Monto,
                        fecha = @Fecha,
                        idProductoComprado = @IdProductoComprado,
                        WHERE idCliente = @IdGasto";

                    //Se ejecuta el query de modificacion
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCliente", gastoModificado.idGasto);
                        comando.Parameters.AddWithValue("@Concepto", gastoModificado.concepto);
                        comando.Parameters.AddWithValue("@Monto", gastoModificado.monto);
                        comando.Parameters.AddWithValue("@Fecha", gastoModificado.fecha);
                        comando.Parameters.AddWithValue("@IdProductoComprado", gastoModificado.idProductoComprado ?? (object)DBNull.Value);

                        comando.ExecuteNonQuery();
                    }

                    //Se cierra la conexion
                    conexion.Close();
                }

                //se retorna true y un mensaje de exito
                mensaje = "El gasto ha sido modificado exitosamente.";
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

        //Consulta todos los gastos existentes en la base de datos, retorna
        //una lista con los gastos consultados, un string y un booleano de respuesta.
        public List<Gasto> ConsultarGastos(out string respuesta, out bool exito)
        {
            List<Gasto> listaGastos = new List<Gasto>();

            try
            {
                //Se crea la conexión con la base de datos
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    //Se abre la conexion
                    conexion.Open();

                    //Se crea el query de consulta
                    SqlCommand comando = new SqlCommand("SELECT * FROM Gasto", conexion);

                    //Se crea el lector de datos  
                    SqlDataReader lector = comando.ExecuteReader();

                    //Se crea un ciclo que se ejecutara mientras el lector registros que leer
                    while (lector.Read())
                    {
                        //Todos los datos de SQL se convierten en datos validos para C#
                        int idGasto = Convert.ToInt32(lector["idGasto"]);
                        string concepto = Convert.ToString(lector["concepto"]);
                        decimal monto = Convert.ToDecimal(lector["monto"]);
                        DateTime fecha = Convert.ToDateTime(lector["fecha"]);
                        int? idProductoComprado = lector["idProductoComprado"] != DBNull.Value ? Convert.ToInt32(lector["idProductoComprado"]) : null;

                        //Se crea el objeto de gasto con todos los datos leidos
                        Gasto gasto = new Gasto
                        {
                            idGasto = idGasto,
                            concepto = concepto,
                            monto = monto,
                            fecha = fecha,
                            idProductoComprado = idProductoComprado,
                        };

                        //Se agrega gasto a la lista 
                        listaGastos.Add(gasto);
                    }

                    //Se cierra la conexion
                    lector.Close();
                }

                //Se retorna la lista de gastos, un true y un mensaje de exito
                exito = true;
                respuesta = "Consulta exitosa";
                return listaGastos;
            }
            catch (SqlException ex)
            {
                //Mensaje de error de Sql
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new List<Gasto>();
            }
            catch (Exception ex)
            {
                //Mensaje de error del sistema
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new List<Gasto>();
            }
        }

        
    }
}
