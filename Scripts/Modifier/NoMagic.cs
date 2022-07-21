using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class NoMagic : ModifierData {
		
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
			Player.currentCreature.container.AddContent("SpellSlowTime");
			Player.currentCreature.container.AddContent("SpellTelekinesis");
			Player.currentCreature.handLeft.caster.AllowCasting(this);
			Player.currentCreature.handLeft.caster.AllowSpellWheel(this);
			Player.currentCreature.handRight.caster.AllowCasting(this);
			Player.currentCreature.handRight.caster.AllowSpellWheel(this);
		}
		
		private void RemoveMagic()
		{

			SpellCaster manaCasterLeft = Player.currentCreature.mana.casterLeft;
			SpellCaster manaCasterRight = Player.currentCreature.mana.casterRight;
			
			manaCasterLeft.UnloadSpell();
			manaCasterRight.UnloadSpell();
			
			manaCasterLeft.DisallowCasting(this);
			manaCasterLeft.DisableSpellWheel(this);
			manaCasterRight.DisallowCasting(this);
			manaCasterRight.DisableSpellWheel(this);
			
			
			//There is a bug in the base game for removing SpellPowerInstances and Telekinesis so we need to do those manually
			Player.currentCreature.container.RemoveContent("SpellSlowTime");
			for (int i = Player.currentCreature.mana.spellPowerInstances.Count - 1; i >= 0; i--)
			{
				if (Player.currentCreature.mana.spellPowerInstances[i].id == "SpellSlowTime")
				{
					Player.currentCreature.mana.spellPowerInstances[i].Unload();
					Player.currentCreature.mana.spellPowerInstances.RemoveAt(i);
				}
			}
			Player.currentCreature.container.RemoveContent("SpellTelekinesis");
			manaCasterLeft.telekinesis.Unload();
			manaCasterLeft.telekinesis = null;
			manaCasterRight.telekinesis.Unload();
			manaCasterRight.telekinesis = null;
			
			
		}
	}
}
