
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Agridea.DataRepository
{
    public class EntityMappings
    {
        #region Members
        private Entities currentDomainModelItems_;
        private ClassMappings classMappings_;
        #endregion

        #region Initialization
        public EntityMappings(Entities entities, ClassMappings classMappings)
        {
            currentDomainModelItems_ = entities;
            classMappings_ = classMappings;
        }
        #endregion

        #region Services
        public EntityType this[EntityType previousEntity]
        {
            get
            {
                string currentEntityName = classMappings_[previousEntity.Name];
                if (currentEntityName == null) return null; //useless see classMappings implicit mapping

                //Checks in the new model if entity does exist
                return currentDomainModelItems_.AllButIgnored().FirstOrDefault(x => x.Name == currentEntityName);
            }
        }
        public string this[string previousEntityName, string previousPropertyName]
        {
            get
            {
                //Look inside the model and if it exists fine, otherwise return null
                string currentPropertyName = classMappings_[previousEntityName, previousPropertyName];
                string currentEntityName = classMappings_[previousEntityName];
                EntityType currentEntityType = currentDomainModelItems_.AllButIgnored().FirstOrDefault(x => x.Name == currentEntityName);
                if (currentEntityType == null) return null;

                EdmProperty currentProperty = currentEntityType.Properties.Where(x => x.Name == currentPropertyName).FirstOrDefault();
                if (currentProperty != null) return currentProperty.Name;
                NavigationProperty currentNavigationProperty = currentEntityType.NavigationProperties.Where(x => x.Name == currentPropertyName).FirstOrDefault();
                if (currentNavigationProperty != null) return currentNavigationProperty.Name;
                return null;
            }
        }
        public string this[string previousEntityName]
        {
            get { return classMappings_.GetNotImplicitMappingType(previousEntityName); }
        }
        #endregion
    }
}
