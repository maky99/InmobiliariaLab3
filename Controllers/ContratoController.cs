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

    public class ContratoController : ControllerBase
    {
        private readonly DataContext _context;

        public ContratoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("contratos")]
        [Authorize]
        public async Task<IActionResult> GetContratos()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del token." });
            }

            int idPropietario = int.Parse(userId);

            var contratos = await _context.Contrato
                .Include(i => i.inquilino)
                .Include(i => i.inmueble)
                .Include(i => i.propietario)
                .Where(i => i.Id_Propietario == idPropietario && i.Estado_Contrato == 1)
                .ToListAsync();

            if (contratos.Count == 0)
            {
                return NotFound(new { mensaje = "No se encontraron contratos para este propietario." });
            }

            // campos necesarios
            var resultado = contratos.Select(contrato => new
            {
                contrato.Id_Contrato,
                contrato.Fecha_Inicio,
                contrato.Fecha_Finalizacion,
                contrato.Estado_Contrato,
                Inquilino = new
                {
                    contrato.inquilino.Nombre,
                    contrato.inquilino.Apellido,
                    contrato.inquilino.Dni
                },
                Inmueble = new
                {
                    contrato.inmueble.Id_Inmueble,
                    contrato.inmueble.Id_Propietario,
                    contrato.inmueble.Direccion,
                    contrato.inmueble.Uso,
                    contrato.inmueble.Ambientes,
                    contrato.inmueble.Tamano,
                    contrato.inmueble.Id_Tipo_Inmueble,
                    contrato.inmueble.Servicios,
                    contrato.inmueble.Bano,
                    contrato.inmueble.Cochera,
                    contrato.inmueble.Patio,
                    contrato.inmueble.Precio,
                    contrato.inmueble.Condicion,
                    contrato.inmueble.tipo,
                    contrato.inmueble.foto,
                    contrato.inmueble.Estado_Inmueble
                },
                Propietario = new
                {
                    contrato.propietario.Nombre,
                    contrato.propietario.Apellido,
                    contrato.propietario.Dni,
                    contrato.propietario.Email,
                    contrato.propietario.Telefono
                }
            });

            return Ok(resultado);
        }

    }


}
