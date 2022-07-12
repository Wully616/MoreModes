using System.Collections;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class BigHead : LevelModuleOptional {
		public float headScale = 2f;
		private List<Ragdoll> ragdolls;
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) { 
				EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;
				
				foreach (Creature creature in Creature.all) {
					if ( creature.ragdoll.headPart.transform.parent.localScale == Vector3.one ) {
						creature.ragdoll.headPart.transform.parent.SetGlobalScale(Vector3.one * headScale);
					}
				}
			}
			yield break;
		}

		private void EventManager_onCreatureSpawn( Creature creature ) {
			if ( creature != Player.currentCreature ) {

				if ( creature.ragdoll.headPart.transform.parent.localScale == Vector3.one ) {
					creature.ragdoll.headPart.transform.parent.SetGlobalScale(Vector3.one * headScale);
				}
				ragdolls.Add(creature.ragdoll);
				creature.ragdoll.OnStateChange += (( state, newState, change, time ) => Ragdoll_OnStateChange(creature.ragdoll, state, newState, change, time));

			}
		}

		private void Ragdoll_OnStateChange( Ragdoll ragdoll, Ragdoll.State previousState, Ragdoll.State newState, Ragdoll.PhysicStateChange physicStateChange, EventTime eventTime ) {
			if ( eventTime == EventTime.OnEnd ) {
				if (IsEnabled()) {
					if (ragdoll.headPart.transform.localScale == Vector3.one) {
						ragdoll.headPart.transform.SetGlobalScale(Vector3.one * headScale);
					}
				}
			}
		}

		public override void OnUnload() {
			if (IsEnabled()) {
				EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
			}

			base.OnUnload();
		}
	}
}