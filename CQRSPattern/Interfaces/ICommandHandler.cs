using System.Collections.Generic;

namespace CQRSPattern.Interfaces
{
    public interface ICommandHandler<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void SaveChanges();
    }
}
