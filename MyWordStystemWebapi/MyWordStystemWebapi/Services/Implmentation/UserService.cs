using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Helpers;
using MyWordStystemWebapi.Services.Interfaces;
using MyWordStystemWebapi.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyWordStystemWebapi.Data;

namespace MyWordStystemWebapi.Services.Implmentation
{
    public class UserService : IUserService
    {

         private readonly MywordDbContext _context;
        private readonly AuthSettings _authSettings;

        public UserService(MywordDbContext context, IOptions<AuthSettings> authSettings)
        {
            _context = context;
            _authSettings = authSettings.Value;
        }


        public User GetUserByEmail(string email)
        {
            return _context.user_Table.FirstOrDefault(u => u.Email == email);
        }


        public AuthenticateResponse Autnenticate(AuthenticateRequest model)
        {
            try
            {
                // 验证用户账号密码
                var user = _context.user_Table.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user == null)
                {
                    return null;
                }

                // 创建令牌
                var token = GenerateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            catch (Exception ex)
            {
                // 记录日志 (根据需要替换为实际日志工具)
                Console.WriteLine($"Authentication error: {ex.Message}");
                throw;
            }
        }

        //创建令牌
        private string GenerateJwtToken(User user)
        {
            byte[] key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                new Claim("sub", user.Userid.ToString()),
                new Claim("email", user.Email),
                    }
                ),
                Expires = DateTime.UtcNow.AddMinutes(360), // 设置过期时间为360分钟
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            //创建token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            byte[] key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddDays(14), // 设置过期时间为14天
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            //创建token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User GetById(int id)
        {
            return _context.user_Table.FirstOrDefault(u => u.Userid == id);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.user_Table.Find(userId);
            if (user == null)
            {
                return false; // 用户不存在
            }

            if (user.Password != currentPassword)
            {
                return false; // 当前密码不匹配
            }

            user.Password = newPassword; // 更新密码
            _context.SaveChanges();
            return true;
        }



        public bool ChangeWookbook(int userId,  string Wordbook)
        {
            var user = _context.user_Table.Find(userId);
            if (user == null)
            {
                return false; // 用户不存在
            }

        
            user.Wordbook = Wordbook; 
            _context.SaveChanges();
            return true;
        }


        public bool ChangePassword(int userId, string newPassword)
        {
            var user = _context.user_Table.Find(userId);
            if (user == null)
            {
                return false; // 用户不存在
            }

            user.Password = newPassword; // 更新密码
            _context.SaveChanges();
            return true;
        }



        public bool RegisterUser(User user, string password)
        {
            // 检查用户是否已存在
            var existingUser = _context.user_Table.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return false; // 用户已存在
            }

            // 密码加密（示例中未加密，实际应用中需要加密）
            user.Password = password;

            _context.user_Table.Add(user);
            _context.SaveChanges();
            return true;
        }

    }
}
