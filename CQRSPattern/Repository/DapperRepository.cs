using CQRSPattern.Interfaces;
using CQRSPattern.Utils;
using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Z.Dapper.Plus;

namespace CQRSPattern.Repository
{
    public class DapperRepository : IDisposable, IDapperRepository
    {
        private SqlConnection connection;
        private SqlTransaction transaction;
        private bool ConnectionContinuous;
        public string ConnectionString { get; private set; }
        public void StartConnection(string connectionString, bool connectionContinuous = true)
        {
            connection = new SqlConnection(connectionString);
            ConnectionString = connectionString;
            ConnectionContinuous = connectionContinuous;

            if (connectionContinuous)
                connection.Open();
        }
        public DapperRepository()
        {
        }
        public string GetConnection() => ConnectionString;
        public virtual long Save<T>(T entity) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var rowsAffected = connection.Insert<T>(entity);


            if (!ConnectionContinuous)
                connection.Close();

            return rowsAffected;
        }
        public virtual bool Update<T>(T entity) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var rowsAffected = connection.UpdateAsync<T>(entity);

            if (!ConnectionContinuous)
                connection.Close();

            return rowsAffected.GetAwaiter().GetResult();
        }
        public virtual void Update<T>(IList<T> entities) where T : class
        {
            try
            {

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                connection.BulkUpdate<T>(entities);


                if (!ConnectionContinuous)
                    connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //TODO: MELHORAR LOGICA PARA INSERÇÃO MAIS PERFORMÁTICA (SEM FOREACH)
        public virtual void Save<T>(IEnumerable<T> entities) where T : class
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                foreach (var entity in entities)
                    connection.Insert(entity);


                if (!ConnectionContinuous)
                    connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual List<T> Get<T>() where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.GetAll<T>().ToList();


            if (!ConnectionContinuous)
                connection.Close();

            return result;
        }
        public virtual List<T> Get<T>(string query, object param = null) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.Query<T>(query, param, commandTimeout: 100000).ToList();


            if (!ConnectionContinuous)
                connection.Close();

            return result;

        }
        public virtual T GetFirstOne<T>(string query, object param) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.QueryFirstOrDefault<T>(query, param);

            if (!ConnectionContinuous)
                connection.Close();

            return result;
        }
        //TODO ADICIONAR OPERADOR PARA WHERE
        public virtual List<T> Get<T>(object parameters) where T : class
        {
            var tableName = "";
            string schema = "dbo";
            var attr = Attribute.GetCustomAttributes(typeof(T)).Where(p => p is Dapper.Contrib.Extensions.TableAttribute).FirstOrDefault();
            var attrSystem = Attribute.GetCustomAttributes(typeof(T)).Where(p => p is System.ComponentModel.DataAnnotations.Schema.TableAttribute).FirstOrDefault();

            schema = ((System.ComponentModel.DataAnnotations.Schema.TableAttribute)attrSystem)?.Schema ?? "dbo";
            tableName = ((TableAttribute)attr)?.Name ?? ((System.ComponentModel.DataAnnotations.Schema.TableAttribute)attrSystem)?.Name;

            var query = $"SELECT * FROM {tableName} ";

            if (parameters != null)
            {
                query += $"  WHERE {parameters.GetType().GetProperties()[0].Name} = @{parameters.GetType().GetProperties()[0].Name}";

                for (var i = 1; i < parameters.GetType().GetProperties().Count(); i++)
                    query += $"  AND {parameters.GetType().GetProperties()[i].Name} = @{parameters.GetType().GetProperties()[i].Name}";
            }


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.Query<T>(query, parameters).ToList();

            if (!ConnectionContinuous)
                connection.Close();

            return result;
        }
        public virtual List<T> Get<T>(Func<T, bool> where) where T : class
        {
            return Get<T>().Where(where).ToList();
        }
        public virtual List<T> Get<T>(string whereConditions) where T : class
        {
            var tableName = "";
            string schema = "dbo";
            var attrDapper = Attribute.GetCustomAttributes(typeof(T)).Where(p => p is Dapper.Contrib.Extensions.TableAttribute).FirstOrDefault();
            var attrSystem = Attribute.GetCustomAttributes(typeof(T)).Where(p => p is System.ComponentModel.DataAnnotations.Schema.TableAttribute).FirstOrDefault();

            schema = ((System.ComponentModel.DataAnnotations.Schema.TableAttribute)attrSystem)?.Schema ?? schema;
            tableName = ((TableAttribute)attrDapper)?.Name ?? ($"[{schema}].[{((System.ComponentModel.DataAnnotations.Schema.TableAttribute)attrSystem)?.Name}]");

            var query = $"SELECT * FROM [{schema}].[{tableName}] {whereConditions}";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.Query<T>(query).ToList();

            if (!ConnectionContinuous)
                connection.Close();

            return result;
        }
        public virtual T GetById<T>(object id) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.GetAsync<T>(id);

            if (!ConnectionContinuous)
                connection.Close();

            return result.GetAwaiter().GetResult();

        }
        public virtual void BulkList<T>(string tableDestination, List<T> list) where T : class
        {
            try
            {
                DataTable dt = list.ConvertToDataTable();

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (SqlBulkCopy bc = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bc.BulkCopyTimeout = 120 * 1000;
                    bc.DestinationTableName = tableDestination;
                    bc.WriteToServer(dt);

                    FieldInfo rowsCopiedField = typeof(SqlBulkCopy).GetField("_rowsCopied", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                }

                if (!ConnectionContinuous)
                    connection.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public virtual void ExecuteQuery(string query)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            connection.Execute(query, null, transaction);

            if (!ConnectionContinuous)
                connection.Close();
        }
        public void OpenTransaction()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            transaction = connection.BeginTransaction();

        }
        public void CloseTransaction(bool CommitSession = true)
        {
            try
            {
                if (CommitSession)
                    transaction.Commit();
                else
                    transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }
        public virtual bool Delete<T>(T entity) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var result = connection.DeleteAsync<T>(entity);

            if (!ConnectionContinuous)
                connection.Close();

            return result.GetAwaiter().GetResult();
        }
        public virtual void DeleteAll<T>(IList<T> list) where T : class
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            connection.BulkDelete<T>(list);

            if (!ConnectionContinuous)
                connection.Close();
        }
        public void SaveChanges()
        {
            connection.Dispose();

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (transaction?.Connection != null)
                transaction.Commit();
        }
        public void Dispose()
        {
            SaveChanges();
        }
    }
}
