using ProyectoIntegradosPZ_KS_DM.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoIntegradosPZ_KS_DM.Data.Servicios
{
    public class UsuarioServicio
    {
        private readonly ContextoPZ_KS_DM _contextoPZ_KS_DM;
        public UsuarioServicio(ContextoPZ_KS_DM con)
        {
            _contextoPZ_KS_DM = con;
        }

        public void ActualizarToken(string correo)
        {
            using (SqlConnection con = new(_contextoPZ_KS_DM.Conexion))
               
            {
                using (SqlCommand cmd = new("ActualizarToken", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    DateTime fecha = DateTime.UtcNow.AddMinutes(5);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    var token = Guid.NewGuid();
                    cmd.Parameters.AddWithValue("@Token", token.ToString());
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();


                    Email email = new();
                    if (correo != null)
                        email.Enviar(correo, token.ToString());
                }
                
            }
            
        }

        public List<Rol> ListarRoles()
        {
            var model = new List<Rol>();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (SqlCommand cmd = new("ListarRoles", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rol = new Rol();
                            rol.RolId = Convert.ToInt32(reader["RolId"]);
                            rol.Nombre = Convert.ToString(reader["Nombre"]);
                            model.Add(rol);
                        }
                    }
                }
            }

            return model;
        }


        public List<Usuario> ListarUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (SqlConnection con = new(_contextoPZ_KS_DM.Conexion))
            {
                using (SqlCommand cmd = new("ListarUsuarios", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; 
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader(); 
                    while (rdr.Read())
                    {
                        var usuario = new Usuario
                        {
                            UsuarioId = (int)rdr["UsuarioId"],
                            Nombre = rdr["Nombre"].ToString(),
                            Apellido= rdr["Apellido"].ToString(),
                            Correo = rdr["Correo"].ToString(),
                            Contrasenia = rdr["Contrasenia"].ToString(),
                            RolId = (int)rdr["RolId"],
                            NombreUsuario = rdr["NombreUsuario"].ToString(),
                            Estado = Convert.ToBoolean(rdr["Estado"]),
                            Token = rdr["Token"].ToString(),
                            FechaExpiracion = Convert.ToDateTime(rdr["FechaExpiracion"])
                        };
                        

                    usuarios.Add(usuario);
                    }
                }
            }
            return usuarios;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            Usuario usuario = new();
            using (SqlConnection con = new(_contextoPZ_KS_DM.Conexion))
            {
                using (SqlCommand cmd = new("ObtenerUsuarioPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuarioId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        usuario = new Usuario
                        {
                            UsuarioId = id,
                            Nombre = rdr["Nombre"].ToString(),
                            Apellido = rdr["Apellido"].ToString(),
                            Correo = rdr["Correo"].ToString(),
                            Contrasenia = rdr["Contraseña"].ToString(),
                            RolId = (int)rdr["RolId"],
                            NombreUsuario = rdr["NombreUsuario"].ToString(),
                            Estado = Convert.ToBoolean(rdr["Estado"]),
                            Token = rdr["Token"].ToString(),
                            FechaExpiracion = Convert.ToDateTime(rdr["FechaExpiracion"])
                        };
                    }
                }
            }
            return usuario;
        }


    }
}
