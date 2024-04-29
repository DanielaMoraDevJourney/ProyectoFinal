using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Data.Servicios;
using ProyectoIntegradosPZ_KS_DM.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace ProyectoIntegradosPZ_KS_DM.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly ContextoPZ_KS_DM _contextoPZ_KS_DM;
        private readonly UsuarioServicio _usuarioServicio;

        public UsuarioController(ContextoPZ_KS_DM contextoPZ_KS_DM)
        {
            _contextoPZ_KS_DM = contextoPZ_KS_DM;

            _usuarioServicio = new UsuarioServicio(contextoPZ_KS_DM);

        }

        [Authorize]
        public ActionResult Perfil()
        {
            int userId = 0;
            var userIdClaim = User.FindFirst("UsuarioId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                userId = parsedUserId;

            Usuario usuario = _usuarioServicio.ObtenerUsuarioPorId(userId);
            return View();
        }

        [HttpPost]
        public ActionResult AtualizarPerfil(Usuario model)
            //aca realice un cambio
        {
            using (SqlConnection con = new (_contextoPZ_KS_DM.Conexion))
            {
                using (SqlCommand cmd = new("AtualizarPerfil", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", model.UsuarioId);
                    cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", model.Apellido);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Perfil");
        }

        [HttpPost]
        public ActionResult EliminarCuenta()
        {
            int userId = 0;
            var userIdClaim = User.FindFirst("UsuarioId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            using (SqlConnection con = new(_contextoPZ_KS_DM.Conexion))
            {
                using (SqlCommand cmd = new("EliminarUsuario", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UsuarioId", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
