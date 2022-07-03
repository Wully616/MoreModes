using System.Collections;
using GameModeLoader.Data;
using ThunderRoad;
using UnityEngine;

namespace GameModeLoader.Component {
	public class RemoveWeaponRack : LevelModuleOptional {
		public override IEnumerator OnLoadCoroutine() {
			SetId();
			if (IsEnabled()) {
                foreach (Level.CustomReference customReference in level.customReferences)
                {
                    if (customReference.name == "Rack")
                    {
                        foreach (Transform transform in customReference.transforms)
                        {
                            transform.gameObject.SetActive(false);
                        }
                    }
                }
            }

			yield break;
		}
	}
}