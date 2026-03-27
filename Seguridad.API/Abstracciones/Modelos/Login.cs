using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos
{
    public class LoginBase
    {
        public string? NombreUsuario { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = string.Empty;
    }

    public class Login : LoginBase
    {
        [Required]
        public Guid Id { get; set; }
    }
}
