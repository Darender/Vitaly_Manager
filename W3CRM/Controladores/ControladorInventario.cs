using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Vitaly_Manager.Entidades.EntidadesAntiguas;


namespace Vitaly_Manager.Controllers
{
    public class ControladorInventario : Controller
    {
        public  List<Tipo_inventario> ListaTipos= new List<Tipo_inventario>();
        public  List<Inventario_Producto> ListaProductos = new List<Inventario_Producto>();
        public  List<Inventario_Instancia> ListaInstancias = new List<Inventario_Instancia>();
        public List<Movimiento> ListaMovimientos = new List<Movimiento>();
        public ControladorInventario() {
            unirDatos();
        }

        public IActionResult EliminarMovimiento(int id)
        {
            foreach (Movimiento movimiento in ListaMovimientos)
            {
                if (movimiento.ID == id)
                {
                    ListaMovimientos.Remove(movimiento);
                    break;
                }
            }

            using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                string query = $"DELETE FROM Movimiento WHERE ID = {id}";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();
                coneccion.Close();
            }

            return Json(true);
        }

        public List<Movimiento> ObtenerListaMovimientos()
        {
            return ListaMovimientos
                .OrderByDescending(i => i.Ingreso)
                .Take(15)
                .ToList();
        }

        public List<Producto_Completo> ObtenerInstanciasCercanasALlegar()
        {
            List<Producto_Completo> retorno = new List<Producto_Completo>();

            List<Inventario_Instancia> listaTop6 = ListaInstancias
                .Where(i => i.Llegada.HasValue && i.Llegada.Value.Date >= DateTime.Today)
                .OrderBy(i => i.Llegada)
                .Take(6)
                .ToList();

            Producto_Completo? completo = null;

            foreach (Inventario_Instancia instancia in listaTop6)
            {
                Inventario_Producto producto = getProducto(instancia);
                completo = new Producto_Completo
                {
                    Instancia_id = instancia.ID,
                    Producto_id = producto.ID,
                    Tipo_id = producto.Tipo,
                    Nombre_Producto = producto.Nombre,
                    Nombre_Tipo = getTipoNombre(instancia),
                    Proveedor = producto.Proveedor,
                    Cantidad = instancia.Cantidad,
                    Costo = instancia.Costo,
                    Vencimiento = instancia.Vencimiento,
                    Llegada = instancia.Llegada
                };

                retorno.Add(completo);
            }
            return retorno;
        }

        public List<Producto_Completo> ObtenerInstanciasCercanasAVencimiento()
        {
            List<Producto_Completo> retorno = new List<Producto_Completo>();

            List<Inventario_Instancia> listaTop6 = ListaInstancias
                .Where(i => i.Vencimiento.HasValue && i.Vencimiento != DateTime.Parse("1900-01-01 00:00:00.000"))
                .OrderBy(i => i.Vencimiento)
                .Take(6)
                .ToList();

            Producto_Completo? completo = null;

            foreach (Inventario_Instancia instancia in listaTop6)
            {
                Inventario_Producto producto = getProducto(instancia);
                completo = new Producto_Completo
                {
                    Instancia_id = instancia.ID,
                    Producto_id = producto.ID,
                    Tipo_id = producto.Tipo,
                    Nombre_Producto = producto.Nombre,
                    Nombre_Tipo = getTipoNombre(instancia),
                    Proveedor = producto.Proveedor,
                    Cantidad = instancia.Cantidad,
                    Costo = instancia.Costo,
                    Vencimiento = instancia.Vencimiento,
                    Llegada = instancia.Llegada
                };

                retorno.Add(completo);
            }
            return retorno;
        }

        [HttpGet]
        public ActionResult Actualizar()
        {
            var proximaCapitalInventario = capitalInventario();
            var proximaporcentajeUltimos30DiasCapital = porcentajeUltimos30DiasCapital();
            var proximaVencimientoProducto = ObtenerProximaVencimientoProducto();
            var proximaVencimientoFecha = ObtenerProximaVencimientoFecha();
            var proximaLlegadaProducto = ObtenerProximaLlegadaProducto();
            var proximaLlegadaFecha = ObtenerProximaLlegadaFecha();

            // Devuelve los datos en formato JSON
            return Json(new
            {
                proximaCapitalInventario,
                proximaporcentajeUltimos30DiasCapital,
                proximaVencimientoProducto,
                proximaVencimientoFecha,
                proximaLlegadaProducto,
                proximaLlegadaFecha
            });
        }

