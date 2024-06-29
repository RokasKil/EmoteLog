namespace EmoteLog.Data
{
    public class CollapsedEmoteEntry
    {
        public int Count { get; set; }
        public EmoteEntry EmoteEntry { get; set; }
        public CollapsedEmoteEntry(int count, EmoteEntry emoteEntry)
        {
            Count = count;
            EmoteEntry = emoteEntry;
        }

    }
}
