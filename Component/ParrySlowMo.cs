using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace GameModeLoader.Component {
	public class ParrySlowMo : LevelModule {
		public int slowMoTime = 1;
		private SpellPowerSlowTime spellPowerSlowTime;
		public override IEnumerator OnLoadCoroutine() {
			if (Level.current.GetOptionAsBool("parryslowmo", true)) {
				EventManager.onCreatureParry += EventManager_onCreatureParry;
				spellPowerSlowTime = Catalog.GetData<SpellPowerSlowTime>("SlowTime");
			}

			yield return base.OnLoadCoroutine();
		}

		private void EventManager_onCreatureParry( Creature creature, CollisionInstance collisionInstance ) {
			if (Utilities.DidPlayerParry(collisionInstance)) {
				level.StartCoroutine(SlowMo());
			}
		}

		public override void OnUnload() {
			if (Level.current.GetOptionAsBool("parryslowmo", true)) {
				EventManager.onCreatureParry -= EventManager_onCreatureParry;
			}

			base.OnUnload();
		}

		private IEnumerator SlowMo() {
			GameManager.SetSlowMotion(true, spellPowerSlowTime.scale, spellPowerSlowTime.enterCurve, spellPowerSlowTime.effectData);
			yield return new WaitForSeconds(this.slowMoTime);
			if (GameManager.slowMotionState == GameManager.SlowMotionState.Running) {
				GameManager.SetSlowMotion(false, spellPowerSlowTime.scale, spellPowerSlowTime.exitCurve);
			}
		}
	}
}