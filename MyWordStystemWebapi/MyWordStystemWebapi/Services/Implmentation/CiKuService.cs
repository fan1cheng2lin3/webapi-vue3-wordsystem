using Azure.Core;
using Microsoft.Data.SqlClient;
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
        //private readonly Dictionary<string, DbSet<CiKuWord>> _viewNameToDbSetMap;

        public CiKuService(MywordDbContext context)
        {
            _context = context;
        }

   


        public async Task<List<CiKuWord>> GetWordsByViewNameAsync(string viewName)
        {
            int pageNumber = 1;
            int pageSize = 500;

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

        public async Task<List<CiKuWord>> GetUnlearnedWordsByViewNameAsync(int userId, string viewName)
        {
            int pageNumber = 1;
            int pageSize = 500;

            if (string.IsNullOrEmpty(viewName))
            {
                Console.WriteLine("无效的视图名称");
                return new List<CiKuWord>(); // 返回空列表
            }

            try
            {
                // 使用子查询过滤 progress 表中已学习的单词
                var query = $@"
            SELECT * 
            FROM {viewName} 
            WHERE id NOT IN (
                SELECT WordId 
                FROM progress 
                WHERE UserId = @UserId
            )
            ORDER BY Id 
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

                // 使用参数化查询避免 SQL 注入
                var words = await _context.CiKuWords.FromSqlRaw(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Offset", (pageNumber - 1) * pageSize),
                    new SqlParameter("@PageSize", pageSize)).ToListAsync();

                if (!words.Any())
                {
                    Console.WriteLine($"视图 {viewName} 返回为空");
                }

                return words;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查询单词时发生错误: {ex.Message}");
                return new List<CiKuWord>(); // 返回空列表
            }
        }






        public async Task<List<Myciku>> GetStartWordsBymyViewNameAsync(int userid)
        {
            int pageNumber = 1;
            int pageSize = 500;

            // 使用 ROW_NUMBER() 来为每个 WordPre 分配行号，并过滤重复的 WordPre
            var query = $@"
        WITH CTE AS (
            SELECT *, ROW_NUMBER() OVER (PARTITION BY WordPre ORDER BY WordId) AS RowNum
            FROM WordBookView
            WHERE UserId = {userid} AND Status = 'start'
        )
        SELECT *
        FROM CTE
        WHERE RowNum = 1
        ORDER BY WordId
        OFFSET {(pageNumber - 1) * pageSize} ROWS
        FETCH NEXT {pageSize} ROWS ONLY";

            // 使用异步查询
            var words = await _context.WordBookView.FromSqlRaw(query).ToListAsync();

            return words;
        }


        public async Task<List<Myciku>> GetUnlearnedStartWordsBymyViewNameAsync(int userid)
        {
            int pageNumber = 1;
            int pageSize = 500;

            // 使用 ROW_NUMBER() 来为每个 WordPre 分配行号，并过滤重复的 WordPre
            var query = @"
        WITH CTE AS (
            SELECT *, ROW_NUMBER() OVER (PARTITION BY WordPre ORDER BY WordId) AS RowNum
            FROM WordBookView
            WHERE UserId = @UserId AND Status = 'start'
        )
        SELECT *
        FROM CTE
        WHERE RowNum = 1 AND WordId NOT IN (
            SELECT WordId 
            FROM progress 
            WHERE UserId = @UserId
        )
        ORDER BY WordId
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            // 使用异步查询和参数化查询
            var words = await _context.WordBookView.FromSqlRaw(query,
                new SqlParameter("@UserId", userid),
                new SqlParameter("@Offset", (pageNumber - 1) * pageSize),
                new SqlParameter("@PageSize", pageSize)).ToListAsync();

            return words;
        }



        //public async Task<List<Myciku>> GetAllWordsBymyViewNameAsync(int userid, string wordbook)
        //{
        //    int pageNumber = 1;
        //    int pageSize = 500;

        //    // 使用参数化查询
        //    var query = @"SELECT DISTINCT * FROM WordBookView 
        //          WHERE UserId = @UserId AND WordBookName = @WordBookName 
        //          ORDER BY WordId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //    // 使用异步查询，传入参数
        //    var words = await _context.WordBookView.FromSqlRaw(query, new SqlParameter("@UserId", userid),
        //                                                        new SqlParameter("@WordBookName", wordbook),
        //                                                        new SqlParameter("@Offset", (pageNumber - 1) * pageSize),
        //                                                        new SqlParameter("@PageSize", pageSize)).ToListAsync();

        //    return words;
        //}




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


        //public async Task updateStartWord(int UserId, int wordId,string start)
        //{
        //    var updatewordbookname = await _context.UserWordbooks_Table.FindAsync(start);
        //    if (updatewordbookname == null || updatewordbookname.UserId != UserId)
        //    {
        //        throw new Exception("not found");
        //    }

        //    updatewordbookname.collect = start;
        //    await _context.SaveChangesAsync();
        //}

        public async Task deleteWordBookname(int UserId, string WordBookname)
        {
            // 首先，查询所有匹配WordBookName的记录
            var wordBooks = await _context.UserWordbooks_Table
                                        .Where(wb => wb.WordBookName == WordBookname && wb.UserId == UserId)
                                        .ToListAsync();

            // 检查是否找到任何记录
            if (!wordBooks.Any())
            {
                throw new Exception("not found");
            }

            // 从上下文中移除找到的记录
            _context.UserWordbooks_Table.RemoveRange(wordBooks);

            // 保存更改
            await _context.SaveChangesAsync();
        }
    }
}
