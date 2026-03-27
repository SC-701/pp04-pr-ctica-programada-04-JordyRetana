using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Reglas;

namespace Web.Pages.Seguridad
{
    public class RegistroModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public RegistroModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        [BindProperty]
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contrasenia { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La confirmación es requerida")]
        [Compare(nameof(Contrasenia), ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasenia { get; set; } = string.Empty;

        public string? MensajeError { get; set; }
        public string? MensajeExito { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var contraseniaLimpia = (Contrasenia ?? string.Empty).Trim();
            var hash = Autenticacion.ObtenerHash(Autenticacion.GenerarHash(contraseniaLimpia));

            var usuario = new UsuarioRegistroRequest
            {
                NombreUsuario = (NombreUsuario ?? string.Empty).Trim(),
                CorreoElectronico = (CorreoElectronico ?? string.Empty).Trim(),
                PasswordHash = hash.Trim()
            };

            using var cliente = new HttpClient();

            string endpoint = _configuracion.ObtenerMetodo("ApiSeguridad", "RegistrarUsuario");

            var respuesta = await cliente.PostAsJsonAsync(endpoint, usuario);
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

                        MensajeError = errorApi?.Mensaje ?? "No fue posible registrar el usuario.";
                    }
                    catch
                    {
                        MensajeError = contenido;
                    }
                }
                else
                {
                    MensajeError = "No fue posible registrar el usuario.";
                }

                return Page();
            }

            TempData["MensajeExito"] = "Usuario registrado correctamente. Ahora puedes iniciar sesión.";
            return RedirectToPage("/Seguridad/Login");
        }
    }
}