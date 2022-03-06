using Microsoft.EntityFrameworkCore;

using System.Linq;

namespace Scanner.Data
{
    internal class ScannerDbInitializer
    {
        private readonly ScannerDB _db;

        public ScannerDbInitializer(ScannerDB db) => _db = db;

        public void Initialize()
        {
            _db.Database.Migrate();
#if DEBUG
            if (!_db.DataTemplates.Any())
                    _db.DataTemplates.AddRange(TestData.DataTemplates);

            _db.SaveChanges();
#endif
        }
    }
}
