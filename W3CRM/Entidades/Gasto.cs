using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades
{
    public class Gasto
    {
        [Key]
        public required int idServicioGeneral { get; set; }

        [Required]
        public required string concepto { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public required decimal monto { get; set; }

        [Required]
        public required DateTime fecha { get; set; }

        public int? idProductoComprado {get;set;}


    }
}
