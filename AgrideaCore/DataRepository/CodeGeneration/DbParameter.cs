using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Agridea.DataRepository
{
    public class DbParameter
    {
        public string Name { get; set; }
        public DataType DataType { get; set; }
    }

    public enum TriggerType
    {
        Delete,
        Insert,
        Update
    }

    public class TriggerParameters
    {
        public TriggerParameters(TriggerType triggerType, ActivationOrder activationOrder)
        {
            TriggerType = triggerType;
            ActivationOrder = activationOrder;
        }
        public TriggerType TriggerType { get; set; }
        public ActivationOrder ActivationOrder { get; set; }
    }
}
