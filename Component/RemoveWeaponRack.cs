using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component {
	public class RemoveWeaponRack : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if (IsEnabled()) {
				var weaponRacks = Object.FindObjectsOfType<WeaponRack>();
				foreach (var weaponRack in weaponRacks) {
					weaponRack.transform.gameObject.SetActive(false);
				}
			}

			yield break;
		}
	}
}