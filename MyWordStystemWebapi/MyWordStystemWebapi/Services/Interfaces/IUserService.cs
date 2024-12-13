using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.ViewModels;

namespace MyWordStystemWebapi.Services.Interfaces
{
    public interface IUserService
    {

        AuthenticateResponse Autnenticate(AuthenticateRequest model);
        User GetById(int id);

        User GetUserByEmail(string email);

        bool ChangeWookbook(int userId, string Wordbook);
        bool ChangePassword(int userId, string currentPassword, string newPassword);

        bool ChangePassword(int userId, string newPassword);

        bool RegisterUser(User user, string password);


    }
}
