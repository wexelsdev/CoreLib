using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using HarmonyLib;

namespace CoreLib
{
    public class CorePlugin : Plugin<Config, Translations>
    {
        public override string Prefix => "core_lib";
        public override string Name => "CoreLib";
        public override string Author => "wexelsdev & Pawmi";

        public override Version Version => new(6, 0, 0);
        public override Version RequiredExiledVersion => new(9, 5, 0);
        
        public override PluginPriority Priority => PluginPriority.Highest;

        public static CorePlugin? Instance;
        
        public Harmony? HarmonyInstance;
        
        public override void OnEnabled()
        {
            Instance = this;
            HarmonyInstance = new Harmony("corelib.wexels.dev");
            
            DevTools.TeleportTools.Events.Subscription();
            CustomItems.Events.Subscription();
            CustomAbilities.Events.Subscription();
            Other.Informer.Events.Subscription();
            CustomSquads.Events.Subscription();
            
            if (Config.AntiAfkIsEnabled) AntiAFK.Events.Subscribtion();
            
            HarmonyInstance.PatchAll();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            DevTools.TeleportTools.Events.UnSubscription();
            CustomAbilities.Events.UnSubscription();
            Other.Informer.Events.Unsubscription();
            CustomSquads.Events.UnSubscription();
            CustomItems.Events.UnSubscription();
            
            if (Config.AntiAfkIsEnabled) AntiAFK.Events.UnSubscribtion();

            HarmonyInstance!.UnpatchAll();
            
            HarmonyInstance = null;
            Instance = null;

            base.OnDisabled();
        }
    }
} 
