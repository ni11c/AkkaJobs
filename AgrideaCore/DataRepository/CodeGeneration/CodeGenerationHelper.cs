
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using Agridea.Diagnostics.Contracts;

namespace Agridea.DataRepository
{
    public class CodeGenerationHelper
    {
        public static string GetTableName<T>()
        {
            return string.Format("{0}", typeof(T).Name);
        }
        public static string GetTableName<T1, T2>(string associationName)
        {
            //return string.Format("{0}-{1}-{2}", typeof(T1).Name, typeof(T2).Name, associationName);
            return string.Format("{0}", associationName);
        }

        public static bool Ignore(EntityType entity)
        {
            if (entity.Documentation == null) return false;
            if (entity.Documentation.LongDescription == null) return false;
            return entity.Documentation.LongDescription.ToLower().Equals(CodeGenerationConstants.Ignored);
        }
        public static bool IgnoreButInterAssembly(EntityType entity)
        {
            if (entity.Documentation == null) return false;
            if (entity.Documentation.LongDescription == null) return false;
            return entity.Documentation.LongDescription.ToLower().Equals(CodeGenerationConstants.IgnoredButInterAssembly);
        }
        public static bool Ignore(NavigationProperty property)
        {
            return //Ignore(property.ToEndMember.GetEntityType()) || 
                Ignore(property.FromEndMember.GetEntityType()) ||
                IgnoreProperty(property);
        }
        public static bool IgnoreProperty(EdmMember property)
        {
            if (property.Documentation == null) return false;
            if (property.Documentation.LongDescription == null) return false;
            return property.Documentation.LongDescription.ToLower().Equals(CodeGenerationConstants.Ignored);
        }

        public static bool Ignore(EdmProperty property)
        {
            if (property.Name.ToLower() == CodeGenerationConstants.Id) return true;
            return IgnoreProperty(property);
        }
        public static bool IndexProperty(EdmMember property)
        {
            if (property.Documentation == null) return false;
            if (property.Documentation.LongDescription == null) return false;
            return property.Documentation.LongDescription.ToLower().Equals(CodeGenerationConstants.Indexed);
        }

        public static bool IsComputed(MetadataItem property, EntityType entity)
        {
            if (property.Documentation == null) return false;
            if (property.Documentation.LongDescription == null) return false;
            return property.Documentation.LongDescription.ToLower().Split('~').Any(x => x == CodeGenerationConstants.Computed);
        }

        public static bool IsDiscriminant(MetadataItem property, EntityType entity)
        {
            if (property.Documentation == null) return false;
            if (property.Documentation.LongDescription == null) return false;
            return property.Documentation.LongDescription.ToLower().Split('~').Any(x => x == CodeGenerationConstants.Discriminant);
        }
        public static bool IsReference(EntityType entity)
        {
            if (entity.Documentation == null) return false;
            if (entity.Documentation.LongDescription == null) return false;
            return entity.Documentation.LongDescription.ToLower().Split('~').Any(x => x == CodeGenerationConstants.Reference);
        }
        public static bool IsDimension(EntityType entity)
        {
            if (entity.Documentation == null) return false;
            if (entity.Documentation.LongDescription == null) return false;
            return entity.Documentation.LongDescription.ToLower().Split('~').Any(x => x == CodeGenerationConstants.Dimension);
        }
        public static bool HasDiscriminant(EntityType entity)
        {
            return PrimitiveProperties(entity).Any(p => IsDiscriminant(p, entity)) ||
                NavigationProperties(entity).Any(p => IsDiscriminant(p, entity));
        }
        public static bool HasDiscriminantIncludingInherited(EntityType entity)
        {
            return AllDiscriminatingProperties(entity).Any();
        }
        public static bool HasComment(MetadataItem item)
        {
            if (item.Documentation == null) return false;
            if (item.Documentation.Summary == null) return false;
            return item.Documentation.Summary.Length > 0;
        }
        public static bool HasBaseType(EntityType entity)
        {
            return entity.BaseType != null;
        }

