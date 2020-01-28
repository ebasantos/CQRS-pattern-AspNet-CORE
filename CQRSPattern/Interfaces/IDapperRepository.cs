using System;
using System.Collections.Generic;

namespace CQRSPattern.Interfaces
{
    public interface IDapperRepository
    {
        string GetConnection();
        void StartConnection(string connectionString, bool connectionContinuous = true);
        long Save<T>(T entity) where T : class;
        void Save<T> (IEnumerable<T> entities) where T : class;
        List<T> Get<T>() where T : class;
        List<T> Get<T>(Func<T, bool> where) where T : class;
        List<T> Get<T>(object parameters) where T : class;
        List<T> Get<T>(string query, object param = null) where T : class;
        List<T> Get<T>(string whereConditions) where T : class;
        T GetFirstOne<T>(string query, object param = null) where T : class;
        void BulkList<T>(string tableDestination, List<T> list) where T : class;
        void OpenTransaction();
        void CloseTransaction(bool CommitSession = true);
        T GetById<T>(object id) where T : class;
        void ExecuteQuery(string query);
        bool Delete<T>(T entity) where T : class;
        void DeleteAll<T>(IList<T> list) where T : class;
        bool Update<T>(T entity) where T : class;
        void Update<T>(IList<T> entities) where T : class;
        void SaveChanges();
    }
}
