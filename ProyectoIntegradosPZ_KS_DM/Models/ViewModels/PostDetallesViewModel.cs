namespace ProyectoIntegradosPZ_KS_DM.Models.ViewModels
{
    public class PostDetallesViewModel
    {
        public Post? Post { get; set; }

        public List<Comentario>? ComentariosPrincipales { get; set; }
        public List<Comentario>? ComentariosHijos { get; set; }
        public List<Comentario> ComentariosNietos { get; set; }
        public List<Post>? PostRecientes { get; set; }
    }
}
