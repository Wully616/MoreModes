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
				// This works for the no TK, it's still active but unusable
				SpellTelekinesis.showHighlighter = false;
				creature.handLeft.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handLeft.caster.telekinesis.radius = 0.0f;
				creature.handLeft.caster.telekinesis.maxAngle = 0.0f;

				creature.handRight.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handRight.caster.telekinesis.radius = 0.0f;
				creature.handRight.caster.telekinesis.maxAngle = 0.0f;
			}
		}


		public override void OnUnload() {
			if ( IsEnabled() ) {
				EventManager.onPossess -= EventManager_onPossess;
			}
		}
	}
}