using System.ComponentModel.DataAnnotations;

namespace ProyectoIntegradosPZ_KS_DM.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        [Required(ErrorMessage= "El campo es requerido")]
        [StringLength(50,ErrorMessage="Limite excedido")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(50, ErrorMessage = "Limite excedido")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(50, ErrorMessage = "Limite excedido")]
        public string? Correo { get; set; }
        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(50, ErrorMessage = "Limite excedido")]
        public string? Contrasenia { get; set; }
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
        public string? NombreUsuario { get; set; }
        public Boolean Estado {  get; set; }
        public string? Token {  get; set; }
        public DateTime? FechaExpiracion { get; set; }


    }
}
