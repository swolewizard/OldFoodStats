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
        private const string ModVersion = "2.0.3";
        private const string ModGUID = "Huntard.OldFoodStats";

        public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, $"{ModGUID}.json");
        private Harmony _harmony;

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
            ItemManager.OnItemsRegistered += RegisterConfigValues;
            PrefabManager.OnPrefabsRegistered += RegisterConfigValues;
            SaveButton.SettingChanged += UpdateSettings;
        }
        public void UpdateSettings(object sender, EventArgs e)
        {
            Jotunn.Logger.LogInfo("Updating...");
            this.RegisterConfigValues();
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                GenerateConfigFileFirst();
                Jotunn.Logger.LogInfo("Generated new configs");
                return;
            }
            return;
        }

        private void GenerateConfigFileFirst()
        {

            var foodConfigs = new List<FoodConfig>();

            //Serpent Stew
            var serpentstewConfig = new FoodConfig();
            serpentstewConfig.FoodPrefabName = "SerpentStew";
            serpentstewConfig.Health = 80;
            serpentstewConfig.Stamina = 80;
            serpentstewConfig.Duration = 2400;
            serpentstewConfig.HealthRegen = 4;
            foodConfigs.Add(serpentstewConfig);

            //Lox Pie
            var loxpieConfig = new FoodConfig();
            loxpieConfig.FoodPrefabName = "LoxPie";
            loxpieConfig.Health = 80;
            loxpieConfig.Stamina = 80;
            loxpieConfig.Duration = 2400;
            loxpieConfig.HealthRegen = 4;
            foodConfigs.Add(loxpieConfig);

            //Fish Wrap
            var fishwrapConfig = new FoodConfig();
            fishwrapConfig.FoodPrefabName = "FishWraps";
            fishwrapConfig.Health = 60;
            fishwrapConfig.Stamina = 90;
            fishwrapConfig.Duration = 2400;
            fishwrapConfig.HealthRegen = 4;
            foodConfigs.Add(fishwrapConfig);

            //Blood Pudding
            var bloodpuddingConfig = new FoodConfig();
            bloodpuddingConfig.FoodPrefabName = "BloodPudding";
            bloodpuddingConfig.Health = 90;
            bloodpuddingConfig.Stamina = 50;
            bloodpuddingConfig.Duration = 2400;
            bloodpuddingConfig.HealthRegen = 4;
            foodConfigs.Add(bloodpuddingConfig);

            //Cooked SerpentMeat
            var cookedserpentmeatConfig = new FoodConfig();
            cookedserpentmeatConfig.FoodPrefabName = "SerpentMeatCooked";
            cookedserpentmeatConfig.Health = 70;
            cookedserpentmeatConfig.Stamina = 40;
            cookedserpentmeatConfig.Duration = 2000;
            cookedserpentmeatConfig.HealthRegen = 3;
            foodConfigs.Add(cookedserpentmeatConfig);

            //Cooked LoxMeat
            var cookedloxmeatConfig = new FoodConfig();
            cookedloxmeatConfig.FoodPrefabName = "CookedLoxMeat";
            cookedloxmeatConfig.Health = 70;
            cookedloxmeatConfig.Stamina = 40;
            cookedloxmeatConfig.Duration = 2000;
            cookedloxmeatConfig.HealthRegen = 3;
            foodConfigs.Add(cookedloxmeatConfig);

            //Bread
            var breadConfig = new FoodConfig();
            breadConfig.FoodPrefabName = "Bread";
            breadConfig.Health = 40;
            breadConfig.Stamina = 70;
            breadConfig.Duration = 1800;
            breadConfig.HealthRegen = 2;
            foodConfigs.Add(breadConfig);

            //Turnip Stew
            var turnipstewConfig = new FoodConfig();
            turnipstewConfig.FoodPrefabName = "TurnipStew";
            turnipstewConfig.Health = 50;
            turnipstewConfig.Stamina = 50;
            turnipstewConfig.Duration = 1600;
            turnipstewConfig.HealthRegen = 2;
            foodConfigs.Add(turnipstewConfig);

            //Sausages
            var sausagesConfig = new FoodConfig();
            sausagesConfig.FoodPrefabName = "Sausages";
            sausagesConfig.Health = 60;
            sausagesConfig.Stamina = 40;
            sausagesConfig.Duration = 1600;
            sausagesConfig.HealthRegen = 3;
            foodConfigs.Add(sausagesConfig);

            //Carrot Soup
            var carrotsoupConfig = new FoodConfig();
            carrotsoupConfig.FoodPrefabName = "CarrotSoup";
            carrotsoupConfig.Health = 20;
            carrotsoupConfig.Stamina = 60;
            carrotsoupConfig.Duration = 1500;
            carrotsoupConfig.HealthRegen = 2;
            foodConfigs.Add(carrotsoupConfig);

            //Queens Jam
            var queensjamConfig = new FoodConfig();
            queensjamConfig.FoodPrefabName = "QueensJam";
            queensjamConfig.Health = 30;
            queensjamConfig.Stamina = 40;
            queensjamConfig.Duration = 1200;
            queensjamConfig.HealthRegen = 2;
            foodConfigs.Add(queensjamConfig);

            //Grilled Neck Tail
            var grillednecktailConfig = new FoodConfig();
            grillednecktailConfig.FoodPrefabName = "NeckTailGrilled";
            grillednecktailConfig.Health = 35;
            grillednecktailConfig.Stamina = 20;
            grillednecktailConfig.Duration = 1000;
            grillednecktailConfig.HealthRegen = 2;
            foodConfigs.Add(grillednecktailConfig);

            //Cooked Boar Meat
            var cookedboarmeatConfig = new FoodConfig();
            cookedboarmeatConfig.FoodPrefabName = "CookedMeat";
            cookedboarmeatConfig.Health = 40;
            cookedboarmeatConfig.Stamina = 30;
            cookedboarmeatConfig.Duration = 1200;
            cookedboarmeatConfig.HealthRegen = 2;
            foodConfigs.Add(cookedboarmeatConfig);

            //Cooked Fish
            var cookedfishConfig = new FoodConfig();
            cookedfishConfig.FoodPrefabName = "FishCooked";
            cookedfishConfig.Health = 45;
            cookedfishConfig.Stamina = 25;
            cookedfishConfig.Duration = 1200;
            cookedfishConfig.HealthRegen = 2;
            foodConfigs.Add(cookedfishConfig);

            //Mushroom Yellow
            var mushroomyellowConfig = new FoodConfig();
            mushroomyellowConfig.FoodPrefabName = "MushroomYellow";
            mushroomyellowConfig.Health = 20;
            mushroomyellowConfig.Stamina = 20;
            mushroomyellowConfig.Duration = 600;
            mushroomyellowConfig.HealthRegen = 1;
            foodConfigs.Add(mushroomyellowConfig);

            //Mushroom Blue
            var mushroomBlueConfig = new FoodConfig();
            mushroomBlueConfig.FoodPrefabName = "MushroomBlue";
            mushroomBlueConfig.Health = 20;
            mushroomBlueConfig.Stamina = 20;
            mushroomBlueConfig.Duration = 600;
            mushroomBlueConfig.HealthRegen = 1;
            foodConfigs.Add(mushroomBlueConfig);

            //Mushroom
            var mushroomConfig = new FoodConfig();
            mushroomConfig.FoodPrefabName = "Mushroom";
            mushroomConfig.Health = 15;
            mushroomConfig.Stamina = 25;
            mushroomConfig.Duration = 600;
            mushroomConfig.HealthRegen = 1;
            foodConfigs.Add(mushroomConfig);

            //Honey
            var honeyConfig = new FoodConfig();
            honeyConfig.FoodPrefabName = "Honey";
            honeyConfig.Health = 20;
            honeyConfig.Stamina = 20;
            honeyConfig.Duration = 300;
            honeyConfig.HealthRegen = 5;
            foodConfigs.Add(honeyConfig);

            //Cloudberry
            var cloudberryConfig = new FoodConfig();
            cloudberryConfig.FoodPrefabName = "Cloudberry";
            cloudberryConfig.Health = 15;
            cloudberryConfig.Stamina = 25;
            cloudberryConfig.Duration = 800;
            cloudberryConfig.HealthRegen = 1;
            foodConfigs.Add(cloudberryConfig);

            //Blueberries
            var blueberryConfig = new FoodConfig();
            blueberryConfig.FoodPrefabName = "Blueberries";
            blueberryConfig.Health = 15;
            blueberryConfig.Stamina = 20;
            blueberryConfig.Duration = 600;
            blueberryConfig.HealthRegen = 1;
            foodConfigs.Add(blueberryConfig);

            //Raspberry
            var raspberryConfig = new FoodConfig();
            raspberryConfig.FoodPrefabName = "Raspberry";
            raspberryConfig.Health = 10;
            raspberryConfig.Stamina = 20;
            raspberryConfig.Duration = 600;
            raspberryConfig.HealthRegen = 1;
            foodConfigs.Add(raspberryConfig);

            //Carrot
            var carrotConfig = new FoodConfig();
            carrotConfig.FoodPrefabName = "Carrot";
            carrotConfig.Health = 15;
            carrotConfig.Stamina = 15;
            carrotConfig.Duration = 600;
            carrotConfig.HealthRegen = 1;
            foodConfigs.Add(carrotConfig);

            //Cooked WolfMeat
            var cookedwolfmeatConfig = new FoodConfig();
            cookedwolfmeatConfig.FoodPrefabName = "CookedWolfMeat";
            cookedwolfmeatConfig.Health = 45;
            cookedwolfmeatConfig.Stamina = 35;
            cookedwolfmeatConfig.Duration = 1200;
            cookedwolfmeatConfig.HealthRegen = 3;
            foodConfigs.Add(cookedwolfmeatConfig);

            //Cooked DeerMeat
            var cookeddeermeatConfig = new FoodConfig();
            cookeddeermeatConfig.FoodPrefabName = "CookedDeerMeat";
            cookeddeermeatConfig.Health = 40;
            cookeddeermeatConfig.Stamina = 30;
            cookeddeermeatConfig.Duration = 1200;
            cookeddeermeatConfig.HealthRegen = 2;
            foodConfigs.Add(cookeddeermeatConfig);

            //Black Soup
            var blacksoupConfig = new FoodConfig();
            blacksoupConfig.FoodPrefabName = "BlackSoup";
            blacksoupConfig.Health = 50;
            blacksoupConfig.Stamina = 50;
            blacksoupConfig.Duration = 1200;
            blacksoupConfig.HealthRegen = 3;
            foodConfigs.Add(blacksoupConfig);

            //Deer Stew
            var deerstewConfig = new FoodConfig();
            deerstewConfig.FoodPrefabName = "DeerStew";
            deerstewConfig.Health = 60;
            deerstewConfig.Stamina = 45;
            deerstewConfig.Duration = 1500;
            deerstewConfig.HealthRegen = 3;
            foodConfigs.Add(deerstewConfig);

            //Eyescream
            var eyescreamConfig = new FoodConfig();
            eyescreamConfig.FoodPrefabName = "Eyescream";
            eyescreamConfig.Health = 40;
            eyescreamConfig.Stamina = 65;
            eyescreamConfig.Duration = 1500;
            eyescreamConfig.HealthRegen = 1;
            foodConfigs.Add(eyescreamConfig);

            //MinceMeatSauce
            var mincemeatsauceConfig = new FoodConfig();
            mincemeatsauceConfig.FoodPrefabName = "MinceMeatSauce";
            mincemeatsauceConfig.Health = 45;
            mincemeatsauceConfig.Stamina = 30;
            mincemeatsauceConfig.Duration = 1500;
            mincemeatsauceConfig.HealthRegen = 3;
            foodConfigs.Add(mincemeatsauceConfig);

            //Onion Soup
            var onionsoupConfig = new FoodConfig();
            onionsoupConfig.FoodPrefabName = "OnionSoup";
            onionsoupConfig.Health = 40;
            onionsoupConfig.Stamina = 60;
            onionsoupConfig.Duration = 1500;
            onionsoupConfig.HealthRegen = 1;
            foodConfigs.Add(onionsoupConfig);

            //Muck Shake
            var muchshakeConfig = new FoodConfig();
            muchshakeConfig.FoodPrefabName = "ShocklateSmoothie";
            muchshakeConfig.Health = 40;
            muchshakeConfig.Stamina = 50;
            muchshakeConfig.Duration = 1200;
            muchshakeConfig.HealthRegen = 1;
            foodConfigs.Add(muchshakeConfig);

            //Wolf Meat Skewer
            var wolfmeatskewerConfig = new FoodConfig();
            wolfmeatskewerConfig.FoodPrefabName = "WolfMeatSkewer";
            wolfmeatskewerConfig.Health = 65;
            wolfmeatskewerConfig.Stamina = 40;
            wolfmeatskewerConfig.Duration = 1500;
            wolfmeatskewerConfig.HealthRegen = 3;
            foodConfigs.Add(wolfmeatskewerConfig);

            //Wolf Jerky
            var wolfjerkyConfig = new FoodConfig();
            wolfjerkyConfig.FoodPrefabName = "WolfJerky";
            wolfjerkyConfig.Health = 35;
            wolfjerkyConfig.Stamina = 35;
            wolfjerkyConfig.Duration = 1800;
            wolfjerkyConfig.HealthRegen = 1;
            foodConfigs.Add(wolfjerkyConfig);

            //Boar Jerky
            var boarjerkyConfig = new FoodConfig();
            boarjerkyConfig.FoodPrefabName = "BoarJerky";
            boarjerkyConfig.Health = 25;
            boarjerkyConfig.Stamina = 25;
            boarjerkyConfig.Duration = 1800;
            boarjerkyConfig.HealthRegen = 1;
            foodConfigs.Add(boarjerkyConfig);


            var jsonText = JsonMapper.ToJson(foodConfigs);
            File.WriteAllText(configPath, jsonText);

        }

        private void GenerateConfigFile()
        {

            var foodConfigs = new List<FoodConfig>();

            //Serpent Stew
            var serpentstewConfig = new FoodConfig();
            var serpentstewPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("SerpentStew");
            serpentstewConfig.FoodPrefabName = "SerpentStew";
            serpentstewConfig.Health = (int)serpentstewPrefab.m_itemData.m_shared.m_food;
            serpentstewConfig.Stamina = (int)serpentstewPrefab.m_itemData.m_shared.m_foodStamina;
            serpentstewConfig.Duration = (int)serpentstewPrefab.m_itemData.m_shared.m_foodBurnTime;
            serpentstewConfig.HealthRegen = (int)serpentstewPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(serpentstewConfig);

            //Lox Pie
            var loxpieConfig = new FoodConfig();
            var loxpiePrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("LoxPie");
            loxpieConfig.FoodPrefabName = "LoxPie";
            loxpieConfig.Health = (int)loxpiePrefab.m_itemData.m_shared.m_food;
            loxpieConfig.Stamina = (int)loxpiePrefab.m_itemData.m_shared.m_foodStamina;
            loxpieConfig.Duration = (int)loxpiePrefab.m_itemData.m_shared.m_foodBurnTime;
            loxpieConfig.HealthRegen = (int)loxpiePrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(loxpieConfig);

            //Fish Wrap
            var fishwrapConfig = new FoodConfig();
            var fishwrapPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("FishWraps");
            fishwrapConfig.FoodPrefabName = "FishWraps";
            fishwrapConfig.Health = (int)fishwrapPrefab.m_itemData.m_shared.m_food;
            fishwrapConfig.Stamina = (int)fishwrapPrefab.m_itemData.m_shared.m_foodStamina;
            fishwrapConfig.Duration = (int)fishwrapPrefab.m_itemData.m_shared.m_foodBurnTime;
            fishwrapConfig.HealthRegen = (int)fishwrapPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(fishwrapConfig);

            //Blood Pudding
            var bloodpuddingConfig = new FoodConfig();
            var bloodpuddingPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("BloodPudding");
            bloodpuddingConfig.FoodPrefabName = "BloodPudding";
            bloodpuddingConfig.Health = (int)bloodpuddingPrefab.m_itemData.m_shared.m_food;
            bloodpuddingConfig.Stamina = (int)bloodpuddingPrefab.m_itemData.m_shared.m_foodStamina;
            bloodpuddingConfig.Duration = (int)bloodpuddingPrefab.m_itemData.m_shared.m_foodBurnTime;
            bloodpuddingConfig.HealthRegen = (int)bloodpuddingPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(bloodpuddingConfig);

            //Cooked SerpentMeat
            var cookedserpentmeatConfig = new FoodConfig();
            var cookedserpentmeatPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("SerpentMeatCooked");
            cookedserpentmeatConfig.FoodPrefabName = "SerpentMeatCooked";
            cookedserpentmeatConfig.Health = (int)cookedserpentmeatPrefab.m_itemData.m_shared.m_food;
            cookedserpentmeatConfig.Stamina = (int)cookedserpentmeatPrefab.m_itemData.m_shared.m_foodStamina;
            cookedserpentmeatConfig.Duration = (int)cookedserpentmeatPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookedserpentmeatConfig.HealthRegen = (int)cookedserpentmeatPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookedserpentmeatConfig);

            //Cooked LoxMeat
            var cookedloxmeatConfig = new FoodConfig();
            var cookedloxmeatPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("CookedLoxMeat");
            cookedloxmeatConfig.FoodPrefabName = "CookedLoxMeat";
            cookedloxmeatConfig.Health = (int)cookedloxmeatPrefab.m_itemData.m_shared.m_food;
            cookedloxmeatConfig.Stamina = (int)cookedloxmeatPrefab.m_itemData.m_shared.m_foodStamina;
            cookedloxmeatConfig.Duration = (int)cookedloxmeatPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookedloxmeatConfig.HealthRegen = (int)cookedloxmeatPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookedloxmeatConfig);

            //Bread
            var breadConfig = new FoodConfig();
            var breadPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Bread");
            breadConfig.FoodPrefabName = "Bread";
            breadConfig.Health = (int)breadPrefab.m_itemData.m_shared.m_food;
            breadConfig.Stamina = (int)breadPrefab.m_itemData.m_shared.m_foodStamina;
            breadConfig.Duration = (int)breadPrefab.m_itemData.m_shared.m_foodBurnTime;
            breadConfig.HealthRegen = (int)breadPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(breadConfig);

            //Turnip Stew
            var turnipstewConfig = new FoodConfig();
            var turnipstewPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("TurnipStew");
            turnipstewConfig.FoodPrefabName = "TurnipStew";
            turnipstewConfig.Health = (int)turnipstewPrefab.m_itemData.m_shared.m_food;
            turnipstewConfig.Stamina = (int)turnipstewPrefab.m_itemData.m_shared.m_foodStamina;
            turnipstewConfig.Duration = (int)turnipstewPrefab.m_itemData.m_shared.m_foodBurnTime;
            turnipstewConfig.HealthRegen = (int)turnipstewPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(turnipstewConfig);

            //Sausages
            var sausagesConfig = new FoodConfig();
            var sausagesPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Sausages");
            sausagesConfig.FoodPrefabName = "Sausages";
            sausagesConfig.Health = (int)sausagesPrefab.m_itemData.m_shared.m_food;
            sausagesConfig.Stamina = (int)sausagesPrefab.m_itemData.m_shared.m_foodStamina;
            sausagesConfig.Duration = (int)sausagesPrefab.m_itemData.m_shared.m_foodBurnTime;
            sausagesConfig.HealthRegen = (int)sausagesPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(sausagesConfig);

            //Carrot Soup
            var carrotsoupConfig = new FoodConfig();
            var carrotsoupPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("CarrotSoup");
            carrotsoupConfig.FoodPrefabName = "CarrotSoup";
            carrotsoupConfig.Health = (int)carrotsoupPrefab.m_itemData.m_shared.m_food;
            carrotsoupConfig.Stamina = (int)carrotsoupPrefab.m_itemData.m_shared.m_foodStamina;
            carrotsoupConfig.Duration = (int)carrotsoupPrefab.m_itemData.m_shared.m_foodBurnTime;
            carrotsoupConfig.HealthRegen = (int)carrotsoupPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(carrotsoupConfig);

            //Queens Jam
            var queensjamConfig = new FoodConfig();
            var queensjamPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("QueensJam");
            queensjamConfig.FoodPrefabName = "QueensJam";
            queensjamConfig.Health = (int)queensjamPrefab.m_itemData.m_shared.m_food;
            queensjamConfig.Stamina = (int)queensjamPrefab.m_itemData.m_shared.m_foodStamina;
            queensjamConfig.Duration = (int)queensjamPrefab.m_itemData.m_shared.m_foodBurnTime;
            queensjamConfig.HealthRegen = (int)queensjamPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(queensjamConfig);

            //Grilled Neck Tail
            var grillednecktailConfig = new FoodConfig();
            var grillednecktailPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("NeckTailGrilled");
            grillednecktailConfig.FoodPrefabName = "NeckTailGrilled";
            grillednecktailConfig.Health = (int)grillednecktailPrefab.m_itemData.m_shared.m_food;
            grillednecktailConfig.Stamina = (int)grillednecktailPrefab.m_itemData.m_shared.m_foodStamina;
            grillednecktailConfig.Duration = (int)grillednecktailPrefab.m_itemData.m_shared.m_foodBurnTime;
            grillednecktailConfig.HealthRegen = (int)grillednecktailPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(grillednecktailConfig);

            //Cooked Boar Meat
            var cookedboarmeatConfig = new FoodConfig();
            var cookedboarmeatPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("CookedMeat");
            cookedboarmeatConfig.FoodPrefabName = "CookedMeat";
            cookedboarmeatConfig.Health = (int)cookedboarmeatPrefab.m_itemData.m_shared.m_food;
            cookedboarmeatConfig.Stamina = (int)cookedboarmeatPrefab.m_itemData.m_shared.m_foodStamina;
            cookedboarmeatConfig.Duration = (int)cookedboarmeatPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookedboarmeatConfig.HealthRegen = (int)cookedboarmeatPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookedboarmeatConfig);

            //Cooked Fish
            var cookedfishConfig = new FoodConfig();
            var cookedfishPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("FishCooked");
            cookedfishConfig.FoodPrefabName = "FishCooked";
            cookedfishConfig.Health = (int)cookedfishPrefab.m_itemData.m_shared.m_food;
            cookedfishConfig.Stamina = (int)cookedfishPrefab.m_itemData.m_shared.m_foodStamina;
            cookedfishConfig.Duration = (int)cookedfishPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookedfishConfig.HealthRegen = (int)cookedfishPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookedfishConfig);

            //Mushroom Yellow
            var mushroomyellowConfig = new FoodConfig();
            var mushroomyellowPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("MushroomYellow");
            mushroomyellowConfig.FoodPrefabName = "MushroomYellow";
            mushroomyellowConfig.Health = (int)mushroomyellowPrefab.m_itemData.m_shared.m_food;
            mushroomyellowConfig.Stamina = (int)mushroomyellowPrefab.m_itemData.m_shared.m_foodStamina;
            mushroomyellowConfig.Duration = (int)mushroomyellowPrefab.m_itemData.m_shared.m_foodBurnTime;
            mushroomyellowConfig.HealthRegen = (int)mushroomyellowPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(mushroomyellowConfig);

            //Mushroom Blue
            var mushroomBlueConfig = new FoodConfig();
            var mushroomBluePrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("MushroomBlue");
            mushroomBlueConfig.FoodPrefabName = "MushroomBlue";
            mushroomBlueConfig.Health = (int)mushroomBluePrefab.m_itemData.m_shared.m_food;
            mushroomBlueConfig.Stamina = (int)mushroomBluePrefab.m_itemData.m_shared.m_foodStamina;
            mushroomBlueConfig.Duration = (int)mushroomBluePrefab.m_itemData.m_shared.m_foodBurnTime;
            mushroomBlueConfig.HealthRegen = (int)mushroomBluePrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(mushroomBlueConfig);

            //Mushroom
            var mushroomConfig = new FoodConfig();
            var mushroomPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Mushroom");
            mushroomConfig.FoodPrefabName = "Mushroom";
            mushroomConfig.Health = (int)mushroomPrefab.m_itemData.m_shared.m_food;
            mushroomConfig.Stamina = (int)mushroomPrefab.m_itemData.m_shared.m_foodStamina;
            mushroomConfig.Duration = (int)mushroomPrefab.m_itemData.m_shared.m_foodBurnTime;
            mushroomConfig.HealthRegen = (int)mushroomPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(mushroomConfig);

            //Honey
            var honeyConfig = new FoodConfig();
            var honeyPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Honey");
            honeyConfig.FoodPrefabName = "Honey";
            honeyConfig.Health = (int)honeyPrefab.m_itemData.m_shared.m_food;
            honeyConfig.Stamina = (int)honeyPrefab.m_itemData.m_shared.m_foodStamina;
            honeyConfig.Duration = (int)honeyPrefab.m_itemData.m_shared.m_foodBurnTime;
            honeyConfig.HealthRegen = (int)honeyPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(honeyConfig);

            //Cloudberry
            var cloudberryConfig = new FoodConfig();
            var cloudberryPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Cloudberry");
            cloudberryConfig.FoodPrefabName = "Cloudberry";
            cloudberryConfig.Health = (int)cloudberryPrefab.m_itemData.m_shared.m_food;
            cloudberryConfig.Stamina = (int)cloudberryPrefab.m_itemData.m_shared.m_foodStamina;
            cloudberryConfig.Duration = (int)cloudberryPrefab.m_itemData.m_shared.m_foodBurnTime;
            cloudberryConfig.HealthRegen = (int)cloudberryPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cloudberryConfig);

            //Blueberries
            var blueberryConfig = new FoodConfig();
            var blueberryPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Blueberries");
            blueberryConfig.FoodPrefabName = "Blueberries";
            blueberryConfig.Health = (int)blueberryPrefab.m_itemData.m_shared.m_food;
            blueberryConfig.Stamina = (int)blueberryPrefab.m_itemData.m_shared.m_foodStamina;
            blueberryConfig.Duration = (int)blueberryPrefab.m_itemData.m_shared.m_foodBurnTime;
            blueberryConfig.HealthRegen = (int)blueberryPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(blueberryConfig);

            //Raspberry
            var raspberryConfig = new FoodConfig();
            var raspberryPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Raspberry");
            raspberryConfig.FoodPrefabName = "Raspberry";
            raspberryConfig.Health = (int)raspberryPrefab.m_itemData.m_shared.m_food;
            raspberryConfig.Stamina = (int)raspberryPrefab.m_itemData.m_shared.m_foodStamina;
            raspberryConfig.Duration = (int)raspberryPrefab.m_itemData.m_shared.m_foodBurnTime;
            raspberryConfig.HealthRegen = (int)raspberryPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(raspberryConfig);

            //Carrot
            var carrotConfig = new FoodConfig();
            var carrotPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Carrot");
            carrotConfig.FoodPrefabName = "Carrot";
            carrotConfig.Health = (int)carrotPrefab.m_itemData.m_shared.m_food;
            carrotConfig.Stamina = (int)carrotPrefab.m_itemData.m_shared.m_foodStamina;
            carrotConfig.Duration = (int)carrotPrefab.m_itemData.m_shared.m_foodBurnTime;
            carrotConfig.HealthRegen = (int)carrotPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(carrotConfig);

            //Cooked WolfMeat
            var cookedwolfmeatConfig = new FoodConfig();
            var cookedwolfmeatPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("CookedWolfMeat");
            cookedwolfmeatConfig.FoodPrefabName = "CookedWolfMeat";
            cookedwolfmeatConfig.Health = (int)cookedwolfmeatPrefab.m_itemData.m_shared.m_food;
            cookedwolfmeatConfig.Stamina = (int)cookedwolfmeatPrefab.m_itemData.m_shared.m_foodStamina;
            cookedwolfmeatConfig.Duration = (int)cookedwolfmeatPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookedwolfmeatConfig.HealthRegen = (int)cookedwolfmeatPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookedwolfmeatConfig);

            //Cooked DeerMeat
            var cookeddeermeatConfig = new FoodConfig();
            var cookeddeermeatPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("CookedDeerMeat");
            cookeddeermeatConfig.FoodPrefabName = "CookedDeerMeat";
            cookeddeermeatConfig.Health = (int)cookeddeermeatPrefab.m_itemData.m_shared.m_food;
            cookeddeermeatConfig.Stamina = (int)cookeddeermeatPrefab.m_itemData.m_shared.m_foodStamina;
            cookeddeermeatConfig.Duration = (int)cookeddeermeatPrefab.m_itemData.m_shared.m_foodBurnTime;
            cookeddeermeatConfig.HealthRegen = (int)cookeddeermeatPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(cookeddeermeatConfig);

            //Black Soup
            var blacksoupConfig = new FoodConfig();
            var blacksoupPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("BlackSoup");
            blacksoupConfig.FoodPrefabName = "BlackSoup";
            blacksoupConfig.Health = (int)blacksoupPrefab.m_itemData.m_shared.m_food;
            blacksoupConfig.Stamina = (int)blacksoupPrefab.m_itemData.m_shared.m_foodStamina;
            blacksoupConfig.Duration = (int)blacksoupPrefab.m_itemData.m_shared.m_foodBurnTime;
            blacksoupConfig.HealthRegen = (int)blacksoupPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(blacksoupConfig);

            //Deer Stew
            var deerstewConfig = new FoodConfig();
            var deerstewPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("DeerStew");
            deerstewConfig.FoodPrefabName = "DeerStew";
            deerstewConfig.Health = (int)deerstewPrefab.m_itemData.m_shared.m_food;
            deerstewConfig.Stamina = (int)deerstewPrefab.m_itemData.m_shared.m_foodStamina;
            deerstewConfig.Duration = (int)deerstewPrefab.m_itemData.m_shared.m_foodBurnTime;
            deerstewConfig.HealthRegen = (int)deerstewPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(deerstewConfig);

            //Eyescream
            var eyescreamConfig = new FoodConfig();
            var eyescreamPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("Eyescream");
            eyescreamConfig.FoodPrefabName = "Eyescream";
            eyescreamConfig.Health = (int)eyescreamPrefab.m_itemData.m_shared.m_food;
            eyescreamConfig.Stamina = (int)eyescreamPrefab.m_itemData.m_shared.m_foodStamina;
            eyescreamConfig.Duration = (int)eyescreamPrefab.m_itemData.m_shared.m_foodBurnTime;
            eyescreamConfig.HealthRegen = (int)eyescreamPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(eyescreamConfig);

            //MinceMeatSauce
            var mincemeatsauceConfig = new FoodConfig();
            var mincemeatsaucePrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("MinceMeatSauce");
            mincemeatsauceConfig.FoodPrefabName = "MinceMeatSauce";
            mincemeatsauceConfig.Health = (int)mincemeatsaucePrefab.m_itemData.m_shared.m_food;
            mincemeatsauceConfig.Stamina = (int)mincemeatsaucePrefab.m_itemData.m_shared.m_foodStamina;
            mincemeatsauceConfig.Duration = (int)mincemeatsaucePrefab.m_itemData.m_shared.m_foodBurnTime;
            mincemeatsauceConfig.HealthRegen = (int)mincemeatsaucePrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(mincemeatsauceConfig);

            //Onion Soup
            var onionsoupConfig = new FoodConfig();
            var onionsoupPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("OnionSoup");
            onionsoupConfig.FoodPrefabName = "OnionSoup";
            onionsoupConfig.Health = (int)onionsoupPrefab.m_itemData.m_shared.m_food;
            onionsoupConfig.Stamina = (int)onionsoupPrefab.m_itemData.m_shared.m_foodStamina;
            onionsoupConfig.Duration = (int)onionsoupPrefab.m_itemData.m_shared.m_foodBurnTime;
            onionsoupConfig.HealthRegen = (int)onionsoupPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(onionsoupConfig);

            //Muck Shake
            var muchshakeConfig = new FoodConfig();
            var muchshakePrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("ShocklateSmoothie");
            muchshakeConfig.FoodPrefabName = "ShocklateSmoothie";
            muchshakeConfig.Health = (int)muchshakePrefab.m_itemData.m_shared.m_food;
            muchshakeConfig.Stamina = (int)muchshakePrefab.m_itemData.m_shared.m_foodStamina;
            muchshakeConfig.Duration = (int)muchshakePrefab.m_itemData.m_shared.m_foodBurnTime;
            muchshakeConfig.HealthRegen = (int)muchshakePrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(muchshakeConfig);

            //Wolf Meat Skewer
            var wolfmeatskewerConfig = new FoodConfig();
            var wolfmeatskewerPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("WolfMeatSkewer");
            wolfmeatskewerConfig.FoodPrefabName = "WolfMeatSkewer";
            wolfmeatskewerConfig.Health = (int)wolfmeatskewerPrefab.m_itemData.m_shared.m_food;
            wolfmeatskewerConfig.Stamina = (int)wolfmeatskewerPrefab.m_itemData.m_shared.m_foodStamina;
            wolfmeatskewerConfig.Duration = (int)wolfmeatskewerPrefab.m_itemData.m_shared.m_foodBurnTime;
            wolfmeatskewerConfig.HealthRegen = (int)wolfmeatskewerPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(wolfmeatskewerConfig);

            //Wolf Jerky
            var wolfjerkyConfig = new FoodConfig();
            var wolfjerkyPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("WolfJerky");
            wolfjerkyConfig.FoodPrefabName = "WolfJerky";
            wolfjerkyConfig.Health = (int)wolfjerkyPrefab.m_itemData.m_shared.m_food;
            wolfjerkyConfig.Stamina = (int)wolfjerkyPrefab.m_itemData.m_shared.m_foodStamina;
            wolfjerkyConfig.Duration = (int)wolfjerkyPrefab.m_itemData.m_shared.m_foodBurnTime;
            wolfjerkyConfig.HealthRegen = (int)wolfjerkyPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(wolfjerkyConfig);

            //Boar Jerky
            var boarjerkyConfig = new FoodConfig();
            var boarjerkyPrefab = PrefabManager.Cache.GetPrefab<ItemDrop>("BoarJerky");
            boarjerkyConfig.FoodPrefabName = "BoarJerky";
            boarjerkyConfig.Health = (int)boarjerkyPrefab.m_itemData.m_shared.m_food;
            boarjerkyConfig.Stamina = (int)boarjerkyPrefab.m_itemData.m_shared.m_foodStamina;
            boarjerkyConfig.Duration = (int)boarjerkyPrefab.m_itemData.m_shared.m_foodBurnTime;
            boarjerkyConfig.HealthRegen = (int)boarjerkyPrefab.m_itemData.m_shared.m_foodRegen;
            foodConfigs.Add(boarjerkyConfig);


            var jsonText = JsonMapper.ToJson(foodConfigs);
            File.WriteAllText(configPath, jsonText);

        }

        private void RegisterConfigValues()
        {
            var foodConfigs = GetJson();

            foreach (var config in foodConfigs)
            {
                try
                {
                    PrefabManager.Cache.GetPrefab<ItemDrop>(config.FoodPrefabName).m_itemData.m_shared.m_food = config.Health;
                    PrefabManager.Cache.GetPrefab<ItemDrop>(config.FoodPrefabName).m_itemData.m_shared.m_foodStamina = config.Stamina;
                    PrefabManager.Cache.GetPrefab<ItemDrop>(config.FoodPrefabName).m_itemData.m_shared.m_foodBurnTime = config.Duration;
                    PrefabManager.Cache.GetPrefab<ItemDrop>(config.FoodPrefabName).m_itemData.m_shared.m_foodRegen = config.HealthRegen;
                }

                catch (Exception e)
                {
                    Jotunn.Logger.LogError($"Loading config for {config.FoodPrefabName} failed. {e.Message} {e.StackTrace}");
                }
            }
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
                GenerateConfigFile();

                Jotunn.Logger.LogInfo("Updated configs");
            }
        }

        internal static List<FoodConfig> GetJson()
        {
            Jotunn.Logger.LogDebug($"Attempting to load config file from path {configPath}");
            var jsonText = AssetUtils.LoadText(configPath);
            Jotunn.Logger.LogDebug("File found. Attempting to deserialize...");
            var foodconfigs = JsonMapper.ToObject<List<FoodConfig>>(jsonText);
            return foodconfigs;
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

    [Serializable]
    public class FoodConfig
    {
        public string FoodPrefabName { get; set; }
        public int Health { get; set; }
        public int Stamina { get; set; }
        public int Duration { get; set; }
        public int HealthRegen { get; set; }
    }
}
