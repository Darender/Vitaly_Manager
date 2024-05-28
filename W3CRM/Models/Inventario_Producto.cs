using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitaly_Manager.Models
{
    public class Inventario_Producto
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Tipo_inventario"), Required]
        public int Tipo { get; set; }
        
        [Required]
        public required string Nombre { get; set; }

        [Required]
        public required string Proveedor { get; set; }
        [Required]
        public required DateTime Ingreso { get; set; }
    }
}
