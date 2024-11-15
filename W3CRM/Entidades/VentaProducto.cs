using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class VentaProducto
    {
        [Key]
        public required int ID_Lote { get; set; }
        [Key]
        public required int Folio_Venta { get; set; }
        [Required]
        public int Cantidad_Vendida { get; set; }
    }
}
