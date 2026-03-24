namespace Abstracciones.Modelos
{
    public class RespuestaLogin
    {
        public bool ValidacionExitosa { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? Expiration { get; set; }
        public string? Mensaje { get; set; }
    }
}