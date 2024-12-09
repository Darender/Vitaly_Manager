using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Entidades;
using Vitaly_Manager.Data;

namespace Vitaly_Manager.Controladores
{
    public class UsuariosController : Controller
    {
        /// <summary>
        /// Lista de usuarios, que se manda a llamar desde su data
        /// </summary>
        public List<Usuario> ListaUsuarios = DataUsuarios.ListaUsuarios(out _, out _);

        public IActionResult AgregarUsuarios()
        {
            UsuariosController controlador = new UsuariosController();
            return View(controlador);
        }

        /// <summary>
        /// Manda a llamar la consulta
        /// </summary>
        /// <returns></returns>
        public IActionResult ConsultaUsuarios()
        {
            return View(this);
        }

        /// <summary>
        /// Método para crear un nuevo usuario.
        /// </summary>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="contraseña">Contraseña del usuario.</param>
        /// <param name="esAdmin"></param>
        /// <param name="mensaje">Mensaje de respuesta.</param>
        /// <returns>Booleano indicando si la creación fue exitosa.</returns>
        public bool AgregarUsuario(string nombre,string correo, string contrasena, bool esAdmin, out string mensaje)
        {
            bool resultado;

            // Crear un nuevo usuario
            Usuario nuevo = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Contraseña = contrasena,
                esAdmin = esAdmin,
            };

            // Enviar el nuevo usuario a DataUsuarios para agregarlo a la base de datos
            resultado = DataUsuarios.Agregar(nuevo, out mensaje);

            return resultado;
        }

        /*
        [HttpPost]
        public JsonResult ModificarUsuario(int idUsuario, string nombre, string apellidoPaterno, string apellidoMaterno, string correo, string? contrasena, string rol)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al modificar el usuario.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validaciones para nombre, apellidos
                var regex = new System.Text.RegularExpressions.Regex("^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$");

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

                if (string.IsNullOrWhiteSpace(apellidoPaterno))
                {
                    mensaje = "El apellido paterno no puede estar vacío.";
                    fallidos.Add("apellidoPaterno");
                }
                apellidoPaterno = apellidoPaterno.Trim();
                if (!regex.IsMatch(apellidoPaterno))
                {
                    mensaje = "El apellido paterno solo puede contener letras y espacios.";
                    fallidos.Add("apellidoPaterno");
                }

                if (string.IsNullOrWhiteSpace(apellidoMaterno))
                {
                    mensaje = "El apellido materno no puede estar vacío.";
                    fallidos.Add("apellidoMaterno");
                }
                apellidoMaterno = apellidoMaterno.Trim();
                if (!regex.IsMatch(apellidoMaterno))
                {
                    mensaje = "El apellido materno solo puede contener letras y espacios.";
                    fallidos.Add("apellidoMaterno");
                }

                // Validaciones para correo electrónico
                if (string.IsNullOrWhiteSpace(correo))
                {
                    mensaje = "El correo electrónico no puede estar vacío.";
                    fallidos.Add("correo");
                }
                else
                {
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                    if (!emailRegex.IsMatch(correo))
                    {
                        mensaje = "El correo electrónico no es válido.";
                        fallidos.Add("correo");
                    }
                }

                // Validación para la contraseña (opcional)
                if (!string.IsNullOrWhiteSpace(contrasena) && (contrasena.Length < 6 || contrasena.Length > 20))
                {
                    mensaje = "La contraseña debe tener entre 6 y 20 caracteres.";
                    fallidos.Add("contraseña");
                }

                // Validación para el rol
                if (string.IsNullOrWhiteSpace(rol) || !(rol == "Administrador" || rol == "Cliente"))
                {
                    mensaje = "Debe seleccionar un rol válido.";
                    fallidos.Add("rol");
                }

                if (fallidos.Count == 0)
                {
                    // Creación del usuario modificado
                    Usuario modificado = new Usuario
                    {
                        ID_Usuario = idUsuario,
                        Nombre = nombre,
                        ApellidoPaterno = apellidoPaterno,
                        ApellidoMaterno = apellidoMaterno,
                        Correo = correo,
                        Contraseña = contrasena,
                        Rol = rol,
                        FechaRegistro = DateTime.Now // Actualizar fecha de registro
                    };

                    // Envío al método para modificar en la base de datos
                    resultado = DataUsuarios.Modificar(modificado, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al modificar el usuario: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        } */


        [HttpPost]
        public IActionResult EliminarUsuarios(int id)
        {
            string mensaje;
            bool exito = DataUsuarios.Eliminar(id, out mensaje);

            if (exito)
            {
                return Json(new { success = true, message = mensaje });
            }
            else
            {
                return Json(new { success = false, message = mensaje });
            }
        }

    }
}
