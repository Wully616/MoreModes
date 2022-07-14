using System.Collections;
using ThunderRoad;
using Wully.MoreModes.Data;
using Wully.Utils;

namespace Wully.MoreModes {
	public class NoMagic : Modifier {
		
		public static NoMagic Instance;
        
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
			Player.local.creature.container.AddContent("SpellSlowTime");
			Player.currentCreature.handLeft.caster.allowCasting = true;
			Player.currentCreature.handLeft.caster.AllowSpellWheel(this);
			Player.currentCreature.handRight.caster.allowCasting = true;
			Player.currentCreature.handRight.caster.AllowSpellWheel(this);
		}
		
		private void RemoveMagic()
		{
			Player.currentCreature.container.RemoveContent("SpellSlowTime");
			Player.currentCreature.handLeft.caster.allowCasting = true;
			Player.currentCreature.handLeft.caster.AllowSpellWheel(this);
			Player.currentCreature.handRight.caster.allowCasting = true;
			Player.currentCreature.handRight.caster.AllowSpellWheel(this);
		}
	}
}