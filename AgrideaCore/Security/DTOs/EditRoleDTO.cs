using System.Collections.Generic;

namespace Agridea.Security
{
    public class EditRoleDTO
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<PermissionDTO> Permissions { get; set; }
        public bool CanAccessAllFarms { get; set; }
        public PermissionTreeViewModel PermissionTreeByMvcActions { get; set; }
        public PermissionTreeViewModel PermissionTreeByMenu { get; set; } 

        public EditRoleDTO()
        {
            Permissions = new List<PermissionDTO>();
        }

        public override string ToString()
        {
            return string.Format("[{0} Name='{1}', Description='{2}', Permissions.Count='{3}' CanAccessAllFarms='{4}']",
                GetType().Name,
                Name,
                Description,
                Permissions.Count,
                CanAccessAllFarms
            );
        }
    }

    public class EditRolePermission
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public PermissionTreeViewModel PermissionTree { get; set; }
    }
}
