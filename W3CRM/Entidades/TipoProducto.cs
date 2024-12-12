using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class TipoProducto
	{
        [Key]
        public int IdTipoProducto { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de producto es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre no puede exceder los 40 caracteres.")]
        public string NombreTipoProducto { get; set; } = string.Empty;
    }
}
