using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Vitaly_Manager.Models;
using System.IO;
using W3CRM.Controllers;
using static System.Net.Mime.MediaTypeNames;

namespace Vitaly_Manager.Controllers
{
    public class ControladorConfiguracion : Controller
    {
		public static Usuario? usuarioActual;
        public List<Usuario> ListaUsuarios = new();

		public bool usuarioValido(Usuario usuario)
		{
			foreach(Usuario item in ListaUsuarios)
			{
				if(item.Password == usuario.Password && item.Correo == usuario.Correo) {
					usuarioActual = item;
					return true;
				}
			}
			return false;
		}

		public void UnirDatos()
		{
			ListaUsuarios.Clear();
			// Ruta de la carpeta que deseas limpiar
			string carpeta = "wwwroot\\imagenes\\usuarios";

			try
			{
				DirectoryInfo di = new DirectoryInfo(carpeta);

				// Obtener todos los archivos en la carpeta
				foreach (FileInfo archivo in di.GetFiles())
				{
					// Eliminar cada archivo
					archivo.Delete();
				}

				Console.WriteLine("Carpeta limpiada exitosamente.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ocurrió un error al limpiar la carpeta: {ex.Message}");
			}

			using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False"))
			{
				coneccion.Open();

				SqlCommand comando = new SqlCommand("SELECT * FROM Usuarios", coneccion);
				SqlDataReader lector = comando.ExecuteReader();

				while (lector.Read())
				{
					int id = Convert.ToInt32(lector["ID"]);
					string nombres = lector["Nombres"].ToString() ?? "N/A";
					string apellidos = lector["Apellidos"].ToString() ?? "N/A";
					string correo = lector["Correo"].ToString() ?? "N/A";
					string password = lector["Password"].ToString() ?? "N/A";
					string telefono = lector["Telefono"].ToString() ?? "N/A";
					string areaDeTrabajo = lector["AreaDeTrabajo"].ToString() ?? "N/A";
					string generoEsMujer = lector["Genero"].ToString() ?? "N/A";
					bool? activo = null;
					int activoIndex = lector.GetOrdinal("Activo");

					if (!lector.IsDBNull(activoIndex))
					{
						activo = lector.GetBoolean(activoIndex);
					}

					DateTime ingreso = Convert.ToDateTime(lector["Ingreso"]);

					byte[]? imagenBytes = lector["FotoPerfil"] as byte[];

					if (imagenBytes != null)
					{
						IFormFile file = new FormFile(new MemoryStream(imagenBytes), 0, imagenBytes.Length, $"imagen_{id}", $"imagen_{id}.jpg");
						string rutaArchivo = Path.Combine("wwwroot\\imagenes\\usuarios", $"imagen_{id}.jpg");

						using (var fileStream = new FileStream(rutaArchivo, FileMode.Create))
						{
							file.CopyTo(fileStream);
						}
					}

					Usuario nuevo = new Usuario
					{
						Id = id,
						Nombres = nombres,
						Apellidos = apellidos,
						Password = password,
						Correo = correo,
						Telefono = telefono,
						AreaDeTrabajo = areaDeTrabajo,
						Genero = generoEsMujer,
						Ingreso = ingreso,
						FotoPerfil = null,
						Activo = activo
					};

					ListaUsuarios.Add(nuevo);
				}

				lector.Close();
				coneccion.Close();
			}
		}

		public void InsertarUsuario(Usuario usuario)
		{
			string connectionString = "Data Source=DESKTOP-7NSJEG7\\SQLEXPRESS;Initial Catalog=\"Vitaly Data\";Integrated Security=True;Encrypt=False";

			string query = "INSERT INTO Usuarios (Nombres, Apellidos, Password, Correo, Telefono, AreaDeTrabajo, Genero, FotoPerfil, Activo, Ingreso) " +
						   "VALUES (@Nombres, @Apellidos, @Password, @Correo, @Telefono, @AreaDeTrabajo, @Genero, @FotoPerfil, @Activo, @Ingreso)";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Nombres", usuario.Nombres);
					command.Parameters.AddWithValue("@Apellidos", (object)usuario.Apellidos ?? DBNull.Value);
					command.Parameters.AddWithValue("@Password", usuario.Password);
					command.Parameters.AddWithValue("@Correo", usuario.Correo);
					command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
					command.Parameters.AddWithValue("@AreaDeTrabajo", (object)usuario.AreaDeTrabajo ?? DBNull.Value);
					command.Parameters.AddWithValue("@Genero", (object)usuario.Genero ?? DBNull.Value);

					if (usuario.FotoPerfil != null)
					{
						/*
						using (var memoryStream = new MemoryStream())
						{
							usuario.FotoPerfil.CopyTo(memoryStream);
							byte[] fotoBytes = memoryStream.ToArray();

							using (var ms = new MemoryStream(fotoBytes))
							{
								using (var originalImage = System.Drawing.Image.FromStream(ms))
								{
									int size = Math.Min(originalImage.Width, originalImage.Height);

									using (var squareImage = new Bitmap(size, size))
									{
										using (var graphics = Graphics.FromImage(squareImage))
										{
											graphics.FillRectangle(Brushes.White, 0, 0, size, size);
											graphics.DrawImage(
												originalImage,
												new System.Drawing.Rectangle(0, 0, size, size),
												new System.Drawing.Rectangle(
													(originalImage.Width - size) / 2,
													(originalImage.Height - size) / 2,
													size,
													size
												),
												GraphicsUnit.Pixel
											);
										}

										using (var resultStream = new MemoryStream())
										{
											squareImage.Save(resultStream, ImageFormat.Jpeg);
											byte[] resultBytes = resultStream.ToArray();

											command.Parameters.Add("@FotoPerfil", System.Data.SqlDbType.VarBinary, resultBytes.Length).Value = resultBytes;
										}
									}
								}
							}
						}*/
						using (var memoryStream = new MemoryStream())
						{
							usuario.FotoPerfil.CopyTo(memoryStream);
							byte[] fotoBytes = memoryStream.ToArray();
							command.Parameters.Add("@FotoPerfil", System.Data.SqlDbType.VarBinary, fotoBytes.Length).Value = fotoBytes;
						}
					}
					else
					{
						command.Parameters.Add("@FotoPerfil", System.Data.SqlDbType.VarBinary).Value = DBNull.Value;
					}

					command.Parameters.AddWithValue("@Activo", (object)usuario.Activo ?? DBNull.Value);
					command.Parameters.AddWithValue("@Ingreso", DateTime.Now);

					connection.Open();
					command.ExecuteNonQuery();
				}
			}
			UnirDatos();
		}
	}
}

