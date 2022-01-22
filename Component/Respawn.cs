using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using GameModeLoader.Module;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using GameModeLoader.Utils;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class Respawn : LevelModule {
		public LevelModuleDeath.Behaviour behaviour = LevelModuleDeath.Behaviour.ShowScores;
		public int lives = 3;
		public float delayBeforeLoad = 10f;
		public bool disablePlayerLocomotion = true;
		private Coroutine deathCoroutine;

		private Coroutine slowMotionDurationCoroutine;
		private int _lives = 3;

		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("respawns", true)) {
				_lives = lives;

				EventManager.onCreatureKill += EventManager_onCreatureKill;
				EventManager.onPossess += EventManager_onPossess;
				level.OnLevelEvent += Level_OnLevelEvent;
			}

			return base.OnLoadCoroutine();
		}

		private void Level_OnLevelEvent() {
			if (Level.current.GetOptionAsBool("respawns", true)) {
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
			if (this.disablePlayerLocomotion)
				player.locomotion.enabled = false;
			if (this.slowMotionDurationCoroutine != null)
				this.level.StopCoroutine(this.slowMotionDurationCoroutine);
			this.slowMotionDurationCoroutine = this.level.StartCoroutine(this.SlowMotionDurationCoroutine());
			if (_lives > 1) {
				_lives--;
				this.deathCoroutine = this.level.StartCoroutine(this.OnRespawnCoroutine(player, creature));
			} else {
				this.deathCoroutine = this.level.StartCoroutine(this.OnDeathCoroutine());
			}
		}

		private IEnumerator SlowMotionDurationCoroutine() {
			yield return null;
			GameManager.SetSlowMotion(true, Catalog.gameData.deathSlowMoRatio, Catalog.gameData.deathSlowMoEnterCurve,
				Catalog.gameData.deathEffectData);
			float t = Time.unscaledTime;
			while ((double) Time.unscaledTime - (double) t < (double) Catalog.gameData.deathSlowMoDuration)
				yield return null;
			GameManager.SetSlowMotion(false, Catalog.gameData.deathSlowMoRatio, Catalog.gameData.deathSlowMoExitCurve);
		}

		protected virtual IEnumerator OnDeathCoroutine() {
			float killTime = Time.unscaledTime;
			if (this.behaviour != LevelModuleDeath.Behaviour.ShowScores)
				CameraEffects.DoTimedEffect(UnityEngine.Color.black, CameraEffects.TimedEffect.FadeIn,
					this.delayBeforeLoad);
			CameraEffects.SetSepia(1f);
			yield return null;
			WaveSpawner waveSpawner;
			if (WaveSpawner.TryGetRunningInstance(out waveSpawner))
				waveSpawner.CancelWave();
			MenuBook.ShowToPlayer(true, false);
			MenuBook.SetPage("Scores");
			if (this.behaviour != LevelModuleDeath.Behaviour.ShowScores) {
				while ((double) Time.unscaledTime - (double) killTime < (double) this.delayBeforeLoad &&
				       ((double) Time.unscaledTime - (double) killTime <= 2.0 || !PlayerControl.uiClickDown))
					yield return null;
				if (this.behaviour == LevelModuleDeath.Behaviour.LoadHome)
					GameManager.LoadLevel(Player.characterData.gameMode.levelHome,
						Player.characterData.gameMode.levelHomeModeName);
				if (this.behaviour == LevelModuleDeath.Behaviour.ReloadLevel)
					GameManager.LoadLevel(GameManager.GetCurrentLevel());
			}
		}

		protected virtual IEnumerator OnRespawnCoroutine(Player player, Creature creature) {
			float killTime = Time.unscaledTime;

			CameraEffects.DoTimedEffect(UnityEngine.Color.black, CameraEffects.TimedEffect.FadeIn,
				this.delayBeforeLoad * 0.5f);
			CameraEffects.SetSepia(1f);
			DisplayText.ShowText(new DisplayText.TextPriority($"You have died.", 10, TutorialData.TextType.INFORMATION,
				5f));
			yield return new WaitForSeconds(2f);

			while (Time.unscaledTime - killTime < this.delayBeforeLoad &&
			       (Time.unscaledTime - killTime <= 2.0)) {
				yield return null;
			}

			CameraEffects.DoTimedEffect(UnityEngine.Color.black, CameraEffects.TimedEffect.FadeOut,
				this.delayBeforeLoad * 0.1f);
			creature.Resurrect(creature.data.health * 0.5f, creature);
			player.SetCreature(creature);
			player.locomotion.enabled = true;
			CameraEffects.SetSepia(0.0f);
			DisplayText.ShowText(new DisplayText.TextPriority($"{_lives} lives left!", 10,
				TutorialData.TextType.INFORMATION,
				4f));
		}

		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("respawns", true)) {
				level.OnLevelEvent -= Level_OnLevelEvent;
				EventManager.onCreatureKill -= EventManager_onCreatureKill;
				EventManager.onPossess -= EventManager_onPossess;
				if (this.deathCoroutine != null)
					this.level.StopCoroutine(this.deathCoroutine);
				if (this.slowMotionDurationCoroutine != null)
					this.level.StopCoroutine(this.slowMotionDurationCoroutine);
				CameraEffects.SetSepia(0.0f);
			}
		}
	}
}