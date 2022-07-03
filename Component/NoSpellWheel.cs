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
                creature.handLeft.caster.AllowSpellWheel(this);

                creature.handRight.caster.allowCasting = true;
                creature.handRight.caster.AllowSpellWheel(this);
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
                creature.handLeft.caster.DisableSpellWheel(this);

                creature.handRight.caster.allowCasting = false;
                creature.handRight.caster.DisableSpellWheel(this);
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
