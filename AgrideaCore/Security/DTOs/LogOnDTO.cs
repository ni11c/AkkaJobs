
namespace Agridea.Security
{
    public class LogOnDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public override string ToString()
        {
            return string.Format("[{0} UserName='{1}' Password='{2}' RememberMe='{3}']",
                GetType().Name,
                UserName,
                "******",
                RememberMe);
        }
    }
}
