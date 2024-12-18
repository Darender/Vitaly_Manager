using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class GastosController : Controller
    {


        public IActionResult RegistrarGastos() 
        {
            return View(this);
        }

        public IActionResult ConsultarGastos()
        {
            string respuesta;
            bool exito;

            List<Gasto> gastos = DataGasto.ListaGastos(out respuesta, out exito);

            if (!exito)
            {
                ViewBag.Error = respuesta;
                return View("Error");
            }

            ViewBag.Mensaje = respuesta;
            return View(gastos);
        }

        [HttpPost]
        public JsonResult Agregar(decimal monto, string descripcion, int idTipoGasto, DateTime fechaRealizado)
        {
            string mensaje;
            int id;

            Gasto nuevoGasto = new Gasto
            {
                Monto = monto,
                Descripcion = descripcion,
                IdTipoGasto = idTipoGasto,
                FechaRealizado = fechaRealizado
            };

            bool resultado = DataGasto.Agregar(nuevoGasto, out mensaje, out id);

            return Json(new { success = resultado, message = mensaje, id });
        }

        //Funcionamiento para eliminar el gasto
        [HttpPost]
        public JsonResult Eliminar(int idGasto)
        {
            string mensaje;

            bool resultado = DataGasto.Eliminar(idGasto, out mensaje);

            return Json(new { success = resultado, message = mensaje });
        }

        // Método para obtener la lista actualizada de gastos
        [HttpGet]
        public JsonResult ActualizarTabla()
        {
            string mensaje;
            bool exito;

            // Obtenemos la lista de gastos actualizada
            List<Gasto> gastos = DataGasto.ListaGastos(out mensaje, out exito);

            if (!exito)
            {
                return Json(new { success = false, message = mensaje });
            }

            // Devolvemos la lista en formato JSON
            return Json(new { success = true, data = gastos });
        }
    }
}

