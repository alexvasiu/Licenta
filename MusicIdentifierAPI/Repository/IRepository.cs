using System;
using System.Linq;

namespace MusicIdentifierAPI.Repository
{
    public interface IRepository<TE> where TE : class
    {
        void Add(TE entity);

        void Remove(int id);

        TE Find(int id);

        TE GetLast();

        TE FirstOrDefault(Func<TE, bool> function);

        void Update(TE newEntity);

        IQueryable<TE> GetAll();

        int Size();
    }
}