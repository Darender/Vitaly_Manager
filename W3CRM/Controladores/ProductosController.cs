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
                mensaje = $"Error al agregar el cliente: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

    }
}
