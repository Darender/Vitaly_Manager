using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProveedoresController : Controller
    {
        public List<Proveedor> listaProveedores = DataProveedores.ListaProveedores(out _, out _);

        public IActionResult ConsultaProveedores()
        {
            // Pasar los proveedores a la vista
            return View(listaProveedores);
        }

        [HttpPost]
        public ActionResult EliminarProveedor(int idProveedor)
        {
            string respuesta;
            bool exito;

            // Llamamos al método estático EliminarProveedor de la clase DataProveedores
            DataProveedores.EliminarProveedor(idProveedor, out respuesta, out exito);

            // Retornamos una respuesta JSON basada en el resultado de la eliminación
            return Json(new { success = exito, message = respuesta });
        }

        //Toma el id del proveedor, verifica si tiene algun producto relacionado. Si es asi devuelve true,
        //de lo contrario devuelve false.
        public JsonResult tieneProductos(int id)
        {
            bool resultado;
            string mensaje;
            try
            {
                /*
                if (DataProveedores.tieneProductos(id))
                {
                    resultado = true;
                    mensaje = $"El proveedor no puede ser eliminado ya que cuenta con productos relacionadas.";
                }
                else
                {
                    resultado = false;
                    mensaje = "";
                }*/
                resultado = false;
                mensaje = "";
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al eliminar el proveedor: {ex.Message}";
            }
            return Json(new { success = resultado, message = mensaje });

        }



        public IActionResult AgregarProveedores()
        {
            ProveedoresController controlador = new ProveedoresController();
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
                        Nombre = nombre,
                         Telefono = telefono,
                         ContactoAlternativo = contactoAlternativo
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

        [HttpGet]
        public IActionResult SeleccionarProveedorModificar(int id)
        {
            Proveedor proveedor = listaProveedores[0];
            foreach (Proveedor valor in listaProveedores)
            {
                if (valor.IdProveedor == id)
                {
                    proveedor = valor;
                    break;
                }
            }

            return Json(proveedor);
        }


        [HttpPost]
        public JsonResult ModificarProveedor(string nombre, string telefono, string? contactoAlternativo, int proveedorSeleccionado)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al modificar el proveedor.";
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

                foreach (Proveedor item in listaProveedores)
                {
                    if (item.Telefono == telefono && item.IdProveedor != proveedorSeleccionado)
                    {
                        fallidos.Add("telefono");
                        mensaje = "Numero de telefono ya existente en la base de datos";
                    }
                }

                if (fallidos.Count == 0)
                {
                    Proveedor modificado = new Proveedor
                    {
                        IdProveedor = proveedorSeleccionado,
                        Nombre = nombre,
                        Telefono = telefono,
                        ContactoAlternativo = contactoAlternativo

                    };

                    resultado = DataProveedores.ModificarProveedor(modificado, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al modificar el cliente: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

        [HttpGet]
        public JsonResult ObtenerProveedoresActualizados()
        {
            return Json(new { success = true, data = listaProveedores });
        }
    }
}
