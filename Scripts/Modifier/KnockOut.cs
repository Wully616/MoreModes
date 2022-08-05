using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class KnockOut : ModifierData {
		public static KnockOut Instance;

		public float knockOutTime = 30f;
		private bool originalInvincibilitySetting;
		private float lastDamageTime;
		public override void Init()
		{
			if (Instance != null) return;
			
			base.Init();
			local = Instance = this;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureHit += OnCreatureHit;
		}
		

		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureHit -= OnCreatureHit;
		}
		
		
		private void OnCreatureHit(Creature creature, CollisionInstance collisionInstance)
		{
			if(creature == Player.currentCreature || !collisionInstance.IsDoneByPlayer() ) return;

			if (collisionInstance.damageStruct.hitRagdollPart.type == RagdollPart.Type.Head && collisionInstance.damageStruct.damageType == DamageType.Blunt)
			{
				creature.ragdoll.SetState(Ragdoll.State.Inert, true);
				creature.brain.AddNoStandUpModifier(this);
				creature.brain.currentTarget = null;
				creature.spawnGroup = null;
				Level.current.StartCoroutine(WakeUp(creature));
			}
		}

		private IEnumerator WakeUp(Creature creature)
		{
			yield return Yielders.ForSeconds(knockOutTime);
			if (creature != null)
			{
				creature.brain.RemoveNoStandUpModifier(this);
			}	
			
		}
	}
}