using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vitaly_Manager.Controladores
{
    public class ConfiguracionController : Controller
    {
            public ActionResult ConsultarPerfilUsuario()
            {
                // Aquí puedes pasar datos a la vista, si es necesario
                return View();
            }
        
    }
}
