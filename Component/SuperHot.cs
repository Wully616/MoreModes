using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class SuperHot : LevelModuleOptional {
		//If you are rotating and moving in a direction, this value will scale back how much your movement affects speeding up time

		private bool enableSloMo;
		private SpellPowerSlowTime spellPowerSlowTime;
		protected Coroutine waveEndedCoroutine;
		private WaveSpawner waveSpawner;

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				spellPowerSlowTime = Catalog.GetData<SpellPowerSlowTime>("SlowTime");
				EventManager.onPossess += EventManager_onPossess;
				EventManager.onUnpossess += EventManager_onUnpossess;
				GameManager.slowMotionState = GameManager.SlowMotionState.Disabled;
				if (WaveSpawner.instances.Count > 0) {
					var waveSpawner = WaveSpawner.instances[0];
					waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnded);
				}
			}

			yield break;
		}

		private void EventManager_onPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnEnd) {
				creature.mana.RemoveSpell("SlowTime");
				enableSloMo = true;
			}
		}

		private void EventManager_onUnpossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnStart) {
				enableSloMo = false;
				GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
			}
		}

		protected void OnWaveEnded() {
			waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
		}

		private IEnumerator WaveEndedCoroutine() {
			GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
			DisplayMessage.ShowMessage(new DisplayMessage.MessageData("Super", 10,
				DisplayMessage.TextType.INFORMATION,
				0.5f));

			yield return new WaitForSeconds(0.5f);
			DisplayMessage.ShowMessage(new DisplayMessage.MessageData("HOT", 10,
				DisplayMessage.TextType.INFORMATION,
				0.5f));
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				GameManager.SetSlowMotion(false, 1, spellPowerSlowTime.exitCurve);
				EventManager.onPossess -= EventManager_onPossess;
				EventManager.onUnpossess -= EventManager_onUnpossess;
				enableSloMo = false;
				if (waveSpawner) {
					waveSpawner.OnWaveAnyEndEvent.RemoveListener(OnWaveEnded);
				}
			}

			base.OnUnload();
		}


		public override void Update() {
			base.Update();
			if (!enableSloMo) {
				return;
			}

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