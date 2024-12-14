using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Gasto
    {
        [Key]
        public int IdGasto { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public required decimal Monto { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El ID del tipo de gasto es obligatorio.")]
        public required int IdTipoGasto { get; set; }

        [Required(ErrorMessage = "La fecha de realización es obligatoria.")]
        public required DateTime FechaRealizado { get; set; }
    }
}
