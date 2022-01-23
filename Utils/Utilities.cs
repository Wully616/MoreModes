using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameModeLoader.Utils {
	public class Utilities {

		public static IEnumerator SlowMo( float slowMoTime ) {
			var spellPowerSlowTime = Catalog.GetData<SpellPowerSlowTime>("SlowTime");
			yield return SlowMotionCoroutine(true, spellPowerSlowTime.scale, spellPowerSlowTime.enterCurve);
			yield return new WaitForSeconds(slowMoTime);
			yield return SlowMotionCoroutine(false, spellPowerSlowTime.scale, spellPowerSlowTime.exitCurve);
		}

		public static IEnumerator SlowMotionCoroutine(
			bool active,
			float scale,
			AnimationCurve curve,
			EffectData effectData = null ) {
			GameManager gameManager = GameManager.local;
			float slowMotionTime = Time.time;
			if ( active ) {
				GameManager.slowMotionState = GameManager.SlowMotionState.Starting;
				if ( effectData != null ) {
					GameManager.slowTimeEffectInstance = effectData.Spawn(gameManager.transform, true, (System.Type[])Array.Empty<System.Type>());
					GameManager.slowTimeEffectInstance.Play();
				}
				while ( (double)Time.timeScale > (double)scale ) {
					GameManager.SetTimeScale(Mathf.Lerp(1f, scale, curve.Evaluate(Time.time - slowMotionTime)));
					yield return (object)null;
				}
				GameManager.SetTimeScale(scale);
				GameManager.slowMotionState = GameManager.SlowMotionState.Running;
			} else {
				if ( GameManager.slowTimeEffectInstance != null )
					GameManager.slowTimeEffectInstance.End();
				GameManager.slowMotionState = GameManager.SlowMotionState.Stopping;
				while ( (double)Time.timeScale < 1.0 ) {
					GameManager.SetTimeScale(Mathf.Lerp(scale, 1f, curve.Evaluate(Time.time - slowMotionTime)));
					yield return (object)null;
				}
				GameManager.SetTimeScale(1f);
				GameManager.slowMotionState = GameManager.SlowMotionState.Disabled;
			}
		}

		public static bool DidPlayerParry(CollisionInstance collisionInstance) {
			if (collisionInstance.sourceColliderGroup?.collisionHandler.item?.mainHandler?.creature.player &&
			    collisionInstance.sourceColliderGroup.collisionHandler.item.data.type == ItemData.Type.Weapon)
				return true;
			if ( !collisionInstance.targetColliderGroup?.collisionHandler.item?.mainHandler?.creature.player || collisionInstance.targetColliderGroup.collisionHandler.item.data.type != ItemData.Type.Weapon )
				return false;
			return true;
		}
		public static void RemoveHealthPotionsFromBook() {
			foreach (UIItemSpawner uiItemSpawner in Object.FindObjectsOfType<UIItemSpawner>()) {
				for (int index = uiItemSpawner.container.contents.Count - 1; index >= 0; --index) {
					var content = uiItemSpawner.container.contents[index];
					if (content.itemData.type == ItemData.Type.Potion &&
					    content.itemData.id.Equals("PotionHealth")) {
						uiItemSpawner.container.contents.RemoveAt(index);
					}
				}

				uiItemSpawner.RefreshCategories();
				uiItemSpawner.RefreshItems(uiItemSpawner.container);
			}
		}

		public static void RemoveHealthPotionsFromPlayerInventory() {
			//enable by default
			for (int index = Player.characterData.inventory.Count - 1; index >= 0; --index) {
				var content = Player.characterData.inventory[index];
				if (content.itemData.type == ItemData.Type.Potion && content.itemData.id.Equals("PotionHealth")) {
					Player.characterData.inventory.RemoveAt(index);
				}
			}
		}

		public static void RemoveActiveHealthPotions() {
			for (int index = Item.allActive.Count - 1; index >= 0; --index) {
				var item = Item.allActive[index];
				if (item.itemId.Equals("PotionHealth")) {
					item.Despawn();
				}
			}
		}
	}
}