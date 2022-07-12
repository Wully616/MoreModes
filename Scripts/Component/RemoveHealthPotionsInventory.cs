using System.Collections;
using Wully.MoreModes.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class RemoveHealthPotionsInventory : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			yield return new WaitForSecondsRealtime(1);
			if (IsEnabled()) {
				Utilities.RemoveHealthPotionsFromPlayerInventory();
			}

			yield break;
		}
	}
}