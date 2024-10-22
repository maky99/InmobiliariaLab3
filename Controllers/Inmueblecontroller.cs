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

    public class InmuebleController : ControllerBase
    {
        private readonly DataContext _context;

        public InmuebleController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("inmuebles")]
        [Authorize]
        public IActionResult GetInmuebles()
        {
            // extraer el id del propietario desde el token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del token." });
            }
            // convertir el id a entero
            int idPropietario = int.Parse(userId);


            // Incluimos las relaciones con Propietario y Tipo_Inmueble
            var inmuebles = _context.Inmueble
                .Include(i => i.propietario)  // Incluimos la relación con Propietario
                .Include(i => i.tipo)  // Incluimos la relación con Tipo_Inmueble
                .Where(i => i.Id_Propietario == idPropietario)
                .Select(i => new
                {
                    i.Id_Inmueble,
                    Propietario = i.propietario != null ? i.propietario.Apellido + ", " + i.propietario.Nombre : "Propietario no asignado",
                    i.Direccion,
                    i.Uso,
                    i.Ambientes,
                    i.Latitud,
                    i.Longitud,
                    i.Tamano,
                    TipoInmueble = i.tipo != null ? i.tipo.Tipo : "Tipo no asignado",
                    i.Servicios,
                    i.Bano,
                    i.Cochera,
                    i.Patio,
                    i.Precio,
                    i.Condicion,
                    i.Estado_Inmueble
                })
                .ToList();
            if (!inmuebles.Any())
            {
                return NotFound(new { mensaje = "No hay inmuebles disponibles para el propietario especificado." });
            }

            return Ok(inmuebles);
        }
    }
}
