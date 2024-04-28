using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProyectoIntegradosPZ_KS_DM.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electronico es requerido.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo electronico valido.")]
        public string? Correo { get; set; }

        [DataType(DataType.Password)]
        public string? Contraseña { get; set; }

        [DisplayName("Mantener sesion activa.")]
        public bool MantenerActivo { get; set; }
    }
}
