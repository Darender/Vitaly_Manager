using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class ComprasController : Controller
    {
        public IActionResult AgregarLoteProducto() {
            return View(this);
        }
    }
}
