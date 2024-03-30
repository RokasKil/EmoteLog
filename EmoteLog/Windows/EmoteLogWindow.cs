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
using EmoteLog.Utils;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Components;
using Dalamud.Interface;
using System.IO;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using Dalamud.Interface.ManagedFontAtlas;

namespace EmoteLog.Windows;

public class EmoteLogWindow : Window, IDisposable
{
    private Plugin Plugin { get; set; }

    private IFontHandle font;

    private IFontHandle iconFont;

    public EmoteLogWindow(Plugin plugin) : base(
        "Emote Log", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(10, 10),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.RespectCloseHotkey = false;

        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.Size = new Vector2(300, 150);



        Plugin = plugin;
        font = Plugin.PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(
            e => e.OnPreBuild(
                tk => tk.AddDalamudDefaultFont(Plugin.Configuration.UseCustomFontSize ? (Plugin.Configuration.FontSize * 4 / 3) : UiBuilder.DefaultFontSizePx)));
        iconFont = Plugin.PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(
            e => e.OnPreBuild(
                tk => tk.AddDalamudAssetFont(
                    Dalamud.DalamudAsset.FontAwesomeFreeSolid,
                    new() { 
                        SizePt = Plugin.Configuration.UseCustomFontSize ? Plugin.Configuration.IconFontSize : UiBuilder.DefaultFontSizePt, 
                        GlyphRanges = FontAtlasBuildToolkitUtilities.ToGlyphRange(FontAwesomeIcon.Trash.ToIconString())
                    })));
    }
    public void Dispose()
    {
        font.Dispose();
        iconFont.Dispose();
    }
    public override bool DrawConditions()
    {
        return (!ConditionUtils.AnyCondition(ConditionUtils.CombatFlags) || Plugin.Configuration.InCombat)
            && (!ConditionUtils.AnyCondition(ConditionUtils.InstanceFlags) || Plugin.Configuration.InInstance)
            && (!ConditionUtils.AnyCondition(ConditionUtils.CutsceneFlags) || Plugin.Configuration.InCutscenes)
            && (Plugin.EmoteQueue.Log.Count != 0 || !Plugin.Configuration.HideEmpty)
            && font.Available && iconFont.Available;
    }

    public override void PreDraw()
    {
        base.PreDraw();
        this.Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        if (!Plugin.Configuration.MoveWindow)
        {
            this.Flags |= ImGuiWindowFlags.NoMove;
        }
        if (!Plugin.Configuration.ResizeWindow)
        {
            this.Flags |= ImGuiWindowFlags.NoResize;
        }
        if (!Plugin.Configuration.ShowWindowFrames)
        {
            this.Flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground;
        }
    }

    public override void Draw()
    {
        var height = ImGui.GetContentRegionAvail().Y;

        using (font.Push())
        {
            if (ImGui.BeginListBox("##emoteLog", new Vector2(-1, height)))
            {

                if (Plugin.Configuration.CollapseSpam)
                {
                    foreach (CollapsedEmoteEntry collapsedEmoteEntry in Plugin.EmoteQueue.CollapsedLog)
                    {
                        AddEntry(collapsedEmoteEntry);
                    }
                }
                else
                {
                    foreach (EmoteEntry emoteEntry in Plugin.EmoteQueue.Log)
                    {
                        AddEntry(emoteEntry);
                    }
                }

                ImGui.EndListBox();
            }
        }
        if (Plugin.Configuration.ShowClearButton)
        {
            Vector2 buttonPos;
            using (iconFont.Push())
            {
                buttonPos = ImGui.GetWindowContentRegionMax() - (ImGui.CalcTextSize(FontAwesomeIcon.Trash.ToIconString() ?? "") + ImGui.GetStyle().FramePadding * 3.0f);
            }
            ImGui.SetCursorPos(buttonPos);
            if (ImGui.BeginChild("##clearButton"))
            {

                using (iconFont.Push())
                {
                    if (ImGui.Button(FontAwesomeIcon.Trash.ToIconString()))
                    {
                        Plugin.EmoteQueue.Clear();
                    }
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Clear Emote Log");
                }
            }

            ImGui.EndChild();
        }
    }

    private void AddEntry(CollapsedEmoteEntry collapsedEmoteEntry)
    {
        AddEntry(collapsedEmoteEntry.Count, collapsedEmoteEntry.EmoteEntry);
    }

    private void AddEntry(EmoteEntry emoteEntry)
    {
        AddEntry(1, emoteEntry);
    }

    private void AddEntry(int count, EmoteEntry emoteEntry)
    {
        StringBuilder sb = new();
        if (Plugin.Configuration.ShowTimestamps)
        {
            sb.Append($"[{emoteEntry.Timestamp.ToString("t")}] ");
        }
        sb.Append($"{emoteEntry.PlayerName} used {emoteEntry.EmoteName}");
        if (count > 1)
        {
            sb.Append($" [{count}]");
        }
        if (Plugin.Configuration.WrapText)
        {
            ImGui.TextWrapped(sb.ToString());
        }
        else
        {
            ImGui.Text(sb.ToString());
        }
    }
}
