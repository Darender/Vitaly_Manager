using System.ComponentModel.DataAnnotations;

namespace W3CRM.Controllers
{
    public class Tipo_inventario
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public required string Nombre { get; set; }
    }
}
