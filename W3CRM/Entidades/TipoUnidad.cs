using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class TipoUnidad
	{
        [Key]
        public int IdTipoUnidad { get; set; }

        [Required(ErrorMessage = "El nombre del tipo de unidad es obligatorio.")]
        [StringLength(40, ErrorMessage = "El nombre no puede exceder los 40 caracteres.")]
        public string NombreTipoUnidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La abreviatura del tipo de unidad es obligatoria.")]
        [StringLength(10, ErrorMessage = "La abreviatura no puede exceder los 10 caracteres.")]
        public string Abreviatura { get; set; } = string.Empty;
    }
}
