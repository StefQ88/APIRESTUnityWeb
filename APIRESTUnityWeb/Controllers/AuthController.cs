using APIRESTUnityWeb.Data;
using APIRESTUnityWeb.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


//Controlador de autenticación. Maneja los endpoints para registrarse e iniciar sesión.
namespace APIRESTUnityWeb.Controllers
{

    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // --------------------------------------
        // POST /auth/register
        // Registra un nuevo usuario con el endpoint /auth/register
        // Recibe un usuario nuevo, verifica que el mail o usuario no estén repetidos, y si todo va bien lo guarda en la base.
        // --------------------------------------
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Verificar si ya existe un usuario con el mismo email o username
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email || u.UserName == user.UserName);

            if (existingUser != null)
            {
                return BadRequest(new { message = "El correo o nombre de usuario ya están registrados." });
            }

            // Asignar la fecha actual y guardar el nuevo usuario
            user.RegistrationDate = DateTime.UtcNow;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Usuario registrado correctamente." });
        }

        // --------------------------------------
        // POST /auth/login
        // Inicia sesión con email/usuario y contraseña
        // Busca un usuario con el mail o usuario y la contraseña que llegaron del frontend.
        // Si lo encuentra, responde con un mensaje de éxito y los datos básicos del usuario.
        // Si no lo encuentra, responde con un error de credenciales inválidas.
        // --------------------------------------
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var foundUser = _context.Users
                .FirstOrDefault(u =>
                    (u.Email == request.UserOrEmail || u.UserName == request.UserOrEmail)
                    && u.Password == request.Password
                );

            if (foundUser == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                user = new
                {
                    id = foundUser.UserId,
                    firstName = foundUser.FirstName,
                    surName = foundUser.SurName,
                    email = foundUser.Email
                }
            });
        }
    }
}