        public string ObtenerProximaVencimientoProducto()
        {
            DateTime ahora = DateTime.Now;
            Inventario_Instancia? obtenido = ListaInstancias
                .OrderBy(instancia => instancia.Vencimiento)
                .FirstOrDefault();

            if (obtenido != null)
            {
                return $"{getProducto(obtenido).Nombre} - {getProducto(obtenido).Proveedor}";
            }
            else
            {
                return "Ninguno";
            }
        }

        public string? ObtenerProximaVencimientoFecha()
        {
            DateTime ahora = DateTime.Now;
            Inventario_Instancia? obtenido = ListaInstancias
                .OrderBy(instancia => instancia.Vencimiento)
                .FirstOrDefault();

            if (obtenido != null)
            {
                return obtenido.Vencimiento.HasValue ? obtenido.Vencimiento.Value.ToString("yyyy-MM-dd") : null;
            }
            else
            {
                return "";
            }
        }

        public string ObtenerProximaLlegadaProducto()
        {
            DateTime ahora = DateTime.Now;
            Inventario_Instancia? obtenido = ListaInstancias
                .Where(instancia => instancia.Llegada > ahora)
                .OrderBy(instancia => instancia.Llegada)
                .FirstOrDefault();

            if (obtenido != null)
            {
                return $"{getProducto(obtenido).Nombre} - {getProducto(obtenido).Proveedor}";
            }
            else
            {
                return "Ninguno";
            }
        }

        public string? ObtenerProximaLlegadaFecha()
        {
            DateTime ahora = DateTime.Now;
            Inventario_Instancia? obtenido = ListaInstancias
                .Where(instancia => instancia.Llegada > ahora)
                .OrderBy(instancia => instancia.Llegada)
                .FirstOrDefault();

            if(obtenido != null)
            {
                return obtenido.Llegada.HasValue ? obtenido.Llegada.Value.ToString("yyyy-MM-dd") : null;
            } else
            {
                return "";
            }
        }

        public float porcentajeUltimos30DiasCapital()
        {
            DateTime hace30Dias = DateTime.Today.AddDays(-30);
            float totalUltimos30 = 0;

            foreach (Inventario_Instancia valor in ListaInstancias)
            {
                if (valor.Ingreso > hace30Dias)
                {
                    totalUltimos30 += valor.Cantidad * valor.Costo;
                }
            }

            if (totalUltimos30 == 0)
                return 0;

            return (capitalInventario() / totalUltimos30) * 100;
        }

        public float capitalInventario()
        {
            float total = 0;
            foreach(Inventario_Instancia valor in ListaInstancias)
            {
                total += valor.Cantidad * valor.Costo;
            }
            return total;
        }

        [HttpGet]
        public IActionResult ObtenerInstancia(int id)
        {
            Producto_Completo? completo = null;

            foreach (Inventario_Instancia instancia in ListaInstancias)
            {
                if (instancia.ID == id)
                {
                    Inventario_Producto producto = getProducto(instancia);
                    completo = new Producto_Completo
                    {
                        Instancia_id = instancia.ID,
                        Producto_id = producto.ID, 
                        Tipo_id = producto.Tipo,
                        Nombre_Producto = producto.Nombre,
                        Nombre_Tipo = getTipoNombre(instancia),
                        Proveedor = producto.Proveedor,
                        Cantidad = instancia.Cantidad,
                        Costo = instancia.Costo,
                        Vencimiento = instancia.Vencimiento,
                        Llegada = instancia.Llegada
                    };
                    break;
                }
            }

            if (completo != null)
            {
                return Json(completo);
            }
            else
            {
                return NotFound();
            }
        }

