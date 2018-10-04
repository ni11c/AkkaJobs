
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Agridea.DataRepository
{
    public class Entities
    {

        #region constants

        private const string externalTypeNameAttributeName = @"http://schemas.microsoft.com/ado/2006/04/codegeneration:ExternalTypeName";

        #endregion
        #region Members
        private IList<EntityType> entities_ = new List<EntityType>();
        private IList<EnumType> enumTypes_ = new List<EnumType>();

        #endregion

        #region Services
        public void Add(IEnumerable<GlobalItem> entities)
        {
            entities_ = entities_.Concat(entities.OfType<EntityType>()).ToList();
            enumTypes_ = enumTypes_.Concat(entities.OfType<EnumType>().Where(e => e.MetadataProperties.All(x => x.Name != externalTypeNameAttributeName))).ToList();
        }
        public IList<EntityType> All()
        {
            return entities_.OrderBy(e => e.Name).ToList();
        }
        public IList<EntityType> AllIncludingIgnoredButInterAssembly()
        {
            return entities_.Where(e => !CodeGenerationHelper.Ignore(e)).OrderBy(e => e.Name).ToList();
        }
        public IList<EntityType> AllButIgnored()
        {
            return entities_.Where(e => !CodeGenerationHelper.Ignore(e) && !CodeGenerationHelper.IgnoreButInterAssembly(e)).OrderBy(e => e.Name).ToList();
        }

        public IList<EnumType> AllEnumTypes()
        {
            return enumTypes_.OrderBy(e => e.Name).ToList();
        }
        public IList<EntityType> AllExceptIgnoredAbstractAndDerived()
        {
            var allButIgnored = AllButIgnored();
            var abstractEntities = CodeGenerationHelper.GetAbstractEntities(allButIgnored);
            IEnumerable<EntityType> derived = new List<EntityType>();
            foreach (var entity in abstractEntities)
                derived = derived.Concat(CodeGenerationHelper.GetDerivedTypes(entity, allButIgnored));

            return allButIgnored
                .Except(abstractEntities)
                .Except(derived)
                .ToList();
        }
        public EntityType this[string entityName]
        {
            get { return entities_.Where(x => x.Name == entityName).FirstOrDefault(); }
        }
        public EntityType this[EntityType entity]
        {
            get { return this[entity.Name]; }
        }
        #endregion
    }
}
