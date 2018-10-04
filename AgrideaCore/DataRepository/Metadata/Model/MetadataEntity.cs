using System;
using System.Collections.Generic;

namespace Agridea.Metadata
{
    //[Serializable]
    public partial class MetadataEntity : ICloneable
    {
        #region Initialization
        public object Clone()
        {
            var clone = new MetadataEntity();
            clone.Id = Id;
            CopyTo(clone);

            clone.MetadataFieldList = MetadataFieldList.Clone();
            clone.MetadataNavigationPropertyList = MetadataNavigationPropertyList.Clone();

            return clone;
        }

        #endregion
    }
}