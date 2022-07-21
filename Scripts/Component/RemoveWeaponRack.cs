using System.Collections;
using Wully.MoreModes;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreModes.Component {
	public class RemoveWeaponRack : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			/*SetId();
			if (IsEnabled()) {
				var weaponRacks = Object.FindObjectsOfType<WeaponRack>();
				foreach (var weaponRack in weaponRacks) {
					weaponRack.transform.gameObject.SetActive(false);
				}
			}
			*/
			yield break;
		}
	}
}