using System.Collections;
using Wully.MoreModes;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes.Component {
	public class RemoveWaveBook : LevelModule {
		public override IEnumerator OnLoadCoroutine() {

			var components = Object.FindObjectsOfType<UIWaveSpawner>();
			foreach (var component in components) {
				//UI wave spawner is normally on a book.. we could disable the book or disable its parent mesh
				//Disabling the functionality is probably enough for now
				component.transform.gameObject.SetActive(false);
			}
			

			yield break;
		}
	}
}