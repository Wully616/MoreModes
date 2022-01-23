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
	}
}