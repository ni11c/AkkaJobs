
namespace Agridea.Web.Mvc.Grid
{
    public interface IGridDataKey<T> : IGridDataKey
    {
        object GetValue(T dataItem);
    }

    public interface IGridDataKey
    {
        string Name { get; }
    }
}
