using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class KillSlowMo : LevelModule {
		public int slowMoTime = 1;

		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("killslowmo", true)) {
				EventManager.onCreatureKill += EventManager_onCreatureKill;
			}

			yield return base.OnLoadCoroutine();
		}

		private void EventManager_onCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime ) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.sourceColliderGroup )
				return;
			if ( collisionInstance.sourceColliderGroup.collisionHandler.item?.lastHandler?.creature.player ) {
				level.StartCoroutine(Utilities.SlowMo(slowMoTime));
			} else {
				if ( !collisionInstance.sourceColliderGroup.collisionHandler.ragdollPart?.ragdoll.creature.player )
					return;
				level.StartCoroutine(Utilities.SlowMo(slowMoTime));
			}

		}

		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("killslowmo", true)) {
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
			}

			base.OnUnload();
		}

	}
}