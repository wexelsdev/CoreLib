using CoreLib.CustomAbilities.API.Features;

namespace CoreLib.CustomAbilities
{
    internal static class Events
    {
        internal static void Subscription()
        {
            Exiled.Events.Handlers.Player.DroppingItem += CustomAbility.OnInternalDroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem += CustomAbility.OnInternalChangingItem;
            Exiled.Events.Handlers.Player.ThrowingRequest += CustomAbility.OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.UsingItem += CustomAbility.OnInternalUsingItem;
            Exiled.Events.Handlers.Player.FlippingCoin += CustomAbility.OnInternalFlippingCoin;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += CustomAbility.OnInternalUpgradingPickup;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += CustomAbility.OnInternalUpgradingInventoryItem;
        }
    
        internal static void UnSubscription()
        {
            Exiled.Events.Handlers.Player.DroppingItem -= CustomAbility.OnInternalDroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem -= CustomAbility.OnInternalChangingItem;
            Exiled.Events.Handlers.Player.ThrowingRequest -= CustomAbility.OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.UsingItem -= CustomAbility.OnInternalUsingItem;
            Exiled.Events.Handlers.Player.FlippingCoin -= CustomAbility.OnInternalFlippingCoin;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= CustomAbility.OnInternalUpgradingPickup;
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= CustomAbility.OnInternalUpgradingInventoryItem;
        }
    }
} 