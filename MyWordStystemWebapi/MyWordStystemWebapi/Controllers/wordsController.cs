
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.DOT.Dtos;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace MyWordStystemWebapi.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class wordsController : ControllerBase
    {

        private readonly MywordDbContext _context;
        private readonly ICiKuService _ciKuService;

        public wordsController(ICiKuService ciKuService,MywordDbContext context)
        {
            _ciKuService = ciKuService;
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


        [HttpGet("GetWordsByViewName/{viewName}")]
        public async Task<ActionResult<List<CiKuWord>>> GetWordsByViewName(string viewName)
        {
            var words = await _ciKuService.GetWordsByViewNameAsync(viewName); // 使用await等待异步结果
            if (words == null || !words.Any())
            {
                return NotFound($"没有找到视图名称为 {viewName} 的数据");
            }
            return Ok(words);
        }


        [HttpGet("GetUnlearnedWordsByViewName/{viewName}")]
        public async Task<ActionResult<List<CiKuWord>>> GetUnlearnedWordsByViewName(string viewName)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            var words = await _ciKuService.GetUnlearnedWordsByViewNameAsync(Convert.ToInt32(userId), viewName);
            if (words == null || !words.Any())
            {
                return NotFound($"没有找到视图名称为 {viewName} 的未学习单词");
            }

            return Ok(words);
        }




        [HttpGet("GetUnlearnedAllWordsBymyViewName/{viewName}")]
        public async Task<ActionResult<List<Myciku>>> GetUnlearnedAllWordsBymyViewNameAsync(string viewName)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            try
            {
                Console.WriteLine($"User ID from token: {userId}");

                // 查询未学习的单词
                var words = await _context.WordBookView
                    .Where(wb => wb.WordBookName == viewName &&
                                 !(_context.progress
                                   .Where(p => p.UserId == Convert.ToInt32(userId))
                                   .Select(p => p.WordId)
                                   .Contains(wb.WordId))) // 排除已学习的单词
                    .OrderBy(wb => wb.WordId)
                    .Distinct()
                    .ToListAsync();

                if (!words.Any())
                {
                    Console.WriteLine("未找到符合条件的单词");
                    return Ok(new List<Myciku>()); // 如果没有符合条件的单词，返回空列表
                }

                return Ok(words); // 返回未学习单词列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查询未学习单词失败: {ex.Message}");
                return StatusCode(500, "服务器内部错误，请稍后再试"); // 捕获异常并返回500状态码
            }
        }




 


        [HttpGet("GetAllWordsBymyViewName/{viewName}")]
        public async Task<ActionResult<List<Myciku>>> GetAllWordsBymyViewNameAsync(string viewName)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }
            Console.WriteLine($"User ID from token: {userId}");

            // 直接使用 DbContext 进行查询，获取指定视图名称的所有单词
            var words = await _context.WordBookView
                .Where(wb => wb.UserId == Convert.ToInt32(userId) && wb.WordBookName == viewName)
                .Distinct()
                .ToListAsync();

            return Ok(words); // 返回查询结果
        }



        [HttpGet("GetStartWordsBymyViewName")]
        public async Task<ActionResult<List<Myciku>>> GetStartWordsBymyViewNameAsync()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
             if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }
            Console.WriteLine($"User ID from token: {userId}");

            var words = await _ciKuService.GetStartWordsBymyViewNameAsync(Convert.ToInt32(userId)); // 使用await等待异步结果
           
            return Ok(words);
        }


        [HttpGet("GetUnlearnedStartWordsBymyViewName")]
        public async Task<ActionResult<List<Myciku>>> GetUnlearnedStartWordsBymyViewNameAsync()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }
            Console.WriteLine($"User ID from token: {userId}");

            var words = await _ciKuService.GetUnlearnedStartWordsBymyViewNameAsync(Convert.ToInt32(userId)); // 使用await等待异步结果

            return Ok(words);
        }



        [HttpGet("search")]
        public async Task<IActionResult> SearchWords([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("搜索内容不能为空");
            }

            // 获取所有单词，并对它们进行排序和筛选
            var searchResults = await _context.word
                .Where(word => word.wordpre.Contains(query) || word.explain.Contains(query))
                .OrderByDescending(word => word.wordpre.StartsWith(query))  // 首先按是否是前缀匹配排序
                .ThenByDescending(word => word.wordpre.Length)  // 长度更长的前缀匹配放前面
                .ThenBy(word => word.wordpre)  // 按字母顺序排序
                .Take(500)  // 最多返回50个结果
                .ToListAsync();


            if (searchResults.Any())
            {
                return Ok(searchResults);

            }

            else
            {
                return NotFound("未找到匹配的单词");
            }
        }


        //// 增加收藏
        [HttpPost("AddstartWord")]
        public async Task<IActionResult> AddstartWord([FromBody] Start det)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 查找该用户和单词的学习记录
            var existingstart = await _context.Start_Table
                .FirstOrDefaultAsync(p => p.UserId == Convert.ToInt32(userId) && p.WordId == det.WordId);

            if (existingstart != null)
            {

                // 如果已经收藏，则取消收藏
                if (existingstart.Status == "start")
                {
                    existingstart.Status = "unstart";
                }
                // 如果未收藏，则添加收藏
                else
                {
                    existingstart.Status = "start";
                }
            }
            else
            {
                // 插入新记录
                var newstart = new Start
                {
                    UserId = Convert.ToInt32(userId),
                    WordId = det.WordId,
                    Status = "start",
                };
                _context.Start_Table.Add(newstart);
            }

            await _context.SaveChangesAsync();
            return Ok(det); // 返回更新后的进度数据

        }


        //[HttpGet("similar")]
        //public async Task<IActionResult> GetSimilarWords([FromQuery] string word)
        //{
        //    if (string.IsNullOrWhiteSpace(word))
        //    {
        //        return BadRequest("输入的单词不能为空");
        //    }

        //    // 获取所有单词，排除当前输入的单词
        //    var allWords = await _context.word
        //        .Where(w => w.wordpre != word)
        //        .ToListAsync();

        //    if (!allWords.Any())
        //    {
        //        return NotFound("没有找到其他单词");
        //    }

        //    // 计算相似度并排序
        //    var similarWords = allWords
        //        .Select(w => new
        //        {
        //            Word = w.wordpre,
        //            Similarity = GetSimilarity(word, w.wordpre) // 自定义相似度计算方法
        //        })
        //        .OrderByDescending(x => x.Similarity)
        //        .Take(4)
        //        .Select(x => x.Word)
        //        .ToList();

        //    // 如果相似单词不足4个，随机补充
        //    if (similarWords.Count < 4)
        //    {
        //        var randomWords = allWords
        //            .Where(w => !similarWords.Contains(w.wordpre)) // 排除已选的相似单词
        //            .OrderBy(_ => Guid.NewGuid()) // 随机排序
        //            .Take(4 - similarWords.Count)
        //            .Select(w => w.wordpre)
        //            .ToList();

        //        similarWords.AddRange(randomWords);
        //    }

        //    return Ok(similarWords);
        //}

        //// 相似度计算方法：这里简单用Levenshtein距离
        //private int GetSimilarity(string word1, string word2)
        //{
        //    int n = word1.Length;
        //    int m = word2.Length;
        //    int[,] dp = new int[n + 1, m + 1];

        //    for (int i = 0; i <= n; i++) dp[i, 0] = i;
        //    for (int j = 0; j <= m; j++) dp[0, j] = j;

        //    for (int i = 1; i <= n; i++)
        //    {
        //        for (int j = 1; j <= m; j++)
        //        {
        //            int cost = (word1[i - 1] == word2[j - 1]) ? 0 : 1;
        //            dp[i, j] = Math.Min(
        //                Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
        //                dp[i - 1, j - 1] + cost
        //            );
        //        }
        //    }

        //    return n + m - dp[n, m]; // 相似度：越大越相似
        //}




        //[HttpPost("calculate-similarities")]
        //public async Task<IActionResult> CalculateSimilarities()
        //{
        //    // 获取所有单词
        //    var allWords = await _context.word.ToListAsync();

        //    if (!allWords.Any())
        //    {
        //        return NotFound("单词表为空，无法计算相似单词。");
        //    }

        //    foreach (var targetWord in allWords)
        //    {
        //        // 过滤掉当前单词自身，获取其他单词
        //        var candidateWords = allWords
        //            .Where(w => w.wordpre != targetWord.wordpre)
        //            .ToList();

        //        // 计算相似度
        //        var similarWords = candidateWords
        //            .Select(w => new
        //            {
        //                Word = w.wordpre,
        //                Similarity = GetSimilarity(targetWord.wordpre, w.wordpre)
        //            })
        //            .OrderByDescending(x => x.Similarity)
        //            .Take(4) // 取前4个最相似的单词
        //            .Select(x => x.Word)
        //            .ToList();

        //        // 更新 similar 列
        //        targetWord.similar1 = similarWords.ElementAtOrDefault(0);
        //        targetWord.similar2 = similarWords.ElementAtOrDefault(1);
        //        targetWord.similar3 = similarWords.ElementAtOrDefault(2);
        //        targetWord.similar4 = similarWords.ElementAtOrDefault(3);
        //    }

        //    // 保存所有更新
        //    await _context.SaveChangesAsync();

        //    return Ok("所有单词的相似项计算并更新完成。");
        //}


    }






}
