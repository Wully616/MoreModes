using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class Bloodlust : ModifierData {
		public static Bloodlust Instance;

		public float healthPerSecond = 5f;

		private bool regen;
		public override void Init()
		{
			if (Instance != null) return;
			
			base.Init();
			local = Instance = this;
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
			//return if it wasnt the player being hit, or if the player did the hit
			if(creature != Player.currentCreature || collisionInstance.IsDoneByPlayer() ) return;
			regen = false;

		}
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;

			regen = true;
		}

		public override void Update()
		{
			if(Player.currentCreature == null) return;
			base.Update();
			
			//only heal if its been longer than the cooldown since we last got hit
			if (regen && Player.currentCreature.currentHealth <= Player.currentCreature.maxHealth)
			{
				Player.currentCreature.Heal(healthPerSecond * Time.deltaTime, Player.currentCreature);
			}
		}
	}
}