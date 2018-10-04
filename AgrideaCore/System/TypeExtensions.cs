using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;
using Agridea.Service;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> ReferencePropertiesByTypeDictionary = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        #region Constants

        private static readonly IEnumerable<PropertyInfo> PocoBaseProperties = typeof(PocoBase).GetProperties();

        #endregion Constants

        #region EnumerablesTypes

        /// <summary>
        /// Checks if a type implements the given interface
        /// </summary>
        /// <remarks>Thanks to http://stackoverflow.com/questions/503263/how-to-determine-if-a-type-implements-a-specific-generic-interface-type
        /// </remarks>
        public static bool ImplementsGenericInterface(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            Asserts<ArgumentException>.IsTrue(@interface != null && @interface.IsInterface && @interface.IsGenericType);
            return type.GetInterfaces().Any(x =>
                                            x.IsGenericType &&
                                            x.GetGenericTypeDefinition() == @interface);
        }

        public static bool IsGenericInterface(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            return
                type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == @interface;
        }

        public static bool IsOrImplementsGenericInterface(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            return
                type.ImplementsGenericInterface(@interface) ||
                type.IsGenericInterface(@interface);
        }

        #endregion EnumerablesTypes

        #region PropertyInfo Lists

        public static IEnumerable<PropertyInfo> GetPublicPropertiesWithVirtualSetters(this Type type)
        {
            return ReferencePropertiesByTypeDictionary.GetOrAdd(type, x =>
            {
                var properties = x.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(m =>
                                     {
                                         var setter = m.GetSetMethod();
                                         return setter != null && setter.IsVirtual;
                                     });
                var excludedProperties = properties
                    .Except(PocoBaseProperties);
                var reallyExcludedProperties = excludedProperties.Where(m => !m.DeclaringType.IsPocoBase());
                return reallyExcludedProperties;
            });
        }

        public static IEnumerable<PropertyInfo> GetPrimitiveIncludingPocoBase(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(m =>
                {
                    var setter = m.GetSetMethod();
                    return setter != null && setter.IsVirtual;
                });
            return properties.Where(m => !m.IsCollection());
        }

        public static IEnumerable<PropertyInfo> GetPrimitiveProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Where(m => !m.IsReference() && !m.IsCollection());
        }

        public static IEnumerable<PropertyInfo> GetReferenceProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Where(m => m.IsReference());
        }

        public static IEnumerable<PropertyInfo> GetCollectionProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Where(m => m.IsCollection());
        }

        public static IEnumerable<PropertyInfo> GetNavigationProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Where(m => m.IsNavigation());
        }

        public static IEnumerable<PropertyInfo> GetDiscriminantProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Where(m => m.IsDiscriminant());
        }

        public static IEnumerable<PropertyInfo> GetPrimitiveDiscriminantProperties(this Type type)
        {
            return type.GetPrimitiveProperties()
                .Where(m => m.IsDiscriminant());
        }

        public static IEnumerable<PropertyInfo> GetReferenceDiscriminantProperties(this Type type)
        {
            return type.GetReferenceProperties()
                .Where(m => m.IsDiscriminant());
        }


        public static IList<string> GetRecursiveDiscriminantProperties(this Type type)
        {
            var result = new List<IList<string>> { new List<string>() };
            type.BuildRecursiveDiscriminantProperties(result);
            return result.Select(list => string.Join(".", list)).ToList();
        }

        private static void BuildRecursiveDiscriminantProperties(this Type type, IList<IList<string>> result)
        {
            var last = result.Last();
            var discriminantProperties = type.GetDiscriminantProperties().ToList();
            foreach (var discriminantProperty in discriminantProperties)
            {
                if (discriminantProperty != discriminantProperties.First())
                    result.Add(last.Contains(type.Name)
                                   ? last.Take(last.IndexOf(last.First(m => m == type.Name)) + 1).ToList()
                                   : new List<string>());

                result.Last().Add(discriminantProperty.Name);
                if (discriminantProperty.PropertyType.IsReference())
                    discriminantProperty.PropertyType.BuildRecursiveDiscriminantProperties(result);
            }
        }

        public static IEnumerable<PropertyInfo> GetCommonProperties(this Type type, Type otherType)
        {
            var firstTypeProperties = type.GetPublicPropertiesWithVirtualSetters().ToList();
            var otherTypeProperties = otherType.GetPublicPropertiesWithVirtualSetters().ToList();
            var commonNames = firstTypeProperties.Select(m => m.Name).Intersect(otherTypeProperties.Select(x => x.Name));
            return firstTypeProperties.Where(m => commonNames.Contains(m.Name));
        }

        #endregion PropertyInfo Lists

        #region FlagsByType

        public static bool HasDiscriminantProperties(this Type type)
        {
            return type.GetPublicPropertiesWithVirtualSetters()
                .Any(m => m.IsDiscriminant());
        }

        public static bool IsCollection(this Type type)
        {
            return
                type != null &&
                type.IsOrImplementsGenericInterface(typeof(IList<>)) &&
                type.GetGenericArguments().Any(t => t.IsReference());
        }

        public static bool IsGenericList(this Type type)
        {
            return type != null &&
                   type.IsOrImplementsGenericInterface(typeof(IList<>)) &&
                   type.GetGenericArguments().Any(t => t.IsClass);
        }

        public static bool IsReference(this Type type)
        {
            return type.IsSubclassOf(typeof(PocoBase));
        }

        public static bool IsPocoBase(this Type type)
        {
            return type == typeof(PocoBase);
        }

        public static bool IsDiscriminant(this Type type)
        {
            return type == typeof(DiscriminantAttribute);
        }
        public static bool IsMandatory(this Type type)
        {
            return type == typeof(MandatoryAttribute);
        }
        public static bool IsReferenceEntity(this Type type)
        {
            //return type.IsDefined(typeof(ReferenceAttribute), inherit:true);
            return type.GetCustomAttributes(true).Any(m => m.GetType() == typeof(ReferenceAttribute));
        }
        public static bool IsView(this Type type)
        {
            return type.GetCustomAttributes(true).Any(m => m.GetType() == typeof(ViewAttribute));
        }
        public static bool IsDimensionEntity(this Type type)
        {
            return type.GetCustomAttributes(true).Any(m => m.GetType() == typeof(DimensionAttribute));
        }

        public static bool IsTransient(this Type type)
        {
            return type == typeof(TransientAttribute);
        }

        public static IList<MemberExpression> MemberExpressionsFor(this Type type, out ParameterExpression parameterExpression)
        {
            var propertyList = type.GetRecursiveDiscriminantProperties();
            parameterExpression = Expression.Parameter(type, "item");
            var memberExpressions = new List<MemberExpression>();
            foreach (var property in propertyList)
                memberExpressions.Add(type.MemberExpressionFor(property, parameterExpression));

            return memberExpressions;
        }

        public static MemberExpression MemberExpressionFor(this Type type, string properties, Expression parameterExpression)
        {
            var member = parameterExpression;
            foreach (var property in properties.Split('.'))
            {
                var propertyInfo = type.GetProperty(property);
                member = Expression.Property(member, propertyInfo);
                type = propertyInfo.PropertyType;
            }
            return member as MemberExpression;
        }

        public static bool PropertyExists(this Type type, string propertyName)
        {
            foreach (var property in propertyName.Split('.'))
            {
                var propertyInfo = type.GetProperty(property);
                if (propertyInfo == null) return false;
                type = propertyInfo.PropertyType;
            }
            return true;
        }

        public static bool IsWritable(this Type type, string propertyName)
        {
            foreach (var property in propertyName.Split('.'))
            {
                var propertyInfo = type.GetProperty(property);
                if (propertyInfo == null || propertyInfo.GetSetMethod(false) == null) return false;
                type = propertyInfo.PropertyType;
            }
            return true;
        }

        #endregion FlagsByType

        #region Reflected Generic Methods

        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(this Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        public static MethodInfo GenericGetByIdMethod(this Type serviceType, Type propertyType)
        {
            return serviceType
                .GetMethods().First(x => x.Name == ServiceBase.GetByIdMethodName && x.GetParameters().Count() == 1)
                .MakeGenericMethod(propertyType);
            //return serviceType.GetMethod(ServiceBase.GetByIdMethodName).MakeGenericMethod(propertyType);
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, Type[] parameterTypes)
        {
            foreach (var method in type.GetMethods().Where(m => m.Name == name))
            {
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                if (methodParameterTypes.SequenceEqual(parameterTypes, new CovariantTypeComparer()))
                    return method;
            }
            return null;
        }

        public static IEnumerable<MethodInfo> GetMethodsBySignature(this Type type, BindingFlags bindingFlags, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods(bindingFlags).Where((m) =>
            {
                if (m.ReturnType != returnType) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0))
                    return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
        }

        private class CovariantTypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type genericParameter, Type effectiveParameter)
            {
                return effectiveParameter.IsSubclassOf(genericParameter.BaseType);
            }

            public int GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }

        public static bool IsSubtypeOf(this Type type, Type supertype)
        {
            return type.Equals(supertype) || type.IsSubclassOf(supertype);
        }

        public static bool IsSubtypeOf<TType>(this Type type) where TType : class
        {
            return type.IsSubtypeOf(typeof(TType));
        }

        public static bool Implements(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            Asserts<ArgumentException>.IsTrue(@interface != null && @interface.IsInterface);
            return type.GetInterfaces().Any(x => x == @interface);
        }

        public static bool Implements<TInterface>(this Type type) where TInterface : class
        {
            return type.Implements(typeof(TInterface));
        }

        #endregion Reflected Generic Methods

        #region Create delegates from property name

        public static Delegate CreateGetter<T>(this string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            var objParm = Expression.Parameter(property.DeclaringType, "o");
            Type delegateType = typeof(Func<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var lambda = Expression.Lambda(delegateType, Expression.Property(objParm, property.Name), objParm);
            return lambda.Compile();
        }

        public static Delegate CreateSetter<T>(this string propertyName)
        {
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            var objParm = Expression.Parameter(property.DeclaringType, "o");
            var valueParm = Expression.Parameter(property.PropertyType, "value");
            Type delegateType = typeof(Action<,>).MakeGenericType(property.DeclaringType, property.PropertyType);
            var lambda = Expression.Lambda(delegateType, Expression.Assign(Expression.Property(objParm, property.Name), valueParm), objParm, valueParm);
            return lambda.Compile();
        }

        #endregion Create delegates from property name

        #region TypeName
        /// <summary>
        /// e.g. Acorda2016.Parcel level 0 gives Parcel, level > 1 gives Acorda2016.Parcel 
        /// </summary>
        public static string GetTypeNameWithNameSpace(this Type type, int level = 1)
        {
            Asserts<ArgumentException>.GreaterOrEqual(level, 0);
            var nameSpaceComponents = type.Namespace.Split('.');
            var subNameSpace = "";
            for (int l = 1; l <= level && l <= nameSpaceComponents.Length; l++)
                subNameSpace = nameSpaceComponents[nameSpaceComponents.Length - l] + "." + subNameSpace;
            return subNameSpace + type.Name;
        }
        #endregion
    }
}