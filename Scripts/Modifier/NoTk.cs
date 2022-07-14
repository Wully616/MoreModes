using System.Collections;
using ThunderRoad;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	public class NoTk : Modifier {
		
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
			AddMagic(); // need to add it back to the creature when unpossed
		}
		
		private void AddMagic()
		{
			Player.currentCreature.container.AddContent("SpellTelekinesis");
		}
		
		private void RemoveMagic()
		{
			Player.currentCreature.container.RemoveContent("SpellTelekinesis");
		}
	}
}