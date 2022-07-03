using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class ParrySlowMo : LevelModuleOptional {
		public int slowMoTime = 1;
        private Coroutine slowMoCoroutine;
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onCreatureParry += EventManager_onCreatureParry;
                EventManager.onCreatureKill += EventManager_onCreatureKill;
			}

			yield break;
		}

		private void EventManager_onCreatureParry(Creature creature, CollisionInstance collisionInstance) {
			if (Utilities.DidPlayerParry(collisionInstance)) {
				level.StartCoroutine(Utilities.SlowMo(slowMoTime));
			}
		}
        private void EventManager_onCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
            EventTime eventTime )
        {
            if ( eventTime == EventTime.OnStart ) return;
            if ( player )
            {
                StopCoroutine();
                return;
            }
        }

		private void StopCoroutine()
        {
            if ( slowMoCoroutine != null )
            {
                level.StopCoroutine(slowMoCoroutine);
            }
        }
		public override void OnUnload() {
			if ( IsEnabled() ) {
                StopCoroutine();
				EventManager.onCreatureParry -= EventManager_onCreatureParry;
                EventManager.onCreatureKill -= EventManager_onCreatureKill;
			}

			base.OnUnload();
		}

	}
}