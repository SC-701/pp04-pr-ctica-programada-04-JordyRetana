using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Reglas
{
    public class AutenticacionBC : IAutenticacionBC
    {
        private readonly IConfiguration _configuration;

        public AutenticacionBC(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidarPasswordHash(string passwordHashIngresado, string passwordHashBD)
        {
            return string.Equals(
                passwordHashIngresado?.Trim(),
                passwordHashBD?.Trim(),
                StringComparison.OrdinalIgnoreCase
            );
        }

        public Token GenerarToken(Usuario usuario, IEnumerable<Perfil> perfiles)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("No se configuró Jwt:Key");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("No se configuró Jwt:Issuer");
            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("No se configuró Jwt:Audience");

            var expireMinutesString = _configuration["Jwt:ExpireMinutes"];
            var expireMinutes = 60;

            if (!string.IsNullOrWhiteSpace(expireMinutesString) && int.TryParse(expireMinutesString, out var minutos))
            {
                expireMinutes = minutos;
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.CorreoElectronico ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("IdUsuario", usuario.Id.ToString()),
                new Claim("NombreUsuario", usuario.NombreUsuario ?? string.Empty),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario ?? string.Empty),
                new Claim(ClaimTypes.Email, usuario.CorreoElectronico ?? string.Empty)
            };

            foreach (var perfil in perfiles)
            {
                claims.Add(new Claim(ClaimTypes.Role, perfil.Id.ToString()));
                claims.Add(new Claim("Perfil", perfil.Nombre ?? string.Empty));
            }

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMinutes(expireMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiracion,
                signingCredentials: credenciales
            );

            var tokenEscrito = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new Token
            {
                AccessToken = tokenEscrito,
                Expiration = expiracion
            };
        }
    }
}