        public void modificarInstancia(Inventario_Instancia modificado)
        {
            using (SqlConnection conexion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                conexion.Open();

                Inventario_Instancia? antiguo = ListaInstancias.FirstOrDefault(c => c.ID == modificado.ID);
                #pragma warning disable CS8602
                string titulo = $"La instancia de {getProducto(modificado).Nombre} - {getProducto(modificado).Proveedor} ah sido modificado";
                string descripcion = $"La instancia con id {modificado.ID} fue modificado por {ControladorConfiguracion.usuarioActual.Nombres}, la comparacion entre antes y despues es el siguiente: " +
                    $". Producto_id: {antiguo.Producto_id} -> {modificado.Producto_id}" +
                    $". Cantidad: {antiguo.Cantidad} -> {modificado.Cantidad}" +
                    $". Costo: {antiguo.Costo} -> {modificado.Costo}" +
                    $". Vencimiento: {antiguo.Vencimiento?.ToString("yyyy-MM-dd") ?? "No especificado"} -> {modificado.Vencimiento?.ToString("yyyy-MM-dd") ?? "No especificado"}" +
                    $". Llegada: {antiguo.Llegada?.ToString("yyyy-MM-dd") ?? "No especificado"} -> {modificado.Llegada?.ToString("yyyy-MM-dd") ?? "No especificado"}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{3}', '{modificado.ID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, conexion);
                comando.ExecuteNonQuery();


                query = @"UPDATE Inventario_Instancia
                         SET Producto_id = @Producto_id,
                             Cantidad = @Cantidad,
                             Costo = @Costo,
                             Vencimiento = @Vencimiento,
                             Llegada = @Llegada
                         WHERE ID = @ID";

                using (comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Producto_id", modificado.Producto_id);
                    comando.Parameters.AddWithValue("@Cantidad", modificado.Cantidad);
                    comando.Parameters.AddWithValue("@Costo", modificado.Costo);

                    if (modificado.Vencimiento.HasValue)
                        comando.Parameters.AddWithValue("@Vencimiento", modificado.Vencimiento.Value);
                    else
                        comando.Parameters.AddWithValue("@Vencimiento", DBNull.Value);

                    if (modificado.Llegada.HasValue)
                        comando.Parameters.AddWithValue("@Llegada", modificado.Llegada.Value);
                    else
                        comando.Parameters.AddWithValue("@Llegada", DBNull.Value);

                    comando.Parameters.AddWithValue("@ID", modificado.ID);

                    comando.ExecuteNonQuery();
                }

                conexion.Close();
            }

            unirDatos();
        }


        [HttpDelete]
        public IActionResult Eliminar(int id)
        {
            using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                Inventario_Instancia? antiguo = ListaInstancias.FirstOrDefault(c => c.ID == id);
                Inventario_Producto? producto = getProducto(antiguo);
                #pragma warning disable CS8602
                string titulo = $"La instancia {producto.Nombre} - {producto.Proveedor} ah sido borrado";
                string descripcion = $"La instancia con id {id} fue eliminado por {ControladorConfiguracion.usuarioActual.Nombres}, la informacion de la instancia eliminada es la siguiente: " +
                    $". Nombre: {producto.Nombre}" +
                    $". Proveedor: {producto.Proveedor}" +
                    $". Cantidad: {antiguo.Cantidad} " +
                    $". Costo: {antiguo.Costo}" +
                    $". Vencimiento: {antiguo.Vencimiento?.ToString("yyyy-MM-dd") ?? "No especificado"}" +
                    $". Llegada: {antiguo.Llegada?.ToString("yyyy-MM-dd") ?? "No especificado"}" +
                    $". Ingreso: {antiguo.Ingreso.ToString("yyyy-MM-dd")}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{3}', '{id}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                query = $"DELETE FROM Inventario_Instancia WHERE ID = {id}";
                comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();
                coneccion.Close();
            }

