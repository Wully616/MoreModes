using ThunderRoad;

namespace Wully.MoreModes
{
    public class OneHitKill : ModifierData
    {
        public static OneHitKill Instance;
        
        public override void Init()
        {
	        if (Instance != null) return;
	        base.Init();
	        local = Instance = this;
        }
        
        protected override void OnEnable()
        {
	        base.OnEnable();
	        EventManager.onCreatureHit += OnCreatureHit;
        }
        protected override void OnDisable()
        {
	        base.OnEnable();
	        EventManager.onCreatureHit -= OnCreatureHit;
        }
        
        private void OnCreatureHit(Creature creature, CollisionInstance collisioninstance)
        {
	        creature.Kill(collisioninstance);
        }
    }
}
