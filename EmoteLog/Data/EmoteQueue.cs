using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Plugin.EmoteReaderHooks.OnEmote += onEmote;
        }
        
        private void onEmote(PlayerCharacter playerCharacter, ushort emoteId)
        {
            if (this.Log.Count > 0)
            {
                while (this.Plugin.Configuration.LogSize <= this.Log.Count)
                {
                    dequeue();
                }
            }
            EmoteEntry emoteEntry = new EmoteEntry(playerCharacter.Name.ToString(), playerCharacter.HomeWorld.Id, emoteId);
            this.Log.AddFirst(emoteEntry);
            if (this.CollapsedLog.Count == 0 || this.CollapsedLog.First().EmoteEntry != emoteEntry)
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

        private void dequeue()
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
            this.Plugin.EmoteReaderHooks.OnEmote -= onEmote;
            Clear();
        }

        public void Clear()
        {
            Log.Clear();
            CollapsedLog.Clear();
        }
    }
}
