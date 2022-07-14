using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	public class HpOnKill : Modifier {
		public int hpAmount = 10;
		public static HpOnKill Instance;

		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				// bit hacky, but we only one 1 modifier, if local isnt set, this modifier isnt Setup
				local = this;
			}
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			EventManager.onCreatureKill += EventManager_onCreatureKill;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureKill -= EventManager_onCreatureKill;
		}
		
		private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;
			Heal(Player.currentCreature, hpAmount);
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