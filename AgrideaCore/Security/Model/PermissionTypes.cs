
using System;
namespace Agridea.Security
{
    [Serializable]
    public enum PermissionTypes
    {
        Deny = 0,
        Read = 1,
        Write = 2,
        Create = 4,
        Delete = 8 
    }
}