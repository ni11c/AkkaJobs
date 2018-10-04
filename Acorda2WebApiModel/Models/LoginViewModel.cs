
namespace Agridea.Acorda.Acorda2.WebApi.Model
{
    public class LogInViewModel
    {
        public string Canton { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return string.Format("[{0} Canton='{1}' UserName='{2}' Password='{3}']", GetType().Name, Canton, UserName, "****");
        }
    }
}