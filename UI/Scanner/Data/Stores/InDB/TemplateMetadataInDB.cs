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
    internal class TemplateMetadataInDB : IStore<TemplateMetadata>
    {
        private readonly ScannerDB _db;
        public TemplateMetadataInDB(ScannerDB db) => _db = db;
        public TemplateMetadata Add(TemplateMetadata Item)
        {
            _db.Entry(Item).State = EntityState.Added;
            _db.SaveChanges();
            return Item;
        }

        public void Delete(int Id)
        {
            var item = GetById(Id);
            if (item is null) return;
            _db.Entry(item).State = EntityState.Deleted;
            _db.SaveChanges();
        }

        public IEnumerable<TemplateMetadata> GetAll() => _db.TemplateMetadata.ToArray();

        public TemplateMetadata GetById(int Id) => _db.TemplateMetadata.SingleOrDefault(r => r.Id == Id);
        public void Update(TemplateMetadata Item)
        {
            _db.Entry(Item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
