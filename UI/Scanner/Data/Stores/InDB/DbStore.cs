using Microsoft.EntityFrameworkCore;

using Scanner.interfaces;
using Scanner.Models.Base;

using System.Collections.Generic;

namespace Scanner.Data.Stores.InDB
{
    internal class DbStore<T> : IStore<T> where T : Entity
    {
        private readonly ScannerDB _Db;
        private DbSet<T> Set { get; }

        public DbStore(ScannerDB db)
        {
            _Db = db;
            Set = db.Set<T>();
        }

        public IEnumerable<T> GetAll() => Set;

        public T GetById(int id) => Set.Find(id);

        public T Add(T item)
        {
            _Db.Add(item);
            _Db.SaveChanges();

            return item;
        }

        public void Update(T item)
        {
            _Db.Update(item);
            _Db.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            _Db.Remove(item);

            _Db.SaveChanges();
        }
    }
}
