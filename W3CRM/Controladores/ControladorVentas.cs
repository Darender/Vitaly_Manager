using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades.EntidadesAntiguas;

namespace Vitaly_Manager.Controllers
{
    public class ControladorVentas : Controller
	{
		public List<ClienteAntiguo> ListaClientes = new List<ClienteAntiguo>();
		public List<Tipo_inventario> ListaTipos = new List<Tipo_inventario>();
		public List<Inventario_Producto> ListaProductos = new List<Inventario_Producto>();
		public List<Inventario_Instancia> ListaInstancias = new List<Inventario_Instancia>();

		public ControladorVentas()
		{
			UnirDatos();
		}

		public void agregarVenta(VentaAntiguo nuevo)
		{
			using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
			{
				coneccion.Open();

				nuevo.Fecha_Ingreso = DateTime.Now;

				string query = $"INSERT INTO Venta (Cliente_id, Producto_id, Ingresos, Cantidad, Fecha_Ingreso) VALUES ('{nuevo.Cliente_id}', '{nuevo.Producto_id}', '{nuevo.Ingresos}', '{nuevo.Cantidad}', '{nuevo.Fecha_Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff")}')";
				SqlCommand comando = new SqlCommand(query, coneccion);
				comando.ExecuteNonQuery();

				query = $"UPDATE Clientes SET UltimaConsulta = '{nuevo.Fecha_Ingreso.ToString("yyyy-MM-dd HH:mm:ss.fff")}' WHERE ID = {nuevo.Cliente_id}";
				comando = new SqlCommand(query, coneccion);
				comando.ExecuteNonQuery();



				coneccion.Close();
				UnirDatos();
			}
		}

		public void modificarVenta(VentaAntiguo nuevo)
		{

		}

		[HttpPost]
		public IActionResult AlterarVenta([FromForm] VentaAntiguo nuevo)
		{
			ControladorVentas controlador = new ControladorVentas();
			if (nuevo.ID == 0)
			{
				controlador.agregarVenta(nuevo);
			}
			else
			{
				controlador.modificarVenta(nuevo);
			}
			return View("../W3CRM/Ventas", controlador);
		}

		void UnirDatos()
		{
			ListaClientes.Clear();
			ListaTipos.Clear();
			ListaInstancias.Clear();
			ListaProductos.Clear();

			using (SqlConnection coneccion = new SqlConnection("Data Source=ForjaDelTrabajo;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
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

					ClienteAntiguo cliente = new ClienteAntiguo
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

					ListaClientes.Add(cliente);
				}

				lector.Close();

				comando = new SqlCommand("SELECT * FROM Tipo_inventario", coneccion);
				lector = comando.ExecuteReader();

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
				coneccion.Close();
			}
		}
	}


}

