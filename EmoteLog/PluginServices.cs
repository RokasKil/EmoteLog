using Dalamud.Data;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game;

namespace EmoteLog
{
    public class PluginServices
    {
        //[PluginService] public static ChatGui ChatGui { get; set; } = null!;
        [PluginService] public static ObjectTable ObjectTable { get; set; } = null!;
        [PluginService] public static ClientState ClientState { get; set; } = null!;
        [PluginService] public static SigScanner SigScanner { get; set; } = null!;
        //[PluginService] public static GameGui GameGui { get; set; } = null!;
        //[PluginService] public static DataManager DataManager { get; set; } = null!;
        public static void Initialize(DalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<PluginServices>();
        }
    }
}
