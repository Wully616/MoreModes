using HarmonyLib;
using ThunderRoad;

namespace Wully.Patch {
	[HarmonyPatch(typeof(UISelectionListButtonsLevelModeOption), "GetValue")]
	public class UISelectionListButtonsLevelModeOptionGetValue {
		private static bool Prefix(UISelectionListButtonsLevelModeOption __instance, ref int __result) {
			if (__instance.optionType == LevelData.Option.Type.Toggle) {
				__result = __instance.toggle.GetCurrentValue();
			} else {
				__result = __instance.GetCurrentValue();
			}

			return false; // skip original
		}
	}
}