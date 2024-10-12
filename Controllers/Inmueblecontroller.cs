using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;  // Cambia el namespace según tu proyecto
using Microsoft.EntityFrameworkCore;  // Para usar Include
using System.Linq;

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

        [HttpGet("{id_propietario}")]
        public IActionResult InmueblesXId(int id_propietario)
        {
            // Incluimos las relaciones con Propietario y Tipo_Inmueble
            var inmuebles = _context.Inmueble
                .Include(i => i.propietario)  // Incluimos la relación con Propietario
                .Include(i => i.tipo)  // Incluimos la relación con Tipo_Inmueble
                .Where(i => i.Id_Propietario == id_propietario)
                .Select(i => new
                {
                    i.Id_Inmueble,
                    Propietario = i.propietario.Apellido + ", " + i.propietario.Nombre,
                    i.Direccion,
                    i.Uso,
                    i.Ambientes,
                    i.Latitud,
                    i.Longitud,
                    i.Tamano,
                    TipoInmueble = i.tipo.Tipo,  // Proyectamos la descripción del tipo de inmueble
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
