using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using EmoteLog.Windows;
using EmoteLog.Hooks;
using EmoteLog.Utils;
using EmoteLog.Data;
using System;
using Dalamud.Utility;

namespace EmoteLog
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Emote Log Plugin";
        private const string CommandName = "/el";

        public DalamudPluginInterface PluginInterface { get; init; }
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
            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            PluginServices.Initialize(PluginInterface);

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
            EmoteReaderHooks = new EmoteReaderHooks();

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new EmoteLogWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);

            WindowSystem.AddWindow(MainWindow);
            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = $"Opens the Emote Log window, \"{CommandName} config\" to open settings and \"{CommandName} clear\" to clear the log"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
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
            CommandManager.RemoveHandler(CommandName);
            EmoteReaderHooks.Dispose();
        }
        private void OnLogin(object? sender, EventArgs e)
        {
            OnLogin();
        }
        private void OnLogin()
        {
            if(Configuration.OpenOnLogin)
            {
                MainWindow.IsOpen = true;
            }
        }
        private void OnCommand(string command, string args)
        {
            if (args.IsNullOrEmpty())
            {
                if(!MainWindow.DrawConditions())
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

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.Toggle();
        }
    }
}
