using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.DOT.Dtos;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Services.Implmentation;
using MyWordStystemWebapi.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

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

        //删除用户词书
        [HttpDelete("deleteWordBooks")]
        public async Task<IActionResult> DeleteWordBooks([FromBody] deletewordbook WordBook)
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            await _wordbook.deleteWordBookname(Convert.ToInt32(userId), WordBook.Id);
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




        //获取用户词书
        [HttpGet("getWordBooks")]
        public async Task<IActionResult> GetWordBooks()
        {
            var userId = GetUserIdFromToken(); // 从 token 中获取用户 ID
            if (userId == null)
            {
                return Unauthorized("无效的或过期的 token"); // 如果 token 无效或过期
            }

            var wordBooks = await _context.UserWordbooks_Table.Where(wb => wb.UserId == Convert.ToInt32(userId)).ToListAsync();
            return Ok(wordBooks);
        } 
        
        


    }
}
