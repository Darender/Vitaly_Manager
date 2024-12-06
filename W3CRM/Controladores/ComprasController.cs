using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{

    public class ComprasController : Controller
    {
        public List<TipoProducto> listaTiposProductos = DataTipoProducto.ListaTiposProductos(out _, out _);

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AgregarLoteProducto()
        {
            return View(listaTiposProductos);
        }

        public IActionResult ConsultarLoteProducto()
        {
            ComprasController controlador = new ComprasController();
            return View(controlador);
        }
    }
}
