using System.Collections;
using GameModeLoader.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModeLoader.GameMode
{
	public class BowArrowOnly : LevelModuleOptional
	{
		private List<ItemData.Type> allowedItemTypes = new List<ItemData.Type> { ItemData.Type.Quiver, ItemData.Type.Prop };
		private bool showHighlighterTK;
		public override IEnumerator OnLoadCoroutine()
		{
			//You must always call the following, so the IDs are setup for this LevelModuleOptional
			SetId();

			//Use IsEnabled around functionality so it only runs if this LevelModuleOptional is enabled
			if (IsEnabled())
			{
				Utilities.BowArrowOnlyItem();
				EventManager.onPossess += EventManager_onPossess;
                EventManager.onUnpossess += EventManager_onUnpossess;
				showHighlighterTK = SpellTelekinesis.showHighlighter;
			}
			yield break;
		}

        private void EventManager_onUnpossess(Creature creature, EventTime eventTime)
        {
			creature.handLeft.OnGrabEvent -= HandLeft_OnGrabEvent;
			creature.handRight.OnGrabEvent -= HandRight_OnGrabEvent;
		}

        private void EventManager_onPossess(Creature creature, EventTime eventTime)
        {
			if (eventTime == EventTime.OnStart)
			{
				Utilities.BowArrowOnlyUIItemSpawner();
				creature.handLeft.OnGrabEvent += HandLeft_OnGrabEvent;
				creature.handRight.OnGrabEvent += HandRight_OnGrabEvent;
				return;
			}
			if (eventTime == EventTime.OnEnd)
			{
				creature.handLeft.caster.allowCasting = false;
				creature.handLeft.caster.allowSpellWheel = false;
				// This works for the no TK, it's still active but unusable
				SpellTelekinesis.showHighlighter = false;
				creature.handLeft.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handLeft.caster.telekinesis.radius = 0.0f;
				creature.handLeft.caster.telekinesis.maxAngle = 0.0f;
				
				creature.handRight.caster.allowCasting = false;
				creature.handRight.caster.allowSpellWheel = false;
				creature.handRight.caster.telekinesis.maxCatchDistance = 0.0f;
				creature.handRight.caster.telekinesis.radius = 0.0f;
				creature.handRight.caster.telekinesis.maxAngle = 0.0f;
				return;
			}
		}
		private void HandRight_OnGrabEvent(Side side, Handle handle, float axisPosition, HandleOrientation orientation, EventTime eventTime)
		{
			UngrabUnallowedItems(handle, eventTime);
		}

		private void HandLeft_OnGrabEvent(Side side, Handle handle, float axisPosition, HandleOrientation orientation, EventTime eventTime)
		{
			UngrabUnallowedItems(handle, eventTime);
		}

		public override void OnUnload()
		{
			base.OnUnload();
			//Remember to unsubscribe to any events you might be listening to
			if (IsEnabled())
			{
				// Revert back to the original showHighlighter
				SpellTelekinesis.showHighlighter = showHighlighterTK;
			}
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
				if (!itemInHand.itemId.Contains("Arrow") && !itemInHand.itemId.Contains("Bow") && !itemInHand.itemId.Contains("Quiver") && !itemInHand.itemId.Contains("Lantern") && !allowedItemTypes.Contains(itemInHand.data.type))
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
