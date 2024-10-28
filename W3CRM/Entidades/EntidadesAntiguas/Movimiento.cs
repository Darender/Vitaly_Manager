using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades.EntidadesAntiguas
{
    public class Movimiento
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public required string Titulo { get; set; }
        [Required]
        public required string Descripcion { get; set; }
        [Required]
        public required int Usuario_id { get; set; }
        [Required]
        public required int Tipo_movimiento_id { get; set; }
        [Required]
        public required int Entidad_id { get; set; }

        [Required]
        public required DateTime Ingreso { get; set; }
    }
}
