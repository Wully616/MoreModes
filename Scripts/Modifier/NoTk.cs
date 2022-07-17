using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class NoTk : ModifierData {
		
		public static NoTk Instance;
        
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
			RemoveMagic();
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			AddMagic();
		}

		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			RemoveMagic();
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if (eventTime == EventTime.OnEnd) return;
			AddMagic(); // need to add it back to the creature when unpossessedd
		}
		
		private void AddMagic()
		{
			Player.currentCreature.container.AddContent("SpellTelekinesis");
		}
		
		private void RemoveMagic()
		{
			SpellCaster manaCasterLeft = Player.currentCreature.mana.casterLeft;
			SpellCaster manaCasterRight = Player.currentCreature.mana.casterRight;
			Player.currentCreature.container.RemoveContent("SpellTelekinesis");
			manaCasterLeft.telekinesis.Unload();
			manaCasterLeft.telekinesis = null;
			manaCasterRight.telekinesis.Unload();
			manaCasterRight.telekinesis = null;
		}
	}
}