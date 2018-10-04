using Agridea.DataRepository;
using System;

namespace Agridea.Security
{
    //[Serializable]
    public partial class Permission
    {
        public Permission Clone()
        {
            var clone = new Permission();
            clone.Id = Id;
            CopyTo(clone);
            //Dont handle Role
            return clone;
        }

    }
}