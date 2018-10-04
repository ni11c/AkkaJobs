using System.Collections.Generic;

namespace Agridea.Web.Mvc.Grid
{
    public class AjaxConfirm
    {
        public AjaxConfirm()
        {
            Datas = new Dictionary<string, string>();
        }
        public string Url { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Datas { get; set; }
    }
}
