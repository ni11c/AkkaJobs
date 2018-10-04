using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agridea.Threading
{
    public class BatchJobViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public string Input { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public string ProgressRate { get; set; }
        public string TimeElapsed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}