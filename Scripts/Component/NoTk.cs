using System.Collections;
using System.Linq;
using Wully.MoreModes.Data;
using ThunderRoad;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class NoTk : LevelModuleOptional {
		private bool showHighlighterTK;
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				EventManager.onPossess += EventManager_onPossess;
				showHighlighterTK = SpellTelekinesis.showHighlighter;
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
				// Revert back to the original showHighlighter
				SpellTelekinesis.showHighlighter = showHighlighterTK;
			}
		}
	}
}