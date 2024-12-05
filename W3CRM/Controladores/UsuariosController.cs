using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Entidades;
using Vitaly_Manager.Data;

namespace Vitaly_Manager.Controladores
{
    public class UsuariosController : Controller
    {

        public IActionResult AgregarUsuarios()
        {
            UsuariosController controlador = new UsuariosController();
            return View(controlador);
        }


        /// <summary>
        /// Método para crear un nuevo usuario.
        /// </summary>
        /// <param name="nombre">Nombre del usuario.</param>
        /// <param name="apellidoPaterno">Primer apellido.</param>
        /// <param name="apellidoMaterno">Segundo apellido.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="contraseña">Contraseña del usuario.</param>
        /// <param name="rol">Rol del usuario.</param>
        /// <param name="mensaje">Mensaje de respuesta.</param>
        /// <returns>Booleano indicando si la creación fue exitosa.</returns>
        public bool CrearUsuario(string nombre, string apellidoPaterno, string apellidoMaterno, string correo, string contraseña, string rol, out string mensaje)
        {
            bool resultado;

            // Crear un nuevo usuario
            Usuario nuevo = new Usuario
            {
                Nombre_Usuario = nombre,
                Apellido_Paterno = apellidoPaterno,
                Apellido_Materno = apellidoMaterno,
                Email = correo,
                Contraseña = contraseña,
                Rol = rol
            };

            // Enviar el nuevo usuario a DataUsuarios para agregarlo a la base de datos
            resultado = DataUsuarios.Agregar(nuevo, out mensaje);

            return resultado;
        }
    }
}
