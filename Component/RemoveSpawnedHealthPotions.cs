using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using GameModeLoader.Module;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using GameModeLoader.Utils;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class RemoveSpawnedHealthPotions : LevelModule {
		
		private int lastActiveItemCount = 0;
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