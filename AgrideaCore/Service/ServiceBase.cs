using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.ObjectMapping;
using Agridea.Resources;
using Agridea.Service.Repository;
using Agridea.Web.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Agridea.Service
{
    [DbConfigurationType(typeof(AgrideaCoreDbConfiguration))]
    public abstract class ServiceBase : Disposable, IService
    {
        #region Constants

        private const char CsvSeparator = ';';
        private const char CsvPropertyValueSeparator = ',';
        private const char CsvCollectionValueSeparator = '|';
        private const string CsvEmptyCell = "";
        public static readonly string GetByIdMethodName = "GetById";
        public static readonly string GetByDiscriminantMethodName = "GetByDiscriminant";
        public static readonly string GetAllMethodName = "GetAll";
        public static readonly string AddMethodName = "Add";
        public static readonly string InsertMethodName = "Insert";
        private static readonly string DateFormat = "yyyy.MM.dd HH:mm:ss.fff";
        private static readonly string SqlEncodeFormat = "'{0}'";
        private static readonly string NULL = "NULL";
        private static readonly string SingleQuote = "'";
        private static readonly string EscapedSingleQuote = "''";

        #endregion Constants

        #region Members

        private IDataRepository repository_;
        private int saveCount_;

        #endregion Members

        #region Initialization

        public ServiceBase(IDataRepository repository)
        {
            Log.Verbose(String.Format("'{0}:{1}' New(...)", GetType().Name, GetHashCode()));
            repository_ = repository;
        }

        #endregion Initialization

        #region Identity

        public override string ToString()
        {
            return String.Format("[{0} Repository='{1}' ]",
                GetType().Name,
                repository_.ToString());
        }

        #endregion Identity

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            Log.Verbose(String.Format("'{0}:{1}' Dispose({2})", GetType().Name, GetHashCode(), disposing));
            if (disposing)
                repository_.Dispose();
        }

        #endregion IDisposable

        #region IService
        public string UserName { get; set; }
        public string ConnectionString { get { return repository_.ConnectionString; } }
        public IList<string> GetTableNames() // accessible thru SMO also...
        {
            //var query = "select name from sys.objects join information_schema.tables on table_name = name where table_type = 'BASE TABLE' order by name asc";
            //var pocos = ExecuteSqlQuery<string>(query);
            //return pocos;
            return repository_.TableNames;
        }

        public long GetRowCount(string tableName)
        {
            return repository_.GetRowCount(tableName);
        }


        public bool Exists
        {
            get { return Repository.Exists; }
        }
        public void Reset()
        {
            saveCount_ = 0;
            repository_.Reset();
        }
        public bool SaveAndResetEvery(int everySaveCount)
        {
            if (saveCount_ < everySaveCount)
            {
                saveCount_++;
                return false;
            }
            Save();
            Reset();
            return true;
        }
        public void SetCommandTimeout(int seconds)
        {
            Repository.SetCommandTimeout(seconds);
        }
        public IService Save()
        {
            try
            {
                repository_.Save();
                return this;
            }
            catch (DbEntityValidationException exception)
            {
                Log.Error(exception);
                throw exception;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw exception;
            }
        }
        public void Delete()
        {
            repository_.Delete();
        }

        public void Detach<TItem>(TItem item) where TItem : class, IPocoBase
        {
            repository_.Detach(item);
        }

        public int ExecuteSqlCommands(string sqlCommands, bool ensureTransaction = true)
        {
            return Repository.ExecuteSqlCommand(sqlCommands, ensureTransaction);
        }
        public int ExecuteSqlCommands(string sqlCommands, int timeout, bool ensureTransaction=true)
        {
           return Repository.ExecuteSqlCommand(sqlCommands, timeout, ensureTransaction);
        }
        public List<T> ExecuteSqlQuery<T>(string commandText)
        {
            return Repository.ExecuteSqlQuery<T>(commandText);
        }
        public bool CompatibleWithModel(bool throwIfNoMetadata)
        {
            return Repository.CompatibleWithModel(throwIfNoMetadata);
        }

        #region Get

        public int GetMax<TPoco>(Expression<Func<TPoco, int>> intProperty) where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().Max(intProperty);
        }

        public IQueryable<TPoco> All<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>();
        }
        public IQueryable<IPocoBase> All(Type type)
        {
            return repository_.All(type);
        }
        public IQueryable<IPocoBase> All(Type type, string predicate, params object[] values)
        {
            return repository_.All(type, predicate, values);
        }

        public IQueryable<TPoco> All<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return repository_.All(predicate);
        }

        public IList<TPoco> GetAll<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().ToList();
        }

        public IList<TPoco> GetAllWithoutTracking<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().AsNoTracking().ToList();
        }

        public IList<TPoco> GetAll<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return repository_.All(predicate).ToList();
        }

        public IList<TPoco> GetAllWithoutTracking<TPoco>(Expression<Func<TPoco, bool>> predicate ) where TPoco : class, IPocoBase
        {
            return repository_.All(predicate).AsNoTracking().ToList();
        }

        public IList<TPoco> GetAllWithEagerReferences<TPoco>() where TPoco : class, IPocoBase
        {
            return GetAllWithEagerReferences<TPoco>(x => true);
        }

        public IList<TPoco> GetAllWithEagerReferences<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            var queryable = repository_.All(predicate);
            var includes = typeof(TPoco).GetReferenceProperties().Select(x => x.Name);
            queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            return queryable.ToList();
        }

        public IList<TPoco> GetAll<TPoco, TSortProperty>(out int totalCount, Expression<Func<TPoco, TSortProperty>> orderingProperty, SortDirection sortDirection, int skipCount, int takeCount) where TPoco : class, IPocoBase
        {
            totalCount = Count<TPoco>();
            var queryable = repository_.All<TPoco>();
            var ordered = sortDirection == SortDirection.Ascending
                ? queryable.OrderBy(orderingProperty)
                : queryable.OrderByDescending(orderingProperty);
            return ordered
                .Skip(skipCount)
                .Take(takeCount)
                .ToList();
        }

        public IList<TPoco> GetAll<TPoco, TSortProperty>(Expression<Func<TPoco, bool>> predicate, Expression<Func<TPoco, TSortProperty>> orderingProperty, SortDirection sortDirection, int page, int pageSize, out int totalCount)
            where TPoco : class, IPocoBase
        {
            var filtered = repository_.All(predicate);
            totalCount = filtered.Count();

            var ordered = sortDirection == SortDirection.Ascending ?
                filtered.OrderBy(orderingProperty) :
                filtered.OrderByDescending(orderingProperty);

            return (pageSize > 0 && page >= 1) ?
                ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList() :
                ordered.ToList();
        }

        public IList<TPoco> GetAll<TPoco>(out int totalCount, string ordering) where TPoco : class, IPocoBase
        {
            totalCount = Count<TPoco>();
            return repository_.All<TPoco>()
                .OrderBy(ordering)
                .ToList();
        }

        public IList<TPoco> GetAll<TPoco>(out int totalCount, string ordering, int skipCount, int takeCount) where TPoco : class, IPocoBase
        {
            totalCount = Count<TPoco>();
            return repository_.All<TPoco>()
                .OrderBy(ordering)
                .Skip(skipCount)
                .Take(takeCount)
                .ToList();
        }

        public TViewModel GetAutoCompleteById<TPoco, TViewModel>(int id)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new()
        {
            var viewModel = GetMappedById<TPoco, TViewModel>(id);
            return viewModel ?? new TViewModel();
        }

        public AutoCompleteEntity<TPoco, TViewModel> GetMappedPaginatedFor<TPoco, TViewModel>(string query, int page, int pageSize, Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new()
        {
            if (predicate == null)
                predicate = m => true;

            var elements = Repository.All(predicate);
            var model = new TViewModel();
            var res = model.Filter(elements, query)
                           .Map<TPoco, TViewModel>();
            var count = res.Count();
            var results = res
                .Skip(Math.Max(page - 1, 0) * pageSize)
                .Take(pageSize)
                .ToList();
            return new AutoCompleteEntity<TPoco, TViewModel>
            {
                Results = results.ToList(),
                Total = count
            };
        }

        public AutoCompleteEntity<TPoco, TViewModel> GetMappedPaginatedFor<TPoco, TViewModel>(IQueryable<TPoco> queryable, string query, int page, int pageSize)
            where TPoco : class, IPocoBase
            where TViewModel : class, IAutoComplete<TPoco>, new()
        {
            var elements = queryable;
            var model = new TViewModel();
            var res = model.Filter(elements, query)
                           .Map<TPoco, TViewModel>();
            var count = res.Count();
            var results = res
                .Skip(Math.Max(page - 1, 0) * pageSize)
                .Take(pageSize)
                .ToList();
            return new AutoCompleteEntity<TPoco, TViewModel>
            {
                Results = results.ToList(),
                Total = count
            };
        }

        public IList<TPoco> GetPaginated<TPoco>(Expression<Func<TPoco, bool>> predicate, Expression<Func<TPoco, object>> orderingProperty, SortDirection sortDirection, int page, int pageSize, out int totalCount)
            where TPoco : class, IPocoBase
        {
            var filtered = Repository.All<TPoco>().Where(predicate);
            totalCount = filtered.Count();

            var ordered = sortDirection == SortDirection.Ascending
                              ? filtered.OrderBy(orderingProperty)
                              : filtered.OrderByDescending(orderingProperty);

            return (pageSize > 0 && page >= 1)
                       ? ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList()
                       : ordered.ToList();
        }

        public bool IsUniqueFor<TPoco>(Expression<Func<TPoco, bool>> predicate, TPoco instance) where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().Where(m => m.Id != instance.Id)
                              .Where(predicate)
                              .FirstOrDefault() == null;
        }

        public int Count<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().Count();
        }

        public int Count<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().Where(predicate).Count();
        }

        public bool Any<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase
        {
            if (predicate == null)
                predicate = x => true;

            return repository_.Any(predicate);
        }

        public int GetSum<TItem>(Expression<Func<TItem, bool>> predicate, Expression<Func<TItem, int>> property) where TItem : class, IPocoBase
        {
            var queryable = Repository.All<TItem>().Where(predicate);
            return queryable.Any() ? queryable.Sum(property) : 0;
        }

        public TPoco GetFirst<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.GetFirst<TPoco>();
        }

        public TPoco GetFirst<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().First(predicate);
        }

        public TPoco GetFirstOrDefault<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().FirstOrDefault(predicate);
        }

        public TPoco GetIncludeFirstOrDefault<TPoco>(Expression<Func<TPoco, bool>> predicate, params Expression<Func<TPoco, object>>[] includes) where TPoco : class, IPocoBase
        {
            return AllIncluding(includes).FirstOrDefault(predicate);
        }

        public TPoco GetFirstOrDefault<TPoco>() where TPoco : class, IPocoBase
        {
            return repository_.GetFirstOrDefault<TPoco>();
        }

        public TPoco GetFirstOrDefaultWithReferences<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return IncludeReferences<TPoco>().FirstOrDefault(predicate);
        }

        public TViewModel GetMappedById<TPoco, TViewModel>(int id)
            where TPoco : class, IPocoBase
            where TViewModel : class, new()
        {
            return repository_.All<TPoco>(x => x.Id == id)
                                    .Map<TPoco, TViewModel>()
                                    .FirstOrDefault();
        }

        public IList<TViewModel> GetAllMapped<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class
        {
            if (predicate == null) predicate = x => true;

            return repository_.All<TPoco>(predicate)
                              .Map<TPoco, TViewModel>()
                              .ToList();
        }

        public IQueryable<TViewModel> AllMapped<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate = null)
            where TPoco : class, IPocoBase
            where TViewModel : class
        {
            predicate = predicate ?? (x => true);
            return All(predicate).Map<TPoco, TViewModel>();
        }

        public TViewModel GetMappedFirstOrDefault<TPoco, TViewModel>(Expression<Func<TPoco, bool>> predicate)
            where TPoco : class, IPocoBase
            where TViewModel : class
        {
            return Repository.All(predicate)
                             .Map<TPoco, TViewModel>()
                             .FirstOrDefault();
        }

        public TPoco GetById<TPoco>(int id) where TPoco : class, IPocoBase
        {
            Asserts<InvalidKeyException>.GreaterOrEqual(id, 0);
            return repository_.All<TPoco>().FirstOrDefault(m => m.Id == id);
        }

        public PocoBase GetPocoById(Type pocoType, int id)
        {
            Requires<ArgumentException>.IsTrue(pocoType.IsSubclassOf(typeof(PocoBase)));
            return GetType().GenericGetByIdMethod(pocoType).Invoke(this, new object[] { id }) as PocoBase;
        }

        public TPoco GetEagerById<TPoco>(int id) where TPoco : class, IPocoBase
        {
            return IncludeReferences<TPoco>().FirstOrDefault(x => x.Id == id);
        }

        public TPoco GetEagerFirstOrDefault<TPoco>(Expression<Func<TPoco, bool>> predicate) where TPoco : class, IPocoBase
        {
            return IncludeReferences<TPoco>().FirstOrDefault(predicate);
        }

        public TItem GetIncludeById<TItem>(int id, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase
        {
            return AllIncluding(paths).FirstOrDefault(m => m.Id == id);
        }

        public IList<TItem> GetIncludeAll<TItem>(Expression<Func<TItem, bool>> predicate, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase
        {
            return AllIncluding(paths).Where(predicate).ToList();
        }

        public IList<TItem> GetIncludeAllWithoutTracking<TItem>(Expression<Func<TItem, bool>> predicate, params Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase
        {
            return AllIncluding(paths).Where(predicate).AsNoTracking().ToList();
        }

        public IList<TItem> GetAllEager<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase
        {
            var queryable = repository_.All(predicate);
            queryable = typeof(TItem).GetNavigationProperties().Select(x => x.Name).Aggregate(queryable, (current, navigationProperty) => current.Include(navigationProperty));

            queryable = queryable.OrderBy(m => m.Id);
            return queryable.ToList();
        }

        public IList<TPoco> GetItemsCreatedOrModifiedBefore<TPoco>(DateTime dateTime)
            where TPoco : class, IPocoBase
        {
            return repository_.All<TPoco>().Where(item =>
                (item.ModificationDate != null && item.ModificationDate < dateTime) ||
                (item.ModificationDate == null && item.CreationDate < dateTime)).ToList();
        }

        public TPoco GetByDiscriminant<TPoco>(TPoco item) where TPoco : class, IPocoBase
        {
            return GetByDiscriminant(item, repository_.All<TPoco>());
        }

        public TPoco GetByDiscriminant<TPoco>(TPoco item, IQueryable<TPoco> items) where TPoco : class, IPocoBase
        {
            var itemType = typeof(TPoco);
            items = GetQueryForDiscriminantProperties(items, item);
            Asserts<InvalidOperationException>.LessOrEqual(items.Count(), 1, String.Format("GetByDiscriminant<{0}>({0} item) returns more than one instance", itemType.Name));
            return items.FirstOrDefault();
        }

        public TPoco GetByDiscriminant<TPoco>(TPoco item, IList<TPoco> itemList) where TPoco : class, IPocoBase
        {
            var itemType = typeof(TPoco);
            itemList = GetQueryForDiscriminantProperties(itemList.AsQueryable(), item).ToList();
            Asserts<InvalidOperationException>.LessOrEqual(itemList.Count(), 1, String.Format("GetByDiscriminant<{0}>({0} item) returns more than one instance", itemType.Name));
            return itemList.SingleOrDefault();
        }

        #endregion Get

        #region Add

        public IService Add<TPoco>(TPoco poco) where TPoco : class, IPocoBase
        {
            poco.CreatedBy = UserName;
            poco.CreationDate = DateTime.Now;
            repository_.Add(poco);
            return this;
        }

        public IService AddOrUpdate<TPoco>(TPoco poco) where TPoco : class, IPocoBase
        {
            if (poco.Id == 0)
                repository_.Add(poco);
            else
                Modify(poco);

            return this;
        }

        public IService AddRange<TPoco>(IEnumerable<TPoco> pocos) where TPoco : class, IPocoBase
        {
            foreach (var item in pocos) Add(item);
            return this;
        }

        public IService AddRange<TItem>(params TItem[] items) where TItem : class, IPocoBase
        {
            return AddRange(items.AsEnumerable());
        }

        public IService AddOrUpdateById<TPoco>(TPoco poco) where TPoco : class, IPocoBase
        {
            return AddOrUpdate(poco, x => GetEagerById<TPoco>(x.Id));
        }

        public IService AddOrUpdateIncludingCollectionsById<TPoco>(TPoco poco) where TPoco : class, IPocoBase
        {
            return AddOrUpdate(poco, x => GetById<TPoco>(x.Id), manageCollections: true);
        }

        public IService AddOrUpdateByDiscriminant<TPoco>(TPoco poco) where TPoco : class, IPocoBase
        {
            return AddOrUpdate(poco, GetByDiscriminant);
        }

        private IService AddOrUpdate<TPoco>(TPoco poco, Func<TPoco, TPoco> get, bool manageCollections = false) where TPoco : class, IPocoBase
        {
            var existing = get(poco);
            return existing == null ?
                Add(LoadNavigationProperties(poco, manageCollections)) :
                Modify(LoadAndCopyFrom(existing, poco, manageCollections));
        }

        #endregion Add

        #region Load

        //TODO : rename LoadOrCreateNavigationProperties (by preserve a backward compatible version tagged Obsolete)
        public TPoco LoadNavigationProperties<TPoco>(TPoco poco, bool manageCollections = false) where TPoco : class, IPocoBase
        {
            typeof(TPoco)
                .GetReferenceProperties().ToList()
                .ForEach(propertyInfo => LoadNavigationProperty(poco, propertyInfo));

            if (manageCollections)
                typeof(TPoco)
                    .GetCollectionProperties().ToList()
                    .ForEach(propertyInfo => LoadListNavigationProperty(poco, propertyInfo));

            return poco;
        }

        //TODO : Refactor without Reflection

        public TPoco LoadNavigationProperty<TPoco>(TPoco poco, PropertyInfo propertyInfo) where TPoco : class, IPocoBase
        {
            var propertyValue = propertyInfo.GetValue(poco) as PocoBase;

            propertyInfo.SetValue(
                poco,
                propertyValue == null ?
                    null :
                    GetPocoById(propertyInfo.PropertyType, propertyValue.Id) ?? propertyValue);

            return poco;
        }

        public TPoco LoadListNavigationProperty<TPoco>(TPoco poco, PropertyInfo propertyInfo) where TPoco : class, IPocoBase
        {
            var pocotype = propertyInfo.GetInnerTypeFromGenericList();
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(pocotype));
            var pocoList = propertyInfo.GetValue(poco) as IEnumerable<PocoBase>;
            Requires<InvalidCastException>.IsNotNull(pocoList);

            pocoList.ToList().ForEach(item => list.Add(GetPocoById(pocotype, item.Id)));
            propertyInfo.SetValue(poco, list);
            return poco;
        }

        public void CleanListNavigationProperty<TPoco>(TPoco poco, PropertyInfo propertyInfo)
        {
            var pocoList = propertyInfo.GetValue(poco) as IEnumerable<PocoBase>;
            Requires<InvalidCastException>.IsNotNull(pocoList);
            pocoList.ToList().Clear();
        }

        public TPoco LoadAndCopyFrom<TPoco>(TPoco destination, TPoco source, bool manageCollections = false) where TPoco : class, IPocoBase
        {
            foreach (var propertyInfo in source.GetType().GetPublicPropertiesWithVirtualSetters())
            {
                if (propertyInfo.IsReference())
                    LoadNavigationProperty(source, propertyInfo);

                if (propertyInfo.IsCollection() && manageCollections)
                {
                    LoadListNavigationProperty(source, propertyInfo);
                    CleanListNavigationProperty(destination, propertyInfo);
                }

                destination.CopyPropertyFrom(source, propertyInfo);
            }
            return destination;
        }

        public void UpdateCollectionProperties<TPoco, TProperty>(TPoco poco, Expression<Func<TPoco, ICollection<TProperty>>> pocoCollection, IList<TProperty> sourceList)
            where TPoco : class, IPocoBase
            where TProperty : class, IPocoBase
        {
            UpdateCollectionProperties(poco, pocoCollection, sourceList.Select(m => m.Id));
        }

        public void UpdateCollectionProperties<TPoco, TProperty>(TPoco poco, Expression<Func<TPoco, ICollection<TProperty>>> pocoCollection, IEnumerable<int> sourceIds)
            where TPoco : class, IPocoBase
            where TProperty : class, IPocoBase
        {
            var collectionPropertyInfo = typeof(TPoco).GetProperty((pocoCollection.Body as MemberExpression).Member.Name);
            var pocoList = (IList<TProperty>)collectionPropertyInfo.GetValue(poco, null);
            var pocoIds = pocoList.Select(m => m.Id).ToList();
            foreach (var sourceId in sourceIds)
            {
                if (!pocoIds.Contains(sourceId))
                {
                    pocoList.Add(GetById<TProperty>(sourceId));
                    continue;
                }
                pocoIds.Remove(sourceId);
            }
            foreach (var id in pocoIds)
                pocoList.Remove(GetById<TProperty>(id));
        }

        public TPoco LoadOrNullifyNavigationProperties<TPoco>(TPoco item) where TPoco : class, IPocoBase
        {
            typeof(TPoco)
                .GetReferenceProperties().ToList()
                .ForEach(propertyInfo => LoadOrNullifyNavigationProperty(item, propertyInfo));

            //To load items in collections
            LoadNavigationProperties(item, manageCollections: true);

            return item;
        }

        public TPoco LoadOrNullifyNavigationProperty<TPoco>(TPoco poco, PropertyInfo propertyInfo) where TPoco : class, IPocoBase
        {
            var propertyValue = propertyInfo.GetValue(poco) as PocoBase;

            propertyInfo.SetValue(poco, propertyValue == null
                                            ? null
                                            : GetPocoById(propertyInfo.PropertyType, propertyValue.Id)
                );

            return poco;
        }

        public TPoco LoadOrNullifySpecificNavigationProperties<TPoco>(TPoco poco, params Expression<Func<TPoco, object>>[] expressions) where TPoco : class, IPocoBase
        {
            foreach (var expression in expressions)
            {
                var name = ExpressionHelper.GetExpressionText(expression);
                var property = typeof(TPoco).GetProperty(name);
                if (property != null && typeof(TPoco).GetReferenceProperties().Select(m => m.Name).Contains(property.Name))
                    LoadOrNullifyNavigationProperty(poco, property);
            }
            return poco;
        }

        #endregion Load

        #region Update
        public IService Modify<TPoco>(TPoco item) where TPoco : class, IPocoBase
        {
            item.ModifiedBy = UserName;
            item.ModificationDate = DateTime.Now;
            repository_.Modify(item);
            return this;
        }

        #endregion Update

        #region Remove

        public IService Remove<TPoco>(TPoco item) where TPoco : class, IPocoBase
        {
            repository_.Remove(item);
            return this;
        }
        public IService RemoveItem(object item)
        {
            repository_.RemoveItem(item);
            return this;
        }

        public IService RemoveRange<TPoco>(IEnumerable<TPoco> items) where TPoco : class, IPocoBase
        {
            foreach (var item in items) Remove<TPoco>(item);
            return this;
        }

        public void CascadeRemove<TPoco>(TPoco item) where TPoco : class, IPocoBase
        {
            repository_.Remove(item);
        }

        public void CascadeRemoveRange<TPoco>(IEnumerable<TPoco> items) where TPoco : class, IPocoBase
        {
            foreach (var item in items) CascadeRemove<TPoco>(item);
        }

        public void SqlInsert<TPoco>(IEnumerable<TPoco> items) where TPoco : class, IPocoBase
        {
        }

        public void SqlUpdate<TPoco>(IEnumerable<TPoco> items) where TPoco : class, IPocoBase
        {
        }

        public void SqlDelete<TPoco>(IEnumerable<TPoco> items) where TPoco : class, IPocoBase
        {
            if (!items.Any()) return;

            var stringBuilder = new StringBuilder();
            var tableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", CodeGenerationHelper.GetTableName<TPoco>());
            foreach (var item in items.Select(m => m.Id))

                stringBuilder.AppendFormat("DELETE FROM {0} WHERE Id = {1}{2}", tableName, item, Environment.NewLine);

            var command = stringBuilder.ToString();

            repository_.ExecuteSqlCommand(command);
        }

        public void SqlDelete<TPoco>(Expression<Func<TPoco, int>> foreignKeyExpression, int value) where TPoco : class, IPocoBase
        {
            var member = foreignKeyExpression.Body as MemberExpression;
            Asserts<ArgumentException>.IsNotNull(member);
            var memberName = member.ToString();
            Asserts<ArgumentException>.AreEqual(3, memberName.Split('.'), "foreignKeyExpression should be something like m.NavigationProperty.Id");
            Asserts<ArgumentException>.IsTrue(memberName.EndsWith(".Id"), "foreignKeyExpression should end with .Id");

            var foreignKeyName = string.Join("_", memberName.Split('.').Skip(1));
            var tableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", CodeGenerationHelper.GetTableName<TPoco>());

            repository_.ExecuteSqlCommand(string.Format("DELETE FROM {0} WHERE {1} = {2}", tableName, foreignKeyName, value));
        }

        public void SqlDeleteTable<TPoco>() where TPoco : class, IPocoBase
        {
            var tableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", CodeGenerationHelper.GetTableName<TPoco>());
            var command = string.Format("DELETE FROM {0}", tableName);

            repository_.ExecuteSqlCommand(command);
        }

        public void SqlDeleteTable(string tableName)
        {
            var fullTableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", tableName);
            var command = string.Format("DELETE FROM {0}", fullTableName);

            repository_.ExecuteSqlCommand(command);
        }

        public void SqlDropAllTables()
        {
            //Must specify do not ensure transaction and this will
            //delete the tablea with no dependee but will complain  for others 
            //because the deletion order is not the proper one : reverse to usage
            // 'Could not drop object 'dbo.XXX' because it is referenced by a FOREIGN KEY constraint'
            //Hence repeat until there is no more error, there will be no more table in the DB
           var command = string.Format("exec sp_MSforeachtable 'DROP TABLE ?'");

            Log.Verbose("Dropping all tables");
            bool error;
            do
            {
                try
                {
                    error = false;
                    repository_.ExecuteSqlCommand(command, ensureTransaction: false);
                }
                catch (Exception)
                {
                    error = true;
                }

            } while (error);
        }
        public void SqlDropTable<TPoco>() where TPoco : class, IPocoBase
        {
            var tableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", CodeGenerationHelper.GetTableName<TPoco>());
            var command = string.Format("IF object_id('{0}') is not null drop table {0}", tableName);

            Log.Verbose("Dropping table '{0}'", tableName);
            repository_.ExecuteSqlCommand(command);
        }

        public void SqlDropTable(string tableName)
        {
            var fullTableName = string.Format("[{0}].[{1}].[{2}]", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", tableName);
            var command = string.Format("IF object_id('{0}') is not null drop table {0}", fullTableName);

            Log.Verbose("Dropping table '{0}'", tableName);
            repository_.ExecuteSqlCommand(command);
        }

        public void SqlDeleteAndReseedTable<TPoco>(int reseedValue = 0) where TPoco : class, IPocoBase
        {
            var tableName = string.Format("'{0}.{1}.{2}'", DataRepositoryHelper.DatabaseNameFor(ConnectionString), "dbo", CodeGenerationHelper.GetTableName<TPoco>());
            SqlDeleteTable<TPoco>();
            var command = string.Format("DBCC CHECKIDENT ({0}, reseed, {1})", tableName, reseedValue);

            repository_.ExecuteSqlCommand(command);
        }

        #endregion Remove

        #region Copy

        public TPoco CloneCopy<TPoco>(TPoco item) where TPoco : class, IPocoBase, new()
        {
            var clone = new TPoco();

            foreach (var primitivePropertyInfo in typeof(TPoco).GetPrimitiveProperties())
                primitivePropertyInfo.SetValue(clone, primitivePropertyInfo.GetValue(item, null), null);

            foreach (var referencePropertyInfo in typeof(TPoco).GetReferenceProperties())
                SetReference(item, clone, referencePropertyInfo);

            foreach (var collectionPropertyInfo in typeof(TPoco).GetCollectionProperties())
            {
                LoadListNavigationProperty(item, collectionPropertyInfo);
                CleanListNavigationProperty(clone, collectionPropertyInfo);
                clone.CopyPropertyFrom(item, collectionPropertyInfo);
            }

            return clone;
        }

        public void SqlBulkCopyFor<TItem>(IList<TItem> list) where TItem : class, IPocoBase
        {
            using (var bulkCopy = new SqlBulkCopy(ConnectionString))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = CodeGenerationHelper.GetTableName<TItem>();

                var table = new DataTable();
                var props = typeof(TItem).GetPrimitiveIncludingPocoBase().ToList();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);

                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Count()];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        public IEnumerable<TDto> SqlQuery<TDto>(string rawSql, params object[] parameters)
        {
            return repository_.SqlQuery<TDto>(rawSql, parameters);
        }

        #endregion Copy

        #region ComboFor

        public IEnumerable<SelectListItem> ComboFor<T>(int selectedValue = 0) where T : class, ICombo
        {
            return ComboFor<T>(m => true, selectedValue);
        }

        public IEnumerable<SelectListItem> ComboFor<T>(Expression<Func<T, bool>> predicate, int selectedValue = 0, Func<T, string> alternateText = null, bool useDefaultValue = true, string defaultText = null)
            where T : class, ICombo
        {
            var query = Repository.All(predicate);
            var result = query.ToList()
                              .Select(m => new
                              {
                                  Value = m.Id,
                                  Text = alternateText != null ? alternateText(m) : m.ComboText,
                                  OrderBy = m.SortFunc(m)
                              })
                              .OrderBy(m => m.OrderBy)
                              .Select(m => new SelectListItem
                              {
                                  Value = m.Value.ToString(),
                                  Text = m.Text,
                                  Selected = m.Value == selectedValue
                              });
            if (useDefaultValue)
                result = result.WithDefaultZeroValue(defaultText ?? AgrideaCoreStrings.WebComboChoose);
            return result;
        }

        public IEnumerable<SelectListItemWithCustomData> CustomComboFor<T>(Expression<Func<T, bool>> predicate, Func<T, object> customData, int selectedValue = 0, Func<T, string> alternateText = null, bool useDefaultValue = true)
            where T : class, ICombo
        {
            var query = Repository.All(predicate);
            var result = query.ToList()
                              .Select(m => new
                              {
                                  Value = m.Id,
                                  Text = alternateText != null ? alternateText(m) : m.ComboText,
                                  OrderBy = m.SortFunc(m),
                                  Data = customData(m)
                              })
                              .OrderBy(m => m.OrderBy)
                              .Select(m => new SelectListItemWithCustomData
                              {
                                  Value = m.Value.ToString(),
                                  Text = m.Text,
                                  Selected = m.Value == selectedValue,
                                  Data = m.Data
                              }).ToList();

            if (useDefaultValue)
                result.Insert(0, new SelectListItemWithCustomData { Value = "0", Text = AgrideaCoreStrings.WebComboChoose });

            return result.AsEnumerable();
        }

        #endregion ComboFor

        #endregion IService

        #region Helpers

        protected IDataRepository Repository { get { return repository_; } }

        private void SetReference<TPoco>(TPoco item, TPoco returnedInstance, PropertyInfo pi)
            where TPoco : class, IPocoBase
        {
            var value = pi.GetValue(item, null) as PocoBase;
            pi.SetValue(returnedInstance,
                        value == null ?
                            null :
                            GetType().GenericGetByIdMethod(pi.PropertyType).Invoke(this, new object[] { value.Id }), null);
        }

        private IQueryable<TPoco> GetQueryForDiscriminantProperties<TPoco>(IQueryable<TPoco> items, IPocoBase item) where TPoco : class, IPocoBase
        {
            var type = typeof(TPoco);
            Asserts<InvalidOperationException>.IsTrue(item.HasDiscriminant(), String.Format("Poco {0} has no discriminant property", type.Name));

            var discriminantProperties = type.GetRecursiveDiscriminantProperties();
            var parameterExpression = Expression.Parameter(type, "item");
            foreach (var discriminant in discriminantProperties)
            {
                Expression memberExpression = parameterExpression;
                object value = null;
                type = typeof(TPoco);
                var curItem = item;
                foreach (var propertyName in discriminant.Split('.'))
                {
                    var pi = type.GetProperty(propertyName);
                    value = pi.GetValue(curItem, null);
                    memberExpression = Expression.Property(memberExpression, pi);
                    type = pi.PropertyType;
                    if (pi.PropertyType.IsReference())
                        curItem = value as PocoBase;
                }
                items = items.Where(Expression.Lambda<Func<TPoco, bool>>(
                    Expression.Equal(memberExpression, Expression.Constant(value)),
                    new[] { parameterExpression }));
            }
            return items;
        }

        protected string SqlEncode(object item)
        {
            if (Object.ReferenceEquals(item, null)) return NULL;

            var toString = (item is DateTime) ?
                ((DateTime)item).ToString(DateFormat) :
                item.ToString();

            return string.Format(SqlEncodeFormat, toString.Replace(SingleQuote, EscapedSingleQuote));
        }

        protected string SqlEncodeId(PocoBase item)
        {
            if (Object.ReferenceEquals(item, null)) return NULL;
            return string.Format(SqlEncodeFormat, item.Id);
        }

        private IQueryable<TItem> AllIncluding<TItem>(Expression<Func<TItem, object>>[] paths) where TItem : class, IPocoBase
        {
            var queryable = repository_.All<TItem>();
            queryable = paths
                .Select(p => p.Body as MemberExpression)
                .Where(member => member != null)
                .Aggregate(queryable, (current, member) => current.Include(string.Join(".", member.ToString().Split('.').Skip(1))));
            return queryable;
        }

        private IQueryable<TItem> IncludeReferences<TItem>() where TItem : class, IPocoBase
        {
            var queryable = repository_.All<TItem>();
            return typeof(TItem).GetReferenceProperties().Select(x => x.Name).Aggregate(queryable, (current, include) => current.Include(include));
        }
        
        #endregion Helpers
    }
}