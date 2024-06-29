using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace EmoteLog.Utils
{
    public class PluginServices
    {
        [PluginService] public static IChatGui ChatGui { get; set; } = null!;
        [PluginService] public static IObjectTable ObjectTable { get; set; } = null!;
        [PluginService] public static IClientState ClientState { get; set; } = null!;
        [PluginService] public static ISigScanner SigScanner { get; set; } = null!;
        [PluginService] public static IDataManager DataManager { get; set; } = null!;
        [PluginService] public static ICondition Condition { get; set; } = null!;
        [PluginService] public static IGameInteropProvider GameInteropProvider { get; set; } = null!;
        [PluginService] public static ICommandManager CommandManager { get; set; } = null!;
        [PluginService] public static IPluginLog PluginLog { get; set; } = null!;
        public static void Initialize(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.Create<PluginServices>();
        }
    }
}
