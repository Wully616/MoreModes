using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class ParrySlowMo : LevelModuleOptional {
		public int slowMoTime = 1;

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onCreatureParry += EventManager_onCreatureParry;
			}

			yield break;
		}

		private void EventManager_onCreatureParry(Creature creature, CollisionInstance collisionInstance) {
			if (Utilities.DidPlayerParry(collisionInstance)) {
				level.StartCoroutine(Utilities.SlowMo(slowMoTime));
			}
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				EventManager.onCreatureParry -= EventManager_onCreatureParry;
			}

			base.OnUnload();
		}

	}
}