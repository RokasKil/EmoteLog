using Dalamud.Game.ClientState.Objects.SubKinds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmoteLog.Data
{
    public class EmoteQueue : IDisposable
    {
        public LinkedList<EmoteEntry> Log { get; set; }
        public LinkedList<CollapsedEmoteEntry> CollapsedLog { get; set; }
        private Plugin Plugin { get; }

        public EmoteQueue(Plugin plugin)
        {
            this.Plugin = plugin;
            this.Log = new LinkedList<EmoteEntry>();
            this.CollapsedLog = new LinkedList<CollapsedEmoteEntry>();
            this.Plugin.EmoteReaderHooks.OnEmote += OnEmote;
        }

        private void OnEmote(IPlayerCharacter playerCharacter, ushort emoteId)
        {
            if (this.Log.Count > 0)
            {
                while (this.Plugin.Configuration.LogSize <= this.Log.Count)
                {
                    Dequeue();
                }
            }
            EmoteEntry emoteEntry = new(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.RowId, emoteId);
            this.Log.AddFirst(emoteEntry);
            if (this.CollapsedLog.Count == 0 || !this.CollapsedLog.First().EmoteEntry.Equals(emoteEntry))
            {
                this.CollapsedLog.AddFirst(new CollapsedEmoteEntry(1, emoteEntry));
            }
            else
            {
                CollapsedEmoteEntry collapsedEmoteEntry = this.CollapsedLog.First();
                collapsedEmoteEntry.Count++;
                collapsedEmoteEntry.EmoteEntry = emoteEntry;
            }

        }

        private void Dequeue()
        {
            this.Log.RemoveLast();
            CollapsedEmoteEntry collapsedEntry = this.CollapsedLog.Last();
            collapsedEntry.Count--;
            if (collapsedEntry.Count == 0)
            {
                this.CollapsedLog.RemoveLast();
            }
        }

        public void Dispose()
        {
            this.Plugin.EmoteReaderHooks.OnEmote -= OnEmote;
            Clear();
        }

        public void Clear()
        {
            Log.Clear();
            CollapsedLog.Clear();
        }
    }
}
