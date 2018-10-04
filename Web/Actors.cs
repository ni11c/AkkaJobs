using System;
using Akka.Actor;

namespace Agridea.Prototypes.Akka.Web
{
    public static class Actors
    {
        public static IActorRef SignalR = ActorRefs.Nobody;
        public static ActorSelection Supervisor = null;
        public static string Guid = System.Guid.NewGuid().ToString();
    }
}