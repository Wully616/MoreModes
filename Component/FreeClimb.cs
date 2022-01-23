using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using GameModeLoader.Module;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using GameModeLoader.Utils;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class FreeClimb : LevelModule {
		
		public override IEnumerator OnLoadCoroutine() {

			if (Level.current.GetOptionAsBool("freeclimb", true)) {
				RagdollHandClimb.climbFree = true;
			}

			yield return base.OnLoadCoroutine();
		}

		public override void OnUnload() {
			if ( Level.current.GetOptionAsBool("freeclimb", true) ) {
				RagdollHandClimb.climbFree = false;
			}
			base.OnUnload();
		}
	}
}