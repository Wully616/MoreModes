using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ThunderRoad;

namespace Wully.Patch {
	[HarmonyPatch(typeof(ThunderRoad.UISelectionListButtonsLevelModeOption), "GetValue")]
	public class UISelectionListButtonsLevelModeOptionGetValue {
		static bool Prefix( UISelectionListButtonsLevelModeOption __instance, ref int __result) {
			if (__instance.optionType == LevelData.Option.Type.Toggle) {
				__result = __instance.toggle.GetCurrentValue();
			} else {
				__result = __instance.GetCurrentValue();
			}
			return false; // skip original
		}
	}
}
