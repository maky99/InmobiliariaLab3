using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaSarchioniAlfonzo.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext _context;

        public InquilinosController(DataContext context)
        {
            _context = context;
        }

        // metodo para obtener todos los inquilinos
        [HttpGet("inquilinos")]
        [Authorize]
        public async Task<IActionResult> GetInquilinos()
        {
            // extraer el id del propietario desde el token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // convertir el id a entero
            int idPropietario = int.Parse(userId);


            var inquilinos = await _context.Contrato
      .Include(c => c.inquilino)         // incluir info del inquilino
      .Include(c => c.inmueble)           // incluir info del inmueble
      .Include(c => c.propietario)        // incluir info del propietario
      .Where(c => c.Id_Propietario == idPropietario && c.Estado_Contrato == 1)
      .Select(c => new
      {
          c.inquilino.Id_Inquilino,
          c.inquilino.Dni,
          c.inquilino.Apellido,
          c.inquilino.Nombre,
          c.inquilino.Telefono,
          c.inquilino.Email,
          c.inquilino.Estado_Inquilino,
          c.Estado_Contrato,
          c.inmueble.Direccion,
          c.inmueble.foto

      })
      .ToListAsync();

            return Ok(inquilinos);

        }

    }

}
