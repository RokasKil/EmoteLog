//Lifted from https://github.com/MgAl2O4/PatMeDalamud
//and modified by me
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using EmoteLog.Utils;
using System;
using System.Linq;

namespace EmoteLog.Hooks
{
    public class EmoteReaderHooks : IDisposable
    {
        public delegate void EmoteDelegate(PlayerCharacter playerCharacter, ushort emoteId);

        public event EmoteDelegate? OnEmote;

        public delegate void OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);
        private readonly Hook<OnEmoteFuncDelegate> hookEmote;

        public EmoteReaderHooks()
        {
            hookEmote = PluginServices.GameInteropProvider.HookFromSignature<OnEmoteFuncDelegate>("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 48 89 7c 24 20 41 56 48 83 ec 30 4c 8b 74 24 60 48 8b d9", OnEmoteDetour);
            hookEmote.Enable();
        }

        public void Dispose()
        {
            hookEmote?.Dispose();
        }

        void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
        {
            // unk - some field of event framework singleton? doesn't matter here anyway
            //PluginLog.Log($"Emote >> unk:{unk:X}, instigatorAddr:{instigatorAddr:X}, emoteId:{emoteId}, targetId:{targetId:X}, unk2:{unk2:X}");

            if (PluginServices.ClientState.LocalPlayer != null)
            {
                if (targetId == PluginServices.ClientState.LocalPlayer.ObjectId)
                {
                    var instigatorOb = PluginServices.ObjectTable.FirstOrDefault(x => (ulong)x.Address == instigatorAddr);
                    if (instigatorOb != null && instigatorOb is PlayerCharacter playerCharacter)
                    {
                        OnEmote?.Invoke(playerCharacter, emoteId);
                    }
                }
            }

            hookEmote.Original(unk, instigatorAddr, emoteId, targetId, unk2);
        }
    }
}
