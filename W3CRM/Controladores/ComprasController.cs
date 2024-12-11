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

        [HttpGet]
        public JsonResult ObtenerProductosPorTipo(int tipoId)
        {
            var productosFiltrados = ListaProductos
                .Where(p => p.ID_TipoProducto == tipoId)
                .Select(p => new
                {
                    p.ID_CatalogoProducto,
                    p.Nombre_Producto
                })
                .ToList();

            return Json(new { success = true, data = productosFiltrados });
        }
        [HttpPost]
        public JsonResult AgregarNuevoLoteProducto( int tipo, int producto, int cantidad, decimal costo, string vencimiento, bool esMaterial, decimal margen, decimal? precio = null)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el lote de producto.";
            List<string> fallidos = new List<string>();
            DateTime fechaVencimiento = DateTime.Now;
            try
            {
                // Validaciones básicas
                if (tipo <= 0)
                {
                    mensaje = "El tipo de producto es obligatorio.";
                    fallidos.Add("tipo");
                }

                if (producto <= 0)
                {
                    mensaje = "El producto es obligatorio.";
                    fallidos.Add("producto");
                }

                if (cantidad <= 0)
                {
                    mensaje = "La cantidad debe ser mayor a cero.";
                    fallidos.Add("cantidad");
                }

                if (costo <= 0)
                {
                    mensaje = "El costo debe ser mayor a cero.";
                    fallidos.Add("costo");
                }

                if (string.IsNullOrWhiteSpace(vencimiento) || !DateTime.TryParse(vencimiento, out fechaVencimiento))
                {
                    mensaje = "La fecha de vencimiento es inválida.";
                    fallidos.Add("vencimiento");
                }

                if (margen < 0)
                {
                    mensaje = "El margen de ganancia no puede ser negativo.";
                    fallidos.Add("margen");
                }

                // Si hay fallos, retornamos el mensaje
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
                    mensaje = "El precio de venta calculado es inválido.";
                    fallidos.Add("precio");
                    return Json(new { success = false, message = mensaje, errores = fallidos });
                }

                // Crear el nuevo lote
                LoteProducto nuevoLote = new LoteProducto
                {
                    ID_CatalogoProducto = producto,
                    Fecha_Ingreso = DateTime.Now,
                    Fecha_Vencimiento = fechaVencimiento,
                    Cantidad = cantidad,
                    Precio_Compra = costo,
                    Precio_Venta = precio.Value,
                    Margen_Ganancia = margen,
                    IVA = 0.16m * costo, // Ejemplo de cálculo de IVA
                    EsMaterial = esMaterial
                };

                // Simular la lógica de guardado en la base de datos
                // Aquí podrías llamar a tu capa de datos o DbContext para guardar
                bool guardado = DataLoteProducto.Agregar(nuevoLote, out mensaje);

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