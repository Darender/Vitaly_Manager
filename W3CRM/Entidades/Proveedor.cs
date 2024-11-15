using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
	public class Proveedor
	{
		[Key]
		public required int ID_Proveedor { get; set; }

		private string _nombreProveedor;

		[Required(ErrorMessage = "El nombre es obligatorio.")]
		[StringLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
		public required string Nombre_Proveedor
		{
			get => _nombreProveedor;
			set
			{
				if (value.Length > 80)
				{
					throw new ArgumentException("El nombre no puede exceder los 80 caracteres.");
				}
				_nombreProveedor = value;
			}
		}

        public string? Telefono { get; set; }
        public string? Pagina_Contacto { get; set; }
	}
}
