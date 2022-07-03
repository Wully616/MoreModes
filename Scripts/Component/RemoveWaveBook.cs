using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component {
	public class RemoveWaveBook : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if (IsEnabled()) {
				var components = Object.FindObjectsOfType<UIWaveSpawner>();
				foreach (var component in components) {
					//UI wave spawner is normally on a book.. we could disable the book or disable its parent mesh
					//Disabling the functionality is probably enough for now
					component.transform.gameObject.SetActive(false);
				}
			}

			yield break;
		}
	}
}