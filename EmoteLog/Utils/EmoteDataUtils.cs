using Lumina.Excel.Sheets;

namespace EmoteLog.Utils
{
    public static class EmoteDataUtils
    {
        public static string? GetEmoteNameById(ushort id)
        {
            return PluginServices.DataManager.GetExcelSheet<Emote>().TryGetRow(id, out var row) ? row.Name.ExtractText() : null;
        }

    }
}
