using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OldFoodStats
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    [BepInDependency("com.jotunn.jotunn", BepInDependency.DependencyFlags.HardDependency)]
    public class FoodMod : BaseUnityPlugin
    {
        private const string ModName = "Huntards H&H Old Food Stats";
        private const string ModVersion = "3.0.0";
        private const string ModGUID = "Huntard.OldFoodStats";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        public static string CustomconfigPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}Custom.json");
        private Harmony _harmony;

        private List<FoodConfig> foodStats = new List<FoodConfig>();

        [HarmonyPatch(typeof(InventoryGrid), "UpdateGui")]
        public class InventoryGrid_UpdateGui
        {
            [HarmonyPostfix]
            public static void Postfix(InventoryGrid __instance, ref Player player, ref ItemDrop.ItemData dragItem, ref Inventory ___m_inventory, ref List<FoodMod.Element> ___m_elements)
            {
                if (__instance != null && ___m_elements != null)
                {
                    int width = ___m_inventory.GetWidth();
                    int height = ___m_inventory.GetHeight();

                    if (FoodMod.Hidefork.Value == true)
                    {
                        foreach (ItemDrop.ItemData itemData in ___m_inventory.GetAllItems())
                        {
                            if (itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable && (itemData.m_shared.m_food > 0f || itemData.m_shared.m_foodStamina > 0f))
                            {
                                int index = itemData.m_gridPos.y * width + itemData.m_gridPos.x;

                                FoodMod.Element element4 = ___m_elements[index];
                                element4.m_food.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        foreach (ItemDrop.ItemData itemData in ___m_inventory.GetAllItems())
                        {
                            if (itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable && (itemData.m_shared.m_food > 0f || itemData.m_shared.m_foodStamina > 0f))
                            {
                                int index = itemData.m_gridPos.y * width + itemData.m_gridPos.x;

                                FoodMod.Element element4 = ___m_elements[index];
                                element4.m_food.gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);

            base.Config.SaveOnConfigSet = true;
            Hidefork = base.Config.Bind<bool>("General", "Hide fork icon", false, new ConfigDescription("Hides food icon on food.", null));
            SaveButton = base.Config.Bind<bool>("General", "Save Config Values", false, new ConfigDescription("Toggle this boolean to re-apply your configuration values, can be done in-game.", null));


            LoadConfig();
            PrefabManager.OnVanillaPrefabsAvailable += RegisterConfigValues;
            PrefabManager.OnVanillaPrefabsAvailable += ScanAndGenerateConsumables;
            ZoneManager.OnVanillaLocationsAvailable += RegisterConfigValues;
            ZoneManager.OnVanillaLocationsAvailable += RegisterCustomConfigValues;
            SaveButton.SettingChanged += UpdateSettings;
        }
        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                GenerateConfigFileFirst();
                Jotunn.Logger.LogInfo("Generated Config");
            }
            if (!File.Exists(CustomconfigPath))
            {
                GenerateCustomConfig();
                Jotunn.Logger.LogInfo("Generated Custom Config");
            }
            return;
        }

        private void RegisterConfigValues()
        {
            UpdateConfigValues(configPath, "Updated Configs");
        }

        private void ScanAndGenerateConsumables()
        {
            foodStats.Clear();

            var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var go in allGameObjects)
            {
                var itemDrop = go.GetComponent<ItemDrop>();
                if (itemDrop != null && itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable)
                {
                    foodStats.Add(new FoodConfig
                    {
                        FoodPrefabName = itemDrop.name,
                        Health = itemDrop.m_itemData.m_shared.m_food,
                        Stamina = itemDrop.m_itemData.m_shared.m_foodStamina,
                        Duration = itemDrop.m_itemData.m_shared.m_foodBurnTime,
                        HealthRegen = itemDrop.m_itemData.m_shared.m_foodRegen,
                        FoodEitr = itemDrop.m_itemData.m_shared.m_foodEitr
                    });
                }
            }

            string jsonText = JsonMapper.ToJson(foodStats);

            File.WriteAllText(configPath, jsonText);
        }

        private void RegisterCustomConfigValues()
        {
            UpdateConfigValues(CustomconfigPath, "Updated Custom Configs");
        }

        private void GenerateConfigFileFirst()
        {
            var foodConfigs = new List<FoodConfig>();

            Dictionary<string, (int Health, int Stamina, int Duration, int HealthRegen, int FoodEitr)> foodValues = new Dictionary<string, (int, int, int, int, int)>
            {
                { "SerpentStew", (80, 80, 2400, 4, 0) },
                { "LoxPie", (80, 80, 2400, 4, 0) },
                { "FishWraps", (60, 90, 2400, 4, 0) },
                { "BloodPudding", (90, 50, 2400, 4, 0) },
                { "SerpentMeatCooked", (70, 40, 2000, 3, 0) },
                { "CookedLoxMeat", (70, 40, 2000, 3, 0) },
                { "Bread", (40, 70, 1800, 2, 0) },
                { "TurnipStew", (50, 50, 1600, 2, 0) },
                { "Sausages", (60, 40, 1600, 3, 0) },
                { "CarrotSoup", (20, 60, 1500, 2, 0) },
                { "QueensJam", (30, 40, 1200, 2, 0) },
                { "NeckTailGrilled", (35, 20, 1000, 2, 0) },
                { "CookedMeat", (40, 30, 1200, 2, 0) },
                { "FishCooked", (45, 25, 1200, 2, 0) },
                { "MushroomYellow", (20, 20, 600, 1, 0) },
                { "MushroomBlue", (20, 20, 600, 1, 0) },
                { "Mushroom", (15, 25, 600, 1, 0) },
                { "Honey", (20, 20, 300, 5, 0) },
                { "Cloudberry", (15, 25, 800, 1, 0) },
                { "Blueberries", (15, 20, 600, 1, 0) },
                { "Raspberry", (10, 20, 600, 1, 0) },
                { "Carrot", (15, 15, 600, 1, 0) },
                { "CookedWolfMeat", (45, 35, 1200, 3, 0) },
                { "CookedDeerMeat", (40, 30, 1200, 2, 0) },
                { "BlackSoup", (50, 50, 1200, 3, 0) },
                { "DeerStew", (60, 45, 1500, 3, 0) },
                { "Eyescream", (40, 65, 1500, 1, 0) },
                { "MinceMeatSauce", (45, 30, 1500, 3, 0) },
                { "OnionSoup", (40, 60, 1500, 1, 0) },
                { "ShocklateSmoothie", (40, 50, 1200, 1, 0) },
                { "WolfMeatSkewer", (65, 40, 1500, 3, 0) },
                { "WolfJerky", (35, 35, 1800, 1, 0) },
                { "BoarJerky", (25, 25, 1800, 1, 0) }
            };

            foreach (var kvp in foodValues)
            {
                var foodConfig = new FoodConfig
                {
                    FoodPrefabName = kvp.Key,
                    Health = kvp.Value.Health,
                    Stamina = kvp.Value.Stamina,
                    Duration = kvp.Value.Duration,
                    HealthRegen = kvp.Value.HealthRegen,
                    FoodEitr = kvp.Value.FoodEitr
                };
                foodConfigs.Add(foodConfig);
            }

            var jsonText = JsonMapper.ToJson(foodConfigs);
            File.WriteAllText(configPath, jsonText);
        }

        private void GenerateCustomConfig()
        {

            var foodConfigs = new List<FoodConfig>();
            var jsonText = JsonMapper.ToJson(foodConfigs);
            File.WriteAllText(CustomconfigPath, jsonText);

        }

        private void UpdateConfigValues(string path, string logMessage)
        {
            var foodConfigs = GetJson(path);

            foreach (var config in foodConfigs)
            {
                try
                {
                    var prefab = PrefabManager.Cache.GetPrefab<ItemDrop>(config.FoodPrefabName);
                    var shared = prefab.m_itemData.m_shared;

                    shared.m_food = config.Health;
                    shared.m_foodStamina = config.Stamina;
                    shared.m_foodBurnTime = config.Duration;
                    shared.m_foodRegen = config.HealthRegen;
                    shared.m_foodEitr = config.FoodEitr;
                }
                catch (Exception e)
                {
                    string errorMessage;
                    if (path == FoodMod.CustomconfigPath)
                    {
                        errorMessage = $"Loading config for {config.FoodPrefabName} failed. Double check your custom config settings to confirm the prefab name is correct. Error: {e.Message}";
                    }
                    else
                    {
                        errorMessage = $"Loading config for {config.FoodPrefabName} failed, so it has been removed from the JSON configuration. Error: {e.Message}";
                    }
                    Jotunn.Logger.LogError(errorMessage);
                }
            }

            if (File.Exists(path) && path != CustomconfigPath)
            {
                File.Delete(path);
                ScanAndGenerateConsumables();
                Jotunn.Logger.LogInfo(logMessage);
            }
        }

        internal static List<FoodConfig> GetJson(string path)
        {
            Jotunn.Logger.LogDebug($"Attempting to load config file from path {path}");
            var jsonText = AssetUtils.LoadText(path);
            Jotunn.Logger.LogDebug("File found. Attempting to deserialize...");
            var foodconfigs = JsonMapper.ToObject<List<FoodConfig>>(jsonText);
            return foodconfigs;
        }
        public void UpdateSettings(object sender, EventArgs e)
        {
            Jotunn.Logger.LogInfo("Updating...");
            this.RegisterConfigValues();
            this.RegisterCustomConfigValues();
        }

        public static ConfigEntry<bool> Hidefork;
        public static ConfigEntry<bool> SaveButton;
        public class Element
        {
            public Vector2i m_pos;

            public GameObject m_go;

            public Image m_icon;

            public Text m_amount;

            public Text m_quality;

            public Image m_equiped;

            public Image m_queued;

            public GameObject m_selected;

            public Image m_noteleport;

            public Image m_food;

            public bool m_used;
        }
    }

    public class FoodConfig
    {
        public string FoodPrefabName { get; set; }
        public float Health { get; set; }
        public float Stamina { get; set; }
        public float Duration { get; set; }
        public float HealthRegen { get; set; }
        public float FoodEitr { get; set; }
    }
}
