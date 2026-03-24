using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IUsuarioFlujo
    {
        Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico);
        Task<IEnumerable<Perfil>> ObtenerPerfilesAsync(Guid usuarioId);
    }
}