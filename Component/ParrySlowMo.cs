using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class ParrySlowMo : LevelModule {
		public int slowMoTime = 1;

		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("parryslowmo", true)) {
				EventManager.onCreatureParry += EventManager_onCreatureParry;
			}

			yield return base.OnLoadCoroutine();
		}

		private void EventManager_onCreatureParry(Creature creature, CollisionInstance collisionInstance) {
			if (Utilities.DidPlayerParry(collisionInstance)) {
				level.StartCoroutine(Utilities.SlowMo(slowMoTime));
			}
		}

		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("parryslowmo", true)) {
				EventManager.onCreatureParry -= EventManager_onCreatureParry;
			}

			base.OnUnload();
		}

	}
}