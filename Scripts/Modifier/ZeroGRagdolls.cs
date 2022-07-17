using ThunderRoad;

namespace Wully.MoreModes {
	public class ZeroGRagdolls : ModifierData {
		
		public static ZeroGRagdolls Instance;
		
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
			EventManager.onCreatureKill += OnCreatureKill;
			foreach (Creature creature in Creature.allActive) {
				if(creature.isKilled){
					creature.ragdoll?.SetPhysicModifier(this, 0);
				};
			}
		}
		
		private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisioninstance, EventTime eventTime)
		{
			if(eventTime == EventTime.OnStart) return;
			creature.ragdoll.SetPhysicModifier(this, 0);
		}
		
		protected override void OnDisable() {
			base.OnDisable();
			EventManager.onCreatureKill -= OnCreatureKill;
			foreach (Creature creature in Creature.allActive) {
				creature.ragdoll?.RemovePhysicModifier(this);
			}
		}
		
	}
}