using System;
using System.Collections.Generic;
using CQRSPattern.Interfaces;
using CQRSPattern.Repository;

namespace CQRSPattern.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private IDapperRepository _repo { get; set; }

        public QueryHandler(string connectionString)
        {
            _repo = new DapperRepository();
            _repo.StartConnection(connectionString);
        }

        public List<T> Get<T>() where T : class => _repo.Get<T>();

        public List<T> Get<T>(Func<T, bool> where) where T : class => _repo.Get(where);

        public List<T> Get<T>(object parameters) where T : class => _repo.Get<T>(parameters);

        public List<T> Get<T>(string query, object param = null) where T : class => _repo.Get<T>(query, param);

        public List<T> Get<T>(string whereConditions) where T : class => _repo.Get<T>(whereConditions);

        public T GetFirstOne<T>(string query, object param = null) where T : class => _repo.GetFirstOne<T>(query, param);

        public T GetById<T>(object id) where T : class => _repo.GetById<T>(id);

        public void ExecuteQuery(string query) => _repo.ExecuteQuery(query);

        public void OpenTransaction() => _repo.OpenTransaction();

        public void CloseTransaction(bool CommitSession = true) => _repo.CloseTransaction();
    }
}
