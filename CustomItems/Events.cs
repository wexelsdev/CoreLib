using CoreLib.CustomItems.API.Features;

namespace CoreLib.CustomItems
{
    internal static class Events
    {
        internal static void Subscription()
        {
            Exiled.Events.Handlers.Map.Generated += OnGenerated;
        }
    
        internal static void UnSubscription()
        {
            Exiled.Events.Handlers.Map.Generated -= OnGenerated;
        }

        private static void OnGenerated()
        {
            foreach (CustomItem customItem in CustomItem.Registered)
            {
                customItem.Spawn();
            }
        }
    }
}