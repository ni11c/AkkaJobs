using System;

namespace Agridea.ObjectMapping
{
    [Serializable]
    public class MapContext
    {
        #region Members
        public Type SourceType { get; private set; }
        public Type TargetType { get; private set; }
        #endregion

        #region Initialization
        public void Clear()
        {
            SourceType = TargetType = null;
        }
        public bool Exists
        {
            get { return SourceType != null && TargetType != null; }
        }
        public void Set(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
        #endregion
    }
}
