using CoreLib.DevTools.Extensions;
using Exiled.API.Enums;

namespace CoreLib.DevTools.TeleportTools.Handlers
{
    public class WarheadHandlers
    {
        public void OnDetonated() => TeleportExtensions.DenyAllRooms(RoomType.Surface);
    }
}