using System.Collections;
using Wully.MoreModes;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;

namespace Wully.MoreModes.Component
{
    public class RemoveHealthPotionsBook : LevelModule
    {
        public override IEnumerator OnLoadCoroutine()
        {
            Utilities.RemoveHealthPotionsFromBook();

            yield break;
        }
    }
}
