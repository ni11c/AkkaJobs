using System.Linq;
using Agridea.Web.UI;

namespace Agridea.Security
{
    public class RoleEditGrid
    {
        public IQueryable<RoleEditableItem> GridModel { get; set; }
    }
    public class RoleEditableItem : IHasId, ICheckChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanAccessAllFarms { get; set; }
        public bool IsWebApi { get; set; }

        
        #region Implementation of IHasId
        public int Id { get; set; }
        #endregion

        #region Implementation of ICheckChanged
        public bool HasChanged { get; set; }
        #endregion
    }

    public class RoleCreate : IHasId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanAccessAllFarms { get; set; }
        public bool IsWebApi { get; set; }
    }

}