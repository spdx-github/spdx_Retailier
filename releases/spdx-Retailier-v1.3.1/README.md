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

The mod generates a pair of files, each inside their own folder in the "Savestore" subdirectory on first run, which it then reads from. These files contain InteractablePreset names that are then pulled from the Toolbox's objectPresetDictionary by the code and modified.

## menus/

This subdirectory contains all files pertaining to menu modifications. By default, `_base.json` is generated and placed inside for the mod to read from after its initial run.

Retailier loads JSON files from this directory and assembles them into a dictionary, with string keys and string array values. Each key corresponds to a MenuPreset name, and their values are sets of InteractablePreset names.

You can add more files to this folder as long as they're formatted properly! Please refer to the base file for a reference.

### Meta

This section contains version control information and whether or not its changes should completely replace (override) other changes. When set to false, these changes are additive.

### Menus

Retailier processes this structure first, populating the corresponding MenuPreset name key with InteractablePreset names.

### Combines

Combines are second to be processed, and combine an arbitrary number of Menus into one. The key is the MenuPreset name to be combined into, and the value is an array containing names from Menus.

This now works with existing Menus, so you can create a "base" Menu and add the contents of other Menus to it by setting up a Combine.

### Aliases

Aliases are last to be processed now that all the entries are configured. It's a simple copy of the existing dictionary keypairs, just under different keys.

Some businesses, for example the Chinese restaurant, have different names to the MenuPresets they use (ChineseEatery > Chinese). These businesses' sold items are replaced just before the game finalizes city generation, as items sold by businesses seem to be "locked in" to game saves to prevent them from changing. The process I use aims to preserve these values through use of the game's own pseudorandom number generation algorithm.

## interactables/

This subdirectory contains all files pertaining to interactable modifications. Like with `menus/`, a `_base.json` is generated, however it currently has no metadata and changes to existing names across tables may conflict.

Retailier loads files in this directory and compiles them into a list of key-value pairs. Each key is a two-entry array containing the InteractablePreset name and the property you'd like to change, followed by a value of a supported datatype.

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