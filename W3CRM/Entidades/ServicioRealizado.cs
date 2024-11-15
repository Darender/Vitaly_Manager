using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades
{
    public class ServicioRealizado
    {
        [Key]
        public required int IdServRealizado { get; set; }

        private int _idServGeneral;
        private int _idCatalogoServ;
        private decimal _precioServ;

        [Required(ErrorMessage = "El ID del servicio general es obligatorio.")]
        public required int IdServGeneral
        {
            get => _idServGeneral;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El ID del servicio general debe ser un número positivo.");
                }
                _idServGeneral = value;
            }
        }

        [Required(ErrorMessage = "El ID del catálogo de servicios es obligatorio.")]
        public required int IdCatalogoServ
        {
            get => _idCatalogoServ;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El ID del catálogo de servicios debe ser un número positivo.");
                }
                _idCatalogoServ = value;
            }
        }

        [Required(ErrorMessage = "El precio del servicio es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio del servicio debe ser mayor a cero.")]
        public required decimal PrecioServ
        {
            get => _precioServ;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El precio del servicio debe ser mayor a cero.");
                }
                _precioServ = value;
            }
        }
    }
}
