using System.Collections;
using Wully.MoreModes.Data;
using GameModeLoader.Utils;
using ThunderRoad;
using Wully.Utils;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
	        creature.data.health = 1;
        }

    }
}
