using System;
using System.Collections.Generic;

namespace MusicIdentifierAPI.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly Dictionary<string, object> _repositories;

        public UnitOfWork()
        {
            _context = new DatabaseContext();
            _repositories = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public GenericRepository<TE> GetRepository<TE>() where TE : class
        {
            var typeKey = typeof(TE).Name;

            if (_repositories.ContainsKey(typeKey)) return _repositories[typeKey] as GenericRepository<TE>;
            var instance = new GenericRepository<TE>(_context);
            _repositories.Add(typeKey, instance);
            return _repositories[typeKey] as GenericRepository<TE>;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}