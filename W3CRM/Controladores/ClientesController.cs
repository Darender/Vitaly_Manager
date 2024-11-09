using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    [ApiController]
    [Route("ClientesController")]
    public class ClientesController : Controller
    {
        public List<Cliente> ListaClientes = DatosClientes.ListaClientes(out _, out _);

        [HttpGet]
        [Route("ConsultaCatalogoClientes")]
        public IActionResult ConsultaCatalogoClientes()
        {
            ClientesController controlador = new ClientesController();
            return View(controlador);
        }
    }
}
