using Azure.Core;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.DOT.Dtos;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace MyWordStystemWebapi.Services.Implmentation
{
    public class CiKuService: ICiKuService
    {
        private readonly MywordDbContext _context;
        private readonly Dictionary<string, DbSet<CiKuWord>> _viewNameToDbSetMap;

        public CiKuService(MywordDbContext context)
        {
            _context = context;
        }

   


        public async Task<List<CiKuWord>> GetWordsByViewNameAsync(string viewName, int pageNumber = 1, int pageSize = 50)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Console.WriteLine("无效的视图名称");
                return new List<CiKuWord>(); // 返回空列表
            }

            var query = $"SELECT * FROM {viewName} ORDER BY Id OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            // 使用异步查询
            var words = await _context.CiKuWords.FromSqlRaw(query).ToListAsync();
            if (!words.Any())
            {
                Console.WriteLine($"视图 {viewName} 返回为空");
            }
            return words;

        }


        public async Task AddWordBook(int UserId, string WordBookName)
        {
          

            var addwordbook = new WordBooks
            {
                UserId = UserId,
                WordBookName = WordBookName
            };
            await _context.UserWordbooks_Table.AddAsync(addwordbook);
            await _context.SaveChangesAsync();
        }


        //public async Task AddWord(int UserId, int WordId, string WordBookName)
        //{
        //    var updatewordbookname = await _context.UserWordbooks_Table.FindAsync(WordBookName);

        //    if (updatewordbookname == null)
        //    {

        //        updatewordbookname.WordId = WordId;
               
        //    }
        //    await _context.UserWordbooks_Table.AddAsync(addwordbook);
        //    await _context.SaveChangesAsync();
        //}





        public async Task updateWordBookname(int UserId, int WordBookId, string WordBookName)
        {
            var updatewordbookname = await _context.UserWordbooks_Table.FindAsync(WordBookId);
            if (updatewordbookname == null || updatewordbookname.UserId != UserId)
            {
                throw new Exception("not found");
            }

            updatewordbookname.WordBookName = WordBookName;
            await _context.SaveChangesAsync();
        }


        public async Task deleteWordBookname(int UserId, int WordBookId)
        {
           

            var wordBooks = await _context.UserWordbooks_Table.FindAsync(WordBookId);
            if (wordBooks == null || wordBooks.UserId != UserId)
            {
                throw new Exception("not found");
            }

            _context.UserWordbooks_Table.Remove(wordBooks);
            await _context.SaveChangesAsync();


        }
    }
}
