using System.Collections;
using System.Linq;
using GameModeLoader.Data;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class NoTk : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onPossess += EventManager_onPossess;
			}

			yield break;
		}

		private void EventManager_onPossess(Creature creature, EventTime eventTime) {
			if (eventTime == EventTime.OnStart) {
				return;
			}

			if ( IsEnabled() ) {
				creature.handLeft.caster.telekinesis.Unload();
				creature.handLeft.caster.telekinesis = null;
				creature.handRight.caster.telekinesis.Unload();
				creature.handRight.caster.telekinesis = null;
			}
		}


		public override void OnUnload() {
			if ( IsEnabled() ) {
				EventManager.onPossess -= EventManager_onPossess;
			}
		}
	}
}