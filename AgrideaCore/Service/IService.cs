using Agridea.DataRepository;
using Agridea.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SortDirection = System.Web.Helpers.SortDirection;

namespace Agridea.Service
{
    public interface IService : IDisposable
    {
        string UserName { get; set; }

        string ConnectionString { get; }

        IList<string> GetTableNames();

        long GetRowCount(string tableName);
        bool Exists { get; }

        void Reset();

        bool SaveAndResetEvery(int saveCount);

        void SetCommandTimeout(int seconds);

        IService Save();

        void Delete();

        void Detach<TItem>(TItem item) where TItem : class, IPocoBase;

        int ExecuteSqlCommands(string sqlCommands, bool ensureTransaction = true);

        int ExecuteSqlCommands(string sqlCommands, int timeout, bool ensureTransaction = true);

        List<T> ExecuteSqlQuery<T>(string commandText);

        bool CompatibleWithModel(bool throwIfNoMetadata);


        int GetMax<TPoco>(Expression<Func<TPoco, int>> property) where TPoco : class, IPocoBase;

        IQueryable<TPoco> All<TPoco>() where TPoco : class, IPocoBase;
        IQueryable<IPocoBase> All(Type type);
        IQueryable<IPocoBase> All(Type type, string predicate, params object[] values);

        IQueryable<TPoco> All<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase;

        IList<TItem> GetAll<TItem>() where TItem : class, IPocoBase;

        IList<TItem> GetAllWithoutTracking<TItem>() where TItem : class, IPocoBase;
            
        IList<TItem> GetAll<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        IList<TPoco> GetAllWithoutTracking<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase;

        /// <summary>
        /// Peformance penalty : use carefully
        /// </summary>
        IList<TItem> GetAllWithEagerReferences<TItem>() where TItem : class, IPocoBase;

        /// <summary>
        /// Peformance penalty : use carefully
        /// </summary>
        IList<TItem> GetAllWithEagerReferences<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        IList<TPoco> GetAll<TPoco, TSortProperty>(out int totalCount, Expression<Func<TPoco, TSortProperty>> orderingProperty, SortDirection sortDirection, int skipCount, int takeCount) where TPoco : class, IPocoBase;

        IList<TPoco> GetAll<TPoco, TSortProperty>(Expression<Func<TPoco, bool>> predicate, Expression<Func<TPoco, TSortProperty>> orderingProperty, SortDirection sortDirection, int page, int pageSize, out int totalCount) where TPoco : class, IPocoBase;

        IList<TPoco> GetAll<TPoco>(out int totalCount, string ordering) where TPoco : class, IPocoBase;

        IList<TPoco> GetAll<TPoco>(out int totalCount, string ordering, int skipCount, int takeCount) where TPoco : class, IPocoBase;

        TViewModel GetAutoCompleteById<TPoco, TViewModel>(int id)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new();

        AutoCompleteEntity<TPoco, TViewModel> GetMappedPaginatedFor<TPoco, TViewModel>(string query, int page, int pageSize, Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new();

        AutoCompleteEntity<TPoco, TViewModel> GetMappedPaginatedFor<TPoco, TViewModel>(IQueryable<TPoco> queryable, string query, int page, int pageSize)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new();


        IList<TPoco> GetPaginated<TPoco>(Expression<Func<TPoco, bool>> predicate, Expression<Func<TPoco, object>> orderingProperty, SortDirection sortDirection, int page, int pageSize, out int totalCount)
            where TPoco : class, IPocoBase;

        bool IsUniqueFor<TPoco>(Expression<Func<TPoco, bool>> predicate, TPoco instance) where TPoco : class, IPocoBase;

        TItem GetFirst<TItem>() where TItem : class, IPocoBase;

        TItem GetFirst<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        TItem GetFirstOrDefault<TItem>() where TItem : class, IPocoBase;

        TItem GetFirstOrDefault<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        TItem GetIncludeFirstOrDefault<TItem>(Expression<Func<TItem, bool>> predicate, params Expression<Func<TItem, object>>[] includes) where TItem : class, IPocoBase;

        TItem GetFirstOrDefaultWithReferences<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        TItem GetById<TItem>(int id) where TItem : class, IPocoBase;

        /// <summary>
        /// Peformance penalty : use carefully
        /// </summary>
        TItem GetEagerById<TItem>(int id) where TItem : class, IPocoBase;

        /// <summary>
        /// Peformance penalty : use carefully
        /// </summary>
        TItem GetEagerFirstOrDefault<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        PocoBase GetPocoById(Type pocoType, int id);

        TViewModel GetMappedById<TPoco, TViewModel>(int id)
            where TPoco : class, IPocoBase
            where TViewModel : class, new();

        IList<TViewModel> GetAllMapped<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class;

        TViewModel GetMappedFirstOrDefault<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate)
            where TPoco : class, IPocoBase
            where TViewModel : class;

        IQueryable<TViewModel> AllMapped<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class;

