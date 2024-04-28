using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIntegradosPZ_KS_DM.Models
{
    public class Comentario
    {
        //references
        public int ComentarioId { get; set; }
        //references
        public string? Contenido { get; set; }
        //references
        public DateTime FechaCreacion { get; set; }
        //references
        public int UsuarioId { get; set; }
        //references
        public int PostId { get; set; }
        //references
        public int? ComentarioPadreId { get; set; }
     
        // references 
        public List <Comentario> ComentariosHijos { get; set; }
        [NotMapped]
        public string? NombreUsuario { get; set; }
        // references 
        public int? ComentarioAbueloId { get; set; }
    }
}
