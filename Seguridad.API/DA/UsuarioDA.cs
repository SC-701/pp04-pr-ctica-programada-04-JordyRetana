using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;
using Dapper;

namespace DA
{
    public class UsuarioDA : IUsuarioDA
    {
        private readonly IRepositorioDapper _repositorioDapper;

        public UsuarioDA(IRepositorioDapper repositorioDapper)
        {
            _repositorioDapper = repositorioDapper;
        }

        public async Task<Usuario?> ObtenerPorCorreoAsync(string correoElectronico)
        {
            using var conexion = _repositorioDapper.ConexionBD();

            var sql = @"
SELECT TOP 1
    Id,
    NombreUsuario,
    CorreoElectronico,
    PasswordHash
FROM Usuarios
WHERE CorreoElectronico = @CorreoElectronico";

            return await conexion.QueryFirstOrDefaultAsync<Usuario>(
                sql,
                new { CorreoElectronico = correoElectronico }
            );
        }

        public async Task<IEnumerable<Perfil>> ObtenerPerfilesAsync(Guid usuarioId)
        {
            using var conexion = _repositorioDapper.ConexionBD();

            var sql = @"
SELECT
    p.Id,
    p.Nombre
FROM UsuariosPerfiles up
INNER JOIN Perfiles p ON p.Id = up.PerfilId
WHERE up.UsuarioId = @UsuarioId";

            var resultado = await conexion.QueryAsync<Perfil>(
                sql,
                new { UsuarioId = usuarioId }
            );

            return resultado;
        }
    }
}