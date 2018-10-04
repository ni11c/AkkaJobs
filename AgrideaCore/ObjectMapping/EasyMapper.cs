using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Agridea.Diagnostics.Contracts;
using Agridea.Service;

namespace Agridea.ObjectMapping
{
    /// <summary>
    /// In the style of Service class
    /// In the style of ValueInjecter
    /// </summary>
    public static class EasyMapper
    {
        #region Members
        public static Mapper Mapper { get; private set; }
        #endregion

        #region Initialization
        static EasyMapper()
        {
            Reset();
        }
        #endregion

        #region Services
        #region Map Creation
        public static void Reset()
        {
            Mapper = new Mapper();
            ContextResolver = new StandardMapContextResolver();
        }
        public static Mapping<TSource, TTarget> EnsureMapper<TSource, TTarget>(
            bool assertConfigurationIsValid = true)
            where TSource : class
            where TTarget : class
        {
            if (!Mapper.Exists<TSource, TTarget>())
                Mapper.CreateMap<TSource, TTarget>();
            if (assertConfigurationIsValid)
                Mapper.AssertConfigurationIsValid();
            return new Mapping<TSource, TTarget>(Mapper);
        }
        public static void AssertConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
        #endregion

        #region Map Query
        public static PropertyPath GetMap<TSource, TTarget>(
            Expression<Func<TSource, object>> sourceProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(sourceProperty);

            EnsureMapper<TSource, TTarget>();
            return Mapper.GetMap<TSource, TTarget>(sourceProperty);
        }
        public static PropertyPath GetMap<TTarget, TSource>(
            string sourcePropertyName)
        {
            Requires<ArgumentNullException>.IsNotNull(sourcePropertyName);
            return Mapper.GetMap<TTarget, TSource>(sourcePropertyName);
        }
        public static PropertyPath GetMap(
            Type sourceType,
            Type targetType,
            string sourcePropertyName)
        {
            Requires<ArgumentNullException>.IsNotNull(sourceType);
            Requires<ArgumentNullException>.IsNotNull(targetType);
            Requires<ArgumentNullException>.IsNotNull(sourcePropertyName);

            return Mapper.GetMap(sourceType, targetType, sourcePropertyName);
        }
        public static PropertyPath GetMap<TTarget, TSource>(
            this TSource source,
            TTarget target,
            Expression<Func<TSource, object>> sourceProperty)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(target);
            Requires<ArgumentNullException>.IsNotNull(sourceProperty);

            EnsureMapper<TSource, TTarget>();
            return Mapper.GetMap<TSource, TTarget>(sourceProperty);
        }
        #endregion

        #region Map Application
        public static TTarget Map<TSource, TTarget>(TSource source, IService service = null)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(source);

            return Mapper.Map(source, service, new Expression<Func<TTarget, object>>[] { });
        }
        public static IQueryable<TTarget> Map<TSource, TTarget>(this IQueryable<TSource> source)
            where TTarget : class
            where TSource : class
        {
            var mapping = EnsureMapper<TSource, TTarget>();
            return source.Project().To<TTarget>(mapping);
        }

        public static void Map<TSource, TTarget>(IList<TTarget> targetList, IList<TSource> sourceList, IService service = null)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(targetList);
            Requires<ArgumentNullException>.IsNotNull(sourceList);

            Mapper.Map(targetList, sourceList, service, new Expression<Func<TTarget, object>>[] { });
        }
        public static void Map<TSource, TTarget>(IList<TTarget> targetList, IList<TSource> sourceList, IService service, params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(targetList);
            Requires<ArgumentNullException>.IsNotNull(sourceList);
            Requires<ArgumentNullException>.IsNotNull(collectionsOwningItems);

            Mapper.Map(targetList, sourceList, service, collectionsOwningItems);
        }

        public static TTarget MapFrom<TSource, TTarget>(
            this TTarget target,
            TSource source)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(source);

            return MapFrom(target, source, null, new Expression<Func<TTarget, object>>[] { });
        }
        public static TTarget MapFrom<TSource, TTarget>(
            this TTarget target,
            TSource source,
            IService service,
            params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class
        {
            Requires<ArgumentNullException>.IsNotNull(source);
            Requires<ArgumentNullException>.IsNotNull(collectionsOwningItems);

            EnsureMapper<TSource, TTarget>();
            Mapper.Map<TSource, TTarget>(source, target, service, collectionsOwningItems);
            return target;
        }

        public static IList<TTarget> MapFrom<TSource, TTarget>(
            this IList<TTarget> targetList,
            IList<TSource> sourceList)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(sourceList);

            return MapFrom(targetList, sourceList, null, new Expression<Func<TTarget, object>>[] { });
        }
        public static IList<TTarget> MapFrom<TSource, TTarget>(
            this IList<TTarget> targetList,
            IList<TSource> sourceList,
            IService service,
            params Expression<Func<TTarget, object>>[] collectionsOwningItems)
            where TSource : class
            where TTarget : class, new()
        {
            Requires<ArgumentNullException>.IsNotNull(sourceList);

            EnsureMapper<TSource, TTarget>();
            Mapper.Map(targetList, sourceList, service, collectionsOwningItems);
            return targetList;
        }
        #endregion

        #region Map Context
        public static IMapContextResolver ContextResolver { private get; set; }
        public static MapContext Context
        {
            get { return ContextResolver.Context; }
        }
        #endregion
        #endregion
    }
}