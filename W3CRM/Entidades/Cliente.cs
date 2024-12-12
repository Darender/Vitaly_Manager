using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(80, ErrorMessage = "Los apellidos no pueden exceder los 80 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30, ErrorMessage = "El teléfono no puede exceder los 30 caracteres.")]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El género no puede exceder los 20 caracteres.")]
        public string? Genero { get; set; }

        public string? ContactoAlternativo { get; set; }

        public int? Edad { get; set; }
    }
}
