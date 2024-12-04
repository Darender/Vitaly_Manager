using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        public IActionResult AgregarProductos()
        {
            return View(this);
        }
    }
}
