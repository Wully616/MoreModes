using System.Collections;
using System.Collections.Generic;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes.GameMode {
	/// <summary>
	///     This survival mode inherits from the base games survival game mode
	/// </summary>
	public class SurvivalMode : LevelModuleSurvival {
		public string id;
		public bool enable;
		private EffectData rewardFxData;

		public string rewardFxId = "Wully.SurvivalMode.RewardFx";

		public override void Update() {
			if (!(Player.currentCreature != null)) { return; }

			Transform headTransform = Player.local.head.transform;
			var frontEyes = headTransform.position + headTransform.forward * 0.5f + headTransform.up * -0.1f;
			rewardsSpawnPosition[0].position = frontEyes + -headTransform.right * 0.15f;
			rewardsSpawnPosition[1].position = frontEyes;
			rewardsSpawnPosition[2].position = frontEyes + headTransform.right * 0.15f;
		}

		public override IEnumerator OnLoadCoroutine() {
			
			spawnPositionHeight = 0f;
			rewardFxData = Catalog.GetData<EffectData>(rewardFxId);

			rewardsSpawnPosition = new List<Transform> {
				new GameObject().transform,
				new GameObject().transform,
				new GameObject().transform
			};
			
			DisableSandboxItems();

			foreach (var rewardTransform in rewardsSpawnPosition) {
				var go = new GameObject("SpawnPosition");
				go.transform.SetParent(rewardTransform);
				go.transform.rotation = Quaternion.Euler(90, 0, 0);
			}

			rewards = new List<Item>();
			waitingToChooseReward = false;
			currentWaveNumberForReward = 0;
			waveIndex = 0;
			if (WaveSpawner.instances.Count > 0)
			{
				waveSpawner = WaveSpawner.instances[0];
				waveSpawner.OnWaveWinEvent.AddListener(OnWaveEnded);
				waveSpawner.OnWaveLossEvent.AddListener(OnWaveEnded);
				waveSpawner.OnWaveCancelEvent.AddListener(OnWaveEnded);
			}
			else
			{
				Debug.LogError("No wave spawner available for survival module!");
				yield break;
			}
			level.StartCoroutine(LevelLoadedCoroutine());

			yield break;
		}
		
		private void DisableSandboxItems()
		{
			foreach (UIWaveSpawner uiWaveSpawner in GameObject.FindObjectsOfType<UIWaveSpawner>())
			{
				uiWaveSpawner.gameObject.SetActive(false);
			}

			foreach (Level.CustomReference customReference in level.customReferences)
			{
				if (customReference.name == "WaveSelector")
				{
					foreach (Transform transform in customReference.transforms)
					{
						transform.gameObject.SetActive(false);
					}
				}
				if (customReference.name == "Rack")
				{
					foreach (Transform transform in customReference.transforms)
					{
						transform.gameObject.SetActive(false);
					}
				}
				if (customReference.name == "WeaponSelector")
				{
					foreach (Transform transform in customReference.transforms)
					{
						transform.gameObject.SetActive(false);
					}
				}
			}
		}

		protected new void OnWaveEnded() {
			waveEndedCoroutine = level.StartCoroutine(WaveEndedCoroutine());
		}

		private IEnumerator LevelLoadedCoroutine() {
			while (!Player.local || !Player.local.creature)
				yield return new WaitForSeconds(2f);
			
			//Remove all of the items in the players holders
			var holders = Player.local.creature.GetComponentsInChildren<Holder>();
			foreach (Holder holder in holders)
			{
				for (int i = holder.items.Count - 1; i >= 0; i--)
				{
					Item item = holder.items[i];
					holder.UnSnap(item, true, false);
					item.Despawn();
				}
			}
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
			Utilities.Message($"{textGroupId} : {textNextWaveId}");
			
			for (int i = 3; i > 0; --i) {
				Utilities.Message($"{i}");
				yield return new WaitForSeconds(2f);
			}

			Utilities.Message($"{textGroupId} : {textFightId}");
			yield return new WaitForSeconds(1f);
			WaveData data = Catalog.GetData<WaveData>(waves[waveIndex].waveID);
			if (data != null)
			{
				waveSpawner.StartWave(data, 5f);
			}
			else
			{
				Debug.LogError($"Unable to find wave {waves[waveIndex].waveID}");
			}
			Utilities.Message($"{textGroupId} : {textWaveId} {waveIndex + 1}");
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
							Debug.LogError($"Wave {waves[waves.Count - 1]} Not found!");
						} else {
							waveSpawner.StartWave(data, 5f, false);
							Utilities.Message($"{textGroupId} : {textWaveId} {waveIndex + 1}");
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
						Debug.LogError($"Wave {waves[waveIndex]} Not found!");
					} else {
						waveSpawner.StartWave(data, 5f, false);
						Utilities.Message($"{textGroupId} : {textWaveId} {waveIndex + 1}");
					}
				}
			}
		}
		
	}
}