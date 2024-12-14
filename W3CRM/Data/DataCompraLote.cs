﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataCompraLote
    {
        private static List<CompraLote> _cacheCompraLotes = new List<CompraLote>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static CompraLote? ObtenerPorId(int idCompraLote, out string mensaje)
        {
            try
            {
                // Obtiene la lista de compras en memoria
                bool exito;
                var listaCompras = ListaCompraLotes(out mensaje, out exito);

                if (!exito)
                {
                    mensaje = $"Error al obtener la lista de compras: {mensaje}";
                    return null;
                }

                // Busca la compra específica por ID
                var compra = listaCompras.FirstOrDefault(c => c.IdCompraLote == idCompraLote);

                if (compra != null)
                {
                    mensaje = "Compra encontrada exitosamente.";
                    return compra;
                }

                // Si no encuentra la compra
                mensaje = "Compra no encontrada en la lista.";
                return null;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al buscar la compra por ID: {ex.Message}";
                return null;
            }
        }

        public static List<CompraLote> ListaCompraLotes(out string respuesta, out bool exito)
        {
            if (_cacheCompraLotes.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheCompraLotes;
            }

            List<CompraLote> lista = new List<CompraLote>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM CompraLote";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            CompraLote compra = new CompraLote
                            {
                                IdCompraLote = Convert.ToInt32(lector["idCompraLote"]),
                                IdCatalogoProducto = Convert.ToInt32(lector["idCatalogoProducto"]),
                                CantidadUnidades = Convert.ToInt32(lector["cantidadUnidades"]),
                                CostoTotal = Convert.ToDecimal(lector["costoTotal"]),
                                IdGasto = Convert.ToInt32(lector["idGasto"]),
                                FechaVencimiento = lector["fechaVencimiento"] != DBNull.Value ? Convert.ToDateTime(lector["fechaVencimiento"]) : null,
                                EsMaterial = Convert.ToBoolean(lector["esMaterial"]),
                                PorcentajeMargenGanancia = Convert.ToDecimal(lector["porcentajeMargenGanancia"]),
                                IdParametros = Convert.ToInt32(lector["idParametros"])
                            };
                            lista.Add(compra);
                        }
                    }
                }

                _cacheCompraLotes = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = "Error: " + ex.Message;
                return new List<CompraLote>();
            }
        }

        public static bool Agregar(CompraLote nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO CompraLote 
                            (idCatalogoProducto, cantidadUnidades, costoTotal, idGasto, fechaVencimiento, esMaterial, porcentajeMargenGanancia, idParametros)
                             VALUES (@IdCatalogoProducto, @CantidadUnidades, @CostoTotal, @IdGasto, @FechaVencimiento, @EsMaterial, @PorcentajeMargenGanancia, @IdParametros);
                             SELECT SCOPE_IDENTITY();";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCatalogoProducto", nuevo.IdCatalogoProducto);
                        comando.Parameters.AddWithValue("@CantidadUnidades", nuevo.CantidadUnidades);
                        comando.Parameters.AddWithValue("@CostoTotal", nuevo.CostoTotal);
                        comando.Parameters.AddWithValue("@IdGasto", nuevo.IdGasto);
                        comando.Parameters.AddWithValue("@FechaVencimiento", nuevo.FechaVencimiento ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EsMaterial", nuevo.EsMaterial);
                        comando.Parameters.AddWithValue("@PorcentajeMargenGanancia", nuevo.PorcentajeMargenGanancia);
                        comando.Parameters.AddWithValue("@IdParametros", nuevo.IdParametros);

                        // Obtener el ID generado automáticamente
                        object result = comando.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int generatedId))
                        {
                            nuevo.IdCompraLote = generatedId; // Asignar el ID generado
                        }
                        else
                        {
                            mensaje = "Error al obtener el ID generado.";
                            return false;
                        }
                    }
                }

                // Agregar al caché con el ID generado
                _cacheCompraLotes.Add(nuevo);
                mensaje = "Compra lote agregada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }


        public static bool Modificar(CompraLote modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE CompraLote
                                     SET idCatalogoProducto = @IdCatalogoProducto, cantidadUnidades = @CantidadUnidades, costoTotal = @CostoTotal,
                                         idGasto = @IdGasto, fechaVencimiento = @FechaVencimiento, esMaterial = @EsMaterial,
                                         porcentajeMargenGanancia = @PorcentajeMargenGanancia, idParametros = @IdParametros
                                     WHERE idCompraLote = @IdCompraLote";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCompraLote", modificado.IdCompraLote);
                        comando.Parameters.AddWithValue("@IdCatalogoProducto", modificado.IdCatalogoProducto);
                        comando.Parameters.AddWithValue("@CantidadUnidades", modificado.CantidadUnidades);
                        comando.Parameters.AddWithValue("@CostoTotal", modificado.CostoTotal);
                        comando.Parameters.AddWithValue("@IdGasto", modificado.IdGasto);
                        comando.Parameters.AddWithValue("@FechaVencimiento", modificado.FechaVencimiento ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EsMaterial", modificado.EsMaterial);
                        comando.Parameters.AddWithValue("@PorcentajeMargenGanancia", modificado.PorcentajeMargenGanancia);
                        comando.Parameters.AddWithValue("@IdParametros", modificado.IdParametros);

                        comando.ExecuteNonQuery();
                    }
                }
                var compraEnCache = _cacheCompraLotes.Find(c => c.IdCompraLote == modificado.IdCompraLote);
                if (compraEnCache != null)
                {
                    _cacheCompraLotes.Remove(compraEnCache);
                    _cacheCompraLotes.Add(modificado);
                }
                mensaje = "Compra lote modificada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static bool Eliminar(int id, out string mensaje)
        {
            try
            {
                bool encontrado = false;
                mensaje = $"El lote es requerido en un material utilizado o en una venta: ";
                foreach (MaterialUtilizado material in DataMaterialUtilizado.ListaMaterialesUtilizados(out _, out _))
                {
                    if (material.IdCompraLote == id)
                    {
                        encontrado = true;
                        mensaje += "\nMaterial servicio realizado: " + material.IdServicioRealizado;
                    }
                }

                foreach (VentaProducto venta in DataVentaProducto.ListaVentasProductos(out _, out _))
                {
                    if (venta.IdCompraLote == id)
                    {
                        encontrado = true;
                        mensaje += "\nVenta: " + venta.FolioVenta;
                    }
                }

                if (encontrado)
                {
                    return false;
                }


                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "DELETE FROM CompraLote WHERE idCompraLote = @IdCompraLote";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdCompraLote", id);
                        comando.ExecuteNonQuery();
                    }
                }
                _cacheCompraLotes.RemoveAll(c => c.IdCompraLote == id);
                mensaje = "Compra lote eliminada exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public static List<CompraLote> ObtenerCache()
        {
            return _cacheCompraLotes;
        }
    }
}
