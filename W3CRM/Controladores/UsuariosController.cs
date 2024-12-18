using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class UsuariosController : Controller
    {
        public IActionResult ConsultaUsuarios()
        {
            // Pasar los proveedores a la vista
            return View(this);
        }

        [HttpPost]
        public JsonResult AgregarUsuario(IFormFile imagenUsuario, string nombre, string apellidos, string correoElectronico,
                                         string telefono, string password, bool esAdministrador)
        {
            string mensaje = "Error al agregar el usuario.";
            bool resultado = false;
            try
            {
                // 1. Validaciones básicas
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correoElectronico))
                {
                    return Json(new { success = false, message = "El nombre y el correo electrónico son obligatorios." });
                }

                // 2. Manejo de la imagen del usuario (si existe)
                string rutaImagen = null;
                if (imagenUsuario != null && imagenUsuario.Length > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagenUsuario.FileName);
                    string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagenes/usuarios");

                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        imagenUsuario.CopyTo(stream);
                    }

                    rutaImagen = "/imagenes/usuarios/" + nombreArchivo; // Ruta para la base de datos
                }

                // 3. Crear el objeto de usuario
                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos ?? "No especificado",
                    CorreoElectronico = correoElectronico,
                    Telefono = telefono ?? "No especificado",
                    Password = password,
                    EsAdministrador = esAdministrador,
                    EstadoActivo = false
                };

                // 4. Insertar el usuario en la base de datos
                if (DataUsuarios.Agregar(nuevoUsuario, out mensaje))
                {
                    resultado = true;
                    mensaje = "Usuario agregado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al agregar el usuario: {ex.Message}";
            }

            // 5. Devolver respuesta
            return Json(new { success = resultado, message = mensaje });
        }

        [HttpGet]
        public JsonResult ObtenerUsuarios()
        {
            bool resultado = false;
            string mensaje = "";
            List<UsuarioDTO> listaUsuarios = new List<UsuarioDTO>();

            try
            {
                // Obtener la lista de usuarios desde DataUsuarios
                List<Usuario> usuarios = DataUsuarios.ListaUsuarios(out mensaje, out resultado);

                if (resultado && usuarios != null && usuarios.Any())
                {
                    // Construir la lista con detalles de usuarios usando el DTO
                    listaUsuarios = usuarios.Select(u => new UsuarioDTO
                    {
                        IdUsuario = u.IdUsuario,
                        Nombre = u.Nombre,
                        Apellidos = u.Apellidos ?? "No especificado",
                        CorreoElectronico = u.CorreoElectronico,
                        Telefono = u.Telefono ?? "No especificado",
                        EstadoActivo = u.EstadoActivo ? "Sí" : "No",
                        EsAdministrador = u.EsAdministrador ? "Sí" : "No"
                    }).ToList();

                    mensaje = "Usuarios obtenidos correctamente.";
                }
                else
                {
                    mensaje = "No hay usuarios registrados.";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener usuarios: {ex.Message}";
            }


            // Devolver la respuesta en formato JSON
            return Json(new { success = resultado, message = mensaje, data = listaUsuarios });
        }

        [HttpPost]
        public JsonResult EliminarUsuario(int idUsuario)
        {
            string mensaje = "Error al eliminar el usuario.";
            bool resultado = false;

            try
            {
                // Eliminar el usuario
                if (DataUsuarios.Eliminar(idUsuario, out mensaje))
                {
                    resultado = true;
                    mensaje = "Usuario eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al eliminar el usuario: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje });
        }

        [HttpGet]
        public JsonResult ObtenerUsuario(int idUsuario)
        {
            string mensaje;
            bool resultado = false;

            try
            {
                var usuario = DataUsuarios.ObtenerPorId(idUsuario, out mensaje);
                if (usuario != null)
                {
                    resultado = true;
                    return Json(new
                    {
                        success = true,
                        usuario = new
                        {
                            idUsuario = usuario.IdUsuario,
                            nombre = usuario.Nombre,
                            apellidos = usuario.Apellidos,
                            correoElectronico = usuario.CorreoElectronico,
                            telefono = usuario.Telefono,
                            esAdministrador = usuario.EsAdministrador,
                        }
                    });
                }
                mensaje = "Usuario no encontrado.";
            }
            catch (Exception ex)
            {
                mensaje = $"Error al obtener el usuario: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje });
        }

        [HttpPost]
        public JsonResult ModificarUsuario(int idUsuario, string nombre, string apellidos, string correoElectronico, string telefono, bool esAdministrador, int? idImagenUsuario, string password)
        {
            string mensaje = "";
            bool resultado = false;

            try
            {
                // Validaciones básicas
                if (idUsuario <= 0)
                {
                    mensaje = "ID del usuario no es válido.";
                    return Json(new { success = resultado, message = mensaje });
                }
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre del usuario es obligatorio.";
                    return Json(new { success = resultado, message = mensaje });
                }
                if (string.IsNullOrWhiteSpace(correoElectronico))
                {
                    mensaje = "El correo electrónico del usuario es obligatorio.";
                    return Json(new { success = resultado, message = mensaje });
                }

                // Crear instancia del usuario
                Usuario usuarioModificado = new Usuario
                {
                    EstadoActivo = false,
                    IdUsuario = idUsuario,
                    Nombre = nombre,
                    Apellidos = apellidos ?? "No especificado",
                    CorreoElectronico = correoElectronico,
                    Telefono = telefono ?? "No especificado",
                    EsAdministrador = esAdministrador,
                    IdImagenUsuario = idImagenUsuario ?? 0,
                    Password = password
                };

                // Llamar a la capa de datos
                if (DataUsuarios.Modificar(usuarioModificado, out mensaje))
                {
                    resultado = true;
                    mensaje = "Usuario modificado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar el usuario: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje });
        }

    }
}

public class UsuarioDTO
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string CorreoElectronico { get; set; }
    public string Telefono { get; set; }
    public string EstadoActivo { get; set; }
    public string EsAdministrador { get; set; }
}