            unirDatos();
            return Json(new {});
        }

        public int getTipoID(Inventario_Instancia instancia)
        {
            Inventario_Producto? producto = getProducto(instancia);

            if (producto == null)
                return 0;

            foreach (Tipo_inventario item in ListaTipos)
            {
                if (item.ID == producto.Tipo)
                {
                    return item.ID;
                }
            }

            return 0;
        }

        public string getTipoNombre(Inventario_Instancia instancia)
        {
            Inventario_Producto? producto = getProducto(instancia);

            if (producto == null)
                return "N/A";

            foreach (Tipo_inventario item in ListaTipos)
            {
                if (item.ID == producto.Tipo)
                {
                    return item.Nombre;
                }
            }

            return "N/A";
        }

        public Inventario_Producto? getProducto(Inventario_Instancia instancia)
        {
            foreach (Inventario_Producto item in ListaProductos)
            {
                if (item.ID == instancia.Producto_id)
                    return item;
            }

            return null;
        }

        [HttpPost]
        public IActionResult AlterarInstancias([FromForm] Inventario_Instancia nuevaInstancia)
        {
            ControladorInventario controlador = new ControladorInventario();
            if (nuevaInstancia.ID == 0)
            {
                controlador.agregarInstancia(nuevaInstancia);
            } else
            {
                controlador.modificarInstancia(nuevaInstancia);
            }
            return View("../W3CRM/Inventario",controlador);
        }

        [HttpPost]
        public IActionResult AgregarProducto([FromForm] Inventario_Producto nuevoProducto)
        {
            ControladorInventario controlador = new ControladorInventario();
            controlador.agregarProducto(nuevoProducto);
            return View("../W3CRM/Inventario", controlador);
        }

        public void agregarInstancia(Inventario_Instancia objeto)
        {
            using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();
                DateTime? vencimiento = objeto.Vencimiento;
                string? vencimientoString = vencimiento.HasValue ? vencimiento.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;

                DateTime? llegada = objeto.Llegada;
                string? llegadaString = llegada.HasValue ? llegada.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;

                objeto.Ingreso = DateTime.Today;
                Inventario_Producto producto = getProducto(objeto);

                string titulo = $"La instancia {producto.Nombre} - {producto.Proveedor} ah sido agregada";
                string descripcion = $"La instancia con id {objeto.ID} fue agregado por {ControladorConfiguracion.usuarioActual.Nombres}, la informacion de la instancia agregada es la siguiente: " +
                    $". Nombre: {producto.Nombre}" +
                    $". Proveedor: {producto.Proveedor}" +
                    $". Cantidad: {objeto.Cantidad} " +
                    $". Costo: {objeto.Costo}" +
                    $". Vencimiento: {objeto.Vencimiento?.ToString("yyyy-MM-dd") ?? "No especificado"}" +
                    $". Llegada: {objeto.Llegada?.ToString("yyyy-MM-dd") ?? "No especificado"}" +
                    $". Ingreso: {objeto.Ingreso.ToString("yyyy-MM-dd")}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{3}', '{objeto.ID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                query = $"INSERT INTO Inventario_Instancia (Producto_id, Cantidad, Costo, Vencimiento, Llegada, Ingreso) VALUES ('{objeto.Producto_id}', '{objeto.Cantidad}', '{objeto.Costo}', '{vencimientoString}', '{llegadaString}', '{objeto.Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                coneccion.Close();
                unirDatos();
            }
        }

        [HttpGet]
        public IActionResult ObtenerProductosPorTipo(int tipo)
        {
            List<Inventario_Producto> productosDelTipo = new List<Inventario_Producto>();

            foreach (Inventario_Producto valor in ListaProductos)
            {
                if(valor.Tipo == tipo)
                    productosDelTipo.Add(valor);
            }

            // Retorna los productos en formato JSON
            return Json(productosDelTipo);
        }



        public float NuevosProductos_porcentaje()
        {
            DateTime hace30Dias = DateTime.Today.AddDays(-30);
            int totalUltimos30 = 0;

            foreach (Inventario_Producto valor in ListaProductos)
            {
                if (valor.Ingreso > hace30Dias)
                {
                    totalUltimos30++;
                }
            }

            if (ListaProductos.Count == 0)
            {
                return 0;
            }
            else
            {
                return (totalUltimos30 / (float)ListaProductos.Count) * 100;
            }
        }

        public void agregarProducto(Inventario_Producto nuevo)
        {
            using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                string tipoNombre = "";
                foreach(Tipo_inventario tipo in ListaTipos)
                {
                    if(tipo.ID == nuevo.Tipo)
                    {
                        tipoNombre = tipo.Nombre;
                    }
                }

                string titulo = $"El producto {nuevo.Nombre} - {nuevo.Proveedor} ah sido agregado";
                string descripcion = $"El producto con id {nuevo.ID} fue agregado por {ControladorConfiguracion.usuarioActual.Nombres}, la informacion del prodcuto agregado es la siguiente: " +
                    $". Nombre: {nuevo.Nombre}" +
                    $". Proveedor: {nuevo.Proveedor}" +
                    $". Tipo: {tipoNombre} ";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{4}', '{nuevo.ID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                query = $"INSERT INTO Inventario_Producto (Tipo, Nombre, Proveedor, Ingreso) VALUES ('{nuevo.Tipo}', '{nuevo.Nombre}', '{nuevo.Proveedor}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                coneccion.Close();
                unirDatos();
            }
        }

        public void unirDatos()
        {
            ListaTipos.Clear();
            ListaInstancias.Clear();
            ListaProductos.Clear();
            ListaMovimientos.Clear();
            using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                SqlCommand comando = new SqlCommand("SELECT * FROM Tipo_inventario", coneccion);
                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int id = Convert.ToInt32(lector["ID"]);
                    string nombre = lector["Nombre"].ToString() ?? "N/A";

                    Tipo_inventario cliente = new Tipo_inventario
                    {
                        ID = id,
                        Nombre = nombre
                    };

                    ListaTipos.Add(cliente);
                }

                lector.Close();

                comando = new SqlCommand("SELECT * FROM Inventario_Producto", coneccion);
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int id = Convert.ToInt32(lector["ID"]);
                    int tipo = Convert.ToInt32(lector["Tipo"]);
                    string nombre = lector["Nombre"].ToString() ?? "N/A";
                    string proveedor = lector["Proveedor"].ToString() ?? "N/A";
                    DateTime ingreso = Convert.ToDateTime(lector["Ingreso"]);

                    Inventario_Producto producto = new Inventario_Producto
                    {
                        ID = id,
                        Tipo = tipo,
                        Nombre = nombre,
                        Proveedor = proveedor,
                        Ingreso = ingreso
                    };

                    ListaProductos.Add(producto);
                }

                lector.Close();

                comando = new SqlCommand("SELECT * FROM Inventario_Instancia", coneccion);
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int id = Convert.ToInt32(lector["ID"]);
                    int producto_id = Convert.ToInt32(lector["Producto_id"]);
                    float cantidad = Convert.ToSingle(lector["Cantidad"]);
                    float costo = Convert.ToSingle(lector["Costo"]);
                    DateTime? vencimiento = Convert.ToDateTime(lector["Vencimiento"]);
                    DateTime? llegada = Convert.ToDateTime(lector["Llegada"]);
                    DateTime ingreso = Convert.ToDateTime(lector["Ingreso"]);

                    Inventario_Instancia instancia = new Inventario_Instancia
                    {
                        ID = id,
                        Producto_id = producto_id,
                        Cantidad = cantidad,
                        Costo = costo,
                        Vencimiento = vencimiento,
                        Llegada = llegada,
                        Ingreso = ingreso
                    };

                    ListaInstancias.Add(instancia);
                }

                lector.Close();

                comando = new SqlCommand("SELECT * FROM Movimiento", coneccion);
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int tipo_movimiento_id = Convert.ToInt32(lector["Tipo_movimiento_id"]);

                    if (tipo_movimiento_id != 4 && tipo_movimiento_id != 3)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(lector["ID"]);
                    string titulo = lector["Titulo"].ToString() ?? "N/A";
                    string descripcion = lector["Descripcion"].ToString() ?? "N/A";
                    int usuario_id = Convert.ToInt32(lector["Usuario_id"]);
                    int entidad_id = Convert.ToInt32(lector["Entidad_id"]);
                    DateTime ingreso = Convert.ToDateTime(lector["Ingreso"]);

                    Movimiento nuevo = new Movimiento
                    {
                        Descripcion = descripcion,
                        Titulo = titulo,
                        ID = id,
                        Tipo_movimiento_id = tipo_movimiento_id,
                        Usuario_id = usuario_id,
                        Entidad_id = entidad_id,
                        Ingreso = ingreso
                    };

                    ListaMovimientos.Add(nuevo);
                }

                lector.Close();
                coneccion.Close();
            }

        }
    }
}
