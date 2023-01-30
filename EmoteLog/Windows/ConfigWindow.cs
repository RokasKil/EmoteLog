using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace EmoteLog.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Emote log configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.Size = new Vector2(0, 0);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var logSize = this.Configuration.LogSize;
        var collapseSpam = this.Configuration.CollapseSpam;
        var showTimestamps = this.Configuration.ShowTimestamps;
        if (ImGui.InputInt("Log size", ref logSize))
        {
            if (logSize > 0 )
            {
                this.Configuration.LogSize = logSize;
                this.Configuration.Save();
            }
        }
        if (ImGui.Checkbox("Collapse spam into one line", ref collapseSpam))
        {
            this.Configuration.CollapseSpam = collapseSpam;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Show timestamps", ref showTimestamps))
        {
            this.Configuration.ShowTimestamps = showTimestamps;
            this.Configuration.Save();
        }
    }
}
