using System;
using System.Collections.Generic;

namespace Agridea.Web.Mvc
{
    [Serializable]
    public class BugReportDTO
    {
        #region Members
        private Dictionary<string, object> values_;
        #endregion

        #region Initialization
        public BugReportDTO()
        {
            values_ = new Dictionary<string, object>();
        }
        #endregion

        #region Services
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string StackTrace { get; set; }
        public string Machine { get; set; }
        public string Browser { get; set; }

        public object this[string key]
        {
            get { return values_[key]; }
            set { values_[key] = value; }
        }
        #endregion
    }
}