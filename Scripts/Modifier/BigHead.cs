using System.Collections;
using System.Collections.Generic;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	//TODO: broke
	public class BigHead : ModifierData {
		public float headScale = 2f;
		private List<Ragdoll> ragdolls;
		
		public static BigHead Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				// bit hacky, but we only one 1 modifier, if local isnt set, this modifier isnt Setup
				local = this;
				ragdolls = new List<Ragdoll>();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureSpawn += OnCreatureSpawn;
			foreach (Creature creature in Creature.all) {
				if ( creature.ragdoll.headPart.transform.parent.localScale == Vector3.one ) {
					if(creature == Player.currentCreature) continue;
					creature.ragdoll.headPart.transform.parent.SetGlobalScale(Vector3.one * headScale);
				}
			}
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureSpawn -= OnCreatureSpawn;
			foreach (Creature creature in Creature.all) {
				if ( creature.ragdoll.headPart.transform.parent.localScale == Vector3.one ) {
					if(creature == Player.currentCreature) continue;
					creature.ragdoll.headPart.transform.parent.SetGlobalScale(Vector3.one);
				}
			}
		}

		private void OnCreatureSpawn( Creature creature ) {
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
				if (ragdoll.headPart.transform.localScale == Vector3.one) {
					ragdoll.headPart.transform.SetGlobalScale(Vector3.one * headScale);
				}
			}
		}

	}
}