using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agridea.Diagnostics.Logging
{
    public class LogHelper
    {
        public static string LineSeparator()
        {
            return new String('=', 100);
        }
        public static string Title(string title)
        {
            return new StringBuilder()
                .Append("--- ")
                .AppendLine(title)
                .ToString();
        }
    }
}
