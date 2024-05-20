using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Nombres { get; set; }

        public string? Apellidos { get; set; }
        [Required]
        public required string Password { get; set; }

        [EmailAddress]
        public required string Correo { get; set; }

        [Required]
        public required string Telefono { get; set; }

        public string? AreaDeTrabajo { get; set; }

        public string? Genero { get; set; }
        [Required]
        public required DateTime Ingreso { get; set; }

        public IFormFile? FotoPerfil { get; set; }

        public bool? Activo { get; set; }
    }
}
