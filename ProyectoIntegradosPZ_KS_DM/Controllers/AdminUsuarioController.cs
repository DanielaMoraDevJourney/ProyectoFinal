using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Data.Servicios;
using ProyectoIntegradosPZ_KS_DM.Models;
using System.Data;
using System.Data.SqlClient;
using X.PagedList;

namespace ProyectoIntegradosPZ_KS_DM.Controllers
{
    public class AdminUsuarioController : Controller
    {
        private readonly ContextoPZ_KS_DM _contexto;
        private readonly UsuarioServicio _usuarioServicio;

        public AdminUsuarioController(ContextoPZ_KS_DM contexto)
        {
            _contexto = contexto;
            _usuarioServicio = new UsuarioServicio(contexto);

        }
        public IActionResult Index(string buscar, int? pagina)
        {
            var usuarios = _usuarioServicio.ListarUsuarios();
            if (!String.IsNullOrEmpty(buscar))
                usuarios = usuarios.Where(u => u.Correo != null && u.Correo.Contains(buscar) || u.NombreUsuario != null && u.NombreUsuario.Contains(buscar)).ToList();
            usuarios = usuarios.OrderBy(u => u.NombreUsuario).ToList();

            List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
            {
                Value = r.RolId.ToString(),
                Text = r.Nombre
            }).ToList();
            ViewBag.Roles = roles;
            int pageSize = 10;
            int pageNumber = (pagina ?? 1);
            var usuariospaginados = usuarios.ToPagedList(pageNumber, pageSize);
            return View(usuariospaginados);


        }


        public IActionResult Create()
        {
            List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
            {
                Value = r.RolId.ToString(),
                Text = r.Nombre
            }).ToList();
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost]


        public IActionResult Create(Usuario usuario)
        {
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("RegistrarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasenia);
                        cmd.Parameters.AddWithValue("@Contrasenia", hashedPassword);
                        cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                        cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
                        cmd.Parameters.AddWithValue("@Token", usuario.Token);
                        cmd.Parameters.AddWithValue("@FechaExpiracion", usuario.FechaExpiracion);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                }
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }


        public IActionResult Edit(int id)
        {
            var usuario = _usuarioServicio.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return NotFound();
            List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
            {
                Value = r.RolId.ToString(),
                Text = r.Nombre
            }).ToList();
            ViewBag.Roles = roles;
            return View(usuario);

        }
        [HttpPost]


        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioId)
                return NotFound();
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    
                    using (SqlCommand cmd = new("ActualizarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UsuarioId", usuario.UsuarioId);
                        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);

                        con.Open();
                        cmd.ExecuteNonQuery();

                    }                   
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }


        }



        public IActionResult Delete(int id)
        {
            var usuario = _usuarioServicio.ObtenerUsuarioPorId(id);
            if (usuario == null)
                return NotFound();
            return View(usuario);
        }
        [HttpPost, ActionName("Delete")]



        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("EliminarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuarioId", id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View(_usuarioServicio.ObtenerUsuarioPorId(id));
            }

        }


    }

    


}
