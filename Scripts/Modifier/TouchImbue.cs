using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class TouchImbue : ModifierData {
		
		public static TouchImbue Instance;
        
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
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
				Imbue(side, item);	
			}
		}

		private void Imbue(Side side, Item item)
		{
			var hand = Player.currentCreature.mana.casterLeft;
			if (side == Side.Right) hand = Player.currentCreature.mana.casterRight;
			//get the spell
			if (hand.spellInstance != null && hand.spellInstance is SpellCastCharge spellCastCharge)
			{
				foreach (var imbue in item.imbues)
				{
					if (imbue.spellCastBase != null && imbue.spellCastBase.hashId != spellCastCharge.hashId)
					{
						imbue.UnloadCurrentSpell();
					} 
					imbue.Transfer(spellCastCharge, float.MaxValue);
					
				}
				
			}
		}
	}
}