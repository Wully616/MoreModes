using System.Collections.Generic;
using ThunderRoad;

namespace Wully.MoreModes {
	/// <summary>
	/// GameModeData defines a LevelData.Mode which will be added to every level except the ones in the exclude list
	/// </summary>
	public class GameModeData : CustomData {
		public List<string> excludeLevelIds;
		public List<string> onlyLevelids;
		public LevelData.Mode mode;
	}
}