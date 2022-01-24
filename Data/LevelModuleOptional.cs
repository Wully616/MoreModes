using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using GameModeLoader.Module;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Data {
	[Serializable]
	public class LevelModuleOptional : LevelModule {
		public string id;
		public bool enable;

		public virtual IEnumerator OnLoadCoroutine() {
			SetId();
			yield return base.OnLoadCoroutine();
		}

		public virtual void SetId() {
			//get the id of this LevelModuleOptionals Option data.
			var options = Module.GameModeLoader.GetLevelOptionList();
			foreach (var option in options) {
				if (option.levelOption.levelModuleOptional.GetType() == this.GetType()) {
					this.id = option.levelOption.name;
					break;
				}
			}
		}

		public virtual bool IsEnabled() {
			//the enable bool is like the master switch, so it can be forcefully enabled for gamemodes
			//the option check is to check if it should be enabled or not on a per map/gamemode basis
			return enable || Level.current.GetOptionAsBool(id);
		}
	}
}