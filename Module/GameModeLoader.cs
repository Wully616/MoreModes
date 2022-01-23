using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModeLoader.Data;
using HarmonyLib;
using ThunderRoad;
using UnityEngine;
using Extensions = Wully.Utils.Extensions;

namespace GameModeLoader.Module {
	public class GameModeLoader : LevelModule {
		public static GameModeLoader local;
		public static Harmony Patch;

		private void Setup() {
			try {
				if (local == null) {
					local = this;
					Debug.Log("Manifest: " + Extensions.GetManifest(typeof(GameModeLoader)));
					Debug.Log("Enabling GameModeLoader");
					Patch = new Harmony("Wully.GameModeLoader");
					Patch.PatchAll();
				}
			}
			catch (Exception exception) {
				Debug.Log($"Error patching Wully.GameModeLoader: {exception}");
			}
		}

		public override IEnumerator OnLoadCoroutine() {
			Setup();
			EventManager.onLevelLoad += EventManager_onLevelLoad;
			//In the master level, update the catalog LevelData's to add the custom gamemodes
			AddModesToMaps();

			yield return base.OnLoadCoroutine();
		}

		private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime) {
			if (eventTime == EventTime.OnEnd) {
				string modules =
					$"Level loaded. Map: {Level.current.data.id}. Mode: {Level.current.mode.name}\nModules:";
				foreach (LevelModule levelModule in Level.current.mode.modules) {
					modules += $"{levelModule.type}, ";
				}

				Debug.Log(modules);
			}
		}


		private bool DoesOptionExist(List<LevelData.Option> options, string optionName) {
			if (options != null) {
				foreach (var option in options) {
					if (option.name.ToLower() == optionName.ToLower()) {
						return true;
					}
				}
			}

			return false;
		}

		private bool DoesModuleExist(List<LevelModule> modules, LevelModule searchModule) {
			if (modules != null) {
				foreach (var module in modules) {
					if (module.GetType() == searchModule.GetType()) {
						return true;
					}
				}
			}

			return false;
		}

		private LevelDataModeCatalog AddOptionToGameModeByOptionId(LevelDataModeCatalog gameMode,
			LevelOptionCatalog option) {
			//gameModes can specify reusable gameOptions to use via Ids, add the provided one if it matches
			if (gameMode.gameModeOptionIds != null &&
			    gameMode.gameModeOptionIds.Contains(option.id, StringComparer.OrdinalIgnoreCase)) {
				return AddOptionToGameMode(gameMode, option);
			}

			return gameMode;
		}

		private LevelDataModeCatalog AddOptionToGameMode(LevelDataModeCatalog gameMode, LevelOptionCatalog option) {
			//Dont add the option to the gamemode if the option excludes that gamemode
			if (option.excludeGameModeNames != null &&
			    option.excludeGameModeNames.Contains(gameMode.mode.name, StringComparer.OrdinalIgnoreCase)) {
				return gameMode;
			}

			//add the option to the gameMode, but not if it already has it
			if (!DoesOptionExist(gameMode.mode.availableOptions, option.levelOption.name)) {
				gameMode.mode.availableOptions.Add(option.levelOption);
			}

			//Add the level module for this option to the gamemode, but not if it already has it
			if (!DoesModuleExist(gameMode.mode.modules, option.levelOption.levelModule)) {
				gameMode.mode.modules.Add(option.levelOption.levelModule);
			}

			return gameMode;
		}

		private LevelDataModeCatalog ProcessGameModeOption(LevelDataModeCatalog gameMode, LevelOptionCatalog option) {
			AddOptionToGameModeByOptionId(gameMode, option);
			if (gameMode.explicitOptions) {
				//the gamemode only uses the options specified in its config, nothing else to do
				return gameMode;
			}

			AddOptionToGameMode(gameMode, option);


			return gameMode;
		}


		private void AddModesToMaps() {
			//Get the gamemodes from the catalog
			var gameModes = GetLevelDataModeList();
			var options = GetLevelOptionList();

			List<LevelData> levelList = GetLevelDataList();

			//Add customOptions to our custom gamemodes
			foreach (var gameMode in gameModes) {
				foreach (LevelOptionCatalog option in options) {
					ProcessGameModeOption(gameMode, option);
				}
			}

			//Add customOptions to the base games gamemodes
			foreach (LevelData levelData in levelList) {
				if (levelData.id.ToLower() != "master" &&
				    levelData.id.ToLower() != "characterselection" &&
				    //levelData.id.ToLower() != "dungeon" &&
				    levelData.id.ToLower() != "home") {
					//Add all options to the basegame gamemodes
					AddOptionsToLevelDataModes(levelData, options);

					//Finally add the custom gamemodes to the maps, unless the gamemode excludes that map
					foreach (var gameMode in gameModes) {
						if (gameMode.excludeLevelIds != null &&
						    !gameMode.excludeLevelIds.Contains(levelData.id, StringComparer.OrdinalIgnoreCase)) {
							levelData.modes.Add(gameMode.mode);
						}
					}
				}
			}
		}

		private void AddOptionsToLevelDataModes(LevelData levelData, List<LevelOptionCatalog> options) {
			foreach (var mode in levelData.modes) {
				foreach (LevelOptionCatalog option in options) {
					//options can exclude specific maps or gamemodes
					if ((option.excludeLevelIds == null ||
					     !option.excludeLevelIds.Contains(levelData.id, StringComparer.OrdinalIgnoreCase)
					    ) &&
					    (option.excludeGameModeNames == null ||
					     !option.excludeGameModeNames.Contains(mode.name))) {
						//add if it doesnt exist
						if (!DoesOptionExist(mode.availableOptions, option.levelOption.name)) {
							mode.availableOptions.Add(option.levelOption);
						}

						//Add the level module for this option to the gamemode, but not if it already has it
						if (!DoesModuleExist(mode.modules, option.levelOption.levelModule)) {
							mode.modules.Add(option.levelOption.levelModule);
						}
					}
				}
			}
		}

		public static List<LevelData> GetLevelDataList() {
			return (
				from item in Catalog.GetDataList(Catalog.Category.Level)
				select (LevelData) item).ToList();
		}

		public static List<LevelDataModeCatalog> GetLevelDataModeList() {
			return (
				from interactable in Catalog.GetDataList(Catalog.Category.Interactable)
				where interactable is LevelDataModeCatalog
				select (LevelDataModeCatalog) interactable).ToList();
		}

		public static List<LevelOptionCatalog> GetLevelOptionList() {
			return (
				from interactable in Catalog.GetDataList(Catalog.Category.Interactable)
				where interactable is LevelOptionCatalog
				select (LevelOptionCatalog) interactable).ToList();
		}
	}
}