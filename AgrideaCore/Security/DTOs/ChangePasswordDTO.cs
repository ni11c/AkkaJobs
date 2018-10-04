
namespace Agridea.Security
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public override string ToString()
        {
            return string.Format("[{0} OldPassword='{1}', NewPassword='{2}' ConfirmPassword='{3}']",
                GetType().Name,
                "******",
                "******",
                "******");
        }
    }
}
