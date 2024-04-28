# [SPDX] Retailier

Default functionality: adds some items I thought were "missing" from the ingame business menus. Also changes some items so that they're removed when they're consumed.

Extended functionality (for modders): lets you add items to vendor lists as long as they're in Toolbox.Instance.objectPresetDictionary, and lets you change (some) properties of InteractablePresets!

# Installation

Installation is best done through r2modmanager and Thunderstore. Manual installation is standard for BepInEx plugins:

* Ensure BepInEx is installed and you've run the game once
* Install SODCommon: https://thunderstore.io/c/shadows-of-doubt/p/Venomaus/SODCommon/
* Go to <game directory>/BepInEx/plugins and extract the folder inside spdx-Retailer.zip into that directory
* Run the game!

# Extended Functionality

The mod, by default, generates a set of files inside the "Savestore" subdirectory on runtime, which it then reads from. These files contain InteractablePreset names that are then pulled from the Toolbox's objectPresetDictionary by the code and modified.

The mod is relatively sandboxed, meaning that if an invalid name is added, the integrity of the game is preserved. For casual modders I would recommend the Unity Explorer available on the Thunderstore for browsing InteractablePresets and MenuPresets.

If you edit these files, please note that the game may throw an error if the JSON is improperly formatted.

## retailier_menus.json

This file consists of three primary parts: Aliases, Combines, and Menus. The mod processes this JSON into a dictionary of string arrays.

### Menus

The first to be processed. They're straight copies from the file; the key is the MenuPreset name, and the value is the array of strings attached.

### Combines

Combines are second to be processed, and combine an arbitrary number of Menus into one. The key is the MenuPreset name to be combined into.

Please note: this does not currently support "building off" existing key-value pairs, as it's a direct replacement. I'm hoping to change that soon.

### Aliases

Aliases are last to be processed now that all the entries are configured. It's a simple copy of the existing dictionary keypairs, just under different keys.

Some businesses, for example the Chinese restaurant, have different names to the MenuPresets they use (ChineseEatery > Chinese). These businesses' sold items are replaced just before the game finalizes city generation, as items sold are often "locked in" to game saves to prevent them from changing.

## retailier_interactables.json

This file contains a list of InteractablePreset names and parameters. At runtime, the names are matched to their presets and the given parameter is parsed and changed depending on its JsonValueType.

Currently supported datatypes:

* Booleans (true and false)
* Strings
* Integers
* Floats
* Arrays (start with a string denoting type, wrap numbers in strings):
	* Vector2
	* Vector3

New datatypes will be implemented on request. I currently don't need to add any more. The current selection provides ample opportunity for basic parameter changing, and the default file should offer some examples.

# Source

The source code for this mod is available on GitHub: https://github.com/spdx-github/spdx_Retailier