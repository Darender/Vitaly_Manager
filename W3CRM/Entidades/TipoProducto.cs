﻿using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class TipoProducto
	{
		[Key]
		public required int ID_TipoProducto { get; set; }

		private string _tipo;

		[Required(ErrorMessage = "El tipo es obligatorio.")]
		[StringLength(20, ErrorMessage = "El nombre del tipo no puede exceder los 20 caracteres.")]
		public required string Tipo
		{
			get => _tipo;
			set
			{
				if (value.Length > 20)
				{
					throw new ArgumentException("El nombre del tipo no puede exceder los 20 caracteres.");
				}
				_tipo = value;
			}
		}
	}
}
