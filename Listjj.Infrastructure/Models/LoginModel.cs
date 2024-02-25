using System.ComponentModel.DataAnnotations;

namespace Listjj.Infrastructure.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string? GoogleJwt { get; set; }
    }
}
