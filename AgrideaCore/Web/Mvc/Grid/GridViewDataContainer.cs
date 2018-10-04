using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid
{
    class GridViewDataContainer<T> : IViewDataContainer
    {
        public GridViewDataContainer(T model, ViewDataDictionary viewData)
        {
            ViewData = viewData;
            ViewData.Model = model;
        }

        public ViewDataDictionary ViewData { get; set; }
    }
    public class GridViewDataContainer : IViewDataContainer
    {
        public GridViewDataContainer(ViewDataDictionary viewData)
        {
            ViewData = viewData;
        }
        public ViewDataDictionary ViewData { get; set; }
    }
}
