using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;

namespace Flujo
{
    public class UsuarioFlujo : IUsuarioFlujo
    {
        private readonly IUsuarioDA _usuarioDA;

        public UsuarioFlujo(IUsuarioDA usuarioDA)
        {
            _usuarioDA = usuarioDA;
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
        {
            return await _usuarioDA.ObtenerPorCorreoAsync(correoElectronico);
        }

        public async Task<IEnumerable<Perfil>> ObtenerPerfilesAsync(Guid usuarioId)
        {
            return await _usuarioDA.ObtenerPerfilesAsync(usuarioId);
        }
    }
}