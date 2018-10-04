
namespace Agridea.Threading
{
    public interface ICancelable
    {
        bool CancelRequested { set; get; }
    }
}
