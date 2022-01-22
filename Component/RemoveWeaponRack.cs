using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace GameModeLoader.Component {
	public class RemoveWeaponRack : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			var weaponRacks = Object.FindObjectsOfType<WeaponRack>();
			foreach ( var weaponRack in weaponRacks ) {
				weaponRack.transform.gameObject.SetActive(false);
			}

			return base.OnLoadCoroutine();
		}
	}
}