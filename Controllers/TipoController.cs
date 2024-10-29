using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InmobiliariaSarchioniAlfonzo.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]

    public class TipoController : ControllerBase
    {
        private readonly DataContext _context;

        public TipoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("tipoInmueble")]
        [Authorize]
        public IActionResult TipoInmuebles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del token." });
            }
            var tipo_inmuebles = _context.Tipo_Inmueble
                                 .Where(t => t.Estado_Tipo_Inmueble == 1)
                                 .ToList();
            return Ok(tipo_inmuebles);
        }
    }
}