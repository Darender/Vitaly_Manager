﻿using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ComprasController : Controller
    {
        public List<TipoProducto> ListasTipoProductos = DataTipoProducto.ListaTiposProductos(out _, out _);
        public List<CatalogoProducto> ListaProductos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);   
        public IActionResult AgregarLoteProducto() {
            return View(this);
        }

        public IActionResult ConsultaCompras()
        {
            return View(this);
        }

        [HttpPost]
        public JsonResult AgregarNuevaCompra(
        string producto,
        int cantidadUnidades,
        decimal costoTotal,
        string? fechaVencimiento,
        decimal porcentajeMargenGanancia)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar la compra.";
            List<string> fallidos = new List<string>();
            int idProducto;

            try
            {
                if(!int.TryParse(producto, out idProducto))
                {
                    mensaje = "Erro al validar el producto.";
                    fallidos.Add("producto");
                } else if (idProducto <= 0)
                {
                    mensaje = "Debe seleccionar un producto válido.";
                    fallidos.Add("producto");
                }

                // Validación de cantidad
                if (cantidadUnidades <= 0)
                {
                    mensaje = "La cantidad debe ser mayor a 0.";
                    fallidos.Add("cantidadUnidades");
                }

                // Validación del costo total
                if (costoTotal <= 0)
                {
                    mensaje = "El costo total debe ser mayor a 0.";
                    fallidos.Add("costoTotal");
                }

                // Validación del margen de ganancia
                if (porcentajeMargenGanancia < 0)
                {
                    mensaje = "El margen de ganancia no puede ser negativo.";
                    fallidos.Add("porcentajeMargenGanancia");
                }

                // Validación de fecha de vencimiento (opcional)
                DateTime? fechaVencimientoParsed = null;
                if (!string.IsNullOrWhiteSpace(fechaVencimiento))
                {
                    if (DateTime.TryParse(fechaVencimiento, out DateTime parsedDate))
                    {
                        fechaVencimientoParsed = parsedDate;
                    }
                    else
                    {
                        mensaje = "La fecha de vencimiento no es válida.";
                        fallidos.Add("fechaVencimiento");
                    }
                }

                if (fallidos.Count == 0)
                {
                    Gasto nuevoGasto = new Gasto
                    {
                        Monto = costoTotal,
                        Descripcion = $"Compra de {cantidadUnidades} {ObtenerNombreProducto(idProducto)}",
                        IdTipoGasto = 1,
                        FechaRealizado = DateTime.Now
                    };

                    int idGastoGenerado;

                    if (!DataGasto.Agregar(nuevoGasto, out mensaje, out idGastoGenerado))
                        return Json(new { success = false, message = mensaje, errores = fallidos });

                    // Crear la nueva compra
                    CompraLote nuevaCompra = new CompraLote
                    {
                        IdCatalogoProducto = idProducto,
                        CantidadUnidades = cantidadUnidades,
                        CostoTotal = costoTotal,
                        FechaVencimiento = fechaVencimientoParsed,
                        PorcentajeMargenGanancia = porcentajeMargenGanancia,
                        IdParametros = 1,
                        IdGasto = idGastoGenerado
                    };

                    // Lógica para guardar la compra en la base de datos
                    resultado = DataCompraLote.Agregar(nuevaCompra, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar la compra: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }


        [HttpGet]
        public JsonResult ObtenerCompras()
        {
            try
            {
                // Simular la obtención de datos desde una base de datos
                var listaCompras = DataCompraLote.ListaCompraLotes(out string mensaje, out bool exito);

                if (!exito)
                {
                    return Json(new { success = false, message = mensaje });
                }

                // Formatear los datos para enviarlos al frontend
                var comprasFormateadas = listaCompras.Select(compra => new
                {
                    idCompraLote = compra.IdCompraLote,
                    nombreProducto = ObtenerNombreProducto(compra.IdCatalogoProducto),
                    cantidadUnidades = compra.CantidadUnidades,
                    costoTotal = compra.CostoTotal,
                    fechaVencimiento = compra.FechaVencimiento?.ToString("yyyy-MM-dd"),
                });

                return Json(new { success = true, data = comprasFormateadas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener las compras: {ex.Message}" });
            }
        }

        public static string ObtenerNombreProducto(int idProducto)
        {
            var producto = DataCatalogoProducto.ListaCatalogoProductos(out _, out _).FirstOrDefault(p => p.IdCatalogoProducto == idProducto);
            return producto != null ? producto.NombreProducto : "Producto no encontrado";
        }

        [HttpGet]
        public JsonResult ObtenerProductosPorTipo(int tipoId)
        {
            var productosFiltrados = ListaProductos
                .Where(p => p.IdTipoProducto == tipoId)
                .Select(p => new
                {
                    p.IdCatalogoProducto,
                    p.NombreProducto
                })
                .ToList();

            return Json(new { success = true, data = productosFiltrados });
        }

        [HttpGet]
        public JsonResult ObtenerProductosFiltrados(int? proveedorId = null, int? tipoProductoId = null)
        {
            try
            {
                string mensaje;
                bool exito;

                // Obtener todos los productos desde la base de datos o caché
                var productos = DataCatalogoProducto.ListaCatalogoProductos(out mensaje, out exito);

                if (!exito)
                {
                    return Json(new { success = false, message = mensaje });
                }

                // Filtrar productos si se proporcionan filtros
                if (proveedorId.HasValue)
                {
                    productos = productos.Where(p => p.IdProveedor == proveedorId.Value).ToList();
                }

                if (tipoProductoId.HasValue)
                {
                    productos = productos.Where(p => p.IdTipoProducto == tipoProductoId.Value).ToList();
                }

                return Json(new { success = true, data = productos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al filtrar productos: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult ObtenerProveedores()
        {
            try
            {
                string mensaje;
                bool exito;

                // Obtener la lista de proveedores desde la base de datos o caché
                var proveedores = DataProveedores.ListaProveedores(out mensaje, out exito);

                if (!exito)
                {
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new { success = true, data = proveedores });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener proveedores: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult ObtenerTiposProductos()
        {
            try
            {
                string mensaje;
                bool exito;

                // Obtener la lista de tipos de productos desde la base de datos o caché
                var tiposProductos = DataTipoProducto.ListaTiposProductos(out mensaje, out exito);

                if (!exito)
                {
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new { success = true, data = tiposProductos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener tipos de productos: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult ObtenerIVA()
        {
            // Obtén el IVA desde la configuración o base de datos
            decimal iva = 0.16m; // Por ejemplo, un valor fijo o dinámico

            return Json(new
            {
                success = true,
                iva
            });
        }

        [HttpGet]
        public JsonResult ObtenerCompra(int id)
        {
            try
            {
                string mensaje;
                CompraLote? compra = DataCompraLote.ObtenerPorId(id, out mensaje); 
                if (compra == null)
                {
                    return Json(new { success = false, message = mensaje });
                }

                CatalogoProducto? producto = DataCatalogoProducto.ObtenerPorId(compra.IdCatalogoProducto, out mensaje);
                if (producto == null)
                {
                    return Json(new { success = false, message = mensaje });
                }

                Proveedor? proveedor = DataProveedores.ObtenerPorId(producto.IdProveedor, out mensaje);
                if (proveedor == null)
                {
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new
                {
                    success = true,
                    compra = new
                    {
                        idCompra = compra.IdCompraLote,
                        producto = producto.NombreProducto, // Debe mapearse con el nombre del producto
                        proveedor = proveedor.Nombre, // Debe mapearse con el nombre del proveedor
                        cantidadUnidades = compra.CantidadUnidades,
                        costoTotal = compra.CostoTotal,
                        fechaVencimiento = compra.FechaVencimiento?.ToString("yyyy-MM-dd"),
                        porcentajeMargenGanancia = compra.PorcentajeMargenGanancia,
                        fechaCompra = compra.FechaVencimiento?.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener la compra: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult ModificarCompra(
            int id,
            int producto,
            int cantidadUnidades,
            decimal costoTotal,
            string? fechaVencimiento,
            decimal porcentajeMargenGanancia)
        {
            string mensaje;
            try
            {
                // Validar la entrada
                if (id <= 0)
                {
                    return Json(new { success = false, message = "ID de compra no válido.", errores = new List<string> { "id" } });
                }
                if (producto <= 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar un producto válido.", errores = new List<string> { "idCatalogoProducto" } });
                }
                if (cantidadUnidades <= 0)
                {
                    return Json(new { success = false, message = "La cantidad debe ser mayor a 0.", errores = new List<string> { "cantidadUnidades" } });
                }
                if (costoTotal <= 0)
                {
                    return Json(new { success = false, message = "El costo total debe ser mayor a 0.", errores = new List<string> { "costoTotal" } });
                }
                if (porcentajeMargenGanancia < 0)
                {
                    return Json(new { success = false, message = "El margen de ganancia no puede ser negativo.", errores = new List<string> { "porcentajeMargenGanancia" } });
                }

                DateTime? fechaParsed = null;
                if (!string.IsNullOrWhiteSpace(fechaVencimiento))
                {
                    if (DateTime.TryParse(fechaVencimiento, out DateTime fecha))
                    {
                        fechaParsed = fecha;
                    }
                    else
                    {
                        return Json(new { success = false, message = "La fecha de vencimiento no es válida.", errores = new List<string> { "fechaVencimiento" } });
                    }
                }

                CompraLote? antiguo = DataCompraLote.ObtenerPorId(id, out mensaje);
                if(antiguo == null)
                    return Json(new { success = false, message = mensaje, errores = new List<string>() });

                Gasto nuevoGasto = new Gasto
                {
                    IdGasto = antiguo.IdGasto,
                    Monto = costoTotal,
                    Descripcion = $"Compra de {cantidadUnidades} {ObtenerNombreProducto(antiguo.IdCatalogoProducto)}",
                    IdTipoGasto = 1,
                    FechaRealizado = DateTime.Now
                };

                if (!DataGasto.Modificar(nuevoGasto, out mensaje))
                    return Json(new { success = false, message = mensaje, errores = new List<string>() });

                // Crear un objeto con los datos actualizados
                CompraLote compraModificada = new CompraLote
                {
                    IdCompraLote = id,
                    IdCatalogoProducto = producto,
                    CantidadUnidades = cantidadUnidades,
                    CostoTotal = costoTotal,
                    FechaVencimiento = fechaParsed,
                    PorcentajeMargenGanancia = porcentajeMargenGanancia,
                    IdParametros = 1,
                    IdGasto = antiguo.IdGasto
                };

                // Actualizar la compra en la base de datos
                bool resultado = DataCompraLote.Modificar(compraModificada, out mensaje);

                if (resultado)
                {
                    return Json(new { success = true, message = "Compra modificada exitosamente." });
                }
                else
                {
                    return Json(new { success = false, message = mensaje, errores = new List<string>() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al modificar la compra: {ex.Message}", errores = new List<string>() });
            }
        }


        [HttpGet]
        public JsonResult ObtenerCompraParaModificar(int id)
        {
            try
            {
                string mensaje;
                CompraLote? compra = DataCompraLote.ObtenerPorId(id, out mensaje);
                if (compra == null)
                {
                    return Json(new { success = false, message = mensaje });
                }

                CatalogoProducto? producto = DataCatalogoProducto.ObtenerPorId(compra.IdCatalogoProducto, out mensaje);
                if (producto == null)
                {
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new
                {
                    success = true,
                    compra = new
                    {
                        proveedor = producto.IdProveedor, 
                        tipo = producto.IdTipoProducto, // Debe mapearse con el nombre del producto
                        producto = producto.IdCatalogoProducto,
                        cantidadUnidades = compra.CantidadUnidades,
                        costoTotal = compra.CostoTotal,
                        fechaVencimiento = compra.FechaVencimiento?.ToString("yyyy-MM-dd"),
                        porcentajeMargenGanancia = compra.PorcentajeMargenGanancia,
                        fechaCompra = compra.FechaVencimiento?.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener la compra: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult EliminarCompra(int id)
        {
            string mensaje;
            bool resultado;

            try
            {
                // Eliminar la compra utilizando el método de DataCompraLote
                resultado = DataCompraLote.Eliminar(id, out mensaje);

                if (resultado)
                {
                    return Json(new { success = true, message = "Compra eliminada exitosamente." });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar la compra: {ex.Message}" });
            }
        }

    }
}