using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Wully.MoreModes;

namespace Wully.MoreModes.GameMode
{
	public class BowArrowOnly : LevelModule
	{
		private List<ItemData.Type> allowedItemTypes = new List<ItemData.Type> { ItemData.Type.Quiver, ItemData.Type.Prop };
		private bool showHighlighterTK;
		public override IEnumerator OnLoadCoroutine()
		{

			Utilities.BowArrowOnlyItem();
			EventManager.onPossess += EventManager_onPossess;
            EventManager.onUnpossess += EventManager_onUnpossess;
			showHighlighterTK = SpellTelekinesis.showHighlighter;
			
			yield break;
		}

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                creature.handLeft.OnGrabEvent -= OnGrabEvent;
                creature.handRight.OnGrabEvent -= OnGrabEvent;
            }
        }

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {

			if (eventTime == EventTime.OnEnd)
			{
                //Utilities.BowArrowOnlyUIItemSpawner(); //not sure how to undo this so disable for now
                creature.handLeft.OnGrabEvent += OnGrabEvent;
                creature.handRight.OnGrabEvent += OnGrabEvent;
 
				creature.handLeft.caster.allowCasting = false;
				creature.handLeft.caster.DisableSpellWheel(this);
				// This works for the no TK, it's still active but unusable
				SpellTelekinesis.showHighlighter = false;
				creature.handLeft.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handLeft.caster.telekinesis.radius = 0.0f;
				creature.handLeft.caster.telekinesis.maxAngle = 0.0f;
				
				creature.handRight.caster.allowCasting = false;
				creature.handRight.caster.DisableSpellWheel(this);
				creature.handRight.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handRight.caster.telekinesis.radius = 0.0f;
				creature.handRight.caster.telekinesis.maxAngle = 0.0f;
				return;
			}
		}
		private void OnGrabEvent(Side side, Handle handle, float axisPosition, HandlePose orientation, EventTime eventTime)
		{
			UngrabUnallowedItems(handle, eventTime);
		}


		public override void OnUnload()
		{
			base.OnUnload();
			//Remember to unsubscribe to any events you might be listening to

			// Revert back to the original showHighlighter
			SpellTelekinesis.showHighlighter = showHighlighterTK;
			
		}

		public void UngrabUnallowedItems(Handle handle, EventTime eventTime)
        {
			// get the item
			Item itemInHand = handle.item;
			// if if its  finished grabbing and handle isn't null and a ragdoll wasn't grabbed
			if (eventTime == EventTime.OnEnd && handle != null && !(handle is HandleRagdoll) && itemInHand)
			{
				//if the item isn't a bow/arrow/quiver
				//or the player didn't pick up an allowed type, Quiver / Prop
				if (!itemInHand.itemId.Contains("Arrow") && !itemInHand.itemId.Contains("Bow") && !itemInHand.itemId.Contains("Quiver") && !allowedItemTypes.Contains(itemInHand.data.type))
				{
					for (int i = itemInHand.handlers.Count() - 1; i >= 0; --i)
					{
						itemInHand.handlers[i].UnGrab(false);
					}
				}
			}
		}
	}
}
