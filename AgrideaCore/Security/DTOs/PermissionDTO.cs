
namespace Agridea.Security
{

    public class PermissionDTO
    {
        public string RoleName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsProperty { get; set; }
        public bool IsAllowed { get; set; }
        
        public PermissionDTO()
        {
        }

        public override string ToString()
        {
            return string.Format("[{0} RoleName='{1}', Name='{2}', Description='{3}' IsAllowed='{4}' IsProperty='{5}']",
                GetType().Name,
                RoleName,
                Name,
                Description,
                IsAllowed,
                IsProperty);
        }
    }
}
