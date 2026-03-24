namespace Abstracciones.Modelos
{
    public class TokenConfiguracion
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int Expires { get; set; }
    }
}