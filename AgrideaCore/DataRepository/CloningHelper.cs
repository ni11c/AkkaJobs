using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Agridea.Diagnostics.Logging;

namespace Agridea.DataRepository
{
    public class CloningHelper
    {
        private Dictionary<IPocoBase, IPocoBase> clones_;
        private Dictionary<IPocoBase, bool> deleted_;
        private Dictionary<string, int> indexForProperty_;
        private Type[] excludedPocos_;

        public CloningHelper(Type[] excludedPocos)
        {
            excludedPocos_ = excludedPocos;
            Reset();
        }
        public void Reset()
        {
            clones_ = new Dictionary<IPocoBase, IPocoBase>();
            deleted_ = new Dictionary<IPocoBase, bool>();
            indexForProperty_ = new Dictionary<string, int>();
        }

        /// <summary>
        /// Clones a whole tree under input.
        /// ReferenceEntity are copied by ref, all others by values
        /// Dicrimimants are changed to maintain UC, except when pragmatic condition is met
        /// </summary>
        public IPocoBase Clone(IPocoBase input, IAgrideaService service)
        {
            if (input == null) return null;
            var pocoType = ObjectContext.GetObjectType(input.GetType());
            var pocoTypeName = pocoType.Name;
            if (excludedPocos_.Contains(pocoType)) return null;
            if (input.GetType().IsReferenceEntity()) return input;
            if (clones_.ContainsKey(input)) return clones_[input];

            Log.Verbose("CloningHelper.Clone ({0})", input);
            IPocoBase result = Activator.CreateInstance(input.GetType()) as IPocoBase;
            result.CreatedBy = service.UserName;
            result.CreationDate = DateTime.Now;
            clones_.Add(input, result);

            var discriminantProperties = input.GetType().GetPrimitiveDiscriminantProperties().ToList();
            var generateNewDiscriminant = discriminantProperties.All(m => m.IsPrimitive() || discriminantProperties.Count() == 1);
            foreach (var primitiveProperty in input.GetType().GetPrimitiveProperties())
            {
                var val = primitiveProperty.GetValue(input);
                if (primitiveProperty.IsDiscriminant())
                {
                    //bug for BankAccount Ccp Number and Iban => discriminant but potentially null
                    val = val is string && string.IsNullOrWhiteSpace(val.ToString()) ? "-" : val;
                    val = generateNewDiscriminant ? NextDicriminantValue(primitiveProperty, val) : val;
                }
                primitiveProperty.SetValue(result, val);
            }
            foreach (var referenceProperty in input.GetType().GetReferenceProperties())
            {
                var reference = referenceProperty.GetValue(input) as IPocoBase;
                if (reference == null) continue;
                referenceProperty.SetValue(result, Clone(reference, service));
            }
            foreach (var collectionProperty in input.GetType().GetCollectionProperties())
            {
                var inputCollection = collectionProperty.GetValue(input) as IList;
                if (inputCollection.Count == 0) continue;
                IList inputCollectionClone = Activator.CreateInstance(typeof(List<>).MakeGenericType(collectionProperty.PropertyType.GenericTypeArguments[0])) as IList;
                collectionProperty.SetValue(result, inputCollectionClone);
                foreach (IPocoBase inputItem in inputCollection)
                    inputCollectionClone.Add(Clone(inputItem, service));
            }
            return result;
        }

        /// <summary>
        /// Removes a whole tree under input, this is the 'inverse' of Clone but this is too slow for a whole Canton...
        /// </summary>
        public void Remove(IPocoBase input, IAgrideaService service)
        {
            if (input == null) return;
            var pocoType = ObjectContext.GetObjectType(input.GetType());
            var pocoTypeName = pocoType.Name;
            if (excludedPocos_.Contains(pocoType)) return;
            if (input.GetType().IsReferenceEntity()) return;
            if (deleted_.ContainsKey(input)) return;

            Log.Verbose("CloningHelper.Remove ({0})", input);
            deleted_.Add(input, true);

            foreach (var referenceProperty in input.GetType().GetReferenceProperties())
            {
                var reference = referenceProperty.GetValue(input) as IPocoBase;
                if (reference == null) continue;
                Remove(reference, service);
            }
            foreach (var collectionProperty in input.GetType().GetCollectionProperties())
            {
                var inputCollection = collectionProperty.GetValue(input) as IList;
                if (inputCollection.Count == 0) continue;  
                foreach (IPocoBase inputItem in new ArrayList(inputCollection)) //to avoid collection change during iteration
                    Remove(inputItem, service);
            }
            service.RemoveItem(input); //Must be at the end otherwise all relationships are cleared
        }

        /// <summary>
        /// Case where B navigates to A, A is mandatory but A cant navigate to B is not handled.
        /// </summary>
        public void CleanOrphans(Type pocoType, IAgrideaService service, bool cleanNonDependentPocos = false)
        {
            var instances = service.All(pocoType).ToList();
            var cleantCount = 0;
            foreach (var instance in instances)
            {
                var classHasRelationships = false;
                var instanceHasLinks = false;
                foreach (var referenceProperty in instance.GetType().GetReferenceProperties())
                {
                    classHasRelationships = true;
                    var reference = referenceProperty.GetValue(instance) as IPocoBase;
                    if (reference != null)
                    {
                        instanceHasLinks = true;
                        break;
                    }
                }
                if (instanceHasLinks) continue;
                foreach (var collectionProperty in instance.GetType().GetCollectionProperties())
                {
                    classHasRelationships = true;
                    var inputCollection = collectionProperty.GetValue(instance) as IList;
                    if (inputCollection.Count != 0)
                    {
                        instanceHasLinks = true;
                        break;
                    }
                }
                if (classHasRelationships && instanceHasLinks) continue;
                if (!classHasRelationships && !cleanNonDependentPocos) continue;

                service.RemoveItem(instance);
                cleantCount++;
            }
            service.Save().Reset();
            if(cleantCount > 0) Log.Info("CloningHelper.CleanOrphans ({0}, cleanNonDependentPocos:{1} Total:{2} / Cleant:{3})", pocoType.Name, cleanNonDependentPocos, instances.Count, cleantCount);
        }

        private object NextDicriminantValue(PropertyInfo property, object val)
        {
            var propertyKey = property.DeclaringType.Name + "." + val + "." + property.Name;
            if (!indexForProperty_.ContainsKey(propertyKey)) indexForProperty_.Add(propertyKey, 0);
            var index = indexForProperty_[propertyKey] += 1;

            if (val is string) return val + "_" + index;
            if (val is int) return Convert.ToInt32(val) + 10000 + index;
            if (val is DateTime) return Convert.ToDateTime(val).AddSeconds(index);
            if (val is Guid) return Guid.NewGuid();
            return val;
        }
    }
}