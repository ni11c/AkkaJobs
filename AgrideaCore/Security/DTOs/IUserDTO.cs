namespace Agridea.Security
{
    public interface IUserDTO
    {
        string UserName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
    }
}
