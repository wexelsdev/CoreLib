using Exiled.Events.EventArgs.Interfaces;
using UnityEngine;

namespace CoreLib.DevTools.Extensions
{
    public static class Other
    {
        public static void Destroy(this Component component, float delay = 0f)
        {
            Object.Destroy(component, delay);
        }
    
        public static void Deny(this IDeniableEvent ev)
        {
            ev.IsAllowed = false;
        }
    }
}