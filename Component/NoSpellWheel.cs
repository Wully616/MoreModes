using System.Collections;
using System.Linq;
using GameModeLoader.Data;
using ThunderRoad;
using Wully.Utils;

namespace GameModeLoader.Component
{
    public class NoSpellWheel : LevelModuleOptional
    {
        public override IEnumerator OnLoadCoroutine()
        {
            SetId();
            if (IsEnabled())
            {
                EventManager.onPossess += EventManager_onPossess;
                EventManager.onUnpossess += EventManager_onUnpossess;
                ;
            }

            yield break;
        }

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                return;
            }

            if (IsEnabled())
            {
                creature.handLeft.caster.allowCasting = true;
                creature.handLeft.caster.allowSpellWheel = true;

                creature.handRight.caster.allowCasting = true;
                creature.handRight.caster.allowSpellWheel = true;
            }
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                return;
            }

            if (IsEnabled())
            {
                creature.handLeft.caster.allowCasting = false;
                creature.handLeft.caster.allowSpellWheel = false;

                creature.handRight.caster.allowCasting = false;
                creature.handRight.caster.allowSpellWheel = false;
            }
        }

        public override void OnUnload()
        {
            if (IsEnabled())
            {
                EventManager.onPossess -= EventManager_onPossess;
                EventManager.onUnpossess -= EventManager_onUnpossess;
            }
        }
    }
}
