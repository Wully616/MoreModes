using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class InstantCasting : ModifierData {
		
		public static InstantCasting Instance;
        
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
			EnableInstantCast();
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			DisableInstantCast();
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if(eventTime == EventTime.OnEnd) return;
			DisableInstantCast();
		}
		
		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			EnableInstantCast();
		}
		
		private void DisableInstantCast()
		{
			if(!Player.currentCreature) return;
			Player.currentCreature.mana.chargeSpeedMultiplier = 1f;
		}
		
		private void EnableInstantCast()
		{
			if(!Player.currentCreature) return;
			//Todo:  this doesnt work for merges?
			Player.currentCreature.mana.chargeSpeedMultiplier = 1000f;
		}
		
	}
}