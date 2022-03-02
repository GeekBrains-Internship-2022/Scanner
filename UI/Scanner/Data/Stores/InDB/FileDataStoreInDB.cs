﻿using Microsoft.EntityFrameworkCore;
using Scanner.interfaces;
using Scanner.Models;
using System.Collections.Generic;
using System.Linq;

namespace Scanner.Data.Stores.InDB
{
    class FileDataStoreInDB : IStore<FileData>
    {
        private readonly ScannerDB _db;

        public FileDataStoreInDB(ScannerDB db) => _db = db;

        public FileData Add(FileData Item)
        {
            //_db.Entry(Item).State = EntityState.Added;    //  С этим связи не работают
            _db.FileDatas.Add(Item);
            _db.SaveChanges();
            return Item;
        }

        public void Delete(int Id)
        {
            var item = GetById(Id);
            if (item is null) return;
            //_db.Entry(item).State = EntityState.Deleted;
            _db.Remove(item);
            _db.SaveChanges();
        }

        public IEnumerable<FileData> GetAll() => _db.FileDatas.Include(fd => fd.Document).Include(zd => zd.Document.Metadata).ToArray();

        //_db.Brands.Include(brand => brand.Products).ToDTO();


        public FileData GetById(int Id) => _db.FileDatas.SingleOrDefault(r => r.Id == Id);


        public void Update(FileData Item)
        {
            //_db.Entry(Item).State = EntityState.Modified;
            _db.Update(Item);
            _db.SaveChanges();
        }
    }
}
