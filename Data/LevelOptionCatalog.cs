using System;
using System.Collections.Generic;
using GameModeLoader.Module;
using ThunderRoad;

namespace GameModeLoader.Data {
	[Serializable]
	public class LevelOptionCatalog : InteractableData {
		public List<string> excludeGameModeNames;

		//This class is a dumb wrapper around the Interactable class so it can be loaded into the catalog
		public List<string> excludeLevelIds;
		public LevelOption levelOption;

		/// <summary>
		/// This will set the json defined option ID onto the levelmodule so it can be flagged on/off
		/// This should be called by the GameModeLoader master levelmodule
		/// </summary>
		public void SetIdOnLevelModule() {
			levelOption.levelModuleOptional.id = this.id;
		}
	}
}