using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades
{
    public class ServicioGeneral
    {
        [Key]
        public required int ID_ServicioGeneral { get; set; }

        [Required]
        [ForeignKey("Cliente")]
        public required int IdCliente { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public required decimal ImporteTotal { get; set; }

        [Required]
        public required DateTime FechaRealizado { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [ForeignKey("Parametros")]
        public required decimal IVA { get; set; }
    }
}
