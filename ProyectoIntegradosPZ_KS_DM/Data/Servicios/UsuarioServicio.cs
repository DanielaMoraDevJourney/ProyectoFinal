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

                    //INSERTAR ENVÍO DE CORREO
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




    }
}
