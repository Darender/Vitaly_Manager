using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        //public List<TipoProducto> ListaTipos = DataTipoProducto.ListaTiposProductos(out _, out _);

        public IActionResult AgregarLoteProducto()
        {
            ProductosController controlador = new ProductosController();
            return View(controlador);
        }

        public IActionResult AgregarProveedores()
        {
            ProductosController controlador = new ProductosController();
            return View(controlador);
        }

        [HttpPost]
        public JsonResult AgregarNuevoProveedores(string nombre, string? telefono, string? contactoAlternativo)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar al Proveedor.";
            List<string> fallidos = new List<string>();

            try
            {
                var regex = new System.Text.RegularExpressions.Regex("^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$");

                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                nombre = nombre.Trim();
                if (!regex.IsMatch(nombre))
                {
                    mensaje = "El nombre solo puede contener letras y espacios.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 50)
                {
                    mensaje = "El nombre debe tener entre 3 y 50 caracteres.";
                    fallidos.Add("nombre");
                }

                // Validación del numero telefonico
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    mensaje = "El número de teléfono no puede estar vacío.";
                    fallidos.Add("telefono");
                }
                else if (telefono.Length < 12 || telefono.Length > 20)
                {
                    mensaje = "El telefono debe tener entre 10 y 20 caracteres.";
                    fallidos.Add("telefono");
                }
                
                if (fallidos.Count == 0)
                {
                     Proveedor nuevo = new Proveedor
                    {
                        Nombre_Proveedor = nombre,
                         Telefono = telefono,
                         Pagina_Contacto = contactoAlternativo
                     };

                    resultado = DataProveedores.Agregar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el Proveedor: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }
    }
}
