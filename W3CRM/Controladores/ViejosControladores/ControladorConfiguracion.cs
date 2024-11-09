using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.IO;
using W3CRM.Controllers;
using static System.Net.Mime.MediaTypeNames;
using Vitaly_Manager.Entidades.EntidadesAntiguas;

namespace Vitaly_Manager.Controladores.ViejosControladores
{
    public class ControladorConfiguracion : Controller
    {
        public static Usuario? usuarioActual;
        public List<Usuario> ListaUsuarios = new();

        [HttpPost]
        public IActionResult AgregarUsuario([FromForm] Usuario nuevoUsuario)
        {
            ControladorConfiguracion controlador = new ControladorConfiguracion();
            controlador.InsertarUsuario(nuevoUsuario);
            return View("../W3CRM/Configuracion", controlador);
        }


        [HttpPost]
        public IActionResult InicioSesion([FromForm] Usuario nuevoUsuario)
        {
            ControladorConfiguracion controlador = new ControladorConfiguracion();
            controlador.UnirDatos();

            if (controlador.usuarioValido(nuevoUsuario))
            {
                return RedirectToAction("ProductosGeneral", "Productos");
            }
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View("../W3CRM/Login");
        }

        public bool usuarioValido(Usuario usuario)
        {
            foreach (Usuario item in ListaUsuarios)
            {
                if (item.Password == usuario.Password && item.Correo == usuario.Correo)
                {
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

            using (SqlConnection coneccion = new SqlConnection("Data Source=DESKTOP-E6EPG51;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False"))
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
            string connectionString = "Data Source=DESKTOP-E6EPG51;Initial Catalog=Vitaly Manager;Integrated Security=True;Encrypt=False";

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

