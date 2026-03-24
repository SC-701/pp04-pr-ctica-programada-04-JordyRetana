namespace Abstracciones.Modelos
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string? NombreUsuario { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? PasswordHash { get; set; }
    }
}