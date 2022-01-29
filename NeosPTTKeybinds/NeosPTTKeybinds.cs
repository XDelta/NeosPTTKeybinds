using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;

namespace NeosPTTKeybinds {
	public class NeosPTTKeybinds : NeosMod {
		public override string Name => "NeosPTTKeybinds";
		public override string Author => "Delta";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/XDelta/NeosPTTKeybinds/";

		//TODO keycode config for rebinding activation keys
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disableMuteKey = new ModConfigurationKey<bool>("disableMuteKey", "Disables the Mute toggle keybind M", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> disablePTTKeys = new ModConfigurationKey<bool>("disableMousePTTKey", "Disables the PTT keybind Mouse5 and V", () => true);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<bool> reenableVPTTKey = new ModConfigurationKey<bool>("reenableVPTTKey", "Re-enables the PTT keybind V", () => false);

		private static ModConfiguration Config;

		public override void OnEngineInit() {
			Config = GetConfiguration();
			Config.Save(true);
			if (Config.GetValue(disableMuteKey)) {
				Debug("Config appears to work, found the value from disableMuteKey");
			}
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
						if (Config.GetValue(disableMuteKey)) {
							globalActions.ToggleMute.ClearBindings();
							Msg("Cleared Mute Key");
						}
						if (Config.GetValue(disablePTTKeys)) {
							globalActions.ActivateTalk.ClearBindings();
							Msg("Cleared PTT Keys");
						}
						if (Config.GetValue(reenableVPTTKey)) {
							globalActions.ActivateTalk.AddBinding(InputNode.Key(Key.V).Gate(InputNode.Key(Key.Control).Invert(), true, false));
							Msg("Re-enabled V Key");
						}
					}
				} catch (Exception e){
					Error(e);
				}
			}
		}
	}
}