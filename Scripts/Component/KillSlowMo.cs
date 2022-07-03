using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class KillSlowMo : LevelModuleOptional {
		public int slowMoTime = 1;
        private Coroutine slowMoCoroutine;
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onCreatureKill += EventManager_onCreatureKill;
			}

			yield break;
		}
		private void EventManager_onCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime ) {
            if ( eventTime == EventTime.OnStart ) return;
            if (player)
            {
                StopCoroutine();
				return;
            }

			if (!collisionInstance.IsDoneByPlayer() ) return;
			slowMoCoroutine = level.StartCoroutine(Utilities.SlowMo(slowMoTime));
		}

        private void StopCoroutine()
        {
            if ( slowMoCoroutine != null )
            {
                level.StopCoroutine(slowMoCoroutine);
            }
		}

		public override void OnUnload() {
			if ( IsEnabled() )
            {
                StopCoroutine();
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
			}

			base.OnUnload();
		}

	}
}