
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Agridea.DataRepository
{
    public class ClassMappings
    {
        #region Members
        private Dictionary<Type, Type> classMappings_;
        private Dictionary<Type, Dictionary<string, string>> classPropertyMappings_;
        #endregion

        #region Intialization
        public ClassMappings()
        {
            classMappings_ = new Dictionary<Type, Type>();
            classPropertyMappings_ = new Dictionary<Type, Dictionary<string, string>>();
        }
        #endregion

        #region Services
        public Type this[Type previousClass]
        {
            get
            {
                if (classMappings_.ContainsKey(KeyFor(previousClass))) return classMappings_[KeyFor(previousClass)];
                return null; //NO implicit mapping
            }
        }
        public string this[string previousClassName]
        {
            get
            {
                var pair = classMappings_.FirstOrDefault(x => x.Key.Name == previousClassName);
                if (pair.Value == null) return previousClassName; //implicit mapping;
                return pair.Value.Name;
            }
        }
        public string GetNotImplicitMappingType(string previousClassName)
        {
            var pair = classMappings_.FirstOrDefault(x => x.Key.Name == previousClassName);
            return (pair.Value == null) ? null : pair.Value.FullName;
        }
        public void SetClassMapping<TPreviousType, TCurrentType>()
        {
            classMappings_.Add(KeyFor(typeof(TPreviousType)), typeof(TCurrentType));
        }
        public string this[Type previousClass, string previousPropertyName]
        {
            get
            {
                if (!classPropertyMappings_.ContainsKey(previousClass) || !classPropertyMappings_[previousClass].ContainsKey(previousPropertyName)) return null;
                return classPropertyMappings_[KeyFor(previousClass)][previousPropertyName];
            }
        }
        public string this[string previousClassName, string previousPropertyName]
        {
            get
            {
                var pair = classPropertyMappings_.FirstOrDefault(x => x.Key.Name == previousClassName);
                Type previousClass = pair.Key;
                if (previousClass == null) return previousPropertyName; //implicit mapping;

                if (!classPropertyMappings_.ContainsKey(KeyFor(previousClass))) return previousPropertyName; //implicit mapping
                if (!classPropertyMappings_[KeyFor(previousClass)].ContainsKey(previousPropertyName)) return previousPropertyName; //implicit mapping
                return classPropertyMappings_[KeyFor(previousClass)][previousPropertyName];
            }
        }
        public void SetClassPropertyMapping<TPreviousType, TCurrentType>(Expression<Func<TPreviousType, object>> previousPropertyExpression, Expression<Func<TCurrentType, object>> currentPropertyExpression)
            where TPreviousType : class
            where TCurrentType : class
        {
            MemberExpression previousBody = previousPropertyExpression.Body as MemberExpression ??
                (previousPropertyExpression.Body as UnaryExpression).Operand as MemberExpression;
            MemberExpression currentBody = currentPropertyExpression.Body as MemberExpression ??
                (currentPropertyExpression.Body as UnaryExpression).Operand as MemberExpression;

            string previousPropertyName = previousBody.Member.Name;
            string currentPropertyName = currentBody.Member.Name;
            Type previousType = typeof(TPreviousType);
            if (!classPropertyMappings_.ContainsKey(KeyFor(previousType)))
                classPropertyMappings_.Add(KeyFor(previousType), new Dictionary<string, string>());

            classPropertyMappings_[KeyFor(previousType)].Add(previousPropertyName, currentPropertyName);
        }
        public bool ClassMappingValueExists(Type currentType)
        {
            return classMappings_.ContainsKey(KeyFor(currentType));
        }
        public bool ClassMappingValueExists(string previousClassName)
        {
            return classMappings_.Select(m => m.Key.Name).Contains(previousClassName);
        }
        #endregion

        #region Helpers
        private Type KeyFor(Type type)
        {
            return type;
        }
        #endregion
    }
}
