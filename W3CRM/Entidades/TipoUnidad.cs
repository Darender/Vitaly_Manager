using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class TipoUnidad
	{
		[Key]
		public required int ID_TipoUnidad { get; set; }

		private string _nombreTipoUnidad;

		[Required(ErrorMessage = "La unidad de medida es obligatoria.")]
		[StringLength(10, ErrorMessage = "El nombre de la unidad de medida no puede exceder los 10 caracteres.")]
		public required string Unidad_Medida
		{
			get => _nombreTipoUnidad;
			set
			{
				if (value.Length > 10)
				{
					throw new ArgumentException("El nombre de la unidad de medida no puede exceder los 10 caracteres.");
				}
                _nombreTipoUnidad = value;
			}
		}
	}
}
