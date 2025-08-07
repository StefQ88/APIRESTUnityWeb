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
            try
            {
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Email == user.Email || u.UserName == user.UserName);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "El correo o nombre de usuario ya están registrados." });
                }

                user.RegistrationDate = DateTime.UtcNow;
                _context.Users.Add(user);
                _context.SaveChanges();

                // Por ahora devolvemos solo el mensaje, sin fecha
                return Ok(new
                {
                    message = "Usuario registrado correctamente."
                });
            }
            catch (Exception ex)
            {
                // Devuelve el stacktrace completo en 'detail'
                return StatusCode(500, new { message = "Error interno del servidor.", detail = ex.ToString() });
            }
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
