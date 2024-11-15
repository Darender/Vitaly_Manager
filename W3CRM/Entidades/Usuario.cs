using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Usuario
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
