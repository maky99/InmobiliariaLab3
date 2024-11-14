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


            // busco las relaciones con propietario y nmueble
            var inmuebles = _context.Inmueble
                .Include(i => i.propietario)  // join con propietario
                .Include(i => i.tipo)  // join con tipo inmueble
                .Where(i => i.Id_Propietario == idPropietario)
                .Select(i => new
                {
                    i.Id_Inmueble,
                    Propietario = i.propietario != null ? i.propietario.Apellido + ", " + i.propietario.Nombre : "Propietario no asignado",
                    i.Direccion,
                    i.Uso,
                    i.Ambientes,
                    i.Tamano,
                    TipoInmueble = i.tipo != null ? i.tipo.Tipo : "Tipo no asignado",
                    i.Servicios,
                    i.Bano,
                    i.Cochera,
                    i.Patio,
                    i.Precio,
                    i.Condicion,
                    i.foto,
                    i.Estado_Inmueble
                })
                .ToList();
            if (!inmuebles.Any())
            {
                return NotFound(new { mensaje = "No hay inmuebles disponibles para el propietario especificado." });
            }

            return Ok(inmuebles);
        }


        [HttpPut("actualiEstado")]
        [Authorize]
        public async Task<IActionResult> ActtuInmueble([FromBody] Inmueble inmueble)
        {
            // saco id del propietario desde el token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // id a entero
            int idPropietario = int.Parse(userId);
            // buscamps el inmueble x id  y que pertenece al propietario autenticado
            var inmuebleExistente = _context.Inmueble.FirstOrDefault(i => i.Id_Inmueble == inmueble.Id_Inmueble && i.Id_Propietario == idPropietario);

            if (inmuebleExistente == null)
            {
                return NotFound(new { mensaje = "Inmueble no encontrado o no autorizado para actualizar." });
            }
            // actualizo el estado del inmueble
            inmuebleExistente.Estado_Inmueble = inmueble.Estado_Inmueble;
            _context.Entry(inmuebleExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Estado del inmueble actualizado exitosamente.");
        }

        // [HttpPost("nuevoInmueble")]
        // [Authorize]
        // public async Task<IActionResult> NuevoInmueble([FromBody] Inmueble inmueble)
        // {
        //     Console.WriteLine("llega" + inmueble);
        //     // saco id del propietario desde el token JWT
        //     var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     // id a entero
        //     int idPropietario = int.Parse(userId);
        //     if (userId == null)
        //     {
        //         return Unauthorized("No se pudo obtener el ID del usuario.");
        //     }

        //     inmueble.Id_Propietario = idPropietario;

        //     try
        //     {
        //         // Agrega el inmueble a la base de datos
        //         _context.Add(inmueble);

        //         // Guarda los cambios de forma asincrónica
        //         await _context.SaveChangesAsync();

        //         return Ok(new { mensaje = "Inmueble creado exitosamente", inmueble });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { mensaje = "Error al crear el inmueble", error = ex.Message });
        //     }

        // }

        [HttpPost("nuevoInmueble")]
        [Authorize]
        public async Task<IActionResult> NuevoInmueble(
            [FromForm] string direccion,
            [FromForm] string uso,
            [FromForm] int ambientes,
            [FromForm] double tamano,
            [FromForm] int id_Tipo_Inmueble,
            [FromForm] string servicios,
            [FromForm] int bano,
            [FromForm] int cochera,
            [FromForm] int patio,
            [FromForm] double precio,
            [FromForm] string condicion,
            [FromForm] bool estado_Inmueble,
            [FromForm] IFormFile archivoFoto
        )
        {
            // Obtén el ID del propietario desde el token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("No se pudo obtener el ID del usuario.");
            }
            int idPropietario = int.Parse(userId);

            // Crea el objeto Inmueble
            var inmueble = new Inmueble
            {
                Direccion = direccion,
                Uso = uso,
                Ambientes = ambientes,
                Tamano = tamano,
                Id_Tipo_Inmueble = id_Tipo_Inmueble,
                Servicios = servicios,
                Bano = bano,
                Cochera = cochera,
                Patio = patio,
                Precio = precio,
                Condicion = condicion,
                Estado_Inmueble = estado_Inmueble ? 1 : 0,
                Id_Propietario = idPropietario
            };

            // Manejo del archivo de imagen (si se proporciona)
            if (archivoFoto != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, archivoFoto.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await archivoFoto.CopyToAsync(stream);
                }
                inmueble.foto = Path.Combine("imagenes", archivoFoto.FileName);
            }

            try
            {
                _context.Add(inmueble);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Inmueble creado exitosamente", inmueble });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el inmueble: {ex.Message}");
                return BadRequest(new { mensaje = "Error al crear el inmueble", error = ex.Message });
            }
        }






    }
}


