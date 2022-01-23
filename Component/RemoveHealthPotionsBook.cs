using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class RemoveHealthPotionsBook : LevelModule {
		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("disable_healthpotions_book", true)) {
				Utilities.RemoveHealthPotionsFromBook();
			}

			return base.OnLoadCoroutine();
		}
	}
}