using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class ImbueEverywhere : ModifierData {
		
		public static ImbueEverywhere Instance;
        
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
			EnableImbueEverywhere();
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			DisableImbueEverywhere();
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if(eventTime == EventTime.OnEnd) return;
			DisableImbueEverywhere();
		}
		
		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			EnableImbueEverywhere();
		}
		
		private void DisableImbueEverywhere()
		{
			if(!Player.currentCreature) return;
			var spellsCount = Player.currentCreature.mana.spells.Count;
			for (var i = 0; i < spellsCount; i++)
			{
				SpellData spellData = Player.currentCreature.mana.spells[i];
				if (spellData is SpellCastCharge { imbueEnabled: true } spellCastCharge)
				{
					var originalSpellData = Catalog.GetData<SpellCastCharge>(spellData.id);
					spellCastCharge.imbueRadius = originalSpellData.imbueRadius;
				}
			}
		}
		
		private void EnableImbueEverywhere()
		{
			if(!Player.currentCreature) return;
			var spellsCount = Player.currentCreature.mana.spells.Count;
			for (var i = 0; i < spellsCount; i++)
			{
				SpellData spellData = Player.currentCreature.mana.spells[i];
				if (spellData is SpellCastCharge  { imbueEnabled: true } spellCastCharge)
				{
					//this doesnt work because the spell data is cloned, the original catalog needs to be edited
					spellCastCharge.imbueRadius = 1000;
				}
			}
		}
	}
}