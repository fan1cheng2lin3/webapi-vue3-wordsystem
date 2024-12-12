
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

        // 导入单词到词书
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

            // 日志：调试输入内容
            Console.WriteLine($"Received WordBookName: {importWordsDto.WordBookName}");
            Console.WriteLine($"File Length: {importWordsDto.File.Length}");

            var wordsToImport = await new StreamReader(importWordsDto.File.OpenReadStream()).ReadToEndAsync();
            var wordArray = wordsToImport
                .ToLower()
                .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => word.Length > 0);

            foreach (var word in wordArray)
            {
                var wordEntity = await _context.word.FirstOrDefaultAsync(w => w.wordpre == word);
                if (wordEntity != null)
                {
                    // 检查是否已经存在相同的 UserId, WordId, WordBookName 组合
                    var existingEntry = await _context.UserWordbooks_Table
                        .FirstOrDefaultAsync(uw => uw.UserId == Convert.ToInt32(userId) && uw.WordId == wordEntity.id && uw.WordBookName == importWordsDto.WordBookName);

               

                    // 如果没有找到相同的记录，则插入
                    if (existingEntry == null)
                    {
                        var userWordBookEntity = new WordBooks
                        {
                            UserId = Convert.ToInt32(userId),
                            WordId = wordEntity.id,
                            WordBookName = importWordsDto.WordBookName
                        };

                        // 打印日志查看是否准备插入数据
                        Console.WriteLine($"Inserting WordId: {wordEntity.id}, UserId: {userId}, WordBookName: {importWordsDto.WordBookName}");
                        await _context.UserWordbooks_Table.AddAsync(userWordBookEntity);
                    }
                    else
                    {
                        // 打印日志查看重复记录
                        Console.WriteLine($"Duplicate entry found for WordId: {wordEntity.id}, UserId: {userId}, WordBookName: {importWordsDto.WordBookName}");
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
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
