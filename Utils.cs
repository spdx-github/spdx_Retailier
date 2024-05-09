using Rewired.Utils;
using SOD.Common;
using System.Text.Json;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Retailier
{
	public class Utils
	{
		// Add an item to a given menu.
		public static void AddToMenu(MenuPreset menu, InteractablePreset interactable, bool suppress = true)
		{
			// if the item DOESN'T exist in the given menu:
			if (!menu.itemsSold.Contains(interactable))
			{
				menu.itemsSold.Add(interactable);
			}
			else if (!suppress)
			{
				Plugin.Log.LogError($"Key '{interactable.GetPresetName()}' already exists in {menu.GetPresetName()}.");
			}
		}
		// Overload. Add multiple items to one menu.
		public static void AddToMenu(MenuPreset menu, InteractablePreset[] interactables, bool suppress = true)
		{
			foreach (InteractablePreset interactable in interactables)
			{
				// if the item DOESN'T exist in the given menu:
				if (!menu.itemsSold.Contains(interactable))
				{
					menu.itemsSold.Add(interactable);
				}
				else if (!suppress)
				{
					Plugin.Log.LogError($"Key '{interactable.GetPresetName()}' already exists in {menu.GetPresetName()}.");
				}
			}
		}

		// Find an item in a dictionary by its name and return its interactable.
		public static InteractablePreset GetInteractable(string name, bool suppress = true)
		{
			var presetDict = Toolbox.Instance.objectPresetDictionary;

			if (presetDict.TryGetValue(name, out InteractablePreset preset))
			{
				return preset;
			}
			else
			{
				if (!suppress) { Plugin.Log.LogError($"'{name}' does not exist in given dictionary."); }
				return null;
			}
		}
		// Overload. Finds an item in a list instead.
		public static InteractablePreset GetInteractable(Il2CppSystem.Collections.Generic.List<InteractablePreset> presetList, string name, bool suppress = true)
		{
			{
				foreach (InteractablePreset preset in presetList)
				{
					if (preset.GetPresetName() == name)
					{
						return preset;
					}
				}

				if (!suppress) { Plugin.Log.LogError($"'{name}' does not exist in given list."); }
				return null;
			}
		}

		// Sets a field of a given type in an InteractablePreset to the desired value.
		public static void SetInteractableProp(InteractablePreset preset, string prop, object value, bool suppress = true)
		{
			if (!preset.IsNullOrDestroyed())
			{
				try
				{
					var propToChange = preset.GetType().GetProperty(prop);

					propToChange.SetValue(preset, value);
				}
				catch (Exception ex)
				{
					if (!suppress)
					{
						Plugin.Log.LogError($"Failed to change {preset.GetPresetName()}.{prop} to value '{value.ToString()}'.");
						Plugin.Log.LogError($"{ex.ToString()}");
					}
				}	
			}
			else if (!suppress)
			{
				Plugin.Log.LogError($"Preset passed is null or destroyed.");
			}
		}

		// Sets a field of a given type in an RetailItemPreset to the desired value.
		public static void SetRetailItemProp(RetailItemPreset preset, string prop, object value, bool suppress = true)
		{
			if (!preset.IsNullOrDestroyed())
			{
				try
				{
					var propToChange = preset.GetType().GetProperty(prop);

					propToChange.SetValue(preset, value);
				}
				catch (Exception ex)
				{
					if (!suppress)
					{
						Plugin.Log.LogError($"Failed to change {preset.GetPresetName()}.{prop} to value '{value.ToString()}'.");
						Plugin.Log.LogError($"{ex.ToString()}");
					}
				}
			}
			else if (!suppress)
			{
				Plugin.Log.LogError($"Preset passed is null or destroyed.");
			}
		}

		// Returns a System object from a JSON value.
		public static object ConvertJSONElement(JsonElement element)
		{
			object value = null;

			switch (element.ValueKind)
			{
				case JsonValueKind.True:
					value = true;
					break;

				case JsonValueKind.False:
					value = false;
					break;

				case JsonValueKind.String:
					value = element.ToString();
					break;

				case JsonValueKind.Number:
					if (element.GetType() == typeof(int))
					{
						value = element.GetInt32();
					}
					else if (element.GetType() == typeof(float))
					{
						value = (float) element.GetDouble();
					}
					break;

				case JsonValueKind.Array:
					// so arrays will be received as string lists
					List<string> valueList = JsonSerializer.Deserialize<List<string>>(element);
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

			return value;
		}
	}
}
