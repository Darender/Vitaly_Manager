using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class CatalogoProducto
    {
        [Key]
        public int IdCatalogoProducto { get; set; }

        [Required(ErrorMessage = "El tipo de producto es obligatorio.")]
        public int IdTipoProducto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre del producto no puede exceder los 255 caracteres.")]
        public string NombreProducto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        public string? PaginaWebProducto { get; set; }

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de unidad es obligatorio.")]
        public int IdTipoUnidad { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El ingreso total debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal? CantidadPorUnidad { get; set; }
    }
}
