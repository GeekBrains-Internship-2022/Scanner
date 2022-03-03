﻿using Microsoft.EntityFrameworkCore;
using Scanner.interfaces;
using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner.Data.Stores.InDB
{
    internal class DocumentMetadataInDB : IStore<DocumentMetadata>
    {

        private readonly ScannerDB _db;
        public DocumentMetadataInDB(ScannerDB db) => _db = db;
        public DocumentMetadata Add(DocumentMetadata Item)
        {
            _db.Metadata.Add(Item);
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

        public IEnumerable<DocumentMetadata> GetAll() => _db.Metadata.ToArray();

        public DocumentMetadata GetById(int Id) => _db.Metadata.SingleOrDefault(r => r.Id == Id);
        public void Update(DocumentMetadata Item)
        {
            _db.Update(Item);
            _db.SaveChanges();
        }
    }
}
