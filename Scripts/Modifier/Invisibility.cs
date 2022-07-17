using System.Collections;
using ThunderRoad;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class Invisibility : ModifierData {
		//TODO: still to test
		public static Invisibility Instance;
		private float originalDistance;
		public override void Init()
		{
			if (Instance == null)
			{
				base.Init();
				Instance = this;
				local = this;
				originalDistance = Catalog.GetData<BrainData>("Player").GetModule<BrainModuleSightable>().sightDetectionMaxDistance;
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			Player.local.SetVisibilityDistance(0);
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			Player.local.SetVisibilityDistance(originalDistance);
		}

		protected override void OnPossess(Creature creature, EventTime eventTime)
		{
			base.OnPossess(creature, eventTime);
			if(eventTime == EventTime.OnStart) return;
			Player.local.SetVisibilityDistance(0);
		}

		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if (eventTime == EventTime.OnEnd) return;
			Player.local.SetVisibilityDistance(originalDistance);
		}
		
	}
}