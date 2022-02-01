using Microsoft.EntityFrameworkCore;
using Scanner.Models;

namespace Scanner.Data
{
    class ScannerDB : DbContext
    {
        public DbSet<ScannerDataTemplate> DataTemplates { get; set; }

        public DbSet<FileData> FileDatas { get; set; }

        public ScannerDB(DbContextOptions<ScannerDB> opt) : base(opt) { }
    }
}
