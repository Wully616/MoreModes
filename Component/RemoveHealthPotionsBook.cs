using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component {
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