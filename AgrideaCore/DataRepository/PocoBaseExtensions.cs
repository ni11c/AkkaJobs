using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Agridea.DataRepository
{
    /// <summary>
    /// Note : all public extensions use implicitely PocoBase (from current)
    /// all private helpers expose Type pocoBaseType as param
    /// </summary>
    public static class PocoBaseExtensions
    {
        //[Obsolete("IsCollectionInEndRelationShip can it be integrated into PocoBase?")]
        public static bool IsCollectionInEndRelationShip<TItem>(this TItem item, Type endRelationType) where TItem : class, IPocoBase
        {
            return endRelationType.IsReference() && endRelationType.GetCollectionProperties().Select(m => m.PropertyType.GetGenericArguments().First()).Contains(typeof(TItem));
        }
        //[Obsolete("CopyPropertyFrom can it be integrated into PocoBase?")]
        /// <remarks>
        /// This method has side effects on source !! It is used by all the ServiceBase.AddOrUpdate methods, and cleans the source after the copying
        /// is done. One must be aware of that if she wants to act on the source after calling AddOrUpdate(source). In addition, this looks
        /// like bad design and needs rethinking IMHO.
        /// </remarks>
        public static TPoco CopyPropertyFrom<TPoco>(this TPoco destination, TPoco source, PropertyInfo propertyInfo) where TPoco : class, IPocoBase
        {
            var needsCleaning = propertyInfo.IsReference() &&
                                destination.IsCollectionInEndRelationShip(propertyInfo.PropertyType) &&
                                (propertyInfo.GetValue(source) != propertyInfo.GetValue(destination) || destination.Id != source.Id);

            propertyInfo.SetValue(destination, propertyInfo.GetValue(source));
            if (needsCleaning)
                propertyInfo.SetValue(source, null);

            return destination;
        }

        public static IList<int> GetFilteredIds<T>(this IEnumerable<T> list, Func<T, bool> predicate) where T : PocoBase
        {
            return list.Where(predicate).Select(m => m.Id).ToList();
        }
    }
}
