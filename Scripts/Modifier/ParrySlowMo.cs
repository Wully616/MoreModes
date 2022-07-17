using System.Collections;
using Wully.MoreModes;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.Utils;

namespace Wully.MoreModes {
	public class ParrySlowMo : ModifierData {
		public int slowMoTime = 1;
        private Coroutine slowMoCoroutine;
        
        public static ParrySlowMo Instance;
        
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
	        EventManager.onCreatureParry += OnCreatureParry;
	        EventManager.onCreatureKill += OnCreatureKill;
        }
        
        protected override void OnDisable() {
	        base.OnDisable();
	        EventManager.onCreatureParry -= OnCreatureParry;
	        EventManager.onCreatureKill -= OnCreatureKill;
        }

		private void OnCreatureParry(Creature creature, CollisionInstance collisionInstance) {
			if (Utilities.DidPlayerParry(collisionInstance)) {
				Level.current.StartCoroutine(Utilities.SlowMo(slowMoTime));
			}
		}
		
        private void OnCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
            EventTime eventTime )
        {
            if ( eventTime == EventTime.OnStart ) return;
            //We want to stop the slow mo effect if the player dies
            if ( player )
            {
                StopCoroutine();
                return;
            }
        }

		private void StopCoroutine()
        {
            if ( slowMoCoroutine != null )
            {
	            Level.current.StopCoroutine(slowMoCoroutine);
            }
        }

	}
}