using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Reglas
{
    public static class Autenticacion
    {
        public static byte[] GenerarHash(string contrasenia)
        {
            using SHA256 shaHash = SHA256.Create();
            return shaHash.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
        }

        public static string ObtenerHash(byte[] hash)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static JwtSecurityToken? LeerToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadToken(token) as JwtSecurityToken;
        }

        public static List<Claim> GenerarClaims(JwtSecurityToken? jwtToken, string accessToken)
        {
            var claims = new List<Claim>();

            if (jwtToken == null)
                return claims;

            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            var idUsuario = jwtToken.Claims.FirstOrDefault(c => c.Type == "IdUsuario")?.Value ?? string.Empty;

            claims.Add(new Claim(ClaimTypes.Name, name));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, idUsuario));
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim("AccessToken", accessToken));

            foreach (var rol in jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Value));
            }

            return claims;
        }
    }
}