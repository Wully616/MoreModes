using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class Beserker : ModifierData {
		public static Beserker Instance;

		public float berserkerTime = 1f;
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
			EventManager.onCreatureKill += OnCreatureKill;
			EventManager.onCreatureHit += OnCreatureHit;
			originalInvincibilitySetting = Player.invincibility;
			lastDamageTime = Time.time;
		}
		

		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureKill -= OnCreatureKill;
			EventManager.onCreatureHit -= OnCreatureHit;
			Player.invincibility = originalInvincibilitySetting;
		}
		
		
		private void OnCreatureHit(Creature creature, CollisionInstance collisionInstance)
		{
			if(creature == Player.currentCreature || !collisionInstance.IsDoneByPlayer() ) return;
			lastDamageTime = Time.time + berserkerTime;
			
		}
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;
			
			lastDamageTime = Time.time + berserkerTime;
		}

		public override void Update()
		{
			base.Update();
			//Player is invincible as long as they did damage within the last berserkerTime seconds
			Player.invincibility = Time.time < lastDamageTime;
		}
	}
}