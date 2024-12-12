using System.ComponentModel.DataAnnotations;

namespace Vitaly_Manager.Entidades
{
    public class Parametros
    {
        [Key]
        public int ID_Parametros { get; set; }

        private decimal _IVA;

        [Range(typeof(decimal), "0.00", "99999999.99", ErrorMessage = "El valor debe estar entre 0.00 y 99999999.99.")]
        [RegularExpression(@"\d{1,8}(\.\d{1,2})?", ErrorMessage = "El valor debe tener hasta 8 dígitos enteros y 2 decimales.")]
        public decimal IVA
        {
            get => _IVA;
            set
            {
                if (value < 0 || value > 99999999.99m)
                {
                    throw new ArgumentOutOfRangeException(nameof(IVA), "El valor debe estar entre 0.00 y 99999999.99.");
                }
                _IVA = Math.Round(value, 2);
            }
        }
    }
}
