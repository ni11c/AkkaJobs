using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Agridea.ObjectMapping
{
    public class PropertyPath
    {
        #region Constants
        private const string IdPropertyName = "Id";
        private const string MapperMapMethoName = "Map";
        private const string ListAddMethoName = "Add";
        private const string ListRemoveMethodName = "Remove";
        private const string ServiceRemoveMethodName = "Remove";
        private static Func<object, object> Identity = x => x;
        private const char Dot = '.';
        public const char PropertyPathSeparator = Dot;
        #endregion

        #region Members
        protected Type type_;
        protected string path_;
        internal IList<PropertyInfo> propertyInfos_;
        private Func<object, object> conversion_ = Identity;
        #endregion

        #region Initialization
        public PropertyPath(Type type, string path)
        {
            Asserts<ArgumentException>.IsNotNull(type);
            Asserts<ArgumentException>.IsNotEmpty(path);

            type_ = type;
            path_ = path;
            MapBothWays = false;
        }
        public PropertyPath(Type type, string path, IList<PropertyInfo> propertyInfos)
            : this(type, path)
        {
            Asserts<ArgumentException>.IsNotNull(type);
            Asserts<ArgumentException>.IsNotEmpty(path);
            Asserts<ArgumentNullException>.IsNotNull(propertyInfos);

            propertyInfos_ = propertyInfos;
        }
        public PropertyPath(Type type, string path, IList<PropertyInfo> propertyInfos, Func<object, object> conversion)
            : this(type, path, propertyInfos)
        {
            Asserts<ArgumentException>.IsNotNull(type);
            Asserts<ArgumentException>.IsNotEmpty(path);
            Asserts<ArgumentNullException>.IsNotNull(propertyInfos);
            //conversion can be null in case of non bijective computation

            conversion_ = conversion;
        }
        #endregion

        #region Services
        public override int GetHashCode()
        {
            return path_.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyPath);
        }
        public bool Equals(PropertyPath other)
        {
            if (object.ReferenceEquals(other, null)) return false;
            if (object.ReferenceEquals(this, other)) return true;
            return Path.Equals(other.Path);
        }
        public override string ToString()
        {
            return string.Format("[{0} Path='{1}']", GetType().Name, Path);
        }

        public virtual Type Type
        {
            get { return type_; }
        }
        public virtual string Path
        {
            get { return path_; }
        }
        public PropertyInfo LastPropertyInfo
        {
            get { return propertyInfos_.LastOrDefault(); }
        }
        public PropertyInfo FirstPropertyInfo
        {
            get { return propertyInfos_.FirstOrDefault(); }
        }
        public bool IsReference()
        {
            return propertyInfos_.Count > 1 && LastPropertyInfo.Name == IdPropertyName;
        }
        public bool IsReferenceId()
        {
            return LastPropertyInfo.Name.EndsWith(IdPropertyName);
        }
        public bool IsDepthGreaterThanOne()
        {
            return propertyInfos_.Count > 1;
        }
        public bool MapBothWays { get; set; }
        public bool IsGenericList()
        {
            return LastPropertyInfo.PropertyType.IsGenericList();
        }

        /// <summary>
        /// TODO, need a serious refactoring...
        /// </summary>
        public virtual void SetValue<TTarget>(object target, object source, PropertyPath targetPropertyPath, Mapper mapper, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TTarget : class
        {
            Asserts<ArgumentNullException>.IsNotNull(target);
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(targetPropertyPath);
            Asserts<ArgumentNullException>.IsNotNull(mapper);
            Asserts<ArgumentNullException>.IsNotNull(collectionsOwningItems);
            Asserts<InvalidOperationException>.IsTrue(CanConvert());

            var targetFirstPropertyInfo = targetPropertyPath.FirstPropertyInfo;
            var targetLastPropertyInfo = targetPropertyPath.LastPropertyInfo;
            bool nullInChain = false;
            var sourceValue = GetValue(source, propertyInfos_, out nullInChain);

            if (nullInChain) //Accomodate nulls
            {
                if (targetPropertyPath.IsReferenceId())
                    DoSetValue(target, targetFirstPropertyInfo, 0);
                //else ignore
            }
            else
            {
                //The target object chain has been walked thru, recreated by GetInstance
                if (service != null)
                {
                    if (targetPropertyPath.IsReference())
                    {
                        if (sourceValue == null)
                            DoSetValue(target, targetFirstPropertyInfo, null);
                        else
                        {
                            int id = Convert.ToInt32(sourceValue);
                            Requires<InvalidKeyException>.GreaterOrEqual(id, -1);
                            if (id == -1)
                            {
                                //Ignore
                                return;
                            }
                            if (id == 0)
                            {
                                DoSetValue(target, targetFirstPropertyInfo, null);
                            }
                            if (id > 0)
                            {
                                var reference = service.GetPocoById(targetFirstPropertyInfo.PropertyType, id);
                                Requires<InvalidOperationException>.IsNotNull(reference);
                                DoSetValue(target, targetFirstPropertyInfo, reference);
                            }
                        }
                    }
                    else if (targetPropertyPath.IsGenericList())
                    {
                        //TODO add a test where the conversion of collection is not x => x

                        var refetchedTarget = service.GetPocoById(target.GetType(), GetId(target));
                        var targetValue = GetValue(target, targetPropertyPath.propertyInfos_, out nullInChain);

                        var modelList = targetValue as IList;
                        var viewModelList = sourceValue as IList;

                        var clonedModelList = new ArrayList(modelList);
                        foreach (var item in clonedModelList)
                        {
                            var viewModelListItem = GetMatchingItem(viewModelList, item);
                            if (viewModelListItem == null)
                            {
                                CallRemove(modelList, item);
                                if (collectionsOwningItems.Any(x => (ExpressionHelper.GetExpressionText(x) == targetLastPropertyInfo.Name)))
                                    CallRemoveFromPersistence(service, item);
                            }
                            else
                            {
                                CallMap(mapper, viewModelListItem, item, service);
                            }
                        }
                        foreach (var newItem in viewModelList)
                        {
                            if (GetId(newItem) != 0) continue;

                            var mappedItem = CreateListItemInstanceOf(targetLastPropertyInfo);
                            CallMap(mapper, newItem, mappedItem, service);
                            CallAdd(modelList, mappedItem);
                        }
                    }
                    else
                    {
                        var targetInstance = GetInstance(target, targetPropertyPath);
                        DoSetValue(targetInstance, targetLastPropertyInfo, conversion_(sourceValue));
                    }
                    service.Modify(target as PocoBase);
                    //Log.Verbose("SetValue applied for {0} to {1}", targetPropertyPath.Path, target.ToString());
                }
                if (service == null)
                {
                    var targetInstance = GetInstance(target, targetPropertyPath);
                    if (targetPropertyPath.IsGenericList())
                    {
                        //This replaces the collection
                        var collectionInstance = sourceValue == null ? null : CreateCollection(sourceValue, targetLastPropertyInfo, mapper, service);
                        DoSetValue(targetInstance, targetLastPropertyInfo, conversion_(collectionInstance));
                        //TODO add a test where the conversion of collection is not x => x
                    }
                    else
                    {
                        DoSetValue(targetInstance, targetLastPropertyInfo, conversion_(sourceValue));
                    }
                }
            }
        }
        public bool CanConvert()
        {
            return conversion_ != null;
        }
        #endregion

        #region Helpers
        private int GetId(object target)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);

            var idProperty = target.GetType().GetProperty(IdPropertyName);
            return Convert.ToInt32(idProperty.GetGetMethod().Invoke(target, null));
        }
        private object GetMatchingItem(IList viewModelList, object item)
        {
            Asserts<ArgumentNullException>.IsNotNull(viewModelList);
            Asserts<ArgumentNullException>.IsNotNull(item);

            foreach (var viewModelListItem in viewModelList)
                if (GetId(viewModelListItem) == GetId(item)) return viewModelListItem;
            return null;
        }
        protected object GetValue(object instance, IList<PropertyInfo> propertyChain, out bool nullInChain)
        {
            Asserts<ArgumentNullException>.IsNotNull(instance);
            Asserts<ArgumentNullException>.IsNotNull(propertyChain);

            object value = instance;
            nullInChain = false;
            foreach (var propertyInfo in propertyChain)
            {
                value = DoGetValue(value, propertyInfo);
                if (value == null && !(propertyInfo.PropertyType.IsNullableType() || propertyInfo.PropertyType == typeof(string)))
                {
                    nullInChain = true;
                    return null; //Accomodate nulls
                }
            }
            return value;
        }
        protected object GetInstance(object instance, PropertyPath propertyPath)
        {
            Asserts<ArgumentNullException>.IsNotNull(instance);
            Asserts<ArgumentNullException>.IsNotNull(propertyPath);

            object value = instance;
            object nextValue = null;
            for (int p = 0; p < propertyPath.propertyInfos_.Count - 1; p++)
            {
                var propertyInfo = propertyPath.propertyInfos_[p];
                nextValue = DoGetValue(value, propertyInfo);
                if (nextValue == null)
                {
                    nextValue = Activator.CreateInstance(propertyInfo.PropertyType);
                    DoSetValue(value, propertyInfo, nextValue);
                }
                value = nextValue;
            }
            return value;
        }
        protected void DoSetValue(object target, PropertyInfo propertyInfo, object value)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);

            try
            {
                var setMethod = propertyInfo.GetSetMethod();

                if (setMethod == null || (value == null && propertyInfo.PropertyType.IsGenericList())) return; //when setter does not exist, e.g. for transient properties
                Asserts<ArgumentNullException>.IsTrue((value == null).Implies(propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsNullableType()));

                setMethod.Invoke(target, new object[] { value });
            }
            catch (Exception e)
            {
                throw new ApplicationException(propertyInfo.Name, e);
            }
        }
        protected object DoGetValue(object source, PropertyInfo propertyInfo)
        {
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);

            return propertyInfo.GetGetMethod().Invoke(source, null);
        }
        protected object CreateCollection(object source, PropertyInfo propertyInfo, Mapper mapper, IService service)
        {
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);
            Asserts<ArgumentNullException>.IsNotNull(mapper);

            var collectionInstance = CreateListOf(propertyInfo);
            foreach (var item in source as IList)
            {
                var mappedItem = CreateListItemInstanceOf(propertyInfo);
                CallMap(mapper, item, mappedItem, service);
                CallAdd(collectionInstance, mappedItem);
            }
            return collectionInstance;
        }

        private object CreateListOf(PropertyInfo propertyInfo)
        {
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);

            return Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments().First()));
        }
        private object CreateListItemInstanceOf(PropertyInfo propertyInfo)
        {
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);

            return Activator.CreateInstance(propertyInfo.PropertyType.GetGenericArguments().First());
        }
        private object CreateInstanceOf(PropertyInfo propertyInfo)
        {
            Asserts<ArgumentNullException>.IsNotNull(propertyInfo);

            return Activator.CreateInstance(propertyInfo.PropertyType);
        }

        private void CallMap(Mapper mapper, object item, object mappedItem, IService service)
        {
            Asserts<ArgumentNullException>.IsNotNull(mapper);
            Asserts<ArgumentNullException>.IsNotNull(item);
            Asserts<ArgumentNullException>.IsNotNull(mappedItem);

            var mapMethod = mapper.GetType().GetMethods().Where(x => x.Name == MapperMapMethoName && x.GetParameters().Count() == 3).First();
            mapMethod.MakeGenericMethod(item.GetType(), mappedItem.GetType()).Invoke(mapper, new object[] { item, mappedItem, service });
        }
        private void CallAdd(object collectionInstance, object mappedItem)
        {
            Asserts<ArgumentNullException>.IsNotNull(collectionInstance);
            Asserts<ArgumentNullException>.IsNotNull(mappedItem);

            collectionInstance.GetType().GetMethod(ListAddMethoName).Invoke(collectionInstance, new object[] { mappedItem });
        }
        private void CallRemove(object collectionInstance, object mappedItem)
        {
            Asserts<ArgumentNullException>.IsNotNull(collectionInstance);
            Asserts<ArgumentNullException>.IsNotNull(mappedItem);

            collectionInstance.GetType().GetMethod(ListRemoveMethodName).Invoke(collectionInstance, new object[] { mappedItem });
        }
        private void CallRemoveFromPersistence(IService service, object mappedItem)
        {
            Asserts<ArgumentNullException>.IsNotNull(mappedItem);

            var removeMethod = service.GetType().GetMethods().Where(x => x.Name == ServiceRemoveMethodName && x.IsGenericMethod).First();
            removeMethod.MakeGenericMethod(mappedItem.GetType()).Invoke(service, new object[] { mappedItem });
        }
        #endregion
    }
}
