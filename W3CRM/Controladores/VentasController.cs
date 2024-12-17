using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class VentasController : Controller
    {
        public IActionResult ConsultaVentas()
        {
            return View(this);
        }

        [HttpGet]
        public IActionResult ObtenerVenta(int folioVenta)
        {
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult AgregarVenta(Venta venta)
        {
            // Lógica para agregar venta
            return Json(new { success = true, message = "Venta agregada exitosamente." });
        }

        [HttpPost]
        public IActionResult ModificarVenta(Venta venta)
        {
            // Lógica para modificar venta
            return Json(new { success = true, message = "Venta modificada exitosamente." });
        }

        [HttpGet]
        public IActionResult ObtenerClientes()
        {
            try
            {
                List <Cliente> clientes = DataClientes.ListaClientes(out _, out _);
                    clientes.Select(c => new {
                        c.IdCliente,
                        c.Nombre // O cualquier otra propiedad
                    }).ToList();

                return Json(new { success = true, data = clientes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener clientes.", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ObtenerProductos()
        {
            try
            {
                // Obtener la lista completa de productos
                List<CatalogoProducto> productos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);

                // Filtrar solo los productos que NO son materiales
                var productosNoMateriales = productos
                    .Where(p => !p.EsMaterial) // Asegúrate de que "EsMaterial" sea el nombre correcto en tu modelo
                    .Select(p => new
                    {
                        p.IdCatalogoProducto,
                        p.NombreProducto
                    })
                    .ToList();

                // Retornar la lista filtrada
                return Json(new { success = true, data = productosNoMateriales });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener productos.", error = ex.Message });
            }
        }


        [HttpPost]
        [Route("Ventas/AgregarVenta")]
        public JsonResult AgregarVenta(int folioVenta, int idCliente, int idProducto, int cantidadVendida, decimal ingresoTotal, DateTime? fechaRealizado = null)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar la venta.";
            List<string> fallidos = new List<string>();

            try
            {
                // 1. Validaciones de entrada
                if (idCliente <= 0)
                {
                    mensaje = "Debe seleccionar un cliente válido.";
                    fallidos.Add("idCliente");
                }

                if (idProducto <= 0)
                {
                    mensaje = "Debe seleccionar un producto válido.";
                    fallidos.Add("idProducto");
                }

                if (cantidadVendida <= 0)
                {
                    mensaje = "La cantidad vendida debe ser mayor a 0.";
                    fallidos.Add("cantidadVendida");
                }

                if (ingresoTotal <= 0)
                {
                    mensaje = "El ingreso total debe ser mayor a 0.";
                    fallidos.Add("ingresoTotal");
                }

                // Si hay errores, detener la ejecución
                if (fallidos.Count > 0)
                    return Json(new { success = false, message = mensaje, errores = fallidos });

                // 2. Obtener todos los lotes disponibles
                List<CompraLote> listaLotes = DataCompraLote.ListaCompraLotes(out _, out _);

                // 3. Registrar la venta principal
                Venta nuevaVenta = new Venta
                {
                    IdCliente = idCliente,
                    IngresoTotal = ingresoTotal,
                    FechaRealizado = fechaRealizado ?? DateTime.Now // Usar fecha proporcionada o fecha actual
                };

                if (!DataVenta.AgregarVenta(nuevaVenta, out mensaje, out int idVentaGenerada))
                {
                    return Json(new { success = false, message = mensaje });
                }

                // 4. Validar si hay suficientes unidades en los lotes
                int cantidadRestante = cantidadVendida;
                List<VentaProducto> ventasProductos = new List<VentaProducto>();

                foreach (CompraLote lote in listaLotes.Where(l => l.IdCatalogoProducto == idProducto && l.CantidadUnidades > 0))
                {
                    if (cantidadRestante <= 0) break;

                    int cantidadARestar = Math.Min(lote.CantidadUnidades, cantidadRestante);

                    // Crear el registro para VentaProducto
                    ventasProductos.Add(new VentaProducto
                    {
                        FolioVenta = nuevaVenta.FolioVenta,
                        IdCompraLote = lote.IdCompraLote,
                        CantidadVendida = cantidadARestar
                    });

                    // Restar unidades al lote
                    lote.CantidadUnidades -= cantidadARestar;
                    DataCompraLote.ActualizarCantidadUnidades(lote.IdCompraLote, lote.CantidadUnidades, out _);

                    cantidadRestante -= cantidadARestar;
                }

                if (cantidadRestante > 0)
                {
                    // Si no hay suficientes unidades, revertir la venta
                    DataVenta.EliminarVenta(nuevaVenta.FolioVenta, out _);
                    mensaje = "No hay suficientes unidades disponibles en el inventario.";
                    return Json(new { success = false, message = mensaje });
                }

                // 5. Registrar los productos vendidos asociados a la venta
                foreach (var ventaProducto in ventasProductos)
                {
                    ventaProducto.FolioVenta = idVentaGenerada;
                    DataVentaProducto.Agregar(ventaProducto, out _);
                }

                resultado = true;
                mensaje = "Venta agregada exitosamente.";
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar la venta: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }


        [HttpGet]
        public JsonResult ObtenerDatosProducto(int idProducto)
        {
            try
            {
                // Obtener los lotes relacionados con el producto
                var lotes = DataCompraLote.ListaCompraLotes(out _, out _)
                    .Where(l => l.IdCatalogoProducto == idProducto && l.CantidadUnidadesDisponibles > 0)
                    .ToList();

                if (!lotes.Any())
                {
                    return Json(new
                    {
                        success = true,
                        cantidadStock = 0,
                        precioUnitario = 0
                    });
                }

                // Calcular cantidad en stock
                int? cantidadStock = lotes.Sum(l => l.CantidadUnidadesDisponibles);

                // Calcular el precio unitario promedio ponderado
                decimal? precioUnitarioTotal = 0;
                int? cantidadTotal = 0;

                foreach (var lote in lotes)
                {
                    precioUnitarioTotal += lote.CostoTotal / lote.CantidadUnidades * lote.CantidadUnidadesDisponibles;
                    cantidadTotal += lote.CantidadUnidadesDisponibles;
                }

                decimal? precioUnitario = cantidadTotal > 0 ? precioUnitarioTotal / cantidadTotal : 0;
                decimal valorNormal = precioUnitario ?? 0m;
                return Json(new
                {
                    success = true,
                    cantidadStock,
                    precioUnitario = Math.Round(valorNormal, 2) // Redondear a 2 decimales
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
