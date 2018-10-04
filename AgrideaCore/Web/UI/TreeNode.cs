using System;
using System.Collections.Generic;
using System.Linq;

namespace Agridea.Web.UI
{
    public class TreeNode
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool NeedsOpeningDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public TreeNode Parent { get; set; }
        public IList<TreeNode> Children { get; set; }

        public int RecensementInfoCategoryId { get; set; }
        public bool IsOpen
        {
            get { return StartDate != null && EndDate != null && StartDate.Value.Date <= DateTime.Now.Date && EndDate.Value.Date >= DateTime.Now.Date; }
        }

        public string OpeningDateText
        {
            get
            {
                return StartDate != null && EndDate != null
                    ? StartDate.Value.ToString("dd.MM.yyyy") + "-" + EndDate.Value.ToString("dd.MM.yyyy")
                    : "Pas de date d'ouverture";
            }
        }

        public bool HasChildren()
        {
            return Children.Any();
        }

        public TreeNode()
        {
            Children = new List<TreeNode>();
        }
    }
}
