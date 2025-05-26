using Exiled.Events.Features;

namespace CoreLib.DevTools.PrimitiveHealth
{
    public static class Handlers
    {
        public static Event<HealthObjectDyingEventArgs> HealthObjectDestroyed { get; set; } = new Event<HealthObjectDyingEventArgs>();
    
        public static Event<HealthObjectHurtingEventArgs> HealthObjectHurting { get; set; } = new Event<HealthObjectHurtingEventArgs>();
    
        public static void OnHealthObjectDestroyed(HealthObjectDyingEventArgs ev)
        {
            HealthObjectDestroyed.InvokeSafely(ev);
        }
    
        public static void OnHealthObjectHurting(HealthObjectHurtingEventArgs ev)
        {
            HealthObjectHurting.InvokeSafely(ev);
        }
    }
}