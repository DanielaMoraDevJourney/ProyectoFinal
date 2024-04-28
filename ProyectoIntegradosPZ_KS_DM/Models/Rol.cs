namespace ProyectoIntegradosPZ_KS_DM.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string? Nombre{ get; set; }
        
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
