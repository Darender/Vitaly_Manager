using System;
using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class CompraLote
    {
        [Key]
        public int IdCompraLote { get; set; }

        [Required(ErrorMessage = "El ID del catálogo de producto es obligatorio.")]
        public int IdCatalogoProducto { get; set; }

        [Required(ErrorMessage = "La cantidad de unidades es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de unidades debe ser mayor a 0.")]
        public int CantidadUnidades { get; set; }

        [Required(ErrorMessage = "El costo total es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo total debe ser un valor positivo.")]
        public decimal CostoTotal { get; set; }

        [Required(ErrorMessage = "El ID del gasto es obligatorio.")]
        public int IdGasto { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El campo 'esMaterial' es obligatorio.")]
        public bool EsMaterial { get; set; }

        [Required(ErrorMessage = "El porcentaje de margen de ganancia es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de margen de ganancia debe estar entre 0 y 100.")]
        public decimal PorcentajeMargenGanancia { get; set; }

        [Required(ErrorMessage = "El precio de venta sugerido es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta sugerido debe ser un valor positivo.")]
        public decimal PrecioVentaSugerido { get; set; }

        [Required(ErrorMessage = "El ID de parámetros es obligatorio.")]
        public int IdParametros { get; set; }
    }
}
