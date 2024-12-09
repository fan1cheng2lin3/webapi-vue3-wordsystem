using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MyWordStystemWebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningProgressController : ControllerBase
    {
        private readonly MywordDbContext _context;

        public LearningProgressController(MywordDbContext context)
        {
            _context = context;
        }

        // 获取用户ID
        private string GetUserIdFromToken()
        {
            var token = Request.Headers["Authorization"].ToString()?.Split(" ").Last();
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

        // 更新学习进度
        [HttpPost("updateProgress")]
        public async Task<IActionResult> UpdateProgress([FromBody] LearningProgress progress)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 查找该用户和单词的学习记录
            var existingProgress = await _context.progress
                .FirstOrDefaultAsync(p => p.UserId ==Convert.ToInt32(userId) && p.WordId == progress.WordId);

            if (existingProgress != null)
            {
                // 更新已有记录
                existingProgress.Score = progress.Score;
                existingProgress.Status = progress.Status;
                existingProgress.count = progress.count;
                existingProgress.lasttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 更新学习时间
            }
            else
            {

                // 插入新记录
                var newProgress = new LearningProgress
                {
                    UserId = Convert.ToInt32(userId),
                    WordId = progress.WordId,
                    Score = progress.Score,
                    Status = progress.Status,
                    count = progress.count,
                    lasttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // 设置学习时间
                };
                _context.progress.Add(newProgress);
            }


            await _context.SaveChangesAsync();
            return Ok(progress); // 返回更新后的进度数据

        }


        //获取用户词书
        [HttpGet("getprogress")]
        public async Task<IActionResult> Getprogress()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            var wordBooks = await _context.progress.Where(wb => wb.UserId == Convert.ToInt32(userId)).ToListAsync();
            return Ok(wordBooks);
        }


    }
}
