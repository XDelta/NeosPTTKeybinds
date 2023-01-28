# NeosPTTKeybinds

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that can disable or remap the Push to Talk / Mute keybinds on Mouse5, M, and V. Set remapped keys using [NeosModSettings](https://github.com/badhaloninja/NeosModSettings) in-game or manually with the numbers from the [Key Code Reference](https://github.com/XDelta/NeosPTTKeybinds/blob/main/KeyCodeReference.txt). Additionally allows setting keybinds to set your current Voicemode. Requires NeosModLoader 1.8.0+

## Installation
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
2. Place [NeosPTTKeybinds.dll](https://github.com/XDelta/NeosPTTKeybinds/releases/latest/download/NeosPTTKeybinds.dll) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR\nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Neos logs.

## Config Options

| Config Option     | Default | Description |
| ------------------ | ------- | ----------- |
| `disableMuteKey` | `true` | Disables the Mute toggle keybind M |
| `disablePTTKeys` | `true` | Disables the PTT keybind Mouse5 and V |
| `remapKeys` | `false` | If `true` will disable the default PTT/Mute keys listed above and use the custom Keys defined below |
| `customMuteKey` | 109 (M) | Set custom Mute keybind, set to 0 to disable |
| `customPTTKey` | 118 (V) | Set custom PTT keybind, set to 0 to disable |
| `WhisperKey` | 0 (None) | Set Whisper Mode keybind |
| `NormalKey` | 0 (None) | Set Normal Mode keybind |
| `ShoutKey` | 0 (None) | Set Shout Mode keybind |
| `BroadcastKey` | 0 (None) | Set Broadcast Mode keybind |
