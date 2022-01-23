using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class RemoveSpawnedHealthPotions : LevelModule {
		private int lastActiveItemCount;

		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("disable_healthpotions_spawn", true)) {
				Utilities.RemoveActiveHealthPotions();
			}

			return base.OnLoadCoroutine();
		}


		public override void Update() {
			base.Update();
			//run every 60 frames
			if (Time.frameCount % 60 == 0) {
				//If the amount of active items has increased or stayed the same, try to remove all potions
				if (Item.allActive.Count >= lastActiveItemCount) {
					if (Level.current.GetOptionAsBool("disable_healthpotions_spawn", true)) {
						Utilities.RemoveActiveHealthPotions();
					}
				}

				lastActiveItemCount = Item.allActive.Count;
			}
		}
	}
}