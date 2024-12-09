using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        public static List<Proveedor> ListaProveedores = DataProveedores.ListaProveedores(out _, out _);
        public static List<TipoProducto> ListasTipoProductos = DataTipoProducto.ListaTiposProductos(out _, out _);
        public static List<TipoUnidad> ListaTipoUnidades = DataTipoUnidad.ListaTiposUnidades(out _, out _);
        public List<CatalogoProducto> ListaProductos = DataCatalogoProducto.ListaCatalogoProductos(out _, out _);
        public IActionResult AgregarLoteProducto()
        {
            return View(this);
        }

        public IActionResult ConsultaProductos()
        {
            return View(this);
        }

        public IActionResult AgregarProductos()
        {
            return View(this);
        }

        [HttpPost]
        public JsonResult AgregarNuevoProducto(string nombre, int tipoProducto, int proveedor, string? paginaProducto, string cantidadUnitaria, int tipounidad)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el cliente.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 100)
                {
                    mensaje = "El nombre debe tener entre 3 y 100 caracteres.";
                    fallidos.Add("nombre");
                }

                int cantidadNumerica = 0;
                // Validación de edad
                if (!string.IsNullOrWhiteSpace(cantidadUnitaria))
                {
                    int temp;
                    if (!int.TryParse(cantidadUnitaria, out temp))
                    {
                        mensaje = "La cantidad debe ser un número válido.";
                        fallidos.Add("cantidad");
                    }
                    cantidadNumerica = temp;
                }


                if (fallidos.Count == 0)
                {
                    CatalogoProducto nuevo = new CatalogoProducto
                    {
                        Nombre_Producto = nombre,
                        Cantidad_Unidades = cantidadNumerica,
                        Pagina_Producto = paginaProducto,
                        ID_Proveedor = proveedor,
                        ID_TipoUnidad = tipounidad,
                        ID_TipoProducto = tipoProducto

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

        [HttpDelete]
        public IActionResult EliminarProducto(int id)
        {
            bool success = false;
            string mensaje = "Error al eliminar el producto";

            foreach(LoteProducto lote in DataLoteProducto.ListaLoteProducto(out _, out _))
            {
                if(lote.ID_CatalogoProducto == id)
                {
                    return Json(new { success = false, message = "Error: el producto ya es utilizado en un lote de producto" });
                }
            }

            success = DataCatalogoProducto.Eliminar(id, out mensaje);
            return Json(new { success, message = mensaje });
        }

        /// <summary>
        /// Metodo encargado de conbertir el id de un tipo de producto al nombre de dicho tipo de producto
        /// </summary>
        /// <param name="idTipoProducto">El id el tipo de producto a buscar</param>
        /// <returns>El nombre del tipo de producto segun el id dado</returns>
        public string ObtenerTipoProducto(int idTipoProducto) { 
            foreach (TipoProducto item in ListasTipoProductos)
            {
                if (item.ID_TipoProducto == idTipoProducto)
                    return item.Nombre_Tipo_Producto;
            }
            return "ERROR NO ENCONTRADO";
        }

        /// <summary>
        /// Metodo encargado de conbertir el id de un proveedor al nombre de dicho proveedor
        /// </summary>
        public string ObtenerProveedor(int idProveedor)
        {
            foreach (Proveedor item in ListaProveedores)
            {
                if (item.ID_Proveedor == idProveedor)
                    return item.Nombre_Proveedor;
            }
            return "ERROR NO ENCONTRADO";
        }

        /// <summary>
        /// Metodo encargado de conbertir el id de un tipo de unidad al nombre de dicho tipo de unidad
        /// </summary>
        public string ObtenerTipoUnidad(int idTipoUnidad)
        {
            foreach (TipoUnidad item in ListaTipoUnidades)
            {
                if (item.ID_TipoUnidad == idTipoUnidad)
                    return item.Unidad_Medida;
            }
            return "ERROR NO ENCONTRADO";
        }

        [HttpGet]
        public JsonResult ObtenerProductosActualizados()
        {
            var productosActualizados = ListaProductos.Select(producto => new
            {
                producto.ID_CatalogoProducto,
                producto.Nombre_Producto,
                TipoProducto = ObtenerTipoProducto(producto.ID_TipoProducto), // Convertir ID a nombre
                Proveedor = ObtenerProveedor(producto.ID_Proveedor),         // Convertir ID a nombre
                producto.Cantidad_Unidades,
                TipoUnidad = ObtenerTipoUnidad(producto.ID_TipoUnidad),      // Convertir ID a nombre
                producto.Pagina_Producto
            }).ToList();

            return Json(new { success = true, data = productosActualizados });
        }


        [HttpGet]
        public IActionResult SeleccionarProductoModificar(int id)
        {
            CatalogoProducto producto = ListaProductos[0];
            foreach (CatalogoProducto valor in ListaProductos)
            {
                if (valor.ID_CatalogoProducto == id)
                {
                    producto = valor;
                    break;
                }
            }

            return Json(producto);
        }

        [HttpPost]
        public JsonResult ModificarProducto(int idSeleccionado, string nombre, int tipoProducto, int proveedor, string? paginaProducto, string cantidadUnitaria, int tipounidad)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el cliente.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 100)
                {
                    mensaje = "El nombre debe tener entre 3 y 100 caracteres.";
                    fallidos.Add("nombre");
                }

                int cantidadNumerica = 0;
                // Validación de edad
                if (!string.IsNullOrWhiteSpace(cantidadUnitaria))
                {
                    int temp;
                    if (!int.TryParse(cantidadUnitaria, out temp))
                    {
                        mensaje = "La cantidad debe ser un número válido.";
                        fallidos.Add("cantidad");
                    }
                    cantidadNumerica = temp;
                }


                if (fallidos.Count == 0)
                {
                    CatalogoProducto nuevo = new CatalogoProducto
                    {
                        ID_CatalogoProducto = idSeleccionado,
                        Nombre_Producto = nombre,
                        Cantidad_Unidades = cantidadNumerica,
                        Pagina_Producto = paginaProducto,
                        ID_Proveedor = proveedor,
                        ID_TipoUnidad = tipounidad,
                        ID_TipoProducto = tipoProducto

                    };

                    resultado = DataCatalogoProducto.Modificar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al modificar el producto: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

    }
}
