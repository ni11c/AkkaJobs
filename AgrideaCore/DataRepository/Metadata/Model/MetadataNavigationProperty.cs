using Agridea.DataRepository;
using Microsoft.Ajax.Utilities;
using System;

namespace Agridea.Metadata
{
    //[Serializable]
    public partial class MetadataNavigationProperty : ICloneable
    {
        #region Initialization

        public object Clone()
        {
            var clone = new MetadataNavigationProperty();
            clone.Id = Id;
            CopyTo(clone);
            return clone;
        }

        #endregion Initialization

        #region Properties

        [Transient]
        public MultiplicityTypes ToMultiplicity
        {
            get { return (MultiplicityTypes)ToMultiplicity_; }
            set { ToMultiplicity_ = Convert.ToInt32(value); }
        }

        [Transient]
        public MultiplicityTypes FromMultiplicity
        {
            get { return (MultiplicityTypes) FromMultiplicity_; }
            set { FromMultiplicity_ = Convert.ToInt32(value); }
        }

        #endregion Properties
    }

    public static class MultiplicityTypesExtensions
    {
        public static string ToDisplayString(this MultiplicityTypes multiplicity)
        {
            switch (multiplicity)
            {
                case MultiplicityTypes.OneExactly:
                    return "1";

                case MultiplicityTypes.ZeroOrOne:
                    return "0..1";

                case MultiplicityTypes.ZeroOrMany:
                    return "*";

                default:
                    return "non défini";
            }
        }
    }
}