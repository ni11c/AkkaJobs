using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Prototypes.Akka.Common
{
    public class ActorPaths
    {
        public static SystemMetadata RemoteV2System = new SystemMetadata("jobshost", "localhost", 10200);
        public static ActorMetadata Supervisor = new ActorMetadata("supervisor", RemoteV2System);
        public static SystemMetadata WebClientSystem = new SystemMetadata("webclient", "localhost", 0);
        public static ActorMetadata SignalR = new ActorMetadata("signalr", WebClientSystem);
    }

    public class ActorMetadata
    {
        public string Name { get; }
        public SystemMetadata System { get; }

        public string Path => System.Path + $"/user/{Name}";

        public ActorMetadata(string name, SystemMetadata system)
        {
            Name = name;
            System = system;
        }
    }

    public class SystemMetadata
    {
        public string Name { get; }

        public string Url { get; }

        public int Port { get; }

        public string Path => $"akka.tcp://{Name}@{Url}:{Port}/";

        public SystemMetadata(string name, string url, int port)
        {
            Name = name;
            Url = url;
            Port = port;
        }
    }
}
