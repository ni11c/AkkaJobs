using System;
using Agridea.Prototypes.Akka.Common;
using Akka.Actor;

namespace Agridea.Prototypes.Akka.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int taskInterval = 5000;
            Console.WriteLine("System starting, messages are sent every {0:##.#} seconds.", taskInterval / 1000.0);
            var timer = new System.Timers.Timer(taskInterval) { AutoReset = true };

            // local system lets us create one local supervisor and one remote worker
            using (var system = ActorSystem.Create("SipaClient"))
            {
                // create remote worker on the remote system, referenced by its adress
                var remoteAddress = Address.Parse("akka.tcp://SipaServer@localhost:10100");
                var worker = system.ActorOf(
                    Props.Create(() => new SipaActor())
                    .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "sipa");

                // then create local supervisor
                var jobsDisplayer = new ConsoleJobsDisplayer();
                var supervisor = system.ActorOf(Props.Create(() => new SupervisorActor(worker, jobsDisplayer)));

                // send commands to supervisor
                timer.Elapsed += (sender, eventargs) =>
                {
                    supervisor.Tell("start");
                };
                timer.Start();
                Console.ReadLine();
            } 
        }
    }
}
