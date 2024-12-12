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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 WordBookView 的主键
            modelBuilder.Entity<Myciku>()
                .HasKey(wb => new { wb.UserId, wb.WordId });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Start> Start_Table { get; set; }

        public DbSet<User> user_Table { get; set; }

        public DbSet<CiKuWord> CiKuWords { get; set; }
        public DbSet<Myciku> WordBookView { get; set; }

        public DbSet<AllWord> word { get; set; }
        
        public DbSet<LearningProgress> progress { get; set; }



        public DbSet<WordBooks> UserWordbooks_Table { get; set; }
        //public DbSet<WordBooks> UserWordbooks_Table { get; set; }



    }
}
