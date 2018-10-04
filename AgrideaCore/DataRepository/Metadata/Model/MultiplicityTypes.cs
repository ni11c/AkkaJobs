
using System;

namespace Agridea.Metadata
{
    [Serializable]
    public enum MultiplicityTypes
    {
        ZeroOrOne,
        OneExactly,
        ZeroOrMany,
        //OneOrMany can't be set from and Edmx
    }
}