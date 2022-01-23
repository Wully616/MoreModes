using System.Collections;
using System.Linq;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class NoSpells : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("no_spells", true)) {
				EventManager.onPossess += EventManager_onPossess;
			}

			return base.OnLoadCoroutine();
		}

		private void EventManager_onPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnStart) {
				return;
			}

			if (Level.current.GetOptionAsBool("no_spells", true)) {
				creature.handLeft.caster.allowCasting = false;
				creature.handLeft.caster.allowSpellWheel = false;
				creature.handLeft.caster.telekinesis.Unload();
				creature.handLeft.caster.telekinesis = null;
				creature.handRight.caster.allowCasting = false;
				creature.handRight.caster.allowSpellWheel = false;
				creature.handRight.caster.telekinesis.Unload();
				creature.handRight.caster.telekinesis = null;
			}
		}

		private void Level_OnLevelEvent() {
			if (Level.current.GetOptionAsBool("no_spells", true)) {
				//Unload LevelModuleDeath
				var levelModuleDeath = level?.mode?.modules?.First(d => d.type == typeof(LevelModuleDeath));
				if (levelModuleDeath != null) {
					//call unload
					levelModuleDeath.OnUnload();
					//remove
					for (int i = level.mode.modules.Count - 1; i >= 0; i--) {
						if (level.mode.modules[i] == levelModuleDeath) {
							level.mode.modules.RemoveAt(i);
						}
					}
				}
			}
		}


		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("no_spells", true)) {
				level.OnLevelEvent -= Level_OnLevelEvent;
			}
		}
	}
}