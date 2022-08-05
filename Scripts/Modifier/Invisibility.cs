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
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Creature creature = Creature.allActive[i];
				//make em forget about the player
				creature.brain.currentTarget = null; // wipe their current target so they search
				creature.spawnGroup = null; // remove their spawn group. Wave creatures dont lose their target.
			}
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
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Creature creature2 = Creature.allActive[i];
				//make em forget about the player
				creature2.brain.currentTarget = null;
				creature2.spawnGroup = null;
			}
		}
		protected override void OnUnPossess(Creature creature, EventTime eventTime)
		{
			base.OnUnPossess(creature, eventTime);
			if (eventTime == EventTime.OnEnd) return;
			Player.local.SetVisibilityDistance(originalDistance);
		}

		public override void Update()
		{
			base.Update();
			MakeEmForget();
		}
				
		private void MakeEmForget()
		{
			int allActiveCount = Creature.allActive.Count;
			for (var i = 0; i < allActiveCount; i++)
			{
				Creature creature = Creature.allActive[i];
				//make em forget about the player
				creature.spawnGroup = null; 
				// need to keep doing this every update because new npcs are spawned from the wavespawner with this set
				// and theres no spawn event for the wavespawner
			}
		}

	}
}