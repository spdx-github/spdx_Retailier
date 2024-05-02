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

The mod generates a set of files inside the "Savestore" subdirectory on runtime, which it then reads from. These files contain InteractablePreset names that are then pulled from the Toolbox's objectPresetDictionary by the code and modified.

The mod is relatively sandboxed, meaning that if an invalid name is added, the integrity of the game is preserved. For casual modders I would recommend the Unity Explorer available on the Thunderstore for browsing InteractablePresets and MenuPresets.

If you edit these files, please note that the game will throw an error if the JSON is improperly formatted - there are many JSON validators online which you can use to quickly check your file's good to go. Also, when the mod updates, these files might be changed or removed by r2modmanager. Please make a backup of modifications just in case!

## retailier_menus.json

This file consists of three primary parts: Aliases, Combines, and Menus. The mod processes this JSON into a dictionary of string arrays.

### Version

This is just a version number to help the mod track updates. Hopefully, files will be preserved between updates from now on.

### Menus

The first to be processed. The key is the MenuPreset name, and the value is an array containing the InteractablePreset names you'd like to add.

### Combines

Combines are second to be processed, and combine an arbitrary number of Menus into one. The key is the MenuPreset name to be combined into, and the value is an array containing names from Menus.

This now works with existing Menus, so you can create a "base" Menu and add the contents of other Menus to it by setting up a Combine.

### Aliases

Aliases are last to be processed now that all the entries are configured. It's a simple copy of the existing dictionary keypairs, just under different keys.

Some businesses, for example the Chinese restaurant, have different names to the MenuPresets they use (ChineseEatery > Chinese). These businesses' sold items are replaced just before the game finalizes city generation, as items sold by businesses seem to be "locked in" to game saves to prevent them from changing. The process I use aims to preserve these values through use of the game's own pseudorandom number generation algorithm.

## retailier_interactables.json

This file contains a list of InteractablePreset names and keypairs, whose keys are property names. At runtime, the mod runs through this list and 

Currently supported datatypes:

* Booleans (true and false)
* Strings
* Integers
* Floats
* Arrays (start with a string denoting type, wrap numbers in strings):
	* Vector2
	* Vector3
* Enumerators! (The mod will automatically detect the property type, then parse the name string to the appropriate Enum)

New datatypes will be implemented on request. I currently don't need to add any more. The current selection provides ample opportunity for basic parameter changing, and the default file should offer some examples.

# Source

The source code for this mod is available on GitHub: https://github.com/spdx-github/spdx_Retailier