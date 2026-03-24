namespace Abstracciones.Modelos
{
    public class LoginBase
    {
        public string? NombreUsuario { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? PasswordHash { get; set; }
    }
}