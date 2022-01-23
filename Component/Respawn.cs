using System.Collections;
using System.Linq;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class Respawn : LevelModuleOptional {
		private int _lives = 3;
		public LevelModuleDeath.Behaviour behaviour = LevelModuleDeath.Behaviour.ShowScores;
		private Coroutine deathCoroutine;
		public float delayBeforeLoad = 10f;
		public bool disablePlayerLocomotion = true;
		public int lives = 3;

		private Coroutine slowMotionDurationCoroutine;

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				_lives = lives;

				EventManager.onCreatureKill += EventManager_onCreatureKill;
				EventManager.onPossess += EventManager_onPossess;
				level.OnLevelEvent += Level_OnLevelEvent;
			}

			yield break;
		}

		private void Level_OnLevelEvent() {
			if ( IsEnabled() ) {
				//Unload LevelModuleDeath
				var levelModuleDeath = level?.mode?.modules?.First(d => d.type == typeof(LevelModuleDeath));
				if (levelModuleDeath != null) {
					//call unload
					levelModuleDeath.OnUnload();
					//remove
					for (int i = level.mode.modules.Count - 1; i >= 0; i--) {
						if (level.mode.modules[i] == levelModuleDeath) {
							level.mode.modules.RemoveAt(i);
						}
					}
				}
			}
		}

		private void EventManager_onPossess(Creature creature, EventTime eventTime) {
			if (eventTime != EventTime.OnEnd || !Player.local?.creature)
				return;
			CameraEffects.SetSepia(0.0f);
			Player.local.locomotion.enabled = false;
			DisplayText.ShowText(new DisplayText.TextPriority($"{_lives} lives left!", 10,
				TutorialData.TextType.INFORMATION,
				4f));
		}

		private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime) {
			if (eventTime != EventTime.OnEnd || !player)
				return;
			if (disablePlayerLocomotion)
				player.locomotion.enabled = false;
			if (slowMotionDurationCoroutine != null)
				level.StopCoroutine(slowMotionDurationCoroutine);
			slowMotionDurationCoroutine = level.StartCoroutine(SlowMotionDurationCoroutine());
			if (_lives > 1) {
				_lives--;
				deathCoroutine = level.StartCoroutine(OnRespawnCoroutine(player, creature));
			} else {
				deathCoroutine = level.StartCoroutine(OnDeathCoroutine());
			}
		}

		private IEnumerator SlowMotionDurationCoroutine() {
			yield return null;
			GameManager.SetSlowMotion(true, Catalog.gameData.deathSlowMoRatio, Catalog.gameData.deathSlowMoEnterCurve,
				Catalog.gameData.deathEffectData);
			float t = Time.unscaledTime;
			while (Time.unscaledTime - (double) t < Catalog.gameData.deathSlowMoDuration)
				yield return null;
			GameManager.SetSlowMotion(false, Catalog.gameData.deathSlowMoRatio, Catalog.gameData.deathSlowMoExitCurve);
		}

		protected virtual IEnumerator OnDeathCoroutine() {
			float killTime = Time.unscaledTime;
			if (behaviour != LevelModuleDeath.Behaviour.ShowScores)
				CameraEffects.DoTimedEffect(Color.black, CameraEffects.TimedEffect.FadeIn,
					delayBeforeLoad);
			CameraEffects.SetSepia(1f);
			yield return null;
			WaveSpawner waveSpawner;
			if (WaveSpawner.TryGetRunningInstance(out waveSpawner))
				waveSpawner.CancelWave();
			MenuBook.ShowToPlayer(true, false);
			MenuBook.SetPage("Scores");
			if (behaviour != LevelModuleDeath.Behaviour.ShowScores) {
				while (Time.unscaledTime - (double) killTime < delayBeforeLoad &&
				       (Time.unscaledTime - (double) killTime <= 2.0 || !PlayerControl.uiClickDown))
					yield return null;
				if (behaviour == LevelModuleDeath.Behaviour.LoadHome)
					GameManager.LoadLevel(Player.characterData.gameMode.levelHome,
						Player.characterData.gameMode.levelHomeModeName);
				if (behaviour == LevelModuleDeath.Behaviour.ReloadLevel)
					GameManager.LoadLevel(GameManager.GetCurrentLevel());
			}
		}

		protected virtual IEnumerator OnRespawnCoroutine(Player player, Creature creature) {
			float killTime = Time.unscaledTime;

			CameraEffects.DoTimedEffect(Color.black, CameraEffects.TimedEffect.FadeIn,
				delayBeforeLoad * 0.5f);
			CameraEffects.SetSepia(1f);
			DisplayText.ShowText(new DisplayText.TextPriority("You have died.", 10, TutorialData.TextType.INFORMATION,
				5f));
			yield return new WaitForSeconds(2f);

			while (Time.unscaledTime - killTime < delayBeforeLoad &&
			       Time.unscaledTime - killTime <= 2.0) {
				yield return null;
			}

			CameraEffects.DoTimedEffect(Color.black, CameraEffects.TimedEffect.FadeOut,
				delayBeforeLoad * 0.1f);
			creature.Resurrect(creature.data.health * 0.5f, creature);
			player.SetCreature(creature);
			player.locomotion.enabled = true;
			CameraEffects.SetSepia(0.0f);
			DisplayText.ShowText(new DisplayText.TextPriority($"{_lives} lives left!", 10,
				TutorialData.TextType.INFORMATION,
				4f));
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				level.OnLevelEvent -= Level_OnLevelEvent;
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
				EventManager.onPossess -= EventManager_onPossess;
				if (deathCoroutine != null)
					level.StopCoroutine(deathCoroutine);
				if (slowMotionDurationCoroutine != null)
					level.StopCoroutine(slowMotionDurationCoroutine);
				CameraEffects.SetSepia(0.0f);
			}
		}
	}
}