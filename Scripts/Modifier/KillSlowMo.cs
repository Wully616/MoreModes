using System.Collections;
using GameModeLoader.Utils;
using ThunderRoad;
using UnityEngine;
using Wully.MoreModes;
using Wully.Utils;

namespace Wully.MoreModes {
	public class KillSlowMo : ModifierData {
		public int slowMoTime = 1;
        private Coroutine slowMoCoroutine;
        
        public static KillSlowMo Instance;
        
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
	        EventManager.onCreatureKill += OnCreatureKill;
        }
        
        protected override void OnDisable() {
	        base.OnDisable();
	        EventManager.onCreatureKill -= OnCreatureKill;
        }

		private void OnCreatureKill( Creature creature, Player player, CollisionInstance collisionInstance,
			EventTime eventTime ) {
            if ( eventTime == EventTime.OnStart ) return;
            if (player)
            {
                StopCoroutine();
				return;
            }

			if (!collisionInstance.IsDoneByPlayer() ) return;
			slowMoCoroutine = Level.current.StartCoroutine(Utilities.SlowMo(slowMoTime));
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