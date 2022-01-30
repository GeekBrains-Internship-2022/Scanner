using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace Scanner.Data
{
    class ScannerDBContextInitializer : IDesignTimeDbContextFactory<ScannerDB>
    {
        public ScannerDB CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScannerDB>();
            optionsBuilder.UseSqlite($"Filename=.\\ScannerDB.db");
            return new ScannerDB(optionsBuilder.Options);
        }
    }
}
