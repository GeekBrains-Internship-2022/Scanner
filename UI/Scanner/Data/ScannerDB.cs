using Microsoft.EntityFrameworkCore;
using Scanner.Models;

namespace Scanner.Data
{
    class ScannerDB : DbContext
    {
        public DbSet<ScannerDataTemplate> DataTemplates { get; set; }

        public DbSet<FileData> FileDatas { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentMetadata> Metadata { get; set; }

        public DbSet<TemplateMetadata> TemplateMetadata { get; set; }

        public ScannerDB(DbContextOptions<ScannerDB> opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Document>()
            //.HasMany(c => c.Metadata);

            //modelBuilder.Entity<DocumentMetadata>()
            //    .HasOne(p => p.Metadata)
            //    .WithMany(b => b.Posts)
            //    .HasForeignKey(p => p.BlogForeignKey);
        }
    }
}
