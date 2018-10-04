namespace Agridea.Web.Mvc.Menu
{
    using Session;

    public interface IMenuBuilder
    {
        IMenuItem Build();
        IMenuBuilder Add(IMenuItem parent, IMenuItem child, bool condition = true);
    }
}