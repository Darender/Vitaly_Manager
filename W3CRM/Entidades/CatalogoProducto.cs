using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class CatalogoProducto
	{
		[Key]
		public int ID_CatalogoProducto { get; set; }

		private string _nombreProducto;

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
		public required string Nombre_Producto
		{
			get => _nombreProducto;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException("El nombre es obligatorio.");
				}
				if (value.Length > 100)
				{
					throw new ArgumentException("El nombre no puede exceder los 100 caracteres.");
				}
				_nombreProducto = value;
			}
		}

		[Required(ErrorMessage = "Las unidades son obligatorias.")]
		public required int Cantidad_Unidades { get; set; }

		public string? Pagina_Producto { get; set; }

		[ForeignKey("Proveedor"), Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
		public int ID_Proveedor { get; set; }

		[ForeignKey("TipoUnidad"), Required(ErrorMessage = "El ID del tipo de unidad es obligatorio.")]
		public int ID_TipoUnidad { get; set; }

		[ForeignKey("TipoProducto"), Required(ErrorMessage = "El ID del tipo de producto es obligatorio.")]
		public int ID_TipoProducto { get; set; }
	}
}
