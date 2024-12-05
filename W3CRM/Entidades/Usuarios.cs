using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Usuario
    {
        public int ID_Usuario { get; set; } // ID del usuario, opcional en la creación
        public string Nombre_Usuario { get; set; } // Nombre del usuario
        public string Apellido_Paterno { get; set; } // Primer apellido
        public string Apellido_Materno { get; set; } // Segundo apellido
        public string Email { get; set; } // Correo electrónico
        public string Contraseña { get; set; } // Contraseña
        public string Rol { get; set; } // Rol del usuario (e.g., Administrador, Usuario, etc.)
    }


    public class Usuarios
    {
        [Key]
        public required int ID_Usuario { get; set; }

        private string _correo;
        [Required(ErrorMessage = "El correor es obligatorio.")]
        [StringLength(320, ErrorMessage = "El correo no puede exceder los 320 caracteres.")]
        public required string Correo
        {
            get => _correo;
            set
            {
                if (value.Length > 320)
                {
                    throw new ArgumentException("El correo no puede exceder los 320 caracteres.");
                }
                _correo = value;
            }
        }

        private string _contrasena;
        [Required(ErrorMessage = "El correor es obligatorio.")]
        [StringLength(64, ErrorMessage = "La contrasena no puede exceder los 64 caracteres.")]
        public required string Contrasena
        {
            get => _contrasena;
            set
            {
                if (value.Length > 64)
                {
                    throw new ArgumentException("La contrasena no puede exceder los 64 caracteres.");
                }
                _contrasena = value;
            }
        }

        private string _nombreUsuario;
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        public required string Nombre_Usuario
        {
            get => _contrasena;
            set
            {
                if (value.Length > 50)
                {
                    throw new ArgumentException("El nombre de usuario no puede exceder los 50 caracteres.");
                }
                _contrasena = value;
            }
        }
        [Required(ErrorMessage = "El campo 'EsAdmin' es obligatorio.")]
        public required bool EsAdmin { get; set; }
    }
}
