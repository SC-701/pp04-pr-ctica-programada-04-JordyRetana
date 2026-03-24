using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IAutenticacionFlujo
    {
        Task<RespuestaLogin> LoginAsync(LoginBase login);
    }
}