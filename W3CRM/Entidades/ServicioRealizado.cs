using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class ServicioRealizado
    {
        [Key]
        public int IdServicioRealizado { get; set; }

        [Required(ErrorMessage = "El ID del servicio general es obligatorio.")]
        public required int IdServicioGeneral { get; set; }

        [Required(ErrorMessage = "El ID del catálogo de servicio es obligatorio.")]
        public required int IdCatalogoServicio { get; set; }

        [Required(ErrorMessage = "El precio calculado del servicio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio calculado debe ser un valor positivo.")]
        public required decimal PrecioServicioCalculado { get; set; }
    }
}
