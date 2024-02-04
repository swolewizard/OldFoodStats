
# Mod to Revert Hearth and Home Food Changes and Balance


![Reverted Stats](https://i.imgur.com/oVlxL5A.jpg)


## Information:

- **Reversion of Food Stats to Pre-Hearth & Home Update:**  
  All food stats that were modified by the Hearth & Home update have been reverted back to their original values.

- **Rebalanced Hearth & Home Foods:**  
  To address the issue of new Hearth & Home foods being impractical, they have been rebalanced to offer similar utility as existing foods, allowing for a diverse food selection.

- **Toggle the New Fork Icon:**  
  You now have the option to disable the fork icons introduced in the Hearth & Home update for food items through the configuration settings.

- **Customize Your Own Stats:**  
  If the default values don't suit your preferences, customize the stats of each food item using the provided configuration files. Simply modify the .json files located in your BepInEx/config folder.

- **Automated Food Configuration:**  
  Implemented a robust code section that automatically scans the game for all consumable items & adding them to the configuration, including those added by other mods. This ensures that any new food items introduced in future updates will be seamlessly integrated into the configuration files.

- **Dynamic Food Configuration Management:**  
  Unused food items, whether removed from the game or from uninstalled mods, are automatically removed from the primary JSON configuration file "Huntard.OldFoodStats.json". To retain specific food items permanently, utilize the "Huntard.OldFoodStatsCustom.json" file, where items will remain unaffected by removal operations.

- **Enhanced Code Quality and Performance:**  
  Significant improvements have been made to the codebase, resulting in enhanced performance and reliability.

Utilize the Configuration Manager mod to toggle the fork icon on food items. A "Save" button is available to apply changes to the .json files while in-game. Json configurations are stored in the "Huntard.OldFoodStats.json" file in your BepInEx/config folder, offering granular control over food item stats.

Example Config for SerpentStew.

	{
	  "FoodPrefabName" : "SerpentStew",
	  "Health": 80,
	  "Stamina"  : 80,
	  "Duration" : 2400,
	  "HealthRegen" : 4,
      "FoodEitr"       : 0
	}

* Json configuration available in a new json file called "Huntard.OldFoodStatsCustom.json" in your "BepInEx/config" folder, which can be used to customize other mods foods values

Example Config for rk_bacon.

	{
	  "FoodPrefabName" : "rk_bacon",
	  "Health": 50,
	  "Stamina"  : 50,
	  "Duration" : 2400,
	  "HealthRegen" : 4,
      "FoodEitr"      : 0
	}

# Change Log:

Check nexus for all the changelogs for this mod:

[https://www.nexusmods.com/valheim/mods/1482](https://www.nexusmods.com/valheim/mods/1482)

# Credits:

* Big thanks to mlimg and his mod Valheim No Food Timer Text (H n H) for giving me permission to use his code to remove the fork icon from food.

# Installation:

* Make sure you have installed all other requirement mods.
* Drag the .dll into your \Valheim\BepInEx\plugins folder.

# Support:

If you like what I do and want to support me.

<a href="https://www.buymeacoffee.com/Huntard"><img src="https://img.buymeacoffee.com/button-api/?text=Buy me a coffee&emoji=&slug=Huntard&button_colour=FFDD00&font_colour=000000&font_family=Cookie&outline_colour=000000&coffee_colour=ffffff" /></a>

## Github:

Feel free to submit bug reports or pull requests here.

[https://github.com/swolewizard/OldFoodStats](https://github.com/swolewizard/OldFoodStats)


# Media:

* Every piece of food changed and configurable.

![Orignal Food](https://i.imgur.com/Wt8HDaH.jpg)
![HH Food](https://i.imgur.com/IoenUhb.jpg)


* Edited using the mods config.

![](https://i.imgur.com/zaFrTlR.jpeg)

