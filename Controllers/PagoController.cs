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

    public class PagoController : ControllerBase
    {
        private readonly DataContext _context;

        public PagoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("pagos/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPagos(int id)
        {
            var idContrato = id;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del token." });
            }

            int idPropietario = int.Parse(userId);
            // Verifica que el contrato pertenece al propietario autenticado
            var contrato = await _context.Contrato
                .FirstOrDefaultAsync(c => c.Id_Contrato == id && c.Id_Propietario == idPropietario);

            if (contrato == null)
            {
                return NotFound(new { mensaje = "No se encontraron pagos para este contrato o el contrato no pertenece al propietario autenticado." });
            }

            // pagos asociados al contrato
            var pagos = await _context.Pago
                .Where(p => p.Id_Contrato == id)
                .Select(pago => new
                {
                    pago.Id_Contrato,
                    pago.Id_Pago,
                    pago.Importe,
                    pago.CuotaPaga,
                    pago.Fecha,
                    pago.Multa,
                    pago.Estado_Pago
                })
                .ToListAsync();


            return Ok(pagos);
        }




    }







}
