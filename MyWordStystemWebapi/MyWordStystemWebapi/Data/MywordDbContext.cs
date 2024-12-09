using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.DOT.Dtos;
using MyWordStystemWebapi.Models;

namespace MyWordStystemWebapi.Data
{
    public class MywordDbContext : DbContext
    {
        public MywordDbContext(DbContextOptions<MywordDbContext> options) : base(options)
        {
        }

        public DbSet<User> user_Table { get; set; }

        public DbSet<CiKuWord> CiKuWords { get; set; }

        public DbSet<AllWord> word { get; set; }
        
        public DbSet<LearningProgress> progress { get; set; }



        public DbSet<WordBooks> UserWordbooks_Table { get; set; }
        //public DbSet<WordBooks> UserWordbooks_Table { get; set; }



    }
}
