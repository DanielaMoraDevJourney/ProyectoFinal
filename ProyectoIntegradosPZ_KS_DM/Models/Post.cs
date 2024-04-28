using ProyectoIntegradosPZ_KS_DM.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradosPZ_KS_DM.Models
{
    public class Post
    {
        public int PostId { get; set; }
        [Required(ErrorMessage = "El titulo es requerido")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El titulo debe tener entre 5 y más caracteres")]

        public string? Titulo { get; set; }
        [Required(ErrorMessage = "El contenido es requerido")]
        [StringLength(100, MinimumLength = 20, ErrorMessage = "El  debe tener entre 5 o más caracteres")]

        public string? Contenido { get; set; }
        [Required(ErrorMessage = "La categoria es requerida")]
        
        public CategoriaEnum? Categoria { get; set; }
        [Required(ErrorMessage = "Fecha de creacion requerida")]
        public DateTime FechaCreacion { get; set; }

    }
}
