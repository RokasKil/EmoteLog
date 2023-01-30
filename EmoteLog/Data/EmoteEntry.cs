using EmoteLog.Utils;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoteLog.Data
{
    public class EmoteEntry
    {
        public DateTime Timestamp;
        public string PlayerName { get; set; }
        public uint HomeWorld { get; set; }

        public ushort EmoteId { get; set; }

        public string EmoteName { get; set; }
        public EmoteEntry(DateTime timestamp, string playerName, uint homeWorld, ushort emoteId, string emoteName)
        {
            Timestamp = timestamp;
            PlayerName = playerName;
            HomeWorld = homeWorld;
            EmoteId = emoteId;
            EmoteName = emoteName;
        }
        public EmoteEntry(string playerName, uint homeWorld, ushort emoteId, string emoteName) : this(DateTime.Now, playerName, homeWorld, emoteId, emoteName) { }
        public EmoteEntry(string playerName, uint homeWorld, ushort emoteId) : this(playerName, homeWorld, emoteId, EmoteDataUtils.GetEmoteNameById(emoteId) ?? "null") { }

        public static bool operator ==(EmoteEntry a, EmoteEntry b)
        {
            return a.PlayerName == b.PlayerName && a.HomeWorld == b.HomeWorld && a.EmoteId == b.EmoteId;
        }
        public static bool operator !=(EmoteEntry a, EmoteEntry b)
        {
            return !(a == b);
        }
    }
}
