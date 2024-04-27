using Rewired.Utils;
using SOD.Common;

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
	}
}
