using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Proveedor
	{
		[Key]
		public required int ID { get; set; }

		private string _nombre;
		private int _telefono;

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(40, ErrorMessage = "El nombre no puede exceder los 40 caracteres.")]
		public required string Nombre
		{
			get => _nombre;
			set
			{
				if (value.Length > 40)
				{
					throw new ArgumentException("El nombre no puede exceder los 40 caracteres.");
				}
				_nombre = value;
			}
		}

		[Required(ErrorMessage = "El teléfono es obligatorio.")]
		[StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
		public int Telefono
		{
			get => _telefono;
			set
			{
				if (value.ToString().Length > 20)
				{
					throw new ArgumentException("El teléfono no puede exceder los 20 caracteres.");
				}
				_telefono = value;
			}
		}

		public string? Pagina_Contacto { get; set; }
	}
}
