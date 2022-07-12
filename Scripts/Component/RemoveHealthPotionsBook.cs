using System.Collections;
using Wully.MoreModes.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;

namespace Wully.MoreModes.Component {
	public class RemoveHealthPotionsBook : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if ( IsEnabled() ) {
				Utilities.RemoveHealthPotionsFromBook();
			}

			yield break;
		}
	}
}