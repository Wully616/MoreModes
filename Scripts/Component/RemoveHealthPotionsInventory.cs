using System.Collections;
using Wully.MoreModes;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class RemoveHealthPotionsInventory : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			Utilities.RemoveHealthPotionsFromPlayerInventory();
			yield break;
		}
	}
}