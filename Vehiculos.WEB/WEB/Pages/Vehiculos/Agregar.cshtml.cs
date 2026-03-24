using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Pages.Vehiculos
{
    [Authorize]
    public class AgregarModel : PageModel
    {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public VehiculoRequest vehiculo { get; set; } = new VehiculoRequest();

        [BindProperty]
        public List<SelectListItem> marcas { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public List<SelectListItem> modelos { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public Guid marcaSeleccionada { get; set; }

        private static readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public AgregarModel(IConfiguracion configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task<IActionResult> OnGet()
        {
            await ObtenerMarcasAsync();
            modelos = new List<SelectListItem>();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            vehiculo.Placa = (vehiculo.Placa ?? "").Trim().ToUpperInvariant();

            if (vehiculo.IdModelo == Guid.Empty)
            {
                ModelState.AddModelError("vehiculo.IdModelo", "Debe seleccionar un modelo.");
            }

            if (!ModelState.IsValid)
            {
                await ObtenerMarcasAsync();

                if (marcaSeleccionada != Guid.Empty)
                {
                    var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
                    modelos = listaModelos.Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    }).ToList();
                }
                else
                {
                    modelos = new List<SelectListItem>();
                }

                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarVehiculo");

            using var cliente = new HttpClient();
            AgregarToken(cliente);

            var respuesta = await cliente.PostAsJsonAsync(endpoint, vehiculo);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Seguridad/Login");
            }

            if (!respuesta.IsSuccessStatusCode)
            {
                var detalle = await respuesta.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty,
                    $"Error API ({(int)respuesta.StatusCode}): {detalle}");

                await ObtenerMarcasAsync();

                if (marcaSeleccionada != Guid.Empty)
                {
                    var listaModelos = await ObtenerModelosAsync(marcaSeleccionada);
                    modelos = listaModelos.Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Nombre
                    }).ToList();
                }

                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task ObtenerMarcasAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerMarcas");

            using var cliente = new HttpClient();
            AgregarToken(cliente);

            var respuesta = await cliente.GetAsync(endpoint);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                Response.Redirect("/Seguridad/Login");
                return;
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            var resultadoDeserializado = JsonSerializer.Deserialize<List<Marca>>(resultado, _jsonOptions) ?? new List<Marca>();

            marcas = resultadoDeserializado.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Nombre
            }).ToList();
        }

        public async Task<JsonResult> OnGetObtenerModelos(Guid marcaId)
        {
            var lista = await ObtenerModelosAsync(marcaId);
            return new JsonResult(lista.Select(m => new { id = m.Id, nombre = m.Nombre }));
        }

        private async Task<List<Modelo>> ObtenerModelosAsync(Guid marcaId)
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerModelos");

            using var cliente = new HttpClient();
            AgregarToken(cliente);

            var url = string.Format(endpoint, marcaId);
            var respuesta = await cliente.GetAsync(url);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new List<Modelo>();
            }

            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Modelo>>(resultado, _jsonOptions) ?? new List<Modelo>();
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