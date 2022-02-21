using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModeLoader.Component
{
    public class OneHPLeft : LevelModuleOptional
    {
		private short healthOri;
		public override IEnumerator OnLoadCoroutine()
		{
			//You must always call the following, so the IDs are setup for this LevelModuleOptional
			SetId();
            if (IsEnabled())
            {
                EventManager.onPossess += EventManager_onPossess;
                EventManager.onUnpossess += EventManager_onUnpossess;
            }
            yield break;
		}

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
			if (eventTime == EventTime.OnStart)
			{
				Player.local.creature.data.health = healthOri;
			}
		}

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
		{
			if (eventTime == EventTime.OnEnd)
			{
				healthOri = Player.local.creature.data.health;
				Player.local.creature.data.health = 1;
				Player.local.creature.maxHealth = 1f;
				Player.local.creature.currentHealth = 1f;
				return;
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
