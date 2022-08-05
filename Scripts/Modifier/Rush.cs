using System.Collections;

using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class Rush : ModifierData {
		public static Rush Instance;
		
		public float speedModifier = 3f;
		
		public float rushTime = 2f;
		private float lastKillTime;
		private bool rushing;
		
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

			
		}
		

		protected override void OnDisable()
		{
			base.OnDisable();
			EventManager.onCreatureKill -= OnCreatureKill;
			Player.local.locomotion.RemoveSpeedModifier(this);
			rushing = false;
		}
		
		
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if ( eventTime == EventTime.OnStart || player || !collisionInstance.IsDoneByPlayer() )
				return;
			
			lastKillTime = Time.time + lastKillTime;
			if (!rushing)
			{
				Player.local.locomotion.RemoveSpeedModifier(this);
				Player.local.locomotion.SetSpeedModifier(this, speedModifier, speedModifier, speedModifier, speedModifier, 1f);
				rushing = true;
			}
		}

		public override void Update()
		{
			base.Update();
			//Player is invincible as long as they did damage within the last berserkerTime seconds
			if (Time.time > lastKillTime)
			{
				if (rushing)
				{
					Player.local.locomotion.RemoveSpeedModifier(this);
					rushing = false;
				}
			}
		}
	}
}