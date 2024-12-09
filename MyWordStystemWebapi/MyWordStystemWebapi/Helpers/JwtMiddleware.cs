using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWordStystemWebapi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace MyWordStystemWebapi.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthSettings _authSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthSettings> authSettings)
        {
            _next = next;
            _authSettings = authSettings.Value;
        }


        //
        public async Task Invoke(HttpContext context, IUserService service)
        {
            // 从上下文里面拿到token
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                // 验证
                AttachUserToContext(context, service, token);
                // 调用下一个中间件

            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, IUserService service, string token)
        {
            // 验证
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            // 转换成 JwtSecurityToken
            var jwtToken = (JwtSecurityToken)validatedToken;
            // 获取用户 ID
            var userId = int.Parse(jwtToken.Claims.First(c => c.Type == "sub").Value);
            context.Items["User"] = service.GetById(userId);
        }
    }
}