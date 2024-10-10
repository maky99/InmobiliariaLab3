using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;  // Cambia el namespace según tu proyecto
using System.Linq;

namespace InmobiliariaLab3.Controllers.API  // Asegúrate que el namespace coincida con el del proyecto
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext _context;  // Utilizamos tu DataContext aquí

        public PropietariosController(DataContext context)
        {
            _context = context;
        }

        // Método para obtener todos los propietarios
        [HttpGet]
        public IActionResult GetPropietarios()
        {
            var propietarios = _context.Propietario.ToList();  // Recupera todos los propietarios de la base de datos
            return Ok(propietarios);  // Devuelve los propietarios en formato JSON
        }

        //Método para obtener un propietario por su ID
        [HttpGet("{id}")]
        public IActionResult GetPropietario(int id)
        {
            var propietario = _context.Propietario.FirstOrDefault(p => p.Id_Propietario == id);  // Busca un propietario por ID

            if (propietario == null)
            {
                return NotFound();  // Si no se encuentra, devuelve un error 404
            }

            return Ok(propietario);  // Devuelve el propietario en formato JSON
        }
    }
}
