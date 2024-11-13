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
    [FromForm] int tipo,  // Cambiado a int, ya que 'tipo' es un ID de Tipo_Inmueble
    [FromForm] int ambientes,
    [FromForm] double precio, // Cambiado a decimal, ya que 'precio' es de tipo decimal
    [FromForm] string servicios,
    [FromForm] int patio,
    [FromForm] bool estado, // Cambiado a bool, ya que 'estado' es de tipo booleano
    [FromForm] IFormFile archivoFoto) // Recibe el archivo de imagen
{
    // Obtén el ID del propietario desde el token JWT
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
    {
        return Unauthorized("No se pudo obtener el ID del usuario.");
    }
    int idPropietario = int.Parse(userId);

    // Crea el objeto Inmueble
    Inmueble inmueble = new Inmueble
    {
        Direccion = direccion,
        Uso = uso,
        Id_Tipo_Inmueble = tipo,  // Aquí asignas el tipo directamente, ya que el tipo se pasa como un ID
        Ambientes = ambientes,
        Precio = precio,  // Usando decimal para el precio
        Servicios = servicios,
        Patio = patio,
        Estado_Inmueble = estado ? 1 : 0, // Convierte el booleano a 1 o 0 para el estado
        Id_Propietario = idPropietario
    };

    if (archivoFoto != null)
    {
        // Guardar la imagen en el servidor o procesarla según sea necesario
        var filePath = Path.Combine("RutaDeImagenes", archivoFoto.FileName); // Define una ruta de almacenamiento
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await archivoFoto.CopyToAsync(stream);
        }
        inmueble.foto = filePath; // Asocia la ruta de la imagen con el inmueble
    }

    try
    {
        // Agrega el inmueble a la base de datos
        _context.Add(inmueble);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Inmueble creado exitosamente", inmueble });
    }
    catch (Exception ex)
    {
        return BadRequest(new { mensaje = "Error al crear el inmueble", error = ex.Message });
    }
}








    }

    
}
