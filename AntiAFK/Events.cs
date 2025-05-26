using CoreLib.AntiAFK.Handlers;
using Exiled.Events.Handlers;

namespace CoreLib.AntiAFK
{
    internal class Events
    {
        private static ServerHandlers? _server;

        internal static void Subscribtion()
        {
            _server = new();
            Server.RoundStarted += _server.OnRoundStart;
        }

        internal static void UnSubscribtion()
        {
            Server.RoundStarted -= _server!.OnRoundStart;
            _server = null;
        }
    }
}
