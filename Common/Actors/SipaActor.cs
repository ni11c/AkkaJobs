using System;
using Akka.Actor;

namespace Agridea.Prototypes.Akka.Common
{
    public class SipaActor : ReceiveActor
    {
        private int exportCount_;
        private int numWaiting_;
        
        public SipaActor()
        {
            Receive<string>(s =>
            {
                if (s != "export")
                    return;

                var sender = Sender; // close over Sender for use in subsequent event handlers
                numWaiting_++;
                string name = "exporter" + ++exportCount_;
                var sipaExporter = new DoNothingJob(name);

                sipaExporter.Finished += () =>
                {
                    sender.Tell(new Progress(name, 100));
                    sender.Tell(new Finished(name));
                    Console.WriteLine(name + " finished.");
                    numWaiting_--;
                    Console.WriteLine("num jobs in pipeline: " + numWaiting_);
                };

                sipaExporter.Progressed += (percent) =>
                {
                    sender.Tell(new Progress(name, percent));
                };
                
                sipaExporter.Started += () =>
                {
                    sender.Tell(new Started(name));
                };

                Console.WriteLine(name + " starting");
                Console.WriteLine("num jobs in pipeline: " + numWaiting_);
                sipaExporter.Run();
            });
        }
    }
}
