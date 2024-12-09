using System.ComponentModel.DataAnnotations;

namespace MyWordStystemWebapi.ViewModels
{
    public class AuthenticateRequest
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
