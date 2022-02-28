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
    class DocumentStoreInDB : IStore<Document>
    {
        private readonly ScannerDB _db;

        public DocumentStoreInDB(ScannerDB dB)
        {
            _db = dB;
        }

        public Document Add(Document Item)
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

        public IEnumerable<Document> GetAll() => _db.Documents.Include(d => d.Metadata).ToArray();

        public Document GetById(int Id) => _db.Documents.SingleOrDefault(d => d.Id == Id);

        public void Update(Document Item)
        {
            _db.Entry(Item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
