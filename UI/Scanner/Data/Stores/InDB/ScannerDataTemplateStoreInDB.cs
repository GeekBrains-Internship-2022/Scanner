using Microsoft.EntityFrameworkCore;
using Scanner.interfaces;
using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Data.Stores.InDB
{
    class ScannerDataTemplateStoreInDB : IStore<ScannerDataTemplate>
    {
        private readonly ScannerDB _db;

        public ScannerDataTemplateStoreInDB(ScannerDB db) => _db = db;

        public ScannerDataTemplate Add(ScannerDataTemplate Item)
        {
            _db.DataTemplates.Add(Item);
            _db.SaveChanges();
            return Item;
        }

        public void Delete(int Id)
        {
            var item = GetById(Id);
            if (item is null) return;
            _db.Remove(item);
            _db.SaveChanges();
        }

        public IEnumerable<ScannerDataTemplate> GetAll() => _db.DataTemplates.Include(fd => fd.TemplateMetadata).ToArray();

        public ScannerDataTemplate GetById(int Id) => _db.DataTemplates.SingleOrDefault(r => r.Id == Id);

        public void Update(ScannerDataTemplate Item)
        {      
            _db.Update(Item);
            _db.SaveChanges();
        }
    }
}
