using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public required string Nombre { get; set; }

        [StringLength(100, ErrorMessage = "Los apellidos no pueden tener más de 100 caracteres.")]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [StringLength(255, ErrorMessage = "El correo electrónico no puede tener más de 255 caracteres.")]
        public required string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, ErrorMessage = "La contraseña no puede tener más de 255 caracteres.")]
        public required string Password { get; set; }

        [StringLength(25, ErrorMessage = "El teléfono no puede tener más de 25 caracteres.")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El estado activo es obligatorio.")]
        public required bool EstadoActivo { get; set; }

        [Required(ErrorMessage = "Debe especificarse si el usuario es administrador.")]
        public required bool EsAdministrador { get; set; }

        public int? IdImagenUsuario { get; set; }
    }
}
