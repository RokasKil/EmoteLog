using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System.Linq;
using EmoteLog.Data;
using System.Text;

namespace EmoteLog.Windows;

public class EmoteLogWindow : Window, IDisposable
{
    private Plugin Plugin { get; set; }

    public EmoteLogWindow(Plugin plugin) : base(
        "Emote Log", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(10, 10),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
    }

    public void Dispose()
    {

    }

    public override void Draw()
    {
        var height = ImGui.GetContentRegionAvail().Y;

        if (ImGui.BeginListBox("##emoteLog", new Vector2(-1, height)))
        {

            if (this.Plugin.Configuration.CollapseSpam)
            {
                foreach (CollapsedEmoteEntry collapsedEmoteEntry in this.Plugin.EmoteQueue.CollapsedLog)
                {
                    addEntry(collapsedEmoteEntry);
                }
            }
            else
            {
                foreach (EmoteEntry emoteEntry in this.Plugin.EmoteQueue.Log)
                {
                    addEntry(emoteEntry);
                }
            }

            ImGui.EndListBox();
        }

    }

    private void addEntry (CollapsedEmoteEntry collapsedEmoteEntry)
    {
        addEntry(collapsedEmoteEntry.Count, collapsedEmoteEntry.EmoteEntry);
    }

    private void addEntry(EmoteEntry emoteEntry)
    {
        addEntry(1, emoteEntry);
    }

    private void addEntry(int count, EmoteEntry emoteEntry)
    {
        ImGui.BeginGroup();
        StringBuilder sb = new StringBuilder();
        if (this.Plugin.Configuration.ShowTimestamps)
        {
            sb.Append($"[{emoteEntry.Timestamp.ToString("t")}] ");
        }
        sb.Append($"{emoteEntry.PlayerName} used {emoteEntry.EmoteName}");
        if (count > 1)
        {
            sb.Append($" [{count}]");
        }
        ImGui.Text(sb.ToString());

        ImGui.EndGroup();
    }
}
