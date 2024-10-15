using Microsoft.AspNetCore.Mvc;
using InmobiliariaLab3.Models;  // Cambia el namespace según tu proyecto
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaLab3.Controllers.API  // Asegúrate que el namespace coincida con el del proyecto
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext _context;  // Utilizamos tu DataContext aquí
        private readonly IConfiguration _configuration;  // se agrega IConfiguration para usar la salt

        public PropietariosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Método para obtener todos los propietarios

        [HttpGet]
        public IActionResult GetPropietarios()
        {
            var propietarios = _context.Propietario.ToList();  // Recupera todos los propietarios de la base de datos
            return Ok(propietarios);  // Devuelve los propietarios en formato JSON
        }

        //Método para obtener un propietario por su ID y
        [HttpGet("{id}")]
        [Authorize]  // Requiere autorización con token

        public IActionResult GetPropietario(int id)
        {
            var propietario = _context.Propietario.FirstOrDefault(p => p.Id_Propietario == id);  // Busca un propietario por ID

            if (propietario == null)
            {
                return NotFound();  // Si no se encuentra, devuelve un error 404
            }

            return Ok(propietario);  // Devuelve el propietario en formato JSON
        }


        //genero contraseña si no tiene 
        [HttpPost("{id}/generarContrasena")]
        public IActionResult HashPassword(int id)
        {
            var propietario = _context.Propietario.FirstOrDefault(p => p.Id_Propietario == id);  // buscamos el propietario por ID

            if (propietario == null)
            {
                return NotFound();  // si no esta encuentra- devuelve un error 404
            }

            var password = propietario.Contrasena;

            // Verifica si la contraseña está vacía o nula
            if (string.IsNullOrEmpty(password))
            {
                byte[] salt;
                salt = Convert.FromBase64String(_configuration["Salt"]);
                // Generar el hash de la nueva contraseña
                string nuevaContrasena = "1234";  //generar una nueva contraseña por defecto.
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                password: nuevaContrasena,
                                salt: salt,
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 1000,
                                numBytesRequested: 256 / 8));

                // actualiza la contraseña del propietario
                propietario.Contrasena = hashed;
                // guarda los cambios en la base de datos
                _context.SaveChanges();

                return Ok(new { mensaje = "Contraseña generada y guardada con éxito", nuevaContrasena });
            }

            return BadRequest(new { mensaje = "El propietario ya tiene una contraseña asignada." });
        }





        [HttpPost("Ingreso")]
        public async Task<IActionResult> Ingreso([FromBody] Login login)
        {
            // Busca el propietario por nombre de usuario
            var propietario = await _context.Propietario.FirstOrDefaultAsync(p => p.Email == login.Email);

            // Verifica si el propietario existe
            if (propietario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });  // Si no se encuentra, devuelve un error 404
            }

            // chequeamos la contraseña
            var hashedPassword = propietario.Contrasena;  // traigo la contraseña hasheada de la base de datos
            if (!VerificarContrasena(login.Clave, hashedPassword))  // comparamos las contraseñas 
            {
                return Unauthorized(new { mensaje = "Contraseña incorrecta." });  // Si no coinicden devuelve un error 401
            }

            // traigo la configuración del token desde el archivo appsettings
            var issuer = _configuration["TokenAuthentication:Issuer"];
            var audience = _configuration["TokenAuthentication:Audience"];
            var secretKey = _configuration["TokenAuthentication:SecretKey"];

            var key = Encoding.ASCII.GetBytes(secretKey);  // convierte la clave secreta a bytes

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, propietario.Id_Propietario.ToString()),  // Identificador único del propietario
            new Claim("Apellido", propietario.Apellido), // Apellido del propietario (uso de Claim personalizado)
            new Claim(ClaimTypes.Name, propietario.Nombre),  // Nombre del propietario
             new Claim(ClaimTypes.Email, propietario.Email),
                }),
                Expires = DateTime.UtcNow.AddHours(1),  // Token válido por 1 hora
                Issuer = issuer,  // Configura el Issuer desde appsettings.json
                Audience = audience,  // Configura el Audience desde appsettings.json
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        // Método para verificar la contraseña (suponiendo que uses el mismo método de hashing que al almacenar)
        private bool VerificarContrasena(string password, string hashedPassword)
        {
            // Asegúrate de usar la misma lógica de hashing que utilizaste para almacenar la contraseña
            var salt = Convert.FromBase64String(_configuration["Salt"]);
            var hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            return hashedInputPassword == hashedPassword;  // Compara la contraseña hasheada ingresada con la almacenada
        }



    }

}
