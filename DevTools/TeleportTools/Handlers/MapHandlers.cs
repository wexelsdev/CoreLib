using System.Linq;
using CoreLib.DevTools.Extensions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;

namespace CoreLib.DevTools.TeleportTools.Handlers
{
    public class MapHandlers
    {
        public void OnGenerated() => TeleportExtensions.AllowAllRooms();

        public void OnDecontaminating(DecontaminatingEventArgs ev)
        {
            if (!ev.IsAllowed) return;

            foreach (Room room in TeleportExtensions.Rooms.ToHashSet())
            {
                if (room is null || room.Zone != ZoneType.LightContainment) continue;

                TeleportExtensions.DenyTeleport(room);
            }
        }
    }
}