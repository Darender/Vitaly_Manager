using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class FinanzasController : Controller
    {
        private readonly DataGasto _dataGasto = new DataGasto();

        // Vista para consultar ganancias
        public ActionResult ConsultaGanancias()
        {
            return View(this);
        }

        // Acción para consultar todos los gastos
        [HttpGet]
        public IActionResult ConsultarGastos()
        {
            try
            {
                var gastos = _dataGasto.ConsultarGastos(out string mensaje, out bool exito);
                return Ok(new { success = true, data = gastos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult AgregarGasto(string concepto, decimal monto, DateTime fecha, int? idProductoComprado = null)
        {
            try
            {
                // Crear el objeto Gasto
                Gasto nuevoGasto = new Gasto
                {
                    concepto = concepto,
                    monto = monto,
                    fecha = fecha,
                    idProductoComprado = idProductoComprado
                };

                // Llamar al método para agregar el gasto
                if (_dataGasto.AgregarGasto(nuevoGasto, out string mensaje))
                {
                    return Json(new { success = true, message = mensaje });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error inesperado: {ex.Message}" });
            }
        }



    }
}
