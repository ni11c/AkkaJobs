using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Agridea.ObjectMapping
{
    public class Mapper
    {
        #region Constants
        internal const string UnknownProperty = "?";
        #endregion

        #region Members
        public MapCollection Maps { get; private set; }
        #endregion

        #region Initialization
        public Mapper()
        {
            Maps = new MapCollection();
        }
        #endregion

        #region Services
        #region Map Creation
        public Mapping<TSource, TTarget> CreateMap<TSource, TTarget>()
            where TSource : class
            where TTarget : class
        {
            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            Map map = new Map();
            Map reverseMap = new Map();
            Maps.Add(sourceType, targetType, map);
            Maps.Add(targetType, sourceType, reverseMap);

            foreach (var targetPropertyPath in targetType.GetPropertyPathsForLeafProperties())
            {
                var sourcePropertyInfos = targetPropertyPath.Path.GetMatchingPropertyChain(targetPropertyPath.LastPropertyInfo, sourceType);

                var sourceProperty = sourcePropertyInfos.LastOrDefault();

                //REFACT condition, explicit : if simple types, they match or if collections the type of items match, or ???
                PropertyPath sourcePropertyPath = null;
                if (sourceProperty == null)
                    sourcePropertyPath = new PropertyPath(sourceType, UnknownProperty, sourcePropertyInfos);
                else
                {
                    var canBeMapped =
                        (sourceProperty.PropertyType == targetPropertyPath.LastPropertyInfo.PropertyType ||
                            (sourceProperty.PropertyType.IsGenericList() && targetPropertyPath.LastPropertyInfo.PropertyType.IsGenericList() &&
                                (sourceProperty.PropertyType.GetGenericArguments().First() == targetPropertyPath.LastPropertyInfo.PropertyType.GetGenericArguments().First() ||
                                Maps[sourceProperty.PropertyType.GetGenericArguments().First(), targetPropertyPath.LastPropertyInfo.PropertyType.GetGenericArguments().First()] != null)
                            )
                        );
                    //Requires
                    //if (!canBeMapped)
                    //    throw new InvalidOperationException(string.Format(
                    //        "Mapped property types do not match source '{0}:{1}' target '{2}:{3}'",
                    //        sourceProperty.Name,
                    //        sourceProperty.PropertyType.Name,
                    //        targetPropertyPath.Path,
                    //        targetPropertyPath.LastPropertyInfo.PropertyType));
                    sourcePropertyPath = new PropertyPath(sourceType, sourcePropertyInfos.GetPath(), sourcePropertyInfos);
                }

                map.Add(sourcePropertyPath, targetPropertyPath);
                if (sourcePropertyPath.Path != Mapper.UnknownProperty)
                    reverseMap.Add(targetPropertyPath, sourcePropertyPath);
            }
            return new Mapping<TSource, TTarget>(this);
        }

        public void MapProperty<TSource, TTarget>(
            Expression<Func<TSource, object>> sourceProperty,
            Expression<Func<TTarget, object>> targetProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(sourceProperty);
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            IList<PropertyInfo> sourcePropertyInfos = sourceProperty.GetPropertyInfos();
            IList<PropertyInfo> targetPropertyInfos = targetProperty.GetPropertyInfos();

            var sourcePropertyInfo = sourcePropertyInfos.Last();
            var targetPropertyInfo = targetPropertyInfos.Last();
            //Requires
            if (sourcePropertyInfo.PropertyType != targetPropertyInfo.PropertyType)
                throw new InvalidOperationException(string.Format(
                    "Mapped property types do not match source '{0}:{1}' target '{2}:{3}'",
                    sourcePropertyInfo.Name,
                    sourcePropertyInfo.PropertyType.Name,
                    targetPropertyInfo.Name,
                    targetPropertyInfo.PropertyType.Name));

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];

            var sourcePropertyPath = new PropertyPath(sourceType, sourceProperty.GetPath(), sourcePropertyInfos);
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath(), targetPropertyInfos);
            map.Replace(sourcePropertyPath, targetPropertyPath);
            reverseMap.Replace(targetPropertyPath, sourcePropertyPath);
        }
        public void MapProperty<TSource, TTarget>(
            Expression<Func<TSource, object>> sourceProperty,
            Expression<Func<TTarget, object>> targetProperty,
            Func<object, object> conversion,
            Func<object, object> reverseConversion)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(sourceProperty);
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            IList<PropertyInfo> sourcePropertyInfos = sourceProperty.GetPropertyInfos();
            IList<PropertyInfo> targetPropertyInfos = targetProperty.GetPropertyInfos();

            //Requires TODO
            //check conversion(source) and reverseConversion(target) are well typed ? 

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];

            var sourcePropertyPath = new PropertyPath(sourceType, sourceProperty.GetPath(), sourcePropertyInfos, conversion);
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath(), targetPropertyInfos, reverseConversion);
            map.Replace(sourcePropertyPath, targetPropertyPath);
            reverseMap.Replace(targetPropertyPath, sourcePropertyPath);
        }
        public void MapProperty<TSource, TTarget>(
            Expression<Func<TSource, object>> sourceProperty1,
            Expression<Func<TSource, object>> sourceProperty2,
            Expression<Func<TTarget, object>> targetProperty,
            Func<object, object, object> computation)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(sourceProperty1);
            Requires<ArgumentNullException>.IsNotNull(sourceProperty2);
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            IList<PropertyInfo> sourcePropertyInfos1 = sourceProperty1.GetPropertyInfos();
            IList<PropertyInfo> sourcePropertyInfos2 = sourceProperty2.GetPropertyInfos();
            IList<PropertyInfo> targetPropertyInfos = targetProperty.GetPropertyInfos();

            //Requires TODO
            //check computation(source1, source2) is well typed ? 
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];

            var sourcePropertyPath = new TwoPropertyPath(sourceType, sourceProperty1.GetPath(), sourcePropertyInfos1, sourceProperty2.GetPath(), sourcePropertyInfos2, computation);
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath(), targetPropertyInfos, null /*non bijective computation*/);
            map.Replace(sourcePropertyPath, targetPropertyPath);
            //reverseMap.Replace(targetPropertyPath, sourcePropertyPath);
            reverseMap.Remove(targetPropertyPath);
        }

        public void DontMapProperty<TSource, TTarget>(
            Expression<Func<TTarget, object>> targetProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath());
            var sourcePropertyPath = map.Get(targetPropertyPath);

            map.Remove(targetPropertyPath);
            if (sourcePropertyPath != null) 
                reverseMap.Remove(sourcePropertyPath);
        }

        public void DontMapPropertyRecursive<TSource, TTarget>(Expression<Func<TTarget, object>> targetProperty)
            where TSource : class
            where TTarget : class
        {
            var sourceType = typeof (TSource);
            var targetType = typeof (TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];
            var basePropertyPath = targetProperty.GetPath();
            foreach (var targetPropertyPath in map.Keys.Where(m => m.Path.StartsWith(basePropertyPath)).ToList())
            {
                var sourcePropertyPath = map.Get(new PropertyPath(targetType, targetPropertyPath.Path));
                map.Remove(targetPropertyPath);
                if (sourcePropertyPath != null)
                    reverseMap.Remove(sourcePropertyPath);;
            }
        }
        public void MapPropertyOneWay<TSource, TTarget>(
            Expression<Func<TTarget, object>> targetProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath());
            var sourcePropertyPath = map.Get(targetPropertyPath);

            if (sourcePropertyPath != null) 
                reverseMap.Remove(sourcePropertyPath);
        }
        /// <summary>
        /// If a class has a property P1 that needs only be displayed and P2 be updated sequence 
        ///   .AsReadOnly()
        ///   .MapBothWays(x => x.P2) does not work
        /// use  instead 
        ///   .AsReadOnly()
        ///   .MapProperty(m => m.P2, m => m.P2) OR
        ///   .MapOneWay(x => x.P1) 
        /// 
        /// MapBothWays marks navigation properties that we are sure we want them updated otherwise a mapping error is issued
        /// The pb is that MapProperty and MapBothWays are not symetrical for regular properties
        /// MapBothWays does not add the property if not yet in the map whereas MapProperty does...
        /// Solution MapProperty base routine (no conversion) lacks a private core that could be used also by MapBothWays to create the map.
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="targetProperty"></param>
        public void MapPropertyBothWays<TSource, TTarget>(
            Expression<Func<TTarget, object>> targetProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(targetProperty);

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var map = Maps[sourceType, targetType];
            var reverseMap = Maps[targetType, sourceType];
            var targetPropertyPath = new PropertyPath(targetType, targetProperty.GetPath());
            var sourcePropertyPath = map.Get(targetPropertyPath);

            if (sourcePropertyPath != null)
                sourcePropertyPath.MapBothWays = true;
        }

        public void AssertConfigurationIsValid()
        {
            CheckUnmappedProperties();
            CheckPropertiesOfNavigationProperties();
        }

        public void AsReadOnly<TSource, TTarget>()
            where TSource : class 
            where TTarget:class
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var reverseMap = Maps[targetType, sourceType];
            reverseMap.Clear();
        }
        public class MappingTypes
        {
            public MappingTypes()
            {
                MappingProperties = new List<string>();
            }
            public Type PocoType { get; set; }
            public Type ViewModelType { get; set; }
            public List<string> MappingProperties { get; set; }
        }
        #endregion

        #region Map Query
        public bool Exists<TSource, TTarget>()
        {
            return Maps.MapExists(typeof(TSource), typeof(TTarget));
        }
        public PropertyPath GetMap<TSource, TTarget>(Expression<Func<TSource, object>> sourceProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(sourceProperty);

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            var reverseMap = Maps[targetType, sourceType];
            Requires<InvalidOperationException>.IsNotNull(reverseMap);
            return reverseMap.Get(new PropertyPath(sourceType, sourceProperty.GetPath()));
        }
        public PropertyPath GetMap(Type sourceType, Type targetType, string sourcePropertyName)
        {
            Requires<ArgumentNullException>.IsNotNull(sourceType);
            Requires<ArgumentNullException>.IsNotNull(targetType);
            Requires<ArgumentNullException>.IsNotEmpty(sourcePropertyName);

            var reverseMap = Maps[targetType, sourceType];
            Requires<InvalidOperationException>.IsNotNull(reverseMap);
            return reverseMap.Get(new PropertyPath(sourceType, sourcePropertyName));
        }
        public PropertyPath GetMap<TSource, TTarget>(string sourcePropertyName)
        {
            Requires<ArgumentNullException>.IsNotEmpty(sourcePropertyName);
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);
            var reverseMap = Maps[targetType, sourceType];
            Requires<InvalidOperationException>.IsNotNull(reverseMap);
            return reverseMap.Get(new PropertyPath(sourceType, sourcePropertyName));
        }
        #endregion

        #region Map Application
        public void Map<TSource, TTarget>(TSource source, TTarget target, IService service = null)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(source);
            Requires<ArgumentNullException>.IsNotNull(target);

            Map(source, target, service, new Expression<Func<TTarget, object>>[] { });
        }
        public void Map<TSource, TTarget>(TSource source, TTarget target, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(source);
            Requires<ArgumentNullException>.IsNotNull(target);
            Requires<ArgumentNullException>.IsNotNull(collectionsOwningItems);

            foreach (var collectionsOwningItem in collectionsOwningItems)
            {
                var propertyName = ExpressionHelper.GetExpressionText(collectionsOwningItem);
                var propertyType = target.GetType().GetProperty(propertyName).PropertyType;
                Requires<InvalidOperationException>.IsTrue(propertyType.IsGenericList(), string.Format("Property '{0}' is not a generic List but '{1}'", propertyName, propertyType));
            }

            Type sourceType = typeof(TSource);
            Type targetType = typeof(TTarget);
            EasyMapper.Context.Set(typeof(TSource), typeof(TTarget));
            var map = Maps[sourceType, targetType];

            PropertyPath sourcePropertyPath = null;
            foreach (var targetPropertyPath in map.Keys)
            {
                sourcePropertyPath = map.Get(targetPropertyPath);
                Requires<InvalidOperationException>.IsNotNull(sourcePropertyPath);
                if (!sourcePropertyPath.CanConvert()) continue;
                sourcePropertyPath.SetValue(target, source, targetPropertyPath, this, service, collectionsOwningItems);
            }
        }

        public void Map<TSource, TTarget>(IList<TTarget> targetList, IList<TSource> sourceList, IService service = null)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(targetList);
            Requires<ArgumentNullException>.IsNotNull(sourceList);

            Map(targetList, sourceList, service, new Expression<Func<TTarget, object>>[] { });
        }
        public void Map<TSource, TTarget>(IList<TTarget> targetList, IList<TSource> sourceList, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(targetList);
            Requires<ArgumentNullException>.IsNotNull(sourceList);
            Requires<ArgumentNullException>.IsNotNull(collectionsOwningItems);

            foreach (var source in sourceList)
            {
                var target = new TTarget();
                Map(source, target, service, collectionsOwningItems);
                targetList.Add(target);
            }
        }

        public TTarget Map<TSource, TTarget>(TSource source, IService service = null)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(source);

            return Map(source, service, new Expression<Func<TTarget, object>>[] { });
        }
        public TTarget Map<TSource, TTarget>(TSource source, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(source);
            Requires<ArgumentNullException>.IsNotNull(collectionsOwningItems);

            var target = new TTarget();
            Map(source, target, service, collectionsOwningItems);
            return target;
        }
        #endregion
        #endregion

        #region Helpers
        private void CheckUnmappedProperties()
        {
            var notMappedPropertyPaths = Maps.SelectMany(x => x.Keys.Where(y => x.Get(y).Path == UnknownProperty));
            if (notMappedPropertyPaths.Any())
            {
                string notMappedPropertyNames = string.Empty;
                notMappedPropertyPaths.ToList()
                    .ForEach(x => notMappedPropertyNames += string.Format("{0}:{1};", x.Type.Name, x.Path));

                Requires<InvalidOperationException>.Fails(string.Format("Some property could not be mapped : {0}", notMappedPropertyNames));
            }
        }
        private void CheckPropertiesOfNavigationProperties()
        {
            var mapCollections = Maps.Maps.Where(x => x.Key.Item2.IsSubclassOf(typeof(PocoBase))).ToList();
            var mappingTypes = new List<MappingTypes>();
            foreach (var mapCollection in mapCollections)
            {
                var dubiousProperties = mapCollection.Value.Keys.Where(x => GetMap(mapCollection.Key.Item2, mapCollection.Key.Item1, x.Path) != null && x.IsDepthGreaterThanOne() && !x.IsReferenceId() && !x.MapBothWays)
                    .ToList();
                if (dubiousProperties.Any())
                {
                    var mappingType = mappingTypes.FirstOrDefault(x => x.ViewModelType == mapCollection.Key.Item1 && x.PocoType == mapCollection.Key.Item2)
                        ?? new MappingTypes
                        {
                            ViewModelType = mapCollection.Key.Item1,
                            PocoType = mapCollection.Key.Item2
                        };

                    mappingType.MappingProperties.AddRange(dubiousProperties.Select(m => m.Path));

                    mappingTypes.Add(mappingType);
                }
            }

            if (mappingTypes.Any())
            {
                string dubiousBothWaysNavigationPropertyNames = string.Empty;
                foreach (var mappingType in mappingTypes)
                {
                    foreach (var mappingProperty in mappingType.MappingProperties)
                        dubiousBothWaysNavigationPropertyNames += string.Format("{0}=>{1}:{2};", mappingType.ViewModelType.Name, mappingType.PocoType.Name, mappingProperty);
                }

                Requires<InvalidOperationException>.Fails(string.Format("Some navigation property are implicitely mapped both ways : {0}", dubiousBothWaysNavigationPropertyNames));
            }
        }
        #endregion
    }
}
