﻿using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public class DataIVA
    {/*
        /// <summary>
        /// Duelve el ultimo iva de la lista
        /// </summary>
        /// <param name="respuesta">Mensaje de respuesta</param>
        /// <param name="exito">Booleano de si fue exito o fracaso la consulta</param>
        /// <returns>Debuelve una entidad IVA</returns>
        public static Parametros UltimoIVA(out string respuesta, out bool exito)
        {
            Parametros listaTiposProductos = new();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand("SELECT TOP 1 * FROM IVA ORDER BY idIVA DESC", conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        int idIVA = lector["idIVA"] != DBNull.Value ? Convert.ToInt32(lector["idIVA"]) : 0;
                        decimal porcentaje = Convert.ToInt32(lector["porcentaje"]);

                        Parametros nuevo = new Parametros
                        {
                            ID_IVA = idIVA,
                            Porcentaje = porcentaje
                        };
                        exito = true;
                        respuesta = "Consulta exitosa";
                        return nuevo;
                    }

                    lector.Close();
                }
                exito = false;
                respuesta = "Consulta fallida ninguno encontrado";
                return new Parametros();
            }
            catch (SqlException ex)
            {
                exito = false;
                respuesta = $"Error en la base de datos: {ex.Message}";
                return new Parametros();
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error inesperado: {ex.Message}";
                return new Parametros();
            }
        }*/
    }
}
