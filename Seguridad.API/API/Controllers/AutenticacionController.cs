using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AutenticacionController : ControllerBase, IAutenticacionController
    {
        private readonly IAutenticacionFlujo _autenticacionFlujo;

        public AutenticacionController(IAutenticacionFlujo autenticacionFlujo)
        {
            _autenticacionFlujo = autenticacionFlujo;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] LoginBase login)
        {
            if (login == null)
            {
                return BadRequest(new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "La solicitud es inválida."
                });
            }

            var respuesta = await _autenticacionFlujo.LoginAsync(login);

            if (!respuesta.ValidacionExitosa)
            {
                return Unauthorized(respuesta);
            }

            return Ok(respuesta);
        }
    }
}