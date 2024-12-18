using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        public List<Proveedor> ListaProveedores = DataProveedores.ListaProveedores(out _, out _);
        public List<TipoProducto> ListasTipoProductos = DataTipoProducto.ListaTiposProductos(out _, out _);
        public List<TipoUnidad> ListaTipoUnidades = DataTipoUnidad.ListaTiposUnidades(out _, out _);
        public List<CatalogoProducto> ListaProductos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);

        public IActionResult ConsultaProductos()
        {
            return View(this);
        }

        [HttpGet]
        public IActionResult ObtenerProducto(int id)
        {
            var producto = ListaProductos.FirstOrDefault(c => c.IdCatalogoProducto == id);

            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            var productoInfo = new
            {
                success = true,
                producto = new
                {
                    producto.IdCatalogoProducto,
                    producto.NombreProducto,
                    TipoProducto = ObtenerTipoProducto(producto.IdTipoProducto),
                    Proveedor = ObtenerProveedor(producto.IdProveedor),
                    producto.PaginaWebProducto,
                    producto.Descripcion,
                    TipoUnidad = ObtenerTipoUnidad(producto.IdTipoUnidad),
                    producto.CantidadPorUnidad,
                    producto.EsMaterial // Nuevo campo agregado
                }
            };

            return Json(productoInfo);
        }


        [HttpPost]
        public JsonResult AgregarNuevoProducto(string nombre, int tipoProducto, int proveedor, string? paginaProducto, string? descripcion, string cantidadPorUnidad, int tipoUnidad, bool esMaterial)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el producto.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 255)
                {
                    mensaje = "El nombre debe tener entre 3 y 255 caracteres.";
                    fallidos.Add("nombre");
                }

                decimal cantidadNumerica = 0;
                // Validación de cantidad por unidad
                if (!string.IsNullOrWhiteSpace(cantidadPorUnidad))
                {
                    if (!decimal.TryParse(cantidadPorUnidad, out cantidadNumerica))
                    {
                        mensaje = "La cantidad debe ser un número válido.";
                        fallidos.Add("cantidad");
                    }
                }

                if (fallidos.Count == 0)
                {
                    CatalogoProducto nuevo = new CatalogoProducto
                    {
                        NombreProducto = nombre,
                        IdTipoProducto = tipoProducto,
                        IdProveedor = proveedor,
                        PaginaWebProducto = paginaProducto,
                        Descripcion = descripcion,
                        CantidadPorUnidad = cantidadNumerica,
                        IdTipoUnidad = tipoUnidad,
                        EsMaterial = esMaterial // Nuevo campo
                    };

                    resultado = DataCatalogoProducto.Agregar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el producto: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

        [HttpPost]
        public JsonResult ModificarProducto(int idSeleccionado, string nombre, int tipoProducto, int proveedor, string? paginaProducto, string? descripcion, string cantidadPorUnidad, int tipoUnidad, bool esMaterial)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al modificar el producto.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 255)
                {
                    mensaje = "El nombre debe tener entre 3 y 255 caracteres.";
                    fallidos.Add("nombre");
                }

                decimal cantidadNumerica = 0;
                // Validación de cantidad por unidad
                if (!string.IsNullOrWhiteSpace(cantidadPorUnidad))
                {
                    if (!decimal.TryParse(cantidadPorUnidad, out cantidadNumerica))
                    {
                        mensaje = "La cantidad debe ser un número válido.";
                        fallidos.Add("cantidad");
                    }
                }

                if (fallidos.Count == 0)
                {
                    CatalogoProducto modificado = new CatalogoProducto
                    {
                        IdCatalogoProducto = idSeleccionado,
                        NombreProducto = nombre,
                        IdTipoProducto = tipoProducto,
                        IdProveedor = proveedor,
                        PaginaWebProducto = paginaProducto,
                        Descripcion = descripcion,
                        CantidadPorUnidad = cantidadNumerica,
                        IdTipoUnidad = tipoUnidad,
                        EsMaterial = esMaterial // Nuevo campo
                    };

                    resultado = DataCatalogoProducto.Modificar(modificado, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al modificar el producto: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

        [HttpGet]
        public JsonResult ObtenerProductosActualizados()
        {
            var productosActualizados = ListaProductos.Select(producto => new
            {
                producto.IdCatalogoProducto,
                producto.NombreProducto,
                TipoProducto = ObtenerTipoProducto(producto.IdTipoProducto),
                Proveedor = ObtenerProveedor(producto.IdProveedor),
                producto.CantidadPorUnidad,
                TipoUnidad = ObtenerTipoUnidad(producto.IdTipoUnidad),
                producto.PaginaWebProducto,
                producto.Descripcion,
                producto.EsMaterial // Nuevo campo
            }).ToList();

            return Json(new { success = true, data = productosActualizados });
        }

        [HttpGet]
        public IActionResult SeleccionarProductoModificar(int id)
        {
            var producto = ListaProductos.FirstOrDefault(p => p.IdCatalogoProducto == id);

            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            return Json(new
            {
                success = true,
                producto = new
                {
                    producto.IdCatalogoProducto,
                    producto.NombreProducto,
                    producto.IdTipoProducto,
                    producto.IdProveedor,
                    producto.PaginaWebProducto,
                    producto.Descripcion,
                    producto.CantidadPorUnidad,
                    producto.IdTipoUnidad,
                    producto.EsMaterial // Nuevo campo
                }
            });
        }

        [HttpDelete]
        public IActionResult EliminarProducto(int id)
        {
            bool success = false;
            string mensaje = "Error al eliminar el producto";

            foreach (CompraLote lote in DataCompraLote.ListaCompraLotes(out _, out _))
            {
                if (lote.IdCatalogoProducto == id)
                {
                    return Json(new { success = false, message = "Error: el producto ya es utilizado en un lote de producto" });
                }
            }

            success = DataCatalogoProducto.Eliminar(id, out mensaje);
            return Json(new { success, message = mensaje });
        }

        // Métodos auxiliares
        public string ObtenerTipoProducto(int idTipoProducto) => ListasTipoProductos.FirstOrDefault(t => t.IdTipoProducto == idTipoProducto)?.NombreTipoProducto ?? "ERROR NO ENCONTRADO";
        public string ObtenerProveedor(int idProveedor) => ListaProveedores.FirstOrDefault(p => p.IdProveedor == idProveedor)?.Nombre ?? "ERROR NO ENCONTRADO";
        public string ObtenerTipoUnidad(int idTipoUnidad) => ListaTipoUnidades.FirstOrDefault(u => u.IdTipoUnidad == idTipoUnidad)?.NombreTipoUnidad ?? "ERROR NO ENCONTRADO";

        [HttpGet]
        public JsonResult ObtenerProveedores() => Json(DataProveedores.ListaProveedores(out _, out _));
        [HttpGet]
        public JsonResult ObtenerTiposProductos() => Json(DataTipoProducto.ListaTiposProductos(out _, out _));
        [HttpGet]
        public JsonResult ObtenerTiposUnidades() => Json(DataTipoUnidad.ListaTiposUnidades(out _, out _));
    }
}
