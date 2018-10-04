using System;
using Agridea.Prototypes.Akka.Common;
using Akka.Actor;
using Web.SignalR;

namespace Agridea.Prototypes.Akka.Web
{
    public class AkkaConfig
    {
        public static void ConfigureActors(ActorSystem system)
        {
            system = ActorSystem.Create(ActorPaths.WebClientSystem.Name);
            Actors.Supervisor = system.ActorSelection(ActorPaths.Supervisor.Path);
            Actors.SignalR = system.ActorOf(Props.Create(() => new SignalRActor()), ActorPaths.SignalR.Name);
        }
    }
}