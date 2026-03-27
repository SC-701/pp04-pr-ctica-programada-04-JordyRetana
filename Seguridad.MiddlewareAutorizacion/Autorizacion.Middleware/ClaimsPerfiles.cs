using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Autorizacion.Abstracciones.Flujo;
using Autorizacion.Abstracciones.Modelos;

namespace Autorizacion.Middleware
{
    public class ClaimsPerfiles
    {
        private readonly RequestDelegate _next;
        private readonly IAutorizacionFlujo _autorizacionFlujo;

        public ClaimsPerfiles(RequestDelegate next, IAutorizacionFlujo autorizacionFlujo)
        {
            _next = next;
            _autorizacionFlujo = autorizacionFlujo;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated)
            {
                var perfiles = await ObtenerInformacionPerfiles(httpContext);
                var claims = perfiles.Select(perfil => new Claim(ClaimTypes.Role, perfil.Id.ToString())).ToList();

                if (httpContext.User.Identity is ClaimsIdentity appIdentity)
                {
                    appIdentity.AddClaims(claims);
                }
            }

            await _next(httpContext);
        }

        private async Task<IEnumerable<Perfil>> ObtenerInformacionPerfiles(HttpContext httpContext)
        {
            var nombreUsuario = httpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                return Enumerable.Empty<Perfil>();
            }

            return await _autorizacionFlujo.ObtenerPerfilesxUsuario(
                new Usuario
                {
                    NombreUsuario = nombreUsuario
                });
        }
    }
}