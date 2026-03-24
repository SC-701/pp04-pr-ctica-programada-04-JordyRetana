using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    [Authorize]
    public class EliminarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public VehiculoResponse vehiculo { get; set; } = default!;

        public EliminarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculo");

            using var cliente = new HttpClient();
            AgregarToken(cliente);

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                return RedirectToPage("/Seguridad/Login");

            if (!respuesta.IsSuccessStatusCode)
                return NotFound();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                vehiculo = JsonSerializer.Deserialize<VehiculoResponse>(resultado, opciones)!;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return NotFound();

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EliminarVehiculo");

            try
            {
                using var cliente = new HttpClient();
                AgregarToken(cliente);

                var solicitud = new HttpRequestMessage(HttpMethod.Delete, string.Format(endpoint, id));
                var respuesta = await cliente.SendAsync(solicitud);

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                    return RedirectToPage("/Seguridad/Login");

                if (!respuesta.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo eliminar el vehículo.");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al intentar eliminar el vehículo.");
                return Page();
            }
        }

        private void AgregarToken(HttpClient cliente)
        {
            var token = User.FindFirst("AccessToken")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                cliente.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}