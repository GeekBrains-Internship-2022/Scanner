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
            _db.Entry(Item).State = EntityState.Added;            
            _db.SaveChanges();
            return Item;
        }

        public void Delete(Guid Id)
        {
            var item = GetById(Id);
            if (item is null) return;            
            _db.Entry(item).State = EntityState.Deleted;
            _db.SaveChanges();
        }

        public IEnumerable<ScannerDataTemplate> GetAll() => _db.DataTemplates.Include(fd => fd.TemplateMetadata).ToArray();


        public ScannerDataTemplate GetById(Guid Id) => _db.DataTemplates.SingleOrDefault(r => r.Id == Id);

        public void Update(ScannerDataTemplate Item)
        {
            _db.Entry(Item).State = EntityState.Modified;            
            _db.SaveChanges();
        }
    }
}
