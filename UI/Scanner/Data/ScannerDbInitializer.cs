using Microsoft.EntityFrameworkCore;

namespace Scanner.Data
{
    class ScannerDbInitializer
    {
        private readonly ScannerDB _db;

        public ScannerDbInitializer(ScannerDB db) => _db = db;

        public void Initialize()
        {
            _db.Database.Migrate();
        }
    }
}
