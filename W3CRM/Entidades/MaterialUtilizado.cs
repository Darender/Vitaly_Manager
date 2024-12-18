using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class MaterialUtilizado
    {
        [Key]
        [Required(ErrorMessage = "El ID del servicio realizado es obligatorio.")]
        public int IdServicioRealizado { get; set; }

        [Key]
        [Required(ErrorMessage = "El ID de la compra lote es obligatorio.")]
        public int IdCompraLote { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La cantidad de envases debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal? CantidadEnvases { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La cantidad de unidades debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal? CantidadUnidades { get; set; }
    }
}
