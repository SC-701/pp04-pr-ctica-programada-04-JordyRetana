using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Web.Pages.Seguridad
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public LoginModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasenia { get; set; } = string.Empty;

        public string? MensajeError { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var contraseniaLimpia = (Contrasenia ?? string.Empty).Trim();
            var hash = Autenticacion.ObtenerHash(Autenticacion.GenerarHash(contraseniaLimpia));

            var login = new LoginRequest
            {
                NombreUsuario = string.Empty,
                CorreoElectronico = (CorreoElectronico ?? string.Empty).Trim(),
                PasswordHash = hash.Trim()
            };

            using var cliente = new HttpClient();

            string endpoint = _configuracion.ObtenerMetodo("ApiSeguridad", "Login");

            var respuesta = await cliente.PostAsJsonAsync(endpoint, login);
            var contenido = await respuesta.Content.ReadAsStringAsync();

            if (!respuesta.IsSuccessStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(contenido))
                {
                    try
                    {
                        var errorApi = JsonSerializer.Deserialize<RespuestaLogin>(
                            contenido,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        MensajeError = errorApi?.Mensaje ?? "Credenciales inválidas.";
                    }
                    catch
                    {
                        MensajeError = contenido;
                    }
                }
                else
                {
                    MensajeError = "Credenciales inválidas.";
                }

                return Page();
            }

            var resultado = JsonSerializer.Deserialize<RespuestaLogin>(contenido, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (resultado == null || !resultado.ValidacionExitosa || string.IsNullOrWhiteSpace(resultado.AccessToken))
            {
                MensajeError = resultado?.Mensaje ?? "No fue posible iniciar sesión.";
                return Page();
            }

            JwtSecurityToken? jwtToken = Autenticacion.LeerToken(resultado.AccessToken);
            List<Claim> claims = Autenticacion.GenerarClaims(jwtToken, resultado.AccessToken);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToPage("/Vehiculos/Index");
        }
    }
}