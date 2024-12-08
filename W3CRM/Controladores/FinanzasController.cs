using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class FinanzasController : Controller
    {
        public ActionResult ConsultaGanancias()
        {
            return View(this);
        }
    }
}
