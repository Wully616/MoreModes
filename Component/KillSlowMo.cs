using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class KillSlowMo : LevelModuleOptional {
		public int slowMoTime = 1;

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onCreatureKill += EventManager_onCreatureKill;
			}

			yield break;
		}
		private void EventManager_onCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime ) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;
			level.StartCoroutine(Utilities.SlowMo(slowMoTime));
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
			}

			base.OnUnload();
		}

	}
}