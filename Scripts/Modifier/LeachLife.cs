using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class LeachLife : ModifierData {
		
		public static LeachLife Instance;

		public override void Init()
		{
			if (Instance != null) return;
			base.Init();
			Instance = this;
			local = this;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureKill += OnCreatureKill;
			EventManager.onCreatureHit += OnCreatureHit;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureKill -= OnCreatureKill;
			EventManager.onCreatureHit -= OnCreatureHit;
		}


		private void OnCreatureHit(Creature creature, CollisionInstance collisionInstance)
		{
			if (creature == Player.currentCreature || !collisionInstance.IsDoneByPlayer()) return;
			var damage = collisionInstance.damageStruct.damage;
			if (damage > 0)
			{
				Heal(Player.currentCreature, damage * 0.5f);
			}
		}
		
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart  || player || !collisionInstance.IsDoneByPlayer() )
				return;

			var damage = collisionInstance.damageStruct.damage;
			if (damage > 0)
			{
				Heal(Player.currentCreature, damage * 0.5f);
			}
		}

		public void Heal( Creature creature, float heal ) {
			if (creature.currentHealth >= creature.maxHealth)
			{
				creature.currentHealth = creature.maxHealth;
				return;
			}
			creature.currentHealth += heal;
			if (creature.currentHealth >= (double)creature.maxHealth)
			{
				creature.currentHealth = creature.maxHealth;
			}
		}
		
	}
}