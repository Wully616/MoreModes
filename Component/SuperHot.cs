using System.Collections;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class SuperHot : LevelModule {
		//If you are rotating and moving in a direction, this value will scale back how much your movement affects speeding up time

		private bool enable;
		private SpellPowerSlowTime spellPowerSlowTime;
		protected Coroutine waveEndedCoroutine;
		private WaveSpawner waveSpawner;

		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("superhot", true)) {
				spellPowerSlowTime = Catalog.GetData<SpellPowerSlowTime>("SlowTime");
				EventManager.onPossess += EventManager_onPossess;
				EventManager.onUnpossess += EventManager_onUnpossess;
				GameManager.slowMotionState = GameManager.SlowMotionState.Disabled;
				if (WaveSpawner.instances.Count > 0) {
					var waveSpawner = WaveSpawner.instances[0];
					waveSpawner.OnWaveEndEvent.AddListener(OnWaveEnded);
				}
			}

			return base.OnLoadCoroutine();
		}

		private void EventManager_onPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnEnd) {
				creature.mana.RemoveSpell("SlowTime");
				enable = true;
			}
		}

		private void EventManager_onUnpossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnStart) {
				enable = false;
				GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
			}
		}

		protected void OnWaveEnded() {
			waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
		}

		private IEnumerator WaveEndedCoroutine() {
			GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
			DisplayText.ShowText(new DisplayText.TextPriority("Super", 10,
				TutorialData.TextType.INFORMATION,
				0.5f));

			yield return new WaitForSeconds(0.5f);
			DisplayText.ShowText(new DisplayText.TextPriority("HOT", 10,
				TutorialData.TextType.INFORMATION,
				0.5f));
		}

		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("superhot", true)) {
				GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
				EventManager.onPossess -= EventManager_onPossess;
				EventManager.onUnpossess -= EventManager_onUnpossess;
				enable = false;
				if (waveSpawner) {
					waveSpawner.OnWaveEndEvent.RemoveListener(OnWaveEnded);
				}
			}

			base.OnUnload();
		}


		public override void Update() {
			base.Update();
			if (!enable) {
				return;
			}

			//check if the players moving.
			//float vel = GetPlayerVelocity();
			float vel = Mathf.Clamp01(GetPlayerInput());


			float lerp = Mathf.Clamp01(vel);

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

		private float GetPlayerVelocity() {
			var creature = Player.currentCreature;
			if (creature == null) {
				return 0;
			}


			var leftHand = creature.handLeft;
			var rightHand = creature.handRight;
			var head = creature.ragdoll.headPart;

			var vel = head.rb.angularVelocity
			          + head.rb.velocity
			          + leftHand.rb.angularVelocity
			          + leftHand.rb.velocity
			          + rightHand.rb.angularVelocity
			          + rightHand.rb.velocity
			          + Player.local.locomotion.rb.velocity
			          + Player.local.locomotion.rb.angularVelocity;
			if (leftHand.grabbedHandle) {
				vel += leftHand.grabbedHandle.rb.velocity;
			}

			if (rightHand.grabbedHandle) {
				vel += rightHand.grabbedHandle.rb.velocity;
			}

			return vel.magnitude / 10f;
		}
	}
}