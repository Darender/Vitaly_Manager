using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Models
{
    public class Inventario_Instancia
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Inventario_Producto"), Required]
        public int Producto_id { get; set; }
        
        [Required]
        public required float Cantidad { get; set; }
        [Required]
        public required float Costo { get; set; }
        public required DateTime? Vencimiento { get; set; }
        public required DateTime? Llegada { get; set; }

        [Required]
        public required DateTime Ingreso { get; set; }
    }
}
