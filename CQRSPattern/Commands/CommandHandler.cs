using CQRSPattern.Commands.Repository;
using CQRSPattern.Commands.UOW;
using CQRSPattern.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CQRSPattern.Commands
{
    public class CommandHandler<TEntity> : ICommandHandler<TEntity>,  IDisposable  where TEntity : class
    {
        private IRepository<TEntity> _repo { get; set; }
        private IUnitOfWork _uow { get; set; }

        public CommandHandler(DbContext context)
        {
            _repo = new Repository<TEntity>(context);
            _uow = new UnitOfWork(context);
        }

        public void Add(TEntity entity) => _repo.Add(entity);
        public void AddRange(IEnumerable<TEntity> entities) => _repo.AddRange(entities);
        public void Remove(TEntity entity) => _repo.Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => _repo.RemoveRange(entities);
        public void Update(TEntity entity) => _repo.Update(entity);
        public void SaveChanges() => _uow.Commit();

        public void Dispose()
        {
            SaveChanges();
        }
    }
}
