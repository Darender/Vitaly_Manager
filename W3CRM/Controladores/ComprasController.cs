using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Se encarga de devolver la vista para agregar un nuevo lote de producto.
        /// Obtiene las listas, ListaTipos y ListaProductos, así como el valor actual del IVA,
        /// y los pasa a la vista para ser utilizados al llenar el formulario de agregar lote de producto.
        /// </summary>
        /// <returns>Devuelve la vista "AgregarLoteProducto" con los datos necesarios para mostrar el formulario.</returns>
        public IActionResult AgregarLoteProducto()
        {
            string respuesta;
            bool exito;

            // Obtiene las listas de productos y tipos
            List<TipoProducto> listaTipos = DataTipoProducto.ListaTiposProductos(out respuesta, out exito);
            List<CatalogoProducto> listaProductos = DataCatalogoProducto.ListaCatalogoProductos(out respuesta, out exito);

            // Deberia obtener el IVA
            Parametros parametros = DataIVA.UltimoIVA(out respuesta, out exito);
            decimal iva = (exito && parametros.IVA > 0) ? parametros.IVA : 0;

            // Pasar datos a la vista
            ViewBag.ListaTipos = listaTipos;
            ViewBag.ListaProductos = listaProductos;
            ViewBag.IVA = iva; // Pasar el IVA a la vista

            return View();
        }

        /// <summary>
        /// Fórmula que incluye el precio de compra, el margen de ganancia y el IVA.
        /// Luego, intenta guardar el lote en la base de datos y muestra un mensaje de éxito o  tambien de error.
        /// </summary>
        /// <param name="lote">Entidad de tipo LoteProducto que contiene la información que agrega.</param>
        /// <returns>Devuelve la vista actual con un mensaje de éxito o error.</returns>
        [HttpPost]
        public IActionResult AgregarLoteProducto(LoteProducto lote)
        {
            string respuesta;
            bool exito;
            //Obtiene el IVA de loteProducto
            decimal iva = lote.IVA; 

            if (lote.Precio_Venta == 0)
            {
                lote.Precio_Venta = lote.Precio_Compra + (lote.Precio_Compra * lote.Margen_Ganancia / 100) + (lote.Precio_Compra * iva / 100);
                //Formula para calcular el precio de venta, pero no pude asignar el iva
            }
            //Guarda el lote en la base de datos
            bool guardado = DataLoteProducto.Agregar(lote, out respuesta);

            if (guardado)
            {
                TempData["Mensaje"] = "Lote guardado con éxito.";
                return RedirectToAction("Index");
            }
            else
            {
                // Si hubo un error al guardar, madara un mensaje
                ViewBag.Error = "Error al guardar el lote: " + respuesta;
                return View(lote);
            }
        }


        public IActionResult ConsultarLoteProducto()
        {
            ComprasController controlador = new ComprasController();
            return View(controlador);
        }
    }
}
