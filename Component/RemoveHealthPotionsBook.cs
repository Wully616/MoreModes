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
	public class RemoveHealthPotionsBook : LevelModule {

		public override IEnumerator OnLoadCoroutine() {

			if ( Level.current.GetOptionAsBool("disable_healthpotions_book", true) ) {
				Utilities.RemoveHealthPotionsFromBook();
			}

			return base.OnLoadCoroutine();
		}

	}
}