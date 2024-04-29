using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradosPZ_KS_DM.Data.Servicios;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Models;
using System.Diagnostics;
using ProyectoIntegradosPZ_KS_DM.Data.Enums;
using X.PagedList;

namespace ProyectoIntegradosPZ_KS_DM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoPZ_KS_DM _contextoPZ_KS_DM;

        private readonly PostServicio _postServicio;

        public HomeController(ContextoPZ_KS_DM contextoPZ_KS_DM)
        {
            _contextoPZ_KS_DM = contextoPZ_KS_DM;
            _postServicio = new PostServicio(contextoPZ_KS_DM);

        }




        public ActionResult Index(string categoria, string buscar, int? pagina)
        {
            var post = new List<Post>();
            if (string.IsNullOrEmpty(categoria) && string.IsNullOrEmpty(buscar))
            {
                post = _postServicio.ObtenerPosts();
            }
            else if (!string.IsNullOrEmpty(categoria))
            {
                var categoriaEnum = Enum.Parse<CategoriaEnum>(categoria);
                post = _postServicio.ObtenerPostsPorCategoria(categoriaEnum);
                if (post.Count == 0)
                {
                    ViewBag.Error = $"No se encontraron publicaciones relacionadas con: {categoriaEnum}.";
                }
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                post = _postServicio.ObtenerPostsPorTitulo(buscar);
                if (post.Count == 0)
                {
                    ViewBag.Error = $"No se encontraron publicaciones en la categoría {buscar}.";
                }
            }

            int pageSize = 6;
            int pageNumber = (pagina ?? 1);

            string descripcionCategoria = !string.IsNullOrEmpty(categoria) ? CategoriaEnumHelper.ObtenerDescripcion(Enum.Parse<CategoriaEnum>(categoria)):"Comunidad UDLA";
            ViewBag.CategoriaDescripcion = descripcionCategoria;

            return View(post.ToPagedList(pageNumber, pageSize));
        }

    }



    




}
