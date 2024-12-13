using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Proveedor
	{
		[Key]
		public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "El teléfono no puede exceder los 30 caracteres.")]
        public string? Telefono { get; set; }
        public string? ContactoAlternativo { get; set; }
	}
}
