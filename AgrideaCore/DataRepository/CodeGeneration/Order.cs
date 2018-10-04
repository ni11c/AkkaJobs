
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Agridea.DataRepository
{
    public class Order
    {
        #region Services
        /// <summary>
        /// BFS algorithm
        /// </summary>
        public IList<EntityType> GetPartialDependencyOrder(IList<EntityType> entities, out bool converged)
        {
            //Discard abstract entities
            //Consider entities to be treated with principal as done
            //Start with independent entities and consider them as done also
            IList<EntityType> abstractEntities = CodeGenerationHelper.GetAbstractEntities(entities).ToList();
            IList<EntityType> entitiesToTreatWithPrincipal = GetEntitiesToTreatWithPrincipal(entities);
            IList<EntityType> discardedEntities = abstractEntities.Concat(entitiesToTreatWithPrincipal).ToList();

            IList<EntityType> toTreatEntities = entities.Where(x => !discardedEntities.Contains(x)).ToList(); //missing some Diff operation...
            IList<EntityType> treatedEntities = new List<EntityType>();;

            //While not all entities are treated
            converged = true;
            int previousToTreatEntitiesCount = 0;
            do
            {
                if (previousToTreatEntitiesCount == toTreatEntities.Count)
                {
                    converged = false; //Algorithm cannot converge, check the model...
                    break;
                }
                previousToTreatEntitiesCount = toTreatEntities.Count;

                IList<EntityType> readyToTreatEntities = GetReadyToTreatEntities(discardedEntities, treatedEntities, toTreatEntities);

                treatedEntities = treatedEntities.Concat(readyToTreatEntities).ToList();
                toTreatEntities = toTreatEntities.Where(x => !treatedEntities.Contains(x)).ToList();

            } while (toTreatEntities.Count > 0);
            return treatedEntities;
        }
        public IList<EntityType> GetEntitiesToTreatWithPrincipal(IList<EntityType> entities)
        {
            return entities
               .Where(e => HasNonNavigableToOneLinkWithin(e, entities) || IsDependentInOneToOne(e, entities))
               .OrderBy(x => x.Name)
               .ToList();
        }
        public IList<NavigationProperty> GetPropertiesSortedByDependency(IList<NavigationProperty> properties)
        {
            List<NavigationProperty> sortedProperties = new List<NavigationProperty>(properties);
            sortedProperties.Sort(new DependencyComparer(Needs));
            return sortedProperties;
        }
        public IList<EntityType> GetNeededEntities(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => Needs(entity, x)).OrderBy(x => x.Name).ToList();
        }
        public IList<EntityType> GetNeedingEntities(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => Needs(x, entity)).OrderBy(x => x.Name).ToList();
        }
        public IList<EntityType> GetEntitiesWithNonNavigableToOneLinkWith(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => HasNonNavigableToOneLinkFrom(x, entity)).OrderBy(x => x.Name).ToList();
        }
        public IList<EntityType> GetEntitiesWithNavigableToOneLinkWith(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => HasNavigableToOneLinkWith(x, entity)).OrderBy(x => x.Name).ToList();
        }
        public IList<EntityType> GetEntitiesWithNavigableToZeroOrOneLinkWith(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => HasNavigableToZeroOrOneLinkWith(x, entity)).OrderBy(x => x.Name).ToList();
        }
        #endregion

        #region Helpers
        private IList<EntityType> GetReadyToTreatEntities(IList<EntityType> discardedEntities, IList<EntityType> treatedEntities, IList<EntityType> toTreatEntities)
        {
            return toTreatEntities
                .Where(x => x.NavigationProperties
                    .All(y => !CodeGenerationHelper.IsToOne(y) || 
                         discardedEntities.Contains(y.ToEndMember.GetEntityType()) ||
                         treatedEntities.Contains(y.ToEndMember.GetEntityType())))
                .OrderBy(x => x.Name)
                .ToList();
        }
        private IList<EntityType> GetIndependentEntities(IList<EntityType> entities)
        {
            return entities
               .Where(e => IsIndependentWithin(e, entities))
               .OrderBy(x => x.Name)
               .ToList();
        }
        private bool IsIndependentWithin(EntityType entity, IList<EntityType> entities)
        {
            var neededEntities = GetNeededEntities(entity, entities);
            return neededEntities.Count == 0;
        }
        private bool Needs(EntityType entity, EntityType other)
        {
            bool hasNavigableToOneLinkWith = HasNavigableToOneLinkWith(entity, other);
            bool hasManyToManyAndIsGreaterLexicographicallyThan = HasManyToManyAndIsGreaterLexicographicallyThan(entity, other);
            bool hasManyToManyWith = HasManyToManyWith(entity, other);
            bool hasManyToManyWithNoInverseWith = HasManyToManyWithNoInverseWith(entity, other);
            bool hasZeroOrOneToZeroOrOneWithNoInverseWith = HasZeroOrOneToZeroOrOneWithNoInverseWith(entity, other);
            bool hasZeroOrOneToManyWithNoInverseWith = HasZeroOrOneToManyWithNoInverseWith(entity, other);
            return
                //entity != other && //forget about reflexive dependencies
                (hasNavigableToOneLinkWith ||
                hasManyToManyWith ||
                hasManyToManyWithNoInverseWith ||
                hasZeroOrOneToZeroOrOneWithNoInverseWith ||
                hasZeroOrOneToManyWithNoInverseWith);
        }
        private bool HasNonNavigableToOneLinkFrom(EntityType entity, EntityType other)
        {
            return
                entity.NavigationProperties.Any(p =>
                DependsOn(p, other) &&
                CodeGenerationHelper.Inverse(p) == null &&
                p.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
        }
        private bool HasNonNavigableToOneLinkWithin(EntityType entity, IList<EntityType> entities)
        {
            return entities.Any(e => HasNonNavigableToOneLinkWith(e, entity)); //REFACT using GetEntitiesWithNonNavigableToOneLinkWith
        }
        private bool HasNonNavigableToOneLinkWith(EntityType entity, EntityType other)
        {
            return
                entity.NavigationProperties.Any(p =>
                DependsOn(p, other) &&
                CodeGenerationHelper.Inverse(p) == null &&
                p.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
        }
        private bool IsDependentInOneToOne(EntityType entity, IList<EntityType> entities)
        {
            return entities.Any(e => IsDependentInOneToOneWith(entity, e));
        }
        private bool IsDependentInOneToOneWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p =>
                DependsOn(p, other) &&
                CodeGenerationHelper.GetAssociationType(p) == AssociationTypes.OneToOne &&
                CodeGenerationHelper.IsPrincipal(p));
        }
        private bool HasNavigableToOneLinkWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p =>
                DependsOn(p, other) &&
                p.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One);
        }
        private bool HasNavigableToZeroOrOneLinkWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p =>
                DependsOn(p, other) &&
                p.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne);
        }
        private bool HasManyToManyWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p => DependsOn(p, other));
        }
        private bool HasManyToManyAndIsGreaterLexicographicallyThan(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p => DependsOn(p, other) && CodeGenerationHelper.IsManyToMany(p) && !CodeGenerationHelper.IsLexicographicallyOrdered(p));
        }
        private bool HasManyToManyWithNoInverseWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p => DependsOn(p, other) && CodeGenerationHelper.IsManyToMany(p) && CodeGenerationHelper.Inverse(p) == null);
        }
        private bool HasZeroOrOneToZeroOrOneWithNoInverseWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p => DependsOn(p, other) && CodeGenerationHelper.IsZeroOrOneToZeroOrOne(p) && CodeGenerationHelper.Inverse(p) == null);
        }
        private bool HasZeroOrOneToManyWithNoInverseWith(EntityType entity, EntityType other)
        {
            return entity.NavigationProperties.Any(p => DependsOn(p, other) && CodeGenerationHelper.IsZeroOrOneToMany(p) && CodeGenerationHelper.Inverse(p) == null);
        }
        private bool DependsOn(NavigationProperty property, EntityType entity)
        {
            return property.ToEndMember.GetEntityType().Name == entity.Name;
        }

        private class DependencyComparer : IComparer<NavigationProperty>
        {
            Func<EntityType, EntityType, bool> needs_;
            public DependencyComparer(Func<EntityType, EntityType, bool> needs)
            {
                needs_ = needs;
            }
            public int Compare(NavigationProperty x, NavigationProperty y)
            {
                return x.ToEndMember.GetEntityType().Equals(y.ToEndMember.GetEntityType()) ?
                    0 :
                    (needs_(x.ToEndMember.GetEntityType(), y.ToEndMember.GetEntityType()) ? 1 : -1);
            }
        }
        #endregion
    }
}
