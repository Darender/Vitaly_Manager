using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades.EntidadesAntiguas
{
    public class Tipo_inventario
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public required string Nombre { get; set; }
    }
}
