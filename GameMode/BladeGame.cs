using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.GameMode {
	/// <summary>
	///     This survival mode inherits from the base games survival game mode
	/// </summary>
	public class BladeGame : LevelModuleOptional {
		protected Coroutine waveEndedCoroutine;
		protected WaveSpawner waveSpawner;

		public override void Update() { }

		public override IEnumerator OnLoadCoroutine() {
			if (IsEnabled()) {
				if (WaveSpawner.instances.Count > 0) {
					waveSpawner = WaveSpawner.instances[0];
					waveSpawner.OnWaveEndEvent.AddListener(OnWaveEnded);
					level.StartCoroutine(LevelLoadedCoroutine());
					yield break;
				}
			}

			Debug.LogError("No wave spawner available for Blade Game module!");
		}

		protected void OnWaveEnded() {
			waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
		}

		private IEnumerator LevelLoadedCoroutine() {
			while (!Player.local || !Player.local.creature)
				yield return new WaitForSeconds(2f);
		}


		private IEnumerator WaveEndedCoroutine() {
			yield return new WaitForSeconds(2f);
		}
	}
}