        public static bool IsPrimitive(EdmMember property)
        {
            return !(property is NavigationProperty);
        }
        public static bool IsOptional(NavigationProperty property)
        {
            return property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne;
        }
        public static bool IsMandatory(NavigationProperty property)
        {
            return property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One;
        }
        public static bool IsManyToMany(NavigationProperty property)
        {
            return property.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many &&
                   property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
        }
        public static bool IsZeroOrOneToMany(NavigationProperty property)
        {
            return property.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne &&
                   property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many;
        }
        public static bool IsZeroOrOneToZeroOrOne(NavigationProperty property)
        {
            return property.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne &&
                   property.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne;
        }
        public static bool IsLexicographicallyOrdered(NavigationProperty property)
        {
            return property.FromEndMember.GetEntityType().Name.CompareTo(property.ToEndMember.GetEntityType().Name) <= 0 &&
                 property.FromEndMember.Name.CompareTo(property.ToEndMember.Name) < 0;
        }
        public static bool IsSymetric(NavigationProperty property)
        {
            AssociationTypes associationType = GetAssociationType(property);
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.OneToOne;
        }
        public static bool IsPrincipal(NavigationProperty property)
        {
            //return !IsSortedLexicographically(property);

            if (property.Documentation == null) return false;
            if (property.Documentation.LongDescription == null) return false;
            return property.Documentation.LongDescription.ToLower() == CodeGenerationConstants.Principal;
        }
        private static bool IsDerivedTypeOf(EdmType derivedTypeCandidate, EntityType entityType)
        {
            if (derivedTypeCandidate.BaseType == null) return false;
            if (derivedTypeCandidate.BaseType.Name == entityType.Name) return true;
            return false;
        }

