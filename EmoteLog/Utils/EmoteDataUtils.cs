using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
