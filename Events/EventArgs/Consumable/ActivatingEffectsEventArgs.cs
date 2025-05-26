using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;

namespace CoreLib.Events.EventArgs.Consumable
{
    public class ActivatingEffectsEventArgs : IDeniableEvent
    {
        public ActivatingEffectsEventArgs(Player player, InventorySystem.Items.Usables.Consumable consumable)
        {
            Player = player;
            Consumable = consumable;
        }

        public Player Player { get; }
        public InventorySystem.Items.Usables.Consumable Consumable { get; }
        public bool IsAllowed { get; set; } = true;
    }
}