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

        /// <summary>
        /// Un metodo encargado de la logica para agregar nuevos proveedores al sistema
        /// </summary>
        /// <param name="nombre">Nombre del nuevo proveedor</param>
        /// <param name="telefono">Numero telefonico del nuevo proveedor</param>
        /// <param name="contactoAlternativo">Cualquier contacto alternativo que tenga el proveedor</param>
        /// <returns>Devuelve un booleado de si fue exitoso, un mensaje de que fue lo que paso y una lista de casillas que fueron fallidas</returns>
        [HttpPost]
        public JsonResult AgregarNuevoProveedores(string nombre, string? telefono, string? contactoAlternativo)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar al Proveedor.";

            // lista de todas las casillas que terminaron como fallido
            List<string> fallidos = new List<string>();

            try
            {
                // El telefono y el contacto alternativo no pueden ser nulos al mismo tiempo
                if (telefono == null && contactoAlternativo == null) {
                    mensaje = "Telefono y contacto alternativo no pueden estar vacios al mismo tiempo";
                    fallidos.Add("telefono");
                    fallidos.Add("alternativo");
                    return Json(new { success = resultado, message = mensaje, errores = fallidos });
                }

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

                    // Envio del nuevo profesor a data para que se envie a la base de datos
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
