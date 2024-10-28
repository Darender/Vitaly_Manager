using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class VentaProducto
    {
        [Key]
        public required string ID_Lote { get; set; }
        [Key]
        public required string Folio_Venta { get; set; }
        [Required]
        public int Cantidad { get; set; }
    }
}
