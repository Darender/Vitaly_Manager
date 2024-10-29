﻿using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class LoteProducto
	{
		[Key]
		public string ID { get; set; }

		[Required(ErrorMessage = "El campo 'EsMaterial' es obligatorio.")]
		public required bool EsMaterial { get; set; }

		[Required(ErrorMessage = "La fecha de ingreso es obligatoria.")]
		public required DateOnly Fecha_Ingreso { get; set; }

		[Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
		public required DateOnly Fecha_Vencimiento { get; set; }

		[Required(ErrorMessage = "La cantidad es obligatoria.")]
		public required int Cantidad { get; set; }

		private decimal _precioVenta;
		private decimal _precioCompra;
		private decimal _margenGanancia;

		[Range(typeof(decimal), "0.00", "99999999.99", ErrorMessage = "El valor debe estar entre 0.00 y 99999999.99.")]
		[RegularExpression(@"\d{1,8}(\.\d{1,2})?", ErrorMessage = "El valor debe tener hasta 8 dígitos enteros y 2 decimales.")]
		public decimal Precio_Venta
		{
			get => _precioVenta;
			set
			{
				if (value < 0 || value > 99999999.99m)
				{
					throw new ArgumentOutOfRangeException(nameof(Precio_Venta), "El valor debe estar entre 0.00 y 99999999.99.");
				}
				_precioVenta = Math.Round(value, 2);
			}
		}

		[Range(typeof(decimal), "0.00", "99999999.99", ErrorMessage = "El valor debe estar entre 0.00 y 99999999.99.")]
		[RegularExpression(@"\d{1,8}(\.\d{1,2})?", ErrorMessage = "El valor debe tener hasta 8 dígitos enteros y 2 decimales.")]
		public decimal Precio_Compra
		{
			get => _precioCompra;
			set
			{
				if (value < 0 || value > 99999999.99m)
				{
					throw new ArgumentOutOfRangeException(nameof(Precio_Compra), "El valor debe estar entre 0.00 y 99999999.99.");
				}
				_precioCompra = Math.Round(value, 2);
			}
		}

		[Range(typeof(decimal), "0.00", "99999999.999", ErrorMessage = "El valor debe estar entre 0.00 y 99999999.999.")]
		[RegularExpression(@"\d{1,8}(\.\d{1,3})?", ErrorMessage = "El valor debe tener hasta 8 dígitos enteros y 3 decimales.")]
		public decimal Margen_Ganancia
		{
			get => _margenGanancia;
			set
			{
				if (value < 0 || value > 99999999.999m)
				{
					throw new ArgumentOutOfRangeException(nameof(Margen_Ganancia), "El valor debe estar entre 0.00 y 99999999.999.");
				}
				_margenGanancia = Math.Round(value, 3);
			}
		}
	}
}