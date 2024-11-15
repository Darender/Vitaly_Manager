using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Venta
	{
		[Key]
		public required int FolioVenta { get; set; }

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

        private decimal _importeTotal;

        [Range(typeof(decimal), "-99999999.99", "99999999.99", ErrorMessage = "El valor debe estar entre -99999999.99 y 99999999.99.")]
        [RegularExpression(@"-?\d{1,8}(\.\d{1,2})?", ErrorMessage = "El valor debe tener hasta 8 dígitos enteros y 2 decimales.")]
        public decimal Importe_Total
        {
            get => _importeTotal;
            set
            {
                if (value < -99999999.99m || value > 99999999.99m)
                {
                    throw new ArgumentOutOfRangeException(nameof(Importe_Total), "El valor debe estar entre -99999999.99 y 99999999.99.");
                }
                _importeTotal = Math.Round(value, 2);
            }
        }
    }
}