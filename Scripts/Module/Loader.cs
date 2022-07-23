using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Extensions = Wully.Utils.Extensions;

namespace Wully.MoreModes
{
    public class Loader : CustomData
    {
        public static Loader local;

        public override void OnCatalogRefresh()
        {
            //Only want one instance of the loader running
            if (local != null) return;
            local = this;
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
                    &&
                    //levelData.id.ToLower() != "dungeon" &&
                    levelData.id.ToLower() != "home")
                {

                    //Finally add the custom gamemodes to the maps, unless the gamemode excludes that map
                    foreach (var gameMode in gameModes)
                    {
                        if (gameMode.excludeLevelIds != null && !gameMode.excludeLevelIds.Contains(levelData.id, StringComparer.OrdinalIgnoreCase))
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
