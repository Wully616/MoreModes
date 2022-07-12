using System;
using System.Collections.Generic;
using ThunderRoad;

namespace Wully.MoreModes.Data {
	[Serializable]
	public class LevelOptionCatalog : CustomData {
		public List<string> excludeGameModeNames;
		
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