using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades
{
    public class Gasto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idGasto { get; set; }

        [Required]
        public required string concepto { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public required decimal monto { get; set; }

        [Required]
        public required DateTime fecha { get; set; }

        [ForeignKey("LoteProducto")]
        public int? idProductoComprado { get; set; }
    }

}
