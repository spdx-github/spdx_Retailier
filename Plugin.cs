using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using SOD.Common;
using System.Reflection;
using System.Text.Json;
using AsmResolver.DotNet.Serialized;

namespace Retailier;
[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Retailier : BasePlugin
{
	public const string PLUGIN_GUID = "spdx.Retailier";

	public const string PLUGIN_NAME = "[SPDX] Retailier";

	public const string PLUGIN_VERSION = "1.3.3";

	public static string PLUGIN_PATH = Lib.SaveGame.GetSavestoreDirectoryPath(Assembly.GetExecutingAssembly());

	public static string PLUGIN_PATH_MENUS = $"{PLUGIN_PATH}\\menus\\";
	public static string PLUGIN_PATH_INTERACTABLES = $"{PLUGIN_PATH}\\interactables\\";
	public static string PLUGIN_PATH_FURNITURE = $"{PLUGIN_PATH}\\furniture\\";

	public static Dictionary<string, string[]> menus = SetupMenusDict(PLUGIN_PATH_MENUS);
	public static List<KeyValuePair<string[], object>> interactables = SetupInteractables(PLUGIN_PATH_INTERACTABLES);
	//public static List<KeyValuePair<string[], object>> furniture = SetupFurniture(PLUGIN_PATH_FURNITURE);

	public override void Load()
	{
		if (!Directory.Exists(PLUGIN_PATH))
		{
			Directory.CreateDirectory(PLUGIN_PATH);
		}

		var harmony = new Harmony(PLUGIN_GUID);

		Log.LogInfo($"Loaded!");

		harmony.PatchAll();
	}

	public static List<KeyValuePair<string[], object>> SetupInteractables(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		if (!File.Exists($"{path}\\_base.json"))
		{
			// just create a new table as hardcoded default
			string table = "{\"BanhMi\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Hamburger\"}}, \"BungeoPpangWhole\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Donut\"}}, \"Eclair\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Donut\"}}, \"FairyBread\": {\"destroyWhenAllConsumed\": true}, \"Gimbap\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Hamburger\"}}, \"KabuliBurger\": {\"destroyWhenAllConsumed\": true}, \"PocketWatch\": {\"isClock\": true, \"readingEnabled\": true, \"readingSource\": \"time\"}, \"Razor\": {\"fpsItemOffset\": [\"Vector3\", \"34\", \"-35\", \"-145\"]}, \"ReubenSandwich\": {\"destroyWhenAllConsumed\": true}, \"TikaToast\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Donut\"}}, \"TinnedFood\": {\"retailItem\": {\"Copies\": \"Donut\"}, \"value\": [\"Vector2\", \"2\", \"4\"]}, \"WashingUpLiquid\": {\"value\": [\"Vector2\", \"5\", \"10\"]}, \"YorkiePie\": {\"destroyWhenAllConsumed\": true, \"retailItem\": {\"Copies\": \"Donut\"}}}";

			File.WriteAllText($"{path}\\_base.json", table);

			Plugin.Log.LogInfo($"{PLUGIN_GUID}: Created default interactable table at {path}!");
		}

		List<KeyValuePair<string[], object>> list = SetupPropertyList(path);

		return list;
	}
	/*
	public static List<KeyValuePair<string[], object>> SetupFurniture(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		if (!File.Exists($"{path}\\_base.json"))
		{
			string table = "{}";

			File.WriteAllText($"{path}\\_base.json", table);

			Plugin.Log.LogInfo($"{PLUGIN_GUID}: Created default furniture table at {path}!");
		}

		List<KeyValuePair<string[], object>> list = SetupPropertyList(path);

		return list;
	}*/

	public static List<KeyValuePair<string[], object>> SetupPropertyList(string path)
	{
		List<KeyValuePair<string[], object>> list = new List<KeyValuePair<string[], object>>();

		string[] fileNames = Directory.GetFiles(path, "*.json");
		// quick sort
		Array.Sort(fileNames);

		foreach (string fileName in fileNames)
		{
			string file = File.ReadAllText(fileName);
			var json = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(file);

			foreach (var item in json)
			{
				object value = null;

				foreach (KeyValuePair<string, JsonElement> prop in item.Value)
				{
					// made this one in a fugue state so sorry if the comments are bullshit
					if(prop.Value.ValueKind == JsonValueKind.Object)
					{
						// make a new list of key value pairs, the object we received is going to be represented like that
						Dictionary<string, object> valueList = new Dictionary<string, object>();
						// deserialise the object as a dictionary of KVPs and iterate
						foreach (KeyValuePair<string, JsonElement> pair in prop.Value.Deserialize<Dictionary<string, JsonElement>>())
						{
							// add the KVP with the converted value to the list
							valueList.Add(pair.Key, Utils.ConvertJSONElement(pair.Value));
						}
						// send a dictionary to the list
						value = valueList;
					}
					else
					{
						value = Utils.ConvertJSONElement((JsonElement) prop.Value);
					}

					list.Add(new KeyValuePair<string[], object>(new string[] { item.Key, prop.Key }, value));
				}
			}
		}

		return list;
	}

	public static Dictionary<string, string[]> SetupMenusDict(string path)
	{
		Dictionary<string, string[]> dict = new Dictionary<string, string[]>();

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		if (!File.Exists($"{path}\\_base.json"))
		{
			// just create a new table as hardcoded default
			string table = "{\"Aliases\": {\"Bar\": [\"AmericanBar\"], \"ChineseEatery\": [\"Chinese\"], \"FastFood\": [\"AmericanDiner\"], \"HardwareStore\": [\"Hardware\"]}, \"Combines\": {\"Supermarket\": [\"SupermarketFruit\", \"SupermarketMagazines\", \"SupermarketShelf\"]}, \"Menus\": {\"AmericanBar\": [\"FishNChipsInBox\", \"MushyPeas\", \"YorkiePie\", \"TikaToast\"], \"AmericanDiner\": [\"ReubenSandwich\", \"PoutineInBox\"], \"Ballroom\": [\"Eclair\", \"Crepe\"], \"BlackmarketTrader\": [\"PropGun\", \"Diamond\", \"JadeNecklace\", \"ClawOfTheFathomsFirstEdition\", \"ChateauDArc1868\"], \"Chemist\": [\"Glasses\", \"ToiletBrush\", \"WashingUpLiquid\"], \"Chinese\": [\"Fishlafel\", \"KabuliBurger\", \"BanhMi\", \"BungeoPpangWhole\", \"Gimbap\", \"BreathMints\"], \"Hardware\": [\"PhotoChemicals\", \"FilmCanister\", \"Plunger\", \"MugEmpty\", \"PaintBucket\", \"PaintTube\", \"PaintBrush\", \"Pallette\", \"CleanPlate\", \"CleaningSpray\", \"PowerDrill\", \"PackingTape\", \"DuctTape\", \"Wool\", \"Thread\", \"KnittingNeedle\", \"JerryCan\", \"OilCan\", \"Bleach\", \"WashingUpLiquid\"], \"PawnShop\": [\"JadeNecklace\", \"WristWatch\", \"PocketWatch\", \"FilmCanister\", \"Katana\", \"TradingCard\", \"BaseballCap\"], \"SupermarketFruit\": [\"MegaMite\", \"Ketchup\", \"Mustard\", \"Vinegar\", \"Salt\", \"Pepper\", \"TinnedFood\", \"FairyBread\", \"PickapepperSauce\"], \"SupermarketMagazines\": [\"PackingTape\", \"Pencil\", \"Sharpener\", \"Eraser\", \"VideoTape\"], \"SupermarketShelf\": [\"Toothbrush\", \"Sponge\", \"Comb\", \"Camera\", \"FilmCanister\", \"MugEmpty\", \"Teacup\", \"WristWatch\", \"Battery\", \"Battery9V\", \"WhiteDice\", \"RedDice\", \"Bleach\", \"WashingUpLiquid\"]}, \"Meta\": {\"Override\": [\"False\"], \"Version\": [\"1.3.2\"]}}";

			File.WriteAllText($"{path}\\_base.json", table);

			Plugin.Log.LogInfo($"{PLUGIN_GUID}: Created default menu table at {path}!");
		}

		string[] fileNames = Directory.GetFiles(path, "*.json");
		// quick sort
		Array.Sort(fileNames);

		foreach (string fileName in fileNames)
		{
			string file = File.ReadAllText(fileName);
			var json = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string[]>>>(file);
			bool overrideOthers = false;

			try
			{
				if (json["Meta"]["Version"][0] != PLUGIN_VERSION)
				{
					Plugin.Log.LogWarning($"{Retailier.PLUGIN_GUID}: Version mismatch! Plugin is {PLUGIN_VERSION}, JSON version is {json["Meta"]["Version"][0]}");
				}

				overrideOthers = bool.Parse(json["Meta"]["Override"][0]);
			}
			catch
			{
				Plugin.Log.LogWarning($"{Retailier.PLUGIN_GUID}: No metadata in {Path.GetFileName(fileName)}!");
			}

			// handling Menus as in json
			foreach (KeyValuePair<string, string[]> menu in json["Menus"])
			{
				if (!dict.TryGetValue(menu.Key, out string[] none))
				{
					dict.Add(menu.Key, menu.Value);
				}
				else if (overrideOthers)
				{
					dict[menu.Key] = menu.Value;
				}
			}
			// this handles combines
			foreach (KeyValuePair<string, string[]> combine in json["Combines"])
			{
				// init as list so it's easy to append to
				List<string> itemsToCombine = new List<string>();

				foreach (string combineValKey in combine.Value)
				{
					itemsToCombine.AddRange(json["Menus"][combineValKey]);
				}

				// if the menu already exists:
				if (dict.TryGetValue(combine.Key, out string[] existingItems))
				{
					if (overrideOthers)
					{
						existingItems = itemsToCombine.ToArray();
					}
					else
					{
						existingItems.Concat(itemsToCombine);
					}
				}
				else
				{
					dict.Add(combine.Key, itemsToCombine.ToArray());
				}
			}

			// this handles aliases
			foreach (KeyValuePair<string, string[]> aliasSet in json["Aliases"])
			{
				if (overrideOthers && dict.TryGetValue(aliasSet.Key, out string[] none))
				{
					dict.Remove(aliasSet.Key);
				}

				foreach (string alias in aliasSet.Value)
				{
					dict.Add(aliasSet.Key, json["Menus"][alias]);
				}
			}
		}

		return dict;
	}
}

public class Patches
{
	[HarmonyPatch(typeof(Toolbox), "LoadAll")]
	internal class PatchInteractableProps
	{
		[HarmonyPostfix]
		public static void Postfix()
		{
			var interactables = Retailier.interactables;
			// cache this bc it's expensive
			var retailItemsCache = Resources.FindObjectsOfTypeAll<RetailItemPreset>();

			foreach (var interactable in interactables)
			{
				InteractablePreset preset = Utils.GetInteractable(interactable.Key[0], false);

				Type propType = preset.GetType().GetProperty(interactable.Key[1]).PropertyType;

				if (propType.BaseType == typeof(Enum))
				{
					Utils.SetInteractableProp(preset, interactable.Key[1], Enum.Parse(propType, interactable.Value.ToString()), false);
				}
				else if (propType == typeof(RetailItemPreset))
				{
					// so we're receiving a dictionary
					Dictionary<string, object> dict = interactable.Value as Dictionary<string, object>;
					RetailItemPreset newRetailItem = null;

					if (dict.TryGetValue("Copies", out object retailItemToCopy))
					{
						// create a clone of the RetailItemPreset
						newRetailItem = (RetailItemPreset) GameObject.Instantiate(retailItemsCache.Where(item => item.name == (string)retailItemToCopy).FirstOrDefault());
						// remove Copies from the dictionary since we're going to iterate over it
						dict.Remove("Copies");

						// setting up the new RetailItemPreset
						newRetailItem.name = interactable.Key[0];
						newRetailItem.itemPreset = preset;
						preset.retailItem = newRetailItem;
					}
					else
					{
						// this allows for modifying existing RetailItemPresets
						newRetailItem = preset.retailItem;
					}

					// now iterate
					if (dict.Count > 0)
					{
						foreach (KeyValuePair<string, object> kvp in dict)
						{
							Utils.SetRetailItemProp(newRetailItem, kvp.Key, kvp.Value);
						}
					}
				}
				else
				{
					Utils.SetInteractableProp(preset, interactable.Key[1], interactable.Value, false);
				}
			}

			Plugin.Log.LogInfo($"{Retailier.PLUGIN_GUID}: InteractablePresets patched!");
		}
	}
	/*
	[HarmonyPatch(typeof(Toolbox), "LoadAll")]
	internal class PatchFurniturePresets
	{
		[HarmonyPostfix]
		public static void Postfix()
		{

		}
	}*/

	[HarmonyPatch(typeof(Toolbox), "LoadAll")]
	internal class AddMenuItems
	{
		[HarmonyPostfix]
		public static void Postfix()
		{
			MenuPreset[] menuPresets = Resources.FindObjectsOfTypeAll<MenuPreset>();

			var dict = Retailier.menus;

			// iterate through menuPresets
			foreach (var preset in menuPresets)
			{
				// if the MenuPreset's name matches one that exists in the dictionary:
				if (dict.TryGetValue(preset.GetPresetName(), out var items))
				{
					foreach (var item in items)
					{
						var interactable = Utils.GetInteractable(item);
						// if the item of the given name exists in the game dictionary:
						if (interactable != null)
						{
							Utils.AddToMenu(preset, interactable);
						}
					}
				}
			}

			Plugin.Log.LogInfo($"{Retailier.PLUGIN_GUID}: MenuPresets patched!");
		}
	}

	[HarmonyPatch(typeof(CityConstructor), "Finalized")]
	internal class SetupCompanies
	{
		[HarmonyPrefix]
		public static void Prefix()
		{
			var dict = Retailier.menus;

			Company[] companies = CityData.Instance.companyDirectory.ToArray();

			foreach (Company entry in companies)
			{
				if (dict.TryGetValue(entry.preset.presetName, out var items))
				{
					foreach (var item in items)
					{
						InteractablePreset newItem = Utils.GetInteractable(item, false);

						string seed = entry.address.district.seed;
						// ripped straight from the source. should use the pseudorandom number gen from the game
						float psdRandValue = (newItem.value.y - newItem.value.x) / 4f
							* Toolbox.Instance.GetPsuedoRandomNumberContained(-0.5f, 0.5f, ref seed);
						// if the item exists in the preset dictionary:
						if (newItem != null)
						{
							// get the item's pseudorandom value
							int value = Mathf.RoundToInt(Mathf.Lerp(newItem.value.x, newItem.value.y,
								(float)entry.address.building.cityTile.landValue / 4f) + psdRandValue
							);
							// replace existing menu items' prices if they exist
							if (entry.prices.TryGetValue(newItem, out int priceToChange))
							{
								priceToChange = value;
							}
							else
							{
								entry.prices.Add(newItem, value);
							}
						}
					}
				}
			}

			Plugin.Log.LogInfo($"{Retailier.PLUGIN_GUID}: Companies patched!");
		}
	}
}