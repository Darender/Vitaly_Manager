using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Controllers;
using Vitaly_Manager.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace W3CRM.Controllers
{
    public class ControladorClientes : Controller
    {
        public List<Cliente> Listaclientes = new();
        public List<Movimiento> ListaMovimiento = new();
        public string ultimos30dias;
        public string activosUltimos30;
        public string porcentajeActivosUltimos30;

        public ControladorClientes()
        {
            UnirDatos();
            ultimos30dias = porcentajeUltimos30Dias().ToString("0.00");
            activosUltimos30 = activosUltimos30Dias() + "";
            porcentajeActivosUltimos30 = porcentajeActivosUltimos30Dias().ToString("0.00");
        }

        [HttpDelete]
        public IActionResult EliminarMovimiento(int id)
        {
            foreach (Movimiento movimiento in ListaMovimiento)
            {
                if(movimiento.ID == id)
                {
                    ListaMovimiento.Remove(movimiento);
                    break;
                }
            }

            using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
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
            return ListaMovimiento
                .OrderByDescending(i => i.Ingreso)
                .Take(15)
                .ToList();
        }

        public float porcentajeActivosUltimos30Dias()
        {
            if (Listaclientes == null || Listaclientes.Count == 0)
            {
                return 0;
            }

            float activos = activosUltimos30Dias();
            if (activos < 0 || activos > Listaclientes.Count)
            {
                // Si activosUltimos30Dias devuelve un número inválido, tomamos una decisión aquí
                return 0; // o puedes lanzar una excepción si lo prefieres
            }

            return (activos / (float)Listaclientes.Count) * 100;
        }

        public int activosUltimos30Dias()
        {
            DateTime hace30Dias = DateTime.Today.AddDays(-30);
            int totalUltimos30 = 0;

            foreach (Cliente valor in Listaclientes)
            {
                if (valor.Ultima_Conusulta > hace30Dias && valor.Ultima_Conusulta != null)
                {
                    totalUltimos30++;
                }
            }

            return totalUltimos30;
        }

        public float porcentajeUltimos30Dias()
        {
            // Obtén la fecha de hace 30 días
            DateTime hace30Dias = DateTime.Today.AddDays(-30);
            float totalUltimos30 = 0;

            foreach(Cliente valor in Listaclientes)
            {
                if(valor.Ingreso > hace30Dias)
                {
                    totalUltimos30++;
                }
            }

            if (Listaclientes.Count == 0)
            {
                return 0;
            }

            if (Listaclientes.Count - totalUltimos30 <= 0)
            {
                return 100 * totalUltimos30;
            }

            return (totalUltimos30 * 100) / (Listaclientes.Count - totalUltimos30);
        }

        public void modificarCliente(Cliente modificado)
        {
            using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();
                Cliente? antiguo = Listaclientes.FirstOrDefault(c => c.ID == modificado.ID);
                #pragma warning disable CS8602 
                string titulo = $"El cliente {antiguo.Nombres} ah sido modificado";
                string descripcion = $"El cliente con id {modificado.ID} fue modificado por {ControladorConfiguracion.usuarioActual.Nombres}, la comparacion entre antes y despues es el siguiente: " +
                    $". Nombre(s): {antiguo.Nombres} -> {modificado.Nombres}" +
                    $". Apellidos: {antiguo.Apellidos} -> {modificado.Apellidos}" +
                    $". Telefono: {antiguo.Telefono} -> {modificado.Telefono}" +
                    $". Edad: {antiguo.Edad} -> {modificado.Edad}" +
                    $". Genero: {antiguo.Genero} -> {modificado.Genero}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{1}', '{modificado.ID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                string fechaFormateada_ingreso = modificado.Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff");
                query = $"UPDATE Clientes SET Nombres = '{modificado.Nombres}', Apellidos = '{modificado.Apellidos}', Telefono = '{modificado.Telefono}', Edad = {modificado.Edad}, Genero = '{modificado.Genero}', UltimaConsulta = NULL, Ingreso = '{fechaFormateada_ingreso}' WHERE ID = {modificado.ID}";
                comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();
                coneccion.Close();
            }

            UnirDatos();
        }

        [HttpGet]
        public IActionResult ObtenerCliente(int id)
        {
            UnirDatos();
            Cliente? cliente = null;

            foreach(Cliente valor in Listaclientes)
            {
                if (valor.ID == id)
                {
                    cliente = valor;
                    break;
                }
            }

            if (cliente != null)
            {
                return Json(cliente);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete]
		public IActionResult Eliminar(int id)
		{
			using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                Cliente? antiguo = Listaclientes.FirstOrDefault(c => c.ID == id);
                #pragma warning disable CS8602
                string titulo = $"El cliente {antiguo.Nombres} ah sido borrado";
                string descripcion = $"El usuario con id {id} fue eliminado por {ControladorConfiguracion.usuarioActual.Nombres}, la informacion del cliente eliminado es la siguiente: " +
                    $". Nombre(s): {antiguo.Nombres}" +
                    $". Apellidos: {antiguo.Apellidos}" +
                    $". Telefono: {antiguo.Telefono} " +
                    $". Edad: {antiguo.Edad}" +
                    $". Genero: {antiguo.Genero}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{1}', '{id}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                query = $"DELETE FROM Clientes WHERE ID = {id}";
                comando = new SqlCommand(query, coneccion);
				comando.ExecuteNonQuery();
                coneccion.Close();
			}
            UnirDatos();
            return Json(new { porcentaje = ultimos30dias, cantidad = Listaclientes.Count }); 
		}

		public void AgregarDatos(Cliente nuevo)
        {
            using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                string fechaFormateada_ingreso = nuevo.Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff");

                string titulo = $"El cliente {nuevo.Nombres} ah sido agregado";
                string descripcion = $"El cliente con id {nuevo.ID} fue agregado por {ControladorConfiguracion.usuarioActual.Nombres}, la informacion del cliente eliminado es la siguiente: " +
                    $". Nombre(s): {nuevo.Nombres}" +
                    $". Apellidos: {nuevo.Apellidos}" +
                    $". Telefono: {nuevo.Telefono} " +
                    $". Edad: {nuevo.Edad}" +
                    $". Genero: {nuevo.Genero}";


                string query = $"INSERT INTO Movimiento (Titulo, Descripcion, Usuario_id, Tipo_movimiento_id, Entidad_id, Ingreso) VALUES ('{titulo}', '{descripcion}', '{ControladorConfiguracion.usuarioActual.Id}', '{1}', '{nuevo.ID}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
                SqlCommand comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();


                query = $"INSERT INTO Clientes (Nombres, Apellidos, Telefono, Edad, Genero, UltimaConsulta, Ingreso) VALUES ('{nuevo.Nombres}', '{nuevo.Apellidos}', '{nuevo.Telefono}', '{nuevo.Edad}', '{nuevo.Genero}', '{null}', '{fechaFormateada_ingreso}')";
                comando = new SqlCommand(query, coneccion);
                comando.ExecuteNonQuery();

                coneccion.Close();
                UnirDatos();
            }
        }
    void UnirDatos()
        {
            Listaclientes.Clear();
            ListaMovimiento.Clear();
            using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
            {
                coneccion.Open();

                SqlCommand comando = new SqlCommand("SELECT * FROM Clientes", coneccion);
                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int id = Convert.ToInt32(lector["ID"]);
                    string nombres = lector["Nombres"].ToString() ?? "N/A";
                    string apellidos = lector["Apellidos"].ToString() ?? "N/A";
                    string telefono = lector["Telefono"].ToString() ?? "N/A";
                    int edad = Convert.ToInt32(lector["Edad"]);
                    string generoEsMujer = lector["Genero"].ToString() ?? "N/A";
                    string? string_ultimaConsulta = lector["UltimaConsulta"].ToString();
                    DateTime? ultimaConsulta;
                    try
                    {
                        ultimaConsulta = Convert.ToDateTime(string_ultimaConsulta);
                    }
                    catch (FormatException)
                    {
                        ultimaConsulta = null;
                    }

                    DateTime ingreso = Convert.ToDateTime(lector["Ingreso"]);

                    Cliente cliente = new Cliente
                    {
                        ID = id,
                        Nombres = nombres,
                        Apellidos = apellidos,
                        Telefono = telefono,
                        Edad = edad,
                        Genero = generoEsMujer,
                        Ultima_Conusulta = ultimaConsulta,
                        Ingreso = ingreso
                    };

                    Listaclientes.Add(cliente);
                }
                lector.Close();

                comando = new SqlCommand("SELECT * FROM Movimiento", coneccion);
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int tipo_movimiento_id = Convert.ToInt32(lector["Tipo_movimiento_id"]);

                    if(tipo_movimiento_id != 1)
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

                    ListaMovimiento.Add(nuevo);
                }

                lector.Close();
                coneccion.Close();

                ultimos30dias = porcentajeUltimos30Dias().ToString("0.00");
                activosUltimos30 = activosUltimos30Dias() + "";
                porcentajeActivosUltimos30 = porcentajeActivosUltimos30Dias().ToString("0.00");
            }
        }
    }
}
