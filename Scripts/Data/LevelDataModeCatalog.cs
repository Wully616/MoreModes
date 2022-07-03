using System;
using System.Collections.Generic;
using ThunderRoad;

namespace GameModeLoader.Data {
	[Serializable]
	public class LevelDataModeCatalog : CustomData {
		//This class is a dumb wrapper around the Interactable class so it can be loaded into the catalog
		//This is instead of using LevelData since we dont want a fake level to appear on the map
		public List<string> excludeLevelIds;

		/// <summary>
		///     If explicitOptions is true, the gamemode will only have the options defined in its mode
		///     If it is false, all possible options will be added to this gamemode
		/// </summary>
		public bool explicitOptions = true;

		public List<string> gameModeOptionIds;
		public LevelData.Mode mode;
	}
}