using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Utils {
	public class Utilities {
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