        TItem GetIncludeById<TItem>(int id, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase;

        IList<TItem> GetIncludeAll<TItem>(Expression<Func<TItem, bool>> predicate, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase;

        IList<TItem> GetIncludeAllWithoutTracking<TItem>(Expression<Func<TItem, bool>> predicate, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase;

        IList<TItem> GetAllEager<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        IList<TItem> GetItemsCreatedOrModifiedBefore<TItem>(DateTime dateTime) where TItem : class, IPocoBase;

        TItem GetByDiscriminant<TItem>(TItem item) where TItem : class, IPocoBase;

        TItem GetByDiscriminant<TItem>(TItem item, IQueryable<TItem> items) where TItem : class, IPocoBase;

        TItem GetByDiscriminant<TItem>(TItem item, IList<TItem> items) where TItem : class, IPocoBase;

        int Count<TItem>() where TItem : class, IPocoBase;

        int Count<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        bool Any<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase;

        int GetSum<TItem>(Expression<Func<TItem, bool>> predicate, Expression<Func<TItem, int>> property) where TItem : class, IPocoBase;

        IService Add<TItem>(TItem item) where TItem : class, IPocoBase;

        IService AddOrUpdate<TPoco>(TPoco poco) where TPoco : class, IPocoBase;

        IService AddRange<TItem>(IEnumerable<TItem> items) where TItem : class, IPocoBase;

        IService AddRange<TItem>(params TItem[] items) where TItem : class, IPocoBase;

        IService AddOrUpdateById<TItem>(TItem item) where TItem : class, IPocoBase;

        IService AddOrUpdateIncludingCollectionsById<TItem>(TItem item) where TItem : class, IPocoBase;

        IService AddOrUpdateByDiscriminant<TItem>(TItem item) where TItem : class, IPocoBase;

        IService Modify<TItem>(TItem item) where TItem : class, IPocoBase;

        IService Remove<TItem>(TItem item) where TItem : class, IPocoBase;
        IService RemoveItem(object item);

        /**/

        IService RemoveRange<TItem>(IEnumerable<TItem> item) where TItem : class, IPocoBase;

        void CascadeRemove<TItem>(TItem item) where TItem : class, IPocoBase;

        void CascadeRemoveRange<TItem>(IEnumerable<TItem> item) where TItem : class, IPocoBase;

        void SqlInsert<TItem>(IEnumerable<TItem> item) where TItem : class, IPocoBase;

        void SqlUpdate<TItem>(IEnumerable<TItem> item) where TItem : class, IPocoBase;

        void SqlDelete<TItem>(IEnumerable<TItem> item) where TItem : class, IPocoBase;

        void SqlDropAllTables();
        void SqlDropTable<TItem>() where TItem : class, IPocoBase;

        void SqlDropTable(string tableName); 
        void SqlDeleteTable<TItem>() where TItem : class, IPocoBase;

        void SqlDeleteTable(string tableName);

        void SqlDeleteAndReseedTable<TItem>(int reseedValue = 0) where TItem : class, IPocoBase;

        TItem LoadNavigationProperties<TItem>(TItem item, bool manageCollections = false) where TItem : class, IPocoBase;

        TPoco LoadAndCopyFrom<TPoco>(TPoco destination, TPoco source, bool manageCollections = false) where TPoco : class, IPocoBase;

        TItem LoadOrNullifyNavigationProperties<TItem>(TItem item) where TItem : class, IPocoBase;

        TPoco LoadOrNullifySpecificNavigationProperties<TPoco>(TPoco poco, params Expression<Func<TPoco, object>>[] expressions) where TPoco : class, IPocoBase;

        void UpdateCollectionProperties<TPoco, TProperty>(TPoco poco, Expression<Func<TPoco, ICollection<TProperty>>> pocoCollection, IList<TProperty> sourceList)
            where TPoco : class, IPocoBase
            where TProperty : class, IPocoBase;
        void UpdateCollectionProperties<TPoco, TProperty>(TPoco poco, Expression<Func<TPoco, ICollection<TProperty>>> pocoCollection, IEnumerable<int> sourceIds)
            where TPoco : class, IPocoBase
            where TProperty : class, IPocoBase;
        /**/

        TItem CloneCopy<TItem>(TItem item) where TItem : class, IPocoBase, new();

        void SqlBulkCopyFor<TItem>(IList<TItem> listItems) where TItem : class, IPocoBase;

        IEnumerable<TDto> SqlQuery<TDto>(string rawSql, params object[] parameters);


        IEnumerable<SelectListItem> ComboFor<T>(Expression<Func<T, bool>> predicate, int selectedValue = 0, Func<T, string> alternateText = null, bool useDefaultValue = true, string defaultText = null)
            where T : class, ICombo;

        IEnumerable<SelectListItem> ComboFor<T>(int selectedValue = 0)
            where T : class, ICombo;

        IEnumerable<SelectListItemWithCustomData> CustomComboFor<T>(Expression<Func<T, bool>> predicate, Func<T, object> customData, int selectedValue = 0, Func<T, string> alternateText = null, bool useDefaultValue = true)
            where T : class, ICombo;
    }
}