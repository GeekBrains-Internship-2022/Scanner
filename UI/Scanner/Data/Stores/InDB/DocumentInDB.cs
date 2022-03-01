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
    internal class DocumentInDB : IStore<Document>
    {

        private readonly ScannerDB _db;
        public DocumentInDB(ScannerDB db) => _db = db;
        public Document Add(Document Item)
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

        public IEnumerable<Document> GetAll() => _db.Documents.ToArray();

        public Document GetById(Guid Id) => _db.Documents.SingleOrDefault(r => r.Id == Id);
        public void Update(Document Item)
        {
            _db.Entry(Item).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
