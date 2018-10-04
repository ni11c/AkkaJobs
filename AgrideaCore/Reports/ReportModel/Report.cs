using System.Collections.Generic;

namespace Agridea.Reports
{
    public class Report
    {
        #region Members
        private List<Table> tables_;
        #endregion

        #region Initialization
        public Report()
        {
            tables_ = new List<Table>();
        }
        #endregion

        #region Services
        public Report Add(Table table)
        {
            tables_.Add(table);
            return this;
        }
        public Table[] Tables { get { return tables_.ToArray(); } }
        #endregion
    }
}
