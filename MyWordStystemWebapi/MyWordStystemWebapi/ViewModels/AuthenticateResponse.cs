using MyWordStystemWebapi.Models;

namespace MyWordStystemWebapi.ViewModels
{
    public class AuthenticateResponse
    {

        public AuthenticateResponse(User user, string token)
        {
            Id = user.Userid;
            Token = token;
            Email = user.Email;

        }

        public int? Id { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }

    }
}
