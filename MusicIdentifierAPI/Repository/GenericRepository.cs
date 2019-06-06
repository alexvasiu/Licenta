using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MusicIdentifierAPI.Repository
{
    public class GenericRepository<TE> : IRepository<TE> where TE : class
    {
        private readonly DbSet<TE> _objects;
        private readonly DatabaseContext _dataContext;

        public GenericRepository(DatabaseContext databaseContext)
        {
            _objects = databaseContext.Set<TE>();
            _dataContext = databaseContext;
        }

        public void Add(TE entity)
        {
            _objects.Add(entity);
        }

        public void Remove(int id)
        {
            var item = Find(id);
            var entity = _dataContext.Entry(item);
            entity.State = EntityState.Deleted;
        }

        public TE Find(int id)
        {
            return _objects.Find(id);
        }

        public void Update(TE newEntity)
        {
            var entity = _dataContext.Entry(newEntity);
            entity.State = EntityState.Modified;
        }

        public IQueryable<TE> GetAll()
        {
            return _objects;
        }

        public int Size()
        {
            return _objects.Count();
        }

        public TE GetLast()
        {
            return _objects.Last();
        }

        public TE FirstOrDefault(Func<TE, bool> function)
        {
            return _objects.FirstOrDefault(function);
        }
    }
}