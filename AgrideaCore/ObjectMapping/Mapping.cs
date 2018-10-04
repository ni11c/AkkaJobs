using System;
using System.Linq.Expressions;
using Agridea.Diagnostics.Contracts;

namespace Agridea.ObjectMapping
{
    /// <summary>
    /// In the style of AutoMapper,
    /// - to make fluent less verbose
    /// - avoid respecifying type params for MapProperty methods
    /// </summary>
    public class Mapping<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        #region Members
        private Mapper mapper_;
        #endregion

        #region Initialization
        public Mapping(Mapper mapper)
        {
            mapper_ = mapper;
        }
        #endregion

        #region Services
        public Mapping<TSource, TTarget> MapProperty(
            Expression<Func<TSource, object>> source,
            Expression<Func<TTarget, object>> target)
        {
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(target);

            mapper_.MapProperty(source, target);
            return this;
        }
        public Mapping<TSource, TTarget> MapProperty(
            Expression<Func<TSource, object>> source,
            Expression<Func<TTarget, object>> target,
            Func<object, object> conversion)
        {
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(target);

            mapper_.MapProperty(source, target, conversion, null);
            return this;
        }
        public Mapping<TSource, TTarget> MapProperty(
            Expression<Func<TSource, object>> source,
            Expression<Func<TTarget, object>> target,
            Func<object, object> conversion,
            Func<object, object> reverseConversion)
        {
            Asserts<ArgumentNullException>.IsNotNull(source);
            Asserts<ArgumentNullException>.IsNotNull(target);
            Asserts<ArgumentNullException>.IsNotNull(conversion);
            Asserts<ArgumentNullException>.IsNotNull(reverseConversion);

            mapper_.MapProperty(source, target, conversion, reverseConversion);
            return this;
        }

        public Mapping<TSource, TTarget> MapProperties(
           Expression<Func<TSource, object>> source1,
           Expression<Func<TSource, object>> source2,
           Expression<Func<TTarget, object>> target,
           Func<object, object, object> computation)
        {
            Asserts<ArgumentNullException>.IsNotNull(source1);
            Asserts<ArgumentNullException>.IsNotNull(source2);
            Asserts<ArgumentNullException>.IsNotNull(target);
            Asserts<ArgumentNullException>.IsNotNull(computation);

            mapper_.MapProperty(source1, source2, target, computation);
            return this;
        }
        public Mapping<TSource, TTarget> DontMapProperty(
            Expression<Func<TTarget, object>> target)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);

            mapper_.DontMapProperty<TSource, TTarget>(target);
            return this;
        }

        public Mapping<TSource, TTarget> DontMapPropertyRecursive(Expression<Func<TTarget, object>> target)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);
            mapper_.DontMapPropertyRecursive<TSource, TTarget>(target);
            return this;
        }
        public Mapping<TSource, TTarget> MapPropertyOneWay(
            Expression<Func<TTarget, object>> target)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);

            mapper_.MapPropertyOneWay<TSource, TTarget>(target);
            return this;
        }
        public Mapping<TSource, TTarget> MapPropertyBothWays(
            Expression<Func<TTarget, object>> target)
        {
            Asserts<ArgumentNullException>.IsNotNull(target);

            mapper_.MapPropertyBothWays<TSource, TTarget>(target);
            return this;
        }
        public Mapping<TSource, TTarget> AssertConfigurationIsValid()
        {
            mapper_.AssertConfigurationIsValid();
            return this;
        }

        public Mapping<TSource, TTarget> AsReadOnly()
        {
            mapper_.AsReadOnly<TSource, TTarget>();
            return this;
        }
        #endregion
    }
}
