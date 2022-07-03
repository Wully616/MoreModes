using System.Collections;
using System.Collections.Generic;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.GameMode {
	/// <summary>
	///     This survival mode inherits from the base games survival game mode
	/// </summary>
	public class SurvivalMode : LevelModuleSurvival, IOption {
		public string id;
		public bool enable;
		private EffectData rewardFxData;

		public string rewardFxId = "SurvivalMode.RewardFx";

		public override void Update() {
			if (!IsEnabled()) { return;}
			if (!(Player.currentCreature != null)) { return; }

			var frontEyes = Player.local.head.transform.position + Player.local.head.transform.forward * 0.5f + Player.local.head.transform.up * -0.1f;
			rewardsSpawnPosition[0].position = frontEyes + -Player.local.head.transform.right * 0.15f;
			rewardsSpawnPosition[1].position = frontEyes;
			rewardsSpawnPosition[2].position = frontEyes + Player.local.head.transform.right * 0.15f;
		}

		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( !IsEnabled() ) { yield break; }
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
				waveSpawner.OnWaveAnyEndEvent.AddListener(OnWaveEnded);
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
			DisplayMessage.ShowMessage(new DisplayMessage.MessageData(
				LocalizationManager.Instance.GetString(textGroupId, textNextWaveId), 10, DisplayMessage.TextType.INFORMATION,
				1f));
			for (int i = 3; i > 0; --i) {
				DisplayMessage.ShowMessage(new DisplayMessage.MessageData(i.ToString(), 10, DisplayMessage.TextType.INFORMATION,
					1f));
				yield return new WaitForSeconds(2f);
			}

			DisplayMessage.ShowMessage(new DisplayMessage.MessageData(LocalizationManager.Instance.GetString(textGroupId, textFightId),
				10, DisplayMessage.TextType.INFORMATION, 1f));
			yield return new WaitForSeconds(1f);
			WaveData data = Catalog.GetData<WaveData>(waves[waveIndex].waveID);
			if (data != null)
				waveSpawner.StartWave(data, 5f);
			else
				Debug.LogError("Unable to find wave " + waves[waveIndex].waveID);
			DisplayMessage.ShowMessage(new DisplayMessage.MessageData(
				LocalizationManager.Instance.GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
				DisplayMessage.TextType.INFORMATION, 3f));
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
							DisplayMessage.ShowMessage(new DisplayMessage.MessageData(
								LocalizationManager.Instance.GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
								DisplayMessage.TextType.INFORMATION, 3f));
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
						DisplayMessage.ShowMessage(new DisplayMessage.MessageData(
							LocalizationManager.Instance.GetString(textGroupId, textWaveId) + " " + (waveIndex + 1), 10,
							DisplayMessage.TextType.INFORMATION, 3f));
					}
				}
			}
		}

		public bool IsEnabled() {
			//the enable bool is like the master switch, so it can be forcefully enabled for gamemodes
			//the option check is to check if it should be enabled or not on a per map/gamemode basis
			return enable || Level.current.GetOptionAsBool(id);
		}

		public void SetId() {
			//get the id of this LevelModuleOptionals Option data.
			var options = Module.GameModeLoader.GetLevelOptionList();
			foreach ( var option in options ) {
				if ( option.levelOption.levelModuleOptional.GetType() == this.GetType() ) {
					this.id = option.levelOption.name;
					break;
				}
			}
		}
	}
}