using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace W3CRM.Controllers
{
    public class ControladorClientes : Controller
    {
        public List<Cliente> Listaclientes = new();
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
        public float porcentajeActivosUltimos30Dias()
        {
            if (Listaclientes.Count == 0)
            {
                return 0;
            }
            else
            {
                return (activosUltimos30Dias() / (float)Listaclientes.Count) * 100;
            }
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

            if(Listaclientes.Count - totalUltimos30 <= 0)
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
                string fechaFormateada_ingreso = modificado.Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string query = $"UPDATE Clientes SET Nombres = '{modificado.Nombres}', Apellidos = '{modificado.Apellidos}', Telefono = '{modificado.Telefono}', Edad = {modificado.Edad}, Genero = '{modificado.Genero}', UltimaConsulta = NULL, Ingreso = '{fechaFormateada_ingreso}' WHERE ID = {modificado.ID}";
                SqlCommand comando = new SqlCommand(query, coneccion);
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
                string query = $"DELETE FROM Clientes WHERE ID = {id}";
                SqlCommand comando = new SqlCommand(query, coneccion);
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

                string nombres = nuevo.Nombres;
                string? apellidos = nuevo.Apellidos;
                string telefono = nuevo.Telefono;
                int? edad = nuevo.Edad;
                string? genero = nuevo.Genero;
                string fechaFormateada_ingreso = nuevo.Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff");

                string query = $"INSERT INTO Clientes (Nombres, Apellidos, Telefono, Edad, Genero, UltimaConsulta, Ingreso) VALUES ('{nuevo.Nombres}', '{nuevo.Apellidos}', '{nuevo.Telefono}', '{edad}', '{nuevo.Genero}', '{null}', '{fechaFormateada_ingreso}')";
                SqlCommand command = new SqlCommand(query, coneccion);
                command.ExecuteNonQuery();

                coneccion.Close();
                UnirDatos();
            }
        }
    void UnirDatos()
        {
            Listaclientes.Clear();
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
                coneccion.Close();

                ultimos30dias = porcentajeUltimos30Dias().ToString("0.00");
                activosUltimos30 = activosUltimos30Dias() + "";
                porcentajeActivosUltimos30 = porcentajeActivosUltimos30Dias().ToString("0.00");
            }
        }
    }
}
