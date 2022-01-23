using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component {
	public class RemoveWeaponRack : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			var weaponRacks = Object.FindObjectsOfType<WeaponRack>();
			foreach (var weaponRack in weaponRacks) {
				weaponRack.transform.gameObject.SetActive(false);
			}

			return base.OnLoadCoroutine();
		}
	}
}