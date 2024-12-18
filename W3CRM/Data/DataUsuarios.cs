using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Vitaly_Manager.Entidades;

namespace Vitaly_Manager.Data
{
    public static class DataUsuarios
    {
        private static List<Usuario> _cacheUsuarios = new List<Usuario>();
        private static DateTime _ultimoCache = DateTime.MinValue;
        private static readonly TimeSpan TiempoCache = TimeSpan.FromMinutes(1);

        public static List<Usuario> ListaUsuarios(out string respuesta, out bool exito)
        {
            if (_cacheUsuarios.Count > 0 && (DateTime.Now - _ultimoCache) < TiempoCache)
            {
                respuesta = "Datos obtenidos desde el caché.";
                exito = true;
                return _cacheUsuarios;
            }

            List<Usuario> lista = new List<Usuario>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "SELECT * FROM Usuario";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        SqlDataReader lector = comando.ExecuteReader();
                        while (lector.Read())
                        {
                            Usuario usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(lector["idUsuario"]),
                                Nombre = lector["nombre"].ToString(),
                                Apellidos = lector["apellidos"]?.ToString(),
                                CorreoElectronico = lector["correoElectronico"].ToString(),
                                Password = lector["password"].ToString(),
                                Telefono = lector["telefono"]?.ToString(),
                                EstadoActivo = Convert.ToBoolean(lector["estadoActivo"]),
                                EsAdministrador = Convert.ToBoolean(lector["esAdministrador"]),
                            };
                            lista.Add(usuario);
                        }
                    }
                }

                _cacheUsuarios = lista;
                _ultimoCache = DateTime.Now;
                exito = true;
                respuesta = "Consulta exitosa desde la base de datos.";
                return lista;
            }
            catch (Exception ex)
            {
                exito = false;
                respuesta = $"Error al obtener la lista de usuarios: {ex.Message}";
                return new List<Usuario>();
            }
        }

        public static bool Agregar(Usuario nuevo, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"INSERT INTO Usuario (nombre, apellidos, correoElectronico, password, telefono, estadoActivo, esAdministrador)
                                     VALUES (@Nombre, @Apellidos, @Correo, @Password, @Telefono, @EstadoActivo, @EsAdministrador)";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", nuevo.Nombre);
                        comando.Parameters.AddWithValue("@Apellidos", nuevo.Apellidos ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Correo", nuevo.CorreoElectronico);
                        comando.Parameters.AddWithValue("@Password", nuevo.Password);
                        comando.Parameters.AddWithValue("@Telefono", nuevo.Telefono ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EstadoActivo", nuevo.EstadoActivo);
                        comando.Parameters.AddWithValue("@EsAdministrador", nuevo.EsAdministrador);

                        comando.ExecuteNonQuery();
                    }
                }

                _cacheUsuarios.Add(nuevo);
                mensaje = "Usuario agregado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al agregar usuario: {ex.Message}";
                return false;
            }
        }

        public static bool Modificar(Usuario modificado, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = @"UPDATE Usuario
                                     SET nombre = @Nombre, apellidos = @Apellidos, correoElectronico = @Correo, 
                                         password = @Password, telefono = @Telefono, estadoActivo = @EstadoActivo, 
                                         esAdministrador = @EsAdministrador
                                     WHERE idUsuario = @IdUsuario";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", modificado.IdUsuario);
                        comando.Parameters.AddWithValue("@Nombre", modificado.Nombre);
                        comando.Parameters.AddWithValue("@Apellidos", modificado.Apellidos ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@Correo", modificado.CorreoElectronico);
                        comando.Parameters.AddWithValue("@Password", modificado.Password);
                        comando.Parameters.AddWithValue("@Telefono", modificado.Telefono ?? (object)DBNull.Value);
                        comando.Parameters.AddWithValue("@EstadoActivo", modificado.EstadoActivo);
                        comando.Parameters.AddWithValue("@EsAdministrador", modificado.EsAdministrador);

                        comando.ExecuteNonQuery();
                    }
                }

                var usuarioEnCache = _cacheUsuarios.Find(u => u.IdUsuario == modificado.IdUsuario);
                if (usuarioEnCache != null)
                {
                    _cacheUsuarios.Remove(usuarioEnCache);
                    _cacheUsuarios.Add(modificado);
                }

                mensaje = "Usuario modificado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al modificar usuario: {ex.Message}";
                return false;
            }
        }

        public static bool Eliminar(int idUsuario, out string mensaje)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(MainServidor.Servidor))
                {
                    conexion.Open();
                    string query = "DELETE FROM Usuario WHERE idUsuario = @IdUsuario";

                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        comando.ExecuteNonQuery();
                    }
                }

                _cacheUsuarios.RemoveAll(u => u.IdUsuario == idUsuario);
                mensaje = "Usuario eliminado exitosamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al eliminar usuario: {ex.Message}";
                return false;
            }
        }

        public static Usuario? ObtenerPorId(int idUsuario, out string mensaje)
        {
            var usuarios = ListaUsuarios(out mensaje, out _);
            var usuario = usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);

            if (usuario != null)
            {
                mensaje = "Usuario encontrado exitosamente.";
                return usuario;
            }

            mensaje = "Usuario no encontrado.";
            return null;
        }
    }
}
