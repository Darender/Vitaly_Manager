using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Venta
	{
		[Key]
		public required int Folio { get; set; }

		[Required(ErrorMessage = "El ID del cliente es obligatorio.")]
		[ForeignKey("Cliente")]
		public required int ID_Cliente { get; set; }

		private DateOnly _fechaVenta;
		[Required(ErrorMessage = "La fecha de venta es obligatoria.")]
		public required DateOnly Fecha_Venta
		{
			get => _fechaVenta;
			set
			{
				if (value > DateOnly.FromDateTime(DateTime.Now))
				{
					throw new ArgumentException("La fecha de venta no puede ser en el futuro.");
				}
				_fechaVenta = value;
			}
		}
	}
}