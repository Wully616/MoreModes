using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class HpOnKill : LevelModuleOptional {
		public int hpAmount = 10;

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onCreatureKill += EventManager_onCreatureKill;
			}

			yield break;
		}

		private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;
			Heal(Player.currentCreature, hpAmount);
		}

		public void Heal( Creature healee, float heal ) {
			if ( (double)healee.currentHealth == (double)healee.maxHealth )
				return;
			healee.currentHealth += heal;
			if ( healee.currentHealth >= (double)healee.maxHealth )
				healee.currentHealth = healee.maxHealth;
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
			}

			base.OnUnload();
		}
	}
}