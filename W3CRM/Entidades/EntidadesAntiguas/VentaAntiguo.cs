using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Entidades.EntidadesAntiguas
{
    public class VentaAntiguo
    {
        [Key]
        public required int ID { get; set; }

        [Required]
        [ForeignKey("Cliente")]
        public required int Cliente_id { get; set; }

        [Required]
        [ForeignKey("Inventario_Producto")]
        public required int Producto_id { get; set; }

        [Required]
        public required float Ingresos { get; set; }
        [Required]
        public required float Cantidad { get; set; }
        [Required]
        public required DateTime Fecha_Ingreso { get; set; }
    }
}
