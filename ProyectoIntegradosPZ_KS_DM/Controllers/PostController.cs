using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Models;
using System.Data.SqlClient;
using System.Data;
using ProyectoIntegradosPZ_KS_DM.Models.ViewModels;
using System.Linq.Expressions;

namespace ProyectoIntegradosPZ_KS_DM.Controllers
{
    public class PostController : Controller
    {
        private readonly ContextoPZ_KS_DM _contextoPZ_KS_DM;
        private readonly PostServicio _postServicio;

        public PostController(ContextoPZ_KS_DM con)
        {
            _contextoPZ_KS_DM = con;
            _postServicio = new PostServicio(con);

        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Post post)
        {
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("InsertarPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Titulo", post.Titulo);
                    command.Parameters.AddWithValue("@Contenido", post.Contenido);
                    command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                    DateTime fc = DateTime.UtcNow;
                    command.Parameters.AddWithValue("@FechaCreacion", fc);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Update(int id)
        {
            var post = _postServicio.ObtenerPostPorId(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Update(Post post)
        {
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ActualizarPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostID", post.PostId);
                    command.Parameters.AddWithValue("@Titulo", post.Titulo);
                    command.Parameters.AddWithValue("@Contenido", post.Contenido);
                    command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("EliminarPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostID", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int id)
        {
            var post = _postServicio.ObtenerPostPorId(id);
            var comentarios = _postServicio.ObtenerComentariosPorPostId(id);
            comentarios = _postServicio.ObtenerComentariosHijos(comentarios);
            comentarios = _postServicio.ObtenerComentariosNietos(comentarios);

            var model = new PostDetallesViewModel
            {
                Post = post,
                ComentariosPrincipales = comentarios.Where(c => c.ComentarioPadreId == null && c.ComentarioAbueloId == null).ToList(),
                ComentariosHijos = comentarios.Where(c => c.ComentarioPadreId != null && c.ComentarioAbueloId == null).ToList(),
                ComentariosNietos = comentarios.Where(c => c.ComentarioAbueloId != null).ToList(),
                PostRecientes = _postServicio.ObtenerPosts().Take(10).ToList()
            };

            return View(model);
        }


        [HttpPost]
        public ActionResult AgregarComentario(int postId, string comentario, int? comentarioPadreId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comentario))
                {
                    ViewBag.Error = "El comentario no puede estar vacio.";
                    return RedirectToAction("Details", "Post", new { id = postId });
                }

                int? userId = null;
                var userIdClaim = User.FindFirst("IdUsuario");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;

                DateTime fechaPublicacion = DateTime.UtcNow;

                using (SqlConnection con = new(_contextoPZ_KS_DM.Conexion))
                {
                    using (SqlCommand cmd = new("AgregarComentario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Contenido", SqlDbType.VarChar).Value = comentario;
                        cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = fechaPublicacion;
                        cmd.Parameters.Add("@PostId", SqlDbType.Int).Value = postId;
                        cmd.Parameters.Add("@UsuarioId",SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@ComentarioPadreId", SqlDbType.Int).Value = comentarioPadreId ?? (object)DBNull.Value;
                        con.Open();
                        cmd.BeginExecuteNonQuery();
                        con.Close( );
                    }
                }

            return RedirectToAction("Details", "Post", new { id = postId });
        }
        catch (System.Exception e)
        {
             ViewBag.Error = e.Message;
            return RedirectToAction("Details", "Post", new { id = postId });
            }
        }
    }
}


