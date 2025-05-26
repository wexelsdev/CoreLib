using System;
using System.ComponentModel;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;

namespace CoreLib.CustomItems.API.Features
{
    public abstract class CustomArmor : CustomItem
    {
        private ItemType _type;
        
        public override ItemType Type
        {
            get => _type;
            set
            {
                if (value != ItemType.None && !value.IsArmor())
                    throw new ArgumentOutOfRangeException(nameof(Type), value, "Invalid armor type.");

                _type = value;
            }
        }

        [Description("The value must be above 1 and below 2")]
        public virtual float StaminaUseMultiplier { get; set; } = 1.15f;

        [Description("The value must be above 0 and below 100")] 
        public virtual int HelmetEfficacy { get; set; } = 80;

        [Description("The value must be above 0 and below 100")]
        public virtual int VestEfficacy { get; set; } = 80;

        public override void Give(Player player, Item item, bool displayMessage = true)
        {
            base.Give(player, item, displayMessage);
            
            Armor armor = (Armor)item;

            armor.Weight = Weight;
            armor.StaminaUseMultiplier = StaminaUseMultiplier;

            armor.VestEfficacy = VestEfficacy;
            armor.HelmetEfficacy = HelmetEfficacy;
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem += OnInternalPickingUpItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= OnInternalPickingUpItem;
            base.UnsubscribeEvents();
        }

        private void OnInternalPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!Check(ev.Pickup) && ev.Player.Items.Count >= 8)
                return;

            OnPickingUp(ev);
        }
    }
}