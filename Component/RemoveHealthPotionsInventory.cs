using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class RemoveHealthPotionsInventory : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			yield return new WaitForSecondsRealtime(1);
			if (Level.current.GetOptionAsBool("disable_healthpotions_inventory", true)) {
				Utilities.RemoveHealthPotionsFromPlayerInventory();
			}

			yield return base.OnLoadCoroutine();
		}
	}
}