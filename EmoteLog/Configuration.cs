using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace EmoteLog
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public int LogSize { get; set; } = 100;
        public bool CollapseSpam { get; set; } = true;
        public bool ShowTimestamps { get; set; } = true;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
