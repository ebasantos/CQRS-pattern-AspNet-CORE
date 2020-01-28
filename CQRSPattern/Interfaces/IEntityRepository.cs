using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CQRSPattern.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        TEntity Find(int? id) ;
        IEnumerable<TEntity> Get() ;
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) ;
        void Add(TEntity entity) ;
        void AddRange(IEnumerable<TEntity> entities) ;
        void Remove(TEntity entity) ;
        void RemoveRange(IEnumerable<TEntity> entities) ;
        void Update(TEntity entity) ;
    }
}
