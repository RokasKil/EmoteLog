//Lifted from https://github.com/MgAl2O4/PatMeDalamud
//and modified by me
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using System;
using System.Linq;

namespace EmoteLog.Hooks
{
    public class EmoteReaderHooks : IDisposable
    {

        public Action<GameObject, ushort> OnEmote;

        public delegate void OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);
        private readonly Hook<OnEmoteFuncDelegate> hookEmote;

        public bool IsValid = false;

        public EmoteReaderHooks()
        {
            try
            {
                var emoteFuncPtr = PluginServices.SigScanner.ScanText("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 48 89 7c 24 20 41 56 48 83 ec 30 4c 8b 74 24 60 48 8b d9 48 81 c1 60 2f 00 00");
                hookEmote = Hook<OnEmoteFuncDelegate>.FromAddress(emoteFuncPtr, OnEmoteDetour);
                hookEmote.Enable();

                IsValid = true;
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "oh noes!");
            }
        }

        public void Dispose()
        {
            hookEmote?.Dispose();
            IsValid = false;
        }

        void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
        {
            // unk - some field of event framework singleton? doesn't matter here anyway
             PluginLog.Log($"Emote >> unk:{unk:X}, instigatorAddr:{instigatorAddr:X}, emoteId:{emoteId}, targetId:{targetId:X}, unk2:{unk2:X}");

            /*if (PluginServices.ClientState.LocalPlayer != null)
            {
                if (targetId == PluginServices.ClientState.LocalPlayer.ObjectId)
                {
                    var instigatorOb = PluginServices.ObjectTable.FirstOrDefault(x => (ulong)x.Address == instigatorAddr);
                    if (instigatorOb != null)
                    {
                        bool canCount = (instigatorOb.ObjectId != targetId);
#if DEBUG
                        canCount = true;
#endif  
                        if (canCount)
                        {
                            OnEmote?.Invoke(instigatorOb, emoteId);
                        }
                    }
                }
            }*/

            hookEmote.Original(unk, instigatorAddr, emoteId, targetId, unk2);
        }
    }
}