
namespace Agridea.Security
{
    public class UserNameDTO
    {
        public string UserName { get; set; }
        public override string ToString()
        {
            return string.Format("[{0} UserName='{1}']",
                GetType().Name,
                UserName);
        }
    }
}
