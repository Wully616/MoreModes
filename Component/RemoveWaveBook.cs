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
	public class RemoveWaveBook : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			var components = Object.FindObjectsOfType<UIWaveSpawner>();
			foreach ( var component in components ) {
				//UI wave spawner is normally on a book.. we could disable the book or disable its parent mesh
				//Disabling the functionality is probably enough for now
				component.transform.gameObject.SetActive(false);
			}
			return base.OnLoadCoroutine();
		}
	}
}