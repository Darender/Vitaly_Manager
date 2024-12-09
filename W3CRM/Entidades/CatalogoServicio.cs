using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class CatalogoServicio
    {
        [Key]
        public int ID_CatalogoServicio { get; set; }

        private string _nombreServ;
        private string _descripcion;
        private TimeSpan _duracionPromedio;

        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del servicio no puede exceder los 50 caracteres.")]
        public required string NombreServ
        {
            get => _nombreServ;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre del servicio es obligatorio.");
                }
                if (value.Length > 50)
                {
                    throw new ArgumentException("El nombre del servicio no puede exceder los 50 caracteres.");
                }
                _nombreServ = value;
            }
        }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder los 100 caracteres.")]
        public required string Descripcion
        {
            get => _descripcion;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("La descripción es obligatoria.");
                }
                if (value.Length > 100)
                {
                    throw new ArgumentException("La descripción no puede exceder los 100 caracteres.");
                }
                _descripcion = value;
            }
        }

        public  TimeSpan DuracionPromedio
        {
            get => _duracionPromedio;
            set
            {
                if (value == TimeSpan.Zero)
                {
                    throw new ArgumentException("La duración promedio es obligatoria.");
                }
                _duracionPromedio = value;
            }
        }
    }
}
