using ThunderRoad;

namespace Wully.MoreModes
{
    public class OneHitKill : Modifier
    {
        public static OneHitKill Instance;
        
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
	        EventManager.onCreatureSpawn += OnCreatureSpawn;
        }

        protected override void OnDisable()
        {
	        base.OnDisable();
	        EventManager.onCreatureSpawn -= OnCreatureSpawn;
        }
        
        private void OnCreatureSpawn(Creature creature)
        {
	        creature.currentHealth = 1;
	        creature.maxHealth = 1;
        }

    }
}
