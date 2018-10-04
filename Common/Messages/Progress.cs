using System;

namespace Agridea.Prototypes.Akka.Common
{
    public class Progress
    {
        public int Percent { get; set; }
        public string Name { get; }

        public Progress(string name, int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentOutOfRangeException("percent must be within 0 and 100%.");

            Percent = percent;
            Name = name;
        }
    }
}
