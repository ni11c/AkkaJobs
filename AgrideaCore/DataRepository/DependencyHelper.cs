using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Agridea.DataRepository
{
    using Agridea.Diagnostics.Logging;
    using Microsoft.SqlServer.Management.Common;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Watch the step : from a relationnal DB perspective, e.g. AnimalCategory has a list of AnimalType 
    /// so only AnimalType depends on AnimalCategory (not the opposite because of foreign key existence...)
    /// </summary>
    public class DependencyHelper
    {
        #region Members
        #endregion

        #region Services
        public void Display(IList<Type> types, IList<Type> dependentTypes)
        {
            foreach (var type in types)
            {
                var typeRank = dependentTypes.IndexOf(type);
                Console.WriteLine("{0}:{1} {2}", typeRank, type.Name, type.IsDimensionEntity() ? "DIM" : "FACT");
                foreach (var referenceProperty in type.GetReferenceProperties())
                {
                    var referencePropertyType = referenceProperty.PropertyType;
                    var referencePropertyTypeRank = referencePropertyType.Name == "Temps" ? -2 : dependentTypes.IndexOf(referencePropertyType);
                    var required = referenceProperty.IsMandatory() ? "!" : (referenceProperty.IsDiscriminant() ? "!!" : "?");
                    Console.WriteLine("  {0} : {1} {2} {3}", referenceProperty.Name, referencePropertyType.Name, required, referencePropertyTypeRank);
                }       
                foreach (var collectionProperty in type.GetCollectionProperties())
                {
                    var collectionItemType = collectionProperty.PropertyType.GenericTypeArguments[0];
                    var collectionItemTypeRank = dependentTypes.IndexOf(collectionItemType);
                    var dependentTypesContains = dependentTypes.Contains(collectionItemType);
                    Console.WriteLine("  {0} : {1} * {2}", collectionProperty.Name, collectionItemType.Name, collectionItemTypeRank);
                }
            }
        }
        /// <summary>
        /// </summary>
        /// <param name="types">input types to consider</param>
        /// <param name="unhandledTypes">if cannot converge, the set of types not handled</param>
        /// <returns>ordered type list, fist is the more independent</returns>
        public List<Type> GetDependencyOrder(List<Type> types, List<Type> excludedTypes, List<Type> unhandledTypes)
        {
            var toTreatEntities = types;
            var treatedEntities = new List<Type>().Concat(excludedTypes).ToList();

            int previousToTreatEntitiesCount = 0;
            /// BDF (breadth first search)  algorithm
            do
            {
                if (previousToTreatEntitiesCount == toTreatEntities.Count)
                {
                    unhandledTypes.AddRange(toTreatEntities);
                    break;
                }
                previousToTreatEntitiesCount = toTreatEntities.Count;

                var readyToTreatEntities = GetReadyToTreatEntities(toTreatEntities, treatedEntities);

                treatedEntities = treatedEntities.Concat(readyToTreatEntities).ToList();
                toTreatEntities = toTreatEntities.Where(x => !treatedEntities.Contains(x)).ToList();

            } while (toTreatEntities.Count > 0);

            return treatedEntities.Where(x => !excludedTypes.Contains(x)).ToList();
        }

        /// <summary>
        /// Transitive closure
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <param name="collectiondAlso">if true includes also recursively all types used in collections, if false only strict dependencies from a db perspective</param>
        public void GetDependeeTypes(Type type, List<Type> result, bool collectiondAlso = false)
        {
            var dependeeTypes = new List<Type>();

            dependeeTypes.AddRange(type.GetReferenceProperties().Select(x => x.PropertyType).Where(y => !result.Contains(y)));
            if(collectiondAlso) dependeeTypes.AddRange(type.GetCollectionProperties().Select(x => x.PropertyType.GenericTypeArguments[0]).Where(y => !result.Contains(y)));
            result.AddRange(dependeeTypes);

            /// BDF (breadth first search)  algorithm
            foreach (var dependeeType in dependeeTypes) GetDependeeTypes(dependeeType, result, collectiondAlso);
        }
        #endregion

        #region Helpers
        private List<Type> GetReadyToTreatEntities(IList<Type> toTreatEntities, IList<Type> treatedEntities)
        {
            return toTreatEntities.Where(x => x.GetReferenceProperties().All(
                y => (!y.IsMandatory() && !y.IsDiscriminant()) || treatedEntities.Contains(y.PropertyType))).ToList();
        }
        #endregion
    }
}