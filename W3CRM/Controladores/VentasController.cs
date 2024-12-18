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
                    lote.CantidadUnidadesDisponibles -= cantidadARestar;
                    int disponibleAtual = lote.CantidadUnidadesDisponibles ?? 0;
                    DataCompraLote.ActualizarCantidadUnidades(lote.IdCompraLote, disponibleAtual, out _);

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

        [HttpGet]
public JsonResult ObtenerVentas()
{
    bool resultado = false;
    string mensaje = "Error al obtener las ventas.";
    List<VentaDetalle?> listaVentas = new List<VentaDetalle?>();

    try
    {
        // Obtener listas necesarias
        List<Venta> ventas = DataVenta.ListaVentas(out _, out _);
        List<CatalogoProducto> productos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);
        List<Cliente> clientes = DataClientes.ListaClientes(out _, out _);
        List<VentaProducto> vp = DataVentaProducto.ListaVentasProductos(out _, out _);

        if (ventas != null && ventas.Any())
        {
                    listaVentas = ventas.Select(v =>
                    {
                        var ventaProducto = vp.FirstOrDefault(c => c.FolioVenta == v.FolioVenta);
                        if (ventaProducto == null) return null;

                        var compraLote = DataCompraLote.ObtenerPorId(ventaProducto.IdCompraLote, out _);
                        var producto = productos.FirstOrDefault(p => p.IdCatalogoProducto == compraLote?.IdCatalogoProducto);

                        return new VentaDetalle
                        {
                            FolioVenta = v.FolioVenta,
                            ClienteNombre = clientes.FirstOrDefault(c => c.IdCliente == v.IdCliente)?.Nombre ?? "Cliente no encontrado",
                            NombreProducto = producto?.NombreProducto ?? "Producto no encontrado",
                            CantidadVendida = vp.Where(c => c.FolioVenta == v.FolioVenta).Sum(c => c.CantidadVendida),
                            IngresoTotal = v.IngresoTotal,
                            FechaRealizada = v.FechaRealizado.ToString("yyyy-MM-dd")
                        };
                    })
        .Where(v => v != null)
        .ToList();


                    resultado = true;
            mensaje = "Ventas obtenidas correctamente.";
        }
        else
        {
            mensaje = "No hay ventas registradas.";
        }
    }
    catch (Exception ex)
    {
        mensaje = $"Error: {ex.Message}";
    }

    return Json(new { success = resultado, message = mensaje, data = listaVentas });
}

        [HttpGet]
        public JsonResult ObtenerVentaUnica(int folioVenta)
        {
            bool resultado = false;
            string mensaje = "No se encontró la información de la venta.";
            object ventaDetalle = null;

            try
            {
                // Obtener las listas necesarias
                List<Venta> ventas = DataVenta.ListaVentas(out _, out _);
                List<CatalogoProducto> productos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);
                List<Cliente> clientes = DataClientes.ListaClientes(out _, out _);
                List<VentaProducto> vp = DataVentaProducto.ListaVentasProductos(out _, out _);

                // Buscar la venta específica
                var venta = ventas.FirstOrDefault(v => v.FolioVenta == folioVenta);
                if (venta != null)
                {
                    var ventaProducto = vp.FirstOrDefault(c => c.FolioVenta == folioVenta);
                    if (ventaProducto != null)
                    {
                        var compraLote = DataCompraLote.ObtenerPorId(ventaProducto.IdCompraLote, out _);
                        var producto = productos.FirstOrDefault(p => p.IdCatalogoProducto == compraLote?.IdCatalogoProducto);

                        // Construir el objeto con los detalles de la venta
                        ventaDetalle = new
                        {
                            clienteNombre = clientes.FirstOrDefault(c => c.IdCliente == venta.IdCliente)?.Nombre ?? "Cliente no encontrado",
                            nombreProducto = producto?.NombreProducto ?? "Producto no encontrado",
                            cantidadVendida = vp.Where(c => c.FolioVenta == folioVenta).Sum(c => c.CantidadVendida),
                            ingresoTotal = venta.IngresoTotal,
                            fechaRealizada = venta.FechaRealizado.ToString("yyyy-MM-dd")
                        };

                        resultado = true;
                        mensaje = "Venta obtenida correctamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
            }

            // Devolver respuesta JSON
            return Json(new { success = resultado, message = mensaje, data = ventaDetalle });
        }

        [HttpPost]
        public JsonResult EliminarVenta(int folioVenta)
        {
            bool resultado = false;
            string mensaje = "No se pudo eliminar la venta.";

            try
            {
                // Obtener listas existentes
                List<Venta> ventas = DataVenta.ListaVentas(out _, out _);
                List<VentaProducto> ventaProductos = DataVentaProducto.ListaVentasProductos(out _, out _);
                List<CompraLote> compraLotes = DataCompraLote.ListaCompraLotes(out _, out _);

                // Buscar la venta original
                var venta = ventas.FirstOrDefault(v => v.FolioVenta == folioVenta);

                if (venta != null)
                {
                    // Filtrar todos los VentaProducto relacionados con el folioVenta
                    var productosRelacionados = ventaProductos.Where(vp => vp.FolioVenta == folioVenta).ToList();

                    foreach (var vp in productosRelacionados)
                    {
                        // Buscar el CompraLote relacionado y actualizar su cantidad disponible
                        var compraLote = compraLotes.FirstOrDefault(cl => cl.IdCompraLote == vp.IdCompraLote);

                        if (compraLote != null)
                        {
                            // Devolver la cantidad vendida al CompraLote
                            compraLote.CantidadUnidadesDisponibles += vp.CantidadVendida;

                            // Actualizar el CompraLote individualmente
                            DataCompraLote.Modificar(compraLote, out _);
                        }

                        // Eliminar el registro de VentaProducto individualmente
                        DataVentaProducto.Eliminar(vp.IdCompraLote, vp.FolioVenta, out _);
                    }

                    // Eliminar la venta original de la lista de ventas
                    DataVenta.EliminarVenta(venta.FolioVenta, out _);

                    resultado = true;
                    mensaje = "Venta eliminada y cantidades actualizadas correctamente.";
                }
                else
                {
                    mensaje = "La venta no fue encontrada.";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
            }

            // Devolver respuesta JSON
            return Json(new { success = resultado, message = mensaje });
        }



    }

}

public class VentaDetalle
{
    public int FolioVenta { get; set; }
    public string ClienteNombre { get; set; }
    public string NombreProducto { get; set; }
    public int CantidadVendida { get; set; }
    public decimal IngresoTotal { get; set; }
    public string FechaRealizada { get; set; }
}