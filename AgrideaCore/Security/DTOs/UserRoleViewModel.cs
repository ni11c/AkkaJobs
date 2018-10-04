using System;
using System.Linq;
using Agridea.Web.UI;

namespace Agridea.Security
{
    public class UserRoleEdit
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IQueryable<UserRoleEditableItem> GridModel { get; set; }
    }
    public class UserRoleEditableItem : IHasId, ICheckChanged
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool RoleCanAccessAllFarms { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public DateTime? CommitmentDate { get; set; }
        public bool IsAllowed { get; set; }

        public bool IsEditable
        {
            get { return !RoleCanAccessAllFarms && IsAllowed && RoleName != Role.BuiltInFarmerRole; }
        }

        #region Implementation of ICheckChanged
        public bool HasChanged { get; set; }
        #endregion
    }
}
