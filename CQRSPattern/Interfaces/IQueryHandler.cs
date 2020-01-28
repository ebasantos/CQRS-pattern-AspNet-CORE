using System;
using System.Collections.Generic;

namespace CQRSPattern.Interfaces
{
    public interface IQueryHandler
    {
        List<T> Get<T>() where T : class;
        List<T> Get<T>(Func<T, bool> where) where T : class;
        List<T> Get<T>(object parameters) where T : class;
        List<T> Get<T>(string query, object param = null) where T : class;
        List<T> Get<T>(string whereConditions) where T : class;
        T GetFirstOne<T>(string query, object param = null) where T : class;
        T GetById<T>(object id) where T : class;
        void ExecuteQuery(string query);
        void OpenTransaction();
        void CloseTransaction(bool CommitSession = true);
    }
}
