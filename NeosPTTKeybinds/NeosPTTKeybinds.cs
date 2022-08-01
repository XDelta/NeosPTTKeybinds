using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;

namespace NeosPTTKeybinds {
	public class NeosPTTKeybinds : NeosMod {
		public override string Name => "NeosPTTKeybinds";
		public override string Author => "Delta";
		public override string Version => "1.1.0";
		public override string Link => "https://github.com/XDelta/NeosPTTKeybinds/";

		//TODO keycode config for rebinding activation keys
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disableMuteKey = new ModConfigurationKey<bool>("disableMuteKey", "Disable the Mute toggle keybind M", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disablePTTKeys = new ModConfigurationKey<bool>("disableMousePTTKey", "Disable the PTT keybind Mouse5 and V", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> remapKeys = new ModConfigurationKey<bool>("remapKeys", "Enable to remap keys, will disable all default PTT keys", () => false);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customMuteKey = new ModConfigurationKey<Key>("customMuteKey", "Set custom Mute keybind, set to 0 to disable", () => Key.M);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<Key> customPTTKey = new ModConfigurationKey<Key>("customPTTKey", "Set custom PTT keybind, set to 0 to disable", () => Key.V);

		private static ModConfiguration Config;

		public override void OnEngineInit() {
			Config = GetConfiguration();
			Config.Save(true);
			Harmony harmony = new Harmony("tk.deltawolf.NeosPTTKeybinds");
			harmony.PatchAll();
			Msg("PTT keybinds patched!");
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
	}
}