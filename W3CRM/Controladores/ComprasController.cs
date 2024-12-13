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


    }
}