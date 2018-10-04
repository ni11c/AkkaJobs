using System;

namespace Agridea.Security
{
    //[Serializable]
    public partial class User
    {
        public bool IsOnlineSince(int timeWindow)
        {
            DateTime lastActivityDate = LastActivityDate ?? DateTime.MinValue;
            return lastActivityDate.AddMinutes(timeWindow) >= DateTime.Now;
        }
    }
}