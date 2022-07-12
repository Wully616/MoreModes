using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	
	public class SuperHot : Modifier {
		private SpellPowerSlowTime spellPowerSlowTime;
		private bool superHotEnabled;
		
		public override void Init()
		{
			base.Init();
			spellPowerSlowTime = Catalog.GetData<SpellPowerSlowTime>("SlowTime");
		}
		
		//Remove slowmo and enable superhot
		private void RemoveSlowMo()
		{
			superHotEnabled = true;
			Player.local.creature.container.RemoveContent("SpellSlowTime");
			GameManager.slowMotionState = GameManager.SlowMotionState.Disabled;
		}
		
		//Add slowmo and disable superhot
		private void AddSlowMo()
		{
			superHotEnabled = false;
			Player.local.creature.container.RemoveContent("SpellSlowTime");
			GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			RemoveSlowMo();
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			AddSlowMo();
		}
		
		protected override void OnPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnEnd)
			{
				RemoveSlowMo();
			}
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnStart)
			{
				AddSlowMo();
			}
		}
		
		public override void Update() {
			base.Update();
			if (!superHotEnabled) return;
			//check if the players moving.
			float lerp = Mathf.Clamp01(GetPlayerInput());
			GameManager.SetTimeScale(lerp);
		}

		private float GetPlayerInput() {
			var loco = Player.local.locomotion;
			var movement = loco.moveDirection.magnitude * 10f;
			movement += Mathf.Abs(loco.turnSnapDirection);
			movement += Mathf.Abs(loco.turnSmoothSnapDirection);
			movement += Mathf.Abs(loco.turnSmoothDirection);
			movement /= 2f;
			movement = movement * movement;
			movement += loco.isJumping ? 1f : 0;
			var hands =
				(PlayerControl.handLeft.GetHandVelocity() + PlayerControl.handRight.GetHandVelocity()).magnitude / 2f;

			return movement + hands;
		}

	}
}