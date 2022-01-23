using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component {
	/// <summary>
	///     This survival mode inherits from the base games survival game mode
	/// </summary>
	public class SurvivalMode : LevelModuleSurvival {
		private EffectData rewardFxData;

		public string rewardFxId = "SurvivalMode.RewardFx";

		public override void Update() {
			if (!(Player.currentCreature != null))
				return;

			var frontEyes = Player.local.head.transform.position + Player.local.head.transform.forward * 0.5f;
			rewardsSpawnPosition[0].position = frontEyes + -Player.local.head.transform.right * 0.5f;
			rewardsSpawnPosition[1].position = frontEyes;
			rewardsSpawnPosition[2].position = frontEyes + Player.local.head.transform.right * 0.5f;
		}

		public override IEnumerator OnLoadCoroutine() {
			spawnPositionHeight = 0f;
			rewardFxData = Catalog.GetData<EffectData>(rewardFxId);

			rewardsSpawnPosition = new List<Transform> {
				new GameObject().transform,
				new GameObject().transform,
				new GameObject().transform
			};
			foreach (var rewardTransform in rewardsSpawnPosition) {
				var go = new GameObject("SpawnPosition");
				go.transform.SetParent(rewardTransform);
				go.transform.rotation = Quaternion.Euler(90, 0, 0);
			}

			rewards = new List<Item>();
			waitingToChooseReward = false;
			currentWaveNumberForReward = 0;
			waveIndex = 0;

			foreach (var component in Object.FindObjectsOfType<UIWaveSpawner>()) {
				component.gameObject.SetActive(false);
			}

			if (WaveSpawner.instances.Count > 0) {
				waveSpawner = WaveSpawner.instances[0];
				waveSpawner.OnWaveEndEvent.AddListener(OnWaveEnded);
				levelModuleTutorial = level.mode.GetModule<LevelModuleTutorial>();
				level.StartCoroutine(LevelLoadedCoroutine());
				yield break;
			}

			Debug.LogError("No wave spawner available for survival module!");
		}

		protected new void OnWaveEnded() {
			waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
		}

		private IEnumerator LevelLoadedCoroutine() {
			while (!Player.local || !Player.local.creature)
				yield return new WaitForSeconds(2f);

			for (int i = 0; i < rewardsSpawnPosition.Count; i++) {
				rewardFxData?.Spawn(rewardsSpawnPosition[i].position, rewardsSpawnPosition[i].rotation).Play();
			}

			yield return new WaitForSeconds(2f);
			waitingToChooseReward = true;
			for (int aNumber = 0; aNumber < rewardsToSpawn; ++aNumber) {
				SpawnRandomItem(aNumber, true);
				if (!waitingToChooseReward)
					break;
			}

			yield return new WaitForSeconds(1f);
			for (int index = 0; index < rewards.Count; ++index) {
				Transform transform1 = rewards[index].transform.Find("Whoosh");
				Transform transform2 = rewards[index].transform.Find("Handle");
				ConfigurableJoint component =
					rewardsSpawnPosition[index].Find("SpawnPosition").GetComponent<ConfigurableJoint>();
				rewards[index].transform.position = component.transform.position;
				component.connectedBody = rewards[index].rb;
				if (transform1 && transform2 && transform1.position.y < (double) transform2.position.y)
					component.targetRotation = new Quaternion(0.0f, 180f, 0.0f, 1f);
			}

			while (waitingToChooseReward)
				yield return 0;
			for (int index = 0; index < rewardsSpawnPosition.Count; ++index) {
				ConfigurableJoint component =
					rewardsSpawnPosition[index].Find("SpawnPosition").GetComponent<ConfigurableJoint>();
				if (component != null)
					component.connectedBody = null;
			}

			for (int i = 0; i < rewardsSpawnPosition.Count; i++) {
				rewardFxData?.Spawn(rewardsSpawnPosition[i].position, rewardsSpawnPosition[i].rotation).Play();
			}

			yield return new WaitForSeconds(startDelay);
			DisplayText.ShowText(new DisplayText.TextPriority(
				Catalog.GetTextData().GetString(textGroupId, textNextWaveId), 10, TutorialData.TextType.INFORMATION,
				1f));
			for (int i = 3; i > 0; --i) {
				DisplayText.ShowText(new DisplayText.TextPriority(i.ToString(), 10, TutorialData.TextType.INFORMATION,
					1f));
				yield return new WaitForSeconds(2f);
			}

			DisplayText.ShowText(new DisplayText.TextPriority(Catalog.GetTextData().GetString(textGroupId, textFightId),
				10, TutorialData.TextType.INFORMATION, 1f));
			yield return new WaitForSeconds(1f);
			WaveData data = Catalog.GetData<WaveData>(waves[waveIndex].waveID);
			if (data != null)
				waveSpawner.StartWave(data, 5f);
			else
				Debug.LogError("Unable to find wave " + waves[waveIndex].waveID);
			DisplayText.ShowText(new DisplayText.TextPriority(
				Catalog.GetTextData().GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
				TutorialData.TextType.INFORMATION, 3f));
		}


		private IEnumerator WaveEndedCoroutine() {
			yield return new WaitForSeconds(2f);
			++waveIndex;
			++currentWaveNumberForReward;
			if (currentWaveNumberForReward >= wavesNumberForReward) {
				currentWaveNumberForReward = 0;
				for (int i = 0; i < rewardsSpawnPosition.Count; i++) {
					rewardFxData?.Spawn(rewardsSpawnPosition[i].position, rewardsSpawnPosition[i].rotation).Play();
				}

				yield return new WaitForSeconds(2f);
				waitingToChooseReward = true;
				rewards = new List<Item>();
				for (int aNumber = 0; aNumber < rewardsToSpawn; ++aNumber) {
					SpawnRandomItem(aNumber);
					if (!waitingToChooseReward)
						break;
				}

				yield return new WaitForSeconds(0.1f);
				for (int index = 0; index < rewards.Count; ++index) {
					Transform transform1 = rewards[index].transform.Find("Whoosh");
					Transform transform2 = rewards[index].transform.Find("Handle");
					ConfigurableJoint component = rewardsSpawnPosition[index].Find("SpawnPosition")
						.GetComponent<ConfigurableJoint>();
					rewards[index].transform.position = component.transform.position;
					component.connectedBody = rewards[index].rb;
					if (transform1 != null && transform2 != null &&
					    transform1.position.y < (double) transform2.position.y)
						component.targetRotation = new Quaternion(0.0f, 180f, 0.0f, 1f);
				}

				while (waitingToChooseReward)
					yield return 0;
				for (int index = 0; index < rewardsSpawnPosition.Count; ++index)
					rewardsSpawnPosition[index].Find("SpawnPosition").GetComponent<ConfigurableJoint>().connectedBody =
						null;
				for (int i = 0; i < rewardsSpawnPosition.Count; i++) {
					rewardFxData?.Spawn(rewardsSpawnPosition[i].position, rewardsSpawnPosition[i].rotation).Play();
				}
			}

			if (Player.local.creature && Player.local.creature.currentHealth > 0.0) {
				if (waveIndex > waves.Count - 1) {
					if (repeatLastWaveIndefinitly) {
						yield return new WaitForSeconds(delayBetweenWave);
						WaveData data = Catalog.GetData<WaveData>(waves[waves.Count - 1].waveID);
						if (data == null) {
							Debug.LogError("Wave " + waves[waves.Count - 1] + " Not found!");
						} else {
							waveSpawner.StartWave(data, 5f, false);
							DisplayText.ShowText(new DisplayText.TextPriority(
								Catalog.GetTextData().GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
								TutorialData.TextType.INFORMATION, 3f));
						}
					} else {
						Level.current.state = Level.State.Success;
						yield return new WaitForSeconds(2f);
						MenuBook.ShowToPlayer(true, false);
						MenuBook.SetPage("Scores");
					}
				} else {
					yield return new WaitForSeconds(delayBetweenWave);
					WaveData data = Catalog.GetData<WaveData>(waves[waveIndex].waveID);
					if (data == null) {
						Debug.LogError("Wave " + waves[waveIndex] + " Not found!");
					} else {
						waveSpawner.StartWave(data, 5f, false);
						DisplayText.ShowText(new DisplayText.TextPriority(
							Catalog.GetTextData().GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
							TutorialData.TextType.INFORMATION, 3f));
					}
				}
			}
		}
	}
}