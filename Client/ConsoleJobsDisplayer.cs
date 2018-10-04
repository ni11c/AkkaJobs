using System;
using System.Collections.Generic;
using System.Text;
using Agridea.Prototypes.Akka.Common;

namespace Agridea.Prototypes.Akka.Client
{
    public class ConsoleJobsDisplayer : IJobsDisplayer<List<Progress>>
    {
        private static string _lastWritten;

        public void Display(List<Progress> jobs)
        {
            var sb = new StringBuilder();
            foreach (Progress t in jobs)
                sb.Append(BuildLine(t));
            var toWrite = sb.ToString();
            if (_lastWritten != toWrite)
            {
                _lastWritten = toWrite;
                Console.Write(toWrite);
                Console.SetCursorPosition(0, Console.CursorTop - jobs.Count);
            }
        }

        private string BuildLine(Progress p, int nameLength = 20, int progressBarLength = 40)
        {
            int numLetterOs = p.Percent*(progressBarLength - 2)/100;
            var name = p.Name.Length <= nameLength ? p.Name : p.Name.Substring(0, nameLength);
            name = name.PadRight(20);
            var line = name + "[" + new string('o', numLetterOs) + new string(' ', progressBarLength - 2 - numLetterOs) + "]" + (p.Percent >= 100 ? " Finished" : "");
            return line + "\n";
        }
    }
}
