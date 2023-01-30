using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using EmoteLog.Windows;
using EmoteLog.Hooks;
using EmoteLog.Utils;
using EmoteLog.Data;

namespace EmoteLog
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Emote Log Plugin";
        private const string CommandName = "/el";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("EmoteLog");

        private ConfigWindow ConfigWindow { get; init; }
        private EmoteLogWindow MainWindow { get; init; }
        public EmoteReaderHooks EmoteReaderHooks { get; init; }
        public EmoteQueue EmoteQueue { get; init; }
        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            PluginServices.Initialize(PluginInterface);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            this.EmoteReaderHooks = new EmoteReaderHooks();

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new EmoteLogWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            WindowSystem.AddWindow(MainWindow);
            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the Emote Log window"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.EmoteQueue = new EmoteQueue(this);
        }

        public void Dispose()
        {
            this.EmoteQueue.Dispose();
            this.WindowSystem.RemoveAllWindows();
            ConfigWindow.Dispose();
            MainWindow.Dispose();
            this.CommandManager.RemoveHandler(CommandName);
            EmoteReaderHooks.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = !MainWindow.IsOpen;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
