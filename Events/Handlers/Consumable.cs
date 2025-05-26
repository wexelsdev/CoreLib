using CoreLib.Events.EventArgs.Consumable;
using Exiled.Events.Features;

namespace CoreLib.Events.Handlers
{
    public static class Consumable
    {
        public static Event<ActivatingEffectsEventArgs> ActivatingEffects { get; set; } = new();

        public static Event<EffectsActivatedEventArgs> EffectsActivated { get; set; } = new();
    
        public static void OnActivatingEffects(ActivatingEffectsEventArgs ev)
        {
            ActivatingEffects.InvokeSafely(ev);
        }

        public static void OnEffectsActivated(EffectsActivatedEventArgs ev)
        {
            EffectsActivated.InvokeSafely(ev);
        }
    }
}