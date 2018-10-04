using System;
using Agridea.Web.UI;
using System.Collections.Generic;

namespace Agridea.Security
{
    public class PermissionTreeViewModel
    {
        public TreeNode TreeNode { get; set; }

        public IEnumerable<string> Selected { get; set; }

        public int Level { get; set; }

        public string CssClass { get; set; }

    }
}
