
namespace Agridea.Security
{
    public class RoleNameDTO
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return string.Format("[{0} Name='{1}']",
                GetType().Name,
                Name);
        }
    }
}
