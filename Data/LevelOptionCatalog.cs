using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameModeLoader.Module;
using ThunderRoad;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace GameModeLoader.Data {
	[Serializable]
	public class LevelOptionCatalog : InteractableData {
		//This class is a dumb wrapper around the Interactable class so it can be loaded into the catalog
		public List<string> excludeLevelIds;
		public List<string> excludeGameModeNames;
		public LevelOption levelOption;
	}
}
