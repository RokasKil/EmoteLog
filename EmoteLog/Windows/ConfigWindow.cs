using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;

namespace EmoteLog.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration  { get; set; }

    private Plugin Plugin { get; set; }

    public ConfigWindow(Plugin plugin) : base(
        "Emote log configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        Size = new Vector2(0, 0);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        using (ImRaii.Group())
        {
            DrawConfiguration();
        }

        KofiButton(ImGui.GetItemRectSize().X + ImGui.GetStyle().WindowPadding.X);
    }
    
    private void DrawConfiguration()
    {
        var logSize = Configuration.LogSize;
        var collapseSpam = Configuration.CollapseSpam;
        var showTimestamps = Configuration.ShowTimestamps;
        var openOnLogin = Configuration.OpenOnLogin;
        var moveWindow = Configuration.MoveWindow;
        var resizeWindow = Configuration.ResizeWindow;
        var showWindowFrames = Configuration.ShowWindowFrames;
        var inCombat = Configuration.InCombat;
        var inInstance = Configuration.InInstance;
        var inCutscenes = Configuration.InCutscenes;
        var hideEmpty = Configuration.HideEmpty;
        var showClearButton = Configuration.ShowClearButton;
        var useCustomFontSize = Configuration.UseCustomFontSize;
        var fontSize = Configuration.FontSize;
        var iconFontSize = Configuration.IconFontSize;
        var wrapText = Configuration.WrapText;

        ImGui.Text("Log Settings");
        ImGui.SetNextItemWidth(120f);
        if (ImGui.InputInt("Log size", ref logSize))
        {
            if (logSize > 0)
            {
                Configuration.LogSize = logSize;
                Configuration.Save();
            }
        }
        if (ImGui.Checkbox("Collapse duplicates into one line", ref collapseSpam))
        {
            Configuration.CollapseSpam = collapseSpam;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show timestamps", ref showTimestamps))
        {
            Configuration.ShowTimestamps = showTimestamps;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show clear button in the Emote Log", ref showClearButton))
        {
            Configuration.ShowClearButton = showClearButton;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Wrap Emote Log text", ref wrapText))
        {
            Configuration.WrapText = wrapText;
            Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Puts log entries into multiple lines if they don't fit in one");
        }

        ImGui.Separator();
        ImGui.Text("Window Settings");
        if (ImGui.Checkbox("Open on login", ref openOnLogin))
        {
            Configuration.OpenOnLogin = openOnLogin;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Allow moving the Emote Log", ref moveWindow))
        {
            Configuration.MoveWindow = moveWindow;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Allow resizing the Emote Log", ref resizeWindow))
        {
            Configuration.ResizeWindow = resizeWindow;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show Emote Log's frames", ref showWindowFrames))
        {
            Configuration.ShowWindowFrames = showWindowFrames;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show Emote Log in combat", ref inCombat))
        {
            Configuration.InCombat = inCombat;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show Emote Log in instance", ref inInstance))
        {
            Configuration.InInstance = inInstance;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Show Emote Log in cutscene", ref inCutscenes))
        {
            Configuration.InCutscenes = inCutscenes;
            Configuration.Save();
        }
        if (ImGui.Checkbox("Hide Emote Log when empty", ref hideEmpty))
        {
            Configuration.HideEmpty = hideEmpty;
            Configuration.Save();
        }
        ImGui.Separator();
        ImGui.Text("Font Settings");

        if (ImGui.Checkbox("Use custom font size", ref useCustomFontSize))
        {
            Configuration.UseCustomFontSize = useCustomFontSize;
            Configuration.Save();
        }
        if (useCustomFontSize)
        {
            ImGui.SetNextItemWidth(120f);
            if (ImGui.InputFloat("Emote Log font size", ref fontSize, 1f, 2f, "%.1fpt"))
            {
                fontSize = Math.Max(fontSize, 1f);
                Configuration.FontSize = fontSize;
                Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Font size is affected by the Global Font Scale setting, default value is 12pt");
            }
            ImGui.SetNextItemWidth(120f);
            if (ImGui.InputFloat("Clear button font size", ref iconFontSize, 1f, 2f, "%.1fpt"))
            {
                iconFontSize = Math.Max(iconFontSize, 1f);
                Configuration.IconFontSize = iconFontSize;
                Configuration.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Font size is affected by the Global Font Scale setting, default value is 12pt");
            }
        }
        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Sync, "Refresh fonts"))
        {
            Plugin.PluginInterface.UiBuilder.FontAtlas.BuildFontsAsync();
        }
        ImGui.TextColored(ImGuiColors.DalamudGrey, "For font size changes to take effect press the refresh button");
    }

    private void KofiButton(float contentWidth)
    {
        var size = ImGui.CalcTextSize("Buy my cat a treat on Ko-fi") + ImGui.GetStyle().FramePadding * 2;
        if (SizeConstraints.HasValue)
        {
            var minimumSize = SizeConstraints.Value.MinimumSize * ImGui.GetIO().FontGlobalScale;
            contentWidth = Math.Max(contentWidth, minimumSize.X);
            if (ImGui.GetCursorPosY() + size.Y + ImGui.GetStyle().WindowPadding.Y <= minimumSize.Y)
            {
                ImGui.SetCursorPosY(minimumSize.Y - size.Y - ImGui.GetStyle().WindowPadding.Y);
            }
        }

        ImGui.Separator();
        ImGui.SetCursorPosX(contentWidth - size.X);
        using (ImRaii.PushColor(ImGuiCol.Text, 0xFFFFFFFF))
        using (ImRaii.PushColor(ImGuiCol.Button, 0xFFFF478E))
        using (ImRaii.PushColor(ImGuiCol.ButtonHovered, 0xFFFF2478))
        using (ImRaii.PushColor(ImGuiCol.ButtonActive, 0xFFBD0048))
            if (ImGui.Button("Buy my cat a treat on Ko-fi"))
            {
                Util.OpenLink("https://ko-fi.com/speedas");
            }
    }
}
