using System.Collections;
using Wully.MoreModes;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class RemoveSpawnedHealthPotions : LevelModule {
		private int lastActiveItemCount;

		public override IEnumerator OnLoadCoroutine() {

			Utilities.RemoveActiveHealthPotions();
			

			yield break;
		}


		public override void Update() {
			base.Update();
			//run every 60 frames
			if (Time.frameCount % 60 == 0) {
				//If the amount of active items has increased or stayed the same, try to remove all potions
				if (Item.allActive.Count >= lastActiveItemCount) {
					
					Utilities.RemoveActiveHealthPotions();
					
				}

				lastActiveItemCount = Item.allActive.Count;
			}
		}
	}
}