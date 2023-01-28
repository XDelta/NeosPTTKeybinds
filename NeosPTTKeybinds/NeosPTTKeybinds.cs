﻿using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;

namespace NeosPTTKeybinds {
	public class NeosPTTKeybinds : NeosMod {
		public override string Name => "NeosPTTKeybinds";
		public override string Author => "Delta";
		public override string Version => "1.2.0";
		public override string Link => "https://github.com/XDelta/NeosPTTKeybinds/";

		//TODO keycode config for rebinding activation keys
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disableMuteKey = new ModConfigurationKey<bool>("disableMuteKey", "Disable the Mute toggle keybind M", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disablePTTKeys = new ModConfigurationKey<bool>("disableMousePTTKey", "Disable the PTT keybind Mouse5 and V", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> remapKeys = new ModConfigurationKey<bool>("remapKeys", "Enable to remap keys, will disable all default PTT keys", () => false);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customMuteKey = new ModConfigurationKey<Key>("customMuteKey", "Set custom Mute keybind, set to 0 (None) to disable", () => Key.M);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customPTTKey = new ModConfigurationKey<Key>("customPTTKey", "Set custom PTT keybind, set to 0 (None) to disable", () => Key.V);

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
					if (group is GlobalActions) {
						var globalActions = ((GlobalActions) group);
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
			static void Postfix(VoiceModeSync __instance) {
				if (__instance.InputInterface.GetKeyDown(Config.GetValue(WhisperKey))) {
					if (VoiceMode.Whisper <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
						__instance.FocusedWorldVoiceMode.Value = VoiceMode.Whisper;
						__instance.GlobalMute.Value = false;
                        Debug("Whisper Keybind pressed: " + Config.GetValue(WhisperKey));
                    }
                }
                if (__instance.InputInterface.GetKeyDown(Config.GetValue(NormalKey))) {
                    if (VoiceMode.Normal <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
                        __instance.FocusedWorldVoiceMode.Value = VoiceMode.Normal;
                        __instance.GlobalMute.Value = false;
                        Debug("Normal Keybind pressed: " + Config.GetValue(WhisperKey));
                    }
                }
                if (__instance.InputInterface.GetKeyDown(Config.GetValue(ShoutKey))) {
                    if (VoiceMode.Shout <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
                        __instance.FocusedWorldVoiceMode.Value = VoiceMode.Shout;
                        __instance.GlobalMute.Value = false;
                        Debug("Shout Keybind pressed: " + Config.GetValue(ShoutKey));
                    }
                }
                if (__instance.InputInterface.GetKeyDown(Config.GetValue(BroadcastKey))) {
                    if (VoiceMode.Broadcast <= __instance.FocusedWorldMaxAllowedVoiceMode.Value) {
                        __instance.FocusedWorldVoiceMode.Value = VoiceMode.Broadcast;
                        __instance.GlobalMute.Value = false;
                        Debug("Broadcast Keybind pressed: " + Config.GetValue(BroadcastKey));
                    }
                }
            }
		}
    }
}