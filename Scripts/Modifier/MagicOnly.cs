using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class MagicOnly : ModifierData {
		
		public static MagicOnly Instance;
		
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				// bit hacky, but we only one 1 modifier, if local isnt set, this modifier isnt Setup
				local = this;
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			Player.currentCreature.handLeft.OnGrabEvent += OnGrabEvent;
			Player.currentCreature.handRight.OnGrabEvent += OnGrabEvent;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			Player.currentCreature.handLeft.OnGrabEvent -= OnGrabEvent;
			Player.currentCreature.handRight.OnGrabEvent -= OnGrabEvent;
		}

		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			creature.handLeft.OnGrabEvent += OnGrabEvent;
			creature.handRight.OnGrabEvent += OnGrabEvent;
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if (eventTime == EventTime.OnEnd) return;
			Player.currentCreature.handLeft.OnGrabEvent -= OnGrabEvent;
			Player.currentCreature.handRight.OnGrabEvent -= OnGrabEvent;
		}
		
		private void OnGrabEvent(Side side, Handle handle, float axisPosition, HandlePose orientation, EventTime eventTime)
		{
			if(eventTime == EventTime.OnStart) return;
			Item item = handle.item;
			if (item != null)
			{
				// pick up staffs only and anything else that isnt a weapon
				if (!item.itemId.Contains("Staff") && item.data.type == ItemData.Type.Weapon)
				{
					for (int i = item.handlers.Count - 1; i >= 0; --i)
					{
						item.handlers[i].UnGrab(false);
					}
				}
			}
		}
	}
}
