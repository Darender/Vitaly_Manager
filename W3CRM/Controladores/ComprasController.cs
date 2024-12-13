using Microsoft.AspNetCore.Mvc;
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
                    esMaterial = compra.EsMaterial
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
        [HttpPost]
        public JsonResult AgregarNuevoLoteProducto(int tipo, int producto, int cantidad, decimal costo, string vencimiento, bool esMaterial, decimal margen, decimal? precio = null)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el lote de producto.";
            List<string> fallidos = new List<string>();
            DateTime temp = DateTime.MinValue;

            try
            {
                // Validaciones básicas
                if (tipo <= 0)
                {
                    fallidos.Add("tipo");
                    mensaje = "El tipo de producto es obligatorio.";
                }

                if (producto <= 0)
                {
                    fallidos.Add("producto");
                    mensaje = "El producto es obligatorio.";
                }

                if (cantidad <= 0)
                {
                    fallidos.Add("cantidad");
                    mensaje = "La cantidad debe ser mayor a cero.";
                }

                if (costo <= 0)
                {
                    fallidos.Add("costo");
                    mensaje = "El costo debe ser mayor a cero.";
                }

                if (string.IsNullOrWhiteSpace(vencimiento) || !DateTime.TryParse(vencimiento, out temp))
                {
                    fallidos.Add("vencimiento");
                    mensaje = "La fecha de vencimiento es inválida.";
                }

                if (margen < 0)
                {
                    fallidos.Add("margen");
                    mensaje = "El margen de ganancia no puede ser negativo.";
                }

                // Si hay errores, retornamos inmediatamente
                if (fallidos.Count > 0)
                {
                    return Json(new { success = false, message = mensaje, errores = fallidos });
                }

                // Calcular el precio de venta si no está proporcionado
                if (!precio.HasValue)
                {
                    precio = costo + (costo * margen / 100);
                }

                // Validar el precio de venta
                if (precio <= 0)
                {
                    fallidos.Add("precio");
                    mensaje = "El precio de venta calculado es inválido.";
                    return Json(new { success = false, message = mensaje, errores = fallidos });
                }

                DateTime? fechaVencimiento = null;

                if (temp != DateTime.MinValue)
                    fechaVencimiento = temp;

                // Crear el nuevo lote
                CompraLote nuevoLote = new CompraLote
                {
                    IdCatalogoProducto = producto,
                    CantidadUnidades = cantidad,
                    CostoTotal = costo * cantidad,
                    FechaVencimiento = fechaVencimiento,
                    EsMaterial = esMaterial,
                    PorcentajeMargenGanancia = margen,
                    PrecioVentaSugerido = precio.Value
                };

                // Guardar en la base de datos
                bool guardado = DataCompraLote.Agregar(nuevoLote, out mensaje);

                if (guardado)
                {
                    resultado = true;
                    mensaje = "Lote de producto agregado correctamente.";
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el lote: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
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


    }
}