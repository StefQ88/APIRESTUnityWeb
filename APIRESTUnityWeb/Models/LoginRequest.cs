using System.ComponentModel.DataAnnotations;

// Modelo que representa los datos que vas a recibir cuando alguien intenta iniciar sesi�n.
namespace APIRESTUnityWeb.Models
{
    public class LoginRequest
    {
        [Required]
        public string UserOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
