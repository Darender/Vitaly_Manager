using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class GastosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GastosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Gastos/Create
        public IActionResult RegistrarGasto()
        {
            return View();
        }

        // POST: Gastos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarGasto(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    gasto.Fecha = DateTime.Now; // Registrar fecha actual automáticamente
                    _context.Gastos.Add(gasto);
                    _context.SaveChanges();
                    TempData["Mensaje"] = "Gasto registrado exitosamente.";
                    return RedirectToAction("RegistrarGasto");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al registrar el gasto: {ex.Message}");
                }
            }
            return View(gasto);
        }

        // MÉTODO OPCIONAL: Listar Gastos
        public IActionResult ListarGastos()
        {
            var gastos = _context.Gastos.ToList();
            return View(gastos);
        }
    }
}

