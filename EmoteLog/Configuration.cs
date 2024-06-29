using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace EmoteLog
{
    [Serializable]
    public class Configuration : IPluginConfiguration // A lot of these are inspired by Peeping Tom (it's not stealing it's market research)
    {
        public int Version { get; set; } = 1;

        public int LogSize { get; set; } = 100;
        public bool CollapseSpam { get; set; } = true;
        public bool ShowTimestamps { get; set; } = true;
        public bool OpenOnLogin { get; set; } = false;
        public bool MoveWindow { get; set; } = true;
        public bool ResizeWindow { get; set; } = true;
        public bool ShowWindowFrames { get; set; } = true;
        public bool InCombat { get; set; } = false;
        public bool InInstance { get; set; } = false;
        public bool InCutscenes { get; set; } = false;
        public bool HideEmpty { get; set; } = false;
        public bool ShowClearButton { get; set; } = true;
        public bool UseCustomFontSize { get; set; } = false;
        public float FontSize { get; set; } = 12.0f;
        public float IconFontSize { get; set; } = 12.0f;
        public bool ScaleClearButton { get; set; } = true;
        public bool WrapText { get; set; } = false;

        [NonSerialized]
        private IDalamudPluginInterface? pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
            if (Version == 0)
            {
                UseCustomFontSize = FontSize != 12.0f;
                IconFontSize = ScaleClearButton ? FontSize : 12.0f;
                Version = 1;
                Save();
            }
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
