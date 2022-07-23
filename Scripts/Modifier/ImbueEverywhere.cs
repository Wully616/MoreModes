using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class ImbueEverywhere : ModifierData {
		
		public static ImbueEverywhere Instance;
		private bool enabled;
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
			enabled = true;
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			enabled = false;
		}

		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			enabled = true;
		}
		
		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if(eventTime == EventTime.OnEnd) return;
			enabled = false;
		}
		
		

		public override void Update()
		{
			if (!enabled) return;
			if (!Player.local) return;
			SpellCaster caster;
			if (IsChargingSpell(Side.Left, out caster))
			{
				MassImbue(caster);
			}
			if (IsChargingSpell(Side.Right, out caster))
			{
				MassImbue(caster);
			}
		}

		private void MassImbue(SpellCaster caster)
		{
			var spellCastCharge = (SpellCastCharge)caster.spellInstance;
			int allActiveCount = Item.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Item item = Item.allActive[i];
				int imbuesCount = item.imbues.Count;
				for (var index = 0; index < imbuesCount; index++)
				{
					var imbue = item.imbues[index];
					if (imbue.spellCastBase != null && imbue.spellCastBase.hashId != spellCastCharge.hashId)
					{
						imbue.UnloadCurrentSpell();
					}
					imbue.Transfer(spellCastCharge, ((spellCastCharge.imbueRate * spellCastCharge.currentCharge)) * Time.deltaTime);
				}
			}
		}
		
		private bool IsChargingSpell(Side side, out SpellCaster caster)
		{
			caster = Player.local.creature.mana.casterLeft;
			if (side == Side.Right) caster = Player.local.creature.mana.casterRight;
			return caster.isFiring && !caster.isMerging && !caster.isSpraying && caster.spellInstance is SpellCastCharge castCharge;

		}
	}
}