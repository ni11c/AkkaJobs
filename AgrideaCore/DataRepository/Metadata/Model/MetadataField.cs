
using System;
namespace Agridea.Metadata
{
    //[Serializable]
    public partial class MetadataField : ICloneable
    {
        #region Initialization
        public object Clone()
        {
            var clone = new MetadataField();
            clone.Id = Id;
            CopyTo(clone);
            return clone;
        }
        #endregion
    }
}