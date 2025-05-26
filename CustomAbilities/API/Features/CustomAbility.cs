using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp914;
using MEC;
using CoreLib.UI;

namespace CoreLib.CustomAbilities.API.Features
{
    public abstract class CustomAbility
    {
        public static List<CustomAbility> Abilities { get; } = new();

        public ushort Serial { get; private set; }
    
        protected abstract uint Id { get; }
        protected abstract string Name { get; }
        protected abstract string Description { get; }
        protected virtual string NameColorHex { get; } = "#FFFFFF";
        protected abstract ItemType ItemType { get; }
        protected virtual TimeSpan Cooldown { get; } = TimeSpan.Zero;
    
        protected virtual bool UsingVariant { get; } = false;

        protected virtual string CooldownFormat { get; } = "<color=#FF0000>Способность в перезарядке! Осталось: $COOLDOWN$ секунд</color>";

        protected virtual string UsedFormat { get; } = "Способность <color=#00FF00>успешно</color> использована, до следующего использования: $TOTALSECONDS$ секунд";

        private DateTime _lastUse = DateTime.MinValue;

        protected virtual void OnUsed(Player user)
        {
            _lastUse = DateTime.UtcNow;
            if (Cooldown > TimeSpan.Zero) user.ShowCoreHint(GetFullString(UsedFormat));
            Log.Debug($"CustomAbility {Name}[{Id}] used by {user}");
        }
    
        protected virtual void OnUsingVariantUsed(Player user)
        {
            _lastUse = DateTime.UtcNow;
            if (Cooldown > TimeSpan.Zero) user.ShowCoreHint(GetFullString(UsedFormat));
            Log.Debug($"CustomAbility {Name}[{Id}] variant 2 used by {user}");
        }

        private bool CheckCooldown() => DateTime.UtcNow - _lastUse >= Cooldown;

        private double GetRemainingCooldown() => Math.Max(0, (Cooldown - (DateTime.UtcNow - _lastUse)).TotalSeconds);

        public void Use(bool overrideCooldown = false, bool showFailedMessage = true, int variant = 1)
        {
            Player? player = Extensions.GetPlayerByItem(Serial);
            if (player == null)
                return;
        
            if (overrideCooldown)
            {
                switch (variant)
                {
                    case 1:
                        OnUsed(player);
                        return;
                    case 2:
                        if (UsingVariant)
                            OnUsingVariantUsed(player);
                        return;
                    default:
                        if (UsingVariant)
                            OnUsingVariantUsed(player);
                        return;
                }
            }
        
            if (!CheckCooldown())
            {
                if (showFailedMessage)
                    player.ShowCoreHint(GetFullString(CooldownFormat));
                return;
            }
        
            if (variant == 1)
            {
                OnUsed(player);
            }
            else if (UsingVariant)
            {
                OnUsingVariantUsed(player);
            }
        }
    
        public static void Give(Player target, CustomAbility ability)
        {
            Log.Debug("Ability give registered check");
            if (!IsRegistered(ability))
                Register(ability);
            Log.Debug("Check completed");
            Timing.CallDelayed(0.25f, () =>
            {
                try
                {
                    Item givedItem = target.AddItem(ability.ItemType);
                    ability.Serial = givedItem.Serial;
                    Log.Debug("Ability gived");
                }
                catch (Exception e)
                {
                    Log.Error($"Error in giving ability: {e}");
                }
            });
        }

        public static void Remove(CustomAbility ability)
        {
            if (IsRegistered(ability))
                UnRegister(ability);
            Item.Get(ability.Serial)?.Destroy();
            Pickup.Get(ability.Serial)?.Destroy();
            ability.Serial = default;
        }
    
        internal static void OnInternalChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Item == null && Abilities.Any(x => x.Serial == ev.Player.CurrentItem?.Serial))
                ev.Player.ClearHints();
        
            if (ev.Item == null)
                return;

            if (Abilities.Any(x => x.Serial == ev.Player.CurrentItem?.Serial))
            {
                ev.Player.ClearHints();
            }
        
