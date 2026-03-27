using Abstracciones.Reglas;
using Abstracciones.DA;
using Abstracciones.Modelos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reglas
{
    public class AutenticacionReglas : IAutenticacionBC
    {
        public IConfiguration _configuration;
        public IUsuarioDA _usuarioDA;
        private Usuario? _usuario;

        public AutenticacionReglas(IConfiguration configuration, IUsuarioDA usuarioDA)
        {
            _configuration = configuration;
            _usuarioDA = usuarioDA;
        }

        public async Task<Token> LoginAync(LoginBase login)
        {
            Token respuestaToken = new Token()
            {
                AccessToken = string.Empty,
                ValidacionExitosa = false
            };

            _usuario = await _usuarioDA.ObtenerUsuario(new Usuario
            {
                CorreoElectronico = login.CorreoElectronico,
                NombreUsuario = null
            });

            if (_usuario == null)
                return respuestaToken;

            var resultadoVerificacionCredenciales = await VerificarHashContraseniaAsync(login);
            if (!resultadoVerificacionCredenciales)
                return respuestaToken;

            TokenConfiguracion? tokenConfiguracion = _configuration.GetSection("Jwt").Get<TokenConfiguracion>();
            if (tokenConfiguracion == null)
                return respuestaToken;

            JwtSecurityToken token = await GenerarTokenJWT(tokenConfiguracion);
            respuestaToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            respuestaToken.ValidacionExitosa = true;

            return respuestaToken;
        }

        private Task<bool> VerificarHashContraseniaAsync(LoginBase login)
        {
            return Task.FromResult(login != null && _usuario != null && login.PasswordHash == _usuario.PasswordHash);
        }

        private Task<JwtSecurityToken> GenerarTokenJWT(TokenConfiguracion tokenConfiguracion)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguracion.key));
            List<Claim> claims = GenerarClaims();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                tokenConfiguracion.Issuer,
                tokenConfiguracion.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(tokenConfiguracion.ExpireMinutes),
                signingCredentials: credentials
            );

            return Task.FromResult(token);
        }

        private List<Claim> GenerarClaims()
        {
            List<Claim> claims = new List<Claim>();

            if (_usuario != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, _usuario.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, _usuario.NombreUsuario ?? string.Empty));
                claims.Add(new Claim(ClaimTypes.Email, _usuario.CorreoElectronico ?? string.Empty));
            }

            return claims;
        }
    }
}