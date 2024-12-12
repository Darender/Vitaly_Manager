using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class ServicioGeneral
    {
        [Key]
        public int IdServGeneral { get; set; }

        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El ingreso total es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El ingreso total debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal IngresoTotal { get; set; }

        [Required(ErrorMessage = "La fecha de realización es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRealizado { get; set; }
    }
}

