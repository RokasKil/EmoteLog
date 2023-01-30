using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoteLog.Data
{
    public class CollapsedEmoteEntry
    {
        public int Count { get; set; }
        public EmoteEntry EmoteEntry { get; set; }
        public CollapsedEmoteEntry (int count, EmoteEntry emoteEntry)
        {
            Count = count;
            EmoteEntry = emoteEntry;
        }

    }
}
