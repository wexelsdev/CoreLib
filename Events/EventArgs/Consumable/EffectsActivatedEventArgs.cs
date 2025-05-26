using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;

namespace CoreLib.Events.EventArgs.Consumable
{
    public class EffectsActivatedEventArgs : IExiledEvent
    {
        public EffectsActivatedEventArgs(Player player, InventorySystem.Items.Usables.Consumable consumable)
        {
            Player = player;
            Consumable = consumable;
        }

        public Player Player { get; }
        public InventorySystem.Items.Usables.Consumable Consumable { get; }
    }
}