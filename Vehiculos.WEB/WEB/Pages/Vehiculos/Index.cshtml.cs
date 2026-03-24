using Microsoft.AspNetCore.Mvc.RazorPages;
using Abstracciones.Interfaces.Reglas;
using System.Net;
using Abstracciones.Modelos;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Web.Pages.Vehiculos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        public IList<VehiculoResponse> vehiculos { get; set; } = new List<VehiculoResponse>();

        public IndexModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task OnGet()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerVehiculos");

            using var cliente = new HttpClient();

            var token = User.FindFirst("AccessToken")?.Value;
            if (!string.IsNullOrWhiteSpace(token))
            {
                cliente.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var solicitud = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Response.Redirect("/Seguridad/Login");
                return;
            }

            respuesta.EnsureSuccessStatusCode();

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                vehiculos = JsonSerializer.Deserialize<List<VehiculoResponse>>(resultado, opciones) ?? new List<VehiculoResponse>();
            }
        }
    }
}