using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;

namespace Flujo
{
    public class AutenticacionFlujo : IAutenticacionFlujo
    {
        private readonly IUsuarioFlujo _usuarioFlujo;
        private readonly IAutenticacionBC _autenticacionBC;

        public AutenticacionFlujo(
            IUsuarioFlujo usuarioFlujo,
            IAutenticacionBC autenticacionBC)
        {
            _usuarioFlujo = usuarioFlujo;
            _autenticacionBC = autenticacionBC;
        }

        public async Task<RespuestaLogin> LoginAsync(LoginBase login)
        {
            if (login == null)
            {
                return new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "La solicitud de login es inválida."
                };
            }

            if (string.IsNullOrWhiteSpace(login.CorreoElectronico))
            {
                return new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "El correo electrónico es requerido."
                };
            }

            if (string.IsNullOrWhiteSpace(login.PasswordHash))
            {
                return new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "La contraseña es requerida."
                };
            }

            var usuario = await _usuarioFlujo.ObtenerPorCorreoAsync(login.CorreoElectronico);

            if (usuario == null)
            {
                return new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "Usuario o contraseña inválidos."
                };
            }

            var passwordValido = _autenticacionBC.ValidarPasswordHash(
                login.PasswordHash,
                usuario.PasswordHash ?? string.Empty
            );

            if (!passwordValido)
            {
                return new RespuestaLogin
                {
                    ValidacionExitosa = false,
                    Mensaje = "Usuario o contraseña inválidos."
                };
            }

            var perfiles = await _usuarioFlujo.ObtenerPerfilesAsync(usuario.Id);

            var token = _autenticacionBC.GenerarToken(usuario, perfiles);

            return new RespuestaLogin
            {
                ValidacionExitosa = true,
                AccessToken = token.AccessToken,
                Expiration = token.Expiration,
                Mensaje = "Inicio de sesión exitoso."
            };
        }
    }
}