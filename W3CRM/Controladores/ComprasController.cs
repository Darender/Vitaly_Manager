using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ComprasController : Controller
    {

        public List<LoteProducto> listaLoteProducto = DataLoteProducto.ListaLoteProducto(out _, out _);

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

        [HttpPost]
        public JsonResult AgregarNuevoLoteProducto(int idCatalogoProd, decimal IVA, bool esMaterial, DateTime fechaIngreso, DateTime? fechaVencimiento, int cantidad, decimal precioVenta, decimal precioCompra, decimal margenGanancia)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar el lote de producto.";
            List<string> fallidos = new List<string>();

            try
            {
                // Validación de ID del catálogo de producto
                if (idCatalogoProd <= 0)
                {
                    mensaje = "El ID del catálogo de producto debe ser mayor a 0.";
                    fallidos.Add("idCatalogoProd");
                }

                // Validación del IVA
                if (IVA < 0 || IVA > 100)
                {
                    mensaje = "El IVA debe estar entre 0 y 100.";
                    fallidos.Add("IVA");
                }

                // Validación de la fecha de ingreso
                if (fechaIngreso > DateTime.Now)
                {
                    mensaje = "La fecha de ingreso no puede ser futura.";
                    fallidos.Add("fechaIngreso");
                }

                // Validación de la fecha de vencimiento
                if (fechaVencimiento.HasValue && fechaVencimiento <= fechaIngreso)
                {
                    mensaje = "La fecha de vencimiento debe ser posterior a la fecha de ingreso.";
                    fallidos.Add("fechaVencimiento");
                }

                // Validación de cantidad
                if (cantidad <= 0)
                {
                    mensaje = "La cantidad debe ser mayor a 0.";
                    fallidos.Add("cantidad");
                }

                // Validación de precios
                if (precioCompra <= 0)
                {
                    mensaje = "El precio de compra debe ser mayor a 0.";
                    fallidos.Add("precioCompra");
                }
                if (precioVenta <= 0)
                {
                    mensaje = "El precio de venta debe ser mayor a 0.";
                    fallidos.Add("precioVenta");
                }
                if (margenGanancia < 0)
                {
                    mensaje = "El margen de ganancia no puede ser negativo.";
                    fallidos.Add("margenGanancia");
                }

                if (fallidos.Count == 0)
                {
                    LoteProducto nuevo = new LoteProducto
                    {
                        ID_CatalogoProducto = idCatalogoProd,
                        IVA = IVA,
                        EsMaterial = esMaterial,
                        Fecha_Ingreso = fechaIngreso,
                        Fecha_Vencimiento = fechaVencimiento ?? default(DateTime),
                        Cantidad = cantidad,
                        Precio_Venta = precioVenta,
                        Precio_Compra = precioCompra,
                        Margen_Ganancia = margenGanancia
                    };

                    resultado = DataLoteProducto.Agregar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el lote de producto: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
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

        //SECCION DE CONSULTA//
        public IActionResult ConsultarLoteProducto()
        {
            string mensaje;
            bool exito;

            // Consulta la lista de lotes de producto
            var listaLoteProducto = DataLoteProducto.ListaLoteProducto(out exito, out mensaje);

            if (!exito)
            {
                ViewData["Error"] = mensaje;
                return View(new List<LoteProducto>());
            }

            // Pasa la lista de lotes a la vista en caso de éxito
            return View(listaLoteProducto);
        }

    }
}
