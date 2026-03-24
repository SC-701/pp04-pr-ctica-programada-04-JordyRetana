using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.DA
{
    public interface IUsuarioDA
    {
        Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico);
        Task<IEnumerable<Perfil>> ObtenerPerfilesAsync(Guid usuarioId);
    }
}