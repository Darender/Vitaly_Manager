using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        //public List<TipoProducto> ListaTipos = DataTipoProducto.ListaTiposProductos(out _, out _);
        public List<Proveedor> listaProveedores = DataProveedores.ListaProveedores(out _, out _);

        public IActionResult ConsultaProveedores()
        {
            var listaProveedores = DataProveedores.ListaProveedores(out _, out _);

            // Pasar la lista a la vista
            return View(listaProveedores);
        }


        public IActionResult AgregarLoteProducto()
        {
            ProductosController controlador = new ProductosController();
            return View(controlador);
        }

        /*
        [HttpGet("AgregarNuevoLoteProducto")]
        public IActionResult AgregarNuevoLoteProducto(
            int Tipo,
            int Producto_id,
            int Cantidad,
            decimal Costo,
            string Vencimiento,
            bool EsMaterial,
            decimal MargenGanancia,
            decimal PrecioVenta)
        {
            string mensaje;
            bool respuesta;
            Parametros iva;

            try
            {
                iva = DataIVA.UltimoIVA(out mensaje, out respuesta);

                if (!respuesta)
                {
                    return StatusCode(500, new { message = "Ocurrió un error(145).", error = mensaje });
                }

                if (!DateTime.TryParse(Vencimiento, out DateTime fechaVencimiento))
                {
                    return BadRequest(new { message = "Formato de fecha de vencimiento inválido." });
                }

                LoteProducto nuevo = new LoteProducto()
                {
                    ID_CatalogoProducto = Producto_id,
                    Cantidad = Cantidad,
                    EsMaterial = EsMaterial,
                    Fecha_Ingreso = DateTime.Now,
                    Fecha_Vencimiento = fechaVencimiento,
                    Precio_Compra = Costo,
                    Precio_Venta = PrecioVenta,
                    Margen_Ganancia = MargenGanancia,
                    ID_IVA = iva.ID_IVA,
                };

                if (DataLoteProducto.Agregar(nuevo, out mensaje))
                {
                    return Ok(new { message = "Lote agregado exitosamente." });
                }
                else
                {
                    return StatusCode(500, new { message = "Ocurrió un error(143). ", error = mensaje });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error en el servidor.", error = ex.Message });
            }
        }*/
        /*
        [HttpGet]
        [Route("ObtenerProductosPorTipo")]
        public IActionResult ObtenerProductosPorTipo(int tipo)
        {
            List<CatalogoProducto> productosDelTipo = new List<CatalogoProducto>();

                foreach (CatalogoProducto valor in DataCatalogoProducto.ListaCatalogoProductos(out _, out _))
                {
                    if (valor.ID_TipoProducto == tipo)
                        productosDelTipo.Add(valor);
                }
                return Json(productosDelTipo);
            }
        }*/
    }
}
