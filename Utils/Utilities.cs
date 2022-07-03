using System;
using System.Collections;
using System.Collections.Generic;
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
					GameManager.slowTimeEffectInstance = effectData.Spawn(gameManager.transform, true);
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

		public static void AddSpellContentsToPlayer( ContainerData.Content content) {
			Player.characterData.inventory.Add(content);
		}

		/// <summary>
		/// Should be called before the player loads at all, so ideally on level load.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Returns the removed contents, so they may be added back later</returns>
		public static List<ContainerData.Content> RemoveSpellFromPlayer(string id) {
			List<ContainerData.Content> removedContents = new List<ContainerData.Content>();
			for ( int i = Player.characterData.inventory.Count - 1; i >= 0; i-- ) {
				var content = Player.characterData.inventory[i];
				if ( content.itemData.type == ItemData.Type.Spell && content.itemData.id.Equals(id,StringComparison.OrdinalIgnoreCase) ) {
					removedContents.Add(content);
					Player.characterData.inventory.RemoveAt(i);
				}
			}

			return removedContents;
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

		public static void RemoveItemTypeFromBook(ItemData.Type type) {
			foreach ( UIItemSpawner uiItemSpawner in Object.FindObjectsOfType<UIItemSpawner>() ) {
				for ( int index = uiItemSpawner.container.contents.Count - 1; index >= 0; --index ) {
					var content = uiItemSpawner.container.contents[index];
					if ( content.itemData.type == type ) {
						uiItemSpawner.container.contents.RemoveAt(index);
					}
				}
				uiItemSpawner.RefreshCategories();
				uiItemSpawner.RefreshItems(uiItemSpawner.container);
			}
		}

		public static void RemoveAllExceptItemTypeFromPlayerInventory( ItemData.Type type ) {
			for ( int index = Player.characterData.inventory.Count - 1; index >= 0; --index ) {
				var content = Player.characterData.inventory[index];
				if ( content.itemData.type != type ) {
					Player.characterData.inventory.RemoveAt(index);
				}
			}
		}

		public static void RemoveItemTypeFromPlayerInventory(ItemData.Type type) {
			for ( int index = Player.characterData.inventory.Count - 1; index >= 0; --index ) {
				var content = Player.characterData.inventory[index];
				if ( content.itemData.type == type ) {
					Player.characterData.inventory.RemoveAt(index);
				}
			}
		}
		public static void RemoveHealthPotionsFromPlayerInventory() {

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
		public static void BowArrowOnlyItem()
		{
			for (int index = Item.allActive.Count - 1; index >= 0; --index)
			{
				var item = Item.allActive[index];
				if (!item.itemId.Contains("Arrow")
					&& !item.itemId.Contains("Bow")
					&& !item.itemId.Contains("Quiver")
					&& !(bool)item.holder
					&& (!item.isTelekinesisGrabbed
					&& !item.isThrowed)
					&& (!item.IsHanded()
					&& !item.isGripped)
					&& (double)item.spawnTime != 0.0)
				{
					item.Despawn();
				}
			}
			for (int index = Player.characterData.inventory.Count - 1; index >= 0; --index)
			{
				var content = Player.characterData.inventory[index];
				if (!content.itemData.id.Contains("Arrow")
					&& !content.itemData.id.Contains("Bow")
					&& !content.itemData.id.Contains("Quiver")
					&& content.itemData.type != ItemData.Type.Wardrobe
					&& !content.itemData.id.Contains("SpellTelekinesis"))
				{
					Player.characterData.inventory.RemoveAt(index);
				}
			}
		}
		public static void BowArrowOnlyUIItemSpawner()
		{
			foreach (UIItemSpawner uiItemSpawner in Object.FindObjectsOfType<UIItemSpawner>())
			{
				for (int index = uiItemSpawner.container.contents.Count - 1; index >= 0; --index)
				{
					var content = uiItemSpawner.container.contents[index];
					if (!content.itemData.id.Contains("Arrow") && !content.itemData.id.Contains("Bow") && !content.itemData.id.Contains("Quiver"))
					{
						uiItemSpawner.container.contents.RemoveAt(index);
					}
				}
				uiItemSpawner.RefreshCategories();
				uiItemSpawner.RefreshItems(uiItemSpawner.container);
			}
		}
    }
}