using Lumina.Excel.GeneratedSheets;

namespace EmoteLog.Utils
{
    public class EmoteDataUtils
    {
        public static string? GetEmoteNameById(ushort id)
        {
            return PluginServices.DataManager.GetExcelSheet<Emote>()?.GetRow(id)?.Name.ToString();
        }

    }
}
