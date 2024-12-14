using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class VentaProducto
    {
        [Key]
        [Required(ErrorMessage = "El ID del lote de compra es obligatorio.")]
        public required int IdCompraLote { get; set; }

        [Key]
        [Required(ErrorMessage = "El folio de la venta es obligatorio.")]
        public required int FolioVenta { get; set; }

        [Required(ErrorMessage = "La cantidad vendida es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad vendida debe ser mayor a 0.")]
        public required int CantidadVendida { get; set; }
    }
}
