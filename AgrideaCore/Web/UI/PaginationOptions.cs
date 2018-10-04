using System;

namespace Agridea.Web.UI
{
    public class PaginationOptions
    {
        public string Order { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public override string ToString()
        {
            return string.Format("[{0} Order='{1}' Page='{2}' PageSize='{3}']", 
                GetType().Name, Order,
                Page.ToDisplayString(),
                PageSize.ToDisplayString());
        }
    }
}