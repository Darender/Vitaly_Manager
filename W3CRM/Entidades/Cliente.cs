using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Cliente
	{
		[Key]
		public required int ID_Cliente { get; set; }

		private string _nombreCliente;
		private string _apellidoP;
		private string _apellidoM;
		private string? _telefono;
		private string? _genero;

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(30, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
		public required string Nombre_Cliente
		{
			get => _nombreCliente;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException("El nombre es obligatorio.");
				}
				if (value.Length > 50)
				{
					throw new ArgumentException("El nombre no puede exceder los 50 caracteres.");
				}
				_nombreCliente = value;
			}
		}

		[Required(ErrorMessage = "El apellido paterno es obligatorio.")]
		[StringLength(30, ErrorMessage = "El apellido paterno no puede exceder los 30 caracteres.")]
		public required string ApellidoP
		{
			get => _apellidoP;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException("El apellido paterno es obligatorio.");
				}
				if (value.Length > 30)
				{
					throw new ArgumentException("El apellido paterno no puede exceder los 30 caracteres.");
				}
				_apellidoP = value;
			}
		}

		[Required(ErrorMessage = "El apellido materno es obligatorio.")]
		[StringLength(30, ErrorMessage = "El apellido materno no puede exceder los 30 caracteres.")]
		public required string ApellidoM
		{
			get => _apellidoM;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException("El apellido materno es obligatorio.");
				}
				if (value.Length > 30)
				{
					throw new ArgumentException("El apellido materno no puede exceder los 30 caracteres.");
				}
				_apellidoM = value;
			}
		}

		[StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
		public string? Telefono
		{
			get => _telefono;
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > 20)
				{
					throw new ArgumentException("El teléfono no puede exceder los 20 caracteres.");
				}
				_telefono = value;
			}
		}

		[StringLength(10, ErrorMessage = "El género no puede exceder los 10 caracteres.")]
		public string? Genero
		{
			get => _genero;
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > 10)
				{
					throw new ArgumentException("El género no puede exceder los 10 caracteres.");
				}
				_genero = value;
			}
		}

		public string? ContractoAlternativo { get; set; }
		public int? Edad { get; set; }

		[Required]
		public required DateOnly FechaRegistro { get; set; }
	}
}