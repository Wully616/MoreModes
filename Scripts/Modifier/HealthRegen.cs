using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class HealthRegen : ModifierData
	{
		public float healthPerSecond = 5f;
		public float damageCooldown = 5f;
		private float lastHit;
		public static HealthRegen Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				local = this;
				lastHit = Time.time;
			}
		}

		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			creature.OnDamageEvent += playerOnDamage;
		}
		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if(eventTime == EventTime.OnEnd) return;
			creature.OnDamageEvent -= playerOnDamage;
		}
		
		private void playerOnDamage(CollisionInstance collisioninstance)
		{
			lastHit = Time.time;
		}

		public override void Update()
		{
			if(Player.currentCreature == null) return;
			base.Update();
			var diff = Time.time - lastHit;
			//only heal if its been longer than the cooldown since we last got hit
			if (diff > damageCooldown && Player.currentCreature.currentHealth <= Player.currentCreature.maxHealth)
			{
				Player.currentCreature.Heal(healthPerSecond * Time.deltaTime, Player.currentCreature);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Player.currentCreature.OnDamageEvent += playerOnDamage;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			Player.currentCreature.OnDamageEvent -= playerOnDamage;
		}
		
	}
}