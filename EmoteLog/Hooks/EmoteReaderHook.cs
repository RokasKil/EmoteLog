using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using EmoteLog.Utils;
using System;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace EmoteLog.Hooks
{
    public unsafe class EmoteReaderHooks : IDisposable
    {
        public delegate void EmoteDelegate(IPlayerCharacter playerCharacter, uint emoteId);

        public event EmoteDelegate? OnEmote;

        private readonly Hook<EmoteController.Delegates.PlayEmote> playEmoteHook;

        public EmoteReaderHooks()
        {
            
            playEmoteHook = PluginServices.GameInteropProvider.HookFromAddress<EmoteController.Delegates.PlayEmote>(EmoteController.Addresses.PlayEmote.Value, PlayEmoteDetour);
            playEmoteHook.Enable();
        }

        private bool PlayEmoteDetour(EmoteController* emoteController, uint emoteId, EmoteController.PlayEmoteOption* options)
        {
            // PluginServices.PluginLog.Debug($"{(IntPtr)emoteController:X}, {emoteId}, {(IntPtr)options:X}");
            var result = playEmoteHook.Original(emoteController, emoteId, options);

            if (PluginServices.ObjectTable.LocalPlayer == null || emoteController == null ||
                emoteController->OwnerObject == null || options == null ||
                options->TargetId != PluginServices.ObjectTable.LocalPlayer.GameObjectId) return result;
            var sourceObject = PluginServices.ObjectTable.CreateObjectReference((IntPtr)emoteController->OwnerObject);
            if (sourceObject is IPlayerCharacter playerCharacter)
            {
                OnEmote?.Invoke(playerCharacter, emoteId);
            }
            return result;
        }

        public void Dispose()
        {
            playEmoteHook?.Dispose();
        }
    }
}
