using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class BigHead : LevelModule {
		public int headScale = 2;
		public override IEnumerator OnLoadCoroutine() {
			if ( Level.current.GetOptionAsBool("bighead", true) ) {
				EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
				foreach (Creature creature in Creature.all) {
					if ( creature.ragdoll.headPart.transform.localScale.x < 2.0f ) {
						creature.ragdoll.headPart.transform.localScale *= headScale;
					}
				}
			}

			yield return base.OnLoadCoroutine();
		}

		private void EventManager_onCreatureSpawn( Creature creature ) {
			if ( creature != Player.currentCreature ) {

				if ( creature.ragdoll.headPart.transform.localScale.x <= 2.0f ) {
					creature.ragdoll.headPart.transform.localScale *= headScale;
				}

				creature.ragdoll.OnStateChange += (( state, newState, change, time ) => Ragdoll_OnStateChange(creature.ragdoll, state, newState, change, time));

			}
		}

		private void Ragdoll_OnStateChange( Ragdoll ragdoll, Ragdoll.State previousState, Ragdoll.State newState, Ragdoll.PhysicStateChange physicStateChange, EventTime eventTime ) {
			if ( eventTime == EventTime.OnEnd ) {
				if ( ragdoll.headPart.transform.localScale.x <= 2.0f ) {
					ragdoll.headPart.transform.localScale *= headScale;
				}
			}
		}

		public override void OnUnload() {
			if ( Level.current.GetOptionAsBool("bighead", true) ) {
				EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
			}

			base.OnUnload();
		}
	}
}