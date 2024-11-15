using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class MaterialesUtilizados
    {
        [Key]
        public required int ID_ServicioRealizado { get; set; }
        [Key]
        public required int ID_LoteProducto { get; set; }
        [Required]
        public int Cantidad_Usada { get; set; }
    }
}
