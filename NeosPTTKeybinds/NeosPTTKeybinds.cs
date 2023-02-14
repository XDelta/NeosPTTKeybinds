using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;

namespace NeosPTTKeybinds {
	public class NeosPTTKeybinds : NeosMod {
		public override string Name => "NeosPTTKeybinds";
		public override string Author => "Delta";
		public override string Version => "1.2.2";
		public override string Link => "https://github.com/XDelta/NeosPTTKeybinds/";

		//TODO keycode config for rebinding activation keys
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disableMuteKey = new ModConfigurationKey<bool>("disableMuteKey", "Disable the Mute toggle keybind M (requires restart)", () => false);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disablePTTKeys = new ModConfigurationKey<bool>("disableMousePTTKey", "Disable the PTT keybind Mouse5 and V (requires restart)", () => false);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> remapKeys = new ModConfigurationKey<bool>("remapKeys", "Enable to remap keys, will disable all default PTT keys (requires restart)", () => false);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customMuteKey = new ModConfigurationKey<Key>("customMuteKey", "Set custom Mute (toggle) keybind, set to 0 (None) to disable (requires restart)", () => Key.M);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customPTTKey = new ModConfigurationKey<Key>("customPTTKey", "Set custom PTT keybind, set to 0 (None) to disable (requires restart)", () => Key.V);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> MuteKey = new ModConfigurationKey<Key>("MuteKey", "Set Mute Mode keybind", () => Key.None);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> WhisperKey = new ModConfigurationKey<Key>("WhisperKey", "Set Whisper Mode keybind", () => Key.None);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> NormalKey = new ModConfigurationKey<Key>("NormalKey", "Set Normal Mode keybind", () => Key.None);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> ShoutKey = new ModConfigurationKey<Key>("ShoutKey", "Set Shout Mode keybind", () => Key.None);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> BroadcastKey = new ModConfigurationKey<Key>("BroadcastKey", "Set Broadcast Mode keybind", () => Key.None);

		private static ModConfiguration Config;

		public override void OnEngineInit() {
			Config = GetConfiguration();
			Config.Save(true);
			Harmony harmony = new Harmony("net.deltawolf.NeosPTTKeybinds");
			harmony.PatchAll();
			Msg("VoiceMode keybinds patched!");
		}

		//globalActions.ToggleMute.clearbindings() then add new bindings
		[HarmonyPatch(typeof(KeyboardAndMouseBindingGenerator), "Bind")]
		class KeyBind_Patch {
			public static void Postfix(KeyboardAndMouseBindingGenerator __instance, InputGroup group) {
				try {
					if (group is GlobalActions globalActions) {
						if (Config.GetValue(remapKeys)) {
							globalActions.ToggleMute.ClearBindings();
							globalActions.ActivateTalk.ClearBindings();
							Msg("Clearing all PTT binds as remapKeys is enabled");
							try {
								globalActions.ToggleMute.AddBinding(InputNode.Key(Config.GetValue(customMuteKey)));
								Msg("Remapped Mute to: " + Config.GetValue(customMuteKey));
								globalActions.ActivateTalk.AddBinding(InputNode.Key(Config.GetValue(customPTTKey)).Gate(InputNode.Key(Key.Control).Invert(), true, false));
								Msg("Remapped PTT to: " + Config.GetValue(customPTTKey));
							} catch (Exception e) {
								Error(e); //likely invalid keycode
							}
						} else {
							if (Config.GetValue(disableMuteKey)) {
								globalActions.ToggleMute.ClearBindings();
								Msg("Cleared Mute Key");
							}
							if (Config.GetValue(disablePTTKeys)) {
								globalActions.ActivateTalk.ClearBindings();
								Msg("Cleared PTT Keys");
							}
						}
					}
				} catch (Exception e){
					Error(e);
				}
			}
		}

		[HarmonyPatch(typeof(VoiceModeSync), "OnCommonUpdate")]
		class VoiceMode_Patch {
			static bool focus;
			static void Postfix(VoiceModeSync __instance) {
				focus = __instance.LocalUser.HasActiveFocus() || Engine.Current.WorldManager.FocusedWorld.LocalUser.HasActiveFocus();
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(MuteKey)) && !focus) {
                    __instance.InputInterface.IsMuted = true;
					Debug("Mute Keybind pressed: " + Config.GetValue(MuteKey));
				}
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(WhisperKey)) && !focus) {
					if (VoiceMode.Whisper <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
						__instance.FocusedWorldVoiceMode.Value = VoiceMode.Whisper;
                        __instance.InputInterface.IsMuted = false;
                        Debug("Whisper Keybind pressed: " + Config.GetValue(WhisperKey));
					}
				}
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(NormalKey)) && !focus ) {
					if (VoiceMode.Normal <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
						__instance.FocusedWorldVoiceMode.Value = VoiceMode.Normal;
                        __instance.InputInterface.IsMuted = false;
						Debug("Normal Keybind pressed: " + Config.GetValue(NormalKey));
					}
				}
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(ShoutKey)) && !focus) {
					if (VoiceMode.Shout <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
						__instance.FocusedWorldVoiceMode.Value = VoiceMode.Shout;
                        __instance.InputInterface.IsMuted = false;
						Debug("Shout Keybind pressed: " + Config.GetValue(ShoutKey));
					}
				}
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(BroadcastKey)) && !focus) {
					if (VoiceMode.Broadcast <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
						__instance.FocusedWorldVoiceMode.Value = VoiceMode.Broadcast;
                        __instance.InputInterface.IsMuted = false;
						Debug("Broadcast Keybind pressed: " + Config.GetValue(BroadcastKey));
					}
				}
			}
		}

        //Todo move keybinds to a different OnCommonUpdate so they continue to work with the dash closed, for now.. this..
        [HarmonyPatch(typeof(Userspace), "OnAttach")]
		class Userspace_Patch {
			static void Postfix(Userspace __instance) {
				Slot overlayRoot = __instance.World.GetGloballyRegisteredComponent<OverlayManager>().OverlayRoot; // Root/Userspace/Overlay/
				Slot vmp = overlayRoot.AddSlot("VoiceModeProxy", false);
				var vd = vmp.AttachComponent<ValueDriver<bool>>();
                vd.ValueSource.Target = vd.EnabledField;
                __instance.World.RunInUpdates(1, () => {
                    vd.DriveTarget.Target = vmp.AttachComponent<VoiceModeSync>().EnabledField;
                });
                vmp.AttachComponent<Comment>().Text.Value = "Generated by NeosPTTKeybinds mod by Delta";
			}
		}
	}
}