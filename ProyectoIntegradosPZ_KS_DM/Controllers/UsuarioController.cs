using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Data.Servicios;
using ProyectoIntegradosPZ_KS_DM.Models;

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



    }
}
