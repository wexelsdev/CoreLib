using CoreLib.Other.Informer.Handlers;
using Player = Exiled.Events.Handlers.Player;

namespace CoreLib.Other.Informer
{
    internal static class Events
    {
        internal static void Subscription()
        {
            Player.Verified += PlayerHandlers.OnVerified;
        }

        internal static void Unsubscription()
        {
            Player.Verified -= PlayerHandlers.OnVerified;
        }
    }
}