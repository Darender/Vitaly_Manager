using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Models
{
    public class Producto_Completo
    {
        public int Instancia_id { get; set; }
        public int Producto_id { get; set; }
        public int Tipo_id { get; set; }
        public string? Nombre_Producto { get; set; }
        public string? Nombre_Tipo { get; set; }
        public string? Proveedor { get; set; }
        public float Cantidad { get; set; }
        public float Costo { get; set; }
        public DateTime? Vencimiento { get; set; }
        public DateTime? Llegada { get; set; }
    }
}
