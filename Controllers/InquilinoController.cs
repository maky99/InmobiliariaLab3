using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;  // Cambia el namespace según tu proyecto
using System.Linq;

namespace InmobiliariaSarchioniAlfonzo.Controllers.API  // Asegúrate que el namespace coincida con el del proyecto
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext _context;  // Utilizamos tu DataContext aquí

        public InquilinosController(DataContext context)
        {
            _context = context;
        }

        // Método para obtener todos los inquilinos
        [HttpGet]
        public IActionResult GetInquilinos()
        {
            var inquilinos = _context.Inquilino.ToList();  // Recupera todos los inquilinos de la base de datos
            return Ok(inquilinos);  // Devuelve los inquilinos en formato JSON
        }

        // Método para obtener un inquilino por su ID
        [HttpGet("{id}")]
        public IActionResult GetInquilino(int id)
        {
            var inquilino = _context.Inquilino.FirstOrDefault(i => i.Id_Inquilino == id);  // Busca un inquilino por ID

            if (inquilino == null)
            {
                return NotFound();  // Si no se encuentra, devuelve un error 404
            }

            return Ok(inquilino);  // Devuelve el inquilino en formato JSON
        }

        // Método para crear un nuevo inquilino
        [HttpPost]
        public IActionResult CreateInquilino([FromBody] Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _context.Inquilino.Add(inquilino);  // Añade el nuevo inquilino a la base de datos
                _context.SaveChanges();  // Guarda los cambios en la base de datos
                return CreatedAtAction(nameof(GetInquilino), new { id = inquilino.Id_Inquilino }, inquilino);  // Devuelve el inquilino creado con un código 201
            }

            return BadRequest(ModelState);  // Devuelve un error si el modelo es inválido
        }

        // Método para actualizar un inquilino existente
        [HttpPut("{id}")]
        public IActionResult UpdateInquilino(int id, [FromBody] Inquilino inquilino)
        {
            if (id != inquilino.Id_Inquilino)
            {
                return BadRequest();  // Si el ID no coincide, devuelve un error 400
            }

            if (ModelState.IsValid)
            {
                _context.Entry(inquilino).State = Microsoft.EntityFrameworkCore.EntityState.Modified;  // Marca el inquilino como modificado
                _context.SaveChanges();  // Guarda los cambios en la base de datos
                return NoContent();  // Devuelve un código 204 para indicar éxito
            }

            return BadRequest(ModelState);  // Devuelve un error si el modelo es inválido
        }

        // Método para eliminar un inquilino
        [HttpDelete("{id}")]
        public IActionResult DeleteInquilino(int id)
        {
            var inquilino = _context.Inquilino.Find(id);  // Busca el inquilino por ID

            if (inquilino == null)
            {
                return NotFound();  // Si no se encuentra, devuelve un error 404
            }

            _context.Inquilino.Remove(inquilino);  // Elimina el inquilino de la base de datos
            _context.SaveChanges();  // Guarda los cambios en la base de datos
            return NoContent();  // Devuelve un código 204 para indicar éxito
        }
    }
}
