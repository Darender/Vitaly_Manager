using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        private string _nombre;
        private string _apellidoPaterno;
        private string _apellidoMaterno;
        private string _correo;
        private string _contraseña;
        private string _rol;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public required string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre es obligatorio.");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("El nombre no puede exceder los 50 caracteres.");
                }
                _nombre = value;
            }
        }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(30, ErrorMessage = "El apellido paterno no puede exceder los 30 caracteres.")]
        public required string ApellidoPaterno
        {
            get => _apellidoPaterno;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El apellido paterno es obligatorio.");
                }
                if (value.Length > 30)
                {
                    throw new ArgumentException("El apellido paterno no puede exceder los 30 caracteres.");
                }
                _apellidoPaterno = value;
            }
        }

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        [StringLength(30, ErrorMessage = "El apellido materno no puede exceder los 30 caracteres.")]
        public required string ApellidoMaterno
        {
            get => _apellidoMaterno;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El apellido materno es obligatorio.");
                }
                if (value.Length > 30)
                {
                    throw new ArgumentException("El apellido materno no puede exceder los 30 caracteres.");
                }
                _apellidoMaterno = value;
            }
        }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        public required string Correo
        {
            get => _correo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El correo es obligatorio.");
                }
                if (value.Length > 100)
                {
                    throw new ArgumentException("El correo no puede exceder los 100 caracteres.");
                }
                _correo = value;
            }
        }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, ErrorMessage = "La contraseña no puede exceder los 50 caracteres.")]
        public required string Contraseña
        {
            get => _contraseña;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("La contraseña es obligatoria.");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("La contraseña no puede exceder los 50 caracteres.");
                }
                _contraseña = value;
            }
        }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(20, ErrorMessage = "El rol no puede exceder los 20 caracteres.")]
        public required string Rol
        {
            get => _rol;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El rol es obligatorio.");
                }
                if (value.Length > 20)
                {
                    throw new ArgumentException("El rol no puede exceder los 20 caracteres.");
                }
                _rol = value;
            }
        }

        [Required]
        public required DateTime FechaRegistro { get; set; }
    }
}
