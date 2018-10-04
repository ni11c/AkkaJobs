using System.Collections.Generic;
using System.Linq;
using System;

namespace Agridea.Web.Helpers
{
    public class SubmitButton
    {
        #region Services
        public string recalculate { get; set; }
        public string create { get; set; }
        public string save { get; set; }
        public string saveandcreatenew { get; set; }
        public string commit { get; set; }
        public string send { get; set; }
        public string wholecanton { get; set; }
        public string bymunicipality { get; set; }
        public IList<DeleteButton> delete { get; set; }

        public bool IsWholeCanton() { return wholecanton != null; }
        public bool IsByMunicipality()
        {
            return bymunicipality != null;
        }
        public bool IsRecalculate()
        {
            return recalculate != null;
        }
        public bool IsCreate()
        {
            return create != null;
        }
        public bool IsSave()
        {
            return save != null;
        }
        public bool IsSaveAndCreateNew()
        {
            return saveandcreatenew != null;
        }
        public bool IsCommit()
        {
            return commit != null;
        }
        public bool IsSend()
        {
            return send != null;
        }
        public bool IsDelete()
        {
            return delete != null && delete.Count > 0 && delete.Any(x => !string.IsNullOrEmpty(x.Value));
        }
        public int GetIdToDelete()
        {
            if (!IsDelete()) return -1;
            return delete.First(x => !string.IsNullOrEmpty(x.Value)).Id;
        }
        #endregion
    }

    public class DeleteButton
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
