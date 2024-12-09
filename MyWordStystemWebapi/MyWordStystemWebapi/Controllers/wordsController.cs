using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Helpers;
using MyWordStystemWebapi.Services.Implmentation;

namespace MyWordStystemWebapi.Controllers
{
    //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyIiwiZW1haWwiOiIxMTEiLCJuYmYiOjE3MzI5NzMxNjcsImV4cCI6MTczMzA1OTU2NywiaWF0IjoxNzMyOTczMTY3fQ.Kofl6nBR3mzfqzHK3uHgfNAlqao3BENrOchTOaRLLe8
    [Route("api/[controller]")]
    [ApiController]
    public class wordsController : ControllerBase
    {

        private readonly MywordDbContext _context;
        private readonly CiKuService _ciKuService;

        public wordsController(CiKuService ciKuService,MywordDbContext context)
        {
            _ciKuService = ciKuService;
            _context = context;
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


        [HttpGet("search")]
        public async Task<IActionResult> SearchWords([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("搜索内容不能为空");
            }

            var searchResults = await _context.word
                .Where(word => word.wordpre.Contains(query) || word.explain.Contains(query) )
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

    }






}
