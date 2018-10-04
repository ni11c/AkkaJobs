using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Agridea.DataRepository
{
    public interface IDataRepository : IDisposable
    {
        string ConnectionString { get; }

        string DatabaseName { get; }

        IList<string> TableNames { get; }

        long GetRowCount(string tableName);

        bool Exists { get; }

        int ExecuteSqlCommand(string command, bool ensureTransaction=true);

        int ExecuteSqlCommand(string command, int timeout, bool ensureTransaction=true);

        List<T> ExecuteSqlQuery<T>(string commandText);

        bool CompatibleWithModel(bool throwIfNoMetadata);

        void Reset();

        IDataRepository Save();

        void Close();

        void Delete();

        void Detach<TItem>(TItem item) where TItem : class, IPocoBase;

        void SetCommandTimeout(int seconds);

        IQueryable<TItem> All<TItem>() where TItem : class, IPocoBase;
        IQueryable<IPocoBase> All(Type type);
        IQueryable<IPocoBase> All(Type type, string predicate, params object[] values);

        IQueryable<TItem> All<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        TItem GetFirst<TItem>() where TItem : class, IPocoBase;

        TItem GetFirstOrDefault<TItem>() where TItem : class, IPocoBase;

        bool Any<TItem>() where TItem : class, IPocoBase;

        bool Any<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        IDataRepository Add<TItem>(TItem item) where TItem : class, IPocoBase;

        IDataRepository AddRange<TItem>(IEnumerable<TItem> items) where TItem : class, IPocoBase;

        IDataRepository Modify<TItem>(TItem item) where TItem : class, IPocoBase;

        IDataRepository Remove<TItem>(TItem item) where TItem : class, IPocoBase;
        IDataRepository RemoveItem(object item);

        IEnumerable<TItem> SqlQuery<TItem>(string rawSql, params object[] parameters);
    }
}