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

namespace EmoteLog.Windows;

public class EmoteLogWindow : Window, IDisposable
{
    private Plugin Plugin { get; set; }

    private ImFontPtr fontPtr;

    private ImFontPtr iconFontPtr;

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

        Plugin.PluginInterface.UiBuilder.BuildFonts += this.HandleBuildFonts;
        Plugin.PluginInterface.UiBuilder.RebuildFonts();
    }
    
    private unsafe void HandleBuildFonts()
    {
        ImFontConfigPtr iconFontConfig = ImGuiNative.ImFontConfig_ImFontConfig();

        iconFontConfig.OversampleH = 1;
        iconFontConfig.OversampleV = 1;
        iconFontConfig.PixelSnapH = true;

        var fontPath = Path.Combine(Plugin.PluginInterface.DalamudAssetDirectory.FullName, "UIRes", "NotoSansCJKjp-Medium.otf");
        this.fontPtr = ImGui.GetIO().Fonts.AddFontFromFileTTF(fontPath, Plugin.Configuration.FontSize * 4.0f / 3.0f + 1, iconFontConfig);

        var iconRangeHandle = GCHandle.Alloc(new ushort[] { (ushort)FontAwesomeIcon.Trash, (ushort)FontAwesomeIcon.Trash + 1, 0, }, GCHandleType.Pinned);
        iconFontConfig.GlyphRanges = iconRangeHandle.AddrOfPinnedObject();
        
        var iconFontPath = Path.Combine(Plugin.PluginInterface.DalamudAssetDirectory.FullName, "UIRes", "FontAwesomeFreeSolid.otf");
        this.iconFontPtr = ImGui.GetIO().Fonts.AddFontFromFileTTF(iconFontPath, Plugin.Configuration.FontSize * 4.0f / 3.0f, iconFontConfig);

        ImGui.GetIO().Fonts.Build();
        iconFontConfig.Destroy();
        iconRangeHandle.Free();
    }

    public void Dispose()
    {
        Plugin.PluginInterface.UiBuilder.BuildFonts -= this.HandleBuildFonts;
    }
    public override bool DrawConditions()
    {
        return (!ConditionUtils.AnyCondition(ConditionUtils.CombatFlags) || Plugin.Configuration.InCombat)
            && (!ConditionUtils.AnyCondition(ConditionUtils.InstanceFlags) || Plugin.Configuration.InInstance)
            && (!ConditionUtils.AnyCondition(ConditionUtils.CutsceneFlags) || Plugin.Configuration.InCutscenes)
            && (Plugin.EmoteQueue.Log.Count != 0 || !Plugin.Configuration.HideEmpty);
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

        ImGui.PushFont(this.fontPtr);
        if (ImGui.BeginListBox("##emoteLog", new Vector2(-1, height)))
        {

            if (Plugin.Configuration.CollapseSpam)
            {
                foreach (CollapsedEmoteEntry collapsedEmoteEntry in Plugin.EmoteQueue.CollapsedLog)
                {
                    addEntry(collapsedEmoteEntry);
                }
            }
            else
            {
                foreach (EmoteEntry emoteEntry in Plugin.EmoteQueue.Log)
                {
                    addEntry(emoteEntry);
                }
            }

            ImGui.EndListBox();
        }
        ImGui.PopFont();

        if (Plugin.Configuration.ShowClearButton)
        {
            var currentIconFontPtr = Plugin.Configuration.ScaleClearButton ? this.iconFontPtr : UiBuilder.IconFont;
            ImGui.PushFont(currentIconFontPtr);
            var buttonPos = ImGui.GetWindowContentRegionMax() - (ImGui.CalcTextSize(FontAwesomeIcon.Trash.ToIconString() ?? "") + ImGui.GetStyle().FramePadding * 3.0f);
            ImGui.PopFont();
            ImGui.SetCursorPos(buttonPos);
            if (ImGui.BeginChild("##clearButton"))
            {

                ImGui.PushFont(currentIconFontPtr);
                if (ImGui.Button(FontAwesomeIcon.Trash.ToIconString()))
                {
                    Plugin.EmoteQueue.Clear();
                }
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Clear Emote Log");
                }
            }

            ImGui.EndChild();
        }
    }

    private void addEntry(CollapsedEmoteEntry collapsedEmoteEntry)
    {
        addEntry(collapsedEmoteEntry.Count, collapsedEmoteEntry.EmoteEntry);
    }

    private void addEntry(EmoteEntry emoteEntry)
    {
        addEntry(1, emoteEntry);
    }

    private void addEntry(int count, EmoteEntry emoteEntry)
    {
        StringBuilder sb = new StringBuilder();
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
