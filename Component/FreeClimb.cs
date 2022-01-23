using System.Collections;
using ThunderRoad;
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
			if (Level.current.GetOptionAsBool("freeclimb", true)) {
				RagdollHandClimb.climbFree = false;
			}

			base.OnUnload();
		}
	}
}