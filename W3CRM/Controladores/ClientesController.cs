using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ClientesController : Controller
    {
        public List<Cliente> listaClientes = DataClientes.ListaClientes(out _, out _);
        public IActionResult AgregarClientes()
        {
            return View(this);
        }

        public IActionResult ConsultaClientes()
        {
            ClientesController controlador = new ClientesController();
            return View(controlador);
        }

        [HttpPost]
        public JsonResult AgregarNuevoCliente(string nombre, string apellidos, string telefono, string? genero, string? contactoAlternativo, string? edad)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el cliente.";
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

                // Validación del apellidos
                if (string.IsNullOrWhiteSpace(apellidos))
                {
                    mensaje = "El apellido paterno no puede estar vacío.";
                    fallidos.Add("apellidos");
                }
                apellidos = apellidos.Trim();
                if (!regex.IsMatch(apellidos))
                {
                    mensaje = "El apellido paterno solo puede contener letras y espacios.";
                    fallidos.Add("apellidos");
                }
                else if (apellidos.Length < 3 || apellidos.Length > 30)
                {
                    mensaje = "El apellido paterno debe tener entre 3 y 30 caracteres.";
                    fallidos.Add("apellidos");
                }


                // Validación del numero telefonico
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    mensaje = "El número de teléfono no puede estar vacío.";
                    fallidos.Add("telefono");
                } else if (telefono.Length < 12 || telefono.Length > 20)
                {
                    mensaje = "El telefono debe tener entre 10 y 20 caracteres.";
                    fallidos.Add("telefono");
                }
                foreach (Cliente item in listaClientes)
                {
                    if (item.Telefono == telefono)
                    {
                        fallidos.Add("telefono");
                        mensaje = "Numero de telefono ya existente en la base de datos";
                    }
                }
                int? edadNumerica = null;
                // Validación de edad
                if (!string.IsNullOrWhiteSpace(edad))
                {
                    int temp;
                    if (!int.TryParse(edad, out temp))
                    {
                        mensaje = "La edad debe ser un número válido.";
                        fallidos.Add("edad");
                    }
                    else if (edadNumerica < 0 || edadNumerica > 120)
                    {
                        mensaje = "La edad debe estar entre 0 y 140 años.";
                        fallidos.Add("edad");
                    }
                    edadNumerica = temp;
                }


                if (fallidos.Count == 0)
                {
                    Cliente nuevo = new Cliente
                    {
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Telefono = telefono,
                        Genero = genero,
                        ContactoAlternativo = contactoAlternativo,
                        Edad = edadNumerica,
                    };

                    resultado = DataClientes.Agregar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el cliente: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

        [HttpGet]
        public IActionResult SeleccionarClienteModificar(int id)
        {
            Cliente cliente = listaClientes[0];
            foreach (Cliente valor in listaClientes)
            {
                if (valor.IdCliente == id)
                {
                    cliente = valor;
                    break;
                }
            }

            return Json(cliente);
        }

        [HttpPost]
        public JsonResult ModificarCliente(string nombre, string apellidos, string telefono, string? genero, string? contactoAlternativo, string? edad, int clienteSeleccionado)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al modificar el cliente.";
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

                // Validación del apellido paterno
                if (string.IsNullOrWhiteSpace(apellidos))
                {
                    mensaje = "El apellido no puede estar vacío.";
                    fallidos.Add("apellidos");
                }
                apellidos = apellidos.Trim();
                if (!regex.IsMatch(apellidos))
                {
                    mensaje = "El apellido solo puede contener letras y espacios.";
                    fallidos.Add("apellidos");
                }
                else if (apellidos.Length < 3 || apellidos.Length > 30)
                {
                    mensaje = "El apellido paterno debe tener entre 3 y 30 caracteres.";
                    fallidos.Add("apellidos");
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
                foreach (Cliente item in listaClientes)
                {
                    if (item.Telefono == telefono && item.IdCliente != clienteSeleccionado)
                    {
                        fallidos.Add("telefono");
                        mensaje = "Numero de telefono ya existente en la base de datos";
                    }
                }
                int? edadNumerica = null;
                // Validación de edad
                if (!string.IsNullOrWhiteSpace(edad))
                {
                    int temp;
                    if (!int.TryParse(edad, out temp))
                    {
                        mensaje = "La edad debe ser un número válido.";
                        fallidos.Add("edad");
                    }
                    else if (edadNumerica < 0 || edadNumerica > 120)
                    {
                        mensaje = "La edad debe estar entre 0 y 140 años.";
                        fallidos.Add("edad");
                    }
                    edadNumerica = temp;
                }


                if (fallidos.Count == 0)
                {
                    Cliente modificado = new Cliente
                    {
                        IdCliente = clienteSeleccionado,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Telefono = telefono,
                        Genero = genero,
                        ContactoAlternativo = contactoAlternativo,
                        Edad = edadNumerica,
                    };

                    resultado = DataClientes.Modificar(modificado, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al modificar el cliente: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }

        [HttpDelete]
        public JsonResult EliminarCliente(int id)
        {
            bool resultado = true;
            string mensaje = "Se ha eliminado el cliente.";
            try
            {
                
                resultado = DataClientes.EliminarCliente(id,out mensaje);
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al eliminar el cliente: {ex.Message}";
            }


            return Json(new { success = resultado, message = mensaje});
        }


        [HttpGet]
        public JsonResult ObtenerClientesActualizados()
        {
            return Json(new { success = true, data = listaClientes });
        }

       

    }
}
