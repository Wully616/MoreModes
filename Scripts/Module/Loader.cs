using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;
using Wully.MoreModes;
using Extensions = Wully.Utils.Extensions;

namespace Wully.MoreModes
{
    
    [HarmonyPatch(typeof(UISelectionListButtons), "Awake")]
    class Patch
    {
        static void Postfix(UISelectionListButtons __instance)
        {
            var title = __instance.transform.Find("Title").GetComponent<Text>();
            title.resizeTextForBestFit = true;
        }
    }
    
    public class Loader : CustomData
    {
        public static Loader local;

        public override void OnCatalogRefresh()
        {
            //Only want one instance of the loader running
            if (local != null) return;
            local = this;
            var harmony = new Harmony("wully");
            harmony.PatchAll();
            Debug.Log($"MoreModes Loader!");
            //update the catalog LevelData's to add the custom gamemodes
            AddModesToMaps();
        }
        
        private void AddModesToMaps()
        {
            //Get the gamemodes from the catalog
            var gameModes = Catalog.GetDataList<GameModeData>();
  
            List<LevelData> levelList = Catalog.GetDataList<LevelData>();
            
            //Add customOptions to the base games gamemodes
            foreach (LevelData levelData in levelList)
            {
                if (levelData.id.ToLower() != "master"
                    && levelData.id.ToLower() != "characterselection"
                    && levelData.id.ToLower() != "home")
                {

                    //Finally add the custom gamemodes to the maps
                    foreach (var gameMode in gameModes)
                    {
                        //Add if its specifically been told to add it
                        if (gameMode.onlyLevelids?.Contains(levelData.id, StringComparer.OrdinalIgnoreCase) == true)
                        {
                            if (levelData.modes == null) levelData.modes = new List<LevelData.Mode>();
                            levelData.modes.Add(gameMode.mode);
                        }
                        //otherwise only add it if its not been excluded
                        else if (gameMode.excludeLevelIds?.Contains(levelData.id, StringComparer.OrdinalIgnoreCase) == false)
                        {
                            if (levelData.modes == null) levelData.modes = new List<LevelData.Mode>();
                            levelData.modes.Add(gameMode.mode);
                        }
                    }
                }
            }
        }
        
    }
}
