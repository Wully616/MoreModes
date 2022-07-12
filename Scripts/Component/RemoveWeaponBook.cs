using System.Collections;
using Wully.MoreModes.Data;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes.Component {
	public class RemoveWeaponBook : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if (IsEnabled()) {
				var components = Object.FindObjectsOfType<UIItemSpawner>();
				foreach (var component in components) {
					//UI item spawner is normally on a book.. we could disable the book or disable its parent mesh
					//Disabling the functionality is probably enough for now
					component.transform.gameObject.SetActive(false);
				}
			}

			yield break;
		}
	}
}