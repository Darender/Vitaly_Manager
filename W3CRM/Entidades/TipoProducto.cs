using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class TipoProducto
	{
		[Key]
		public required int ID { get; set; }

		private string _tipo;

		[Required(ErrorMessage = "El tipo es obligatorio.")]
		[StringLength(20, ErrorMessage = "El nombre del tipo no puede exceder los 10 caracteres.")]
		public required string Tipo
		{
			get => _tipo;
			set
			{
				if (value.Length > 10)
				{
					throw new ArgumentException("El nombre del tipo no puede exceder los 10 caracteres.");
				}
				_tipo = value;
			}
		}
	}
}
