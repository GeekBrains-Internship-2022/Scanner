using Microsoft.EntityFrameworkCore;
using Scanner.Models;

namespace Scanner.Data
{
    class ScannerDB : DbContext
    {
        public DbSet<DataTemplate> DataTemplates { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<FileData> FileDatas { get; set; }

        public ScannerDB(DbContextOptions<ScannerDB> opt) : base(opt) { }

        //protected override void OnModelCreating(ModelBuilder db)
        //{
        //    db.Entity<SchedulerTask>()
        //       .HasMany(t => t.Recipients)
        //       .WithOne("Task")
        //       .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}
