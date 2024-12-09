using System.IdentityModel.Tokens.Jwt;

namespace MyWordStystemWebapi.Helpers
{
    public class GetUID
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUID(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // 获取用户ID
        public string GetUserIdFromToken()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return null;
            }

            // 获取用户的 claim 信息
            var userIdClaim = jsonToken?.Claims?.FirstOrDefault(c => c.Type == "sub");
            return userIdClaim?.Value; // 返回用户 ID
        }



    }
}
