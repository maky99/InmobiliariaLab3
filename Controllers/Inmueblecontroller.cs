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
        public async Task<IActionResult> NuevoInmueble([FromForm] Inmueblee dpto)
        {
            // Imprimiendo las propiedades individualmente
            Console.WriteLine("Llega Dirección: " + dpto.Direccion);
            Console.WriteLine("Llega Uso: " + dpto.Uso);
            Console.WriteLine("Llega Ambientes: " + dpto.Ambientes);
            Console.WriteLine("Llega Tamaño: " + dpto.Tamano);
            Console.WriteLine("Llega Tipo de Inmueble Id: " + dpto.Id_Tipo_Inmueble);
            Console.WriteLine("Llega Servicios: " + dpto.Servicios);
            Console.WriteLine("Llega Baños: " + dpto.Bano);
            Console.WriteLine("Llega Cochera: " + dpto.Cochera);
            Console.WriteLine("Llega Patio: " + dpto.Patio);
            Console.WriteLine("Llega Precio: " + dpto.Precio);
            Console.WriteLine("Llega Condición: " + dpto.Condicion);
            Console.WriteLine("Llega Estado de Inmueble: " + dpto.Estado_Inmueble);
            // Console.WriteLine("Llega Foto: " + dpto.foto.FileName);


            try
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
                    Direccion = dpto.Direccion,
                    Uso = dpto.Uso,
                    Ambientes = dpto.Ambientes,
                    Tamano = dpto.Tamano,
                    Id_Tipo_Inmueble = dpto.Id_Tipo_Inmueble,
                    Servicios = dpto.Servicios,
                    Bano = dpto.Bano,
                    Cochera = dpto.Cochera,
                    Patio = dpto.Patio,
                    Precio = dpto.Precio,
                    Condicion = dpto.Condicion,
                    Estado_Inmueble = dpto.Estado_Inmueble,
                    Id_Propietario = idPropietario

                };

                // Si hay una imagen, guárdala en la carpeta `wwwroot/img`
                if (dpto.foto != null)
                {
                    // Genera la ruta completa para guardar la imagen
                    var imagePath = Path.Combine("wwwroot/imagenes", dpto.foto.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await dpto.foto.CopyToAsync(stream);
                    }
                    // Guarda solo el nombre del archivo en la base de datos
                    inmueble.foto = dpto.foto.FileName; // Guardamos solo el nombre del archivo
                }
                // Guarda el inmueble en la base de datos
                _context.Inmueble.Add(inmueble);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Inmueble creado exitosamente", inmueble });
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}










