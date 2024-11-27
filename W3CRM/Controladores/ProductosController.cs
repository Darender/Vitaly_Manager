using Microsoft.AspNetCore.Mvc;
using Vitaly_Manager.Data;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Controladores
{
    public class ProductosController : Controller
    {
        //public List<TipoProducto> ListaTipos = DataTipoProducto.ListaTiposProductos(out _, out _);

        public ActionResult ConsultaProductos()
        {
            
            string respuesta;
            bool exito;

            // Llama al método 'ListaCatalogoProductos' que obtiene los productos del catálogo desde la capa de Data
            // El método retorna una lista de productos (listaCatalogoProductos) 
            var listaCatalogoProductos = DataCatalogoProducto.ListaCatalogoProductos(out respuesta, out exito);

            // Llama al método 'ListaTiposProductos' que obtiene la lista de tipos de productos disponibles
            var listaTipoProductos = DataTipoProducto.ListaTiposProductos(out respuesta, out exito);

            // var listaProveedores = DataProveedores.ListaProveedores(out respuesta, out exito) !!Aun no esta en uso; 

            // Combina las listas: listaCatalogoProductos y listaTipoProducto
            var productos = from cp in listaCatalogoProductos

                         // join p in listaProveedores on cp.ID_Proveedor equals p.ID_Proveedor

                            // Este join asocia los productos del catálogo con los tipos de producto a través de 'ID_TipoProducto'
                            join tp in listaTipoProductos on cp.ID_TipoProducto equals tp.ID_TipoProducto
                            select new
                            {
                                // Se seleccionan las propiedades de cada producto para enviarlas a la vista en View
                                ID_CatalogoProducto = cp.ID_CatalogoProducto,  
                                Nombre_Producto = cp.Nombre_Producto,          
                                Cantidad_Unidades = cp.Cantidad_Unidades,    
                                Pagina_Producto = cp.Pagina_Producto,        
                             // Proveedor = p.Nombre_Proveedor,
                                TipoProducto = tp.Nombre_Tipo_Producto
                            };

            // Se devuelve la vista, pasando la lista combinada (productos) a la vista .
            //Convierte la lista a una lista de objetos (ToList()) para poderla utilizar en la vista de ConsultAProductos.
            return View(productos.ToList());
        }



        public IActionResult AgregarLoteProducto()
        {
            ProductosController controlador = new ProductosController();
            return View(controlador);
        }

        public IActionResult AgregarProveedores()
        {
            ProductosController controlador = new ProductosController();
            return View(controlador);
        }

        /// <summary>
        /// Un metodo encargado de la logica para agregar nuevos proveedores al sistema
        /// </summary>
        /// <param name="nombre">Nombre del nuevo proveedor</param>
        /// <param name="telefono">Numero telefonico del nuevo proveedor</param>
        /// <param name="contactoAlternativo">Cualquier contacto alternativo que tenga el proveedor</param>
        /// <returns>Devuelve un booleado de si fue exitoso, un mensaje de que fue lo que paso y una lista de casillas que fueron fallidas</returns>
        [HttpPost]
        public JsonResult AgregarNuevoProveedores(string nombre, string? telefono, string? contactoAlternativo)
        {
            bool resultado = false;
            string mensaje = "Hubo un problema al agregar al Proveedor.";

            // lista de todas las casillas que terminaron como fallido
            List<string> fallidos = new List<string>();

            try
            {
                // El telefono y el contacto alternativo no pueden ser nulos al mismo tiempo
                if (telefono == null && contactoAlternativo == null)
                {
                    mensaje = "Telefono y contacto alternativo no pueden estar vacios al mismo tiempo";
                    fallidos.Add("telefono");
                    fallidos.Add("alternativo");
                    return Json(new { success = resultado, message = mensaje, errores = fallidos });
                }

                var regex = new System.Text.RegularExpressions.Regex("^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$");

                // Validación del nombre
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    mensaje = "El nombre no puede estar vacío.";
                    fallidos.Add("nombre");
                }
                nombre = nombre.Trim();
                if (!regex.IsMatch(nombre))
                {
                    mensaje = "El nombre solo puede contener letras y espacios.";
                    fallidos.Add("nombre");
                }
                else if (nombre.Length < 3 || nombre.Length > 50)
                {
                    mensaje = "El nombre debe tener entre 3 y 50 caracteres.";
                    fallidos.Add("nombre");
                }

                // Validación del numero telefonico
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    mensaje = "El número de teléfono no puede estar vacío.";
                    fallidos.Add("telefono");
                }
                else if (telefono.Length < 12 || telefono.Length > 20)
                {
                    mensaje = "El telefono debe tener entre 10 y 20 caracteres.";
                    fallidos.Add("telefono");
                }

                if (fallidos.Count == 0)
                {
                    Proveedor nuevo = new Proveedor
                    {
                        Nombre_Proveedor = nombre,
                        Telefono = telefono,
                        Pagina_Contacto = contactoAlternativo
                    };

                    // Envio del nuevo profesor a data para que se envie a la base de datos
                    resultado = DataProveedores.Agregar(nuevo, out mensaje);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                mensaje = $"Error al agregar el Proveedor: {ex.Message}";
            }

            return Json(new { success = resultado, message = mensaje, errores = fallidos });
        }
    }

}


