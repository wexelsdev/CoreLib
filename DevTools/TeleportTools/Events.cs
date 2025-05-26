using CoreLib.DevTools.TeleportTools.Handlers;
using Exiled.Events.Handlers;

namespace CoreLib.DevTools.TeleportTools
{
    internal class Events
    {
        private static WarheadHandlers? _warhead;
        private static PlayerHandlers? _player;
        private static MapHandlers? _map;
    
        internal static void Subscription()
        {
            _warhead = new();
            _player = new();
            _map = new();
        
            Map.Generated += _map.OnGenerated;
            Map.Decontaminating += _map.OnDecontaminating;

            Player.ChangingRole += _player.OnChangingRole;
            Player.Left += _player.OnLeft;

            Warhead.Detonated += _warhead.OnDetonated;
        }
        internal static void UnSubscription()
        {
            Map.Generated -= _map!.OnGenerated;
            Map.Decontaminating -= _map.OnDecontaminating;

            Player.ChangingRole -= _player!.OnChangingRole;
            Player.Left -= _player.OnLeft;

            Warhead.Detonated -= _warhead!.OnDetonated;
        
            _warhead = null;
            _player = null;
            _map = null;
        }
    }
}