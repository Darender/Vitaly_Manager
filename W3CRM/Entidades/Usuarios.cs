using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        private string _nombre;
        private string _correo;
        private string _contraseña;
        private bool _esAdmin;

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

        public bool esAdmin
        {
            get => _esAdmin;
            set
            {  
                _esAdmin = value;
            }
        }
    }
}
