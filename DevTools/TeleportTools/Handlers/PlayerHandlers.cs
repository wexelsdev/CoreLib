using CoreLib.DevTools.Extensions;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace CoreLib.DevTools.TeleportTools.Handlers
{
    public class PlayerHandlers
    {
        public void OnLeft(LeftEventArgs ev) => ev.Player.DenyTeleport();

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole is not RoleTypeId.None and not RoleTypeId.Spectator and not RoleTypeId.Overwatch and not RoleTypeId.CustomRole)
            {
                ev.Player.AllowTeleport();
            }
            else
            {
                ev.Player.DenyTeleport();
            }
        }
    }
}