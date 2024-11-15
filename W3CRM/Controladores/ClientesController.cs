using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class ClientesController : Controller
    {
        public IActionResult AgregarClientes()
        {
            ClientesController controlador = new ClientesController();
            return View(controlador);
        }
    }
}
