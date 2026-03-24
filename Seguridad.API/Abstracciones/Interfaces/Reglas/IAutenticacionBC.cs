using Abstracciones.Modelos;

namespace Abstracciones.Interfaces.Reglas
{
    public interface IAutenticacionBC
    {
        Token GenerarToken(Usuario usuario, IEnumerable<Perfil> perfiles);
        bool ValidarPasswordHash(string passwordHashIngresado, string passwordHashBD);
    }
}