using System.ComponentModel;
using CoreLib.CustomRoundEnding.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace CoreLib
{
    public class Config : IConfig
    {
        #region CoreLib
    
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool DisableHelp { get; set; } = true;
        
        #endregion

        #region AntiAFK
    
        public bool AntiAfkIsEnabled { get; set; } = false;
        public uint MaxAfkTime { get; set; } = 180;
        public uint MessageAfkTime { get; set; } = 120;
    
        #endregion
    
        #region CustomRoundEnding

        public bool CustomRoundEnding { get; set; } = true;
        
        public RestartType RestartType { get; set; } = RestartType.Silent;
        
        #endregion

        #region CustomItems

        [Description("The hint that is shown when someone pickups a custom item.")]
        public Hint PickedUpHint { get; private set; } = new("{0}", 2);
        
        [Description("The hint that is shown when someone drops a custom item.")]
        public Hint DroppedHint { get; private set; } = new("{0}", 2);
        
        [Description("The hint that is shown when someone selects a custom item.")]
        public Hint SelectedHint { get; private set; } = new("{0}\n{1}", 5);

        #endregion
        
        #region CustomRoles

        public Hint GotRoleHint { get; set; } = new()
        {
            Content = "You have spawned as a {0}\n      {1}",
            Duration = 6,
            Show = true
        };

        #endregion
    }
}