        public static bool IsFromSingleToMany(NavigationProperty property)
        {
            return IsFromSingleToMany(GetAssociationType(property));
        }
        public static bool IsFromSingleToMany(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToMany ||
                associationType == AssociationTypes.OneToMany;
        }
        public static bool IsFromSingleToOne(NavigationProperty property)
        {
            return IsFromSingleToOne(GetAssociationType(property));
        }
        public static bool IsFromSingleToOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.OneToOne;
        }
        public static bool IsFromMany(NavigationProperty property)
        {
            return IsFromMany(GetAssociationType(property));
        }
        public static bool IsFromMany(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ManyToMany ||
                associationType == AssociationTypes.ManyToZeroOrOne ||
                associationType == AssociationTypes.ManyToOne;
        }
        public static bool IsFromManyToSingle(NavigationProperty property)
        {
            return IsFromManyToSingle(GetAssociationType(property));
        }
        public static bool IsFromManyToSingle(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ManyToZeroOrOne ||
                associationType == AssociationTypes.ManyToOne;
        }
        public static bool IsFromSingle(NavigationProperty property)
        {
            return IsFromSingle(GetAssociationType(property));
        }
        public static bool IsFromSingle(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.OneToZeroOrOne ||
                associationType == AssociationTypes.OneToOne ||
                associationType == AssociationTypes.ZeroOrOneToMany ||
                associationType == AssociationTypes.OneToMany;
        }
        public static bool IsFromOne(NavigationProperty property)
        {
            return IsFromOne(GetAssociationType(property));
        }
        public static bool IsFromOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.OneToZeroOrOne ||
                associationType == AssociationTypes.OneToOne ||
                associationType == AssociationTypes.OneToMany;
        }
        public static bool IsFromZeroOrOne(NavigationProperty property)
        {
            return IsFromZeroOrOne(GetAssociationType(property));
        }
        public static bool IsFromZeroOrOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.ZeroOrOneToMany;
        }
        public static bool IsFromZeroOrOneToOne(NavigationProperty property)
        {
            return IsFromZeroOrOne(GetAssociationType(property));
        }
        public static bool IsFromZeroOrOneToOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToOne;
        }
        public static bool IsFromSingleToSingle(NavigationProperty property)
        {
            return IsFromSingleToSingle(GetAssociationType(property));
        }
        public static bool IsFromSingleToSingle(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.OneToZeroOrOne ||
                associationType == AssociationTypes.OneToOne;
        }

        public static bool IsToMany(NavigationProperty property)
        {
            return IsToMany(GetAssociationType(property));
        }
        public static bool IsToMany(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToMany ||
                associationType == AssociationTypes.OneToMany ||
                associationType == AssociationTypes.ManyToMany;
        }
        public static bool IsToSingle(NavigationProperty property)
        {
            return IsToSingle(GetAssociationType(property));
        }
        public static bool IsToSingle(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.OneToZeroOrOne ||
                associationType == AssociationTypes.OneToOne ||
                associationType == AssociationTypes.ManyToZeroOrOne ||
                associationType == AssociationTypes.ManyToOne;
        }
        public static bool IsToOne(NavigationProperty property)
        {
            return IsToOne(GetAssociationType(property));
        }
        public static bool IsToOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToOne ||
                associationType == AssociationTypes.OneToOne ||
                associationType == AssociationTypes.ManyToOne;
        }
        public static bool IsToZeroOrOne(NavigationProperty property)
        {
            return IsToZeroOrOne(GetAssociationType(property));
        }
        public static bool IsToZeroOrOne(AssociationTypes associationType)
        {
            return
                associationType == AssociationTypes.ZeroOrOneToZeroOrOne ||
                associationType == AssociationTypes.OneToZeroOrOne ||
                associationType == AssociationTypes.ManyToZeroOrOne;
        }

        public static string Comment(MetadataItem item, string defaultComment = "")
        {
            if (!HasComment(item)) return defaultComment;
            return item.Documentation.Summary;
        }
        public static string GetBaseType(EntityType entity)
        {
            return string.Format(" : {0}", HasBaseType(entity) ? entity.BaseType.Name : Parameters.CurrentPocoBase);
        }
        public static EdmType GetRootType(EdmType entity)
        {
            if (entity.BaseType == null) return entity;
            return GetRootType(entity.BaseType);
        }
        public static int GetInheritanceDepth(EdmType entity)
        {
            if (entity.BaseType == null) return 0;
            return GetInheritanceDepth(entity.BaseType) + 1;
        }
        public static IList<EntityType> GetRelativesWithCommonAncestor(EntityType entity, IList<EntityType> entities)
        {
            return entities
                .Where(x => entity.BaseType != null &&
                    x.Name != entity.Name &&  //Not the entity it-self
                    GetRootType(x).Name != x.Name &&  //Not the ancestor it-self
                    GetRootType(x).Name == GetRootType(entity).Name) //Relation can be father/son, brothers, cousins...
                .ToList();
        }
        public static bool NeedsDiscriminatorInBaseTable(EntityType entity, IList<EntityType> entities)
        {
            return GetInheritanceType(entity) == InheritanceTypes.TablePerHierarchy &&
                (!GetRootType(entity).Abstract || GetRelativesWithCommonAncestor(entity, entities).Count > 0);
        }
        public static AssociationTypes GetAssociationType(NavigationProperty property)
        {
            switch (property.FromEndMember.RelationshipMultiplicity)
            {
                case RelationshipMultiplicity.Many:
                    switch (property.ToEndMember.RelationshipMultiplicity)
                    {
                        case RelationshipMultiplicity.Many:
                            return AssociationTypes.ManyToMany;
                        case RelationshipMultiplicity.ZeroOrOne:
                            return AssociationTypes.ManyToZeroOrOne;
                        case RelationshipMultiplicity.One:
                            return AssociationTypes.ManyToOne;
                        default:
                            throw new ApplicationException("Impossible case");
                    }
                case RelationshipMultiplicity.ZeroOrOne:
                    switch (property.ToEndMember.RelationshipMultiplicity)
                    {
                        case RelationshipMultiplicity.Many:
                            return AssociationTypes.ZeroOrOneToMany;
                        case RelationshipMultiplicity.ZeroOrOne:
                            return AssociationTypes.ZeroOrOneToZeroOrOne;
                        case RelationshipMultiplicity.One:
                            return AssociationTypes.ZeroOrOneToOne;
                        default:
                            throw new ApplicationException("Impossible case");
                    }
                case RelationshipMultiplicity.One:
                    switch (property.ToEndMember.RelationshipMultiplicity)
                    {
                        case RelationshipMultiplicity.Many:
                            return AssociationTypes.OneToMany;
                        case RelationshipMultiplicity.ZeroOrOne:
                            return AssociationTypes.OneToZeroOrOne;
                        case RelationshipMultiplicity.One:
                            return AssociationTypes.OneToOne;
                        default:
                            throw new ApplicationException("Impossible case");
                    }
                default:
                    throw new ApplicationException("Impossible case");
            }
        }

        public static IEnumerable<EntityType> AllEntities(EdmItemCollection items)
        {
            return items.GetItems<EntityType>().OrderBy(e => e.Name);
        }
        public static IEnumerable<EntityType> GetEntities(EdmItemCollection items)
        {
            return items.GetItems<EntityType>().Where(e => !Ignore(e)).OrderBy(e => e.Name);
        }
        public static InheritanceTypes GetInheritanceType(EntityType entity)
        {
            if (!HasBaseType(entity)) return InheritanceTypes.None;
            EdmType rootType = GetRootType(entity);
            if (rootType.Documentation == null) return InheritanceTypes.TablePerHierarchy;
            if (rootType.Documentation.LongDescription == null) return InheritanceTypes.TablePerHierarchy;
            if (rootType.Documentation.LongDescription == CodeGenerationConstants.TablePerType) return InheritanceTypes.TablePerType;
            return InheritanceTypes.TablePerHierarchy;
        }
        public static IList<EntityType> GetDerivedTypes(EntityType entity, IList<EntityType> entities)
        {
            return entities.Where(x => IsDerivedTypeOf(x, entity)).ToList();
        }
        public static bool HasDerivedTypeIn(EntityType entity, IList<EntityType> entities)
        {
            return entities.Any(e => IsDerivedTypeOf(e, entity));
        }

        public static IEnumerable<EdmMember> DiscriminatingProperties(EntityType entity)
        {
            return entity.Properties.Where(p => IsDiscriminant(p, entity)).OrderBy(p => p.Name);
        }
        public static IEnumerable<NavigationProperty> DiscriminatingNavigationProperties(EntityType entity)
        {
            return entity.NavigationProperties.Where(p => IsDiscriminant(p, entity)).OrderBy(p => p.Name);
        }
        public static IEnumerable<EdmMember> AllDiscriminatingProperties(EntityType entity)
        {
            return DiscriminatingProperties(entity).Union(DiscriminatingNavigationProperties(entity)).Where(x => x.Name != "Temps");
        }
        public static IEnumerable<EdmProperty> PrimitiveProperties(EntityType entity, bool includeInherited = false)
        {
            return entity.Properties
                .Where(p => (p.TypeUsage.EdmType is PrimitiveType || p.TypeUsage.EdmType is EnumType) &&
                       (includeInherited || p.DeclaringType == entity) &&
                       !Ignore(p))
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<EdmProperty> PrimitiveSettableProperties(EntityType entity, bool includeInherited = false)
        {
            return PrimitiveProperties(entity, includeInherited).Where(edmProperty => !IsComputed(edmProperty, entity));
        }

        public static IEnumerable<EdmProperty> DecimalPropertiesWithScale(EntityType entity, bool includeInherited = false)
        {
            return PrimitiveProperties(entity, includeInherited)
                .Where(m => m.TypeUsage.EdmType.Name.ToLower() == "decimal" && m.TypeUsage.Facets["Scale"] != null && m.TypeUsage.Facets["Scale"].Value != null);
        }
        public static int GetScale(EdmProperty property)
        {
            return Convert.ToInt32(property.TypeUsage.Facets["Scale"].Value);
        }
        public static IEnumerable<NavigationProperty> NavigationProperties(EntityType entity, bool includeInherited = false)
        {
            return entity.NavigationProperties
                .Where(np => (includeInherited || np.DeclaringType == entity) &&
                       !Ignore(np))
                .OrderBy(np => np.Name);
        }
        public static IEnumerable<NavigationProperty> OptionalProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => np.DeclaringType == entity &&
                        IsOptional(np) &&
                        !Ignore(np))
                .OrderBy(np => np.Name);
        }
        public static IEnumerable<NavigationProperty> MandatoryProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => np.DeclaringType == entity &&
                        IsMandatory(np) &&
                        !Ignore(np))
                .OrderBy(np => np.Name);
        }
        public static IEnumerable<NavigationProperty> ManyToManyProperties(EntityType entity, bool includeInherited = false)
        {
            var manyToManyProperties = entity.NavigationProperties
                .Where(np => (includeInherited || np.DeclaringType == entity) &&
                              IsManyToMany(np) &&
                              (Inverse(np) == null || IsLexicographicallyOrdered(np)) && //to avoid generating both sides of bidir assoc. 
                              !Ignore(np))
                .OrderBy(p => p.Name);
            return manyToManyProperties;
        }
        public static IEnumerable<NavigationProperty> SingleToManyReflexiveProperties(EntityType entity)
        {
            var singleToManyReflexiveProperties = entity.NavigationProperties
                .Where(np => np.DeclaringType == entity && IsFromSingleToMany(np) && !Ignore(np) && entity.Name == np.ToEndMember.GetEntityType().Name);
            return singleToManyReflexiveProperties;
        }
        public static bool IsReflexive(NavigationProperty navigationProperty)
        {
            return navigationProperty.ToEndMember.GetEntityType().Name == navigationProperty.FromEndMember.GetEntityType().Name;
        }
        public static IEnumerable<NavigationProperty> ToManyProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => IsToMany(np))
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<NavigationProperty> FromSingleToManyProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => np.DeclaringType == entity &&
                              IsFromSingleToMany(np) &&
                              !Ignore(np))
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<NavigationProperty> FromManyToSingleProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => np.DeclaringType == entity &&
                              IsFromManyToSingle(np) &&
                              !Ignore(np))
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<NavigationProperty> ToSingleProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => IsToSingle(np))
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<NavigationProperty> FromSingleProperties(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(np => np.DeclaringType == entity &&
                              IsFromSingle(np) &&
                              !Ignore(np))
                .OrderBy(p => p.Name);
        }
        public static IList<EntityType> GetAbstractEntities(IList<EntityType> entities)
        {
            return entities
                .Where(e => e.Abstract)
                .OrderBy(x => x.Name)
                .ToList();
        }
        public static NavigationProperty Inverse(NavigationProperty navigationProperty)
        {
            EntityType toEntity = TargetEntity(navigationProperty);
            if (toEntity == null) return null;

            return toEntity.NavigationProperties
                .SingleOrDefault(n => Object.ReferenceEquals(n.RelationshipType, navigationProperty.RelationshipType) && !Object.ReferenceEquals(n, navigationProperty));
        }
        public static EntityType TargetEntity(NavigationProperty property)
        {
            if (property == null) return null;
            return property.ToEndMember.GetEntityType();
        }
        public static IEnumerable<EntityType> GetOneToManyTargets(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(p => IsFromOne(p) && IsToMany(p))
                .Select(p => TargetEntity(p));
        }
        public static IEnumerable<EntityType> GetOneToZeroOrOneTargets(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(p => IsFromOne(p) && IsToZeroOrOne(p))
                .Select(p => TargetEntity(p));
        }
        public static IEnumerable<EntityType> GetOneToOneTargets(EntityType entity)
        {
            return entity.NavigationProperties
                .Where(p => IsFromOne(p) && IsToOne(p))
                .Select(p => TargetEntity(p));
        }
        public static IList<EntityType> ExceptAbstractAndDerivedTypes(IList<EntityType> entities)
        {
            var baseTypes = GetAbstractEntities(entities);
            var derived = baseTypes.ToList().Select(t => GetDerivedTypes(t, entities)).SelectMany(x => x);
            return entities.Except(baseTypes).Except(derived).ToList();
        }
        public static IList<EntityType> ExceptHierarchies(IList<EntityType> entities)
        {
            return entities.Where(e =>
                !HasBaseType(e) &&
                !HasDerivedTypeIn(e, entities))
                .ToList();
        }
        public static IList<string> BuildRecursiveDiscriminantProperties(EntityType entityType)
        {
            var result = new List<string>();
            BuildRecursiveDiscriminantProperties(entityType, null, result);
            return result;
        }
        private static void BuildRecursiveDiscriminantProperties(EntityType type, string currentPropertyPath, IList<string> result)
        {
            var discriminantProperties = AllDiscriminatingProperties(type).ToList();
            foreach (var discriminantProperty in discriminantProperties)
            {
                string newCurrentPropertyPath =  (currentPropertyPath != null)
                    ? currentPropertyPath + "." + discriminantProperty.Name
                    : discriminantProperty.Name;

                if (discriminantProperty is NavigationProperty)
                    BuildRecursiveDiscriminantProperties((discriminantProperty as NavigationProperty).ToEndMember.GetEntityType(), newCurrentPropertyPath, result);
                else
                    result.Add(newCurrentPropertyPath);
            }
        }
         public static IList<string> BuildRecursiveDiscriminantPropertiesForNaturalKey(EntityType entityType)
        {
            var result = new List<string>();
            BuildRecursiveDiscriminantPropertiesForNaturalKey(entityType, null, result);
            return result;
        }
        private static void BuildRecursiveDiscriminantPropertiesForNaturalKey(EntityType type, string currentPropertyPath, IList<string> result)
        {
            var discriminantProperties = AllDiscriminatingProperties(type).ToList();
            foreach (var discriminantProperty in discriminantProperties)
            {
                string newCurrentPropertyPath = (currentPropertyPath != null)
                    ? currentPropertyPath + "." + discriminantProperty.Name
                    : discriminantProperty.Name;

                if (discriminantProperty is NavigationProperty)
                    BuildRecursiveDiscriminantPropertiesForNaturalKey((discriminantProperty as NavigationProperty).ToEndMember.GetEntityType(),newCurrentPropertyPath, result);
                else
                    result.Add(newCurrentPropertyPath +(discriminantProperty.TypeUsage.EdmType.Name == typeof(string).Name
                      ? ""
                      : ".ToString()"));
            }
        }
        public static bool IsUnchanged(EntityType previous, EntityType current)
        {
            if (current == null || current.Abstract) return false;

            // Check that all properties in previous are there at the same index in current
            for (int i = 0; i < previous.Properties.Count; i++)
            {
                var property = previous.Properties[i];
                if (IsPocoBaseOrNavigation(property)) continue;

                int numCurrentProperties = current.Properties.Count;
                if (i >= numCurrentProperties || property.Name != current.Properties[i].Name)
                    return false;
            }

            // check that no property was added in current
            if (current.Properties
                .Where(p => !IsPocoBaseOrNavigation(p))
                .Any(p => !previous.Properties.Select(pp => pp.Name).Contains(p.Name)))
                return false;

            return true;
        }
        private static bool IsPocoBaseOrNavigation(EdmProperty property)
        {
            return
                property.Name == CodeGenerationConstants.Id_Uppercase ||
                property.Name == CodeGenerationConstants.CreationDate ||
                property.Name == CodeGenerationConstants.CreatedBy ||
                property.Name == CodeGenerationConstants.ModificationDate ||
                property.Name == CodeGenerationConstants.ModifiedBy ||
                (property as EdmMember) is NavigationProperty;
        }
        public static bool HasOneToOneNavigationProperty(EntityType entity)
        {
            return entity.NavigationProperties.Any(p => IsToOne(p) && IsFromOne(p));
        }
        public static bool HasOneToZeroOrOneNavigationProperty(EntityType entity)
        {
            return entity.NavigationProperties.Any(p => IsToZeroOrOne(p) && IsFromOne(p));
        }
        public static bool HasOneToManyNavigationProperty(EntityType entity)
        {
            return entity.NavigationProperties.Any(p => IsToMany(p) && IsFromOne(p));
        }
        public static bool ParticipatesInManyToManyRelationShip(EntityType entity)
        {
            return ManyToManyProperties(entity).Count() > 0;
        }
        public static NavigationProperty GetNavigationProperty(EntityType entity, string propertyName)
        {
            return entity.NavigationProperties.FirstOrDefault(np => np.Name == propertyName);
        }

        public static IEnumerable<MetadataItem> GetEnumMembers(EdmType enumType)
        {
            Requires<ArgumentNullException>.IsNotNull(enumType);

            var membersProperty = enumType.GetType().GetProperty("Members");
            return membersProperty != null
                ? membersProperty.GetValue(enumType, null) as IEnumerable<MetadataItem>
                : Enumerable.Empty<MetadataItem>();
        }

        public static object GetEnumMemberValue(MetadataItem enumMember)
        {
            Requires<ArgumentNullException>.IsNotNull(enumMember);

            var valueProperty = enumMember.GetType().GetProperty("Value");
            return valueProperty == null ? null : valueProperty.GetValue(enumMember, null);
        }

        public static string GetEnumMemberName(MetadataItem enumMember)
        {
            Requires<ArgumentNullException>.IsNotNull(enumMember);

            var nameProperty = enumMember.GetType().GetProperty("Name");
            return nameProperty == null ? null : (string)nameProperty.GetValue(enumMember, null);
        }
    }

    public static class RelationshipMultiplicityExtensions
    {
        public static string ToDisplayString(this RelationshipMultiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case RelationshipMultiplicity.One:
                    return "1";

                case RelationshipMultiplicity.ZeroOrOne:
                    return "0..1";

                case RelationshipMultiplicity.Many:
                    return "*";

                default:
                    return "non défini";
            }
        }
    }
}
