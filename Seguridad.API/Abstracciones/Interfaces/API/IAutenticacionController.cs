using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace Abstracciones.Interfaces.API
{
    public interface IAutenticacionController
    {
        Task<IActionResult> PostAsync(LoginBase login);
    }
}