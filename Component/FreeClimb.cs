using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class FreeClimb : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				RagdollHandClimb.climbFree = true;
			}

			yield break;
		}

		public override void OnUnload() {
			if ( IsEnabled() ) {
				RagdollHandClimb.climbFree = false;
			}

			base.OnUnload();
		}
	}
}