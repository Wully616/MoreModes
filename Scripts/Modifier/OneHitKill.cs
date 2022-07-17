using ThunderRoad;

namespace Wully.MoreModes
{
    public class OneHitKill : ModifierData
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
	        foreach (Creature creature in Creature.all) {
		        creature.currentHealth = 1;
		        creature.maxHealth = 1;
	        }
	        EventManager.onCreatureSpawn += OnCreatureSpawn;
        }

        protected override void OnDisable()
        {
	        base.OnDisable();
	        EventManager.onCreatureSpawn -= OnCreatureSpawn;
	        foreach (Creature creature in Creature.all) {
		        creature.maxHealth = creature.currentHealth = creature.data.health;
	        }
	        
        }
        
        private void OnCreatureSpawn(Creature creature)
        {
	        //TODO: works for player but not for all creatures?
	        //maybe need to do it on wave spawn
	        creature.currentHealth = 1;
	        creature.maxHealth = 1;
        }

    }
}
