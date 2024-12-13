using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.DOT.Dtos;

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

        // 获取用户词书进度并计算词书数量
        //[HttpGet("Getwordcout")]
        //public async Task<IActionResult> Getprogressw()
        //{
        //    var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
        //    if (userId == null)
        //    {
        //        return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
        //    }

        //    // 获取今天的日期字符串
        //    var todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

        //    // 计算除了今天的所有记录数量
        //    int wordCount = _context.progress
        //                            .Where(wp => wp.UserId == Convert.ToInt32(userId) && wp.lasttime.CompareTo(todayString) < 0)
        //                            .Count();

        //    return Ok(wordCount); // 直接返回数量
        //}


        //获取已学习的单词数
        [HttpGet("Getwordcoutquanbu")]
        public async Task<IActionResult> Getprogresswd()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 获取今天的日期字符串
            var todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // 计算除了今天的所有记录数量
            int wordCount = _context.progress
                                    .Where(wp => wp.UserId == Convert.ToInt32(userId) && wp.lasttime.CompareTo(todayString) != 0)
                                    .Count();

            return Ok(wordCount); // 直接返回数量
        }


        //计算一天的学习数量
        [HttpGet("Getwordcoutnowday")]
        public async Task<IActionResult> Getprogresswx()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 获取今天的日期字符串
            var todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // 计算除了今天的所有记录数量
            int wordCount = _context.progress
                                    .Where(wp => wp.UserId == Convert.ToInt32(userId) && wp.lasttime.CompareTo(todayString) > 0)
                                    .Count();

            return Ok(wordCount); // 直接返回数量
        }



        //记录用户每次关闭网页的学习时长
        [HttpPost("addstudytime")]
        public async Task<IActionResult> AddStudyTime([FromBody] StudyTimeRequest request)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 获取当天日期字符串
            var todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // 检查Adduptime是否为"0"或者转换为整型后小于60
            if (request.Adduptime == "0" ||
                (int.TryParse(request.Adduptime, out var adduptime) && adduptime < 60))
            {
                return BadRequest("学习时长无效，必须大于等于60");
            }

            // 创建新记录
            var newRecord = new Studydate_Table
            {
                UserId = Convert.ToInt32(userId),
                Day = todayString,
                Adduptime = request.Adduptime
            };

            // 将新记录添加到数据库上下文
            _context.Studydate_Table.Add(newRecord);

            // 保存修改
            await _context.SaveChangesAsync();

            return Ok("学习时长已记录");
        }



        [HttpGet("getstudytime")]
        public async Task<IActionResult> GetStudyTime()
        {
            var userId = GetUserIdFromToken(); // 获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            var todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // 直接在数据库查询中转换和过滤
            var totalSeconds = await _context.Studydate_Table
                .Where(s => s.UserId == Convert.ToInt32(userId) && s.Day == todayString)
                .Select(s =>
                    (int?)Convert.ToInt32(s.Adduptime) // 转换为整数类型，使用可空类型避免不合适的值
                )
                .Where(time => time.HasValue && time.Value >= 60) // 过滤小于 60 秒的记录
                .SumAsync(time => time.Value); // 累加有效的学习时长（秒）

            var totalMinutes = Math.Floor(totalSeconds / 60.0); // 将秒数转换为分钟并取整

            return Ok(totalMinutes); // 返回今天的总学习时长（分钟）
        }


        [HttpGet("gettotalstudytime")]
        public async Task<IActionResult> GetTotalStudyTime()
        {
            var userId = GetUserIdFromToken(); // 获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 将 Adduptime 转换为整数并过滤小于 60 秒的数据，然后进行求和操作
            var totalSeconds = await _context.Studydate_Table
                .Where(s => s.UserId == Convert.ToInt32(userId))
                .Select(s =>
                    (int?)Convert.ToInt32(s.Adduptime) // 转换为整数类型，使用可空类型避免不合适的值
                )
                .Where(time => time.HasValue && time.Value >= 60) // 过滤小于 60 秒的记录
                .SumAsync(time => time.Value); // 累加有效的学习时长（秒）

            var totalMinutes = Math.Floor(totalSeconds / 60.0); // 将秒数转换为分钟并取整

            return Ok(totalMinutes); // 返回总学习时长（分钟）
        }



        [HttpGet("getstudyalltime")]
        public async Task<IActionResult> GetStudyallTime()
        {
            var userId = GetUserIdFromToken(); // 获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 获取今天的日期
            var todayDate = DateTime.UtcNow.Date;  // 获取当前日期
                                                   // 获取过去七天的日期，包括今天
            var dateRange = Enumerable.Range(0, 7)
                                      .Select(i => todayDate.AddDays(-i).ToString("yyyy-MM-dd"))
                                      .ToList();

            // 查询数据库，按日期获取学习时长，存储到字典中
            var studyTimes = await _context.Studydate_Table
                .Where(s => s.UserId == Convert.ToInt32(userId) && dateRange.Contains(s.Day))
                .GroupBy(s => s.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    TotalSeconds = g.Sum(s => (int?)Convert.ToInt32(s.Adduptime)) ?? 0
                })
                .ToDictionaryAsync(x => x.Day, x => x.TotalSeconds);

            // 构建返回的结果
            var result = dateRange.Select(date =>
            {
                // 如果该日期在字典中没有数据，填充为0
                var totalSeconds = studyTimes.ContainsKey(date) ? studyTimes[date] : 0;
                var totalMinutes = Math.Floor(totalSeconds / 60.0); // 转换为分钟
                return new
                {
                    Date = date,
                    StudyTime = totalMinutes
                };
            }).ToList();

            return Ok(result); // 返回包括今天及前6天的学习时长（分钟）
        }







    }
}
