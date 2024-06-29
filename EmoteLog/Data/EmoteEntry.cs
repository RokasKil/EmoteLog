using EmoteLog.Utils;
using System;

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

        public override bool Equals(object? obj)
        {
            if (obj is EmoteEntry entry)
            {
                return this.PlayerName == entry.PlayerName && this.HomeWorld == entry.HomeWorld && this.EmoteId == entry.EmoteId;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerName, HomeWorld, EmoteId);
        }
    }
}
