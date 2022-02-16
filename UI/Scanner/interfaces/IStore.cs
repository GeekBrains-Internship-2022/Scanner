using Scanner.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scanner.interfaces
{
    public interface IStore<T> where T : Entity
    {
        IEnumerable<T> GetAll();

        T GetById(Guid Id);

        T Add(T Item);

        void Update(T Item);

        void Delete(Guid Id);
    }
}
