
using Agridea.DataRepository;
using System;
using System.Collections.Generic;
namespace Agridea.Security
{

    /// <summary>
    /// http://en.wikipedia.org/wiki/Role-based_access_control
    /// Clone should be in this assembly and the rest in Acorda2 but partial classes inter assemblies impossibles.
    /// Extensions are possible but not for properties.
    /// </summary>
    //[Serializable]
    public partial class Role : ICombo
    {
        #region Constants
        public const string AnyRole = "*";
        public const string DisconnectedRole = "disconnected";
        public const string BuiltInDemoRole = "demo";
        public const string BuiltInFarmerRole = "farmer";
        public const string BuiltInAppointeeRole = "appointee";
        public const string BuiltInInspectorRole = "inspector";
        public const string BuiltInAdminRole = "admin";
        public const string BuiltInAdminOperatorRole = "adminoperator";
        public const string BuiltInFillOperatorRole = "filloperator";
        public const string BuiltInMandataireRole = "Mandataire";
        public const string BuiltInValidatorRole = "validator";
        //These two roles are builtin because they need by default access to all farms : see CanManageAllFarms()
        public const string BuiltInPerInspectorRole = "perInspector";
        public const string BuiltInScavRole = "SCAV";
        #endregion

        #region Initialization
        public Role Clone()
        {
            var clone = new Role();
            clone.Id = Id;
            CopyTo(clone);

            clone.PermissionList = new List<Permission>();
            foreach (var permission in PermissionList)
            {
                var clonePermission = permission.Clone();
                clone.PermissionList.Add(clonePermission);
                clonePermission.Role = clone;
            }
            //Dont handle UserList
            return clone;
        }
        #endregion




        #region Implementation of ICombo
        public string ComboText
        {
            get { return Description + " (" + Name + ")"; }
        }
        public Func<IPocoBase, object> SortFunc
        {
            get { return m => Description; }
        }
        #endregion
    }
}