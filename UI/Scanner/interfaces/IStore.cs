using Scanner.Models.Base;

using System.Collections.Generic;

namespace Scanner.interfaces
{
    public interface IStore<T> where T : Entity
    {
        IEnumerable<T> GetAll();

        T GetById(int Id);

        T Add(T Item);

        void Update(T Item);

        void Delete(int Id);
    }
}
