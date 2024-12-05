using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ComprasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AgregarLoteProducto()
        {
            ComprasController controlador = new ComprasController();
            return View(controlador);
        }

        public IActionResult ConsultarLoteProducto()
        {
            ComprasController controlador = new ComprasController();
            return View(controlador);
        }
    }
}
