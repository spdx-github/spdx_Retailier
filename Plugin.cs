using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using SOD.Common;
using Rewired.Utils;
using System.Reflection;
using System.Text.Json;
using Il2CppInterop.Runtime;
using System.Linq;

namespace Retailier;
[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Retailier : BasePlugin
{
	public const string PLUGIN_GUID = "spdx.Retailier";

	public const string PLUGIN_NAME = "[SPDX] Retailier";

	public const string PLUGIN_VERSION = "1.2.1";

	public static string PLUGIN_PATH = Lib.SaveGame.GetSavestoreDirectoryPath(Assembly.GetExecutingAssembly());

	public static string PLUGIN_PATH_MENUS = $"{PLUGIN_PATH}\\menus\\";
	public static string PLUGIN_PATH_INTERACTABLES = $"{PLUGIN_PATH}\\interactables\\";

	public static Dictionary<string, string[]> menus = SetupMenusDict(PLUGIN_PATH_MENUS);
	public static List<KeyValuePair<string[], object>> interactables = SetupInteractablesList(PLUGIN_PATH_INTERACTABLES);

	public override void Load()
	{
		if (!Directory.Exists(PLUGIN_PATH))
		{
			Directory.CreateDirectory(PLUGIN_PATH);
		}

		var harmony = new Harmony(PLUGIN_GUID);

		Log.LogInfo($"Loaded! Version {PLUGIN_VERSION}");

		harmony.PatchAll();
	}

	public static List<KeyValuePair<string[], object>> SetupInteractablesList(string path)
	{
		List<KeyValuePair<string[], object>> list = new List<KeyValuePair<string[], object>>();

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		if (!File.Exists($"{path}\\_base.json"))
		{
			// just create a new table as hardcoded default
			string table = "{\"BanhMi\": {\"destroyWhenAllConsumed\": true}, \"BungeoPpangWhole\": {\"destroyWhenAllConsumed\": true}, \"Eclair\": {\"destroyWhenAllConsumed\": true}, \"FairyBread\": {\"destroyWhenAllConsumed\": true}, \"KabuliBurger\": {\"destroyWhenAllConsumed\": true}, \"PocketWatch\": {\"isClock\": true, \"readingEnabled\": true, \"readingSource\": \"time\"}, \"Razor\": {\"fpsItemOffset\": [\"Vector3\", \"34\", \"-35\", \"-145\"]}, \"ReubenSandwich\": {\"destroyWhenAllConsumed\": true}, \"TikaToast\": {\"destroyWhenAllConsumed\": true}, \"TinnedFood\": {\"value\": [\"Vector2\", \"2\", \"4\"]}, \"WashingUpLiquid\": {\"value\": [\"Vector2\", \"5\", \"10\"]}, \"YorkiePie\": {\"destroyWhenAllConsumed\": true}}";

			File.WriteAllText($"{path}\\_base.json", table);

			Plugin.Log.LogInfo($"{PLUGIN_GUID}: Created default interactable table at {path}!");
		}

		string[] fileNames = Directory.GetFiles(path, "*.json");

		foreach (string fileName in fileNames)
		{
			string file = File.ReadAllText(fileName);
			var json = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(file);

			foreach (var item in json)
			{
				object value = null;

				foreach (var prop in item.Value)
				{
					switch (prop.Value.ValueKind)
					{
						case JsonValueKind.True:
							value = true;
							break;
						case JsonValueKind.False:
							value = false;
							break;
						case JsonValueKind.String:
							value = prop.Value.ToString();
							break;
						case JsonValueKind.Number:
							if (prop.Value.GetType() == typeof(int))
							{
								value = prop.Value.GetInt32();
							}
							else if (prop.Value.GetType() == typeof(double))
							{
								value = ((float)prop.Value.GetDouble());
							}
							break;
						case JsonValueKind.Array:
							// so arrays will be received as string lists
							List<string> valueList = JsonSerializer.Deserialize<List<string>>(prop.Value);
							// the first entry in the list will determine its type going forward
							switch (valueList[0].ToString().ToLower())
							{
								case "vector2":
									value = new Vector2(
										float.Parse(valueList[1]),
										float.Parse(valueList[2])
									);
									break;
								case "vector3":
									value = new Vector3(
										float.Parse(valueList[1]),
										float.Parse(valueList[2]),
										float.Parse(valueList[3])
									);
									break;
									/*case "objects":
										valueList.RemoveAt(0);
										value = valueList.ToArray();
										break;*/
							}
							break;
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
			string table = "{\"Meta\": { \"Version\": [\"1.2.1\"], \"Override\": [\"False\"] }, \"Aliases\": {\"Bar\": [\"AmericanBar\"], \"ChineseEatery\": [\"Chinese\"], \"FastFood\": [\"AmericanDiner\"], \"HardwareStore\": [\"Hardware\"]}, \"Combines\": {\"Supermarket\": [\"SupermarketFruit\", \"SupermarketMagazines\", \"SupermarketShelf\"]}, \"Menus\": {\"AmericanBar\": [\"FishNChipsInBox\", \"MushyPeas\", \"YorkiePie\", \"TikaToast\"], \"AmericanDiner\": [\"ReubenSandwich\", \"PoutineInBox\"], \"Ballroom\": [\"Eclair\", \"Crepe\"], \"BlackmarketTrader\": [\"PropGun\", \"Diamond\", \"JadeNecklace\", \"ClawOfTheFathomsFirstEdition\", \"ChateauDArc1868\"], \"Chemist\": [\"Glasses\", \"ToiletBrush\"], \"Chinese\": [\"Fishlafel\", \"KabuliBurger\", \"BanhMi\", \"BungeoPpangWhole\", \"Gimbap\", \"BreathMints\"], \"Hardware\": [\"PhotoChemicals\", \"FilmCanister\", \"Plunger\", \"MugEmpty\", \"PaintBucket\", \"PaintTube\", \"PaintBrush\", \"Pallette\", \"CleanPlate\", \"CleaningSpray\", \"PowerDrill\", \"PackingTape\", \"DuctTape\", \"Wool\", \"Thread\", \"KnittingNeedle\", \"JerryCan\", \"OilCan\", \"Bleach\", \"WashingUpLiquid\"], \"PawnShop\": [\"JadeNecklace\", \"WristWatch\", \"PocketWatch\", \"FilmCanister\", \"Katana\", \"TradingCard\", \"BaseballCap\"], \"SupermarketFruit\": [\"MegaMite\", \"Ketchup\", \"Mustard\", \"Vinegar\", \"Salt\", \"Pepper\", \"TinnedFood\", \"FairyBread\", \"PickapepperSauce\"], \"SupermarketMagazines\": [\"PackingTape\", \"Pencil\", \"Sharpener\", \"Eraser\", \"VideoTape\"], \"SupermarketShelf\": [\"Toothbrush\", \"Sponge\", \"Comb\", \"Camera\", \"FilmCanister\", \"MugEmpty\", \"Teacup\", \"WristWatch\", \"Battery\", \"Battery9V\", \"WhiteDice\", \"RedDice\", \"Bleach\", \"WashingUpLiquid\"]}}";

			File.WriteAllText($"{path}\\_base.json", table);

			Plugin.Log.LogInfo($"{PLUGIN_GUID}: Created default menu table at {path}!");
		}

		string[] fileNames = Directory.GetFiles(path, "*.json");

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
				Plugin.Log.LogWarning($"{Retailier.PLUGIN_GUID}: Failed to get version from JSON, configuration predates 1.1.1");
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
				if (overrideOthers && dict.TryGetValue(aliasSet.Key, out string[] toClear))
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

			foreach (var interactable in interactables)
			{
				InteractablePreset preset = Utils.GetInteractable(interactable.Key[0], false);

				Type propType = preset.GetType().GetProperty(interactable.Key[1]).PropertyType;

				if (propType.BaseType == typeof(Enum))
				{
					Utils.SetInteractableProp(preset, interactable.Key[1], Enum.Parse(propType, interactable.Value.ToString()), false);
				}
				/*else if (interactable.Value.GetType() == typeof(string[]))
				{
					Plugin.Log.LogInfo("Fired!");
					// Lists have hard types
					dynamic list = preset.GetType().GetProperty(interactable.Key[1]).GetValue(preset);
					Type memberType = preset.GetType().GetGenericArguments()[0];
					string[] names = (string[]) interactable.Value;

					var objectsOfType = Toolbox.FindObjectsOfType(Il2CppType.From(memberType)).Where(preset => names.Contains(preset.name));

					foreach (var obj in objectsOfType)
					{
						list.GetType().GetMethod("Add").Invoke(list, new object[] { obj } );
					}
				}*/
				else
				{
					Utils.SetInteractableProp(preset, interactable.Key[1], interactable.Value, false);
				}
			}

			Plugin.Log.LogInfo($"{Retailier.PLUGIN_GUID}: InteractablePresets patched!");
		}
	}

	[HarmonyPatch(typeof(Toolbox), "LoadAll")]
	internal class AddRetailItems
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
						if (!interactable.IsNullOrDestroyed())
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
						// ripped straight from the source. should use the pseudorandom number gen from the game
						float psdRandValue = (newItem.value.y - newItem.value.x) / 4f
							* Toolbox.Instance.GetPsuedoRandomNumberContained(-0.5f, 0.5f, entry.address.district.seed, out string seed);
						// if the item exists in the preset dictionary:
						if (!newItem.IsNullOrDestroyed())
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