//Lifted from https://github.com/MgAl2O4/PatMeDalamud
//and modified by me
using Dalamud.Hooking;
using EmoteLog.Utils;
using System;
using System.Linq;

namespace EmoteLog.Hooks
{
    public class EmoteReaderHooks : IDisposable
    {
        public delegate void EmoteDelegate(IPlayerCharacter playerCharacter, ushort emoteId);

        public event EmoteDelegate? OnEmote;

        public delegate void OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);
        private readonly Hook<OnEmoteFuncDelegate> hookEmote;

        public EmoteReaderHooks()
        {
            hookEmote = PluginServices.GameInteropProvider.HookFromSignature<OnEmoteFuncDelegate>("40 53 56 41 54 41 57 48 83 EC ?? 48 8B 02", OnEmoteDetour);
            hookEmote.Enable();
        }

        public void Dispose()
        {
            hookEmote?.Dispose();
        }

        void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
        {
            // unk - some field of event framework singleton? doesn't matter here anyway
            //PluginServices.PluginLog.Info($"Emote >> unk:{unk:X}, instigatorAddr:{instigatorAddr:X}, emoteId:{emoteId}, targetId:{targetId:X}, unk2:{unk2:X}");

            if (PluginServices.ClientState.LocalPlayer != null)
            {
                if (targetId == PluginServices.ClientState.LocalPlayer.GameObjectId)
                {
                    var instigatorOb = PluginServices.ObjectTable.FirstOrDefault(x => (ulong)x.Address == instigatorAddr);
                    if (instigatorOb != null && instigatorOb is IPlayerCharacter playerCharacter)
                    {
                        OnEmote?.Invoke(playerCharacter, emoteId);
                    }
                }
            }

            hookEmote.Original(unk, instigatorAddr, emoteId, targetId, unk2);
        }
    }
}
