
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.DOT.Dtos;
using MyWordStystemWebapi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using MyWordStystemWebapi.Models;

using System;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace MyWordStystemWebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddWordBookController : ControllerBase
    {


        private readonly MywordDbContext _context;
        private readonly ICiKuService _wordbook;

        public AddWordBookController(MywordDbContext context, ICiKuService addwordbook)
        {
            _context = context;
            _wordbook = addwordbook;
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


        // 增加词书
        [HttpPost("AddWordBook")]
        public async Task<IActionResult> AddWordBook([FromBody] addwordbookDto WordBook)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 插入新记录
            await _wordbook.AddWordBook(Convert.ToInt32(userId), WordBook.WordBookName);
            return Ok();

        }

        [HttpPost("ImportWordsToWordBook")]
        public async Task<IActionResult> ImportWordsToWordBook([FromForm] ImportWordsDto importWordsDto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token");
            }

            if (importWordsDto.File == null || importWordsDto.File.Length == 0)
            {
                return BadRequest("未上传文件");
            }

            try
            {
                // 读取文件并分割单词
                var wordsToImport = await new StreamReader(importWordsDto.File.OpenReadStream()).ReadToEndAsync();
                var wordArray = wordsToImport
                    .ToLower() // 转换为小写
                    .Split(new[] { ' ', '\r', '\n', '\t', ',', '.', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries) // 分割单词
                    .Where(word => !string.IsNullOrWhiteSpace(word)) // 去除空字符串
                    .Distinct(); // 去重

                // 获取数据库中所有单词的小写映射
                var dbWords = await _context.word
                    .Select(w => new { w.id, WordPreLower = w.wordpre.ToLower() })
                    .ToListAsync();

                // 构造单词的字典以快速查找
                var dbWordDict = dbWords.ToDictionary(w => w.WordPreLower, w => w.id);

                // 构建需要插入的单词关系
                var userWordBooks = new List<WordBooks>();

                foreach (var word in wordArray)
                {
                    if (dbWordDict.TryGetValue(word, out var wordId))
                    {
                        // 检查是否已经存在相同的记录
                        var exists = await _context.UserWordbooks_Table.AnyAsync(uw =>
                            uw.UserId == Convert.ToInt32(userId) &&
                            uw.WordId == wordId &&
                            uw.WordBookName == importWordsDto.WordBookName);

                        if (!exists)
                        {
                            // 不存在则添加到插入列表
                            userWordBooks.Add(new WordBooks
                            {
                                UserId = Convert.ToInt32(userId),
                                WordId = wordId,
                                WordBookName = importWordsDto.WordBookName
                            });
                        }
                    }
                }

                // 批量插入需要添加的记录
                if (userWordBooks.Any())
                {
                    await _context.UserWordbooks_Table.AddRangeAsync(userWordBooks);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"成功插入 {userWordBooks.Count} 条记录");
                }
                else
                {
                    Console.WriteLine("未发现新的单词需要插入");
                }

                return Ok(new { Message = "单词导入完成", InsertedCount = userWordBooks.Count });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"导入过程中发生错误: {ex.Message}");
                return StatusCode(500, "服务器内部错误");
            }
        }




        // 增加单个单词或则修改词书
        [HttpPost("AddWords")]
        public async Task<IActionResult> UpdateProgress([FromBody] WordBooks addword)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 查找该用户和单词的学习记录
            var existingProgress = await _context.UserWordbooks_Table
                .FirstOrDefaultAsync(p => p.UserId == Convert.ToInt32(userId) && p.WordId == addword.WordId);

            if (existingProgress != null)
            {
                // 更新已有记录
                existingProgress.WordBookName = addword.WordBookName;
            }
            else
            {

                // 插入新记录
                var newword = new WordBooks
                {
                    UserId = Convert.ToInt32(userId),
                    WordId = addword.WordId,
                    WordBookName = addword.WordBookName

                };
                _context.UserWordbooks_Table.Add(newword);
            }

            await _context.SaveChangesAsync();
            return Ok(addword); // 返回更新后的进度数据

        }


        //删除用户词书
        [HttpDelete("deleteWordBooks")]
        public async Task<IActionResult> DeleteWordBooks([FromBody] deletewordbook WordBook)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            await _wordbook.deleteWordBookname(Convert.ToInt32(userId), WordBook.WordBookName);
            return Ok();

        }


        //更新词书
        [HttpPut("updateWordBookname")]
        public async Task<IActionResult> UpdateWordBookname([FromBody] updatebooknanmeDto WordBook)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            await _wordbook.updateWordBookname(Convert.ToInt32(userId), WordBook.Id, WordBook.WordBookName);
            return Ok();
        }




        //获取所有用户词书
        [HttpGet("getAllWordBooks")]
        public async Task<IActionResult> GetAllWordBooks()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            var wordBooks = await _context.UserWordbooks_Table.Where(wb => wb.UserId == Convert.ToInt32(userId)).ToListAsync();
            return Ok(wordBooks);
        }





        //获取用户词书
        [HttpGet("getWordBooks")]
        public async Task<IActionResult> GetWordBooks()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            // 获取 UserWordbooks_Table 表中 WordBookName 的所有唯一值
            var uniqueWordBookNames = await _context.UserWordbooks_Table
                .Where(wb => wb.UserId == Convert.ToInt32(userId))
                .Select(wb => wb.WordBookName)
                .Distinct()
                .ToListAsync();

            return Ok(uniqueWordBookNames);
        }


    }
}