            CustomAbility? ability = Abilities.FirstOrDefault(x => x.Serial == ev.Item.Serial);
            if (ability == null)
                return;
            ev.Player.ShowCoreHint($"<b>=[<color={ability.NameColorHex}>{ability.Name}</color>]=</b>\n{ability.Description}", 6f);
        }

        internal static void OnInternalDroppingItem(DroppingItemEventArgs ev)
        {
            if (Abilities.All(x => x.Serial != ev.Item.Serial)) return;
            {
                ev.IsAllowed = false;
                CustomAbility ability = Abilities.First(x => x.Serial == ev.Item.Serial);
                ability.Use();
            }
        }
    
        internal static void OnInternalThrowingRequest(ThrowingRequestEventArgs ev)
        {
            if (Abilities.All(x => x.Serial != ev.Throwable.Serial)) return;
            {
                CustomAbility ability = Abilities.First(x => x.Serial == ev.Throwable.Serial);
                ev.Throwable.Destroy();
                ev.Item.Destroy();
                Give(ev.Player, ability);
            }
        }

        internal static void OnInternalUsingItem(UsingItemEventArgs ev)
        {
            if (ev.Item == null) return;
            if (Abilities.Any(x => x.Serial == ev.Item.Serial))
            {
                CustomAbility ability = Abilities.First(x => x.Serial == ev.Item.Serial);
                ability.Use(variant: 2);
                ev.IsAllowed = false;
            }
        }

        internal static void OnInternalFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (ev.Item == null) return;
            if (Abilities.Any(x => x.Serial == ev.Item.Serial))
            {
                CustomAbility ability = Abilities.First(x => x.Serial == ev.Item.Serial);
                ability.Use(variant: 2);
                ev.IsAllowed = false;
            }
        }

        internal static void OnInternalUpgradingPickup(UpgradingPickupEventArgs ev)
        {
            if (TryGetCustomAbility(ev.Pickup.Serial, out _))
                ev.IsAllowed = false;
        }
   
        internal static void OnInternalUpgradingInventoryItem(UpgradingInventoryItemEventArgs ev)
        {
            if (TryGetCustomAbility(ev.Item.Serial, out _))
                ev.IsAllowed = false;
        }

        public string GetFullString(string formatedString)
        {
            if (formatedString == null)
                throw new ArgumentNullException(nameof(formatedString));
       
            formatedString = formatedString
                .Replace("$COOLDOWN$", Math.Truncate(GetRemainingCooldown()).ToString(CultureInfo.CurrentCulture))
                .Replace("$TOTALSECONDS$", Cooldown.TotalSeconds.ToString(CultureInfo.CurrentCulture))
                .Replace("$COLOR$", NameColorHex);
       
            string pattern = @"%(\w+)%";
            Regex regex = new Regex(pattern);
       
            string result = regex.Replace(formatedString, match =>
            {
                string fieldName = match.Groups[1].Value;
                PropertyInfo? propertyInfo = GetType().GetProperty(fieldName);

                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(this);
                    return value?.ToString() ?? string.Empty;
                }
                return match.Value;
            });
            return result;
        }
    
        public static void Register(CustomAbility ability)
        {
            if (IsRegistered(ability))
                return;
            Abilities.Add(ability);
        }

        public static bool IsRegistered(CustomAbility ability) => Abilities.Contains(ability);
        public static bool IsRegistered(ushort serial) => Abilities.Any(x => x.Serial == serial);
        public static void UnRegister(CustomAbility ability) => Abilities.Remove(ability);
        public static void UnRegister(ushort serial) => Abilities.Remove(Abilities.FirstOrDefault(x => x.Serial == serial));
        public static bool IsCustomAbility(Item item) => Abilities.Any(x => x.Serial == item.Serial);
        public static bool IsCustomAbility(Pickup pickup) => Abilities.Any(x => x.Serial == pickup.Serial);
        public static bool IsCustomAbility(ushort serial) => Abilities.Any(x => x.Serial == serial);
        public static CustomAbility? GetCustomAbility(ushort serial) => Abilities.FirstOrDefault(x => x.Serial == serial);

        public static bool TryGetCustomAbility(ushort serial, out CustomAbility? ability)
        {
            ability = GetCustomAbility(serial);
            return ability is not null;
        }
    }
} 
