using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using EmoteLog.Data;
using EmoteLog.Hooks;
using EmoteLog.Utils;
using EmoteLog.Windows;

namespace EmoteLog
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string CommandName = "/elog";

        public IDalamudPluginInterface PluginInterface { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("EmoteLog");

        private ConfigWindow ConfigWindow { get; init; }
        private EmoteLogWindow MainWindow { get; init; }
        public EmoteReaderHooks EmoteReaderHooks { get; init; }
        public EmoteQueue EmoteQueue { get; init; }
        public Plugin(IDalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;

            PluginServices.Initialize(PluginInterface);

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
            EmoteReaderHooks = new EmoteReaderHooks();

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new EmoteLogWindow(this);

            WindowSystem.AddWindow(ConfigWindow);

            WindowSystem.AddWindow(MainWindow);
            PluginServices.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = $"Opens the Emote Log window, \"{CommandName} config\" to open settings and \"{CommandName} clear\" to clear the log"
            });

            PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
            PluginInterface.UiBuilder.OpenConfigUi += ConfigWindow.Toggle;
            PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;

            EmoteQueue = new EmoteQueue(this);
            if (PluginServices.ClientState.IsLoggedIn)
            {
                OnLogin();
            }
            PluginServices.ClientState.Login += OnLogin;
        }

        public void Dispose()
        {
            EmoteQueue.Dispose();
            WindowSystem.RemoveAllWindows();
            ConfigWindow.Dispose();
            MainWindow.Dispose();
            PluginServices.CommandManager.RemoveHandler(CommandName);
            EmoteReaderHooks.Dispose();
        }

        private void OnLogin()
        {
            if (Configuration.OpenOnLogin)
            {
                MainWindow.IsOpen = true;
            }
        }

        private void OnCommand(string command, string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                ToggleMainWindow();
            }
            else if (args == "settings" || args == "config")
            {
                ConfigWindow.Toggle();
            }
            else if (args == "clear")
            {
                EmoteQueue.Clear();
            }
            else
            {
                PluginServices.ChatGui.PrintError($"[Emote Log] Unknown argument: \"{args}\"");
            }
        }
        private void ToggleMainWindow()
        {
            if (!MainWindow.DrawConditions())
            {
                if (!MainWindow.IsOpen)
                {
                    PluginServices.ChatGui.Print("[Emote Log] Emote Log window opened, but your configuration is currently hiding it.");
                }
                else
                {
                    PluginServices.ChatGui.Print("[Emote Log] Emote Log window closed, your configuration was hiding it.");
                }
            }
            MainWindow.Toggle();
        }
    }
}
